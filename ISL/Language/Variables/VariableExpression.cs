using ISL.Interpreter;
using ISL.Language.Expressions;
using ISL.Language.Types;
using ISL.Runtime.Errors;

namespace ISL.Language.Variables
{
    /// <summary>
    /// After <see cref="ConstantExpression"/>, the logical next step.<br/>
    /// Returns the value of a variable.
    /// </summary>
    internal class VariableExpression : Expression
    {
        public string variable = "";
        public override IslValue Eval(IslProgram program)
        {
            if (variable.Length == 0) throw new InvalidReferenceError("Variable expression refers to nothing!");
            return program.GetVariableImperative(variable).Value;
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
