using ISL.Language.Types;

namespace ISL.Language.Expressions
{
    internal abstract class ConstantExpression : Expression
    {
        public static ConstantExpression For(IslValue val)
        {
            if (val is IslInt iint) return new IntExpression() { value = iint };
            if (val is IslFloat iflt) return new FloatExpression() { value = iflt };
            if (val is IslString istr) return new StringExpression() { value = istr };
            if (val is IslComplex icmp) return new ComplexExpression() { value = icmp };
            if (val is IslNull) return new NullExpression();
            if (val is IslIdentifier iidn) return new IdentifierExpression() { value = iidn };
            return Expression.Null;
        }
        public override Expression Simplify()
        {
            return this;
        }
    }
}
