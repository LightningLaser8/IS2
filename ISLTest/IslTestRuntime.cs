using System.CommandLine;
using ISL;
using ISL.Compiler;
using ISL.Language.Types;
namespace ISLTest
{
    internal class IslTestRuntime
    {
        static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ISL";

        static IslInterface? @interface;
        static IslProgram? program;
        static string debugOutput = " ";
        static string runOutput = " ";
        static bool debug = false;
        static bool verboseDebug = false;
        static bool saveProgram = false;
        static void Init()
        {
            if (!Directory.Exists(AppDataPath))
                Directory.CreateDirectory(AppDataPath);
        }
        enum DebugVerbosity
        {
            None = 0,
            Runtime = 1,
            All = 2
        }
        static async Task<int> Main(string[] args)
        {
            Init();
            var debugOption = new Option<DebugVerbosity>(
                name: "--debug",
                description: "Outputs debug information about compilation and runtime.",
                getDefaultValue: () => DebugVerbosity.None
                );

            var saveOption = new Option<bool>(
                name: "--save",
                description: "Saves created program to a file.",
                getDefaultValue: () => false
                );

            var fileOption = new Option<FileInfo?>(
                name: "--file",
                description: "The .isl file to read and execute. File will be created if not present.",
                isDefault: true,
                parseArgument: result =>
                {
                    if (result.Tokens.Count == 0)
                    {
                        return new FileInfo(AppDataPath + "\\source.isl");
                    }
                    string? filePath = result.Tokens.Single().Value;
                    if (!File.Exists(filePath))
                    {
                        result.ErrorMessage = "File does not exist";
                        return null;
                    }
                    else
                    {
                        return new FileInfo(filePath);
                    }
                });


            var fileCommand = new Command(
                name: "run",
                description: "Opens, compiles and executes an ISL file."
                )
            { fileOption };
            var directCommand = new Command(
                name: "open",
                description: "Provides an interface for running ISL directly."
                )
            { fileOption };

            var rootCommand = new RootCommand("App for testing and debugging pure ISL.");
            rootCommand.AddGlobalOption(debugOption);
            rootCommand.AddCommand(fileCommand);
            rootCommand.AddCommand(directCommand);

            fileCommand.SetHandler((file, debug) =>
            {
                IslTestRuntime.debug = (int)debug > 0;
                IslTestRuntime.verboseDebug = (int)debug > 1;
                FileMode(file!);
            },
            fileOption, debugOption);

            directCommand.SetHandler((debug) =>
            {
                IslTestRuntime.debug = (int)debug > 0;
                IslTestRuntime.verboseDebug = (int)debug > 1;
                DirectMode();
            },
             debugOption);

            return await rootCommand.InvokeAsync(args);
        }
        static void FileMode(FileInfo file)
        {
            if (!file.Exists)
            {
                CreateFile(file.FullName, Examples.HelloWorld);
            }
            string source = string.Join('\n', File.ReadLines(file.FullName));
            var compiler = new IslInterface();
            try
            {
                var prog = compiler.Compile(source, debug);
                Console.WriteLine("---------------------------\n");
                if (verboseDebug)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write(string.Join('\n', compiler.LastDebug.Split('\n').Select(x => "  " + x)) + "\n");
                    Console.ResetColor();
                    Console.WriteLine("---------------------------\n");
                }
                prog.SafeExecute();
                if (debug)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(string.Join('\n', compiler.CompilerDebug.Split('\n').Select(x => "  " + x)) + "\n");
                    Console.ResetColor();
                    Console.WriteLine("---------------------------\n");
                }
                foreach (var output in prog.LastOutputs)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write($"{output.Key}");
                    Console.ResetColor();
                    Console.Write(" = ");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write($"[{output.Value.Type}] ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"{output.Value.Stringify()}");
                    Console.ResetColor();
                }
            }
            catch (Exception e)
            {
                if (verboseDebug)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write(string.Join('\n', compiler.LastDebug.Split('\n').Select(x => "  " + x)) + "\n");
                    Console.ResetColor();
                    Console.WriteLine("---------------------------\n");
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Compilation Error: ({e.GetType().Name}) {e.Message}");
                Console.ResetColor();
            }
        }

        static void CreateFile(string path, byte[] file)
        {
            var stream = File.Create(path);
            stream.Write(file);
            stream.Close();
        }
        static void DirectMode()
        {
            string source = "";
            WriteInstructions();
            bool listening = true;
            bool repeat = true;
            while (repeat)
            {
                @interface = new IslInterface();
                string inp = TakeInput();
                if (inp == "!quit")
                {
                    WriteResponse("!quit", "Stopping program.");
                    break;
                }
                else if (inp == "!help")
                {
                    debug = !debug;
                    WriteResponse("!help", $"Helping.");
                    WriteHelps();
                }
                else if (inp == "!end")
                {
                    listening = false;
                    WriteResponse("!end", "ISL input ended.");
                    WriteSeparator("  -   ------------------   - ");
                }
                else if (inp == "!reset")
                {
                    if (saveProgram)
                    {
                        WriteResponse("!reset", "Compiled program cleared.");
                        program = null;
                    }
                    else WriteResponse("!reset", "Compile mode is off: use !compmode to enable", true);
                    WriteSeparator("  -   ------------------   - ");
                }
                else if (inp == "!clear")
                {
                    Console.Clear();
                    WriteInstructions();
                    Console.WriteLine();
                    source = "";
                    WriteResponse("!clear", "Console (and source) cleared.");
                }
                else if (inp == "!exec")
                {
                    if (source.Length == 0)
                    {
                        WriteResponse("!exec", "No ISL to execute.", true);
                    }
                    else
                    {
                        WriteResponse("!exec", "Executing ISL.");
                        if (program is null)
                        {
                            try
                            {
                                program = @interface.Compile(source, debug);
                            }
                            catch (Exception e)
                            {
                                WriteError($"Compilation Error! > {e.GetType().Name}: {e.Message}");
                            }
                        }
                        debugOutput = @interface.LastDebug;
                        if (program is not null)
                        {
                            program.AddInput("debug", debug);
                            program.AddInput("takes-input", listening);
                            program.SafeExecute();
                            runOutput = @interface.CompilerDebug;
                        }
                        ShowResult(debug);
                        if (!saveProgram) program = null;
                        source = "";
                    }
                    WriteSeparator("  -   ------------------   - ");
                }
                else
                {
                    if (listening) source += inp + "\n";
                }
            }
            WriteSeparator(" [   ISL Testing Complete  ] ");

            WriteSeparator("Press any key to exit...");
            Console.ReadKey();
        }

        static string TakeInput()
        {
            InputArrow();
            return Console.ReadLine() ?? "";
        }

        static void InputArrow()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("> ");
            Console.ResetColor();
        }
        static void WriteHelps()
        {
            WriteSeparator("  -   ------ Help ------   -");

            WriteSeparator("        -- Keywords --      ");
            WriteHelp("if <condition> <statement>", "If <condition> returns true, evaluate <statement>.");
            WriteHelp("else <statement>", "If the last [if]'s <condition> was false, evaluate <statement>.");
            WriteHelp("elseif <condition> <statement>", "Combines [if] and [else].");
            WriteSeparator("        --------------      ");
            WriteSeparator("        - Operators --      ");
            WriteSeparator("  Basic Mathematics:");
            WriteHelp("<augend> + <addend>", "Returns the result of the addition of <addend> to <augend>.");
            WriteHelp("<minuend> - <subtrahend>", "Returns the result of the subtraction of <subtrahend> from <minuend>.");
            WriteHelp("<multiplicand> * <multiplier>", "Returns the result of the multiplication of <multiplicand> by <multiplier>.");
            WriteHelp("<dividend> / <divisor>", "Returns the result of the division of <dividend> by <divisor>.");
            WriteHelp("<dividend> % <divisor>", "Returns the remainder of the division of <dividend> by <divisor>.");
            WriteHelp("<base> ** <power>", "Returns the result of the exponentiation of <base> to the power of <power>.");
            WriteSeparator("  Trigonometry:");
            WriteHelp("sin <angle>", "Returns the sine of <angle> (where <angle> is measured in radians.)");
            WriteHelp("cos <angle>", "Returns the cosine of <angle> (where <angle> is measured in radians.)");
            WriteHelp("tan <angle>", "Returns the tangent of <angle> (where <angle> is measured in radians.)");
            WriteSeparator("  Logical Operators:");
            WriteHelp("! <value>", "Returns the (bitwise) inverse of <value>.");
            WriteHelp("<left> == <right>", "Returns whether <left> and <right> have the same value.");
            WriteSeparator("  Binary Manipulation:");
            WriteHelp("binmant <float>", "Returns the binary mantissa of the floating-point number <float>.");
            WriteHelp("binexp <float>", "Returns the binary exponent of the floating-point number <float>.");
            WriteSeparator("  Variable Manipulation:");
            WriteHelp("<var> = <value>", "Sets the value of the variable at <var> to <value>.");
            WriteHelp("<var> += <value>", "Sets the value of the variable at <var> to its current value + <value>.");
            WriteHelp("<var> <~ <value>", "Appends <value> to the end of <var>. Returns the variable's value.");
            WriteSeparator("  Collection Manipulation:");
            WriteSubheading(" Works on groups and strings.");
            WriteHelp("<col> <~ <value>", "Appends <value> to the end of <col>. Returns the collection.");
            WriteHelp("<col> at <index>", "Returns the element at index <index> of the collection <col>. Also can be used to search strings.");
            WriteSeparator("  Types:");
            WriteHelp("<value> -> <type>", "Returns <value> cast to type <type>. <type> must be a capitalised type name.");
            WriteSeparator("        -- Variables -      ");
            WriteSubheading(" Technically operators");
            WriteHelp("string <name>", "Creates and returns a string variable with name <name>.");
            WriteHelp("int <name>", "Creates and returns a 64-bit integer variable with name <name>.");
            WriteHelp("float <name>", "Creates and returns a double-precision floaring point number variable with name <name>.");
            WriteHelp("complex <name>", "Creates and returns a complex number variable with name <name>.");
            WriteHelp("bool <name>", "Creates and returns a Boolean variable with name <name>.");
            WriteHelp("group <name>", "Creates and returns a collection variable with name <name>.");
            WriteHelp("infer <name>", "Creates and returns a variable with name <name>. The type is inferred on first assignment to the vaariable.");
            WriteSeparator("These can only be used before a variable declaration, in this order (right to left):");
            WriteHelp("imply <declaration>", "Modifies a variable to automatically cast values to its type on assignment.");
            WriteHelp("const <declaration>", "Makes a variable read-only.");
            WriteHelp("out <declaration>", "Sets a variable to be output on program end.");
            WriteHelp("in <declaration>", "Sets a variable to an input of the same name.");
            WriteSeparator("        - Constructs -      ");
            WriteHelp("( ... )", "Bracket expression: Encapsulates an expression, causing it to be evaluated first.");
            WriteHelp("[ ... , ... ]", "Collection expression: Combines multiple expressions into a group structure.");
            WriteHelp("{ ... }", "Code block: Evaluates multiple expressions, and returns the rewsult of the last one.");
            WriteHelp(@"\ ... \", "Variable getter: Returns the value of the variable at the rwsult of the contained expression.");
            WriteSeparator("        --------------      ");
            WriteSeparator("All operators and keywords must be surrounded by spaces");
            WriteSeparator("All statements must be separated by a semicolon");

            WriteSeparator("  -   ------------------   - ");
        }

        static void WriteInstructions()
        {
            WriteSeparator("##   ISL Terminal Runtime  ##");
            WriteSeparator("  -   -- Instructions --   -");
            WriteInstruction("!debug", "Toggles debug mode.");
            WriteInstruction("!runmode", "Mode: Runs ISL code once. The default.");
            WriteInstruction("!compmode", "Mode: Stores compiled ISL code.");
            WriteInstruction("!exec", "Executes the stored program.");
            WriteInstruction("!clear", "Clears all console output.");
            WriteInstruction("!reset", "Clears all ISL input and compiled programs.");
            WriteInstruction("!end", "Stops taking ISL input.");
            WriteInstruction("!quit", "Stops recieving input, and stops the program.");
            WriteInstruction("!help", "Outputs a list of descriptions of ISL features.");
            WriteSeparator("  -   ----- Inputs -----   -");
            WriteInput("debug", IslType.Bool, "True if debug mode is on, false if not.");
            WriteInput("takes-input", IslType.Bool, "True if code input can still be taken.");
            WriteSeparator("  -   ------------------   - ");
        }

        static void ShowResult(bool debug)
        {

            if (@interface is null)
            {
                if (debug)
                {
                    WriteSeparator("  -   ------ Debug -----   - ");
                    WriteError("No interface connected!");
                }
                return;
            }
            if (debug)
            {
                if (debugOutput.Length > 0)
                {
                    WriteSeparator("  -   ------ Debug -----   - ");
                    WriteISLOutput(debugOutput);
                }
                if (runOutput.Length > 0)
                {
                    WriteSeparator("  -   ---- Run Debug ---   - ");
                    WriteISLOutput(runOutput);
                }
            }
            if (program is null)
            {
                WriteSeparator("  -   ----- Output -----   -  ");
                WriteError("No program has been compiled.");
            }
            else
            {
                WriteSeparator("  -   ----- Output -----   - ");
                var outputs = program.LastOutputs;
                foreach (var item in outputs)
                {
                    WriteISLOutput($"({item.Value?.GetType().Name}) {item.Key} = {item.Value?.ToString()}", true);
                }
            }
            WriteSeparator("  -   -- Return Value --   -  ");
            if (@interface.Errored)
                WriteError(" <!> " + @interface.ErrorMessage);
            else
                WriteISLOutput("  " + program?.LastResult.Stringify(), true);
        }

        public static void ClearLastLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        static void WriteISLOutput(string output, bool isBright = false)
        {
            Console.ForegroundColor = isBright ? ConsoleColor.Cyan : ConsoleColor.Blue;
            Console.WriteLine(output);
            Console.ResetColor();
        }
        static void WriteError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ResetColor();
        }
        static void WriteResponse(string input, string response, bool isNegative = false)
        {
            ClearLastLine();
            InputArrow();
            Console.Write(input);
            Console.ForegroundColor = isNegative ? ConsoleColor.Red : ConsoleColor.Green;
            Console.Write(" :: ");
            Console.Write(response);
            Console.Write("\n");
            Console.ResetColor();
        }
        static void WriteHelp(string islthing, string help)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(islthing);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write(" :: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.ResetColor();
            Console.Write(help);
            Console.Write("\n");
        }
        static void WriteSeparator(string separator)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(separator);
            Console.ResetColor();
        }
        static void WriteSubheading(string separator)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(separator);
            Console.ResetColor();
        }
        static void WriteInstruction(string instruction, string desc = "")
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(instruction);
            Console.ResetColor();
            Console.Write(" - ");
            Console.Write(desc);
            Console.Write('\n');
        }
        static void WriteInput(string id, IslType type, string desc = "")
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(id);
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(" (");
            Console.Write(type.ToString());
            Console.Write(")");
            Console.ResetColor();
            Console.Write(" - ");
            Console.Write(desc);
            Console.Write('\n');
        }
    }
}
