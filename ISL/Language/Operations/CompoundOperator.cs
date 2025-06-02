using ISL.Language.Types;

namespace ISL.Language.Operations
{
    /// <summary>
    /// Represents an option between a binary and unary operator, such as '-'.
    /// </summary>
    internal class CompoundOperator(string id, BinaryOperator binary, UnaryOperator unary, int precedence) : BinaryOperator(id, (l, r, prog) => IslValue.Null, precedence)
    {
        public CompoundOperator(string id, BinaryOperator binary, UnaryOperator unary) : this(id, binary, unary, 0)
        { }

        public BinaryOperator BinaryOperator { get; } = binary;
        public UnaryOperator UnaryOperator { get; } = unary;
    }
}
