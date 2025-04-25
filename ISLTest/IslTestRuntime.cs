using ISL;
using ISL.Compiler;
using ISL.Language.Types;
namespace ISLTest
{
    internal class IslTestRuntime
    {
        static IslInterface? @interface;
        static IslProgram? program;
        static void Main(string[] args)
        {
            bool isCompiling = false;
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
                    isCompiling = false;
                    WriteResponse("!runmode", "Now running input once.");
                }
                else if (inp == "!compmode")
                {
                    isCompiling = true;
                    WriteResponse("!compmode", "Now compiling input.");
                }
                else if (inp == "!end")
                {
                    listening = false;
                    WriteResponse("!end", "ISL input ended.");
                    WriteSeparator("      ------------------     ");
                }
                else if (inp == "!reset")
                {
                    if (isCompiling)
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
                        if (isCompiling)
                        {
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
                            program?.SafeExecute();
                        }
                        else @interface.CompileAndExecute(source, out program, debug);
                        ShowResult(isCompiling);
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
            return Console.ReadLine()??"";
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
            WriteInstruction("!compmode", "Mode: Compiles ISL code.");
            WriteInstruction("!exec", "Executes the stored program.");
            WriteInstruction("!clear", "Clears all console output.");
            WriteInstruction("!reset", "Clears all ISL input and compiled programs.");
            WriteInstruction("!end", "Stops taking ISL input.");
            WriteInstruction("!quit", "Stops recieving input, and stops the program.");
            WriteSeparator("-     -----------------     -");
        }

        static void ShowResult(bool isCompiled)
        {
            if(@interface is null)
            {
                WriteSeparator("      ----- Output -----     ");
                WriteError("No interpreter connected!");
                return;
            }
            if (isCompiled && program is null)
            {
                WriteSeparator("      ----- Output -----     ");
                WriteError("No program has been compiled.");
                return;
            }
            string outp = @interface.LastOutput;
            if(outp.Length > 0)
            {
                WriteSeparator("      ----- Output -----     ");
                WriteISLOutput(outp);
            }
            if (@interface is null) return;

            WriteSeparator("      ----- Result -----     ");
            IslValue res = (isCompiled ? program!.LastResult : @interface.LastResult);
            if (@interface.Errored)
                WriteError(" <!> " + res.Stringify());
            else
                WriteISLOutput($"({res.Type}) > {res.Stringify()}", true);
        }

        public static void ClearLastLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
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
    }
}
