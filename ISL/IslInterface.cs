using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Types;
using ISL.Runtime.Errors;
using ISL.Compiler;

namespace ISL
{
    /// <summary>
    /// Class for interaction with ISL.
    /// </summary>
    public class IslInterface
    {
        private readonly IslCompiler compiler = new();
        /// <summary>
        /// Stores debug output of the compiler.
        /// </summary>
        public string LastDebug => debug;
        private string debug = "";
        /// <summary>
        /// Stores debug output of the compiler.
        /// </summary>
        public string CompilerDebug => compiler.debug;
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
        public IslProgram Compile(string source, bool debug = false)
        {
            IslProgram program;
            compiler.debugMode = debug;
            try
            {
                program = compiler.Compile(source);
                this.debug = compiler.debug;
                compiler.debug = "";
                errored = false;
                error = "";
                return program;
            }
            catch (IslError e)
            {
                this.debug = compiler.debug + "Error encountered!\n";
                compiler.debug = "";
                errored = true;
                error = e.GetType().Name + ": " + e.Message;
                throw;
            }
            catch (Exception e)
            {
                this.debug = compiler.debug + "Internal error encountered!\n";
                compiler.debug = "";
                errored = true;
                error = e.Message;
                throw;
            }
        }
    }
}
