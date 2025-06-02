using ISL.Interpreter;
using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal class Operator(string id, Func<IslProgram, IslValue> operate)
    {
        public Operator(string id, Func<IslProgram, IslValue> operate, int precedence) : this(id, operate)
        {
            Precedence = precedence;
        }
        /// <summary>
        /// Determines the order in which operators are evaluated. <br/>
        /// Higher precedence means evaluated sooner, e.g. addition precedence &lt; multiplication precedence.
        /// </summary>
        public int Precedence { get; protected set; }

        public string id = id;

        /// <summary>
        /// Performs the operation.
        /// </summary>
        public Func<IslProgram, IslValue> Operate { get; init; } = operate;
        public bool AutoSplit { get; init; } = false;
        /// <summary>
        /// Controls whether or not this operation supports constant folding.
        /// </summary>
        public bool IsFoldable { get; init; } = true;
        public bool ValidatesExprs { get; init; } = true;
    }
}
