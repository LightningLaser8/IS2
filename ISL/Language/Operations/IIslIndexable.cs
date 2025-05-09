using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal interface IIslIndexable
    {
        public IslValue Index(IslValue index);
    }
}
