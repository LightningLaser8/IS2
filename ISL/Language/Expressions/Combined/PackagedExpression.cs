using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Types;

namespace ISL.Language.Expressions.Combined
{
    /// <summary>
    /// An expression wrapped by some brackets.
    /// </summary>
    internal class PackagedExpression : Expression
    {
        public required Expression expression;
        public override IslValue Eval()
        {
            return expression.Eval();
        }

        public override Expression Simplify()
        {
            return expression.Simplify();
        }
        public override string ToString()
        {
            return $"(( {expression} ))";
        }
    }
}
