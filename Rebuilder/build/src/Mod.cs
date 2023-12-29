using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

[assembly: AssemblyTitle("Duck Game Rebuilt")]
[assembly: AssemblyCompany("DGR Team")]
[assembly: AssemblyDescription("An installer and manager mod for Duck Game Rebuilt")]
[assembly: AssemblyVersion("1.0.0")]

namespace DuckGame.Cobalt
{
    public sealed class Mod : ClientMod
    {
        public static bool OnDGR;
        
        protected override void OnPreInitialize()
        {
            OnDGR = IsOnDGR();

            if (!OnDGR)
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

        private string DGRFilePath => configuration.directory + "/dgr/DuckGame.exe";
        private string PatchFilePath => configuration.directory + "/patch/quickload.patch";
        private string BsDiffFilePath => configuration.directory + "/patch/BsDiff.dll";
        private string SharpZipLibFilePath => configuration.directory + "/patch/ICSharpCode.SharpZipLib.dll";

        private void RestartToDGR()
        {
            Process.Start(DGRFilePath, Program.commandLine + $" -from \"{configuration.directory}\"");
            Process.GetCurrentProcess().Kill();
        }

        // assuming this is currently vanilla dg...
        private void PatchForDGRQuickload()
        {
            string gamePath = typeof(ItemBox).Assembly.Location;
            string root = Path.GetDirectoryName(gamePath);
            string tempGamePath = gamePath + ".tmp";

            // generate files
            if (File.Exists(tempGamePath))
                File.Delete(tempGamePath);
            File.Move(gamePath, tempGamePath);

            using FileStream vanilla = File.OpenRead(tempGamePath);
            using FileStream patched = File.Create(gamePath);

            // load BsDiff and apply patch
            string sharpZipLibNewPath = root + "/ICSharpCode.SharpZipLib.dll";
            if (!File.Exists(sharpZipLibNewPath))
                File.Copy(SharpZipLibFilePath, sharpZipLibNewPath);

            try
            {
                Assembly.LoadFile(BsDiffFilePath)
                    .ExportedTypes.First()
                    .GetMethod("Apply", BindingFlags.Public | BindingFlags.Static)!
                    .Invoke(null, new object[] {vanilla, new Func<Stream>(() => File.OpenRead(PatchFilePath)), patched});
            }
            catch
            {
                vanilla.Close();
                patched.Close();
                
                File.Delete(gamePath);
                File.Move(tempGamePath, gamePath);
                
                throw;
            }

            // indicator for DGR quickloading
            File.WriteAllLines(root + "/rebuilt.quack", new[]
            {
                DGRFilePath,
                
                // WHY WAS THIS INTERNAL AND NOT PUBLIC, PARIL ??!
                (string) typeof(ModLoader).GetProperty("modConfigFile", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null),
                
                configuration.uniqueID
            });
        }
    }
}