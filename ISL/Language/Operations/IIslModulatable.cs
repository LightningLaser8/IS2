using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal interface IIslModulatable
    {
        public IslValue Modulate(IslValue divisor);
    }
}
