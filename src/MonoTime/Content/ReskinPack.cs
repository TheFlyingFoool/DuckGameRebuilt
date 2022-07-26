// Decompiled with JetBrains decompiler
// Type: DuckGame.ReskinPack
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace DuckGame
{
    public class ReskinPack : ContentPack
    {
        public string name;
        public string path;
        public string contentPath;
        public bool hasIngameMusic;
        public bool isHD;
        public Dictionary<string, Color> recolors = new Dictionary<string, Color>();
        public static List<ReskinPack> active = new List<ReskinPack>();
        public static List<ReskinPack> _reskins = new List<ReskinPack>();
        public static ReskinPack context;
        private static bool _loadingMusic;
        private SoundEffect _currentMusic;

        public ReskinPack()
          : base((ModConfiguration)null)
        {
        }

        public static Mod LoadReskin(
          string pDir,
          Mod pExistingMod = null,
          ModConfiguration pExistingConfig = null)
        {
            ReskinPack reskinPack = new ReskinPack()
            {
                name = Path.GetFileName(pDir),
                path = pDir
            };
            reskinPack.Initialize();
            ReskinPack._reskins.Add(reskinPack);
            if (pExistingMod == null && pExistingConfig == null)
            {
                if (!DuckFile.FileExists(pDir + "/screenshot.png"))
                    System.IO.File.Copy(DuckFile.contentDirectory + "/reskin_screenshot.pngfile", pDir + "/screenshot.png");
                if (!DuckFile.FileExists(pDir + "/preview.png"))
                    System.IO.File.Copy(DuckFile.contentDirectory + "/reskin_preview.pngfile", pDir + "/preview.png");
                if (!DuckFile.FileExists(pDir + "/info.txt"))
                    DuckFile.SaveString("My DG Textures(" + reskinPack.name + ")\nDan Rando\nEdit info.txt to change this information!\n<replace this line with the two letters 'hd' to tell DG that this is a high definition texture pack>", pDir + "/info.txt");
                if (!DuckFile.DirectoryExists(pDir + "/Content"))
                    DuckFile.CreatePath(pDir + "/Content");
            }
            Mod mod = pExistingMod;
            if (mod == null)
            {
                reskinPack.contentPath = reskinPack.path + "/Content";
                mod = (Mod)new ClientMod(pDir + "/", pExistingConfig);
                mod.configuration.LoadOrCreateConfig();
                mod.configuration.SetModType(ModConfiguration.Type.Reskin);
                reskinPack.isHD = mod.configuration.isHighResReskin;
                ModLoader.AddMod(mod);
                string str = pDir + "/colors.txt";
                if (DuckFile.FileExists(str))
                {
                    foreach (string readAllLine in DuckFile.ReadAllLines(str))
                    {
                        char[] chArray = new char[1] { '=' };
                        string[] strArray = readAllLine.Split(chArray);
                        if (strArray.Length == 2)
                        {
                            string lowerInvariant = strArray[0].Trim().ToLowerInvariant();
                            string pString = strArray[1].Trim();
                            try
                            {
                                reskinPack.recolors[lowerInvariant] = Color.FromHexString(pString);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
            }
            else if (mod.configuration.isExistingReskinMod)
                reskinPack.contentPath = reskinPack.path;
            mod.SetPriority(Priority.Reskin);
            if (!mod.configuration.disabled)
                ReskinPack.active.Add(reskinPack);
            if (mod.configuration.content == null)
                mod.configuration.content = new ContentPack(mod.configuration);
            return mod;
        }

        public static void InitializeReskins()
        {
            foreach (string directory in DuckFile.GetDirectories(DuckFile.skinsDirectory))
                ReskinPack.LoadReskin(directory);
            if (Steam.user == null)
                return;
            foreach (string directory in DuckFile.GetDirectories(DuckFile.globalSkinsDirectory))
                ReskinPack.LoadReskin(directory);
        }

        public static void FinalizeReskins()
        {
            List<ClassMember> staticMembers = Editor.GetStaticMembers(typeof(Color));
            staticMembers.AddRange((IEnumerable<ClassMember>)Editor.GetStaticMembers(typeof(Colors)));
            Dictionary<string, ClassMember> dictionary = new Dictionary<string, ClassMember>();
            foreach (ClassMember classMember in staticMembers)
            {
                if (classMember.type == typeof(Color))
                {
                    string lowerInvariant = classMember.name.ToLowerInvariant();
                    dictionary[lowerInvariant] = classMember;
                }
            }
            for (int index = ReskinPack.active.Count - 1; index >= 0; --index)
            {
                foreach (KeyValuePair<string, Color> recolor in ReskinPack.active[index].recolors)
                {
                    ClassMember classMember = (ClassMember)null;
                    if (dictionary.TryGetValue(recolor.Key, out classMember))
                        classMember.SetValue((object)null, (object)recolor.Value);
                }
            }
        }

        public void Initialize() => this.hasIngameMusic = DuckFile.GetFiles(this.contentPath + "/Audio/Music/InGame").Length != 0;

        public static T LoadAsset<T>(string pName, bool pMusic = false)
        {
            ReskinPack._loadingMusic = pMusic;
            foreach (ReskinPack reskinPack in ReskinPack.active)
            {
                T obj = reskinPack.Load<T>(pName);
                if ((object)obj != null)
                    return obj;
            }
            return default(T);
        }

        public override T Load<T>(string name)
        {
            if (name.Contains(":"))
                return default(T);
            if (typeof(T) == typeof(string[]))
            {
                if (System.IO.File.Exists(this.contentPath + "/" + name))
                    return (T)(object)System.IO.File.ReadAllLines(this.contentPath + "/" + name);
            }
            else
            {
                if (typeof(T) == typeof(Texture2D) || typeof(T) == typeof(Tex2D))
                {
                    Texture2D texture2D1 = (Texture2D)null;
                    if (this._textures.TryGetValue(name, out texture2D1))
                        return (T)(object)texture2D1;
                    Texture2D texture2D2 = ContentPack.LoadTexture2D(this.contentPath + "/" + name, this._modConfig == null || this._modConfig.processPinkTransparency);
                    this._textures[name] = texture2D2;
                    return (T)(object)texture2D2;
                }
                if (typeof(T) == typeof(SoundEffect))
                {
                    SoundEffect soundEffect1 = (SoundEffect)null;
                    if (!ReskinPack._loadingMusic && this._sounds.TryGetValue(name, out soundEffect1))
                        return (T)(object)soundEffect1;
                    if (ReskinPack._loadingMusic && this._currentMusic != null)
                        this._currentMusic.Dispose();
                    SoundEffect soundEffect2 = this.LoadSoundEffect(this.contentPath + "/" + name);
                    if (ReskinPack._loadingMusic)
                        this._currentMusic = soundEffect2;
                    else
                        this._sounds[name] = soundEffect2;
                    return (T)(object)soundEffect2;
                }
                if (typeof(T) == typeof(Song))
                {
                    Song song1 = (Song)null;
                    if (this._songs.TryGetValue(name, out song1))
                        return (T)(object)song1;
                    Song song2 = this.LoadSong(this.contentPath + "/" + name);
                    this._songs[name] = song2;
                    return (T)(object)song2;
                }
            }
            return default(T);
        }
    }
}
