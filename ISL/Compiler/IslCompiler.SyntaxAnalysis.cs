using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Expressions;
using ISL.Language.Expressions.Combined;
using ISL.Language.Keywords;
using ISL.Language.Operations;
using ISL.Language.Variables;
using ISL.Runtime.Errors;

namespace ISL.Compiler
{
    internal partial class IslCompiler
    {
        //Syntax analysis, builds expression (syntax) trees from tokenised code
        private void Parse()
        {
            expressions.Clear();

            expressions.AddRange(tokens.Select((x) => Expression.From(x, this)));
            Debug("Bracketise:");
            //Put shit in brackets
            Bracketise(expressions);

            Treeify(expressions);
            Debug(" Validate Expression Syntax:");
            expressions.ForEach(ex => { Debug($" validating {ex}"); ex.Validate(); });
            Debug("Syntax Analysis / Code Gen Finished!");
        }

        private void Treeify(List<Expression> expressions)
        {
            Debug("Operator Precedence:");

            //Get precedence
            int highestPrecedence = int.MinValue;
            OperatorExpression? highOp = null;
            int lowestPrecedence = int.MaxValue;
            OperatorExpression? lowOp = null;
            foreach (var expr in expressions)
            {
                if (expr is OperatorExpression oe)
                {
                    var op = oe.GetOp();
                    if (op is not null)
                    {
                        if (op.Precedence < lowestPrecedence)
                        {
                            lowestPrecedence = op.Precedence;
                            lowOp = oe;
                        }
                        if (op.Precedence > highestPrecedence)
                        {
                            highestPrecedence = op.Precedence;
                            highOp = oe;
                        }
                    }
                }
            }
            if (highestPrecedence == int.MinValue)
            {
                Debug("  No operators present.");
                Debug("  No tree creation required.");
            }
            else
            {
                Debug($"  Highest precedence is {highestPrecedence} ({highOp?.ToString() ?? ""})");
                Debug($"  Lowest precedence is {lowestPrecedence} ({lowOp?.ToString() ?? ""})");
                Debug("Create Expression Tree:");
                for (int precedence = highestPrecedence; precedence >= lowestPrecedence; precedence--)
                    CreateTreeLevel(precedence, expressions);
                Debug("  Tree Created.");
            }
            MakeKeywordsGrabExpressions(expressions);
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
                    int end = FindClosingBracket(i, level, cexp.bracket.Open, cexp.bracket.Close, expressions);
                    if (end == -1) throw new SyntaxError("Expected closing " + cexp.bracket.Close);
                    Debug($"  Found closing {cexp.bracket.Close} at {end}");
                    expressions[end] = Expression.Null;
                    var captured = expressions[(i + 1)..end];
                    Bracketise(captured, level);
                    Debug("Treeifying captured expressions:");
                    Treeify(captured);
                    var packaged = cexp.bracket.Create.Invoke(captured);
                    expressions[i] = packaged;
                    expressions.RemoveRange(i + 1, end - i);
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
            }
            expressions.RemoveAll((expr) => expr == Expression.Null);
        }

        private void MakeKeywordsGrabExpressions(List<Expression> expressions)
        {
            Debug($" Keyword Loop:");
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

                if (expr is KeywordExpression kwe)
                {
                    Debug($" > found keyword {expr}");
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
        private int FindClosingBracket(int currentIndex, int level, char openingToken, char closingToken, List<Expression> expressions)
        {
            int currentLevel = level;
            for (int i = currentIndex; i < expressions.Count; i++)
            {
                var expr = expressions[i];
                if (expr is null) break; //If out of bounds (i.e. it's done)

                Debug($"  Expression {expr}");
                if (expr is BracketExpression brx)
                {
                    if (brx.bracket.Open == openingToken)
                    {
                        currentLevel++;
                        Debug($"  Opening bracket {openingToken} | Level is now " + currentLevel);
                        continue;
                    }
                }
                if (expr is TokenExpression ide)
                {
                    if (ide.value == new string(closingToken, 1))
                    {
                        currentLevel--;
                        Debug($"  Closing bracket {closingToken} | Level is now " + currentLevel);
                        if (currentLevel == level)
                        {
                            Debug($"  Found it!");
                            return i;
                        }
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

        private enum EvaluationDirection
        {
            Left, Right
        }

        private void Debug(string message)
        {
            if (debugMode) output += message + "\n";
            else output = "Debug mode is disabled.\n";
        }
    }
}