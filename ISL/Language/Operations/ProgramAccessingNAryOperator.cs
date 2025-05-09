using ISL.Interpreter;
using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal class ProgramAccessingNAryOperator(string id, string[] separators, Func<IslValue[], IslProgram, IslValue> operate, int precedence) : NAryOperator(id, separators, (vs) => IslValue.Null, precedence)
    {
        /// <summary>
        /// Performs the operation.
        /// </summary>
        public new Func<IslValue[], IslProgram, IslValue> Operate { get; set; } = operate;
        public ProgramAccessingNAryOperator(string[] separators, Func<IslValue[], IslProgram, IslValue> operate, int precedence) : this("", separators, operate, precedence)
        {
        }
    }
}
