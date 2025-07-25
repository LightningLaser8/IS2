﻿using ISL.Interpreter;
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
                return Operation.UnaryOperator.Operate.Invoke(affectedRequired ?? Null, program);
            else
                return Operation.BinaryOperator.Operate.Invoke(affectedOptional ?? Null, affectedRequired ?? Null, program);
        }
        public override Expression Simplify()
        {
            Resolve();
            affectedOptional = affectedOptional?.Simplify();
            affectedRequired = affectedRequired?.Simplify();
            if (isNotBinary)
            {
                if (affectedRequired is ConstantExpression ar && Operation.UnaryOperator.IsFoldable)
                {
                    return ConstantExpression.For(Operation.UnaryOperator.Operate.Invoke(ar, null));
                }
            }
            else
            {
                if (affectedOptional is ConstantExpression al && affectedRequired is ConstantExpression ar && Operation.BinaryOperator.IsFoldable)
                {
                    return ConstantExpression.For(Operation.BinaryOperator.Operate.Invoke(al, ar, null));
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
            if (Operation.ValidatesExprs)
            {
                affectedOptional?.Validate();
                affectedRequired?.Validate();
            }
            if (affectedRequired is null) throw new SyntaxError($"Operator {value.Stringify()} requires at least one input, (right) operand is missing!");
        }
        public override Operator? GetOp()
        {
            return resolved ? (isNotBinary ? Operation.UnaryOperator : Operation.BinaryOperator) : Operation;
        }
        public override string Stringify() => resolved ? (isNotBinary ? $"{value.Stringify()} {affectedRequired?.Stringify()}" : $"{affectedOptional?.Stringify()} {value.Stringify()} {affectedRequired?.Stringify()}") : $"({affectedOptional?.Stringify()})? {value.Stringify()} {affectedRequired?.Stringify()}";
        public override bool Equals(Expression? other)
            => other is CompoundOperatorExpression ib && ib.Operation == Operation && (ib.affectedOptional?.Equals(affectedOptional) ?? false) && (ib.affectedRequired?.Equals(affectedRequired) ?? false)
            //Honestly, I don't think i really need this, but here it is anyway
            || (resolved && !isNotBinary && other is BinaryOperatorExpression be && be.Operation == Operation.BinaryOperator && (be.affectedL?.Equals(affectedOptional) ?? false) && (be.affectedR?.Equals(affectedRequired) ?? false))
            //Couldn't add binaries without unaries too
            || (resolved && isNotBinary && other is UnaryOperatorExpression ue && ue.Operation == Operation.UnaryOperator && (ue.affected?.Equals(affectedRequired) ?? false));
    }
}
