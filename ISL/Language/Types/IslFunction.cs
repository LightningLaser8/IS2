using ISL.Interpreter;
using ISL.Language.Expressions;
using ISL.Runtime.Errors;

namespace ISL.Language.Types
{
    public class IslFunction : IslValue
    {
        private readonly Expression Subroutine = Expression.Null;
        public override IslType Type => IslType.Function;
        internal IslFunctionSignature Signature { get; private set; } = new("", [], []);
        public IslValue Call(IslProgram program, IslValue[] parameters)
        {
            if(parameters.Length != Signature.paramTypes.Length) throw new SyntaxError($"Function {Signature.name} expects {Signature.paramTypes.Length} arguments, got {parameters.Length}.");
            for (int i = 0; i < parameters.Length; i++)
            {
                program.CreateVariable($"{Signature.name}::{Signature.paramNames[i]}", Signature.paramTypes[i], parameters[i]);
            }
            var res = Subroutine.Eval(program);
            for (int i = 0; i < parameters.Length; i++)
            {
                program.DeleteVariable($"{Signature.name}::{Signature.paramNames[i]}");
            }
            return res;
        }
        public override string Stringify() => $"(function) => {Subroutine.Stringify()}";
        public override object? ToCLR() => (IslProgram context) => Subroutine.Eval(context);
    }
    internal class IslFunctionSignature
    {
        internal readonly string name = "";
        internal readonly string[] paramNames = [];
        internal readonly IslType[] paramTypes = [];
        public IslFunctionSignature(string name, string[] paramNames, IslType[] paramTypes)
        {
            this.name = name;
            this.paramNames = paramNames;
            this.paramTypes = paramTypes;
            if (paramTypes.Length != paramNames.Length) throw new SyntaxError("Mismatched parameter name and type list.");
        }
    }
}
