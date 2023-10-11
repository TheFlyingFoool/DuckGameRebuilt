using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class DuckVessel : SomethingSomethingVessel
    {
        public DuckVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Duck));

            AddSynncl("hold", new SomethingSync(typeof(ushort)));
            AddSynncl("position", new SomethingSync(typeof(int)));

            AddSynncl("infoed", new SomethingSync(typeof(ushort)));
            AddSynncl("holdang", new SomethingSync(typeof(ushort)));

            AddSynncl("input", new SomethingSync(typeof(ushort)));
            AddSynncl("converted", new SomethingSync(typeof(ushort)));

            AddSynncl("tongue", new SomethingSync(typeof(int)));
            AddSynncl("indicators", new SomethingSync(typeof(byte)));
        }
        public ulong ID; 
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            byte prof = b.ReadByte();
            Profile p = Corderator.instance.profiles[prof];
            ushort ush = b.ReadUShort();
            DuckVessel dv = new DuckVessel(new Duck(0, -2000, p) { invincible = true }) { p = p };
            for (int i = 0; i < ush; i++)
            {
                int z = b.ReadInt();
                BitBuffer bf = b.ReadBitBuffer(false);
                bf.position = 0;

                StoredItem storedItem = new StoredItem();
                BinaryClassChunk bChunk = BinaryClassChunk.FromData<BinaryClassChunk>(bf);
                storedItem.serializedData = bChunk;
                storedItem.thing = LoadThing(bChunk);

                dv.changesInPB.Add(z, storedItem);
            }
            return dv;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            prevBuffer.Write((byte)Corderator.instance.profiles.IndexOf(((Duck)t).profile));
            prevBuffer.Write((ushort)changesInPB.Count);
            for (int i = 0; i < changesInPB.Count; i++)
            {
                prevBuffer.Write(changesInPB.ElementAt(i).Key);
                prevBuffer.Write(changesInPB.ElementAt(i).Value.serializedData.GetData());
            }
            return prevBuffer;
        }
        public int lastObj;
        public Holdable lastHold;
        public Profile p;
        
        public bool STRAFE;
        public bool RAGDOLL;
        public bool GRAB;
        public bool SHOOT;
        public bool LEFT;
        public bool RIGHT;
        public bool DOWN;
        public bool UP;
        public bool QUACK;
        public bool JUMP;
        public override void Draw()
        {
            if (playBack && Corderator.instance != null)
            {
                if (Keyboard.Down(Keys.LeftShift))
                {
                    string inputstring = "";
                    int rLength = 0;
                    if (LEFT) Graphics.DrawString("@LEFT@", new Vec2(t.left - 8, t.y), Color.White, 1);
                    if (RIGHT) Graphics.DrawString("@RIGHT@", new Vec2(t.right+2, t.y), Color.White, 1);
                    if (DOWN) Graphics.DrawString("@DOWN@", new Vec2(t.x, t.y), Color.White, 1);
                    if (STRAFE)
                    {
                        inputstring += "@STRAFE@";
                        rLength++;
                    }
                    if (JUMP)
                    {
                        inputstring += "@JUMP@";
                        rLength++;
                    }
                    if (QUACK)
                    {
                        inputstring += "@QUACK@";
                        rLength++;
                    }
                    if (UP)
                    {
                        inputstring += "@UP@";
                        rLength++;
                    }
                    if (GRAB)
                    {
                        inputstring += "@GRAB@";
                        rLength++;
                    }
                    if (SHOOT)
                    {
                        inputstring += "@SHOOT@";
                        rLength++;
                    }
                    if (RAGDOLL)
                    {
                        inputstring += "@RAGDOLL@";
                        rLength++;
                    }
                    Graphics.DrawString(inputstring, new Vec2(t.x - rLength * 4, t.top - 16), Color.White, 1);
                }
            }
            base.Draw();
        }

        public int pbIndex;
        public bool spawnedPlusOne;
        public override void PlaybackUpdate()
        {
            Duck d = (Duck)t;
            if (changesInPB.Count > pbIndex && changesInPB.ElementAt(pbIndex).Key <= exFrames)
            {
                StoredItem st = changesInPB.ElementAt(pbIndex).Value;
                if (Level.core._storedItems.ContainsKey(d.profile)) Level.core._storedItems[d.profile] = st;
                else Level.core._storedItems.Add(d.profile, st);
                pbIndex++;
            }
            d.position = CompressedVec2Binding.GetUncompressedVec2((int)valOf("position"), 10000);
            ushort infodByte = (ushort)valOf("infoed");
            ushort z = (ushort)valOf("input");
            d.holdAngleOff = Maths.DegToRad(BitCrusher.UShortToFloat((ushort)valOf("holdang"), 720) - 360);
            int hObj = (ushort)valOf("hold") - 1;



            d.tounge = CompressedVec2Binding.GetUncompressedVec2((int)valOf("tongue"), 10000);

            d.velocity = Vec2.Zero;
            d.updatePhysics = false;


            BitArray b_ARR = new BitArray(16);
            BitCrusher.UShortIntoArray(infodByte, ref b_ARR);
            int divide = 16;
            int frame = 0;
            for (int i = 0; i < 5; i++)
            {
                if (b_ARR[i]) frame += divide;
                divide /= 2;
            }


            d._hovering = b_ARR[5];
            d.spriteImageIndex = (byte)frame;
            d.offDir = (sbyte)(b_ARR[6] ? 1 : -1);

            d.visible = b_ARR[7];
            int current = 0;
            if (b_ARR[8]) current += 8;
            if (b_ARR[9]) current += 4;
            if (b_ARR[10]) current += 2;
            if (b_ARR[11]) current += 1;

            d.quack = b_ARR[12] ? 20 : 0;

            if (!d.dead && b_ARR[13])
            {
                if (d.GetPos().y > Level.current.camera.bottom)
                {
                    Vec2 pos = d.GetEdgePos();
                    Vec2 dir = (pos - d.GetPos()).normalized;
                    for (int i = 0; i < DGRSettings.ActualParticleMultiplier * 8; i++)
                    {
                        Feather feather = Feather.New(pos.x - dir.x * 16f, pos.y - dir.y * 16f, d.persona);
                        feather.hSpeed += dir.x * 1f;
                        feather.vSpeed += dir.y * 1f;
                        Level.Add(feather);
                    }
                }
                else
                {
                    for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 8; ++index)
                        Level.Add(Feather.New(d.cameraPosition.x, d.cameraPosition.y, d.persona));
                }
            }
            d.dead = b_ARR[13];

            bool stuck = b_ARR[14];

            if (d.currentPlusOne == null && b_ARR[15] && !spawnedPlusOne)
            {
                spawnedPlusOne = true;
                PlusOne pls = new PlusOne(0, 0, d.profile, false, true);
                pls._duck = d;
                pls.anchor = d;
                pls.anchor.offset = new Vec2(0f, -16f);
                pls.depth = (Depth)0.95f;
                d.currentPlusOne = pls;
                Level.Add(pls);
            }
            switch (current)
            {
                case 0:
                    d._sprite.currentAnimation = "idle";
                    break;
                case 1:
                    d._sprite.currentAnimation = "run";
                    break;
                case 2:
                    d._sprite.currentAnimation = "jump";
                    break;
                case 3:
                    d._sprite.currentAnimation = "slide";
                    break;
                case 4:
                    d._sprite.currentAnimation = "crouch";
                    break;
                case 5:
                    d._sprite.currentAnimation = "groundSlide";
                    break;
                case 6:
                    d._sprite.currentAnimation = "dead";
                    break;
                case 7:
                    d._sprite.currentAnimation = "netted";
                    break;
                case 8:
                    d._sprite.currentAnimation = "listening";
                    break;
            }
            d._sprite.speed = 0;

            Main.SpecialCode = "future Looking";

            if (syncled["hold"].Count > 1 && (ushort)syncled["hold"][1] != 0 && bArray[0])
            {
                int indx = ((int)(ushort)(syncled["hold"][1]) - 1);
                Main.SpecialCode = "reaching into the future, index " + indx;
                if (Corderator.instance.somethingMapped.Contains(indx)) Corderator.instance.somethingMapped[indx].skipPositioning = 1;
            }

            Main.SpecialCode = "not looking into the future";
            if (hObj == -1 && lastHold != null)
            {
                lastHold.owner = null;
                d.holdObject = null;
                if (lastObj != -1)
                {
                    Corderator.instance.somethingMapped[lastObj].skipPositioning = 1;
                }
            }
            else if (hObj != -1 && lastHold == null && Corderator.instance.somethingMap.Contains(hObj))
            {
                d.GiveHoldable((Holdable)Corderator.instance.somethingMap[hObj]);
            }
            if (d.holdObject != null) d.holdObject.owner = d;
            lastHold = d.holdObject;
            lastObj = hObj;

            if (d._trapped != null && d._trapped.active)
            {
                d.visible = true;
            }



            d.listening = (z & 2048) > 0;
            bool val = (z & 1024) > 0;
            if (val && !d.localSpawnVisible)
            {
                Vec3 color = d.profile.persona.color;
                Level.Add(new SpawnLine(d.x, d.y, 0, 0f, new Color((int)color.x, (int)color.y, (int)color.z), 32f));
                Level.Add(new SpawnLine(d.x, d.y, 0, -4f, new Color((int)color.x, (int)color.y, (int)color.z), 4f));
                Level.Add(new SpawnLine(d.x, d.y, 0, 4f, new Color((int)color.x, (int)color.y, (int)color.z), 4f));
                Level.Add(new SpawnAimer(d.x, d.y, 0, 4f, new Color((int)color.x, (int)color.y, (int)color.z), d.persona, 4f));
                SFX.Play("pullPin", 0.7f);
            }
            d.localSpawnVisible = val;


            if (d.ragdoll != null)
            {
                d.ragdoll.part1.enablePhysics = false;
                d.ragdoll.part2.enablePhysics = false;
                d.ragdoll.part3.enablePhysics = false;
                if (stuck) d.ragdoll.tongueStuck = d.tounge;
                else d.ragdoll.tongueStuck = Vec2.Zero;
            }

            d.cordHover = d._hovering;
            if (d._hovering) d.UpdateAnimation();

            STRAFE = (z & 512) > 0;
            RAGDOLL = (z & 256) > 0;
            GRAB = (z & 128) > 0;
            SHOOT = (z & 64) > 0;
            LEFT = (z & 32) > 0;
            RIGHT = (z & 16) > 0;
            DOWN = (z & 8) > 0;
            UP = (z & 4) > 0;
            QUACK = (z & 2) > 0;
            JUMP = (z & 1) > 0;



            int convert = (ushort)valOf("converted") - 1;
            if (convert != lastConvert && Corderator.instance != null && Corderator.instance.somethingMap.Contains(convert))
            {
                lastConvert = convert;
                d.isConversionMessage = true;
                d.ConvertDuck((Duck)Corderator.instance.somethingMap[convert]);
            }

            BitArray indicators = new BitArray(new byte[] { (byte)valOf("indicators") });
            /*
             * if (d.profile.netData != null)
            {
                indicators[0] = d.profile.netData.Get<bool>("gamePaused");
                indicators[1] = d.profile.netData.Get<bool>("gameInFocus");
                indicators[2] = d.profile.netData.Get<bool>("chatting");
                indicators[3] = d.profile.netData.Get<bool>("consoleOpen");
            }
            if (d.connection != null && d.connection != DuckNetwork.localConnection)
            {
                indicators[4] = d.connection.isExperiencingConnectionTrouble;
                indicators[5] = d.connection.manager.ping > 0.25f;
                indicators[6] = d.connection.manager.accumulatedLoss > 10;
            }
            indicators[7] = d.afk;
            */
            d.forcePaused = indicators[0];
            d.forceTabbed = indicators[1];
            d.forceChatting = indicators[2];
            d.forceConsole = indicators[3];
            d.forceDisconnection = indicators[4];
            d.forceLag = indicators[5];
            d.forceLoss = indicators[6];
            d.forceAFK = indicators[7];

            base.PlaybackUpdate();
        }
        public int lastConvert = -1;
        public StoredItem sd;

        public Dictionary<int, StoredItem> changesInPB = new Dictionary<int, StoredItem>();
        public override void RecordUpdate()
        {
            Duck d = (Duck)t;

            StoredItem rsd = PurpleBlock.GetStoredItem(d.profile);
            if (rsd != null && rsd.serializedData != null)
            {
                if (sd == null || rsd.serializedData != sd.serializedData)
                {
                    StoredItem STOARD = new StoredItem();
                    STOARD.serializedData = BinaryClassChunk.FromData<BinaryClassChunk>(rsd.serializedData.GetData());
                    changesInPB.Add(exFrames, STOARD);
                }
                sd = new StoredItem() { serializedData = rsd.serializedData };
            }

            addVal("position", CompressedVec2Binding.GetCompressedVec2(d.position, 10000));
            byte b;
            switch (d._sprite.currentAnimation)
            {
                default:
                case "idle":
                    b = 0;
                    break;
                case "run":
                    b = 1;
                    break;
                case "jump":
                    b = 2;
                    break;
                case "slide":
                    b = 3;
                    break;
                case "crouch":
                    b = 4;
                    break;
                case "groundSlide":
                    b = 5;
                    break;
                case "dead":
                    b = 6;
                    break;
                case "netted":
                    b = 7;
                    break;
                case "listening":
                    b = 8;
                    break;
            }

            BitArray b_ARR = new BitArray(16);
            int z = d.spriteImageIndex;

            Main.SpecialCode = "coded 1-arr";
            b_ARR[0] = (z & 16) > 0;
            b_ARR[1] = (z & 8) > 0;
            b_ARR[2] = (z & 4) > 0;
            b_ARR[3] = (z & 2) > 0;
            b_ARR[4] = (z & 1) > 0;
            b_ARR[5] = d._hovering;
            b_ARR[6] = d.offDir > 0;
            b_ARR[7] = d.visible;
            b_ARR[8] = (b & 8) > 0;
            b_ARR[9] = (b & 4) > 0;
            b_ARR[10] = (b & 2) > 0;
            b_ARR[11] = (b & 1) > 0;
            b_ARR[12] = d.quack > 0;
            b_ARR[13] = d.dead;
            b_ARR[14] = (d.ragdoll != null && d.ragdoll.tongueStuckThing != null);
            b_ARR[15] = d.currentPlusOne != null;

            Main.SpecialCode = "coded 1-weird";
            addVal("infoed", BitCrusher.BitArrayToUShort(b_ARR));


            if (d.holdObject != null && Corderator.instance != null && Corderator.instance.somethingMap.Contains(d.holdObject)) addVal("hold", (ushort)(Corderator.instance.somethingMap[d.holdObject] + 1));
            else addVal("hold", (ushort)0);

            float f = Maths.RadToDeg(d.holdAngleOff) % 360;
            addVal("holdang", BitCrusher.FloatToUShort(f + 360, 720));

            ushort value = 0;

            if (d.listening) value |= 2048;
            if (d.localSpawnVisible) value |= 1024;
            if (d.inputProfile.Down("STRAFE") || d.inputProfile.Pressed("STRAFE")) value |= 512;
            if (d.inputProfile.Down("RAGDOLL") || d.inputProfile.Pressed("RAGDOLL")) value |= 256;
            if (d.inputProfile.Down("GRAB") || d.inputProfile.Pressed("GRAB")) value |= 128;
            if (d.inputProfile.Down("SHOOT") || d.inputProfile.Pressed("SHOOT")) value |= 64;
            if (d.inputProfile.Down("LEFT") || d.inputProfile.Pressed("LEFT")) value |= 32;
            if (d.inputProfile.Down("RIGHT") || d.inputProfile.Pressed("RIGHT")) value |= 16;
            if (d.inputProfile.Down("DOWN") || d.inputProfile.Pressed("DOWN")) value |= 8;
            if (d.inputProfile.Down("UP") || d.inputProfile.Pressed("UP")) value |= 4;
            if (d.inputProfile.Down("QUACK") || d.inputProfile.Pressed("QUACK")) value |= 2;
            if (d.inputProfile.Down("JUMP") || d.inputProfile.Pressed("JUMP")) value |= 1;
            addVal("input", value);

            Vec2 vc = d.tounge;
            if (d.ragdoll != null && d.ragdoll.tongueStuckThing != null) vc = d.ragdoll.tongueStuck;
            addVal("tongue", CompressedVec2Binding.GetCompressedVec2(vc, 10000));
            if (d.converted != null && Corderator.instance != null && Corderator.instance.somethingMap.Contains(d.converted)) addVal("converted", (ushort)(Corderator.instance.somethingMap[d.converted] + 1));
            else addVal("converted", (ushort)0);

            BitArray indicators = new BitArray(8);
            if (d.profile.netData != null)
            {
                indicators[0] = d.profile.netData.Get<bool>("gamePaused");
                indicators[1] = d.profile.netData.Get<bool>("gameInFocus");
                indicators[2] = d.profile.netData.Get<bool>("chatting");
                indicators[3] = d.profile.netData.Get<bool>("consoleOpen");
            }
            if (d.connection != null && d.connection != DuckNetwork.localConnection)
            {
                indicators[4] = d.connection.isExperiencingConnectionTrouble;
                indicators[5] = d.connection.manager.ping > 0.25f;
                indicators[6] = d.connection.manager.accumulatedLoss > 10;
            }
            indicators[7] = d.afk;
            addVal("indicators", BitCrusher.BitArrayToByte(indicators));
        }
    }
}
