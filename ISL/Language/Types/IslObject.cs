using ISL.Interpreter;
using ISL.Language.Types.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISL.Language.Types
{
    public class IslObject : IslValue
    {
        public override IslType Type => IslType.Object;
        public IslClass Class { get; set; } = IslInterpreter.Object;
        internal Dictionary<string, IslValue> data = [];
        internal IslValue DirectlyGetData(string name) => data.TryGetValue(name, out IslValue? value) ? value : IslValue.Null;
        internal IslValue DirectlySetData(string name, IslValue newVal) => data.TryAdd(name, newVal) ? newVal : data[name] = newVal;
        public override string Stringify()
        {
            return $"<{string.Join(", ", data.Select(x => $"{x.Value.Type} {x.Key} = {x.Value.Stringify()}"))}>";
        }

        public override object? ToCLR()
        {
            return this;
        }
        public IslValue Get(IslProgram program, string name)
        {
            return this.Class.Get(program, name, this);
        }
        public IslValue Set(IslProgram program, string name, IslValue newValue)
        {
            return this.Class.Set(program, name, newValue, this);
        }
    }
}
