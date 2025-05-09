using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal interface IIslEquatable
    {
        public IslBool EqualTo(IslValue other);
    }
}
