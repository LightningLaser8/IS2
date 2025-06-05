using ISL.Language.Types;
using ISL.Runtime.Errors;

namespace ISL.Language.Variables
{
    public class IslVariableScope
    {
        public IslVariableScope() { }
        public IslVariableScope(IslVariableScope parent)
        {
            Parent = parent;
        }
        public IslVariableScope? Parent { get; init; }
        internal Dictionary<string, IslVariable> Vars { get; } = [];
        public void Reset()
        {
            Vars.Clear();
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
            if (!Vars.TryGetValue(name, out var islVariable)) return Parent?.GetVariable(name);
            return islVariable;
        }
        public IslVariable GetVariableImperative(string name)
        {
            if (!Vars.TryGetValue(name, out var islVariable)) return Parent?.GetVariableImperative(name) ?? throw new InvalidReferenceError($"Variable '{name}' doesn't exist in the current scope.");
            return islVariable;
        }
        public override string ToString() => $"<{string.Join(", ", Vars.Select(x => $"{x.Value.VarType} {x.Key} = {x.Value.Value}"))}{(Parent is null ? "" : $" :: {Parent}")}>";
    }
}
