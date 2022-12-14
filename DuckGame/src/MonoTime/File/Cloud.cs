// Decompiled with JetBrains decompiler
// Type: DuckGame.Cloud
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace DuckGame
{
    public static class Cloud
    {
        public static bool uploadEnabled = true;
        public static bool downloadEnabled = true;
        public static bool nocloud;
        public static HashSet<string> deletedFiles = new HashSet<string>();
        private static Dictionary<string, DateTime> _indexTable = new Dictionary<string, DateTime>();
        private static Queue<CloudOperation> _operations = new Queue<CloudOperation>();
        private static int _totalOperations;
        private static bool _initializedCloud;
        public static bool loaded = false;

        public static bool enabled => _initializedCloud && !nocloud && Steam.IsInitialized() && Steam.CloudEnabled();

        public static bool hasPendingDeletions => deletedFiles.Count > 0;

        public static bool processing => _operations.Count > 0;

        public static float progress => _totalOperations != 0 && _operations.Count != 0 ? Math.Max((1f - _operations.Count / _totalOperations), 0f) : 1f;

        public static void Initialize()
        {
            if (!Steam.IsInitialized())
                return;
            DownloadLatestData();
        }

        public static void Update()
        {
            if (_operations.Count <= 0)
                return;
            if (_operations.Count > _totalOperations)
                _totalOperations = _operations.Count;
            int num = 10;
            while (_operations.Count > 0)
            {
                _operations.Dequeue().Execute();
                if (_operations.Count == 0)
                {
                    _totalOperations = 0;
                    break;
                }
                --num;
                if (num < 0)
                    break;
            }
        }

        public static void Delete(string pPath)
        {
            if (!uploadEnabled)
            {
                if (!MonoMain.logFileOperations)
                    return;
                DevConsole.Log(DCSection.General, "Cloud.Delete(" + pPath + ") skipped (uploadEnabled = false)");
            }
            else
            {
                if (MonoMain.logFileOperations)
                    DevConsole.Log(DCSection.General, "Cloud.Delete(" + pPath + ")");
                if (!Steam.IsInitialized())
                    return;
                CloudFile local = CloudFile.GetLocal(pPath, true);
                if (local == null)
                    return;
                Steam.FileDelete(local.cloudPath);
                if (MonoMain.logFileOperations)
                    DevConsole.Log(DCSection.General, "Steam.FileDelete(" + local.cloudPath + ")");
                local.cloudDate = DateTime.MinValue;
            }
        }

        public static void Write(string path)
        {
            if (!uploadEnabled)
            {
                if (!MonoMain.logFileOperations)
                    return;
                DevConsole.Log(DCSection.General, "Cloud.Write(" + path + ") skipped (uploadEnabled = false)");
            }
            else
            {
                if (MonoMain.logFileOperations)
                    DevConsole.Log(DCSection.General, "Cloud.Write(" + path + ")");
                if (!Steam.IsInitialized())
                    return;
                CloudFile local = CloudFile.GetLocal(path);
                if (local == null || local.isOld)
                    return;
                byte[] data = File.ReadAllBytes(local.localPath);
                Steam.FileWrite(local.cloudPath, data, data.Length);
                if (MonoMain.logFileOperations)
                    DevConsole.Log(DCSection.General, "Steam.FileWrite(" + local.cloudPath + ")");
                local.localDate = DateTime.Now;
                local.cloudDate = DateTime.Now;
                File.SetLastWriteTime(local.localPath, DateTime.Now);
            }
        }

        public static void ReplaceLocalFileWithCloudFile(string pLocalPath, string pCloudString = null)
        {
            if (MonoMain.editSave)
            {
                if (!MonoMain.logFileOperations)
                    return;
                DevConsole.Log(DCSection.General, "Cloud.ReplaceLocalFileWithCloudFile(" + pLocalPath + ") skipped (MonoMain.editSave)");
            }
            else
            {
                pLocalPath = DuckFile.PreparePath(pLocalPath);
                if (!Steam.IsInitialized())
                    return;
                ReplaceLocalFileWithCloudFile(CloudFile.GetLocal(pLocalPath));
            }
        }

        public static void ReplaceLocalFileWithCloudFile(CloudFile pFile)
        {
            if (!enabled)
                return;
            if (!downloadEnabled)
            {
                if (!MonoMain.logFileOperations)
                    return;
                DevConsole.Log(DCSection.General, "Cloud.ReplaceLocalFileWithCloudFile(" + pFile.cloudPath + ") skipped (downloadEnabled = false)");
            }
            else if (MonoMain.editSave)
            {
                if (!MonoMain.logFileOperations)
                    return;
                DevConsole.Log(DCSection.General, "Cloud.ReplaceLocalFileWithCloudFile(" + pFile.cloudPath + ") skipped (MonoMain.editSave)");
            }
            else
            {
                if (pFile == null)
                    return;
                if (MonoMain.logFileOperations)
                    DevConsole.Log(DCSection.General, "Cloud.ReplaceLocalFileWithCloudFile(" + pFile.cloudPath + ")");
                byte[] data = Steam.FileRead(pFile.cloudPath);
                if (data == null || data.Length == 0)
                    return;
                DuckFile.TryFileOperation(() =>
               {
                   DuckFile.TryClearAttributes(pFile.localPath);
                   if (File.Exists(pFile.localPath))
                       File.Delete(pFile.localPath);
                   DuckFile.CreatePath(pFile.localPath, true);
                   FileStream fileStream = File.Create(pFile.localPath);
                   fileStream.Write(data, 0, data.Length);
                   fileStream.Close();
                   pFile.localDate = DateTime.Now;
               }, "ReplaceLocalFileWithCloudFile(" + pFile.localPath + ")");
            }
        }

        public static void ZipUpCloudData(string pFile)
        {
            using (FileStream fileStream = new FileStream(pFile, FileMode.Create))
            {
                using (ZipArchive zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Create, true))
                {
                    int count = Steam.FileGetCount();
                    for (int file = 0; file < count; ++file)
                    {
                        string name = Steam.FileGetName(file);
                        if (!name.EndsWith(".lev") && !name.EndsWith(".png") && !name.EndsWith(".play"))
                        {
                            using (Stream stream = zipArchive.CreateEntry(name).Open())
                            {
                                byte[] buffer = Steam.FileRead(name);
                                stream.Write(buffer, 0, buffer.Length);
                            }
                        }
                    }
                }
            }
        }

        public static void DeleteAllCloudData(bool pNewDataOnly)
        {
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "Cloud.DeleteAllCloudData(" + pNewDataOnly.ToString() + ")");
            UICloudProcess uiCloudProcess = new UICloudProcess("DELETING CLOUD", MonoMain.pauseMenu);
            Level.Add(uiCloudProcess);
            uiCloudProcess.Open();
            MonoMain.pauseMenu = uiCloudProcess;
            int count = Steam.FileGetCount();
            for (int file = 0; file < count; ++file)
            {
                CloudFile cloudFile = CloudFile.Get(Steam.FileGetName(file));
                if (cloudFile != null && (!pNewDataOnly || cloudFile.cloudPath.StartsWith("nq500000_")))
                    _operations.Enqueue(new CloudOperation()
                    {
                        type = CloudOperation.Type.Delete,
                        file = cloudFile
                    });
            }
            _operations.Enqueue(new CloudOperation()
            {
                type = CloudOperation.Type.FinishDeletingCloud
            });
        }

        private static void DownloadLatestData()
        {
            _initializedCloud = true;
            CloudFile.Initialize();
            if (!enabled)
                return;
            int count = Steam.FileGetCount();
            for (int file = 0; file < count; ++file)
            {
                CloudFile cloudFile = CloudFile.Get(Steam.FileGetName(file));
                if (cloudFile != null)
                {
                    CloudOperation cloudOperation = null;
                    if (MonoMain.recoversave && cloudFile.cloudPath.StartsWith("nq500000_"))
                        cloudOperation = new CloudOperation()
                        {
                            type = CloudOperation.Type.WriteToCloud,
                            file = cloudFile
                        };
                    else if (count > 100 && cloudFile.cloudPath.StartsWith("nq403216_") && (cloudFile.cloudPath.Contains("nq403216_Levels/Btooom Mod/") || cloudFile.cloudPath.Contains("nq403216_Levels/Basket/") || cloudFile.cloudPath.Contains("nq403216_Levels/Dorito/") || cloudFile.cloudPath.Contains("nq403216_Levels/DWEP levels") || cloudFile.cloudPath.Contains("nq403216_Levels/DWEPdev levels") || cloudFile.cloudPath.Contains("nq403216_Levels/EDMP/") || cloudFile.cloudPath.Contains("nq403216_Levels/Fair_Spawn_Maps/") || cloudFile.cloudPath.Contains("nq403216_Levels/NDC_1v1") || cloudFile.cloudPath.Contains("nq403216_Levels/QC 1V1/") || cloudFile.cloudPath.Contains("nq403216_Levels/Random Stuff/") || cloudFile.cloudPath.Contains("nq403216_Levels/SD 1v1 Pack/") || cloudFile.cloudPath.Contains("nq403216_Levels/UFF/") || cloudFile.cloudPath.Contains("nq403216_Levels/TMG/") || cloudFile.cloudPath.Contains("nq403216_Levels/C44PMaps/") || cloudFile.cloudPath.Contains("nq403216_Levels/DuckUnbreakable/") || cloudFile.cloudPath.Contains("nq403216_Levels/DWEP levels 1point3 edition/") || cloudFile.cloudPath.Contains("nq403216_Levels/antikore/")))
                        continue;
                    if (cloudOperation == null && cloudFile.localDate == DateTime.MinValue)
                        cloudOperation = new CloudOperation()
                        {
                            type = CloudOperation.Type.ReadFromCloud,
                            file = cloudFile
                        };
                    if (cloudOperation != null)
                    {
                        if (cloudFile.localPath.EndsWith("options.dat") || cloudFile.localPath.EndsWith("localsettings.dat") || cloudFile.localPath.EndsWith("global.dat"))
                            cloudOperation.Execute();
                        else
                            _operations.Enqueue(cloudOperation);
                    }
                    if (cloudFile.cloudDate == DateTime.MinValue)
                        cloudFile.cloudDate = DateTime.Now;
                }
            }
            _operations.Enqueue(new CloudOperation()
            {
                type = CloudOperation.Type.UploadNewFiles
            });
        }

        public class UICloudProcess : UIMenu
        {
            private UIComponent _openOnClose;
            private UIBox _box;
            private bool _closing;

            public UICloudProcess(string pProcessName, UIComponent pOpenOnClose)
              : base(pProcessName, Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 180f, 50f)
            {
                if (pOpenOnClose != null)
                    _openOnClose = pOpenOnClose.rootMenu;
                _box = new UIBox(0f, 0f, high: 26f, isVisible: false);
                Add(_box, true);
            }

            public override void Close()
            {
                if (_closing)
                    return;
                _closing = true;
                if (_openOnClose != null)
                {
                    MonoMain.pauseMenu = _openOnClose;
                    _openOnClose.Open();
                }
                base.Close();
            }

            public override void Draw()
            {
                if (open)
                {
                    string text = "" + "Working... (" + ((int)(progress * 100f)).ToString() + "%)";
                    Graphics.DrawRect(new Rectangle((_box.x - _box.halfWidth + 8f), _box.y, _box.width - 16f, 10f), Color.LightGray, (Depth)0.8f);
                    Graphics.DrawRect(new Rectangle((_box.x - _box.halfWidth + 8f), _box.y, Lerp.FloatSmooth(0f, _box.width - 16f, progress), 10f), Color.White, (Depth)0.8f);
                    Graphics.DrawString(text, new Vec2(_box.x - Graphics.GetStringWidth(text) / 2f, _box.y - 10f), Color.White, (Depth)0.8f);
                    if (!processing)
                        Close();
                }
                base.Draw();
            }
        }

        private class CloudOperation
        {
            public Type type;
            public CloudFile file;
            //private static BitBuffer _backupBuffer;
            //private static int _backupPart;

            public void Execute()
            {
                if (type == Type.FinishDeletingCloud)
                    HUD.AddPlayerChangeDisplay("@PLUG@|LIME|Cloud data cleared!");
                else if (type == Type.FinishDeletingFiles)
                    HUD.AddPlayerChangeDisplay("@PLUG@|LIME|Local deletions applied!");
                else if (type == Type.FinishRecoveringFiles)
                    HUD.AddPlayerChangeDisplay("@PLUG@|LIME|Local files recovered!");
                else if (type == Type.Delete)
                {
                    if (!uploadEnabled)
                    {
                        if (!MonoMain.logFileOperations)
                            return;
                        DevConsole.Log(DCSection.General, "Cloud.Execute.Delete(" + file.cloudPath + ") skipped (uploadEnabled == false)");
                    }
                    else
                    {
                        if (MonoMain.logFileOperations)
                            DevConsole.Log(DCSection.General, "Cloud.Execute.Delete(" + file.cloudPath + ")");
                        Steam.FileDelete(file.cloudPath);
                        file.cloudDate = DateTime.MinValue;
                    }
                }
                else if (type == Type.ReadFromCloud)
                {
                    if (!downloadEnabled)
                    {
                        if (!MonoMain.logFileOperations)
                            return;
                        DevConsole.Log(DCSection.General, "Cloud.Execute.ReadFromCloud(" + file?.ToString() + ") skipped (downloadEnabled == false)");
                    }
                    else
                        ReplaceLocalFileWithCloudFile(file);
                }
                else if (type == Type.WriteToCloud)
                {
                    if (!uploadEnabled)
                    {
                        if (!MonoMain.logFileOperations)
                            return;
                        DevConsole.Log(DCSection.General, "Cloud.Execute.WriteToCloud(" + file.cloudPath + ") skipped (uploadEnabled == false)");
                    }
                    else
                    {
                        byte[] data = File.ReadAllBytes(file.localPath);
                        Steam.FileWrite(file.cloudPath, data, data.Length);
                        if (MonoMain.logFileOperations)
                            DevConsole.Log(DCSection.General, "Cloud.Execute.WriteToCloud(" + file.cloudPath + ")");
                        file.cloudDate = DateTime.Now;
                    }
                }
                else
                {
                    if (type != Type.UploadNewFiles)
                        return;
                    loaded = true;
                }
            }

            public enum Type
            {
                Delete,
                ReadFromCloud,
                WriteToCloud,
                FinishDeletingCloud,
                FinishDeletingFiles,
                FinishRecoveringFiles,
                UploadNewFiles,
            }
        }
    }
}
