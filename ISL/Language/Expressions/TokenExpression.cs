using ISL.Language.Types;
using ISL.Runtime.Errors;

namespace ISL.Language.Expressions
{
    internal class TokenExpression : IdentifierExpression
    {
        public override IslIdentifier Eval()
        {
            throw new SyntaxError("Unexpected token '" + value + "'");
        }
        public override Expression Simplify()
        {
            throw new SyntaxError("Unexpected token '" + value + "'");
        }
        public override string ToString()
        {
            return $"(Token) {value.Stringify()}";
        }
        public override string Stringify() => $"{value.Stringify()}";
        public override bool Equals(Expression? other) => other is TokenExpression ib && ib.value == value;
    }
}
