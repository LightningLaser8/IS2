using ISL.Compiler;
using ISL.Language.Operations;
using ISL.Language.Types;
using ISL.Runtime.Errors;

namespace ISL.Language.Expressions
{
    internal class BinaryOperatorExpression : OperatorExpression
    {
        public new required BinaryOperator Operation { get; set; }
        public Expression? affectedL;
        public Expression? affectedR;
        public override IslValue Eval(IslProgram program)
        {
            if (Operation is ProgramAccessingBinaryOperator pao)
                return pao.Operate.Invoke(affectedL?.Eval(program) ?? IslValue.Null, affectedR?.Eval(program) ?? IslValue.Null, program);
            return Operation.Operate.Invoke(affectedL?.Eval(program) ?? IslValue.Null, affectedR?.Eval(program) ?? IslValue.Null);
        }
        public override string ToString()
        {
            return $"(Binary Operator) {value.Stringify()} on {{{affectedL?.ToString()}}} and {{{affectedR?.ToString()}}}";
        }
        public override Expression Simplify()
        {
            affectedL = affectedL?.Simplify();
            affectedR = affectedR?.Simplify();
            if (affectedL is ConstantExpression al && affectedR is ConstantExpression ar && Operation is not ProgramAccessingBinaryOperator)
            {
                return ConstantExpression.For(Operation.Operate.Invoke(al.Eval(), ar.Eval()));
            }
            return this;
        }
        public override Operator? GetOp()
        {
            return this.Operation;
        }
        public override void Validate()
        {
            affectedL?.Validate();
            affectedR?.Validate();
            if (affectedL is null) throw new SyntaxError($"Binary operator {value.Stringify()} requires 2 inputs, left operand is missing!");
            if (affectedR is null) throw new SyntaxError($"Binary operator {value.Stringify()} requires 2 inputs, right operand is missing!");
        }

        public override string Stringify() => affectedL?.Stringify() + " " + value.Stringify() + " " + affectedR?.Stringify();
    }
}
