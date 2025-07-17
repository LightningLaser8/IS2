using ISL.Language.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISL.Language.Operations
{
    internal interface IIslComparable
    {
        CompareResult CompareTo(IslValue other);
    }
    internal enum CompareResult
    {
        Equal,
        LessThan,
        GreaterThan,
        GenericNotEqual
    }
}
