using ISL.Interpreter;
using ISL.Language.Expressions;
using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal class BinaryOperator(string id, Func<Expression, Expression, IslProgram?, IslValue> operate) : Operator(id, (p) => IslValue.Null)
    {
        public BinaryOperator(string id, Func<Expression, Expression, IslProgram?, IslValue> operate, int precedence) : this(id, operate)
        {
            Precedence = precedence;
        }
        public BinaryOperator(Func<Expression, Expression, IslProgram?, IslValue> operate) : this("", operate)
        {
        }

        /// <summary>
        /// Performs the operation on two inputs.
        /// </summary>
        public new Func<Expression, Expression, IslProgram?, IslValue> Operate { get; set; } = operate;
    }
}
