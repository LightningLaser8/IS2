using ISL.Language.Types;

namespace ISL.Language.Expressions
{
    internal class NullExpression : ConstantExpression
    {
        public override IslNull Eval()
        {
            return new IslNull();
        }
        public override string ToString()
        {
            return "(Null) null";
        }
        public override string Stringify() => $"null";
        public override bool Equals(Expression? other) => other is NullExpression;
    }
}
