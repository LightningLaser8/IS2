using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Expressions;
using ISL.Language.Expressions.Combined;
using ISL.Language.Operations;
using ISL.Language.Types;
using ISL.Language.Variables;
using ISL.Runtime.Errors;

namespace ISL.Compiler
{
    internal partial class IslCompiler
    {
        public BracketType[] Brackets = [];
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
                    return new PackagedExpression() { expression = expr[0] };
                }),
                new BracketType('[', ']', (expr) => {
                    return new CollectionExpression() { expressions = expr };
                }),
                new BracketType('\\', '\\', (expr) => {
                    if (expr.Count > 1) throw new SyntaxError("Variable getters \\ .. \\ can only contain one expression!");
                    return new GetterExpression() { NameProvider = expr[0] };
                }),
                new BracketType('{', '}', (expr) => {
                    return new CodeBlockExpression() { expressions = expr };
                }),
            ];
        }
    }
}

/*          "{", "}",
            "[", "]",
            "(", ")",
            "<", ">",

          Non-standard

            "⟨", "⟩",
            "⁅", "⁆"
*/