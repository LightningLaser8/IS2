using ISL.Language.Expressions;
using ISL.Language.Types;
using ISL.Language.Variables;
using ISL.Runtime.Errors;

namespace ISL.Interpreter
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
        public string GetMeta(string key)
        {
            try
            {
                return meta[key];
            }
            catch (Exception)
            {
                return "";
            }
        }
        public string[] GetMetaTags()
        {
            return [.. meta.Keys];
        }
        private readonly Dictionary<string, string> meta = [];

        internal IslProgram(List<Expression> code, Dictionary<string, string> meta) : this(code)
        {
            this.meta = meta;
        }
        internal IslProgram(List<Expression> code) : this()
        {
            codePoints = code;
        }
        public IslProgram()
        {
            CurrentScope = GlobalScope;
        }
        private readonly List<Expression> codePoints = [];

        readonly Dictionary<string, IslVariableScope> namespaces = [];
        public void RegisterNamespace(string id, IslVariableScope ns)
        {
            if(!namespaces.TryAdd(id, ns)) throw new IslError($"Namespace {id} already declared!");
        }
        public IslVariableScope GetNamespace(string id)
        {
            return namespaces.TryGetValue(id, out var val) ? val : throw new IslError($"Namespace {id} doesn't exist!");
        }

        public Dictionary<string, IslValue> LastOutputs => outputs;
        public Dictionary<string, object?> LastCLROutputs => new(outputs.Select(kvp => new KeyValuePair<string, object?>(kvp.Key, kvp.Value.ToCLR())));

        private Dictionary<string, IslValue> outputs = [];
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
            namespaces.Clear();
            foreach (var point in codePoints)
            {
                result = point.Eval(this);
            }
            CreateOutputs();
            GlobalScope.Reset();
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
                result = Execute();
                return result;
            }
            catch (IslError e)
            {
                result = IslErrorMessage.FromString(e.GetType().Name + ": " + e.Message);
                return result;
            }
        }

        public void AddInput(string name, long value)
        {
            AddInputV(name, new IslInt(value));
        }
        public void AddInput(string name, double value)
        {
            AddInputV(name, new IslFloat(value));
        }
        public void AddInput(string name, double real, double imaginary)
        {
            AddInputV(name, new IslComplex(real, imaginary));
        }
        public void AddInput(string name, bool value)
        {
            AddInputV(name, value ? IslBool.True : IslBool.False);
        }
        public void AddInput(string name, string value)
        {
            AddInputV(name, new IslString(value));
        }
        public void AddInput(string name, IslValue value)
        {
            AddInputV(name, value);
        }
        internal void AddInputV(string name, IslValue value)
        {
            if (!Ins.TryAdd(name, value)) Ins[name] = value;
        }

        private void CreateOutputs()
        {
            outputs = Outs.Select(x => new KeyValuePair<string, IslValue>(x.Name, x.Value)).ToDictionary();
            //new(GlobalScope.Vars.Where(kvp => Outs.Contains(kvp.Key)).Select(kvp2 => new KeyValuePair<string, IslValue>(kvp2.Key, kvp2.Value.Value)));
        }

        internal Dictionary<string, IslValue> Ins { get; } = [];
        //internal Dictionary<string, IslVariable> Vars { get; } = [];

        public IslVariableScope GlobalScope { get; init; } = new();
        internal IslVariableScope CurrentScope { get; set; }
        internal List<IslVariable> Outs { get; } = [];
        internal void OutputVariable(IslVariable ivar)
        {
            Outs.Add(ivar);
        }
        public IslVariable CreateVariable(string name, IslType type, IslValue value) => GlobalScope.CreateVariable(name, type, value);
        public IslVariable CreateVariable(string name, IslType type) => GlobalScope.CreateVariable(name, type);
        public IslValue SetVariable(string name, IslValue value) => GlobalScope.SetVariable(name, value);
        public void DeleteVariable(string name) => GlobalScope.DeleteVariable(name);
        public IslVariable? GetVariable(string name) => GlobalScope.GetVariable(name);
        public IslVariable GetVariableImperative(string name) => GlobalScope.GetVariableImperative(name);
    }
}
