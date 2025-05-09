using ISL.Interpreter;
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
        /// Stores debug output of the compiler.
        /// </summary>
        public string LastDebug => debug;
        private string debug = "";
        /// <summary>
        /// Stores debug output of the compiler.
        /// </summary>
        public string CompilerDebug => interpreter.debug;
        /// <summary>
        /// Stores the last error the compiler threw. An empty string is the last run was successful.
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
        public IslProgram CreateProgram(string source, bool debug = false)
        {
            IslProgram program;
            interpreter.debugMode = debug;
            try
            {
                program = interpreter.CreateProgram(source);
                this.debug = interpreter.debug;
                interpreter.debug = "";
                errored = false;
                error = "";
                return program;
            }
            catch (IslError e)
            {
                this.debug = interpreter.debug + "Error encountered!\n";
                interpreter.debug = "";
                errored = true;
                error = e.GetType().Name + ": " + e.Message;
                throw;
            }
            catch (Exception e)
            {
                this.debug = interpreter.debug + "Internal error encountered!\n";
                interpreter.debug = "";
                errored = true;
                error = e.Message;
                throw;
            }
        }
    }
}
