using ISL.Language.Types;

namespace ISL.Language.Expressions
{
    internal class IntExpression : ConstantExpression
    {
        public IslInt value = 0;
        public override IslInt Eval()
        {
            return value;
        }
        public override string ToString()
        {
            return $"(Int) {value.Stringify()}";
        }
        public override string Stringify() => $"{value.Stringify()}";
    }
}
