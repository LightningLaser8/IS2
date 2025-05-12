namespace Integrate.Metadata
{
    public struct DefinitionEntry
    {
        public DefinitionEntry() { }
        public string name = string.Empty;
        public string path = "./content.json";
        public string registry = "content";
        public override readonly string ToString() => $"(Unresolved) {name} in {registry} at {path}";
    }
}
