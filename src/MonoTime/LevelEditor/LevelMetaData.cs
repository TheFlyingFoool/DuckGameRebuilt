// Decompiled with JetBrains decompiler
// Type: DuckGame.LevelMetaData
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DuckGame
{
    public class LevelMetaData : BinaryClassChunk
    {
        public static List<LevelMetaData.SaveLevelPreviewTask> _completedPreviewTasks = new List<LevelMetaData.SaveLevelPreviewTask>();
        public string guid;
        public LevelType type;
        public LevelSize size;
        public ulong workshopID;
        public bool online;
        public bool onlineMode;
        public int version = 1;
        public bool deathmatchReady;
        public bool hasCustomArt;
        public bool eightPlayer;
        public bool eightPlayerRestricted;

        public LevelMetaData.PreviewPair LoadPreview()
        {
            LevelMetaData.PreviewPair previewPair = (LevelMetaData.PreviewPair)null;
            try
            {
                string str1 = DuckFile.LoadString(DuckFile.editorPreviewDirectory + this.guid);
                if (str1 != null)
                {
                    int length = str1.IndexOf('@');
                    string str2 = str1.Substring(0, length);
                    string pTexture = str1.Substring(length + 1);
                    previewPair = new LevelMetaData.PreviewPair();
                    previewPair.strange = str2[0] == '1';
                    previewPair.challenge = str2[1] == '1';
                    previewPair.arcade = str2[2] == '1';
                    string str3 = str2.Substring(3);
                    previewPair.preview = Editor.MassiveBitmapStringToTexture(pTexture);
                    if (previewPair.preview == null)
                        throw new Exception("PreviewPair.preview is null");
                    if (str3.Length > 0)
                    {
                        Dictionary<string, int> dictionary = new Dictionary<string, int>();
                        string str4 = str3;
                        char[] chArray1 = new char[1] { '|' };
                        foreach (string str5 in str4.Split(chArray1))
                        {
                            char[] chArray2 = new char[1] { ',' };
                            string[] strArray = str5.Split(chArray2);
                            if (strArray.Length == 2)
                                dictionary[strArray[0]] = Convert.ToInt32(strArray[1]);
                        }
                        previewPair.invalid = dictionary;
                    }
                    else
                        previewPair.invalid = new Dictionary<string, int>();
                }
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "Failed to load preview string in metadata for " + this.guid);
                previewPair = (LevelMetaData.PreviewPair)null;
            }
            return previewPair;
        }

        private void RunSaveLevelPreviewTask(LevelMetaData.SaveLevelPreviewTask pTask)
        {
            try
            {
                pTask.ltData = new Color[pTask.levelTexture.Width * pTask.levelTexture.Height];
                pTask.levelTexture.GetData<Color>(pTask.ltData);
                pTask.ltWidth = pTask.levelTexture.Width;
                pTask.ltHeight = pTask.levelTexture.Height;
                new Task((Action)(() =>
               {
                   try
                   {
                       pTask.levelString = pTask.levelString + "@" + Editor.TextureToMassiveBitmapString(pTask.ltData, pTask.ltWidth, pTask.ltHeight);
                       lock (LevelMetaData._completedPreviewTasks)
                           LevelMetaData._completedPreviewTasks.Add(pTask);
                   }
                   catch (Exception ex)
                   {
                   }
               })).Start();
            }
            catch (Exception ex)
            {
            }
        }

        public LevelMetaData.PreviewPair SavePreview(
          Texture2D pPreview,
          Dictionary<string, int> pInvalidData,
          bool pStrange,
          bool pChallenge,
          bool pArcade)
        {
            try
            {
                string str = "" + (pStrange ? "1" : "0") + (pChallenge ? "1" : "0") + (pArcade ? "1" : "0");
                foreach (KeyValuePair<string, int> keyValuePair in pInvalidData)
                    str = str + keyValuePair.Key + "," + keyValuePair.Value.ToString() + "|";
                this.RunSaveLevelPreviewTask(new LevelMetaData.SaveLevelPreviewTask()
                {
                    levelString = str,
                    levelTexture = pPreview,
                    savePath = DuckFile.editorPreviewDirectory + this.guid
                });
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "Failed to save preview string in metadata for " + this.guid);
            }
            return new LevelMetaData.PreviewPair()
            {
                preview = pPreview,
                invalid = pInvalidData,
                strange = pStrange,
                challenge = pChallenge,
                arcade = pArcade
            };
        }

        public override BitBuffer Serialize(BitBuffer data = null, bool root = true) => base.Serialize(data, root);

        public class PreviewPair
        {
            public bool pending;
            public Texture2D preview;
            public Dictionary<string, int> invalid;
            public bool strange;
            public bool challenge;
            public bool arcade;
        }

        public class SaveLevelPreviewTask
        {
            public string savePath;
            public string levelString;
            public Texture2D levelTexture;
            public Color[] ltData;
            public int ltWidth;
            public int ltHeight;
        }
    }
}
