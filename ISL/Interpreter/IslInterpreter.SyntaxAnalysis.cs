using ISL.Language.Expressions;
using ISL.Language.Expressions.Combined;
using ISL.Language.Keywords;
using ISL.Language.Types;
using ISL.Language.Types.Functions;
using ISL.Language.Variables;
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
            IslDebugOutput.Debug("Bracketise:");
            //Put shit in brackets
            Bracketise(expressions);

            Treeify(expressions);
            IslDebugOutput.Debug(" Validate Expression Syntax:");
            ValidateCodeBlock(expressions);
            expressions.ForEach(ex => { IslDebugOutput.Debug($"  validating {ex}"); ex.Validate(); });
            IslDebugOutput.Debug("Syntax Analysis / Code Gen Finished!");
        }
        internal static void ValidateCodeBlock(List<Expression> expressions)
        {
            bool needsSemicolon = false;
            for (int i = 0; i < expressions.Count; i++)
            {
                Expression ex = expressions[i];
                IslDebugOutput.Debug("  token or expression " + ex.ToString() + (needsSemicolon ? ", wants semicolon here" : ", wants expression here"));
                //if (!needsSemicolon && ex is TokenExpression te && te.value == ";") throw new SyntaxError("Unexpected ;"); //Screw this, semicolon spam
                if (needsSemicolon && ex is not TokenExpression && !(i > 0 && expressions[i - 1] is CodeBlockExpression)) throw new SyntaxError("Expected ;");
                needsSemicolon = !(ex is TokenExpression t2e && t2e.value == ";");
                if (!needsSemicolon)
                {
                    expressions.RemoveAt(i);
                    i--;
                }
            }
        }

        private void Treeify(List<Expression> expressions)
        {
            IslDebugOutput.Debug("Operator Precedence:");

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
            MakeVariableDeclarations(expressions);
            if (precedences.Count == 0)
            {
                IslDebugOutput.Debug("  No operators present.");
                IslDebugOutput.Debug("  No tree creation required.");
            }
            else
            {
                IslDebugOutput.Debug($"  Precedences: [{string.Join(", ", precedences)}]");
                IslDebugOutput.Debug("Create Expression Tree:");
                foreach (int precedence in precedences)
                    CreateTreeLevel(precedence, expressions);
                IslDebugOutput.Debug("  Tree Created.");
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
                    IslDebugOutput.Debug($"  Found {cexp.bracket.Open} at {i}, looking for {cexp.bracket.Close}");
                    int end = FindClosingBracket(i + 1, level, cexp.bracket.Open, cexp.bracket.Close, expressions);
                    if (end == -1 || i + 1 > expressions.Count) throw new SyntaxError("Expected closing " + cexp.bracket.Close);
                    IslDebugOutput.Debug($"  Found closing {cexp.bracket.Close} at {end}");
                    expressions[end] = Expression.Null;
                    var captured = expressions[(i + 1)..end];
                    Bracketise(captured, level);
                    IslDebugOutput.Debug("Treeifying captured expressions:");
                    Treeify(captured);
                    var packaged = cexp.bracket.Create.Invoke(captured);
                    IslDebugOutput.Debug("Removing expressions:");
                    expressions[i] = packaged;
                    if (expressions.Count > 0) expressions.RemoveRange(i + 1, end - i);
                    IslDebugOutput.Debug($"  Brackets enclose {packaged}");
                }
            }
        }
        private static void CreateTreeLevel(int precedence, List<Expression> expressions)
        {
            IslDebugOutput.Debug($" Precedence level {precedence}:");
            int currentIndex = -1;
            while (true)
            {
                IslDebugOutput.Debug($"  index from {(currentIndex == -1 ? "start" : currentIndex)}");
                currentIndex = FindNextNonNullExpression(currentIndex, EvaluationDirection.Right, expressions);
                if (currentIndex == -1)
                {
                    IslDebugOutput.Debug("  program ended");
                    break; //Stop if program done
                }
                else IslDebugOutput.Debug($"  next non-null was {currentIndex}");
                var expr = expressions[currentIndex];
                IslDebugOutput.Debug($"  index {currentIndex} is {expr}");


                if (expr is UnaryOperatorExpression uoe)
                {
                    if (uoe.Operation.Precedence == precedence)
                    {
                        IslDebugOutput.Debug(" > unary operator " + uoe.ToString() + " at " + currentIndex.ToString());
                        //Find next non-null
                        EvalUnaryOperator(uoe, currentIndex, expressions);
                    }
                    else IslDebugOutput.Debug($" > skipped (precedence {uoe.Operation.Precedence} does not match target {precedence})");
                }

                if (expr is BinaryOperatorExpression boe)
                {
                    if (boe.Operation.Precedence == precedence)
                    {
                        IslDebugOutput.Debug(" > binary operator " + boe.ToString() + " at " + currentIndex.ToString());
                        //Find next non-null
                        int targetR = FindNextNonNullExpression(currentIndex, EvaluationDirection.Right, expressions);
                        if (targetR != -1 && expressions[targetR] is not TokenExpression)
                        {
                            boe.affectedR = expressions[targetR];
                            expressions[targetR] = Expression.Null;
                            IslDebugOutput.Debug("  found right operand " + boe.affectedR.ToString() + " at " + targetR.ToString());
                        }
                        //Find last non-null
                        int targetL = FindNextNonNullExpression(currentIndex, EvaluationDirection.Left, expressions);
                        if (targetL != -1 && expressions[targetL] is not TokenExpression)
                        {
                            boe.affectedL = expressions[targetL];
                            expressions[targetL] = Expression.Null;
                            IslDebugOutput.Debug("  found left operand " + boe.affectedL.ToString() + " at " + targetL.ToString());
                        }
                    }
                    else IslDebugOutput.Debug($" > skipped (precedence {boe.Operation.Precedence} does not match target {precedence})");
                }
                if (expr is CompoundOperatorExpression coe)
                {
                    if (coe.Operation.Precedence == precedence)
                    {
                        IslDebugOutput.Debug($" > unary/binary operator {coe} at {currentIndex}, assume binary");
                        //Find next non-null
                        int targetR = FindNextNonNullExpression(currentIndex, EvaluationDirection.Right, expressions);
                        if (targetR != -1 && expressions[targetR] is not TokenExpression)
                        {
                            coe.affectedRequired = expressions[targetR];
                            expressions[targetR] = Expression.Null;
                            IslDebugOutput.Debug("  found right operand " + coe.affectedRequired.ToString() + " at " + targetR.ToString());
                        }
                        //Find last non-null
                        int targetL = FindNextNonNullExpression(currentIndex, EvaluationDirection.Left, expressions);
                        if (targetL != -1 && expressions[targetL] is not TokenExpression)
                        {
                            coe.affectedOptional = expressions[targetL];
                            expressions[targetL] = Expression.Null;
                            IslDebugOutput.Debug("  found left operand " + coe.affectedOptional.ToString() + " at " + targetL.ToString());
                        }
                    }
                    else IslDebugOutput.Debug($" > skipped (precedence {coe.Operation.Precedence} does not match target {precedence})");
                }
            }
            expressions.RemoveAll((expr) => expr == Expression.Null);
        }

        private static void EvalUnaryOperator(UnaryOperatorExpression unary, int currentIndex, List<Expression> expressions, int depth = 0)
        {
            IslDebugOutput.Debug($"{new string(' ', depth)} >> simplifying {unary.Operation.id}");
            int target = FindNextNonNullExpression(currentIndex, EvaluationDirection.Right, expressions);
            if (target != -1)
            {
                var texp = expressions[target];
                expressions[target] = Expression.Null;
                unary.affected = texp;
                IslDebugOutput.Debug($"{new string(' ', depth)}  | now affecting {texp}");
                if (texp is UnaryOperatorExpression ue)
                {
                    IslDebugOutput.Debug($"{new string(' ', depth)}  > found another unary op ({unary.Operation.id}), which may or may not be impossible to evaluate without this bit");
                    EvalUnaryOperator(ue, target, expressions, depth + 1);
                }
                IslDebugOutput.Debug($"{new string(' ', depth)} << {unary}");
            }
        }

        private void MakeVariableDeclarations(List<Expression> expressions)
        {
            IslDebugOutput.Debug($" VarDec/FnCall Loop:");
            int currentIndex = expressions.Count;
            while (true)
            {
                IslDebugOutput.Debug($"  index from {(currentIndex == expressions.Count ? "end" : currentIndex)}");
                currentIndex = FindNextNonNullExpression(currentIndex, EvaluationDirection.Left, expressions);
                if (currentIndex == -1)
                {
                    IslDebugOutput.Debug("  program ended");
                    break; //Stop if program done
                }
                else IslDebugOutput.Debug($"  next expression at {currentIndex}");
                var expr = expressions[currentIndex];
                IslDebugOutput.Debug($"  index {currentIndex} is {expr}");



                if (expr is IdentifierExpression ie && ie is not TokenExpression)
                {
                    var (native, present) = HasType(ie.value);
                    if (present)
                    {
                        IslDebugOutput.Debug($" > found {(native ? "native" : "user-defined")} type {expr}");
                        int nameIndex = FindNextNonNullExpression(currentIndex, EvaluationDirection.Right, expressions);
                        if (nameIndex == -1)
                        {
                            IslDebugOutput.Debug("  ok, type doesn't have an identifier");
                            IslDebugOutput.Debug("  i may break this in the future");
                        }
                        else
                        {
                            IslDebugOutput.Debug($"  var name at {nameIndex}");
                            var naem = expressions[nameIndex];
                            IslDebugOutput.Debug($"  creating declaration for {naem}");
                            if (naem is not IdentifierExpression ide) throw new SyntaxError("Expected identifier or nothing after type name");
                            if (naem is TokenExpression)
                            {
                                //Stops shit like 'String [' showing up in debug
                                continue;
                            }
                            var val = GetNativeType(ie.value)();
                            var declaration = new VariableDeclarationExpr() { name = ide.value, varType = val.Type, initialValue = val, IsTypeInferred = val is IslNull };
                            expressions[currentIndex] = declaration;

                            IslDebugOutput.Debug($"  created declaration for {expressions[currentIndex]}");
                            expressions[nameIndex] = Expression.Null;

                            //imply/const
                            int modifierIndex = FindNextNonNullExpression(currentIndex, EvaluationDirection.Left, expressions);
                            if (modifierIndex == -1)
                            {
                                IslDebugOutput.Debug("  ok, type doesn't have any modifiers, that's fine");
                            }
                            else
                            {
                                if (expressions[modifierIndex] is IdentifierExpression ide2)
                                {
                                    if (ide2.value == "imply")
                                    {
                                        expressions[modifierIndex] = Expression.Null;
                                        if (declaration.IsTypeInferred) throw new SyntaxError("Cannot both infer and imply type of a variable!");
                                        declaration.IsTypeImplied = true;
                                    }
                                    else if (ide2.value == "const")
                                    {
                                        expressions[modifierIndex] = Expression.Null;
                                        if (declaration.IsTypeInferred || declaration.IsTypeImplied) throw new SyntaxError("Cannot make an inferred or implied variable read-only.");
                                        declaration.IsReadOnly = true;
                                    }
                                }
                            }
                            continue;
                        }
                    }
                    else
                    {
                        IslDebugOutput.Debug($" > found general identifier {expr}");
                        int nameIndex = FindNextNonNullExpression(currentIndex, EvaluationDirection.Right, expressions);
                        if (nameIndex == -1)
                        {
                            IslDebugOutput.Debug("  this is not a function call");
                            IslDebugOutput.Debug("  i may break this in the future");
                        }
                        else
                        {
                            IslDebugOutput.Debug($"  parameter list at {nameIndex}");
                            var paramlst = expressions[nameIndex];
                            if (paramlst is not CollectionExpression cexp) continue;
                            IslDebugOutput.Debug($"  creating function call for {ie} with {paramlst}");
                            var declaration = new FunctionCallExpression() { function = ie.value, parameters = cexp };
                            expressions[currentIndex] = declaration;

                            IslDebugOutput.Debug($"  created function call for {ie.value.Value}");
                            expressions[nameIndex] = Expression.Null;
                        }
                    }
                }
            }
            expressions.RemoveAll((expr) => expr == Expression.Null);
        }

        private static void MakeKeywordsGrabExpressions(List<Expression> expressions)
        {
            IslDebugOutput.Debug($" Keyword Loop:");
            int currentIndex = expressions.Count;
            while (true)
            {
                IslDebugOutput.Debug($"  index from {(currentIndex == expressions.Count ? "end" : currentIndex)}");
                currentIndex = FindNextNonNullExpression(currentIndex, EvaluationDirection.Left, expressions);
                if (currentIndex == -1)
                {
                    IslDebugOutput.Debug("  program ended");
                    break; //Stop if program done
                }
                else IslDebugOutput.Debug($"  next expression at {currentIndex}");
                var expr = expressions[currentIndex];
                IslDebugOutput.Debug($"  index {currentIndex} is {expr}");

                if (expr is KeywordExpression kwe)
                {
                    IslDebugOutput.Debug($" > found keyword {expr}");
                    IslDebugOutput.Debug($"   search index from {(currentIndex == -1 ? "start" : currentIndex)}");
                    for (byte i = 0; i < kwe.Keyword.ArgumentCount; i++)
                    {
                        int searchIndex = FindNextNonNullExpression(currentIndex, EvaluationDirection.Right, expressions);
                        if (searchIndex == -1)
                        {
                            IslDebugOutput.Debug("  oh no, keyword doesn't have enough expressions, let's error!");
                            throw new SyntaxError($"Unexpected end of input!");
                        }
                        else IslDebugOutput.Debug($"  next argument at {searchIndex}");
                        kwe.Expressions.Add(expressions[searchIndex]);
                        IslDebugOutput.Debug($"  keyword ate {expressions[searchIndex]}");
                        expressions[searchIndex] = Expression.Null;
                    }

                }
            }
            expressions.RemoveAll((expr) => expr == Expression.Null);
        }
        private static void MakeKeywordsBackReference(List<Expression> expressions)
        {
            IslDebugOutput.Debug($" Add Backreferences:");
            int currentIndex = -1;
            while (true)
            {
                IslDebugOutput.Debug($"  index from {(currentIndex == -1 ? "start" : currentIndex)}");
                currentIndex = FindNextNonNullExpression(currentIndex, EvaluationDirection.Right, expressions);
                if (currentIndex == -1)
                {
                    IslDebugOutput.Debug("  program ended");
                    break; //Stop if program done
                }
                else IslDebugOutput.Debug($"  next expression at {currentIndex}");
                var expr = expressions[currentIndex];
                IslDebugOutput.Debug($"  index {currentIndex} is {expr}");

                if (expr is KeywordExpression kwe && kwe.Keyword is BackReferencingKeyword brk)
                {

                    IslDebugOutput.Debug($" < keyword wants backreference");
                    IslDebugOutput.Debug($"  search index from {(currentIndex == -1 ? "start" : currentIndex)}");
                    int lastkw = FindNextNonNullExpression(currentIndex, EvaluationDirection.Left, expressions);
                    if (lastkw == -1)
                    {
                        throw new SyntaxError($"Keyword {brk.identifier} must come directly after a statement or expression!");
                    }
                    IslDebugOutput.Debug($"  keyword at {(lastkw == -1 ? "end" : lastkw)} ({expressions[lastkw]})");
                    if (expressions[lastkw] is TokenExpression tx && tx.value == ";")
                    {
                        IslDebugOutput.Debug("  skipped semicolon");
                        lastkw = FindNextNonNullExpression(lastkw, EvaluationDirection.Left, expressions);
                        IslDebugOutput.Debug($"  keyword at {(lastkw == -1 ? "end" : lastkw)} ({expressions[lastkw]})");
                    }
                    if (lastkw == -1)
                    {
                        throw new SyntaxError($"Keyword {brk.identifier} must come directly after a statement or expression!");
                    }
                    var toGrab = expressions[lastkw];
                    if (toGrab is not KeywordExpression kx) throw new SyntaxError($"Keyword {brk.identifier} must come directly after a keyword! (currently {toGrab}");
                    if (!brk.AllowedReferences.Contains(kx.Keyword.identifier)) throw new SyntaxError($"Keyword {brk.identifier} must come directly after one of these keywords: {string.Join(' ', brk.AllowedReferences)} (found {kx.Keyword.identifier})!");
                    kwe.Reference = kx;
                    IslDebugOutput.Debug($" ~ Keyword refs {kwe.Reference}{(ReferenceEquals(kwe, kwe.Reference) ? " (itself!)" : "")}");
                }
            }
            expressions.RemoveAll((expr) => expr == Expression.Null);
        }
        private static int FindClosingBracket(int currentIndex, int level, char openingToken, char closingToken, List<Expression> expressions)
        {
            int currentLevel = level;
            for (int i = currentIndex; i < expressions.Count; i++)
            {
                var expr = expressions[i];
                if (expr is null) break; //If out of bounds (i.e. it's done)

                IslDebugOutput.Debug($"  Expression {expr}");

                if (expr is TokenExpression ide)
                {
                    if (ide.value == new string(closingToken, 1))
                    {
                        currentLevel--;
                        IslDebugOutput.Debug($"  Closing bracket {ide.value} | Level is now " + currentLevel);
                        if (currentLevel == level - 1)
                        {
                            IslDebugOutput.Debug($"  Found it!");
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
                        IslDebugOutput.Debug($"  Closing bracket {closingToken} | Level is now " + currentLevel);
                        if (currentLevel == level - 1)
                        {
                            IslDebugOutput.Debug($"  Found it!");
                            return i;
                        }
                        continue;
                    }
                    if (brx.bracket.Open == openingToken)
                    {
                        currentLevel++;
                        IslDebugOutput.Debug($"  Opening bracket {openingToken} | Level is now " + currentLevel);
                        continue;
                    }
                }
            }
            return -1;
        }

        private static int FindNextNonNullExpression(int from, EvaluationDirection direction, List<Expression> expressions)
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
    }
    internal enum EvaluationDirection
    {
        Left, Right
    }
}