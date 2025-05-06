using ISL.Compiler;
using ISL.Language.Types;

namespace ISL.Language.Operations
{
    /// <summary>
    /// Operator which accesses the program's current state.<br/>
    /// Cannot be optimised, as the compiler doesn't know what the operator will do with the program.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="operate"></param>
    /// <param name="precedence"></param>
    internal class ProgramAccessingUnaryOperator(Func<string, bool> predicate, Func<IslValue, IslProgram, IslValue> operate, int precedence) : UnaryOperator(predicate, (v) => IslValue.Null, precedence)
    {
        /// <summary>
        /// Performs the operation.
        /// </summary>
        public new Func<IslValue, IslProgram, IslValue> Operate { get; set; } = operate;
        public ProgramAccessingUnaryOperator(Func<IslValue, IslProgram, IslValue> operate, int precedence) : this((s) => false, operate, precedence)
        {
        }
    }
    internal class ProgramAccessingBinaryOperator(Func<string, bool> predicate, Func<IslValue, IslValue, IslProgram, IslValue> operate, int precedence) : BinaryOperator(predicate, (v, v2) => IslValue.Null, precedence)
    {
        /// <summary>
        /// Performs the operation.
        /// </summary>
        public new Func<IslValue, IslValue, IslProgram, IslValue> Operate { get; set; } = operate;
        public ProgramAccessingBinaryOperator(Func<IslValue, IslValue, IslProgram, IslValue> operate, int precedence) : this((s) => false, operate, precedence)
        {
        }
    }
}
