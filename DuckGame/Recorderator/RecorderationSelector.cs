using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.IO.Compression;
using Mono.Cecil;
using System.Collections;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class RECProfile
    {
        public DuckPersona persona;
        public string name;
        public ulong id;
        public Team hat;

        public bool rebuilt;
        public bool spectator;
        public bool host;
        public bool local;

        public SpriteMap bust;
    }
    public class RECReplay
    {
        public RECReplay(string Path)
        {
            FileInfo fileInfo = new FileInfo(Path);
            if (!fileInfo.Name.StartsWith("cord"))
            {
                displayName = System.IO.Path.GetFileNameWithoutExtension(Path);
            }
            else displayName = $"{fileInfo.CreationTime:ddd yyyy-MM - dd hh: mm: ss}";
            path = Path;
        }

        public string path;
        public int tilLoad;
        public void LoadPreview()
        {
            /*loadedPreview = true;

            using (FileStream zipStream = new FileStream(path, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                {
                    byte[] data1 = Recorderator.DataFromEntry(archive.Entries[5]);//preview
                }
            }*/

        }

        public Tex2D preview;
        
        public int players;
        public int specs;
        public string host = "LOCAL";
        public List<RECProfile> profiles = new List<RECProfile>();
        public void LoadInfo()
        {
            using (FileStream zipStream = new FileStream(path, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                {
                    byte[] data3 = Recorderator.DataFromEntry(archive.Entries[2]);//hats
                    byte[] data4 = Recorderator.DataFromEntry(archive.Entries[3]);//metadata


                    loadedInfo = true;
                    players = 0;
                    specs = 0;

                    profiles = new List<RECProfile>();


                    Dictionary<ushort, Team> tts = new Dictionary<ushort, Team>();

                    BitBuffer hatArray = new BitBuffer(data3);

                    while (hatArray.ReadByte() == 1)
                    {
                        /*
                         *  hatsPreviewBuffer.Write((ushort)teams.IndexOf(p.team));
                        hatsPreviewBuffer.Write(p.team.customData);
                        */
                        ushort index = hatArray.ReadUShort();
                        byte[] data = hatArray.ReadBytes();

                        if (!tts.ContainsKey(index))
                        {
                            Team t = Team.Deserialize(data);
                            tts.Add(index, t);
                        }
                    }


                    BitBuffer metadata = new BitBuffer(data4);
                    time = metadata.ReadInt();
                    byte profAmount = metadata.ReadByte();
                    for (int i = 0; i < profAmount; i++)
                    {
                        Main.SpecialCode = i.ToString();
                        RECProfile rp = new RECProfile();
                        byte persona = metadata.ReadByte();
                        string name = metadata.ReadString();
                        ulong id = metadata.ReadULong();

                        byte ds = metadata.ReadByte();
                        BitArray br = new BitArray(new byte[] { ds });

                        rp.name = name;
                        rp.id = id;
                        rp.host = br[0];
                        rp.spectator = br[1];
                        rp.local = br[2];
                        rp.rebuilt = br[3];

                        if (rp.host) host = Extensions.CleanFormatting(name);

                        if (br[6])
                        {
                            Team team = null;
                            ushort ls = metadata.ReadUShort();
                            if (tts.ContainsKey(ls)) team = tts[ls];
                            else team = Teams.ParseFromIndex(ls);
                            rp.hat = team;
                        }
                        rp.persona = Persona.alllist[persona];
                        rp.bust = (SpriteMap)rp.persona.chatBust.Clone();
                        if (br[5])
                        {
                            if (br[1]) specs++;
                            else players++;
                            profiles.Add(rp);
                        }
                    }
                }
            }
        }
        public bool loadedInfo;
        public bool loadedPreview;
        public int time = -1;

        public Sprite levPreview;

        public string displayName;
    }
    public class RECFolder
    {
        public List<RECFolder> folders = new List<RECFolder>();
        public List<RECReplay> replays = new List<RECReplay>();
        public int scroll;
        public bool sub;

        public bool Initialized;
        public string Path;
        public RECFolder(string path)
        {
            Path = path;
            FileInfo fileInfo = new FileInfo(path);
            displayName = fileInfo.Name;
        }
        public string displayName;
        public void Initialize()
        {
            if (Path == null) return;
            Initialized = true;
            List<string> replaysList = DuckFile.ReGetFiles(Path, "*.crf", SearchOption.TopDirectoryOnly);
            List<string> foldersList = DuckFile.ReGetDirectories(Path);

            for (int i = 0; i < foldersList.Count; i++)
            {
                folders.Add(new RECFolder(foldersList[i]) { sub = true });
            }
            for (int i = 0; i < replaysList.Count; i++)
            {
                replays.Add(new RECReplay(replaysList[i]));
            }
        }
    }
    public class RecorderationSelector : Level
    {
        public override void Initialize()
        {
            icons = new SpriteMap("iconSheet", 16, 16);
            icons.frame = 12;
            icons.CenterOrigin();
            main = new Sprite("RecorderatorMain");
            door = new Sprite("RecorderatorDoor");

            load = new SpriteMap("quackLoader", 31, 31);
            load.AddAnimation("load", 0.3f, true, 0, 1, 2, 3, 4, 5, 6, 7);
            load.SetAnimation("load");
            load.scale = new Vec2(2);
            load.CenterOrigin();

            backgroundColor = Color.Black;

            currentFolder = new RECFolder(DuckFile.saveDirectory + "Recorderations/");
            currentFolder.Initialize();
            base.Initialize();
        }
        public SpriteMap load;
        public Sprite main;
        public Sprite door;
        public SpriteMap icons;
        public override void Update()
        {
            if (selectedFolder != null)
            {
                selectedFolder.Initialize();
                currentFolder = selectedFolder;
                selectedFolder = null;
            }
            if (replayToPlay != null)
            {
                Recorderator.PlayReplay(replayToPlay.path);
                replayToPlay = null;
            }
            /*if (Mouse.scrollingUp || Keyboard.Pressed(Keys.PageUp))
                scrollIndex++;
            else if (Mouse.scrollingDown || Keyboard.Pressed(Keys.PageDown))
                scrollIndex--;*/

                        base.Update();
        }
        public const float fontSize = 0.6f;
        public bool loading;


        public RECFolder currentFolder;

        public RECFolder selectedFolder;
        public RECReplay replayToPlay;
        public override void Draw()
        {
            Graphics.Draw(main, 0, 0, -0.8f);
            Graphics.Draw(door, 194, 10.5f, -1f);

            if (loading)
            {
                load.depth = -0.6f;
                Graphics.Draw(load, 258, 46);
            }

            Vec2 v = Mouse.positionScreen;
            Rectangle rect = new Rectangle(v - new Vec2(2.5f), v + new Vec2(2.5f));
            if (Collision.Rect(rect, new Rectangle(new Vec2(304, 160), new Vec2(318, 174))))
            {
                icons.frame = 12;
                icons.scale = new Vec2(1.2f);
                Graphics.Draw(icons, 311, 169);
            }
            else
            {
                icons.frame = 12;
                icons.scale = new Vec2(1);
                Graphics.Draw(icons, 311, 169);
            }

            FeatherFashion.DrawCursor(v);


            if (currentFolder != null)
            {

                icons.scale = new Vec2(fontSize);
                for (int i = currentFolder.scroll; i < currentFolder.folders.Count + currentFolder.scroll; i++)
                {
                    icons.frame = 1;
                    string displayName = currentFolder.folders[i].displayName;
                    Vec2 stringSize = Extensions.GetFancyStringSize(displayName, fontSize);
                    Rectangle textBounds = new(29, 1 + (stringSize.y + 1) * i, stringSize.x, stringSize.y);
                    Graphics.Draw(icons, textBounds.Left - 5, textBounds.y + 3);

                    bool hovered = textBounds.Contains(Mouse.positionScreen);
                    Color textColor = hovered ? Color.Yellow : Color.White;

                    if (selectedFolder == null && hovered && Mouse.left == InputState.Pressed)
                    {
                        selectedFolder = currentFolder.folders[i];
                    }

                    Graphics.DrawFancyString(displayName, textBounds.tl, textColor, 1f, fontSize);
                }
                int zed = currentFolder.folders.Count;

                loading = false;


                for (int i = currentFolder.scroll; i < currentFolder.replays.Count + currentFolder.scroll; i++)
                {
                    RECReplay replay = currentFolder.replays[i];

                    string displayName = replay.displayName;
                    Vec2 stringSize = Extensions.GetFancyStringSize(displayName, fontSize);
                    Rectangle textBounds = new(18, 1 + (stringSize.y + 1) * (i + zed), stringSize.x, stringSize.y);

                    bool hovered = textBounds.Contains(Mouse.positionScreen);
                    Color textColor = hovered ? Color.Yellow : Color.White;

                    if (hovered)
                    {
                        if (!replay.loadedInfo) replay.LoadInfo();

                        int secs = replay.time / 60;
                        int minutes = (int)((secs % 3600) / 60);
                        int seconds = (int)(secs % 60);
                        Graphics.DrawFancyString($"DURATION {minutes:00}:{seconds:00} HOST {replay.host}\nPLAYERS {replay.players} SPECTATORS {replay.specs}", new Vec2(195, 95f), Color.White, 1f, 0.5f);
                        if (!replay.loadedPreview)
                        {
                            replay.tilLoad++;
                            if (replay.tilLoad > 10)
                            {
                                replay.LoadPreview();
                            }
                            loading = true;
                        }
                        else
                        {
                            
                            Graphics.Draw(replay.preview, 0, 0, 1, 0.5f, -1);
                            loading = false;
                        }
                        for (int x = 0; x < replay.profiles.Count; x++)
                        {
                            RECProfile rc = replay.profiles[x];
                            if (rc.hat != null)
                            {
                                rc.hat.hat.center = new Vec2(0);
                                rc.hat.hat.scale = new Vec2(0.4f);
                                rc.hat.hat.depth = -0.98f;
                                rc.hat.hat.alpha = rc.spectator ? 0.5f : 1;
                                Graphics.Draw(rc.hat.hat, 191 + x * 10, 141.6f);
                            }
                            rc.bust.depth = -0.99f;
                            rc.bust.scale = new Vec2(0.4f);
                            rc.bust.alpha = rc.spectator ? 0.5f : 1;
                            Graphics.Draw(rc.bust, 198 + x * 10, 150.12f);
                        }
                    }
                    else replay.tilLoad = 0;

                   

                    if (hovered && Mouse.left == InputState.Pressed && replayToPlay == null && selectedFolder == null)
                    {
                        replayToPlay = replay;
                    }

                    Graphics.DrawFancyString(displayName, textBounds.tl, textColor, 1f, fontSize);
                }

                /*
                 * int replayIndex = i - scrollIndex;
                    FileInfo fileInfo = new(Recorderations[Recorderations.Length - 1 - replayIndex]);

                    float matchLengthSeconds = 0;
                    string mapName = "LEVEL";

                    string cordDisplayName = $"{fileInfo.CreationTime:ddd yyyy-MM-dd hh:mm:ss} [{mapName}] [{matchLengthSeconds}s]";
                    Vec2 stringSize = Extensions.GetFancyStringSize(cordDisplayName, fontSize);
                    Rectangle textBounds = new(18, 1 + (stringSize.y + 1) * i, stringSize.x, stringSize.y);

                    bool hovered = textBounds.Contains(Mouse.positionScreen);
                    Color textColor = hovered ? Color.Yellow : Color.White;

                    if (hovered && Mouse.left == InputState.Pressed)
                    {
                        Recorderator.PlayReplay(Recorderations.Length - 1 - replayIndex);
                    }

                    Graphics.DrawFancyString(cordDisplayName, textBounds.tl, textColor, 1f, fontSize);
                */
            }
        }
    }
}