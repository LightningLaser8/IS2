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
        private readonly Dictionary<string, string> meta = [];

        internal IslProgram(List<Expression> code, Dictionary<string, string> meta)
        {
            codePoints = code;
            this.meta = meta;
        }
        internal IslProgram(List<Expression> code)
        {
            codePoints = code;
        }
        private readonly List<Expression> codePoints = [];

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
            foreach (var point in codePoints)
            {
                result = point.Eval(this);
            }
            CreateOutputs();
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
                result = this.Execute();
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
        internal void AddInputV(string name, IslValue value)
        {
            if (!Ins.TryAdd(name, value)) Ins[name] = value;
        }

        private void CreateOutputs()
        {
#pragma warning disable IDE0306 // Simplify collection initialization, literally impossible here
            outputs = new(Vars.Where(kvp => Outs.Contains(kvp.Key)).Select(kvp2 => new KeyValuePair<string, IslValue>(kvp2.Key, kvp2.Value.Value)));
#pragma warning restore IDE0306
        }

        internal Dictionary<string, IslValue> Ins { get; } = [];
        internal Dictionary<string, IslVariable> Vars { get; } = [];
        internal List<string> Outs { get; } = [];
        internal void OutputVariable(string name)
        {
            Outs.Add(name);
        }
        public IslVariable CreateVariable(string name, IslType type, IslValue value)
        {
            IslVariable vari = new(name, type)
            {
                Value = value
            };
            if (Vars.ContainsKey(name)) throw new InvalidReferenceError(name + " already exists! It's a " + GetVariable(name)?.VarType);
            Vars.Add(name, vari);
            return vari;
        }
        public IslVariable CreateVariable(string name, IslType type)
        {
            IslVariable vari = new(name, type);
            if (Vars.ContainsKey(name)) throw new InvalidReferenceError(name + " already exists! It's a " + GetVariable(name)?.VarType);
            Vars.Add(name, vari);
            return vari;
        }
        public IslValue SetVariable(string name, IslValue value)
        {
            var vari = GetVariableImperative(name);
            vari.Value = value;
            return value;
        }
        public void DeleteVariable(string name)
        {
            Vars.Remove(name);
        }
        public IslVariable? GetVariable(string name)
        {
            Vars.TryGetValue(name, out var islVariable);
            return islVariable;
        }
        public IslVariable GetVariableImperative(string name)
        {
            if (!Vars.TryGetValue(name, out var islVariable)) throw new InvalidReferenceError($"Variable '{name}' doesn't exist.");
            return islVariable;
        }
    }
}
