using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Types;

namespace ISL.Language.Variables
{
    public class IslVariable(string name, IslType type) : IslValue
    {
        public bool InferType { get; set; } = false;
        public string Name { get; } = name;
        public IslValue Value { get; set; } = DefaultForType(type);
        public override IslType Type { get; protected set; } = type;

        public override string Stringify()
        {
            return $"[Local] {Name}: ({Type}) {Value.Stringify()}";
        }
        internal void ChangeType(IslType type)
        {
            if(InferType) Type = type;
        }
    }
}
