using System;
using System.Windows.Forms;
using DuckGame.src.MonoTime.Console;
namespace CrashWindow
{
    public static class Program
    {
        public static bool IsLinux;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("=== CrashWindow Program.Main started ===");
            Console.WriteLine($"Arguments received ({args.Length} total): {string.Join(" ", args)}");
            int p = (int)Environment.OSVersion.Platform;
            IsLinux = (p == 4) || (p == 6) || (p == 128);
            Console.WriteLine($"OS Platform code: {p} | IsLinux: {IsLinux}");
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-modResponsible")
                {
                    bool val = args[i + 1] == "0" ? false : true;
                    ExceptionForm.modResponsible = val;
                    Console.WriteLine($"  -> Set modResponsible = {val}");
                    i++;
                }
                else if (args[i] == "-modDisabled")
                {
                    if (args[i + 1] == "2")
                    {
                        ExceptionForm.modDisableDisabled = true;
                        Console.WriteLine("  -> Set modDisableDisabled = true");
                    }
                    else
                    {
                        bool val = args[i + 1] == "0" ? false : true;
                        ExceptionForm.modDisabled = val;
                        Console.WriteLine($"  -> Set modDisabled = {val}");
                    }
                    i++;
                }
                else if (args[i] == "-source")
                {
                    ExceptionForm.exceptionSource = args[i + 1];
                    Console.WriteLine($"  -> Set exceptionSource = \"{ExceptionForm.exceptionSource}\"");
                    i++;
                }
                else if (args[i] == "-commandLine")
                {
                    ExceptionForm.commandLine = args[i + 1];
                    Console.WriteLine($"  -> Set commandLine = \"{ExceptionForm.commandLine}\"");
                    i++;
                }
                else if (args[i] == "-executable")
                {
                    ExceptionForm.executablePath = args[i + 1];
                    Console.WriteLine($"  -> Set executablePath = \"{ExceptionForm.executablePath}\"");
                    i++;
                }
                else if (args[i] == "-pVersion")
                {
                    ExceptionForm.pVersion = args[i + 1];
                    Console.WriteLine($"  -> Set pVersion = \"{ExceptionForm.pVersion}\"");
                    i++;
                }
                else if (args[i] == "-pMods")
                {
                    ExceptionForm.pMods = CrashWindow.Base64Decode(args[i + 1]);
                    Console.WriteLine($"  -> Set pMods\n {ExceptionForm.pMods})");
                    i++;
                }
                else if (args[i] == "-pAssembly")
                {
                    ExceptionForm.pAssembly = CrashWindow.Base64Decode(args[i + 1]);
                    Console.WriteLine($"  -> Set pAssembly  \n{ExceptionForm.pAssembly}");
                    i++;
                }
                else if (args[i] == "-pException")
                {
                    ExceptionForm.pException = CrashWindow.Base64Decode(args[i + 1]);
                    Console.WriteLine($"  -> Set pException \n{ExceptionForm.pException }");
                    i++;
                }
                else if (args[i] == "-pLogMessage")
                {
                    ExceptionForm.pLogMessage = CrashWindow.Base64Decode(args[i + 1]);
                    Console.WriteLine($"  -> Set pLogMessage \n{ExceptionForm.pLogMessage}");
                    i++;
                }
                else if (args[i] == "-pComment")
                {
                    ExceptionForm.pComment = CrashWindow.Base64Decode(args[i + 1]);
                    Console.WriteLine($"  -> Set pComment \n {ExceptionForm.pComment}");
                    i++;
                }
                else if (args[i] == "-modName")
                {
                    ExceptionForm.modName = args[i + 1];
                    Console.WriteLine($"  -> Set modName = \"{ExceptionForm.modName}\"");
                    i++;
                }
            }
            Console.WriteLine("=== All arguments parsed successfully ===");

            // Enable visual styles (with logging)
            try
            {
                Application.EnableVisualStyles();
                Console.WriteLine("Application.EnableVisualStyles() succeeded");
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: Application.EnableVisualStyles() failed -> {e.Message}");
                Console.WriteLine($"       StackTrace: {e.StackTrace}");
            }

            // Set compatible text rendering (with logging)
            try
            {
                Application.SetCompatibleTextRenderingDefault(false);
                Console.WriteLine("Application.SetCompatibleTextRenderingDefault(false) succeeded");
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: Application.SetCompatibleTextRenderingDefault() failed -> {e.Message}");
                Console.WriteLine($"       StackTrace: {e.StackTrace}");
            }

            Console.WriteLine("Launching ExceptionForm...");
            Console.WriteLine("=== Starting Windows Forms application ===");

            Application.Run(new ExceptionForm());

            Console.WriteLine("=== CrashWindow Program.Main ended ===");
        }
    }
}
