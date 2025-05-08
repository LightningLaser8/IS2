using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Interpreter;
using ISL.Language.Expressions;
using ISL.Language.Types;
using ISL.Runtime.Errors;

namespace ISL.Language.Variables
{
    /// <summary>
    /// After <see cref="ConstantExpression"/>, the logical next step.
    /// </summary>
    internal class VariableExpression : Expression
    {
        public string variable = "";
        public override IslValue Eval(IslProgram program)
        {
            if (variable.Length == 0) throw new InvalidReferenceError("Variable expression refers to nothing!");
            if (!program.Vars.TryGetValue(variable, out IslVariable? value)) throw new InvalidReferenceError($"Variable '{variable}' doesn't exist!");
            return value.Value;
        }
        public override Expression Simplify()
        {
            return this;
        }
        public override string ToString()
        {
            return $@"\ <ref {variable}> \";
        }
        public override string Stringify() => $@"\{variable}\ ";
    }
}
