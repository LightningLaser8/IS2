using ISL.Interpreter;
using ISL.Language.Operations;
using ISL.Language.Types.Functions;
using ISL.Runtime.Errors;

namespace ISL.Language.Types.Classes
{
    public class IslClass(string name) : IslValue, IIslConvertible, IIslEquatable
    {
        internal IslClass() : this("<anonymous>")
        {
        }
        internal IslClass(Dictionary<string, IslTypeField> members) : this("<anonymous>", members)
        {
        }
        internal IslClass(string name, Dictionary<string, IslTypeField> members) : this(name)
        {
            Members = members;
        }
        public string Name = name;
        internal Dictionary<string, IslTypeField> Members = [];
        internal IslFunction constructor = new();
        public override IslType Type => IslType.Class;

        public override string Stringify()
        {
            return $"{Name} << {string.Join(", ", Members.Select(x => x.Value.Value is IslFunction ifn ? $"{x.Key}[{string.Join(", ", ifn.Signature.paramTypes)}]" : $"{x.Key} = {(x.Value.readOnly ? "readonly " : "")}{x.Value.Value.Stringify()}"))} >>";
            //return $"{Name} [{string.Join(", ", Members.Select(x => $"{x.Key} = {x.Value}"))}]";
        }
        public override string ToString()
        {
            return $"{Name} << {string.Join(", ", Members.Select(x => $"{x.Key} = {x.Value}"))} >>";
        }
        /// <summary>
        /// ISL-declared types can't be converted into CLR ones, just use the existing methods. This method will return this type.
        /// </summary>
        /// <returns></returns>
        public override object? ToCLR()
        {
            return this;
        }
        public IslObject Instantiate()
        {
            var obj = new IslObject() { Class = this };
            foreach (var keyValue in Members)
            {
                var member = keyValue.Value;
                if (member is IslTypeField ifield) obj.AddProperty(keyValue.Key, ifield.FieldType, ifield.Value, ifield.readOnly, !ifield.isUninitialised);
            }
            return obj;
        }
        public IslValue Convert(IslType type)
        {
            if (type == IslType.String) return new IslString(Stringify());
            throw new TypeConversionError(Type.ToString(), type.ToString());
        }
        public IslBool EqualTo(IslValue other)
        {
            return other is IslClass ic && ic.Members.SequenceEqual(Members);
        }
    }
}
