using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal class BinaryOperator(string id, Func<IslValue, IslValue, IslValue> operate) : Operator(id, () => IslValue.Null)
    {
        public BinaryOperator(string id, Func<IslValue, IslValue, IslValue> operate, int precedence) : this(id, operate)
        {
            Precedence = precedence;
        }
        public BinaryOperator(Func<IslValue, IslValue, IslValue> operate) : this("", operate)
        {
        }

        /// <summary>
        /// Performs the operation on two inputs.
        /// </summary>
        public new Func<IslValue, IslValue, IslValue> Operate { get; set; } = operate;
    }
}
