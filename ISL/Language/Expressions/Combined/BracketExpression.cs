using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Types;
using ISL.Runtime.Errors;

namespace ISL.Language.Expressions.Combined
{
    /// <summary>
    /// Literally just a container for expressions. May as well not exist.<br/>
    /// Not sure what you were expecting.
    /// </summary>
    internal class BracketExpression : Expression
    {
        public required BracketType bracket;
        public override IslValue Eval()
        {
            throw new SyntaxError("Unresolved bracket expression!");
        }
        public override Expression Simplify()
        {
            throw new SyntaxError("Unresolved bracket expression!");
        }
    }
}
