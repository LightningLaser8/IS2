using ISL.Language.Expressions;
using ISL.Language.Types;
using ISL.Runtime.Errors;

namespace ISL.Interpreter
{
    internal sealed partial class IslInterpreter
    {
        public IslInterpreter()
        {
            InitOperators();
            InitKeywords();
            InitBrackets();
        }

        public bool debugMode = false;

        //Stuff to clean up after runtime
        string source = "";
        readonly List<string> tokens = [];
        readonly List<Expression> expressions = [];
        readonly List<Expression> code = [];
        internal string debug = "";

        readonly Dictionary<string, string> metas = [];

        //Entry point

        public IslValue Run(string sourceCode, out IslProgram program)
        {
            program = CreateProgram(sourceCode);
            return program.Execute();
        }

        public IslProgram CreateProgram(string sourceCode)
        {
            // Initialize the interpreter
            source = sourceCode.ReplaceLineEndings("\n");
            debug = "";
            if (source.Length == 0)
            {
                throw new SyntaxError("No code input!");
            }

            // Lexical analysis
            IslDebugOutput.Debug("\nLexical Analysis: \n");
            RemoveComments();
            if (source.Length == 0)
            {
                throw new SyntaxError("Only comments input!");
            }
            IslDebugOutput.Debug("Getting metadata:");
            ProcessMetadata();
            IslDebugOutput.Debug("Replacing placeholders:");
            DestandardiseBrackets();
            IslDebugOutput.Debug("Tokenising:");
            Tokenise();
            if (tokens.Count > 0)
                IslDebugOutput.Debug("  " + tokens.Count.ToString() + " tokens:\n" +
                    "  " + string.Join(", ", tokens.Select(x => $"'{x}'")));
            else IslDebugOutput.Debug("No tokens.");
            // Syntax analysis
            // Code generation
            IslDebugOutput.Debug("\nSyntax Analysis and Code Generation:\n");
            Parse();
            IslDebugOutput.Debug("  >> " + expressions.Count.ToString() + " code points: \n  " + string.Join(", ", expressions.Select(x => x.ToString())));
            // Code Optimisation
            IslDebugOutput.Debug("\nCode Optimisation:\n");
            Optimise();
            IslDebugOutput.Debug("  >- " + code.Count.ToString() + " final code points: \n  " + string.Join(", ", code.Select(x => x.ToString())));
            IslDebugOutput.Debug("\n  > Final (Human-Readable) Code:\n " + string.Join(";\n", code.Select(x => x.Stringify())) + ";");
            IslDebugOutput.Debug("\nCompilation Complete!\n");

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
