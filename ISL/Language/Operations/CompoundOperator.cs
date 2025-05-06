using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Types;

namespace ISL.Language.Operations
{
    /// <summary>
    /// Represents an option between a binary and unary operator, such as '-'.
    /// </summary>
    internal class CompoundOperator(Func<string, bool> predicate, BinaryOperator binary, UnaryOperator unary, int precedence) : BinaryOperator(predicate, (l, r) => IslValue.Null, precedence)
    {
        public CompoundOperator(Func<string, bool> predicate, BinaryOperator binary, UnaryOperator unary) : this(predicate, binary, unary, 0)
        { }

        public BinaryOperator BinaryOperator { get; } = binary;
        public UnaryOperator UnaryOperator { get; } = unary;
    }
}
