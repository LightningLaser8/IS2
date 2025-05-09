using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Types;
using ISL.Runtime.Errors;

namespace ISL.Language.Variables
{
    public class IslVariable(string name, IslType type) : IslValue
    {
        public bool ReadOnly { get; set; } = false;
        public bool InferType { get; set; } = false;
        public bool ImpliedType { get; set; } = false;
        public string Name { get; } = name;
        public IslValue Value { get; set; } = DefaultForType(type);
        public override IslType Type { get; protected set; } = IslType.Variable;
        public IslType VarType { get; protected set; } = type;

        public override string Stringify()
        {
            return $"[{Name}] ({(ReadOnly ? "readonly " : "")}{VarType}) {Value.Stringify()}";
        }

        public override object? ToCLR()
        {
            return Value.ToCLR();
        }

        internal void ChangeType(IslType type)
        {
            if (ReadOnly) throw new AccessError("Variable cannot be cast - it is read-only.");
            if (InferType) VarType = type;
        }
    }
}
