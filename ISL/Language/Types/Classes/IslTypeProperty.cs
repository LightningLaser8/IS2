using ISL.Language.Types.Functions;

namespace ISL.Language.Types.Classes
{
    internal class IslTypeProperty : IslTypeMember
    {
        public IslType ReturnType = IslType.Null;
        private readonly IslFunction fn = new();

        public override string ToString()
        {
            return $"{ReturnType} property";
        }
    }
}
