namespace Integrate.ModContent
{
    public class Mod
    {
        public string DisplayName { get; init; } = "Mod";
        public string Name { get; init; } = "mod";
        public string Version { get; init; } = "1.0.0";
        public string Author { get; init; } = "the community";
        public string Tagline { get; init; } = "An Integrate Mod.";
        public string Description { get; init; } = "[Description goes here]";

        public Content[] content = [];
        public string Describe()
        {
            return $"{DisplayName} ('{Name}' by {Author}, v{Version}): {Tagline}\n {Description}\nContent:\n  {string.Join("\n  ", content.Select(x => $"'{x.name}' in {x.registry}: {x.JSON}"))}";
        }
    }
}
