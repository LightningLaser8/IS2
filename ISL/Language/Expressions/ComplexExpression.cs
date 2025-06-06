using ISL.Language.Types;

namespace ISL.Language.Expressions
{
    internal class ComplexExpression : ConstantExpression
    {
        public IslComplex value = new();
        public override IslValue Eval()
        {
            return value;
        }
        public override string ToString()
        {
            return $"(Complex) {value.Value.Real} + {value.Value.Imaginary}i";
        }

        public override string Stringify() => $"{value.Value.Real} + {value.Value.Imaginary}i";
        public override bool Equals(Expression? other) => other is ComplexExpression ib && ib.value == value;
    }
}
