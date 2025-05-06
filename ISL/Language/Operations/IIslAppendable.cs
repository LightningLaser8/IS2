using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal interface IIslAppendable
    {
        public void Append(IslValue value);
    }
}
