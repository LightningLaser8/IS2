using ISL.Compiler;
using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal class ProgramAccessingUnaryOperator(Func<string, bool> predicate, Func<IslValue, IslProgram, IslValue> operate, int precedence) : UnaryOperator(predicate, (v) => IslValue.Null, precedence)
    {
        /// <summary>
        /// Performs the operation.
        /// </summary>
        public new Func<IslValue, IslProgram, IslValue> Operate { get; set; } = operate;
    }
    internal class ProgramAccessingBinaryOperator(Func<string, bool> predicate, Func<IslValue, IslValue, IslProgram, IslValue> operate, int precedence) : BinaryOperator(predicate, (v, v2) => IslValue.Null, precedence)
    {
        /// <summary>
        /// Performs the operation.
        /// </summary>
        public new Func<IslValue, IslValue, IslProgram, IslValue> Operate { get; set; } = operate;
    }
}
