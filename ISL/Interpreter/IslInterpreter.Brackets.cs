﻿using ISL.Language.Expressions;
using ISL.Language.Expressions.Combined;
using ISL.Runtime.Errors;

namespace ISL.Interpreter
{
    internal partial class IslInterpreter
    {
        internal BracketType[] Brackets = [];
        public bool HasBracket(char brkt)
        {
            foreach (var bracket in Brackets)
            {
                if (bracket.Open == brkt || bracket.Close == brkt) return true;
            }
            return false;
        }
        public void InitBrackets()
        {
            Brackets = [
                new BracketType('(', ')', (expr) => {
                    if(expr.Count > 1) throw new SyntaxError("Round brackets ( .. ) can only contain one expression!");
                    if(expr.Count == 0) return new BracketedExpression() { expression = Expression.Null };
                    return new BracketedExpression() { expression = expr[0] };
                }),
                new BracketType('[', ']', (expr) => {
                    return new CollectionExpression() { expressions = expr };
                }),
                new BracketType('\\', '\\', (expr) => {
                    if (expr.Count > 1) throw new SyntaxError("Variable getters \\ .. \\ can only contain one expression!");
                    if(expr.Count == 0) return new GetterExpression();
                    return new GetterExpression() { NameProvider = expr[0] };
                }),
                new BracketType('{', '}', (expr) => {
                    return new CodeBlockExpression() { expressions = expr };
                }),
                new BracketType('⟨', '⟩', (expr) => {
                    return new ClassCreationExpr() { expressions = expr };
                }),
            ];
        }
    }
}

/*          "{", "}",
            "[", "]",
            "(", ")",
            "<", ">", //kind of impossible

          Non-standard

            "⟨", "⟩",
            "⁅", "⁆"
*/