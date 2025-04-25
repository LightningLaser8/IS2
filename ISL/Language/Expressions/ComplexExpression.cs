using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Types;

namespace ISL.Language.Expressions
{
    internal class ComplexExpression : ConstantExpression
    {
        public IslComplex value = new();
        public override IslValue Eval()
        {
            return value;
        }
        public override string ToString()
        {
            return $"(Complex) {value.Value.Real} + {value.Value.Imaginary}i";
        }
    }
}
