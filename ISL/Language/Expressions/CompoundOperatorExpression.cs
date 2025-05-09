using ISL.Interpreter;
using ISL.Language.Operations;
using ISL.Language.Types;
using ISL.Runtime.Errors;

namespace ISL.Language.Expressions
{
    internal class CompoundOperatorExpression : OperatorExpression
    {
        public new required CompoundOperator Operation { get; set; }
        public Expression? affectedOptional;
        public Expression? affectedRequired;
        private bool isNotBinary = false;
        private bool resolved = false;
        public override string ToString()
        {
            return resolved
                ?
                    isNotBinary
                    ? $"(<Resolved> Unary Operator) {value.Stringify()} on {{{affectedRequired?.ToString()}}}"
                    : $"(<Resolved> Binary Operator) {value.Stringify()} on {{{affectedOptional?.ToString()}}} and {{{affectedRequired?.ToString()}}}"
                : $"(<Unresolved> Unary/Binary Operator) {value.Stringify()} on {{{affectedOptional?.ToString()}}} and {{{affectedRequired?.ToString()}}}";
        }

        public override IslValue Eval(IslProgram program)
        {
            Resolve();
            if (isNotBinary)
            {
                if (Operation.UnaryOperator is ProgramAccessingUnaryOperator pao)
                    return pao.Operate.Invoke(affectedRequired?.Eval(program) ?? IslValue.Null, program);
                return Operation.UnaryOperator.Operate.Invoke(affectedRequired?.Eval(program) ?? IslValue.Null);
            }
            else
            {
                if (Operation.BinaryOperator is ProgramAccessingBinaryOperator pao)
                    return pao.Operate.Invoke(affectedOptional?.Eval(program) ?? IslValue.Null, affectedRequired?.Eval(program) ?? IslValue.Null, program);
                return Operation.BinaryOperator.Operate.Invoke(affectedOptional?.Eval(program) ?? IslValue.Null, affectedRequired?.Eval(program) ?? IslValue.Null);
            }
        }
        public override Expression Simplify()
        {
            Resolve();
            affectedOptional = affectedOptional?.Simplify();
            affectedRequired = affectedRequired?.Simplify();
            if (isNotBinary)
            {
                if (affectedRequired is ConstantExpression ar && Operation.UnaryOperator is not ProgramAccessingUnaryOperator)
                {
                    return ConstantExpression.For(Operation.UnaryOperator.Operate.Invoke(ar.Eval()));
                }
            }
            else
            {
                if (affectedOptional is ConstantExpression al && affectedRequired is ConstantExpression ar && Operation.BinaryOperator is not ProgramAccessingBinaryOperator)
                {
                    return ConstantExpression.For(Operation.BinaryOperator.Operate.Invoke(al.Eval(), ar.Eval()));
                }
            }
            return this;
        }
        private void Resolve()
        {
            if (resolved) return;
            if (affectedOptional is null)
                isNotBinary = true;
            resolved = true;
        }
        public override void Validate()
        {
            Resolve();
            affectedOptional?.Validate();
            affectedRequired?.Validate();
            if (affectedRequired is null) throw new SyntaxError($"Operator {value.Stringify()} requires at least one input, (right) operand is missing!");
        }
        public override Operator? GetOp()
        {
            return resolved ? (isNotBinary ? Operation.UnaryOperator : Operation.BinaryOperator) : Operation;
        }
        public override string Stringify() => resolved ? (isNotBinary ? $"{value.Stringify()} {affectedRequired?.Stringify()}" : $"{affectedOptional?.Stringify()} {value.Stringify()} {affectedRequired?.Stringify()}") : $"({affectedOptional?.Stringify()})? {value.Stringify()} {affectedRequired?.Stringify()}";
    }
}
