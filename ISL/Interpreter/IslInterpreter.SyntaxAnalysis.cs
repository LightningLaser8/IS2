using ISL.Language.Expressions;
using ISL.Language.Expressions.Combined;
using ISL.Language.Keywords;
using ISL.Runtime.Errors;

namespace ISL.Interpreter
{
    internal partial class IslInterpreter
    {
        //Syntax analysis, builds expression (syntax) trees from tokenised code
        private void Parse()
        {
            expressions.Clear();

            expressions.AddRange(tokens.Select((x) =>
            {
                return Expression.From(x, this);
            }));
            Debug("Bracketise:");
            //Put shit in brackets
            Bracketise(expressions);

            Treeify(expressions);
            Debug(" Validate Expression Syntax:");
            ValidateCodeBlock(expressions);
            expressions.ForEach(ex => { Debug($"  validating {ex}"); ex.Validate(); });
            Debug("Syntax Analysis / Code Gen Finished!");
        }
        internal static void ValidateCodeBlockStatic(List<Expression> expressions)
        {
            bool wasSemi = true;
            for (int i = 0; i < expressions.Count; i++)
            {
                Expression ex = expressions[i];
                if (wasSemi && ex is TokenExpression te && te.value == ";") throw new SyntaxError("Unexpected ;");
                if (!wasSemi && ex is not TokenExpression) throw new SyntaxError("Expected ;, got expression");
                wasSemi = (ex is TokenExpression t2e && t2e.value == ";");
                if (wasSemi)
                {
                    expressions.RemoveAt(i);
                    i--;
                }
            }
        }
        internal void ValidateCodeBlock(List<Expression> expressions)
        {
            bool wasSemi = true;
            for (int i = 0; i < expressions.Count; i++)
            {
                Expression ex = expressions[i];
                Debug("  token or expression " + ex.ToString() + (wasSemi ? ", wants expression here" : ", wants semicolon here"));
                if (wasSemi && ex is TokenExpression te && te.value == ";") throw new SyntaxError("Unexpected ;");
                if (!wasSemi && ex is not TokenExpression) throw new SyntaxError("Expected ;, got expression");
                wasSemi = (ex is TokenExpression t2e && t2e.value == ";");
                if (wasSemi)
                {
                    expressions.RemoveAt(i);
                    i--;
                }
            }
        }

        private void Treeify(List<Expression> expressions)
        {
            Debug("Operator Precedence:");

            //Get precedence
            List<int> precedences = [];
            foreach (var expr in expressions)
            {
                if (expr is OperatorExpression oe)
                {
                    var op = oe.GetOp();
                    if (op is not null)
                    {
                        if (!precedences.Contains(op.Precedence)) precedences.Add(op.Precedence);
                    }
                }
            }
            precedences.Sort();
            precedences.Reverse();
            if (precedences.Count == 0)
            {
                Debug("  No operators present.");
                Debug("  No tree creation required.");
            }
            else
            {
                Debug($"  Precedences: [{string.Join(", ", precedences)}");
                Debug("Create Expression Tree:");
                foreach (int precedence in precedences)
                    CreateTreeLevel(precedence, expressions);
                Debug("  Tree Created.");
            }
            MakeKeywordsGrabExpressions(expressions);
            MakeKeywordsBackReference(expressions);
        }

