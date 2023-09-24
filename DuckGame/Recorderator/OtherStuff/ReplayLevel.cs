using System;
using System.Collections;
using System.Collections.Generic;
using static DuckGame.CMD;

namespace DuckGame
{
    public class ReplayLevel : Level, IHaveAVirtualTransition
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
                Vec2 v = CompressedVec2Binding.GetUncompressedVec2(b.ReadInt(), 10000);
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
                p._initializedConnections = false;
                p.searchUp = true;
                p.searchDown = true;
                p.searchLeft = true;
                p.searchRight = true;

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
                Vec2 v = CompressedVec2Binding.GetUncompressedVec2(b.ReadInt(), 10000);
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
                t.direction = f; //idk how i messed this up lol
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
                byte byteOne = b.ReadByte();
                byte dataByte = b.ReadByte();
                Main.SpecialCode2 = "btr: " + byteOne + "  btwo: " + dataByte;
                Vec2 v = CompressedVec2Binding.GetUncompressedVec2(b.ReadInt(), 10000);
                Thing somethingthing = null;
                switch (byteOne)
                {
                    case 0:
                        {
                            switch (dataByte)
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
                            switch (dataByte)
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
                            switch (dataByte)
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
                            somethingthing = new WaterCooler(v.x, v.y);
                            break;
                        }
                    case 17:
                        {
                            Altar alt = new Altar(v.x, v.y, 0);
                            alt.wide.value = dataByte;
                            somethingthing = alt;
                            break;
                        }
                    case 18:
                        {
                            somethingthing = new SnowGenerator(v.x, v.y);
                            somethingthing.visible = false;
                            break;
                        }
                    case 19:
                        {
                            somethingthing = new WaterFall(v.x, v.y);
                            break;
                        }
                    case 20:
                        {
                            somethingthing = new WaterFallTile(v.x, v.y);
                            break;
                        }
                    case 80:
                        {
                            somethingthing = new MallardBillboard(v.x, v.y);
                            break;
                        }
                    case 81:
                        {
                            somethingthing = new ClippingSign(v.x, v.y);
                            ((ClippingSign)somethingthing).style.value = dataByte;
                            break;
                        }
                    case 82:
                        {
                            somethingthing = new SnowDrift(v.x, v.y, 0) { flipHorizontal = dataByte > 0 };
                            break;
                        }
                    case 83:
                        {
                            somethingthing = new StreetLight(v.x, v.y) { flipHorizontal = dataByte > 0 };
                            break;
                        }
                    case 84:
                        {
                            somethingthing = new PyramidBLight(v.x, v.y) { flipHorizontal = dataByte > 0 };
                            break;
                        }
                    case 85:
                        {
                            somethingthing = new TroubleLight(v.x, v.y) { flipHorizontal = dataByte > 0 };
                            break;
                        }
                    case 86:
                        {
                            somethingthing = new RaceSign(v.x, v.y) { flipHorizontal = dataByte > 0 };
                            break;
                        }
                    case 87:
                        {
                            somethingthing = new ArrowSign(v.x, v.y) { flipHorizontal = dataByte > 0 };
                            break;
                        }
                    case 88:
                        {
                            somethingthing = new DangerSign(v.x, v.y) { flipHorizontal = dataByte > 0 };
                            break;
                        }
                    case 89:
                        {
                            somethingthing = new EasySign(v.x, v.y) { flipHorizontal = dataByte > 0 };
                            break;
                        }
                    case 90:
                        {
                            somethingthing = new HardLeft(v.x, v.y) { flipHorizontal = dataByte > 0 };
                            break;
                        }
                    case 91:
                        {
                            somethingthing = new UpSign(v.x, v.y) { flipHorizontal = dataByte > 0 };
                            break;
                        }
                    case 92:
                        {
                            somethingthing = new VeryHardSign(v.x, v.y) { flipHorizontal = dataByte > 0 };
                            break;
                        }
                    case 93:
                        {
                            somethingthing = new SnowPile(v.x, v.y, 0) { flipHorizontal = dataByte > 0 };
                            break;
                        }
                    case 94:
                        {
                            somethingthing = new WaterFallEdge(v.x, v.y) { flipHorizontal = dataByte > 0 };
                            break;
                        }
                    case 95:
                        {
                            somethingthing = new WaterFallEdgeTop(v.x, v.y) { flipHorizontal = dataByte > 0 };
                            break;
                        }
                    default:
                        DevConsole.Log("|RED|RECORDERATOR |WHITE|A non indexed prop was found :" + byteOne);
                        break;
                }
                /*if (s is Saws) write = 0;
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

                else if (s is MallardBillboard) write = 81;//special case for  80 or more
                else if (s is ClippingSign) write = 82;
                else if (s is SnowDrift) write = 83;
                else if (s is StreetLight) write = 84;
                else if (s is PyramidBLight) write = 85;
                else if (s is TroubleLight) write = 86;
                else if (s is RaceSign) write = 87;
                else if (s is ArrowSign) write = 88;
                else if (s is DangerSign) write = 89;
                else if (s is EasySign) write = 90;
                else if (s is HardLeft) write = 91;
                else if (s is UpSign) write = 92;
                else if (s is VeryHardSign) write = 93;
                else if (s is SnowPile) write = 94;
                 * */
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
                Vec2 v = CompressedVec2Binding.GetUncompressedVec2(b.ReadInt(), 10000);
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
                ushort indx = b.ReadUShort();
                Vec2 v = CompressedVec2Binding.GetUncompressedVec2(b.ReadInt(), 10000);
                byte bote = b.ReadByte();

