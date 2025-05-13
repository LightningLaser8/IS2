namespace ISL.Runtime.Errors
{
    public class IslError(string message) : Exception(message)
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
