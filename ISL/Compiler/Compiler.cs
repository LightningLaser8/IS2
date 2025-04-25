using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Expressions;
using ISL.Language.Types;
using ISL.Runtime.Errors;
using ISL.Compiler;

namespace ISL.Compiler
{
    internal sealed partial class IslCompiler
    {
        public IslCompiler()
        {
            InitOperators();
            InitBrackets();
        }

        public bool debugMode = false;

        public List<IslProgram> programs = [];

        //Stuff to clean up after runtime
        string source = "";
        readonly List<string> tokens = [];
        readonly List<Expression> expressions = [];
        readonly List<Expression> code = [];
        internal string output = "";

        readonly Dictionary<string, string> metas = [];

        //Entry point

        public IslValue CompileAndRun(string sourceCode, out IslProgram program)
        {
            program = Compile(sourceCode);
            return program.Execute();
        }

        public IslProgram Compile(string sourceCode)
        {
            // Initialize the interpreter
            source = sourceCode;
            output = "";
            if (source.Length == 0)
            {
                throw new SyntaxError("No code input!");
            }

            // Lexical analysis
            Debug("\nLexical Analysis: \n");
            RemoveComments();
            if (source.Length == 0)
            {
                throw new SyntaxError("Only comments input!");
            }
            Debug("Getting metadata:");
            ProcessMetadata();
            Tokenise();
            Debug("  " + tokens.Count.ToString() + " tokens:\n" +
                "  " + tokens.Select(x => $"'{x}'").Aggregate((prev, curr) => prev + ", " + curr));
            // Syntax analysis
            // Code generation
            Debug("\nSyntax Analysis and Code Generation:\n");
            Parse();
            Debug("  >> " + expressions.Count.ToString() + " code points: \n  " + expressions.Aggregate<Expression, string>("", (x, y) => x + y.ToString() + ", "));
            // Code Optimisation
            Debug("\nCode Optimisation:\n");
            Optimise();
            Debug("  >- " + code.Count.ToString() + " final code points: \n  " + code.Aggregate<Expression, string>("", (x, y) => x + y.ToString() + ", "));
            Debug("\nCompilation Complete!\n");

            var program = new IslProgram(CloneList(code), CloneDict(metas));

            //Clean up
            source = "";
            tokens.Clear();
            expressions.Clear();
            code.Clear();
            metas.Clear();
            return program;
        }

        private static List<T> CloneList<T>(List<T> list)
        {
            var lst = new List<T>();
            lst.AddRange(list);
            return lst;
        }
        private static Dictionary<K, V> CloneDict<K, V>(Dictionary<K, V> dict) where K : notnull
        {
            return new Dictionary<K, V>(dict);
        }
    }
}
