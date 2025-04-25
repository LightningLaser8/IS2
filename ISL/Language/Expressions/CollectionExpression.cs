using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Types;
using ISL.Language.Types.Collections;

namespace ISL.Language.Expressions
{
    internal class CollectionExpression : Expression
    {
        public List<Expression> expressions = [];
        public override IslValue Eval()
        {
            return new IslGroup() { Value = [.. expressions.Select((expr) => expr.Eval())] };
        }

        public override Expression Simplify()
        {
            return new CollectionExpression() { expressions = [.. expressions.Select((expr) => expr.Simplify())] };
        }

        public override string ToString()
        {
            return $"(Collection) {{{expressions.Aggregate("", (prev, curr) => $"{prev} {curr}")} }}";
        }
    }
}
