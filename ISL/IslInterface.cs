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
    public class IslInterface
    {
        private readonly IslCompiler compiler = new();
        public string LastOutput => output;
        private string output = "";
        public IslValue LastResult => result;
        private IslValue result = IslValue.Null;

        public bool Errored => errored;
        private bool errored = false;
        public IslInterface() { }
        /// <summary>
        /// Compiles an ISL program to a C# expression tree. This program can then be run repeatedly.<br/>
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
                output = compiler.output;
                errored = false;
                return program;
            }
            catch (Exception)
            {
                output = compiler.output + "Error encountered!\n";
                errored = true;
                throw;
            }
        }
        /// <summary>
        /// Executes some ISL code. Returns a string with the output of the code. Will not throw ISL errors.<br/>
        /// Will actually compile the program fully, then run it, so it is not recommended to use this method repeatedly. Instead, use Compile(), and repeatedly Execute() the program.<br/>
        /// Returns an IslErrorMessage is an error occurred in the code.
        /// </summary>
        /// <param name="sourceCode">The ISL code to run.</param>
        /// <returns>The result of the execution.</returns>
        public IslValue CompileAndExecute(string sourceCode, out IslProgram? program, bool debug = false)
        {
            compiler.debugMode = debug;
            try
            {
                result = compiler.CompileAndRun(sourceCode, out program);
                output = compiler.output;
                errored = false;
                return result;
            }
            catch (Exception e)
            {
                result = IslErrorMessage.FromString(e.GetType().Name + ": " + e.Message);
                output = compiler.output + "Error encountered!\n";
                errored = true;
                program = null;
                return result;
            }
        }
    }
}
