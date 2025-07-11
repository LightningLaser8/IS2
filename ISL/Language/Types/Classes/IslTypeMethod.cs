using ISL.Interpreter;
using ISL.Language.Types.Functions;

namespace ISL.Language.Types.Classes
{
    internal class IslTypeMethod : IslTypeMember
    {
        public IslType ReturnType = IslType.Null;
        private readonly IslFunction fn = new();

        public IslFunction GetFn(IslProgram program, IslObject instance)
        {
            fn.ThisArg = instance;
            return fn;
        }

        public override string ToString()
        {
            return $"{ReturnType} method";
        }
    }
}
