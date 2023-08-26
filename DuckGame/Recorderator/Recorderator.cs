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
        [DevConsoleCommand(Name = "playreplay")]
        public static void PlayReplay(int replay)
        {
            try
            {
                string[] ss = Directory.GetFiles(Corderator.CordsPath, "*.rdt");
                string Replay = ss[replay];

                using (FileStream zipStream = new FileStream(Replay, FileMode.Open))
                {
                    using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                    {
                        ZipArchiveEntry entry = archive.GetEntry(Replay.Split('/').Last()); //dont ask

                        if (entry != null)
                        {
                            using (MemoryStream extractedStream = new MemoryStream())
                            {
                                using (Stream entryStream = entry.Open())
                                {
                                    entryStream.CopyTo(extractedStream);
                                }

                                byte[] extractedData = extractedStream.ToArray();
                                Corderator.ReadCord(extractedData);
                            }
                        }
                        else
                        {
                            DevConsole.Log("????????????????????????????");
                            //??? explode ig
                        }
                    }
                }
            }
            catch
            {
                DevConsole.Log("an error occured " + Main.SpecialCode, Colors.DGRed);
            }
        }
        [AutoConfigField]
        public static int Record = 0;

        [AutoConfigField]
        public static int ClipLength = 5;
        public static Map<byte, Type> autoBlockIDX = new Map<byte, Type>();
        public static Map<byte, Type> autoPlatIDX = new Map<byte, Type>();
        public static Map<byte, Type> bgIDX = new Map<byte, Type>();
        public static Map<byte, Type> bgtileIDX = new Map<byte, Type>();
        public static void Initialize()
        {
            RecorderatorOptions.LoadFile();
            instance = new Recorderator();
        }
        [PostInitialize]
        public static void PostInitialize()
        {
            SomethingSomethingVessel.YeahFillMeUpWithLists();

            List<Type> gtiles = Extensions.GetSubclasses(typeof(AutoBlock)).ToList();
            byte b = 0;
            for (int i = 0; i < gtiles.Count; i++)
            {
                if (gtiles[i] == typeof(BlockGroup)) continue;
                autoBlockIDX.Add(b, gtiles[i]);
                b++;
            }
            List<Type> gplats = Extensions.GetSubclasses(typeof(AutoPlatform)).ToList();
            b = 0;
            for (int i = 0; i < gplats.Count; i++)
            {
                autoPlatIDX.Add(b, gplats[i]);
                b++;
            }
            List<Type> bgs = Extensions.GetSubclasses(typeof(BackgroundUpdater)).ToList();
            b = 0;
            for (int i = 0; i < bgs.Count; i++)
            {
                bgIDX.Add(b, bgs[i]);
                b++;
            }
            List<Type> bgts = Extensions.GetSubclasses(typeof(BackgroundTile)).ToList();
            b = 0;
            for (int i = 0; i < bgts.Count; i++)
            {
                bgtileIDX.Add(b, bgts[i]);
                b++;
            }
            (typeof(Game).GetField("updateableComponents", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).GetValue(MonoMain.instance) as List<IUpdateable>).Add(new update());
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
