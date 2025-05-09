using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal interface IIslDivisible : IIslSubtractable
    {
        public IslValue Divide(IslValue divisor);
    }
}
