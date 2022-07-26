// Decompiled with JetBrains decompiler
// Type: DuckGame.DGSave
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.IO;

namespace DuckGame
{
    public static class DGSave
    {
        public static bool upgradingFromVanilla;
        public static bool showModsDisabledMessage;
        public static bool showOnePointFiveMessages;

        public static void Initialize(bool pForce = false)
        {
            if (Steam.IsInitialized() && Steam.user != null)
            {
                if (Steam.user != null)
                {
                    DevConsole.Log(DCSection.General, "Welcome to the Grid, |DGBLUE|" + Steam.user.name + "|WHITE|.");
                    DevConsole.Log(DCSection.General, "You are running Duck Game Steam Build " + Program.steamBuildID.ToString() + ".");
                }
                if (!Directory.Exists(DuckFile.userDirectory) | pForce)
                {
                    DGSave.upgradingFromVanilla = true;
                    DevConsole.Log(DCSection.General, "DGSave.Initialize(Steam, No User Directory):");
                    DevConsole.Log(DCSection.General, DuckFile.userDirectory);
                    DGSave.Copy(DuckFile.globalOptionsDirectory, DuckFile.optionsDirectory, "options.dat");
                    DGSave.Copy(DuckFile.globalOptionsDirectory, DuckFile.optionsDirectory, "global.dat");
                    DGSave.Copy(DuckFile.globalOptionsDirectory, DuckFile.optionsDirectory, "input.dat");
                    DGSave.Copy(DuckFile.globalModsDirectory, DuckFile.modsDirectory, "mods.conf");
                    DGSave.Copy(DuckFile.globalProfileDirectory, DuckFile.profileDirectory, Steam.user.id.ToString() + ".pro");
                    if (Directory.Exists(DuckFile.globalProfileDirectory))
                    {
                        foreach (string file in Directory.GetFiles(DuckFile.globalProfileDirectory))
                        {
                            string fileName = Path.GetFileName(file);
                            if (fileName.Length < 14)
                                DGSave.Copy(DuckFile.globalProfileDirectory, DuckFile.profileDirectory, fileName);
                        }
                    }
                    string str = DuckFile.saveDirectory + "read_before_editing_save.txt";
                    if (!System.IO.File.Exists(str))
                        DuckFile.SaveString("Hello, fellow savegame file editors! The new version of Duck Game has introduced some file system changes. \r\nAll Profile/Options data for a Steam account is now located in (SteamID)/Profile or (SteamID)/Options.\r\n\r\nNote that the DuckGame/Profiles and DuckGame/Options folders are LEGACY, and remain only for compatibility\r\nwith older versions of Duck Game. So If you modify DuckGame/Options/options.dat, nothing will happen! You need to\r\nmodify (DuckGame/4290354908354069823/Options/options.dat). Thanks for understanding.", str);
                }
                else
                {
                    DevConsole.Log(DCSection.General, "DGSave.Initialize(Steam, Has User Directory):");
                    DevConsole.Log(DCSection.General, DuckFile.userDirectory);
                }
                if (!Directory.Exists(DuckFile.cloudLevelDirectory) || Directory.Exists(DuckFile.levelDirectory + "CopiedAlphaLevels"))
                    return;
                foreach (string file in DuckFile.GetFiles(DuckFile.cloudLevelDirectory))
                    DGSave.Move(DuckFile.cloudLevelDirectory, DuckFile.levelDirectory + "CopiedAlphaLevels/", Path.GetFileName(file));
            }
            else
                DevConsole.Log(DCSection.General, "DGSave.Initialize(No Steam)");
        }

        internal static void Move(
          string pFromFolder,
          string pToFolder,
          string pFilename,
          string pToFilename = null,
          bool pCopy = false)
        {
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DGSave.Move(" + pFromFolder + ", " + pToFolder + ", " + pFilename + ", " + (pToFilename == null ? "" : pToFilename) + ", " + pCopy.ToString() + ")");
            if (pToFilename == null)
                pToFilename = pFilename;
            if (!System.IO.File.Exists(pFromFolder + pFilename))
                return;
            DuckFile.CreatePath(pToFolder);
            if (System.IO.File.Exists(pToFolder + pToFilename))
                System.IO.File.Delete(pToFolder + pToFilename);
            if (pCopy)
                System.IO.File.Copy(pFromFolder + pFilename, pToFolder + pToFilename);
            else
                System.IO.File.Move(pFromFolder + pFilename, pToFolder + pToFilename);
        }

        internal static void Copy(
          string pFromFolder,
          string pToFolder,
          string pFilename,
          string pToFilename = null)
        {
            DGSave.Move(pFromFolder, pToFolder, pFilename, pToFilename, true);
        }

        internal static void CopyIfMissing(string pFromFolder, string pToFolder, string pFilename)
        {
            if (!System.IO.File.Exists(pFromFolder + pFilename) || System.IO.File.Exists(pToFolder + pFilename))
                return;
            DGSave.Copy(pFromFolder, pToFolder, pFilename);
        }
    }
}
