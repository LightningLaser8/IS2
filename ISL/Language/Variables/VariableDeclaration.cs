using ISL.Interpreter;
using ISL.Language.Expressions;
using ISL.Language.Types;
using ISL.Runtime.Errors;

namespace ISL.Language.Variables
{
    /// <summary>
    /// Creates a variable.
    /// </summary>
    internal class VariableDeclaration : Expression
    {
        public bool IsReadOnly { get; set; } = false;
        public bool IsTypeInferred { get; set; } = false;
        public bool IsTypeImplied { get; set; } = false;

        public string name = "";
        public IslType varType = IslType.Null;

        public IslValue initialValue = IslValue.Null;
        public override IslValue Eval(IslProgram program)
        {
            var vari = initialValue == IslValue.Null ? program.CreateVariable(name, varType) : program.CreateVariable(name, varType, initialValue);
            vari.ImpliedType = IsTypeImplied;
            vari.ReadOnly = IsReadOnly;
            vari.InferType = IsTypeInferred;
            return vari;
        }
        public override Expression Simplify()
        {
            return this;
        }
        public override string ToString()
        {
            return Stringify();
        }
        public override string Stringify() => $@"{(IsReadOnly ? "const " : "")}{(IsTypeInferred ? "imply " : "")}{(IsTypeInferred ? "infer" : varType)} {name}";
        public override void Validate()
        {
            base.Validate();
            if (IsTypeImplied && IsTypeInferred) throw new SyntaxError("A variable declaration cannot be both implied and inferred");
        }
    }
}
