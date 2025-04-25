using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal interface IIslMultiplicable : IIslAddable
    {
        public IslValue Multiply(IslValue multiplier);
    }
}
