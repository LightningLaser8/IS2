using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal interface IIslMultiplicable : IIslAddable
    {
        public IslValue Multiply(IslValue multiplier);
    }
}
