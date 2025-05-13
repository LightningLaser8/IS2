using System.Diagnostics.CodeAnalysis;
using ISL;
using ISL.Interpreter;
using ISL.Runtime.Errors;

namespace Integrate.ModContent.ISL
{
    public class Script(string source, string location = "<anonymous>")
    {
        private static readonly IslInterface _islInterface = new();
        private readonly string _sourceCode = source;
        private readonly string _location = location;
        public string Location => _location;
        private IslProgram? cache;
        public string[] GetMetadata(string tag)
        {
            if (cache == null) Compile();
            return cache.GetMeta(tag).Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }
        [MemberNotNull(nameof(cache))]
        public void Compile()
        {
            TryDo(() => cache = _islInterface.CreateProgram(_sourceCode));
            cache ??= new IslProgram();
        }
        public void Execute()
        {
            if (cache is null) Compile();
            TryDo(() => _ = cache!.Execute());
        }
        private void TryDo(Action thing)
        {
            try
            {
                thing();
            }
            catch (IslError e)
            {
                throw new ScriptException(this, e);
            }
        }
    }
}
