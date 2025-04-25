using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISL.Runtime.Errors
{
    internal class IslError(string message) : Exception(message)
    {
        /// <summary>
        /// The line number of the error.
        /// </summary>
        uint LineNumber { get; } = 0;
        /// <summary>
        /// The column number of the error.
        /// </summary>
        uint ColumnNumber { get; } = 0;
    }
}
