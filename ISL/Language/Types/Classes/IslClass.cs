using ISL.Interpreter;
using ISL.Language.Types.Functions;

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
        public IslObject Instantiate(IslProgram prog)
        {
            var obj = new IslObject() { Class = this };
            foreach (var keyValue in Members)
            {
                var member = keyValue.Value;
                if (member is IslTypeField ifield) obj.AddProperty(keyValue.Key, ifield.FieldType, ifield.Value, ifield.readOnly, !ifield.isUninitialised);
            }
            return obj;
        }
    }
}
