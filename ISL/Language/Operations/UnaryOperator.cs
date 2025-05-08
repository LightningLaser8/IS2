using ISL.Interpreter;
using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal class UnaryOperator(Func<string, bool> predicate, Func<IslValue, IslValue> operate, EvaluationDirection operandDirection = EvaluationDirection.Right) : Operator(predicate, () => IslValue.Null)
    {
        public UnaryOperator(Func<string, bool> predicate, Func<IslValue, IslValue> operate, int precedence, EvaluationDirection operandDirection = EvaluationDirection.Right) : this(predicate, operate, operandDirection)
        {
            Precedence = precedence;
        }
        public UnaryOperator(Func<IslValue, IslValue> operate) : this((s) => false, operate)
        {
        }
        /// <summary>
        /// Performs the operation on one input.
        /// </summary>
        public new Func<IslValue, IslValue> Operate { get; set; } = operate;
        internal EvaluationDirection OperandDirection { get; set; } = operandDirection;
    }
}
