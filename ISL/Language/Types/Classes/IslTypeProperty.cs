using ISL.Interpreter;
using ISL.Runtime.Errors;

namespace ISL.Language.Types.Classes
{
    internal class IslTypeProperty : IslTypeMember
    {
        public IslType ReturnType = IslType.Null;
        private readonly IslFunction fn = new();

        public override IslValue Get(IslProgram program, IslObject instance) {
            var resolved = fn.Call(program, [instance]);
            if(resolved.Type != ReturnType) throw new TypeError($"Function returned an incorrect type (got {resolved.Type}, expected {ReturnType})");
            return resolved;
        }

        public override IslValue Set(IslProgram program, IslObject instance, IslValue newVal)
        {
            if (newVal.Type == IslType.Function || newVal is not IslFunction fn) throw new TypeError("Cannot set a method to a non-function type!");
            instance.DirectlySetData(Name, fn);
            return newVal;
        }

        public override string ToString()
        {
            return $"(property {Name}) => {ReturnType}";
        }
    }
}
