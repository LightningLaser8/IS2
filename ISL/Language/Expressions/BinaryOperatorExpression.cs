using ISL.Language.Operations;
using ISL.Language.Types;

namespace ISL.Language.Expressions
{
    internal class BinaryOperatorExpression : OperatorExpression
    {
        public new required BinaryOperator Operation { get; set; }
        public Expression? affectedL;
        public Expression? affectedR;
        public override IslValue Eval()
        {
            return Operation.Operate.Invoke(affectedL?.Eval() ?? IslValue.Null, affectedR?.Eval() ?? IslValue.Null);
        }
        public override string ToString()
        {
            return $"(Binary Operator) {value.Stringify()} on {{{affectedL?.ToString()}}} and {{{affectedR?.ToString()}}}";
        }
        public override Expression Simplify()
        {
            affectedL = affectedL?.Simplify();
            affectedR = affectedR?.Simplify();
            if (affectedL is ConstantExpression && affectedR is ConstantExpression)
            {
                return ConstantExpression.For(Operation.Operate.Invoke(affectedL.Eval(), affectedR.Eval()));
            }
            return this;
        }
        public override Operator? GetOp()
        {
            return this.Operation;
        }
    }
}
