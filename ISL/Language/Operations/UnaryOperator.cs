using ISL.Interpreter;
using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal class UnaryOperator(string id, Func<IslValue, IslValue> operate, EvaluationDirection operandDirection = EvaluationDirection.Right) : Operator(id, () => IslValue.Null)
    {
        public UnaryOperator(string id, Func<IslValue, IslValue> operate, int precedence, EvaluationDirection operandDirection = EvaluationDirection.Right) : this(id, operate, operandDirection)
        {
            Precedence = precedence;
        }
        public UnaryOperator(Func<IslValue, IslValue> operate) : this("", operate)
        {
        }
        /// <summary>
        /// Performs the operation on one input.
        /// </summary>
        public new Func<IslValue, IslValue> Operate { get; set; } = operate;
        internal EvaluationDirection OperandDirection { get; set; } = operandDirection;
    }
}
