using ISL.Interpreter;
using ISL.Language.Types.Functions;
using ISL.Runtime.Errors;

namespace ISL.Language.Types.Classes
{
    internal class IslTypeMethod : IslTypeMember
    {
        public IslType ReturnType = IslType.Null;
        private readonly IslFunction fn = new();

        public override IslValue Get(IslProgram program, IslObject instance)
        {
            fn.ThisArg = instance;
            return fn;
        }

        public override IslValue Set(IslProgram program, IslObject instance, IslValue newVal)
        {
            if (newVal.Type == IslType.Function || newVal is not IslFunction fn) throw new TypeError("Cannot set a method to a non-function type!");
            instance.DirectlySetData(AccessorKey, fn);
            return newVal;
        }

        public override string ToString()
        {
            return $"{ReturnType} method";
        }
    }
}
