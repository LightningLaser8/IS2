using ISL.Interpreter;
using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal class ProgramAccessingBinaryOperator(string id, Func<IslValue, IslValue, IslProgram, IslValue> operate, int precedence) : BinaryOperator(id, (v, v2) => IslValue.Null, precedence)
    {
        /// <summary>
        /// Performs the operation.
        /// </summary>
        public new Func<IslValue, IslValue, IslProgram, IslValue> Operate { get; set; } = operate;
        public ProgramAccessingBinaryOperator(Func<IslValue, IslValue, IslProgram, IslValue> operate, int precedence) : this("", operate, precedence)
        {
        }
    }
}
