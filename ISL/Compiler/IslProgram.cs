using ISL.Runtime.Errors;
using ISL.Language.Expressions;
using ISL.Language.Types;

namespace ISL.Compiler
{
    public class IslProgram
    {
        /// <summary>
        /// Checks if the program has a specified metadata key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>True if the tag was present, false if the tag was not present.</returns>
        public bool HasMeta(string key)
        {
            return meta.ContainsKey(key);
        }
        /// <summary>
        /// Gets the value associated with the specified metadata key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The value of the metadata tag, or an empty string if the tag was not present.</returns>
        public string GetMeta(string key) {
            try
            {
                return meta[key];
            }
            catch (Exception)
            {
                return "";
            }
        }
        private readonly Dictionary<string, string> meta = [];

        internal IslProgram(List<Expression> code, Dictionary<string, string> meta)
        {
            codePoints = code;
            this.meta = meta;
        }
        internal IslProgram(List<Expression> code)
        {
            codePoints = code;
        }
        private readonly List<Expression> codePoints = [];

        public IslValue LastResult => result;
        private IslValue result = IslValue.Null;
        /// <summary>
        /// Executes the ISL program.
        /// </summary>
        /// <returns>The final return value of the program.</returns>
        /// <exception cref="IslError"/>
        /// <exception cref="SyntaxError"/>
        /// <exception cref="TypeError"/>
        /// <exception cref="OverflowError"/>
        public IslValue Execute()
        {
            foreach (var point in codePoints)
            {
                result = point.Eval();
            }
            return result;
        }
        /// <summary>
        /// Executes the ISL program. Will not throw errors.
        /// </summary>
        /// <returns>The final return value of the program. Can be a string error message.</returns>
        public IslValue SafeExecute()
        {
            try
            {
                result = this.Execute();
                return result;
            }
            catch (Exception e)
            {
                result = IslErrorMessage.FromString(e.GetType().Name + ": " + e.Message);
                return result;
            }
        }
    }
}
