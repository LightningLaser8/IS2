using ISL.Interpreter;
using ISL.Runtime.Errors;

namespace ISL.Language.Types.Classes
{
    internal class IslTypeField : IslTypeMember
    {
        public IslType FieldType = IslType.Null;
        public IslValue Value = IslValue.Null;

        public override IslValue Get(IslProgram program, IslObject instance) => Value;

        public override IslValue Set(IslProgram program, IslObject instance, IslValue newVal)
        {
            if (newVal.Type != FieldType) throw new TypeError($"Attempt to set field of type {FieldType} to value of type {newVal.Type}. Try casting with '->' or '~>'.");
            Value = newVal;
            return Value;
        }

        public override string ToString()
        {
            return $"(field {FieldType}) {Name}";
        }
    }
}
