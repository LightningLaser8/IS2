using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISL.Language.Types
{
    internal class IslErrorMessage : IslIdentifier
    {
        public static new IslErrorMessage FromString(string str)
        {
            return new IslErrorMessage() { Value = str };
        }
    }
}