                AutoBlock bb = (AutoBlock)Editor.CreateThing(Recorderator.autoBlockIDX[bote]);
                bb.blockIndex = indx;
                bb.position = v;
                //itakemyleave.Add(bb);
                //DevConsole.Log("added block to position " + v, Color.Blue);

                reAdd.Add(bb);
            }

            Main.SpecialCode = "tile s";
            x = b.ReadUShort();
            for (int i = 0; i < x; i++)
            {
                Vec2 v = CompressedVec2Binding.GetUncompressedVec2(b.ReadInt(), 10000);
                byte bote = b.ReadByte();

                AutoTile bb = (AutoTile)Editor.CreateThing(Recorderator.autoTileIDX[bote]);
                bb.position = v;

                reAdd.Add(bb);
            }

            Main.SpecialCode = "platform s";
            x = b.ReadUShort();
            for (int i = 0; i < x; i++)
            {
                Vec2 v = CompressedVec2Binding.GetUncompressedVec2(b.ReadInt(), 10000);
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
                Vec2 position = CompressedVec2Binding.GetUncompressedVec2(b.ReadInt(), 10000);
                Main.SpecialCode2 = "FrameRead";
                ushort Frame = b.ReadUShort();
                Main.SpecialCode2 = "bgTileIndex Read";
                byte bgTileIndex = b.ReadByte();

                //replaced this with a thing because ForegroundTile doesn't inherit BackgroundTile resulting in a crash when casting those to BackgroundTiles
                Thing bgTiles = Editor.CreateThing(Recorderator.bgtileIDX[bgTileIndex]);
                bgTiles.position = position;

                Main.SpecialCode2 = "Bit array creation";
                BitArray b_arr = new BitArray(16);
                BitCrusher.UShortIntoArray(Frame, ref b_arr);

                int val = 0;
                int divide = 1024;
                for (int y = 0; y < 11; y++)
                {
                    Main.SpecialCode2 = "ll Loop " + y;
                    if (b_arr[y]) val += divide;
                    divide /= 2;
                }
                Main.SpecialCode2 = "Something went wrong here";
                bgTiles.frame = val;
                bgTiles.flipHorizontal = b_arr[15];
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
            if (CCorderr.cFrame == 0)
            {
                List<AutoBlock> autoBlocks = Extensions.GetListOfThings<AutoBlock>();
                for (int i = 0; i < autoBlocks.Count; i++) autoBlocks[i].PlaceBlock();
                List<AutoPlatform> autoPlatforms = Extensions.GetListOfThings<AutoPlatform>();
                for (int i = 0; i < autoPlatforms.Count; i++) autoPlatforms[i].PlaceBlock();
                List<AutoTile> autoTiles = Extensions.GetListOfThings<AutoTile>();
                for (int i = 0; i < autoTiles.Count; i++) autoTiles[i].PlaceBlock();
            }
            base.Update();
        }
        public override void PostDrawLayer(Layer layer)
        {
            if (layer == Layer.Console)
            {
                //TODO this
                /*List<ChatMessage> ms = Corderator.instance.chatMessages;
                float num6 = 0.1f;
                foreach (ChatMessage chatMessage in ms)
                {
                    float num7 = 10 * (Options.Data.chatHeadScale + 1) * num3;
                    _core._chatFont._currentConnection = chatMessage.who.connection == localConnection ? null : chatMessage.who.connection;
                    _core._chatFont.scale = new Vec2(Resolution.fontSizeMultiplier * chatMessage.scale * chatScale);
                    _core._chatFont.scale = new Vec2(num3, num3) * chatMessage.scale;
                    if (_core._chatFont is RasterFont)
                        _core._chatFont.scale = new Vec2(0.5f);
                    float x = (float)(_core._chatFont.GetWidth(chatMessage.text) + num7 + 8 * chatScale);
                    if (chatMessage.who.slotType == SlotType.Spectator)
                    {
                        if (_core._chatFont is RasterFont)
                        {
                            float num8 = (float)((_core._chatFont as RasterFont).data.fontSize * RasterFont.fontScaleFactor / 10);
                            x += 6f * num8;
                        }
                        else x += 8f * _core._chatFont.scale.x;
                    }
                    float y = chatMessage.newlines * (_core._chatFont.characterHeight + 2) * _core._chatFont.scale.y;
                    Vec2 p1 = new Vec2(14f, num1 + (vec2_1.y - (y + 10f)));
                    Vec2 p2 = p1 + new Vec2(x, y);
                    Graphics.DrawRect(p1 + new Vec2(-1f, -1f), p2 + new Vec2(1f, 1f), Color.Black * 0.8f * chatMessage.alpha * num4, (Depth)(num6 - 0.0015f), false, 1f * chatScale);
                    float num9 = (0.3f + chatMessage.text.Length * 0.007f);
                    if (num9 > 0.5f)
                        num9 = 0.5f;
                    if (chatMessage.slide > 0.8f)
                        chatMessage.scale = Lerp.FloatSmooth(chatMessage.scale, 1f, 0.1f, 1.1f);
                    else if (chatMessage.slide > 0.5f)
                        chatMessage.scale = Lerp.FloatSmooth(chatMessage.scale, 1f + num9, 0.1f, 1.1f);
                    chatMessage.slide = Lerp.FloatSmooth(chatMessage.slide, 1f, 0.1f, 1.1f);
                    Color color = Color.White;
                    Color black = Color.Black;
                    if (chatMessage.who.persona != null)
                    {
                        color = chatMessage.who.persona.colorUsable;
                        if (chatMessage.who.persona == Persona.Duck2)
                        {
                            color.r += 30;
                            color.g += 30;
                            color.b += 30;
                        }
                        if (chatMessage.who.slotType == SlotType.Spectator)
                            color = Colors.DGPurple;
                        SpriteMap g = null;
                        SpriteMap chatBust = chatMessage.who.persona.chatBust;
                        Vec2 vec2_2 = Vec2.Zero;
                        if (chatMessage.who.team != null && chatMessage.who.team.hasHat && (chatMessage.who.connection != localConnection || !chatMessage.who.team.locked))
                        {
                            vec2_2 = chatMessage.who.team.hatOffset * num3 * (Options.Data.chatHeadScale + 1);
                            g = chatMessage.who.team.GetHat(chatMessage.who.persona);
                        }
                        bool flag = chatMessage.who.netData.Get<bool>("quack");
                        if (chatMessage.who.duck != null && !chatMessage.who.duck.dead && !chatMessage.who.duck.removeFromLevel)
                            flag = chatMessage.who.duck.quack > 0;
                        Vec2 vec2_3 = new Vec2(p1.x, p1.y + -2 * (Options.Data.chatHeadScale + 1));
                        if (g != null)
                        {
                            g.CenterOrigin();
                            g.depth = (Depth)(num6 - 1f / 1000f);
                            g.alpha = chatMessage.alpha * num4;
                            g.frame = !flag || g.texture == null || g.texture.width <= 32 ? 0 : 1;
                            g.scale = new Vec2(num3, num3) * (Options.Data.chatHeadScale + 1);
                            Graphics.Draw(g, vec2_3.x - vec2_2.x, vec2_3.y - vec2_2.y);
                            g.scale = new Vec2(1f, 1f);
                            g.alpha = 1f;
                        }
                        chatBust.frame = !flag ? 0 : 1;
                        chatBust.depth = (Depth)(num6 - 0.0015f);
                        chatBust.alpha = chatMessage.alpha * num4;
                        chatBust.scale = new Vec2(num3, num3) * (Options.Data.chatHeadScale + 1);
                        Graphics.Draw(chatBust, vec2_3.x + 2f * chatBust.scale.x, vec2_3.y + 5f * chatBust.scale.y);
                        color *= 0.85f;
                        color.a = byte.MaxValue;
                    }
                    Graphics.DrawRect(p1, p2, color * 0.85f * chatMessage.alpha * num4, (Depth)(num6 - 1f / 500f));
                    _core._chatFont.symbolYOffset = 4f;
                    _core._chatFont.lineGap = 2f;
                    if (chatMessage.who.slotType == SlotType.Spectator)
                        _core._chatFont.Draw("@SPECTATORBIG@" + chatMessage.text, p1 + new Vec2(2f + num7, 1f * _core._chatFont.scale.y), black * chatMessage.alpha * num4, (Depth)1f);
                    else
                        _core._chatFont.Draw(chatMessage.text, p1 + new Vec2(2f + num7, 1f * _core._chatFont.scale.y), black * chatMessage.alpha * num4, (Depth)1f);
                    _core._chatFont._currentConnection = null;
                    num1 -= y + 4f;
                    num6 -= 0.01f;
                    if (num2 == 0)
                        break;
                    --num2;
                }*/
            }
            base.PostDrawLayer(layer);
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
