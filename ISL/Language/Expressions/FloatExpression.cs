using ISL.Language.Types;

namespace ISL.Language.Expressions
{
    internal class FloatExpression : ConstantExpression
    {
        public IslFloat value = 0;
        public override IslFloat Eval()
        {
            return value;
        }

        public override string ToString()
        {
            return $"(Float) {value.Stringify()}";
        }
        public override string Stringify() => $"{value.Stringify()}";
    }
}
