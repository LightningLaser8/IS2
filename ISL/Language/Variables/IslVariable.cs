using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Types;

namespace ISL.Language.Variables
{
    internal class IslVariable(string name, IslType type)
    {
        public string Name { get; } = name;
        public IslValue Value { get; set; } = IslValue.DefaultForType(type);
        public IslType Type { get; } = type;

        public override string ToString()
        {
            return "(Local) " + Name + ": " + Value.Stringify();
        }
    }
}
