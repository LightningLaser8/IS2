using ISL.Interpreter;
using ISL.Language.Types;
using ISL.Runtime.Errors;

namespace ISL.Language.Expressions.Combined
{
    /// <summary>
    /// Literally just a container for expressions. May as well not exist.<br/>
    /// Not sure what you were expecting.
    /// </summary>
    internal class BracketExpression : Expression
    {
        public required BracketType bracket;

        public override bool Equals(Expression? other) => other is BracketExpression be && be.bracket == bracket;

        public override IslValue Eval(IslProgram program)
        {
            throw new SyntaxError("Unresolved bracket expression!");
        }
        public override Expression Simplify()
        {
            throw new SyntaxError("Unresolved bracket expression!");
        }
        public override string Stringify() => $"{bracket.Open} or {bracket.Close}";
        public override string ToString() => $"{bracket.Open} or {bracket.Close}";
    }
}
