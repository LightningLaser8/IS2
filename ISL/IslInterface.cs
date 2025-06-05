using ISL.Interpreter;
using ISL.Language.Types;
using ISL.Runtime.Errors;

namespace ISL
{
    /// <summary>
    /// Class for interaction with ISL.
    /// </summary>
    public class IslInterface
    {
        private readonly IslInterpreter interpreter = new();
        /// <summary>
        /// Stores debug output of the interpreter during creation of the last program.
        /// </summary>
        public string LastDebug => debug;
        private string debug = "";
        /// <summary>
        /// Stores the last error the interpreter threw. An empty string is the last run was successful.
        /// </summary>
        public string ErrorMessage => error;
        private string error = "";
        /// <summary>
        /// True if the last run was successful, false if not.
        /// </summary>
        public bool Errored => errored;
        private bool errored = false;
        public IslInterface() { }
        /// <summary>
        /// Compiles an ISL program to a C# expression tree. This program can then be run repeatedly.<br/>
        /// May throw syntax errors.<br/>
        /// May throw type errors due to optimising code.
        /// </summary>
        /// <exception cref="IslError"></exception>
        /// <exception cref="SyntaxError"></exception>
        /// <exception cref="OverflowError"></exception>
        /// <exception cref="TypeError"></exception>
        /// <param name="source">The ISL code to run. Will throw a SyntaxError if invalid.</param>
        /// <returns>The result of the execution.</returns>
        public IslProgram CreateProgram(string source)
        {
            IslDebugOutput.Reset();
            IslProgram program;
            try
            {
                program = interpreter.CreateProgram(source);
                this.debug = IslDebugOutput.Message;
                IslDebugOutput.Reset();
                errored = false;
                error = "";
                return program;
            }
            catch (IslError e)
            {
                this.debug = IslDebugOutput.Message + "Error encountered!\n";
                IslDebugOutput.Reset();
                errored = true;
                error = e.GetType().Name + ": " + e.Message;
                throw;
            }
            catch (Exception e)
            {
                this.debug = IslDebugOutput.Message + "Internal error encountered!\n";
                IslDebugOutput.Reset();
                errored = true;
                error = e.Message;
                throw;
            }
        }


        public static IslType GetNativeIslType(string token)
        {
            return IslInterpreter.GetNativeType(token)().Type;
        }
        public static IslValue GetNativeIslValue(string token)
        {
            return IslInterpreter.GetNativeType(token)();
        }
    }
}
