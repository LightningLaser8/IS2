namespace ISL.Interpreter
{
    internal partial class IslInterpreter
    {
        /// <summary>
        /// Includes anything that would be used as a separator or *closing* bracket. Opening/symmetrical brackets are not needed.
        /// </summary>
        public readonly string[] Tokens = [",", ";", ")", "]", "\\", "}", ":", "?"];
    }
}
