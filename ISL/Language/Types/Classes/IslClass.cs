using ISL.Interpreter;
using ISL.Language.Types.Functions;
using ISL.Runtime.Errors;

namespace ISL.Language.Types.Classes
{
    public class IslClass(string name) : IslValue
    {
        internal IslClass() : this("<anonymous>")
        {
        }
        internal IslClass(Dictionary<string, IslTypeMember> members) : this("<anonymous>", members)
        {
        }
        internal IslClass(string name, Dictionary<string, IslTypeMember> members) : this(name)
        {
            Members = members;
        }
        public string Name = name;
        internal Dictionary<string, IslTypeMember> Members = [];
        internal IslFunction constructor = new();
        public override IslType Type => IslType.Class;

        public override string Stringify()
        {
            return $"{Name} [{string.Join(", ", Members.Select(x => $"{x.Key} = {x.Value}"))}]";
        }
        /// <summary>
        /// ISL-declared types can't be converted into CLR ones, just use the existing methods. This method will return this type.
        /// </summary>
        /// <returns></returns>
        public override object? ToCLR()
        {
            return this;
        }
        public IslValue Get(IslProgram program, string name, IslObject instance)
        {
            if (!Members.TryGetValue(name, out IslTypeMember? value)) throw new InvalidReferenceError($"Class {Name} does not contain a definition for {name}");
            return value.Get(program, instance);
        }
        public IslValue Set(IslProgram program, string name, IslValue newVal, IslObject instance)
        {
            if (!Members.TryGetValue(name, out IslTypeMember? value)) throw new InvalidReferenceError($"Class {Name} does not contain a definition for {name}");
            return value.Set(program, instance, newVal);
        }
        public IslObject Instantiate()
        {
            var obj = new IslObject() { Class = this };
            return obj;
        }
    }
}
