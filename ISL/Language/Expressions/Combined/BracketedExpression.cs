using ISL.Interpreter;
using ISL.Language.Types;

namespace ISL.Language.Expressions.Combined
{
    /// <summary>
    /// An expression wrapped by some brackets.
    /// </summary>
    internal class BracketedExpression : Expression
    {
        public required Expression expression;
        public override IslValue Eval(IslProgram program)
        {
            return expression.Eval(program);
        }

        public override Expression Simplify()
        {
            return expression.Simplify();
        }
        public override string ToString() => $"(Bracketed) ( {expression} )";

        public override void Validate()
        {
            expression.Validate();
        }
        public override string Stringify() => $"( {expression} )";

        public override bool Equals(Expression? other) => expression.Equals(other);
    }
}
