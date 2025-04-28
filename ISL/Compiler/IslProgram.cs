using ISL.Runtime.Errors;
using ISL.Language.Expressions;
using ISL.Language.Types;
using ISL.Language.Variables;

namespace ISL.Compiler
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

        internal Dictionary<string, IslVariable> Vars { get; } = [];
        public IslVariable CreateVariable(string name, IslType type, IslValue value)
        {
            IslVariable vari = new(name, type)
            {
                Value = value
            };
            Vars.Add(name, vari);
            return vari;
        }
        public IslVariable CreateVariable(string name, IslType type)
        {
            IslVariable vari = new(name, type);
            Vars.Add(name, vari);
            return vari;
        }
        public IslValue SetVariable(string name, IslValue value)
        {
            var vari = GetVariable(name) ?? throw new InvalidReferenceError($"Variable '{name}' doesn't exist.");
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
            if(!Vars.TryGetValue(name, out var islVariable)) throw new InvalidReferenceError($"Variable '{name}' doesn't exist.");
            return islVariable;
        }
    }
}
