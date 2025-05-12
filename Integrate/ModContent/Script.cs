using ISL;
using ISL.Interpreter;

namespace Integrate.ModContent
{
    public class Script(string source, string location = "<anonymous>")
    {
        private static readonly IslInterface _iint = new();
        private readonly string _sourceCode = source;
        private readonly string _location = location;
        private IslProgram? cache;
        public void Compile()
        {
            cache = _iint.CreateProgram(_sourceCode);
        }
        public void Execute()
        {
            if (cache is null) Compile();
            cache!.Execute();
        }
    }
}
