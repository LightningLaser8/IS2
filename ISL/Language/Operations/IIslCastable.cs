using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal interface IIslCastable
    {
        public IslValue Cast(IslType type);
    }
}
