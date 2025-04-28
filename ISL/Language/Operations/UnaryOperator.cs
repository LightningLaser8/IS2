using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal class UnaryOperator(Func<string, bool> predicate, Func<IslValue, IslValue> operate) : Operator(predicate, () => IslValue.Null)
    {
        public UnaryOperator(Func<string, bool> predicate, Func<IslValue, IslValue> operate, int precedence) : this(predicate, operate)
        {
            Precedence = precedence;
        }
        /// <summary>
        /// Performs the operation on one input.
        /// </summary>
        public new Func<IslValue, IslValue> Operate { get; set; } = operate;
    }
}
