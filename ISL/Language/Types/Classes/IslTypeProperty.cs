using ISL.Interpreter;
using ISL.Language.Types.Functions;
using ISL.Runtime.Errors;

namespace ISL.Language.Types.Classes
{
    internal class IslTypeProperty : IslTypeMember
    {
        public IslType ReturnType = IslType.Null;
        private readonly IslFunction fn = new();

        public override IslValue Get(IslProgram program, IslObject instance)
        {
            //get cached function
            var getattempt = instance.DirectlyGetData(AccessorKey);
            if(getattempt == IslValue.Null) getattempt = instance.DirectlySetData(AccessorKey, fn);
            if(getattempt is not IslFunction fnc) throw new TypeError($"Function get returned a non-function type.");
            var resolved = fnc.Call(program, [instance]);
            if (resolved.Type != ReturnType) throw new TypeError($"Function returned an incorrect type (got {resolved.Type}, expected {ReturnType})");
            return resolved;
        }

        public override IslValue Set(IslProgram program, IslObject instance, IslValue newVal)
        {
            if (newVal.Type == IslType.Function || newVal is not IslFunction fn) throw new TypeError("Cannot set a property to a non-function type!");
            instance.DirectlySetData(AccessorKey, fn);
            return newVal;
        }

        public override string ToString()
        {
            return $"{ReturnType} property";
        }
    }
}
