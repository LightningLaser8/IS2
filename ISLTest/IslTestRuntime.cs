using ISL;
using ISL.Compiler;
using ISL.Language.Types;
namespace ISLTest
{
    internal class IslTestRuntime
    {
        static IslInterface? @interface;
        static IslProgram? program;
        static string debugOutput = " ";
        static string runOutput = " ";
        static void Main(string[] args)
        {
            bool saveProgram = false;
            string source = "";
            bool listening = true;
            bool repeat = true;
            bool debug = false;
            WriteInstructions();
            while (repeat)
            {
                @interface = new IslInterface();
                string inp = TakeInput();
                if (inp == "!quit")
                {
                    WriteResponse("!quit", "Stopping program.");
                    break;
                }
                else if (inp == "!debug")
                {
                    debug = !debug;
                    WriteResponse("!debug", $"Debug mode {(debug ? "" : "de")}activated.");
                }
                else if (inp == "!runmode")
                {
                    saveProgram = false;
                    WriteResponse("!runmode", "Now running input once.");
                }
                else if (inp == "!compmode")
                {
                    saveProgram = true;
                    WriteResponse("!compmode", "Now storing input.");
                }
                else if (inp == "!end")
                {
                    listening = false;
                    WriteResponse("!end", "ISL input ended.");
                    WriteSeparator("      ------------------     ");
                }
                else if (inp == "!reset")
                {
                    if (saveProgram)
                    {
                        WriteResponse("!reset", "Compiled program cleared.");
                        program = null;
                    }
                    else WriteResponse("!reset", "Compile mode is off: use !compmode to enable", true);
                    WriteSeparator("      ------------------     ");
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
                        ShowResult();
                        if (!saveProgram) program = null;
                        source = "";
                    }
                    WriteSeparator("      ------------------     ");
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

        static void WriteInstructions()
        {
            WriteSeparator("##   ISL Terminal Runtime  ##");
            WriteSeparator("-    --- Instructions ---   -");
            WriteInstruction("!debug", "Toggles debug mode.");
            WriteInstruction("!runmode", "Mode: Runs ISL code once. The default.");
            WriteInstruction("!compmode", "Mode: Stores compiled ISL code.");
            WriteInstruction("!exec", "Executes the stored program.");
            WriteInstruction("!clear", "Clears all console output.");
            WriteInstruction("!reset", "Clears all ISL input and compiled programs.");
            WriteInstruction("!end", "Stops taking ISL input.");
            WriteInstruction("!quit", "Stops recieving input, and stops the program.");
            WriteSeparator("-     ----- Inputs ----     -");
            WriteInput("debug", IslType.Bool, "True if debug mode is on, false if not.");
            WriteInput("takes-input", IslType.Bool, "True if code input can still be taken.");
            WriteSeparator("-     -----------------     -");
        }

        static void ShowResult()
        {
            if (@interface is null)
            {
                WriteSeparator("      ------ Debug -----     ");
                WriteError("No interface connected!");
                return;
            }
            if (debugOutput.Length > 0)
            {
                WriteSeparator("      ------ Debug -----     ");
                WriteISLOutput(debugOutput);
            }
            if (runOutput.Length > 0)
            {
                WriteSeparator("      ---- Run Debug ---     ");
                WriteISLOutput(runOutput);
            }
            if (program is null)
            {
                WriteSeparator("      ----- Output -----     ");
                WriteError("No program has been compiled.");
            }
            else
            {
                WriteSeparator("      ----- Output -----     ");
                var outputs = program.LastOutputs;
                foreach (var item in outputs)
                {
                    WriteISLOutput($"({item.Value?.GetType().Name}) {item.Key} = {item.Value?.ToString()}", true);
                }
            }
            WriteSeparator("      ----- Result -----     ");
            if (@interface.Errored)
                WriteError(" <!> " + @interface.ErrorMessage);
            else
                WriteISLOutput("  "+program?.LastResult.Stringify(), true);
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
        static void WriteSeparator(string separator)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
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
