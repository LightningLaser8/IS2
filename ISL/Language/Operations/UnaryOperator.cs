using ISL.Interpreter;
using ISL.Language.Expressions;
using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal class UnaryOperator(string id, Func<Expression, IslProgram?, IslValue> operate, EvaluationDirection operandDirection = EvaluationDirection.Right) : Operator(id, (p) => IslValue.Null)
    {
        public UnaryOperator(string id, Func<Expression, IslProgram?, IslValue> operate, int precedence, EvaluationDirection operandDirection = EvaluationDirection.Right) : this(id, operate, operandDirection)
        {
            Precedence = precedence;
        }
        public UnaryOperator(Func<Expression, IslProgram?, IslValue> operate) : this("", operate)
        {
        }
        /// <summary>
        /// Performs the operation on one input.
        /// </summary>
        public new Func<Expression, IslProgram?, IslValue> Operate { get; set; } = operate;
        internal EvaluationDirection OperandDirection { get; set; } = operandDirection;
    }
}
