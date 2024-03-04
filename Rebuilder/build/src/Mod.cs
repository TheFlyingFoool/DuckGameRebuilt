using BsDiff;
using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;

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
            _properties.Set("isDgrMod", true);
            
            OnDGR = IsOnDGR();
            alreadyPatched = File.Exists(Path.GetDirectoryName(typeof(ItemBox).Assembly.Location) + "/rebuilt.quack");

            if (!OnDGR && !alreadyPatched)
            {
                AppDomain.CurrentDomain.AssemblyResolve += OnCurrentDomainOnAssemblyResolve;
                
                PatchForDGRQuickload();
                SaveVanillaPath();
                RestartToDGR();
            }
            
            base.OnPreInitialize();
        }
        public bool alreadyPatched;
        protected override void OnPostInitialize()
        {
            if (!OnDGR && alreadyPatched)
            {
                AppDomain.CurrentDomain.AssemblyResolve += OnCurrentDomainOnAssemblyResolve;

                PatchForDGRQuickload();
                SaveVanillaPath();
                RestartToDGR();
            }
            base.OnPostInitialize();
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

        private void SaveVanillaPath()
        {
            File.WriteAllText(DuckFile.saveDirectory + "/vanilla_dg.path", typeof(ItemBox).Assembly.Location);
        }

        // assuming this isn't already-patched dg...
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

            try
            {
                BinaryPatch.Apply(vanilla, () => File.OpenRead(PatchFilePath), patched);
            }
            catch
            {
                // unfuck stuff in case it crashes
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

        private Assembly OnCurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("ICSharpCode.SharpZipLib"))
            {
                return Assembly.LoadFile(SharpZipLibFilePath);
            }
            else if (args.Name.StartsWith("BsDiff"))
            {
                return Assembly.LoadFile(BsDiffFilePath);
            }

            return null;
        }
    }
}