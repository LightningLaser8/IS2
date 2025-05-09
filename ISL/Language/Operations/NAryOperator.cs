using ISL.Language.Types;

namespace ISL.Language.Operations
{
    /// <summary>
    /// Represents an operator with an arbitrary number of operands.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="operate"></param>
    /// <param name="precedence"></param>
    internal class NAryOperator(string id, string[] separators, Func<IslValue[], IslValue> operate, int precedence) : Operator(id, () => IslValue.Null, precedence)
    {
        public NAryOperator(string id, string[] separators, Func<IslValue[], IslValue> operate) : this(id, separators, operate, 0) { }
        public NAryOperator(string[] separators, Func<IslValue[], IslValue> operate) : this("", separators, operate, 0) { }
        public string[] Separators { get; set; } = separators;
        public new Func<IslValue[], IslValue> Operate { get; set; } = operate;
    }
}
