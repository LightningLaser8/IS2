using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Types;

namespace ISL.Language.Expressions
{
    internal class BoolExpression : ConstantExpression
    {
        public IslBool value = false;
        public override IslValue Eval()
        {
            return value;
        }

        public override string ToString()
        {
            return $"(Bool) {value.Stringify()}";
        }
        public override string Stringify() => value ? "true" : "false";
    }
}
