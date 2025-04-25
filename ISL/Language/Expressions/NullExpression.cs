using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Types;

namespace ISL.Language.Expressions
{
    internal class NullExpression : ConstantExpression
    {
        public override IslNull Eval()
        {
            return new IslNull();
        }
        public override string ToString()
        {
            return "(Null) null";
        }
    }
}
