using ISL.Interpreter;
using ISL.Language.Expressions;
using ISL.Language.Expressions.Combined;
using ISL.Language.Types.Collections;
using ISL.Runtime.Errors;

namespace ISL.Language.Types.Functions
{
    internal class FunctionCallExpression : Expression
    {
        public required IslIdentifier function;
        public required Expression parameters;
        public override IslValue Eval(IslProgram program)
        {
            var fn = program.CurrentScope.GetVariableImperative(function.Value);
            if (fn.VarType != IslType.Function) throw new TypeError($"Function call expected a function variable, got {fn.VarType}");
            if (fn.Value is not IslFunction isf) throw new TypeError($"Variable was falsely marked as a function");
            if (parameters is not CollectionExpression cexp) throw new TypeError($"Parameter list for function call must be a collection expression.");
            return isf.Call(program, [.. ((IslGroup)cexp.Eval(program)).Value]);
        }

        public override Expression Simplify() => new FunctionCallExpression() { function = function, parameters = parameters.Simplify() };
        public override void Validate() => parameters.Validate();

        public override string Stringify() => $"{function.Stringify()}{parameters.Stringify()}";
        public override string ToString() => $"(Call) {function.Stringify()} with {parameters.Stringify()}";
    }
}
