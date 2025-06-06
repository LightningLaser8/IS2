using ISL.Language.Types;

namespace ISL.Language.Expressions
{
    internal class TypeExpression : ConstantExpression
    {
        public IslIdentifier value = "";
        public override IslIdentifier Eval()
        {
            return value;
        }

        public override string ToString()
        {
            return $"(Type Identifier) {value.Stringify()}";
        }
        public override string Stringify() => $"{value.Stringify()}";
        public override bool Equals(Expression? other) => other is TypeExpression ib && ib.value == value;
    }
}
