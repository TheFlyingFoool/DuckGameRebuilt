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
        [DevConsoleCommand(Name = "playreplay")]

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

        [AutoConfigField]
        public static int Record = 0;

        [AutoConfigField]
        public static int ClipLength = 5;
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
        [PostInitialize]
        public static void PostInitialize()
        {
            SomethingSomethingVessel.YeahFillMeUpWithLists();

            IEnumerable<Type> gtiles = Extensions.GetSubclasses(typeof(AutoBlock));
            byte b = 0;
            foreach (Type t in gtiles)
            {
                if (t == typeof(BlockGroup)) continue;
                autoBlockIDX.Add(b, t);
                b++;
            }
            IEnumerable<Type> atiles = Extensions.GetSubclasses(typeof(AutoTile));
            b = 0;
            foreach (Type t in atiles)
            {
                autoTileIDX.Add(b, t);
                b++;
            }
            IEnumerable<Type> gplats = Extensions.GetSubclasses(typeof(AutoPlatform));
            b = 0;
            foreach (Type t in gplats)
            {
                autoPlatIDX.Add(b, t);
                b++;
            }
            IEnumerable<Type> bgs = Extensions.GetSubclasses(typeof(BackgroundUpdater));
            b = 0;
            foreach (Type t in bgs)
            {
                bgIDX.Add(b, t);
                b++;
            }
            IEnumerable<Type> bgts = Extensions.GetSubclasses(typeof(BackgroundTile));
            b = 0;
            foreach (Type t in bgts)
            {
                bgtileIDX.Add(b, t);
                b++;
            }
            bgts = Extensions.GetSubclasses(typeof(ForegroundTile));
            foreach(Type t in bgts)
            {
                bgtileIDX.Add(b, t);
                b++;
            }
            if (Program.IS_DEV_BUILD)
            {
                (typeof(Game).GetField("updateableComponents", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).GetValue(MonoMain.instance) as List<IUpdateable>).Add(new updateCorderator());
            }
        }
        public static Recorderator instance;
        public static UIMenu CreateRecorderatorMenu(UIMenu pPrev)
        {
            UIMenu menu = new UIMenu("|PINK|♥|WHITE|RECORDERATOR|PINK|♥", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 240f, conString: "@CANCEL@BACK @SELECT@SELECT");

            menu.Add(new UIDGRDescribe(Colors.DGPink) { scale = new Vec2(0.5f) }, true);
            menu.Add(new UIText(" ", Colors.DGPink) { scale = new Vec2(0.5f) });

            menu.Add(new UIMenuItemNumber("Clip Length", field: new FieldBinding(instance, "ClipLength", 0, 11, 1), valStrings: new List<string>()
            {
                "5s ", //why duck game, why do i have to put spaces here for it to space out correctly -NiK0
                "10s ",
                "15s ",
                "20s ",
                "25s ",
                "30s ",
                "35s ",
                "40s ",
                "45s ",
                "50s ",
                "55s ",
                "60s "
            })
            {
                dgrDescription = "The amount of seconds a clip will save"
            });

            menu.Add(new UIText(" ", Colors.DGPink) { scale = new Vec2(0.5f) });

            //menu.Add(new UIMenuItemToggle("Ambient Particles", field: new FieldBinding(instance, "AmbientParticles"))
            //{
            //    dgrDescription = "Extra cosmetic particles added by DGR, embers from lamps, leafs from trees, etc"
            //});


            menu.Add(new UIMenuItemNumber("Record", field: new FieldBinding(instance, "Record", 0, 2, 1), valStrings: new List<string>()
            {
                "Never",
                "Clip",
                "Always",
            })
            {
                dgrDescription = "When Recorderator should record stuff"
            });


            menu.Add(new UIText(" ", Color.White));
            menu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(menu, pPrev), backButton: true));
            return menu;
        }

    }
}
