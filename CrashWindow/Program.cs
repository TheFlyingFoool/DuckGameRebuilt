using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DuckGame.src.MonoTime.Console;
namespace CrashWindow
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-modResponsible")
                {
                    ExceptionForm.modResponsible = args[i + 1] == "0" ? false : true;
                    i++;
                }
                else if (args[i] == "-modDisabled")
                {
                    if (args[i + 1] == "2")
                        ExceptionForm.modDisableDisabled = true;
                    else
                        ExceptionForm.modDisabled = args[i + 1] == "0" ? false : true;
                    i++;
                }
                else if (args[i] == "-source")
                {
                    ExceptionForm.exceptionSource = args[i + 1];
                    i++;
                }
                else if (args[i] == "-commandLine")
                {
                    ExceptionForm.commandLine = args[i + 1];
                    i++;
                }
                else if (args[i] == "-executable")
                {
                    ExceptionForm.executablePath = args[i + 1];
                    i++;
                }
                else if (args[i] == "-pVersion")
                {
                    ExceptionForm.pVersion = args[i + 1];
                    i++;
                }
                else if (args[i] == "-pMods")
                {
                    ExceptionForm.pMods = CrashWindow.Base64Decode(args[i + 1]);
                    i++;
                }
                else if (args[i] == "-pAssembly")
                {
                    ExceptionForm.pAssembly = CrashWindow.Base64Decode(args[i + 1]);
                    i++;
                }
                else if (args[i] == "-pException")
                {
                    ExceptionForm.pException = CrashWindow.Base64Decode(args[i + 1]);
                    i++;
                }
                else if (args[i] == "-pLogMessage")
                {
                    ExceptionForm.pLogMessage = CrashWindow.Base64Decode(args[i + 1]);
                    i++;
                }
                else if (args[i] == "-pComment")
                {
                    ExceptionForm.pComment = CrashWindow.Base64Decode(args[i + 1]);
                    i++;
                }
                else if (args[i] == "-modName")
                {
                    ExceptionForm.modName = args[i + 1];
                    i++;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new ExceptionForm());
        }
    }
}
