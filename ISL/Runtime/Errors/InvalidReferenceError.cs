using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISL.Runtime.Errors
{
    internal class InvalidReferenceError(string message) : IslError(message)
    {
    }
}
