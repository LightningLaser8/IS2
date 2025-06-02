using ISL;
using ISL.Interpreter;
using ISL.Language.Types;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Integrate.ModContent.ISL
{
    /// <summary>
    /// Encapsulates an ISL program.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="location"></param>
    public sealed class Script(string source, string location = "<anonymous>")
    {
        private static readonly IslInterface _islInterface = new();
        private readonly string _sourceCode = source;
        private readonly string _location = location;
        /// <summary>
        /// The file path to this script.
        /// </summary>
        public string Location => _location;
        private IslProgram? cache;
        internal IslProgram GetIslProgram()
        {
            if (cache is null) Compile();
            return cache;
        }
        /// <summary>
        /// Gets the values of a metadata tag for this script.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public string[] GetMetadata(string tag)
        {
            if (cache is null) Compile();
            return cache.GetMeta(tag).Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }
        /// <summary>
        /// Forces the script to compile early.
        /// </summary>
        [MemberNotNull(nameof(cache))]
        public void Compile()
        {
            TryDo(() => cache = _islInterface.CreateProgram(_sourceCode));
            cache ??= new IslProgram();
        }
        /// <summary>
        /// Runs the script.
        /// </summary>
        public IslExecutionResult Execute()
        {
            if (cache is null) Compile();
            TryDo(() => _ = cache!.Execute());
            return new(cache.LastOutputs);
        }
        /// <summary>
        /// Adds inputs to the program.
        /// Each input can be <see cref="bool"/>, <see cref="int"/>, <see cref="long"/>, <see cref="float"/>, <see cref="double"/>, <see cref="string"/>, <see cref="Complex"/>, <see cref="null"/> or any <see cref="IslValue"/>. Other inputs are ignored.<br/>
        /// Will not duplicate inputs, instead will overwrite them.
        /// </summary>
        /// <param name="inputs"></param>
        public void SetInputs(IDictionary<string, object?> inputs)
        {
            foreach (var input in inputs)
            {
                SetInput(input.Key, input.Value);
            }
        }
        /// <summary>
        /// Adds a single input to the program.
        /// The input can be <see cref="bool"/>, <see cref="int"/>, <see cref="long"/>, <see cref="float"/>, <see cref="double"/>, <see cref="string"/>, <see cref="Complex"/>, <see cref="null"/> or any <see cref="IslValue"/>. Other inputs are ignored.<br/>
        /// Will not duplicate inputs, instead will overwrite them.
        /// </summary>
        /// <param name="inputs"></param>
        public void SetInput(string name, object? value)
        {
            if (cache is null) Compile();
            if (value is bool b) cache.AddInput(name, b);
            if (value is int i) cache.AddInput(name, i);
            if (value is long l) cache.AddInput(name, l);
            if (value is float f) cache.AddInput(name, f);
            if (value is double d) cache.AddInput(name, d);
            if (value is string s) cache.AddInput(name, s);
            if (value is Complex c) cache.AddInput(name, c.Real, c.Imaginary);
            if (value is IslValue v) cache.AddInput(name, v);
            if (value is null) cache.AddInput(name, IslValue.Null);
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
