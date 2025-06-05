using Integrate;
using Integrate.ModContent.ISL;
using Integrate.Registry;
using System.Diagnostics;

internal class IntegrateTester
{
    static readonly Stopwatch clk = new();
    static readonly string defModPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Integrate\Mods\";
    private static async Task Main()
    {
        Setup();
        CreateRegistry("blocks");
        TestLoad();
        ModLoader.SetPrefix(true);
        TestAdd();
        TestConstruct();
        TestEvent();
        await TestIslPerf();
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

    private static async Task AsyncTest(string header, Func<Task> test)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($" -- {header} -- ");
        Console.ResetColor();
        try
        {
            await test();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("An error occurred: " + ex.Message);
            Console.ResetColor();
        }
    }
    private static void Test(string header, Action test)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($" -- {header} -- ");
        Console.ResetColor();
        try
        {
            test();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("An error occurred: " + ex.Message);
            Console.ResetColor();
        }
    }
    private static void TestLoad()
    {
        Test("ModLoader.Load(...)", () =>
        {
            var mod = ModLoader.Load(defModPath + "TestMod\\");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"-- {mod.DisplayName} Loaded --");
            Console.WriteLine($"-> {mod.Name} (v{mod.Version})");
            Console.WriteLine($"\"{mod.Tagline}\"");
            Console.WriteLine($" {mod.content.Length} additions");
            Console.ResetColor();
            ModLoader.Mods.Remove(mod);
        });
    }
    private static void TestAdd()
    {
        Test("ModLoader.Add(...)", () =>
        {
            Console.ResetColor();
            var mod = ModLoader.Add(defModPath + "TestMod\\");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"-- {mod.DisplayName} Added --");
            Console.WriteLine($"-> {mod.Name} (v{mod.Version})");
            Console.WriteLine($"\"{mod.Tagline}\"");
            Console.WriteLine($" {mod.content.Length} additions");
            Console.ResetColor();
        });
    }
    private static void TestConstruct()
    {
        Test("ModLoader.Construct(...)", () => Console.WriteLine(ModLoader.Construct("mod:wall", typeof(Block)))); Console.WriteLine(" -- ModLoader.Construct<T>(...) -- ");
        Test("ModLoader.Construct<T>(...)", () => Console.WriteLine(ModLoader.Construct<Block>("mod:wall")));
    }

    private static void TestEvent()
    {
        Test("ModLoader.Event(...)", () =>
        {
            var res = ModLoader.Event("ev1", new()
            {
                ["iteration"] = 1
            });
            Console.WriteLine("Produces " + res.ToString());
        });
    }
    private static async Task TestIslPerf()
    {
        await AsyncTest("ISL Performance:", async () =>
        {
            Console.WriteLine(" Tests performance with ISL scripts from the Integrate test mod, which needs to be installed separately");
            Console.WriteLine(" GC having a field day here");
            await Task.Delay(3000);
            TestIslPerfEvents("No Listeners", "ev2", 100000);
            await Task.Delay(3000);
            TestIslPerfEvents("One Script", "ev3", 100000);
            await Task.Delay(3000);
            TestIslPerfEvents("Two Scripts", "ev4", 100000);
        });
    }
    private static void PerfUnitTest(string method, Action<int> action, int tests)
    {
        double ctrl = PerfControlTest();
        clk.Restart();
        for (var i = 0; i < tests; i++)
        {
            action(i);
        }
        clk.Stop();
        DisplayPerfResults(method, clk.Elapsed, tests, ctrl);
    }
    private static void TestIslPerfEvents(string title, string eventName, int tests)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($" -- {tests} events: {title} ({eventName}) -- ");
        Console.ResetColor();
        var ev = new Event(eventName);
        ModLoader.AddEvent(ev);

        PerfUnitTest("SetInput() then FireEvent()", i =>
        {
            ev.SetInput("iteration", i);
            ModLoader.FireEvent(eventName);
        }, tests);
        PerfUnitTest("SetInputs() then FireEvent()", i =>
        {
            ev.SetInputs(new Dictionary<string, object?>() { ["iteration"] = i });
            ModLoader.FireEvent(eventName);
        }, tests);
        PerfUnitTest("Inline FireEvent()", i =>
        {
            ModLoader.FireEvent(eventName, new()
            {
                ["iteration"] = i
            });
        }, tests);
        PerfUnitTest("Inline Event()", i =>
        {
            ModLoader.Event(eventName, new()
            {
                ["iteration"] = i
            });
        }, tests);
        PerfUnitTest("Parameterless Event", i =>
        {
            ModLoader.Event(eventName);
        }, tests);
    }
    private static void DisplayPerfResults(string method, TimeSpan results, int tests, double control)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(method);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(": ");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write(results.TotalMilliseconds + "ms");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(" | Avg ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write((results.TotalMilliseconds * 1000 / tests) + "μs");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(" per event" + Environment.NewLine);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(" => ");
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.Write(control / results.TotalMilliseconds);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(" unweighted performance index" + Environment.NewLine);
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(" => ");
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.Write(tests / results.TotalSeconds);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(" events per second (");
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.Write(tests / 60 / results.TotalSeconds);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(" events per frame)" + Environment.NewLine);
        Console.ResetColor();

    }
    private static double PerfControlTest()
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        clk.Restart();
        for (var i = 0; i < 100000; i++)
        {
            var j = 0; j.ToString();
        }
        clk.Stop();
        Console.WriteLine("||  Control: " + clk.Elapsed.TotalMilliseconds + "ms | Avg " + (clk.Elapsed.TotalMilliseconds / 100) + "μs per control");
        Console.ResetColor();
        return clk.Elapsed.TotalMilliseconds;
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