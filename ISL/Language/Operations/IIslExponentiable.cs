using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal interface IIslExponentiable : IIslMultiplicable
    {
        public IslValue Exponentiate(IslValue exponent);
    }
}
