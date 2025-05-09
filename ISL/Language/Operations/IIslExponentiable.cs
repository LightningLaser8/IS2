using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal interface IIslExponentiable : IIslMultiplicable
    {
        public IslValue Exponentiate(IslValue exponent);
    }
}
