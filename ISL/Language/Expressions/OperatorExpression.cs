using ISL.Compiler;
using ISL.Language.Operations;
using ISL.Language.Types;

namespace ISL.Language.Expressions
{
    internal class OperatorExpression : Expression
    {
        public IslIdentifier value = "";
        public Operator? Operation { get; set; }
        public override IslValue Eval(IslProgram program)
        {
            return Operation?.Operate.Invoke() ?? IslValue.Null;
        }
        public override string ToString()
        {
            return $"(Operator) {value.Stringify()}";
        }

        public override Expression Simplify()
        {
            return this;
        }
        public virtual Operator? GetOp()
        {
            return this.Operation;
        }
        public override string Stringify() => $"{value.Stringify()}";
    }
}
