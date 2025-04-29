using ISL.Language.Types;
using ISL.Runtime.Errors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISL.Language.Expressions
{
    internal class TokenExpression : IdentifierExpression
    {
        public override IslIdentifier Eval()
        {
            throw new SyntaxError("Unexpected token '"+value+"'");
        }
        public override Expression Simplify()
        {
            throw new SyntaxError("Unexpected token '" + value + "'");
        }
        public override string ToString()
        {
            return $"(Token) {value.Stringify()}";
        }
    }
}
