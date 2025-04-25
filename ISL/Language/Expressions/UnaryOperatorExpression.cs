using ISL.Language.Operations;
using ISL.Language.Types;

namespace ISL.Language.Expressions
{
    internal class UnaryOperatorExpression : OperatorExpression
    {
        public new required UnaryOperator Operation { get; set; }
        public Expression? affected;
        public override IslValue Eval()
        {
            return Operation.Operate.Invoke(affected?.Eval() ?? IslValue.Null);
        }
        public override string ToString()
        {
            return $"(Unary Operator) {value.Stringify()} on {{{affected?.ToString()}}}";
        }
        public override Expression Simplify()
        {
            affected = affected?.Simplify();
            if (affected is ConstantExpression)
            {
                return ConstantExpression.For(Operation.Operate.Invoke(affected.Eval()));
            }
            return this;
        }

        public override Operator? GetOp()
        {
            return this.Operation;
        }
    }
}
