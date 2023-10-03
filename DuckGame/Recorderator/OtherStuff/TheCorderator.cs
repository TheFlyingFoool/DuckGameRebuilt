using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Reflection;
using System.IO.Compression;
using System.Buffers;

namespace DuckGame
{
    public class BlockbreakData
    {
        public BlockbreakData(int frame, HashSet<ushort> data)
        {
            Frame = frame;
            Data = data;
        }
        public int Frame;
        public HashSet<ushort> Data;
    }
    public class SoundData
    {
        public SoundData(int h, float v, float p)
        {
            hash = h;
            volume = v;
            pitch = p;
        }
        public int hash;
        public float pitch;
        public float volume;
    }
    public class Corderator : Thing
    {
        public Corderator() : base(-9999, -9999)
        {
            instance = this;
        }
        public bool fake;
        public int maxFrame;
        public static ReplayLevel outLev;
        public static ushort Indexify(Thing t)
        {
            if (t == null || instance == null || !instance.somethingMap.Contains(t)) return 0;
            return (ushort)(instance.somethingMap[t] + 1);
        }
        public static Thing Unindexify(ushort u)
        {
            if (u == 0 || instance == null || !instance.somethingMap.Contains(u)) return null;
            return instance.somethingMap[u - 1];
        }
        public static Corderator ReadCord(byte[] data1, byte[] levelData, bool fake = false)
        {
            Main.SpecialCode = "Begin ReadCord";
            BitBuffer b = new BitBuffer(data1);
            Corderator cd = new Corderator();
            cd.fake = fake;
            cd.PlayingThatShitBack = true;
            cd.cFrame = -1;



            cd.maxFrame = b.ReadInt();
            cd.gamemodeStarted = b.ReadInt();

            ushort teams = b.ReadUShort();
            for (int i = 0; i < teams; i++)
            {
                byte[] tData = b.ReadBytes();
                Team th = Team.Deserialize(tData);
                th.recordIndex = i;
                cd.teams.Add(th);
            }
            

            byte prs = b.ReadByte();
            for (int i = 0; i < prs; i++)
            {
                byte persona = b.ReadByte();
                string name = b.ReadString();
                ulong steamid = b.ReadULong();
                Profile p = new Profile(name, null, null, Persona.all.ElementAt(persona));
                p.steamID = steamid;
                BitArray br = new BitArray(new byte[] { b.ReadByte() });
                p.ReplayHost = br[0];
                p.ReplaySpectator = br[1];
                p.ReplayLocal = br[2];
                p.ReplayRebuilt = br[3];
                if (br[6])
                {
                    int ush = b.ReadUShort() - 1;
                    if (ush != -1 && cd.teams.Count > ush)
                    {
                        p.team = cd.teams[ush];
                    }
                }
                cd.profiles.Add(p);
            }
            if (!fake) DevConsole.DebugLog("|RED|RECORDERATOR |WHITE|Teams and profiles size:" + levelData.Length);
            int cPos = b.position;

            Main.SpecialCode = "readbytes";


            #region WOW
            byte xd = b.ReadByte();
            byte xd2 = b.ReadByte();
            BitArray doCustomTile = new BitArray(new byte[] { xd, xd2 });
            if (doCustomTile[0])
            {
                string s1 = b.ReadString();
                string s2 = b.ReadString();
                CustomParallax.customParallax = s1;
                Custom.ApplyCustomData(new CustomTileData() { path = s1, texture = Editor.StringToTexture(s2) }, 0, CustomType.Parallax);
            }
            if (doCustomTile[1])
            {//bloc
                string s1 = b.ReadString();
                string s2 = b.ReadString();
                typeof(CustomTileset).GetField("_customType", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, CustomType.Block);
                CustomTileset.customTileset01 = s1;
                Custom.ApplyCustomData(new CustomTileData() { path = s1, texture = Editor.StringToTexture(s2) }, 0, CustomType.Block);
            }
            if (doCustomTile[2])
            {//bloc
                string s1 = b.ReadString();
                string s2 = b.ReadString();
                typeof(CustomTileset2).GetField("_customType", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, CustomType.Block);
                CustomTileset2.customTileset02 = s1;
                Custom.ApplyCustomData(new CustomTileData() { path = s1, texture = Editor.StringToTexture(s2) }, 1, CustomType.Block);
            }
            if (doCustomTile[3])
            {//bloc
                string s1 = b.ReadString();
                string s2 = b.ReadString();
                typeof(CustomTileset3).GetField("_customType", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, CustomType.Block);
                CustomTileset3.customTileset03 = s1;
                Custom.ApplyCustomData(new CustomTileData() { path = s1, texture = Editor.StringToTexture(s2) }, 2, CustomType.Block);
            }
            if (doCustomTile[4])
            {//plat
                string s1 = b.ReadString();
                string s2 = b.ReadString();
                CustomPlatform.customPlatform01 = s1;
                Custom.ApplyCustomData(new CustomTileData() { path = s1, texture = Editor.StringToTexture(s2) }, 0, CustomType.Platform);
            }
            if (doCustomTile[5])
            {//plat
                string s1 = b.ReadString();
                string s2 = b.ReadString();
                CustomPlatform2.customPlatform02 = s1;
                Custom.ApplyCustomData(new CustomTileData() { path = s1, texture = Editor.StringToTexture(s2) }, 1, CustomType.Platform);
            }
            if (doCustomTile[6])
            {//plat
                string s1 = b.ReadString();
                string s2 = b.ReadString();
                CustomPlatform3.customPlatform03 = s1;
                Custom.ApplyCustomData(new CustomTileData() { path = s1, texture = Editor.StringToTexture(s2) }, 2, CustomType.Platform);
            }
            if (doCustomTile[7])
            {//back
                string s1 = b.ReadString();
                string s2 = b.ReadString();
                CustomBackground.customBackground01 = s1;
                Custom.ApplyCustomData(new CustomTileData() { path = s1, texture = Editor.StringToTexture(s2) }, 0, CustomType.Background);
            }
            if (doCustomTile[8])
            {//back
                string s1 = b.ReadString();
                string s2 = b.ReadString();
                CustomBackground2.customBackground02 = s1;
                Custom.ApplyCustomData(new CustomTileData() { path = s1, texture = Editor.StringToTexture(s2) }, 1, CustomType.Background);
            }
            if (doCustomTile[9])
            {//back
                string s1 = b.ReadString();
                string s2 = b.ReadString();
                CustomBackground3.customBackground03 = s1;
                Custom.ApplyCustomData(new CustomTileData() { path = s1, texture = Editor.StringToTexture(s2) }, 2, CustomType.Background);
            }
            Main.SpecialCode = "Past tiles";
            if (!fake) DevConsole.DebugLog("|RED|RECORDERATOR |WHITE|Custom texture size:" + (b.position - cPos));
            cPos = b.position;
            #endregion
            if (!fake)
            {
                Level.current = new ReplayLevel() { CCorderr = cd };
                (Level.current as ReplayLevel).DeserializeLevel(new BitBuffer(levelData));
            }
            else
            {
                outLev = new ReplayLevel() { CCorderr = cd };
                outLev.DeserializeLevel(new BitBuffer(levelData));
            }
            if (!fake) DevConsole.DebugLog("|RED|RECORDERATOR |WHITE|Level size:" + levelData.Length);

            //Level.current.Initialize();
            
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
            if (!fake) DevConsole.DebugLog("|RED|RECORDERATOR |WHITE|Camera buffer size:" + (b.position - cPos));
            cPos = b.position;

            iters = b.ReadInt();

            bool logExtra = false;
            if (!fake && Keyboard.Down(Keys.Tab)) logExtra = true;
            for (int i = 0; i < iters; i++)
            {
                SomethingSomethingVessel.FirstDeser = true;
                SomethingSomethingVessel deserialized = SomethingSomethingVessel.RCDeserialize(new BitBuffer(b.ReadBytes()));
                if (logExtra)
                {
                    DevConsole.DebugLog($"|RED|RECORDERATOR |WHITE|{deserialized.GetType().Name} size:" + (b.position - cPos));
                    cPos = b.position;
                }

                if (deserialized == null) continue;
                cd.somethings.Add(deserialized);
            }
            if (!fake && !logExtra) DevConsole.DebugLog("|RED|RECORDERATOR |WHITE|Something vessel size:" + (b.position - cPos));
            cPos = b.position;

            iters = b.ReadInt();
            for (int i = 0; i < iters; i++)
            {
                int iters2 = b.ReadUShort();
                List<SoundData> sd = new List<SoundData>();
                for (int x = 0; x < iters2; x++)
                {
                    int hash = b.ReadInt();
                    float volume = BitCrusher.UShortToFloat(b.ReadUShort(), 8) - 4;
                    float pitch = BitCrusher.UShortToFloat(b.ReadUShort(), 20) - 10;
                    sd.Add(new SoundData(hash, volume, pitch));
                }
                cd.SFXToPlaySave.Add(i, sd);
            }
            if (!fake) DevConsole.DebugLog("|RED|RECORDERATOR |WHITE|Sound buffer size:" + (b.position - cPos));
            cPos = b.position;
            iters = b.ReadInt();
            for (int i = 0; i < iters; i++)
            {
                int frame = b.ReadInt();
                ushort count = b.ReadUShort();
                HashSet<ushort> data = new HashSet<ushort>();
                for (int x = 0; x < count; x++)
                {
                    data.Add(b.ReadUShort());
                }
                cd.BlocksBroken.Add(new BlockbreakData(frame, data));
            }
            if (!fake) DevConsole.DebugLog("|RED|RECORDERATOR |WHITE|Misc buffer size:" + (b.position - cPos));

            iters = b.ReadInt();
            for (int i = 0; i < iters; i++)
            {
                ushort key = b.ReadUShort();
                ushort count = b.ReadUShort();
                List<ChatMessage> list = new List<ChatMessage>();
                for (int x = 0; x < count; x++)
                {
                    byte prof = b.ReadByte();
                    Profile pref = cd.profiles[prof];
                    string message = b.ReadString();
                    ushort index = b.ReadUShort();
                    list.Add(new ChatMessage(pref, message, index));
                }
                cd.chatMessages.Add(key, list);
            }
            if (!fake) DevConsole.DebugLog("|RED|RECORDERATOR |WHITE|Chat buffer size:" + (b.position - cPos));

            Main.SpecialCode = "Outside of recorderator load stuff";
            if (!fake) instance = cd;
            return cd;
        }
        public BackgroundUpdater bgUPD;
        
