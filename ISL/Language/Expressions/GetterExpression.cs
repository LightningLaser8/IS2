using ISL.Interpreter;
using ISL.Language.Types;
using ISL.Language.Variables;
using ISL.Runtime.Errors;

namespace ISL.Language.Expressions
{
    internal class GetterExpression : Expression
    {
        public Expression? NameProvider { get; internal set; }
        public override string ToString() => $"\\ {NameProvider} \\";

        public override IslValue Eval(IslProgram program)
        {
            var res = NameProvider!.Eval(program);
            if (res is IslVariable ivar)
                return ivar.Value;
            if (res is IslIdentifier iide)
                return program.CurrentScope.GetVariableImperative(iide).Value;
            throw new TypeError("Cannot get value of a " + res.Type);
        }

        public override Expression Simplify()
        {
            return this;
        }

        public override void Validate()
        {
            if (NameProvider is not null) NameProvider.Validate();
            else throw new TypeError("Cannot get value of nothing!");
        }

        public override string Stringify() => $@"\{NameProvider?.Stringify()}\";
    }
}