        private void Bracketise(List<Expression> expressions, int level = 0)
        {
            for (int i = 0; i < expressions.Count; i++)
            {
                var expr = expressions[i];
                if (expr is null) break; //If out of bounds (i.e. it's done)

                if (expr is BracketExpression cexp)
                {
                    Debug($"  Found {cexp.bracket.Open} at {i}, looking for {cexp.bracket.Close}");
                    int end = FindClosingBracket(i + 1, level, cexp.bracket.Open, cexp.bracket.Close, expressions);
                    if (end == -1) throw new SyntaxError("Expected closing " + cexp.bracket.Close);
                    Debug($"  Found closing {cexp.bracket.Close} at {end}");
                    expressions[end] = Expression.Null;
                    var captured = expressions[(i + 1)..end];
                    Bracketise(captured, level);
                    Debug("Treeifying captured expressions:");
                    Treeify(captured);
                    var packaged = cexp.bracket.Create.Invoke(captured);
                    Debug("Removing expressions:");
                    expressions[i] = packaged;
                    if (expressions.Count > 0) expressions.RemoveRange(i + 1, end - i);
                    Debug($"  Brackets enclose {packaged}");
                }
            }
        }
        private void CreateTreeLevel(int precedence, List<Expression> expressions)
        {
            Debug($" Precedence level {precedence}:");
            int currentIndex = -1;
            while (true)
            {
                Debug($"  index from {(currentIndex == -1 ? "start" : currentIndex)}");
                currentIndex = FindNextNonNullExpression(currentIndex, EvaluationDirection.Right, expressions);
                if (currentIndex == -1)
                {
                    Debug("  program ended");
                    break; //Stop if program done
                }
                else Debug($"  next non-null was {currentIndex}");
                var expr = expressions[currentIndex];
                Debug($"  index {currentIndex} is {expr}");


                if (expr is UnaryOperatorExpression uoe)
                {
                    if (uoe.Operation.Precedence == precedence)
                    {
                        Debug(" > unary operator " + uoe.ToString() + " at " + currentIndex.ToString());
                        //Find next non-null
                        int target = FindNextNonNullExpression(currentIndex, EvaluationDirection.Right, expressions);
                        if (target != -1)
                        {
                            uoe.affected = expressions[target];
                            expressions[target] = Expression.Null;
                        }
                    }
                    else Debug($" > skipped (precedence {uoe.Operation.Precedence} does not match target {precedence})");
                }

                if (expr is BinaryOperatorExpression boe)
                {
                    if (boe.Operation.Precedence == precedence)
                    {
                        Debug(" > binary operator " + boe.ToString() + " at " + currentIndex.ToString());
                        //Find next non-null
                        int targetR = FindNextNonNullExpression(currentIndex, EvaluationDirection.Right, expressions);
                        if (targetR != -1)
                        {
                            boe.affectedR = expressions[targetR];
                            expressions[targetR] = Expression.Null;
                            Debug("  found right operand " + boe.affectedR.ToString() + " at " + targetR.ToString());
                        }
                        //Find last non-null
                        int targetL = FindNextNonNullExpression(currentIndex, EvaluationDirection.Left, expressions);
                        if (targetL != -1)
                        {
                            boe.affectedL = expressions[targetL];
                            expressions[targetL] = Expression.Null;
                            Debug("  found left operand " + boe.affectedL.ToString() + " at " + targetL.ToString());
                        }
                    }
                    else Debug($" > skipped (precedence {boe.Operation.Precedence} does not match target {precedence})");
                }
                if (expr is CompoundOperatorExpression coe)
                {
                    if (coe.Operation.Precedence == precedence)
                    {
                        Debug($" > unary/binary operator {coe} at {currentIndex}, assume binary");
                        //Find next non-null
                        int targetR = FindNextNonNullExpression(currentIndex, EvaluationDirection.Right, expressions);
                        if (targetR != -1)
                        {
                            coe.affectedRequired = expressions[targetR];
                            expressions[targetR] = Expression.Null;
                            Debug("  found right operand " + coe.affectedRequired.ToString() + " at " + targetR.ToString());
                        }
                        //Find last non-null
                        int targetL = FindNextNonNullExpression(currentIndex, EvaluationDirection.Left, expressions);
                        if (targetL != -1)
                        {
                            coe.affectedOptional = expressions[targetL];
                            expressions[targetL] = Expression.Null;
                            Debug("  found left operand " + coe.affectedOptional.ToString() + " at " + targetL.ToString());
                        }
                    }
                    else Debug($" > skipped (precedence {coe.Operation.Precedence} does not match target {precedence})");
                }
                if (expr is NAryOperatorExpression noe)
                {
                    if (noe.Operation.Precedence == precedence)
                    {
                        int grabs = noe.Operation.Separators.Length * 2 + 1;
                        Debug(" > n-ary operator " + noe.ToString() + " at " + currentIndex.ToString() + " wanting " + grabs + " exprs");
                        //Find next non-null
                        for (int i = 0; i < grabs; i++)
                        {
                            int target = FindNextNonNullExpression(currentIndex, EvaluationDirection.Right, expressions);
                            if (target != -1)
                            {
                                noe.affected.Add(expressions[target]);
                                Debug("   ate " + expressions[target].Stringify());
                                expressions[target] = Expression.Null;
                            }
                        }
                        noe.affected.RemoveAll(x => x is TokenExpression te && noe.Operation.Separators.Contains(te.value.Value));
                    }
                    else Debug($" > skipped (precedence {noe.Operation.Precedence} does not match target {precedence})");
                }
            }
            expressions.RemoveAll((expr) => expr == Expression.Null);
        }

