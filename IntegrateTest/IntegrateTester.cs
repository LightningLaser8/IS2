using Integrate;
using Integrate.Registry;

internal class IntegrateTester
{
    static readonly string defModPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Integrate\Mods\";
    private static void Main(string[] args)
    {
        Setup();
        CreateRegistry("blocks");
        TestLoad();
        ModLoader.SetPrefix(true);
        TestAdd();
        TestConstruct();
    }
    private static void Setup()
    {
        ModLoader.SetInfoOutput(x => Console.WriteLine(x));
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("Testing Integrate: ");
        Console.ResetColor();
    }
    private static Registry CreateRegistry(string name)
    {
        var reg = new Registry();
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(" Adding Registries: '" + name + "'");
        ModLoader.AddModdableRegistry(reg, name);
        Console.ResetColor();
        return reg;
    }
    private static void TestLoad()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(" -- ModLoader.Load(...) -- ");
        Console.ResetColor();
        try
        {
            var mod = ModLoader.Load(defModPath + "TestMod\\");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"-- {mod.DisplayName} Loaded --");
            Console.WriteLine($"-> {mod.Name} (v{mod.Version})");
            Console.WriteLine($"\"{mod.Tagline}\"");
            Console.WriteLine($" {mod.content.Length} additions");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("An error occurred: " + ex.Message);
            Console.ResetColor();
        }
    }
    private static void TestAdd()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(" -- ModLoader.Add(...) -- ");
        Console.ResetColor();
        try
        {
            Console.ResetColor();
            var mod = ModLoader.Add(defModPath + "TestMod\\");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"-- {mod.DisplayName} Added --");
            Console.WriteLine($"-> {mod.Name} (v{mod.Version})");
            Console.WriteLine($"\"{mod.Tagline}\"");
            Console.WriteLine($" {mod.content.Length} additions");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("An error occurred: " + ex.Message);
            Console.ResetColor();
        }
    }
    private static void TestConstruct()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(" -- ModLoader.Construct(...) -- ");
        Console.ResetColor();
        Console.WriteLine(ModLoader.Construct("mod:wall", typeof(Block)));
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(" -- ModLoader.Construct<T>(...) -- ");
        Console.ResetColor();
        Console.WriteLine(ModLoader.Construct<Block>("mod:wall"));
    }

    class Block : IConstructible
    {
        public string Name { get; set; } = "";
        public int Health { get; set; }
        private int _maxHealth;
        public void Init()
        {
            _maxHealth = Health;
        }
        public override string ToString()
        {
            return $"{Name}: {Health}/{_maxHealth}";
        }
    }
}