using ISL.Language.Types;
using ISL.Language.Variables;
using ISL.Runtime.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISL.Interpreter
{
    internal partial class IslInterpreter
    {
        readonly Dictionary<string, IslVariable> variables = [];
        public IslVariable GetVariable(string name)
        {
            if (!variables.TryGetValue(name, out var value)) throw new InvalidReferenceError($"Variable '{name}' does not exist!");
            return value;
        }
    }
}