        private void MakeKeywordsGrabExpressions(List<Expression> expressions)
        {
            Debug($" Keyword Loop:");
            int currentIndex = expressions.Count;
            while (true)
            {
                Debug($"  index from {(currentIndex == expressions.Count ? "end" : currentIndex)}");
                currentIndex = FindNextNonNullExpression(currentIndex, EvaluationDirection.Left, expressions);
                if (currentIndex == -1)
                {
                    Debug("  program ended");
                    break; //Stop if program done
                }
                else Debug($"  next expression at {currentIndex}");
                var expr = expressions[currentIndex];
                Debug($"  index {currentIndex} is {expr}");

                if (expr is KeywordExpression kwe)
                {
                    Debug($" > found keyword {expr}");
                    Debug($"   search index from {(currentIndex == -1 ? "start" : currentIndex)}");
                    for (byte i = 0; i < kwe.Keyword.ArgumentCount; i++)
                    {
                        int searchIndex = FindNextNonNullExpression(currentIndex, EvaluationDirection.Right, expressions);
                        if (searchIndex == -1)
                        {
                            Debug("  oh no, keyword doesn't have enough expressions, let's error!");
                            throw new SyntaxError($"Unexpected end of input!");
                        }
                        else Debug($"  next argument at {searchIndex}");
                        kwe.Expressions.Add(expressions[searchIndex]);
                        Debug($"  keyword ate {expressions[searchIndex]}");
                        expressions[searchIndex] = Expression.Null;
                    }

                }
            }
            expressions.RemoveAll((expr) => expr == Expression.Null);
        }
        private void MakeKeywordsBackReference(List<Expression> expressions)
        {
            Debug($" Add Backreferences:");
            int currentIndex = -1;
            while (true)
            {
                Debug($"  index from {(currentIndex == -1 ? "start" : currentIndex)}");
                currentIndex = FindNextNonNullExpression(currentIndex, EvaluationDirection.Right, expressions);
                if (currentIndex == -1)
                {
                    Debug("  program ended");
                    break; //Stop if program done
                }
                else Debug($"  next expression at {currentIndex}");
                var expr = expressions[currentIndex];
                Debug($"  index {currentIndex} is {expr}");

                if (expr is KeywordExpression kwe && kwe.Keyword is BackReferencingKeyword brk)
                {

                    Debug($" < keyword wants backreference");
                    Debug($"  search index from {(currentIndex == -1 ? "start" : currentIndex)}");
                    int lastkw = FindNextNonNullExpression(currentIndex, EvaluationDirection.Left, expressions);
                    Debug($"  keyword at {(lastkw == -1 ? "end" : lastkw)} ({expressions[lastkw]})");
                    if (lastkw == -1)
                    {
                        throw new SyntaxError($"Keyword {brk.identifier} must come directly after a statement or expression!");
                    }
                    if (expressions[lastkw] is TokenExpression tx && tx.value == ";")
                    {
                        Debug("  skipped semicolon");
                        lastkw = FindNextNonNullExpression(lastkw, EvaluationDirection.Left, expressions);
                        Debug($"  keyword at {(lastkw == -1 ? "end" : lastkw)} ({expressions[lastkw]})");
                    }
                    if (lastkw == -1)
                    {
                        throw new SyntaxError($"Keyword {brk.identifier} must come directly after a statement or expression!");
                    }
                    var toGrab = expressions[lastkw];
                    if (toGrab is not KeywordExpression kx) throw new SyntaxError($"Keyword {brk.identifier} must come directly after a keyword! (currently {toGrab}");
                    if (!brk.AllowedReferences.Contains(kx.Keyword.identifier)) throw new SyntaxError($"Keyword {brk.identifier} must come directly after one of these keywords: {string.Join(' ', brk.AllowedReferences)} (found {kx.Keyword.identifier})!");
                    kwe.Reference = kx;
                    Debug($" ~ Keyword refs {kwe.Reference}{(ReferenceEquals(kwe, kwe.Reference) ? " (itself!)" : "")}");
                }
            }
            expressions.RemoveAll((expr) => expr == Expression.Null);
        }
        private int FindClosingBracket(int currentIndex, int level, char openingToken, char closingToken, List<Expression> expressions)
        {
            int currentLevel = level;
            for (int i = currentIndex; i < expressions.Count; i++)
            {
                var expr = expressions[i];
                if (expr is null) break; //If out of bounds (i.e. it's done)

                Debug($"  Expression {expr}");

                if (expr is TokenExpression ide)
                {
                    if (ide.value == new string(closingToken, 1))
                    {
                        currentLevel--;
                        Debug($"  Closing bracket {ide.value} | Level is now " + currentLevel);
                        if (currentLevel == level - 1)
                        {
                            Debug($"  Found it!");
                            return i;
                        }
                        continue;
                    }
                }
                if (expr is BracketExpression brx)
                {
                    //Allow stuff like backslashes
                    if (brx.bracket.Open == brx.bracket.Close && brx.bracket.Close == closingToken)
                    {
                        currentLevel--;
                        Debug($"  Closing bracket {closingToken} | Level is now " + currentLevel);
                        if (currentLevel == level - 1)
                        {
                            Debug($"  Found it!");
                            return i;
                        }
                        continue;
                    }
                    if (brx.bracket.Open == openingToken)
                    {
                        currentLevel++;
                        Debug($"  Opening bracket {openingToken} | Level is now " + currentLevel);
                        continue;
                    }
                }
            }
            return -1;
        }

        private int FindNextNonNullExpression(int from, EvaluationDirection direction, List<Expression> expressions)
        {
            int search = from;
            Expression expr = Expression.Null;
            while (expr == Expression.Null)
            {
                if (direction == EvaluationDirection.Right) search++;
                else search--;
                if (search < 0 || search >= expressions.Count) return -1;
                expr = expressions[search];
            }
            return search;
        }

        internal void Debug(string message)
        {
            if (debugMode) debug += message + "\n";
            else debug = "Debug mode is disabled.\n";
        }
    }
    internal enum EvaluationDirection
    {
        Left, Right
    }
}