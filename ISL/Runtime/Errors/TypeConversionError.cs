using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISL.Runtime.Errors
{
    internal class TypeConversionError(string typeFrom, string typeTo) : IslError($"Cannot convert {typeFrom} to {typeTo}." +
            $" Please check the type of the value you are trying to convert.")
    {
        public string TypeFrom { get; } = typeFrom;
        public string TypeTo { get; } = typeTo;
    }
}
