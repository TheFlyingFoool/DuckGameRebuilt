using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Reflection;
using System.IO.Compression;

namespace DuckGame
{
    public class Corderator : Thing
    {
        public Corderator() : base(-9999, -9999)
        {
            instance = this;
        }
        public int maxFrame;
        public static Corderator ReadCord(byte[] zeByted)
        {
            BitBuffer b = new BitBuffer(zeByted);
            Corderator cd = new Corderator();
            cd.PlayingThatShitBack = true;
            cd.cFrame = -1;

            cd.maxFrame = b.ReadInt();

            BitBuffer levBuffer = new BitBuffer(b.ReadBytes());
            #region WOW
            byte xd = b.ReadByte();
            byte xd2 = b.ReadByte();
            BitArray WOW = new BitArray(new byte[] { xd, xd2 });
            if (WOW[0])
            {
                string s1 = b.ReadString();
                string s2 = b.ReadString();
                CustomParallax.customParallax = s1;
                Custom.ApplyCustomData(new CustomTileData() { path = s1, texture = Editor.StringToTexture(s2) }, 0, CustomType.Parallax);
            }
            if (WOW[1])
            {//bloc
                string s1 = b.ReadString();
                string s2 = b.ReadString();
                typeof(CustomTileset).GetField("_customType", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, CustomType.Block);
                CustomTileset.customTileset01 = s1;
                Custom.ApplyCustomData(new CustomTileData() { path = s1, texture = Editor.StringToTexture(s2) }, 0, CustomType.Block);
            }
            if (WOW[2])
            {//bloc
                string s1 = b.ReadString();
                string s2 = b.ReadString();
                typeof(CustomTileset2).GetField("_customType", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, CustomType.Block);
                CustomTileset2.customTileset02 = s1;
                Custom.ApplyCustomData(new CustomTileData() { path = s1, texture = Editor.StringToTexture(s2) }, 1, CustomType.Block);
            }
            if (WOW[3])
            {//bloc
                string s1 = b.ReadString();
                string s2 = b.ReadString();
                typeof(CustomTileset3).GetField("_customType", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, CustomType.Block);
                CustomTileset3.customTileset03 = s1;
                Custom.ApplyCustomData(new CustomTileData() { path = s1, texture = Editor.StringToTexture(s2) }, 2, CustomType.Block);
            }
            if (WOW[4])
            {//plat
                string s1 = b.ReadString();
                string s2 = b.ReadString();
                CustomPlatform.customPlatform01 = s1;
                Custom.ApplyCustomData(new CustomTileData() { path = s1, texture = Editor.StringToTexture(s2) }, 0, CustomType.Platform);
            }
            if (WOW[5])
            {//plat
                string s1 = b.ReadString();
                string s2 = b.ReadString();
                CustomPlatform2.customPlatform02 = s1;
                Custom.ApplyCustomData(new CustomTileData() { path = s1, texture = Editor.StringToTexture(s2) }, 1, CustomType.Platform);
            }
            if (WOW[6])
            {//plat
                string s1 = b.ReadString();
                string s2 = b.ReadString();
                CustomPlatform3.customPlatform03 = s1;
                Custom.ApplyCustomData(new CustomTileData() { path = s1, texture = Editor.StringToTexture(s2) }, 2, CustomType.Platform);
            }
            if (WOW[7])
            {//back
                string s1 = b.ReadString();
                string s2 = b.ReadString();
                CustomBackground.customBackground01 = s1;
                Custom.ApplyCustomData(new CustomTileData() { path = s1, texture = Editor.StringToTexture(s2) }, 0, CustomType.Background);
            }
            if (WOW[8])
            {//back
                string s1 = b.ReadString();
                string s2 = b.ReadString();
                CustomBackground2.customBackground02 = s1;
                Custom.ApplyCustomData(new CustomTileData() { path = s1, texture = Editor.StringToTexture(s2) }, 1, CustomType.Background);
            }
            if (WOW[9])
            {//back
                string s1 = b.ReadString();
                string s2 = b.ReadString();
                CustomBackground3.customBackground03 = s1;
                Custom.ApplyCustomData(new CustomTileData() { path = s1, texture = Editor.StringToTexture(s2) }, 2, CustomType.Background);
            }
            DevConsole.Log("chain of wows");
            DevConsole.Log(WOW[0] + "|" + WOW[1] + "|" + WOW[2] + "|" + WOW[3] + "|" + WOW[4] + "|" + WOW[5] + "|" + WOW[6] + "|" + WOW[7] + "|" + WOW[8] + "|" + WOW[9]);
            #endregion
            Level.current = new ReplayLevel() { CCorderr = cd };
            (Level.current as ReplayLevel).DeserializeLevel(levBuffer);

            int iters = b.ReadInt();
            Vec2 lastV = Vec2.Zero;
            Vec2 lastV2 = Vec2.Zero;
            for (int i = 0; i < iters; i++)
            {
                byte bo = b.ReadByte();
                BitArray b_array = new BitArray(new byte[] { bo });

                if (!b_array[0]) lastV = b.ReadVec2();
                if (!b_array[1]) lastV2 = b.ReadVec2();
                cd.camPos.Add(lastV);
                cd.camSize.Add(lastV2);
            }

            iters = b.ReadInt();
            for (int i = 0; i < iters; i++)
            {
                SomethingSomethingVessel.FirstDeser = true;
                SomethingSomethingVessel deserialized = SomethingSomethingVessel.RCDeserialize(new BitBuffer(b.ReadBytes()));
                if (deserialized == null)
                {
                    DevConsole.Log("nulled", Color.Red);
                    continue;
                }
                cd.somethings.Add(deserialized);
            }

            instance = cd;
            return cd;
        }
        public BackgroundUpdater bgUPD;
        //damn this sucks
        //xd 
        public List<CustomParallaxSegment> CustomParallaxSegments = new List<CustomParallaxSegment>();
        public List<AutoBlock> StartingBlocks = new List<AutoBlock>();
        public List<PipeTileset> Pipes = new List<PipeTileset>();
        public List<Teleporter> Teleporters = new List<Teleporter>();
        public List<AutoPlatform> AutoPlatforms = new List<AutoPlatform>();
        public List<BackgroundTile> BackgroundTiles = new List<BackgroundTile>();
        public List<Thing> TheThings = new List<Thing>();
        public List<Thing> theSpikesSawsSpringsETC = new List<Thing>();
        public List<string> names = new List<string>();
        public static string CordsPath = DuckFile.saveDirectory + "Recorderations/";
        public byte[] SaveToFile()
        {
            BitBuffer buffer = new BitBuffer();
            buffer.Write(cFrame);

            Main.SpecialCode = "in lev buffer";
            BitArray array = new BitArray(16);
            //yay, its hell
            #region levShit
            BitBuffer levBuffer = new BitBuffer();
            if (bgUPD != null)
            {
                levBuffer.Write(Recorderator.bgIDX[bgUPD.GetType()]);
                if (bgUPD is CustomParallax)
                {
                    array[0] = true;
                }
            }
            else levBuffer.Write(255);
            levBuffer.Write((ushort)Pipes.Count);//0-65536
            for (int i = 0; i < Pipes.Count; i++)
            {
                PipeTileset p = Pipes[i];
                levBuffer.Write(p.position);
                BitArray br = new BitArray(8);
                int wow = 0;
                int z = ((SpriteMap)p.graphic).imageIndex;
                if (p is PipeBlue) wow = 1;
                else if (p is PipeGreen) wow = 2;
                br[0] = (z & 16) > 0;
                br[1] = (z & 8) > 0;
                br[2] = (z & 4) > 0;
                br[3] = (z & 2) > 0;
                br[4] = (z & 1) > 0;
                br[5] = (wow & 2) > 0;
                br[6] = (wow & 1) > 0;
                br[7] = p.IsBackground();

                levBuffer.Write(BitCrusher.BitArrayToByte(br));
            }
            levBuffer.Write((ushort)Teleporters.Count);
            for (int i = 0; i < Teleporters.Count; i++)
            {
                Teleporter t = Teleporters[i];
                levBuffer.Write(t.position);
                BitArray br = new BitArray(16);
                int z = t.direction;
                int x = t.teleHeight;
                br[0] = t.noduck;
                br[1] = t.horizontal;
                br[2] = (z & 4) > 0;
                br[3] = (z & 2) > 0;
                br[4] = (z & 1) > 0;
                br[5] = (x & 16) > 0;
                br[6] = (x & 8) > 0;
                br[7] = (x & 4) > 0;
                br[8] = (x & 2) > 0;
                br[9] = (x & 1) > 0;
                levBuffer.Write(BitCrusher.BitArrayToBytes(br));
            }

            levBuffer.Write((ushort)theSpikesSawsSpringsETC.Count);
            for (int i = 0; i < theSpikesSawsSpringsETC.Count; i++)
            {
                Thing s = theSpikesSawsSpringsETC[i];
                if (s is Saws) levBuffer.Write((byte)0);
                else if (s is Spikes) levBuffer.Write((byte)1);
                else if (s is Spring) levBuffer.Write((byte)2);
                else levBuffer.Write((byte)69);
                switch (s.editorName)
                {
                    case "Saws Up":
                    case "Spikes Up":
                    case "Spring":
                        levBuffer.Write((byte)0);
                        break;
                    case "Saws Down":
                    case "Spikes Down":
                    case "Spring Down":
                        levBuffer.Write((byte)1);
                        break;
                    case "Saws Left":
                    case "Spikes Left":
                    case "Spring DownLeft":
                        levBuffer.Write((byte)2);
                        break;
                    case "Saws Right":
                    case "Spikes Right":
                    case "Spring DownRight":
                        levBuffer.Write((byte)3);
                        break;
                    case "Spring Left":
                        levBuffer.Write((byte)4);
                        break;
                    case "Spring Right":
                        levBuffer.Write((byte)5);
                        break;
                    case "Spring UpLeft":
                        levBuffer.Write((byte)6);
                        break;
                    case "Spring UpRight":
                        levBuffer.Write((byte)7);
                        break;
                    default:
                        levBuffer.Write((byte)255);
                        break;
                }
                levBuffer.Write(s.position);
            }

            levBuffer.Write((ushort)TheThings.Count);
            for (int i = 0; i < TheThings.Count; i++)
            {
                Thing t = TheThings[i];
                byte wow = 0;
                switch (t.GetType().Name)
                {
                    case "CityWall":
                        wow = 0;
                        break;
                    case "TreeTop":
                        wow = 1;
                        break;
                    case "TreeTopDead":
                        wow = 2;
                        break;
                    case "RockWall":
                        wow = 3;
                        break;
                    case "PyramidWall":
                        wow = 4;
                        break;
                    case "VerticalDoor":
                        wow = 5;
                        break;
                    case "PyramidDoor":
                        wow = 6;
                        break;
                    case "IceWedge":
                        wow = 7;
                        break;
                    case "CityRamp":
                        wow = 8;
                        break;
                    case "WaterFlow":
                        wow = 9;
                        break;
                }
                BitArray br = new BitArray(8);

                br[0] = (wow & 8) > 0;
                br[1] = (wow & 4) > 0;
                br[2] = (wow & 2) > 0;
                br[3] = (wow & 1) > 0;
                br[6] = t.flipVertical;
                br[7] = t.flipHorizontal;
                levBuffer.Write(BitCrusher.BitArrayToByte(br));
                levBuffer.Write(t.position);
            }

            levBuffer.Write((ushort)StartingBlocks.Count);
            Type b1 = typeof(CustomTileset);
            Type b2 = typeof(CustomTileset2);
            Type b3 = typeof(CustomTileset3);
            StartingBlocks.Sort(Extensions.AutoBlockSortred);
            for (int i = 0; i < StartingBlocks.Count; i++)
            {
                AutoBlock b = StartingBlocks[i];
                Type xd = b.GetType();
                if (xd == b1) array[1] = true;
                else if (xd == b2) array[2] = true;
                else if (xd == b3) array[3] = true;
                levBuffer.Write(b.position);
                levBuffer.Write(Recorderator.autoBlockIDX[b.GetType()]);
            }

            levBuffer.Write((ushort)AutoPlatforms.Count);
            Type p1 = typeof(CustomPlatform);
            Type p2 = typeof(CustomPlatform2);
            Type p3 = typeof(CustomPlatform3);
            for (int i = 0; i < AutoPlatforms.Count; i++)
            {
                AutoPlatform p = AutoPlatforms[i];
                Type xd = p.GetType();
                if (xd == p1) array[4] = true;
                else if (xd == p2) array[5] = true;
                else if (xd == p3) array[6] = true;
                levBuffer.Write(p.position);
                levBuffer.Write(Recorderator.autoPlatIDX[p.GetType()]);
            }
            levBuffer.Write((ushort)BackgroundTiles.Count);
            Type cc = typeof(CustomBackground);
            Type ccTWO = typeof(CustomBackground2);
            Type ccTHREE = typeof(CustomBackground3);
            for (int i = 0; i < BackgroundTiles.Count; i++)
            {
                BackgroundTile t = BackgroundTiles[i];
                Type xd = t.GetType();
                if (xd == cc) array[7] = true;
                else if (xd == ccTWO) array[8] = true;
                else if (xd == ccTHREE) array[9] = true;
                levBuffer.Write(t.position);

                int w = Maths.Clamp(t.frame, 0, 127);
                BitArray ineedtostopdoingthis = new BitArray(8);
                ineedtostopdoingthis[0] = (w & 64) > 0;
                ineedtostopdoingthis[1] = (w & 32) > 0;
                ineedtostopdoingthis[2] = (w & 16) > 0;
                ineedtostopdoingthis[3] = (w & 8) > 0;
                ineedtostopdoingthis[4] = (w & 4) > 0;
                ineedtostopdoingthis[5] = (w & 2) > 0;
                ineedtostopdoingthis[6] = (w & 1) > 0;
                ineedtostopdoingthis[7] = t.flipHorizontal;
                levBuffer.Write(BitCrusher.BitArrayToByte(ineedtostopdoingthis));
                levBuffer.Write(Recorderator.bgtileIDX[t.GetType()]);
            }

            List<byte> Lbf = levBuffer.buffer.ToList();
            if (levBuffer.position + 13 < levBuffer.buffer.Count())
            {
                Lbf.RemoveRange(levBuffer.position + 10, levBuffer.buffer.Count() - levBuffer.position - 11);
            }
            buffer.Write(Lbf.ToArray(), true);
            #endregion
            Main.SpecialCode = "outside of lev buffer";
            byte[] buff = BitCrusher.BitArrayToBytes(array);
            buffer.Write(buff[0]);
            buffer.Write(buff[1]);
            if (array[0])
            {//parral
                buffer.Write(Custom.data[CustomType.Parallax][0]);
                buffer.Write(Editor.TextureToString(Custom.GetData(0, CustomType.Parallax).texture));
            }
            if (array[1])
            {//block
                buffer.Write(Custom.data[CustomType.Block][0]);
                buffer.Write(Editor.TextureToString(Custom.GetData(0, CustomType.Block).texture));
            }
            if (array[2])
            {
                buffer.Write(Custom.data[CustomType.Block][1]);
                buffer.Write(Editor.TextureToString(Custom.GetData(1, CustomType.Block).texture));
            }
            if (array[3])
            {
                buffer.Write(Custom.data[CustomType.Block][2]);
                buffer.Write(Editor.TextureToString(Custom.GetData(2, CustomType.Block).texture));
            }
            if (array[4])
            {//plat
                buffer.Write(Custom.data[CustomType.Platform][0]);
                buffer.Write(Editor.TextureToString(Custom.GetData(0, CustomType.Platform).texture));
            }
            if (array[5])
            {
                buffer.Write(Custom.data[CustomType.Platform][1]);
                buffer.Write(Editor.TextureToString(Custom.GetData(1, CustomType.Platform).texture));
            }
            if (array[6]) 
            {
                buffer.Write(Custom.data[CustomType.Platform][2]);
                buffer.Write(Editor.TextureToString(Custom.GetData(2, CustomType.Platform).texture));
            }
            if (array[7])
            {//back
                buffer.Write(Custom.data[CustomType.Background][0]);
                buffer.Write(Editor.TextureToString(Custom.GetData(0, CustomType.Background).texture));
            }
            if (array[8]) 
            {
                buffer.Write(Custom.data[CustomType.Background][1]);
                buffer.Write(Editor.TextureToString(Custom.GetData(1, CustomType.Background).texture));
            }
            if (array[9]) 
            {
                buffer.Write(Custom.data[CustomType.Background][2]);
                buffer.Write(Editor.TextureToString(Custom.GetData(2, CustomType.Background).texture));
            }
            buffer.Write(camPos.Count);
            for (int i = 0; i < camPos.Count; i++)
            {
                Vec2 v1 = camPos[i];
                Vec2 v2 = camSize[i];
                BitArray br = new BitArray(8);
                if (v1.y < -500000) br[0] = true;
                if (v2.y < -500000) br[1] = true;
                buffer.Write(BitCrusher.BitArrayToByte(br));

                if (!br[0]) buffer.Write(v1);
                if (!br[1]) buffer.Write(v2);
            }

            bool isThisReplayBroken = false;
            string crashStuff = "";
            buffer.Write(somethings.Count);
            for (int i = 0; i < somethings.Count; i++)
            {
                try
                {
                    Main.SpecialCode2 = "something serialize " + somethings[i].editorName;
                    buffer.Write(somethings[i].ReSerialize(), true);
                }
                catch (Exception ex)
                {
                    isThisReplayBroken = true;
                    crashStuff += i + " crash serialize, name: " + somethings[i].editorName;
                    crashStuff += "\nmain special code: " + Main.SpecialCode + " / " + Main.SpecialCode2;
                    crashStuff += "\nsomething code: " + SomethingSomethingVessel.somethingCrash;
                    crashStuff += "\ncrashlog: ";
                    crashStuff += ex.Message;
                    crashStuff += "\n\n\n";
                    crashStuff += ex.ToString();
                    crashStuff += "end of " + i + " \n\n\n\n\n";
                }
            }

            List<byte> bf = buffer.buffer.ToList();
            if (buffer.position + 13 < buffer.buffer.Count())
            {
                bf.RemoveRange(buffer.position + 10, buffer.buffer.Count() - buffer.position - 11);
            }

            if (isThisReplayBroken)
            {
                File.WriteAllText(CordsPath + "broken_cord_" + DateTime.Now.ToString().Replace('/', '-').Replace(':', '-').Replace(' ', '-') + ".txt", crashStuff);
                File.WriteAllBytes(CordsPath + "broken_cord_" + DateTime.Now.ToString().Replace('/', '-').Replace(':', '-').Replace(' ', '-') + ".rdt", bf.ToArray());
            }
            else
            {
                string path = CordsPath + "cord_" + DateTime.Now.ToString().Replace('/', '-').Replace(':', '-').Replace(' ', '-') + ".rdt";
                int V = 0;
                while (File.Exists(path))
                {
                    V++;
                    path = CordsPath + "cord_" + DateTime.Now.ToString().Replace('/', '-').Replace(':', '-').Replace(' ', '-') + "_" + V + ".rdt";
                }
                using (MemoryStream customFileStream = new MemoryStream(bf.ToArray()))
                {
                    string zipFilePath = CordsPath + "cord_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".rdt";

                    using (FileStream zipStream = new FileStream(zipFilePath, FileMode.Create))
                    {
                        using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
                        {
                            ZipArchiveEntry entry = archive.CreateEntry("cord_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".rdt");

                            using (Stream entryStream = entry.Open())
                            {
                                customFileStream.CopyTo(entryStream);
                            }
                        }
                    }
                }
                //File.WriteAllBytes(path, bf.ToArray());
            }
            return bf.ToArray();
        }
        public int cFrame;

        public List<Vec2> camPos = new List<Vec2>();
        public List<Vec2> camSize = new List<Vec2>();
        public List<SomethingSomethingVessel> somethings = new List<SomethingSomethingVessel>();
        public Map<int, Thing> somethingMap = new Map<int, Thing>();
        public Map<int, SomethingSomethingVessel> somethingMapped = new Map<int, SomethingSomethingVessel>();
        public bool PlayingThatShitBack;
        public static Corderator instance;
        public void ReAddSomeVessel(SomethingSomethingVessel yes)
        {
            if (yes.t == null)
            {
                DevConsole.Log("t was null, created nilvessel");
                if (yes.syncled.ContainsKey("position"))
                {
                    Level.Add(new NilVessel(yes.syncled["position"], yes.changeRemove, "Nulled, vessel name: " + yes.editorName + " vessel reason:" + yes.destroyedReason));
                    return;
                }
                Level.Add(new NilVessel(Vec2.Zero, "Nulled, vessel name: " + yes.editorName + " vessel reason:" + yes.destroyedReason));
                return;
            }
            if (somethingMap.Contains(yes.t))
            {
                DevConsole.Log("already containted", Color.Orange);
                return;
            }
            if (yes.doIndex)
            {
                Main.SpecialCode = "ReAdding " + yes.editorName + "\ndid it contain same index: " + somethingMap.ContainsKey(yes.myIndex) + "\ndid it contain same thing: " + somethingMap.ContainsValue(yes.t);
                Main.SpecialCode = "something something map";
                somethingMap.Add(yes.myIndex, yes.t);
                Main.SpecialCode = "something something mapPED";
                somethingMapped.Add(yes.myIndex, yes);
            }
            Main.SpecialCode = "something something level add not t";
            Level.Add(yes);
            Main.SpecialCode = "something something level add t";
            Level.Add(yes.t);
            Main.SpecialCode = "something something not in here no more";
        }
        public void MapSomeSomeVessel(SomethingSomethingVessel yes)
        {
            if (somethingMap.Contains(yes.t))
            {
                DevConsole.Log("already containted", Color.Orange);
                return;
            }
            somethings.Add(yes);
            somethingMap.Add(yes.myIndex, yes.t);
            somethingMapped.Add(yes.myIndex, yes);
            Level.Add(yes);
        }
        public void UnmapVessel(SomethingSomethingVessel yes)
        {
            somethings.Remove(yes);
            somethingMap.Remove(yes.myIndex);
            somethingMapped.Remove(yes.myIndex);
            Level.Remove(yes);
            Level.Remove(yes.t);
        }
        public static bool Paused;
        public override void Update()
        {
            if (PlayingThatShitBack)
            {
                //run that back
                if (!Paused)
                {
                    PlayThatBack();
                }
            }
            else
            {
                if (camPos.Count > 0 && camPos.Last() == Level.current.camera.position)
                {
                    camPos.Add(new Vec2(0, -999999999));
                }
                else camPos.Add(Level.current.camera.position);
                Vec2 v = new Vec2(Level.current.camera.width, Level.current.camera.height);
                if (camSize.Count > 0 && camSize.Last() == v)
                {
                    camSize.Add(new Vec2(0, -999999999));
                }
                else camSize.Add(v);
                if (cFrame == 1)
                {
                    List<Thing> allTheThings = Extensions.GetListOfThings<Thing>();
                    for (int i = 0; i < allTheThings.Count; i++)
                    {
                        Thing th = allTheThings[i];
                        Type typeoid = th.GetType();
                        if (SomethingSomethingVessel.tatchedVessels.ContainsKey(typeoid))
                        {
                            object[] args = new object[] { th };
                            SomethingSomethingVessel vs = (SomethingSomethingVessel)Activator.CreateInstance(SomethingSomethingVessel.tatchedVessels[typeoid], args);
                            MapSomeSomeVessel(vs);
                        }
                        else if (th is BackgroundTile bt) BackgroundTiles.Add(bt);
                        else if (th is AutoPlatform ap) AutoPlatforms.Add(ap);
                        else if (th is TreeTop || th is TreeTopDead || th is IBigStupidWall || th is PyramidWall || th is VerticalDoor || th is IceWedge || th is WaterFlow)
                        {
                            TheThings.Add(th);
                        }
                        else if (th is Saws || th is Spikes || th is Spring) theSpikesSawsSpringsETC.Add(th);
                        else if (th is PipeTileset pt) Pipes.Add(pt);
                        else if (th is Teleporter t) Teleporters.Add(t);
                        else if (th is AutoBlock bb)
                        {
                            if (bb is BlockGroup bg)
                            {
                                for (int WHAT = 0; WHAT < bg.blocks.Count; WHAT++)
                                {
                                    Block b = bg.blocks[WHAT];
                                    if (b is AutoBlock ab) StartingBlocks.Add(ab);
                                }
                            }
                            else StartingBlocks.Add(bb);
                        }
                        else if (th is Holdable h)
                        {
                            if (h is IAmADuck || h is Equipment) continue;
                            if (h is Gun)
                            {
                                GunVessel gv = new GunVessel(h);
                                MapSomeSomeVessel(gv);
                                continue;
                            }
                            HoldableVessel hv = new HoldableVessel(h);
                            MapSomeSomeVessel(hv);
                        }
                        else if (th is Equipment e)
                        {
                            EquipmentVessel ev = new EquipmentVessel(e);
                            MapSomeSomeVessel(ev);
                        }
                        else if (th is CustomParallaxSegment cps) CustomParallaxSegments.Add(cps);
                    }
                    bgUPD = Level.current.FirstOfType<BackgroundUpdater>();
                }
                cFrame++;
            }
            base.Update();
        }
        public void PlayThatBack()
        {
            cFrame++;
            SFX.enabled = false;
            if (cFrame >= 0 && cFrame < maxFrame - 1)
            {
                Main.SpecialCode2 = "record frame " + cFrame;
                for (int i = 0; i < somethings.Count; i++)
                {
                    SomethingSomethingVessel ves = somethings[i];
                    Main.SpecialCode = "is ves null, if true " + (ves == null);
                    if (cFrame == ves.addTime)
                    {
                        Main.SpecialCode = "crash ves.OnAdd";
                        ves.OnAdd();
                        ReAddSomeVessel(ves);
                        Main.SpecialCode = "After ReAddSomeVessel";
                    }
                    else if (cFrame == ves.deleteTime)
                    {
                        ves.OnRemove();
                        Level.Remove(ves.t);
                    }
                }
                Main.SpecialCode = "crash at camSize";
                Level.current.camera.width = camSize[cFrame].x;
                Level.current.camera.height = camSize[cFrame].y;
                Main.SpecialCode = "the pain is eternal";
                Level.current.camera.position = camPos[cFrame];
            }
            SFX.enabled = true;
            //do sfx stuff here
        }
    }
}