using ISL.Interpreter;
using ISL.Language.Types.Classes;
using ISL.Language.Types.Functions;
using ISL.Language.Variables;
using ISL.Runtime.Errors;

namespace ISL.Language.Types
{
    public class IslObject : IslValue
    {
        public override IslType Type => IslType.Object;
        public IslClass Class { get; set; } = IslInterpreter.Object;
        internal Dictionary<string, IslVariable> data = [];
        internal IslVariable GetProperty(string name) => data.TryGetValue(name, out IslVariable? value) ? value : throw new InvalidReferenceError($"Property {name} does not exist on object of type {Class.Name}");
        internal IslValue AddProperty(string name, IslType type, IslValue newVal, bool readOnly = false, bool init = true) => data.TryAdd(name, new(name, type) { Value = newVal, ReadOnly = readOnly, Initialised = init }) ? newVal : data[name] = new(name, type) { Value = newVal, ReadOnly = readOnly, Initialised = init };
        public override string Stringify()
        {
            return $"{Class.Name} <{string.Join(", ", data.Select(x => x.Value.Value is IslFunction ifn ? $"{x.Key}[{string.Join(", ", ifn.Signature.paramTypes)}]" : $"{x.Key} = {(x.Value.ReadOnly ? "readonly " : "")}{x.Value.Value.Stringify()}"))}>";
        }

        public override object? ToCLR()
        {
            return this;
        }
    }
}
