using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal class BinaryOperator(Func<string, bool> predicate, Func<IslValue, IslValue, IslValue> operate) : Operator(predicate, () => IslValue.Null)
    {
        public BinaryOperator(Func<string, bool> predicate, Func<IslValue, IslValue, IslValue> operate, int precedence) : this(predicate, operate)
        {
            Precedence = precedence;
        }
        public BinaryOperator(Func<IslValue, IslValue, IslValue> operate) : this((s) => false, operate)
        {
        }

        /// <summary>
        /// Performs the operation on two inputs.
        /// </summary>
        public new Func<IslValue, IslValue, IslValue> Operate { get; set; } = operate;
    }
}
