using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrate.ModContent.ISL
{
    /// <summary>
    /// An exception thrown by an executing (or compiling) ISL script.
    /// </summary>
    /// <param name="script"></param>
    /// <param name="error"></param>
    public class ScriptException(Script script, global::ISL.Runtime.Errors.IslError error) : Exception($"{error.GetType().Name} in {script.Location}: {error.Message}", error)
    {
    }
}