        public List<CustomParallaxSegment> CustomParallaxSegments = new List<CustomParallaxSegment>();
        public List<AutoBlock> StartingBlocks = new List<AutoBlock>();
        public List<BlockbreakData> BlocksBroken = new List<BlockbreakData>();
        public List<AutoTile> StartingTiles = new List<AutoTile>();
        public List<PipeTileset> Pipes = new List<PipeTileset>();
        public List<Teleporter> Teleporters = new List<Teleporter>();
        public List<AutoPlatform> AutoPlatforms = new List<AutoPlatform>();
        public List<Thing> BackgroundTiles = new List<Thing>();
        public List<Thing> TheThings = new List<Thing>();
        public List<Thing> theLevelDetailsETC = new List<Thing>();
        public List<string> names = new List<string>();
        public static string CordsPath = DuckFile.saveDirectory + "Recorderations/";
        public List<Profile> profiles = new List<Profile>();
        public List<Team> teams = new List<Team>();
        public byte[] SaveToFile(Level lastLevel)
        {
            if (!Directory.Exists(CordsPath)) Directory.CreateDirectory(CordsPath);
            
            Main.SpecialCode = "before anything existed";
            BitBuffer buffer = new BitBuffer();
            BitBuffer metadataBuffer = new BitBuffer();
            BitBuffer hatsPreviewBuffer = new BitBuffer();
            buffer.Write(cFrame);

            buffer.Write(gamemodeStarted);

            Main.SpecialCode = "there were profiles";

            buffer.Write((ushort)teams.Count);
            for (int i = 0; i < teams.Count; i++)
            {
                Team t = teams[i];
                buffer.Write(t.customData, true);
            }

            buffer.Write((byte)profiles.Count);
            metadataBuffer.Write(cFrame);
            metadataBuffer.Write((byte)profiles.Count);
            for (int i = 0; i < profiles.Count; i++)
            {
                Profile p = profiles[i];

                buffer.Write((byte)p.persona.index);
                buffer.Write(p.name);
                buffer.Write(p.steamID);

                metadataBuffer.Write((byte)p.persona.index);
                metadataBuffer.Write(p.name);
                metadataBuffer.Write(p.steamID);
                Main.SpecialCode = "and the profiles have some data";
                BitArray br = new BitArray(8);
                if (p.connection != null)
                {
                    br[0] = p.isHost;
                    br[2] = p.localPlayer;
                }
                br[1] = p.spectator;
                

                br[3] = p.isUsingRebuilt;
                br[4] = p.duck != null;
                br[5] = Profiles.active.Contains(p);
                br[6] = p.team != null;
                Main.SpecialCode = "or something like that";
                if (p.team != null) br[7] = p.team.defaultTeam;
                buffer.Write(BitCrusher.BitArrayToByte(br));
                if (p.team != null)
                {
                    if (teams.Contains(p.team))
                    {
                        hatsPreviewBuffer.Write((byte)1);
                        hatsPreviewBuffer.Write((ushort)Teams.IndexOf(p.team));
                        hatsPreviewBuffer.Write(p.team.customData, true);

                        buffer.Write((ushort)(teams.IndexOf(p.team) + 1));
                    }
                    else buffer.Write((ushort)0);
                }

                metadataBuffer.Write(BitCrusher.BitArrayToByte(br));
                if (p.team != null) metadataBuffer.Write((ushort)Teams.IndexOf(p.team));
                Main.SpecialCode = "end";
            }
            hatsPreviewBuffer.Write((byte)0);

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
                levBuffer.Write(CompressedVec2Binding.GetCompressedVec2(p.position, 10000));
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

                //levBuffer.Write((ushort)Pipes.IndexOf(p.Up()));

                levBuffer.Write(BitCrusher.BitArrayToByte(br));
            }
            levBuffer.Write((ushort)Teleporters.Count);
            for (int i = 0; i < Teleporters.Count; i++)
            {
                Teleporter t = Teleporters[i];
                levBuffer.Write(CompressedVec2Binding.GetCompressedVec2(t.position, 10000));
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

            levBuffer.Write((ushort)theLevelDetailsETC.Count);
            for (int i = 0; i < theLevelDetailsETC.Count; i++)
            {
                Thing s = theLevelDetailsETC[i];
                byte write = 255;
                if (s is Saws) write = 0;
                else if (s is Spikes) write = 1;
                else if (s is Spring) write = 2;
                else if (s is ArcadeLight) write = 3;
                else if (s is PyramidLightRoof) write = 4;
                else if (s is PyramidWallLight) write = 5;
                else if (s is Bulb) write = 6;
                else if (s is HangingCityLight) write = 7;
                else if (s is Lamp) write = 8;
                else if (s is OfficeLight) write = 9;
                else if (s is WallLightRight) write = 10;
                else if (s is Sun) write = 11;
                else if (s is ArcadeTableLight) write = 12;
                else if (s is OfficeLight) write = 13;
                else if (s is WallLightLeft) write = 14;
                else if (s is FishinSign) write = 15;
                else if (s is WaterCooler) write = 16;
                else if (s is Altar) write = 17;
                else if (s is SnowGenerator) write = 18;
                else if (s is WaterFall) write = 19;
                else if (s is WaterFallTile) write = 20;

                else if (s is MallardBillboard) write = 80;//special case for  80 or more
                else if (s is ClippingSign) write = 81;
                else if (s is SnowDrift) write = 82;
                else if (s is StreetLight) write = 83;
                else if (s is PyramidBLight) write = 84;
                else if (s is TroubleLight) write = 85;
                else if (s is RaceSign) write = 86;
                else if (s is ArrowSign) write = 87;
                else if (s is DangerSign) write = 88;
                else if (s is EasySign) write = 99;
                else if (s is HardLeft) write = 90;
                else if (s is UpSign) write = 91;
                else if (s is VeryHardSign) write = 92;
                else if (s is SnowPile) write = 93;
                else if (s is WaterFallEdge) write = 94;
                else if (s is WaterFallEdgeTop) write = 95;
                //else if (s is ArcadeFrame) levBuffer.Write((byte)14);
                else levBuffer.Write((byte)255);
                levBuffer.Write(write);
                switch (s)
                {
                    case ClippingSign:
                        levBuffer.Write((byte)((ClippingSign)s).style.value);
                        break;
                    case SawsRight:
                    case SpikesRight:
                    case SpringDownRight:
                        levBuffer.Write((byte)3);
                        break;
                    case SawsLeft:
                    case SpikesLeft:
                    case SpringDownLeft:
                        levBuffer.Write((byte)2);
                        break;
                    case SawsDown:
                    case SpikesDown:
                    case SpringDown:
                        levBuffer.Write((byte)1);
                        break;
                    case SpringLeft:
                        //THESE .FLIPHORIZONTAL CHECKS ARE HERE BECAUSE DUCK GAME IS FUCKING DUMB
                        //let me explain, theres a city map with a springUpLeft flipped to be to the right
                        //so i had to add this dumbass edge case because springs worked when flipped
                        //yes this will make it so on the replay flipped springs will be replaced with their 
                        //other spring counterpart but honestly i could not care less since the gameplay aint
                        //gonna be affected its just some internal dumb shit, maybe it is affected because
                        //the collision can change a bit, also i think im going crazy -NiK0
                        if (s.flipHorizontal) levBuffer.Write((byte)5);
                        else levBuffer.Write((byte)4);
                        break;
                    case SpringRight:
                        if (s.flipHorizontal) levBuffer.Write((byte)4);
                        else levBuffer.Write((byte)5);
                        break;
                    case SpringUpLeft:
                        if (s.flipHorizontal) levBuffer.Write((byte)7);
                        else levBuffer.Write((byte)6);
                        break;
                    case SpringUpRight:
                        if (s.flipHorizontal) levBuffer.Write((byte)6);
                        else levBuffer.Write((byte)7);
                        break;
                    case Saws:
                    case Spikes:
                    case Spring:
                        levBuffer.Write((byte)0);
                        break;
                    case Altar:
                        levBuffer.Write((byte)((Altar)s).wide.value);
                        break;
                    default:
                        if (write >= 80)
                        {
                            levBuffer.Write((byte)(s.flipHorizontal ? 1 : 0));
                        }
                        else
                        {
                            levBuffer.Write((byte)255);
                        }
                        break;
                }
                levBuffer.Write(CompressedVec2Binding.GetCompressedVec2(s.position, 10000));
            }

            levBuffer.Write((ushort)TheThings.Count);
            for (int i = 0; i < TheThings.Count; i++)
            {
                Thing t = TheThings[i];
                byte wow = 0;
                switch (t.GetType().Name)
                {//idiotic
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
                levBuffer.Write(CompressedVec2Binding.GetCompressedVec2(t.position, 10000));
            }

            levBuffer.Write((ushort)StartingBlocks.Count);
            Type b1 = typeof(CustomTileset);
            Type b2 = typeof(CustomTileset2);
            Type b3 = typeof(CustomTileset3);
            for (int i = 0; i < StartingBlocks.Count; i++)
            {
                AutoBlock b = StartingBlocks[i];
                Type xd = b.GetType();
                if (xd == b1) array[1] = true;
                else if (xd == b2) array[2] = true;
                else if (xd == b3) array[3] = true;
                levBuffer.Write(b.blockIndex);
                levBuffer.Write(CompressedVec2Binding.GetCompressedVec2(b.position, 10000));
                levBuffer.Write(Recorderator.autoBlockIDX[b.GetType()]);
            }

            levBuffer.Write((ushort)StartingTiles.Count);
            for (int i = 0; i < StartingTiles.Count; i++)
            {
                AutoTile b = StartingTiles[i];
                Type xd = b.GetType();
                levBuffer.Write(CompressedVec2Binding.GetCompressedVec2(b.position, 10000));
                levBuffer.Write(Recorderator.autoTileIDX[b.GetType()]);
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
                levBuffer.Write(CompressedVec2Binding.GetCompressedVec2(p.position, 10000));
                levBuffer.Write(Recorderator.autoPlatIDX[p.GetType()]);
            }
            levBuffer.Write((ushort)BackgroundTiles.Count);
            Type cc = typeof(CustomBackground);
            Type ccTWO = typeof(CustomBackground2);
            Type ccTHREE = typeof(CustomBackground3);
            for (int i = 0; i < BackgroundTiles.Count; i++)
            {
                Thing t = BackgroundTiles[i];
                Type type = t.GetType();
                if (type == cc) array[7] = true;
                else if (type == ccTWO) array[8] = true;
                else if (type == ccTHREE) array[9] = true;
                levBuffer.Write(CompressedVec2Binding.GetCompressedVec2(t.position, 10000));

                int w = t.frame;
                BitArray br = new BitArray(16);
                br[0] = (w & 1024) > 0;
                br[1] = (w & 512) > 0;
                br[2] = (w & 256) > 0;
                br[3] = (w & 128) > 0;
                br[4] = (w & 64) > 0;
                br[5] = (w & 32) > 0;
                br[6] = (w & 16) > 0;
                br[7] = (w & 8) > 0;
                br[8] = (w & 4) > 0;
                br[9] = (w & 2) > 0;
                br[10] = (w & 1) > 0;

                br[15] = t.flipHorizontal;
                levBuffer.Write(BitCrusher.BitArrayToUShort(br));
                levBuffer.Write(Recorderator.bgtileIDX[t.GetType()]);
            }

            List<byte> Lbf = levBuffer.buffer.ToList();
            if (levBuffer.position + 13 < levBuffer.buffer.Count())
            {
                Lbf.RemoveRange(levBuffer.position + 10, levBuffer.buffer.Count() - levBuffer.position - 11);
            }
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

            buffer.Write(SFXToPlaySave.Count);
            for (int i = 0; i < SFXToPlaySave.Count; i++)
            {
                List<SoundData> sd = SFXToPlaySave[i];
                buffer.Write((ushort)sd.Count); //i was gonna go with a byte but just to be safe im going with ushort even though theres no way more than 255 sounds are gonna be played at once
                for (int x = 0; x < sd.Count; x++)
                {
                    SoundData s = sd[x];
                    buffer.Write(s.hash);
                    buffer.Write(BitCrusher.FloatToUShort(Maths.Clamp(s.volume, -4, 4) + 4, 8));
                    buffer.Write(BitCrusher.FloatToUShort(Maths.Clamp(s.pitch, -10, 10) + 10, 20));
                }
            }

            buffer.Write(BlocksBroken.Count);
            for (int i = 0; i < BlocksBroken.Count; i++)
            {
                BlockbreakData bd = BlocksBroken[i];
                buffer.Write(bd.Frame);
                if (bd.Data == null) buffer.Write((ushort)0);//just in case
                else
                {
                    buffer.Write((ushort)bd.Data.Count);
                    for (int x = 0; x < bd.Data.Count; x++)
                    {
                        buffer.Write(bd.Data.ElementAt(x));
                    }
                }
            }

            buffer.Write(chatMessages.Count);
            for (int i = 0; i < chatMessages.Count; i++)
            {
                KeyValuePair<int, List<ChatMessage>> lls = chatMessages.ElementAt(i);
                List<ChatMessage> list = lls.Value;
                buffer.Write((ushort)lls.Key);
                buffer.Write((ushort)list.Count);
                for (int x = 0; x < list.Count; x++)
                {
                    ChatMessage cm = list[x];
                    buffer.Write((byte)profiles.IndexOf(cm.who));
                    buffer.Write(cm.text);
                    buffer.Write(cm.index);
                }
            }




            //HAHHAHAHHAHAHHAHAHHAHAHAHHAHHAHAHAHHAHAHHAHAHHAHAHHAHAHAHHAHAHHAHAHHAHAHHAHAH -NIK0!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            List<byte> bf = buffer.buffer.ToList();
            if (buffer.position + 13 < buffer.buffer.Count())
            {
                bf.RemoveRange(buffer.position + 10, buffer.buffer.Count() - buffer.position - 11);
            }

            List<byte> bf2 = metadataBuffer.buffer.ToList();
            if (metadataBuffer.position + 13 < metadataBuffer.buffer.Count())
            {
                bf2.RemoveRange(metadataBuffer.position + 10, metadataBuffer.buffer.Count() - metadataBuffer.position - 11);
            }


            List<byte> bf4 = hatsPreviewBuffer.buffer.ToList();
            if (hatsPreviewBuffer.position + 13 < hatsPreviewBuffer.buffer.Count())
            {
                bf4.RemoveRange(hatsPreviewBuffer.position + 10, hatsPreviewBuffer.buffer.Count() - hatsPreviewBuffer.position - 11);
            }

            if (isThisReplayBroken)
            {
                File.WriteAllText(CordsPath + "broken_cord_" + DateTime.Now.ToString().Replace('/', '-').Replace(':', '-').Replace(' ', '-') + ".txt", crashStuff);
                File.WriteAllBytes(CordsPath + "broken_cord_" + DateTime.Now.ToString().Replace('/', '-').Replace(':', '-').Replace(' ', '-') + ".rdt", bf.ToArray());
            }
            else
            {
                string path = CordsPath + "cord_" + DateTime.Now.ToString().Replace('/', '-').Replace(':', '-').Replace(' ', '-') + ".crf";
                int V = 0;
                while (File.Exists(path))
                {
                    V++;
                    path = CordsPath + "cord_" + DateTime.Now.ToString().Replace('/', '-').Replace(':', '-').Replace(' ', '-') + "_" + V + ".crf";
                }
                using (MemoryStream customFileStream = new MemoryStream(bf.ToArray()))
                {
                    string zipFilePath = CordsPath + "cord_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".crf";

                    using (FileStream zipStream = new FileStream(zipFilePath, FileMode.Create))
                    {
                        using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
                        {
                            ZipArchiveEntry entry = archive.CreateEntry("replaydata.rdt");

                            using (Stream entryStream = entry.Open())
                            {
                                customFileStream.CopyTo(entryStream);
                            }
                            //Lbf


                            ZipArchiveEntry entry3 = archive.CreateEntry("level.lbf");
                            using (Stream entryStream3 = entry3.Open())
                            {
                                using (MemoryStream customFileStream3 = new MemoryStream(Lbf.ToArray()))
                                {
                                    customFileStream3.CopyTo(entryStream3);
                                }
                            }

                            ZipArchiveEntry entry4 = archive.CreateEntry("hatpreview.hpb");
                            using (Stream entryStream4 = entry4.Open())
                            {
                                using (MemoryStream customFileStream4 = new MemoryStream(bf4.ToArray()))
                                {
                                    customFileStream4.CopyTo(entryStream4);
                                }
                            }

                            //save metadata last -NiK0
                            // Second entry from bf2 with the name "metadata.rmt" -ChatGPT
                            ZipArchiveEntry entry2 = archive.CreateEntry("metadata.rmt");
                            using (Stream entryStream2 = entry2.Open())
                            {
                                using (MemoryStream customFileStream2 = new MemoryStream(bf2.ToArray()))
                                {
                                    customFileStream2.CopyTo(entryStream2);
                                }
                            }
                        }
                    }
                }
                //File.WriteAllBytes(path, bf.ToArray());
            }
            return bf.ToArray();
        }
        public int cFrame;

        public Dictionary<int, List<SoundData>> SFXToPlaySave = new Dictionary<int, List<SoundData>>();
        public int gamemodeStarted = -1;
        public List<Vec2> camPos = new List<Vec2>();
        public List<Vec2> camSize = new List<Vec2>();
        public List<SomethingSomethingVessel> somethings = new List<SomethingSomethingVessel>();
        public Map<int, Thing> somethingMap = new Map<int, Thing>();
        public Map<int, SomethingSomethingVessel> somethingMapped = new Map<int, SomethingSomethingVessel>();
        public bool PlayingThatShitBack;
        public static Corderator instance;
        public List<SoundData> toAddThisFrame = new List<SoundData>();
        public Dictionary<int, List<ChatMessage>> chatMessages = new Dictionary<int, List<ChatMessage>>();
        public void ReAddSomeVessel(SomethingSomethingVessel vessel)
        {
            if (vessel.t == null)
            {
                if (vessel.syncled.ContainsKey("position"))
                {
                    Level.Add(new NilVessel(vessel.syncled["position"], vessel.changeRemove, "Nulled, vessel name: " + vessel.editorName + " vessel reason:" + vessel.destroyedReason));
                    return;
                }
                Level.Add(new NilVessel(Vec2.Zero, "Nulled, vessel name: " + vessel.editorName + " vessel reason:" + vessel.destroyedReason));
                return;
            }
            if (somethingMap.Contains(vessel.t)) return;
            if (vessel.doIndex)
            {
                Main.SpecialCode = "ReAdding " + vessel.editorName + "\ndid it contain same index: " + somethingMap.ContainsKey(vessel.myIndex) + "\ndid it contain same thing: " + somethingMap.ContainsValue(vessel.t);
                Main.SpecialCode = "something something map";
                somethingMap.Add(vessel.myIndex, vessel.t);
                Main.SpecialCode = "something something mapPED";
                somethingMapped.Add(vessel.myIndex, vessel);
            }
            Main.SpecialCode = "something something level add not t";
            Level.Add(vessel);
            Main.SpecialCode = "something something level add t";
            Level.Add(vessel.t);
            Main.SpecialCode = "something something not in here no more";
        }
        public void MapSomeSomeVessel(SomethingSomethingVessel yes)
        {
            if (somethingMap.Contains(yes.t)) return;
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

        public List<ChatMessage> receivedMessages = new List<ChatMessage>();
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

                if (profiles.Count == 0)
                {
                    for (int i = 0; i < Profiles.alllist.Count; i++)
                    {
                        Profile pr = Profiles.alllist[i];
                        profiles.Add(pr);
                    }
                }
                
                if (DuckNetwork.active)
                {
                    List<ChatMessage> cms = new List<ChatMessage>();
                    for (int i = 0; i < receivedMessages.Count; i++)
                    {
                        ChatMessage cm = receivedMessages[i];
                        cms.Add(new ChatMessage(cm.who, cm.text, cm.index));
                    }
                    receivedMessages.Clear();
                    if (cms.Count > 0)
                    {
                        chatMessages.Add(cFrame, cms);
                    }
                }

                SFXToPlaySave.Add(cFrame, toAddThisFrame);
                toAddThisFrame = new List<SoundData>();
                if (GameMode.started && gamemodeStarted == -1)
                {
                    gamemodeStarted = cFrame;
                }
                if (cFrame == 1)
                {
                    List<Thing> allTheThings = Level.current.things.ToList();//still dumb but prolly better than what i was using before
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
                        else if (th is ForegroundTile ft) BackgroundTiles.Add(ft);
                        else if (th is AutoPlatform ap) AutoPlatforms.Add(ap);
                        else if (th is TreeTop || th is TreeTopDead || th is IBigStupidWall || th is PyramidWall || th is VerticalDoor || th is IceWedge)
                        {
                            TheThings.Add(th);
                        }
                        else if (th is WaterFlow wf)
                        {
                            if (wf.extraWater.Count > 0)
                            {
                                TheThings.AddRange(wf.extraWater);
                            }
                            TheThings.Add(wf);
                        }
                        else if (th is Saws || th is Spikes || th is Spring || th is ArcadeLight || th is PyramidLightRoof || th is PyramidWallLight || th is Bulb || th is HangingCityLight || th is Lamp || th is OfficeLight || th is WallLightRight || th is Sun || th is ArcadeTableLight || th is OfficeLight || th is WallLightLeft || th is FishinSign || th is MallardBillboard || th is ClippingSign || th is StreetLight || th is PyramidBLight || th is TroubleLight || th is RaceSign || th is ArrowSign || th is DangerSign || th is EasySign || th is HardLeft || th is UpSign || th is VeryHardSign || th is WaterCooler || th is Altar || th is SnowGenerator || th is SnowDrift || th is SnowPile || th is WaterFall || th is WaterFallEdge || th is WaterFallEdgeTop  || th is WaterFallTile) theLevelDetailsETC.Add(th);
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
                        else if (th is AutoTile at)
                        {
                            StartingTiles.Add(at);
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
                HashSet<ushort> ush = new HashSet<ushort>();
                for (int i = 0; i < StartingBlocks.Count; i++)
                {
                    AutoBlock ab = StartingBlocks[i];
                    if (ab.corderatorIndexedthemAlready) continue;

                    if (ab.shouldWreck || ab.removeFromLevel)
                    {
                        ush.Add(ab.blockIndex);
                        ab.corderatorIndexedthemAlready = true;
                    }
                }
                if (ush.Count > 0)
                {
                    BlocksBroken.Add(new BlockbreakData(cFrame, ush));
                }
                cFrame++;
            }
            base.Update();
        }
        public void PlayThatBack()
        {
            if (!fake)
            {
                if (SFXToPlaySave.ContainsKey(cFrame))
                {
                    List<SoundData> sd = SFXToPlaySave[cFrame];
                    for (int i = 0; i < sd.Count; i++)
                    {
                        SoundData s = sd[i];
                        SFX.Play(s.hash, s.volume, s.pitch);
                    }
                }
                if (chatMessages.Count > 0 && chatMessages.ContainsKey(cFrame))
                {
                    List<ChatMessage> cms = chatMessages[cFrame];
                    SFX.Play("chatmessage", 0.8f, Rando.Float(-0.15f, 0.15f));
                    for (int i = 0; i < cms.Count; i++)
                    {
                        DuckNetwork.core.AddChatMessage(cms[i]);
                    }
                }
                List<ChatMessage> chatMessageList = new List<ChatMessage>();
                foreach (ChatMessage chatMessage in DuckNetwork.core.chatMessages)
                {
                    chatMessage.timeout -= 0.016f;
                    if (chatMessage.timeout < 0)
                        chatMessage.alpha -= 0.01f;
                    if (chatMessage.alpha < 0)
                        chatMessageList.Add(chatMessage);
                }
                foreach (ChatMessage chatMessage in chatMessageList)
                    DuckNetwork.core.chatMessages.Remove(chatMessage);
                if (BlocksBroken.Count > 0 && BlocksBroken[0].Frame <= cFrame)
                {
                    BlockbreakData bd = BlocksBroken[0];
                    foreach (AutoBlock autoBlock in Level.current.things[typeof(AutoBlock)])
                    {
                        if (bd.Data.Contains(autoBlock.blockIndex))
                        {
                            bd.Data.Remove(autoBlock.blockIndex);
                            autoBlock.shouldWreck = true;
                            autoBlock.skipWreck = true;
                            if (!autoBlock.shouldbeinupdateloop)
                            {
                                Level.current.AddUpdateOnce(autoBlock);
                            }
                        }
                    }
                    BlocksBroken.RemoveAt(0);
                }
            }
            cFrame++;
            SFX.enabled = false;
            if (cFrame >= 0 && cFrame < maxFrame - 1)
            {
                Main.SpecialCode2 = "record frame " + cFrame;
                for (int i = 0; i < somethings.Count; i++)
                {
                    SomethingSomethingVessel ves = somethings[i];
                    if (ves.t != null) ves.t.shouldbegraphicculled = false;
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
                        if (ves.t != null) Level.Remove(ves.t);
                    }
                }
                Level.current.camera.LerpWithoutFear = true;
                Main.SpecialCode = "crash at camSize";
                Level.current.camera.width = camSize[cFrame].x;
                Level.current.camera.height = camSize[cFrame].y;
                Main.SpecialCode = "the pain is eternal";
                Level.current.camera.position = camPos[cFrame];
            }
            else
            {
                Recorderator.Playing = false;
                PlayingThatShitBack = false;
                if (Level.current is ReplayLevel rl && rl.prev != null) Level.current = rl.prev;
                else Level.current = new RecorderationSelector();
            }
            SFX.enabled = true;
            //do sfx stuff here
        }
    }
}