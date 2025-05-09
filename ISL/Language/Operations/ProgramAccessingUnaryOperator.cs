using ISL.Interpreter;
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
    internal class ProgramAccessingUnaryOperator(string id, Func<IslValue, IslProgram, IslValue> operate, int precedence) : UnaryOperator(id, (v) => IslValue.Null, precedence)
    {
        /// <summary>
        /// Performs the operation.
        /// </summary>
        public new Func<IslValue, IslProgram, IslValue> Operate { get; set; } = operate;
        public ProgramAccessingUnaryOperator(Func<IslValue, IslProgram, IslValue> operate, int precedence) : this("", operate, precedence)
        {
        }
    }
}
