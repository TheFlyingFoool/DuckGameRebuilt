using System;
using System.Collections;
using System.Collections.Generic;

namespace DuckGame
{
    public class ReplayLevel : Level
    {
        public void DeserializeLevel(BitBuffer b)
        {
            //this is dumb
            byte bited = b.ReadByte();

            reAdd.Add(CCorderr);
            //DevConsole.Log(bited.ToString(), Color.Blue);
            if (bited != 255)
            {
                BackgroundUpdater bu = (BackgroundUpdater)Editor.CreateThing(Recorderator.bgIDX[bited]);
                bu.y = -8000;
                reAdd.Add(bu);
            }
            Main.SpecialCode = "pipetilesets";
            int x = b.ReadUShort();
            for (int i = 0; i < x; i++)
            {
                Vec2 v = b.ReadVec2();
                byte shorted = b.ReadByte();
                BitArray a = new BitArray(new byte[] { shorted });
                PipeTileset p = null;
                int d = 16;
                int f = 0;
                for (int y = 0; y < 5; y++)
                {
                    if (a[y]) f += d;
                    d /= 2;
                }
                d = 2;
                int wow = 0;
                for (int y = 5; y < 7; y++)
                {
                    if (a[y]) wow += d;
                    d /= 2;
                }
                if (wow == 0) p = new PipeRed(v.x, v.y);
                else if (wow == 1) p = new PipeBlue(v.x, v.y);
                else if (wow == 2) p = new PipeGreen(v.x, v.y);
                p._sprite.frame = f;
                p.background.value = a[7];
                
                //Extensions.SetPrivateFieldValue(p, "searchUp", a[1]);
                //Extensions.SetPrivateFieldValue(p, "searchDown", a[2]);
                //Extensions.SetPrivateFieldValue(p, "searchLeft", a[3]);
                //Extensions.SetPrivateFieldValue(p, "searchRight", a[4]);
                reAdd.Add(p);
            }

            Main.SpecialCode = "teleporters";
            x = b.ReadUShort();
            for (int i = 0; i < x; i++)
            {
                Vec2 v = b.ReadVec2();
                byte b1 = b.ReadByte();
                byte b2 = b.ReadByte();
                BitArray a = new BitArray(new byte[] { b1, b2 });
                Teleporter t = new Teleporter(v.x, v.y);
                t.noduck = a[0];
                t.horizontal = a[1];
                int f = 0;
                int d = 4;
                for (int y = 2; y < 5; y++)
                {
                    if (a[y]) f += d;
                    d /= 2;
                }
                t.direction = d;
                f = 0;
                d = 16;
                for (int y = 5; y < 10; y++)
                {
                    if (a[y]) f += d;
                    d /= 2;
                }
                t.teleHeight = f;
                /* br[0] = t.noduck;
                br[1] = t.horizontal;
                br[2] = (z & 4) > 0;
                br[3] = (z & 2) > 0;
                br[4] = (z & 1) > 0;
                br[5] = (x & 16) > 0;
                br[6] = (x & 8) > 0;
                br[7] = (x & 4) > 0;
                br[8] = (x & 2) > 0;
                br[9] = (x & 1) > 0;*/
                reAdd.Add(t);
            }


            Main.SpecialCode = "springs/saws/spikes";
            x = b.ReadUShort();
            for (int i = 0; i < x; i++)
            {
                byte bitedTHREE = b.ReadByte();
                byte bitedTWO = b.ReadByte();
                Main.SpecialCode2 = "btr: " + bitedTHREE + "  btwo: " + bitedTWO;
                Vec2 v = b.ReadVec2();
                Thing somethingthing;
                DevConsole.Log(bitedTHREE);
                switch (bitedTHREE)
                {
                    case 0:
                        {
                            switch (bitedTWO)
                            {
                                case 0:
                                    somethingthing = new Saws(v.x, v.y);
                                    break;
                                case 1:
                                    somethingthing = new SawsDown(v.x, v.y);
                                    break;
                                case 2:
                                    somethingthing = new SawsLeft(v.x, v.y);
                                    break;
                                case 3:
                                    somethingthing = new SawsRight(v.x, v.y);
                                    break;
                                case 255:
                                default:
                                    continue;
                            }
                            break;
                        }
                    case 1:
                        {
                            switch (bitedTWO)
                            {
                                case 0:
                                    somethingthing = new Spikes(v.x, v.y);
                                    break;
                                case 1:
                                    somethingthing = new SpikesDown(v.x, v.y);
                                    break;
                                case 2:
                                    somethingthing = new SpikesLeft(v.x, v.y);
                                    break;
                                case 3:
                                    somethingthing = new SpikesRight(v.x, v.y);
                                    break;
                                case 255:
                                default:
                                    continue;
                            }
                            break;
                        }
                    case 2:
                        {
                            switch (bitedTWO)
                            {
                                case 0:
                                    somethingthing = new Spring(v.x, v.y);
                                    break;
                                case 1:
                                    somethingthing = new SpringDown(v.x, v.y);
                                    break;
                                case 2:
                                    somethingthing = new SpringDownLeft(v.x, v.y);
                                    break;
                                case 3:
                                    somethingthing = new SpringDownRight(v.x, v.y);
                                    break;
                                case 4:
                                    somethingthing = new SpringLeft(v.x, v.y);
                                    break;
                                case 5:
                                    somethingthing = new SpringRight(v.x, v.y);
                                    break;
                                case 6:
                                    somethingthing = new SpringUpLeft(v.x, v.y);
                                    break;
                                case 7:
                                    somethingthing = new SpringUpRight(v.x, v.y);
                                    break;
                                case 255:
                                default:
                                    continue;
                            }
                            break;
                        }
                    case 3:
                        {
                            somethingthing = new ArcadeLight(v.x, v.y);
                            break;
                        }
                    case 4:
                        {
                            somethingthing = new PyramidLightRoof(v.x, v.y);
                            break;
                        }
                    case 5:
                        {
                            somethingthing = new PyramidWallLight(v.x, v.y);
                            break;
                        }
                    case 6:
                        {
                            somethingthing = new Bulb(v.x, v.y);
                            break;
                        }
                    case 7:
                        {
                            somethingthing = new HangingCityLight(v.x, v.y);
                            break;
                        }
                    case 8:
                        {
                            somethingthing = new Lamp(v.x, v.y);
                            break;
                        }
                    case 9:
                        {
                            somethingthing = new OfficeLight(v.x, v.y);
                            break;
                        }
                    case 10:
                        {
                            somethingthing = new WallLightRight(v.x, v.y);
                            break;
                        }
                    case 11:
                        {
                            somethingthing = new Sun(v.x, v.y);
                            break;
                        }
                    case 12:
                        {
                            somethingthing = new ArcadeTableLight(v.x, v.y);
                            break;
                        }
                    case 13:
                        {
                            somethingthing = new OfficeLight(v.x, v.y);
                            break;
                        }
                    case 14:
                        {
                            somethingthing = new WallLightLeft(v.x, v.y);
                            break;
                        }
                    case 15:
                        {
                            somethingthing = new FishinSign(v.x, v.y);
                            break;
                        }
                    case 16:
                        {
                            somethingthing = new MallardBillboard(v.x, v.y);
                            break;
                        }
                    case 17:
                        {
                            somethingthing = new ClippingSign(v.x, v.y);
                            ((ClippingSign)somethingthing).style.value = bitedTWO;
                            break;
                        }
                    case 19:
                        {
                            somethingthing = new StreetLight(v.x, v.y) { flipHorizontal = bitedTWO > 0 };
                            break;
                        }
                    case 20:
                        {
                            somethingthing = new PyramidBLight(v.x, v.y) { flipHorizontal = bitedTWO > 0 };
                            break;
                        }
                    case 21:
                        {
                            somethingthing = new TroubleLight(v.x, v.y) { flipHorizontal = bitedTWO > 0 };
                            break;
                        }
                    case 22:
                        {
                            somethingthing = new RaceSign(v.x, v.y) { flipHorizontal = bitedTWO > 0 };
                            break;
                        }
                    case 23:
                        {
                            somethingthing = new ArrowSign(v.x, v.y) { flipHorizontal = bitedTWO > 0 };
                            break;
                        }
                    case 24:
                        {
                            somethingthing = new DangerSign(v.x, v.y) { flipHorizontal = bitedTWO > 0 };
                            break;
                        }
                    case 25:
                        {
                            somethingthing = new EasySign(v.x, v.y) { flipHorizontal = bitedTWO > 0 };
                            break;
                        }
                    case 26:
                        {
                            somethingthing = new HardLeft(v.x, v.y) { flipHorizontal = bitedTWO > 0 };
                            break;
                        }
                    case 27:
                        {
                            somethingthing = new UpSign(v.x, v.y) { flipHorizontal = bitedTWO > 0 };
                            break;
                        }
                    case 28:
                        {
                            somethingthing = new VeryHardSign(v.x, v.y) { flipHorizontal = bitedTWO > 0 };
                            break;
                        }
                    default:
                        continue;
                }
                Main.SpecialCode = "a weird crash";
                if (somethingthing == null) continue;
                reAdd.Add(somethingthing);
            }
            Main.SpecialCode2 = "";

            Main.SpecialCode = "rprops";
            x = b.ReadUShort();
            for (int i = 0; i < x; i++)
            {
                byte z = b.ReadByte();
                BitArray br = new BitArray(new byte[] { z });

                int switched = 0;
                int divide = 8;
                for (int y = 0; y < 4; y++)
                {
                    if (br[y]) switched += divide;
                    divide /= 2;
                }
                Vec2 v = b.ReadVec2();
                Thing t;
                switch (switched)
                {
                    case 0:
                        t = new CityWall(v.x, v.y);
                        break;
                    case 1:
                        t = new TreeTop(v.x, v.y);
                        break;
                    case 2:
                        t = new TreeTopDead(v.x, v.y);
                        break;
                    case 3:
                        t = new RockWall(v.x, v.y);
                        break;
                    case 4:
                        t = new PyramidWall(v.x, v.y);
                        break;
                    case 5:
                        t = new VerticalDoor(v.x, v.y);
                        break;
                    case 6:
                        t = new PyramidDoor(v.x, v.y);
                        break;
                    case 7:
                        t = new IceWedge(v.x, v.y, 0);
                        break;
                    case 8:
                        t = new CityRamp(v.x, v.y, 0);
                        break;
                    case 9:
                        t = new WaterFlow(v.x, v.y);
                        break;
                    default:
                        reAdd.Add(new NilVessel(v, "lev vessel: " + switched + " idx: " + i));
                        continue;
                }
                t.flipVertical = br[6];
                t.flipHorizontal = br[7];
                reAdd.Add(t);
            }

            //List<AutoBlock> itakemyleave = new List<AutoBlock>();
            Main.SpecialCode = "block s";
            x = b.ReadUShort();
            for (int i = 0; i < x; i++)
            {
                //DevConsole.Log("chocoloate ", Color.Blue);
                Vec2 v = b.ReadVec2();
                byte bote = b.ReadByte();

                AutoBlock bb = (AutoBlock)Editor.CreateThing(Recorderator.autoBlockIDX[bote]);
                bb.blockIndex = (ushort)i;
                bb.position = v;
                //itakemyleave.Add(bb);
                //DevConsole.Log("added block to position " + v, Color.Blue);

                reAdd.Add(bb);
            }

            Main.SpecialCode = "platform s";
            x = b.ReadUShort();
            for (int i = 0; i < x; i++)
            {
                Vec2 v = b.ReadVec2();
                byte bote = b.ReadByte();

                AutoPlatform pp = (AutoPlatform)Editor.CreateThing(Recorderator.autoPlatIDX[bote]);
                pp.position = v;
                //itakemyleave.Add(bb);
                //DevConsole.Log("added platform to position " + v, Color.Blue);

                reAdd.Add(pp);
            }

            Main.SpecialCode = "its in here somewhere";
            x = b.ReadUShort();
            for (int i = 0; i < x; i++)
            {
                Main.SpecialCode2 = "posRead";
                Vec2 position = b.ReadVec2();
                Main.SpecialCode2 = "FrameRead";
                byte Frame = b.ReadByte();
                Main.SpecialCode2 = "bgTileIndex Read";
                byte bgTileIndex = b.ReadByte();

                BackgroundTile bgTiles = (BackgroundTile)Editor.CreateThing(Recorderator.bgtileIDX[bgTileIndex]);
                bgTiles.position = position;

                Main.SpecialCode2 = "Bit array creation";
                BitArray b_arr = new BitArray(new byte[] { Frame });
                int lol = 0;
                int divide = 64;
                for (int y = 0; y < 7; y++)
                {
                    Main.SpecialCode2 = "ll Loop " + y;
                    if (b_arr[y]) lol += divide;
                    divide /= 2;
                }
                Main.SpecialCode2 = "Something went wrong here";
                bgTiles.frame = lol;
                bgTiles.flipHorizontal = b_arr[7];
                reAdd.Add(bgTiles);
            }
            Main.SpecialCode2 = "";
            Main.SpecialCode = "out of lev buffer";
        }
        public List<Thing> reAdd = new List<Thing>();
        public Corderator CCorderr;
        public override void Update()
        {
            Recorderator.Playing = true;
            if (Input.Pressed("START", "Any"))
            {
                //do gui stuff
            }
            if (CCorderr.cFrame == 0)
            {
                List<AutoBlock> autoBlocks = Extensions.GetListOfThings<AutoBlock>();
                for (int i = 0; i < autoBlocks.Count; i++) autoBlocks[i].PlaceBlock();
                List<AutoPlatform> autoPlatforms = Extensions.GetListOfThings<AutoPlatform>();
                for (int i = 0; i < autoPlatforms.Count; i++) autoPlatforms[i].PlaceBlock();
            }
            base.Update();
        }
        public override void Initialize()
        {
            for (int i = 0; i < reAdd.Count; i++)
            {
                Add(reAdd[i]);
            }
            base.Initialize();
        }
    }
}
