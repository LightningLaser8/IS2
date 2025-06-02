using ISL.Runtime.Errors;
namespace Integrate.ModContent.ISL
{
    /// <summary>
    /// An exception thrown by an executing (or compiling) ISL script.
    /// </summary>
    /// <param name="script"></param>
    /// <param name="error"></param>
    public sealed class ScriptException(Script script, IslError error) : Exception($"{error.GetType().Name} in {script.Location}: {error.Message}", error)
    {
    }
}
