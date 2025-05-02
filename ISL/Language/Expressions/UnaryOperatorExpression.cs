using ISL.Compiler;
using ISL.Language.Operations;
using ISL.Language.Types;
using ISL.Runtime.Errors;

namespace ISL.Language.Expressions
{
    internal class UnaryOperatorExpression : OperatorExpression
    {
        public new required UnaryOperator Operation { get; set; }
        public Expression? affected;
        public override IslValue Eval(IslProgram program)
        {
            if (Operation is ProgramAccessingUnaryOperator pao)
                return pao.Operate.Invoke(affected?.Eval(program) ?? IslValue.Null, program);
            return Operation.Operate.Invoke(affected?.Eval(program) ?? IslValue.Null);
        }
        public override string ToString()
        {
            return $"(Unary Operator) {value.Stringify()} on {{{affected?.ToString()}}}";
        }
        public override Expression Simplify()
        {
            affected = affected?.Simplify();
            if (affected is ConstantExpression a && Operation is not ProgramAccessingUnaryOperator)
            {
                return ConstantExpression.For(Operation.Operate.Invoke(a.Eval()));
            }
            return this;
        }

        public override Operator? GetOp()
        {
            return this.Operation;
        }
        public override void Validate()
        {
            affected?.Validate();
            if (affected is null) throw new SyntaxError($"Unary operator {value.Stringify()} requires an input, operand is missing!");
        }
        public override string Stringify() => $"{value.Stringify()} {affected?.Stringify()}";
    }
}
