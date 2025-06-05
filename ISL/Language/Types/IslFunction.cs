using ISL.Interpreter;
using ISL.Language.Expressions;
using ISL.Language.Variables;
using ISL.Runtime.Errors;

namespace ISL.Language.Types
{
    public class IslFunction : IslValue
    {
        public IslFunction() { }
        public IslFunction(IslFunctionSignature sig)
        {
            Signature = sig;
        }

        private readonly Expression Subroutine = Expression.Null;
        public override IslType Type => IslType.Function;
        internal IslFunctionSignature Signature { get; private set; } = new([], []);
        public IslValue Call(IslProgram program, IslValue[] parameters)
        {
            //Create temporary scope
            IslVariableScope VariableScope = new(program.CurrentScope);
            if (parameters.Length != Signature.paramTypes.Length) throw new SyntaxError($"Function expects {Signature.paramTypes.Length} arguments, got {parameters.Length}.");
            for (int i = 0; i < parameters.Length; i++)
            {
                VariableScope.CreateVariable($"{Signature.paramNames[i]}", Signature.paramTypes[i], parameters[i]);
            }
            program.CurrentScope = VariableScope;
            
            var res = Subroutine.Eval(program);

            //Restore the old scope
            program.CurrentScope = VariableScope.Parent!;

            return res;
        }
        public override string Stringify() => $"({string.Join(", ", Signature.paramTypes)}) => {Subroutine.Stringify()}";
        public override string ToString() => $"({string.Join(", ", Signature.paramTypes)}) => {Subroutine}";
        public override object? ToCLR() => (IslProgram context) => Subroutine.Eval(context);
    }
    public class IslFunctionSignature
    {
        internal readonly string[] paramNames = [];
        internal readonly IslType[] paramTypes = [];
        public IslFunctionSignature(string[] paramNames, IslType[] paramTypes)
        {
            this.paramNames = paramNames;
            this.paramTypes = paramTypes;
            if (paramTypes.Length != paramNames.Length) throw new SyntaxError("Mismatched parameter name and type list.");
        }
    }
}
