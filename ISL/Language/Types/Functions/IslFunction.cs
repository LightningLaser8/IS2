using ISL.Interpreter;
using ISL.Language.Expressions;
using ISL.Language.Variables;
using ISL.Runtime.Errors;

namespace ISL.Language.Types.Functions
{
    public class IslFunction : IslValue
    {
        internal IslObject? ThisArg = null;
        public IslFunction() { }
        public IslFunction(IslFunctionSignature sig)
        {
            Signature = sig;
        }
        internal IslFunction(IslFunctionSignature sig, Expression subroutine) : this(sig)
        {
            Subroutine = subroutine;
        }

        private readonly Expression Subroutine = Expression.Null;
        public override IslType Type => IslType.Function;
        internal IslFunctionSignature Signature { get; private set; } = new([], []);
        public IslValue Call(IslProgram program, IslValue[] parameters)
        {
            //Create temporary scope
            IslVariableScope VariableScope = new(program.CurrentScope);
            if (ThisArg is not null)
            {
                var @this = VariableScope.CreateVariable("this", IslType.Object);
                @this.ReadOnly = true;
                @this.Initialised = true;
                @this.Value = ThisArg;
            }
            if (parameters.Length != Signature.paramTypes.Count()) throw new SyntaxError($"Function expects {Signature.paramTypes.Count()} arguments, got {parameters.Length}.");
            for (int i = 0; i < parameters.Length; i++)
            {
                VariableScope.CreateVariable($"{Signature.paramNames.ElementAt(i)}", Signature.paramTypes.ElementAt(i), parameters[i]);
            }
            program.CurrentScope = VariableScope;
            
            var res = Subroutine.Eval(program);

            //Restore the old scope
            program.CurrentScope = VariableScope.Parent!;

            return res;
        }
        public override string Stringify() => $"[{string.Join(", ", Signature.paramTypes)}] => {Subroutine.Stringify()}";
        public override string ToString() => $"[{string.Join(", ", Signature.paramTypes)}] => {Subroutine}";
        public override object? ToCLR() => (IslProgram context) => Subroutine.Eval(context);
    }
    public class IslFunctionSignature
    {
        internal readonly IEnumerable<string> paramNames = [];
        internal readonly IEnumerable<IslType> paramTypes = [];
        public IslFunctionSignature(IEnumerable<string> paramNames, IEnumerable<IslType> paramTypes)
        {
            this.paramNames = paramNames;
            this.paramTypes = paramTypes;
            if (paramTypes.Count() != paramNames.Count()) throw new SyntaxError("Mismatched parameter name and type list.");
        }
    }
}
