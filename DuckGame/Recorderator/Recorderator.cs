using AddedContent.Firebreak;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Reflection;
using System.IO;
using System.IO.Compression;

namespace DuckGame
{
    //Welcome to my hell, recorderator -NiK0
    //This used to be a mod i worked on ages ago that i never finished due to burn out and it being more than i could chew
    //Now im here to give it another shot and implement it into DGR with the finished functionality and promises that i made for the original mod.
    //
    public class Recorderator
    {
        public static byte[] DataFromEntry(ZipArchiveEntry entry)
        {
            using (MemoryStream extractedStream = new MemoryStream())
            {
                using (Stream entryStream = entry.Open())
                {
                    entryStream.CopyTo(extractedStream);
                }

                return extractedStream.ToArray();
            }
        }
        [Marker.DevConsoleCommand(Name = "playreplay")]

        public static void PlayReplay(int replay)
        {
            try
            {
                string[] ss = Directory.GetFiles(Corderator.CordsPath, "*.crf");
                string Replay = ss[replay < 0 ? ss.Length + replay : replay];

                PlayReplay(Replay);
            }
            catch (Exception ex)
            {
                SomethingSomethingVessel.doDestroy = false;
                DevConsole.Log("an error occured " + Main.SpecialCode, Colors.DGRed);
                DevConsole.LogComplexMessage(ex.ToString(), Color.DarkGreen);
            }
        }

        public static void PlayReplay(string path)
        {
            using (FileStream zipStream = new FileStream(path, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                {
                    byte[] data1 = DataFromEntry(archive.Entries[0]);
                    byte[] data2 = DataFromEntry(archive.Entries[1]);
                    Corderator.ReadCord(data1, data2);
                }
            }
        }

        [Marker.AutoConfig]
        public static int RecordType = 0;

        [Marker.AutoConfig]
        public static bool DoRecordMods;

        public static bool Clipped;

        public static bool Playing;
        public static Map<byte, Type> autoBlockIDX = new Map<byte, Type>();
        public static Map<byte, Type> autoTileIDX = new Map<byte, Type>();
        public static Map<byte, Type> autoPlatIDX = new Map<byte, Type>();
        public static Map<byte, Type> bgIDX = new Map<byte, Type>();
        public static Map<byte, Type> bgtileIDX = new Map<byte, Type>();
        public static void Initialize()
        {
            instance = new Recorderator();
        }
        
        public static void PostInitialize()
        {
            if (Program.IS_DEV_BUILD || Program.RecorderatorWatchMode)
            {
                SomethingSomethingVessel.YeahFillMeUpWithLists();

                List<Type> gtiles = Extensions.GetSubclassesList(typeof(AutoBlock));
                byte b = 0;
                for (int i = 0; i < gtiles.Count; i++)
                {
                    Type t = gtiles[i];
                    if (t == typeof(BlockGroup)) continue;
                    autoBlockIDX.Add(b, t);
                    b++;
                }
                List<Type> atiles = Extensions.GetSubclassesList(typeof(AutoTile));
                b = 0;
                for (int i = 0; i < atiles.Count; i++)
                {
                    Type t = atiles[i];
                    autoTileIDX.Add(b, t);
                    b++;
                }
                List<Type> gplats = Extensions.GetSubclassesList(typeof(AutoPlatform));
                b = 0;
                for (int i = 0; i < gplats.Count; i++)
                {
                    Type t = gplats[i];
                    autoPlatIDX.Add(b, t);
                    b++;
                }
                List<Type> bgs = Extensions.GetSubclassesList(typeof(BackgroundUpdater));
                b = 0;
                for (int i = 0; i < bgs.Count; i++)
                {
                    Type t = bgs[i];
                    bgIDX.Add(b, t);
                    b++;
                }
                List<Type> bgts = Extensions.GetSubclassesList(typeof(BackgroundTile));
                b = 0;
                for (int i = 0; i < bgts.Count; i++)
                {
                    Type t = bgts[i];
                    bgtileIDX.Add(b, t);
                    b++;
                }
                bgts = Extensions.GetSubclassesList(typeof(ForegroundTile));
                for (int i = 0; i < bgts.Count; i++)
                {
                    Type t = bgts[i];
                    bgtileIDX.Add(b, t);
                    b++;
                }
                if (!Program.RecorderatorWatchMode) (typeof(Game).GetField("updateableComponents", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).GetValue(MonoMain.instance) as List<IUpdateable>).Add(new updateCorderator());
            }
        }
        public static Recorderator instance;
        public static UIMenu CreateRecorderatorMenu(UIMenu pPrev)
        {
            UIMenu menu = new UIMenu("|PINK|♥|WHITE|RECORDERATOR|PINK|♥", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 240f, conString: "@CANCEL@BACK @SELECT@SELECT");

            menu.Add(new UIDGRDescribe(Colors.DGPink) { scale = new Vec2(0.5f) }, true);
            menu.Add(new UIText(" ", Colors.DGPink) { scale = new Vec2(0.5f) });

            menu.Add(new UIMenuItemNumber("Record", field: new FieldBinding(instance, nameof(RecordType), 0, 2, 1), valStrings: new List<string>()
            {
                "Never",
                "Clip",
                "Always",
            })
            {
                dgrDescription = "When Recorderator should record stuff, F2 to clip"
            });

            menu.Add(new UIMenuItemToggle("Record Modded", field: new FieldBinding(instance, nameof(DoRecordMods)))
            {
                dgrDescription = "Force Recorderator to record anyways even when content mods are enabled |DGRED|(RECORDERATOR MAY CRASH WITH MODS AND REPLAYS CAN BE BROKEN WITH MOD ITEMS)"
            });


            menu.Add(new UIText(" ", Color.White));
            menu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(menu, pPrev), backButton: true));
            return menu;
        }

    }
}
