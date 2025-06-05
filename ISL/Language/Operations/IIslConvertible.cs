using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal interface IIslConvertible
    {
        public IslValue Convert(IslType type);
    }
}
