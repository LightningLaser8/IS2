using ISL.Interpreter;
using ISL.Language.Expressions;
using ISL.Language.Types;
using ISL.Runtime.Errors;

namespace ISL.Language.Variables
{
    /// <summary>
    /// Creates a variable.
    /// </summary>
    internal class VariableDeclarationExpr : Expression
    {
        public bool IsReadOnly { get; set; } = false;
        public bool IsTypeInferred { get; set; } = false;
        public bool IsTypeImplied { get; set; } = false;

        public string name = "";
        public IslType varType = IslType.Null;

        public IslValue initialValue = IslValue.Null;
        public override IslValue Eval(IslProgram program)
        {
            var vari = initialValue == IslValue.Null ? program.CurrentScope.CreateVariable(name, IsTypeImplied ? IslType.Null : varType) : program.CurrentScope.CreateVariable(name, varType, initialValue);
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
        public override string Stringify() => $@"{(IsReadOnly ? "const " : IsTypeImplied ? "imply " : "")}{(IsTypeInferred ? "infer" : varType)} {name}";
        public override void Validate()
        {
            if (IsTypeImplied && IsTypeInferred) throw new SyntaxError("A variable declaration cannot be both type- implied and inferred");
            if (IsTypeImplied && IsReadOnly) throw new SyntaxError("A variable declaration cannot be type-implied and readonly");
            if (IsTypeInferred && IsReadOnly) throw new SyntaxError("A variable declaration cannot be type-inferred and readonly");
        }
        public override bool Equals(Expression? other) => other is VariableDeclarationExpr ib && ib.name == name; //Name's all that matters
    }
}
