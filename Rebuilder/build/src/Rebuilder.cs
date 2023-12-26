using System;
using System.Diagnostics;
using System.Reflection;

[assembly: AssemblyTitle("Duck Game Rebuilt")]
[assembly: AssemblyCompany("DGR Team")]
[assembly: AssemblyDescription("An installer and manager mod for Duck Game Rebuilt")]
[assembly: AssemblyVersion("1.0.0")]

namespace DuckGame.Cobalt
{
    public sealed class Rebuilder : ClientMod
    {
        public static bool OnDGR;
        
        protected override void OnPreInitialize()
        {
            OnDGR = IsOnDGR();

            if (!OnDGR && !Program.commandLine.Contains("-dgrmodhyperlinkdysfunction"))
            {
                PatchForDGRQuickload();
                RestartToDGR();
            }
            
            base.OnPreInitialize();
        }

        private static bool IsOnDGR()
        {
            // dan will probably kill me for this ~Firebreak
            return typeof(Program).GetField("CURRENT_VERSION_ID", BindingFlags.Public | BindingFlags.Static) is { IsLiteral: true, IsInitOnly: false };
        }

        private void RestartToDGR()
        {
            string dgrPath = configuration.directory + "/DGR/DuckGame.exe";
            
            Process.Start(dgrPath, Program.commandLine + $" -from \"{configuration.directory}\"");
            Process.GetCurrentProcess().Kill();
        }

        private static void PatchForDGRQuickload()
        {
            
        }
    }
}