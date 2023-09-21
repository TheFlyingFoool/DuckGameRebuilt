using System.Collections;
using System.Linq;

namespace DuckGame
{
    //only place with actually readable code
    //because i was having a headache when figuring out
    //how to optimize serialization and deserialization with
    //the bitarray bullshit
    public class DuckVessel : SomethingSomethingVessel
    {
        public DuckVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Duck));

            //MOVED HOLD TO THE TOP TO KEEP THE bArray CONSISTENT TO 0
            AddSynncl("hold", new SomethingSync(typeof(ushort)));//x
            AddSynncl("position", new SomethingSync(typeof(int)));

            AddSynncl("infoed", new SomethingSync(typeof(byte)));
            //AddSynncl("trappedpos", new SomethingSync(typeof(Vec2)));//x
            //AddSynncl("trappedowner", new SomethingSync(typeof(int)));//x
            AddSynncl("infoed_2", new SomethingSync(typeof(byte)));

            //vec 6 4*2*3 8*3 24 bytes per rpos
            AddSynncl("rpos", new SomethingSync(typeof(Vec6)));


            AddSynncl("holdang", new SomethingSync(typeof(ushort)));//x
            AddSynncl("input", new SomethingSync(typeof(ushort)));//8

            AddSynncl("tongue", new SomethingSync(typeof(int)));
            //65536
            //
            //1 2 4 8 16 32 64 128 256 512 1024 2048 4096 8192 16384 32768
        }
        public ulong ID;
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            byte persona = b.ReadByte();
            string pName = b.ReadString();
            ID = b.ReadULong();
            p = new Profile(pName, null, null, Persona.all.ElementAt(persona));
            DuckVessel dv = new DuckVessel(new Duck(0, -2000, p) { invincible = true }) { p = p };
            return dv;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            prevBuffer.Write((byte)((Duck)t).persona.index);
            prevBuffer.Write(((Duck)t).profile.name);
            prevBuffer.Write(((Duck)t).profile.steamID);
            return prevBuffer;
        }
        public int prevNetOwner;
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
        public override void DoUpdateThing()
        {
            Duck d = (Duck)t;
            d.DoUpdate();
            if (d.ragdoll != null)
            {
                d.ragdoll.DoUpdate();
                d.ragdoll.part1.DoUpdate();
                d.ragdoll.part2.DoUpdate();
                d.ragdoll.part3.DoUpdate();
            }
            /*if (d._trapped != null)
            {
                d._trapped.DoUpdate();
            }*/
        }
        public override void Draw()
        {
            if (playBack && Corderator.instance != null)
            {
                if (Corderator.instance.cFrame < 160 && p != null)
                {
                    string z = Program.RemoveColorTags(p.name);
                    Graphics.DrawStringOutline(z, new Vec2(t.x - z.Length * 4, t.top - 8), p.persona.colorUsable, Color.Black, 1);
                }
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
        public override void PlaybackUpdate()
        {
            Duck d = (Duck)t;
            d.position = CompressedVec2Binding.GetUncompressedVec2((int)valOf("position"), 10000);
            byte b = (byte)valOf("infoed");
            ushort z = (ushort)valOf("input");
            d.holdAngleOff = Maths.DegToRad(BitCrusher.UShortToFloat((ushort)valOf("holdang"), 360));
            int hObj = (ushort)valOf("hold") - 1;
            Vec6 v6 = (Vec6)valOf("rpos");
            //Vec2 tPos = (Vec2)valOf("trappedpos");
            //int tOwner = (int)valOf("trappedowner");

            byte Q = (byte)valOf("infoed_2");
            BitArray brI = new BitArray(new byte[] { Q });

            int current = 0;
            int div = 8;
            for (int i = 0; i < 4; i ++)
            {
                if (brI[i]) current += div;
                div /= 2;
            }
            d.quack = brI[4] ? 20 : 0;

            if (!d.dead && brI[5])
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
                        Level.Add(Feather.New(cameraPosition.x, cameraPosition.y, d.persona));
                }
            }
            d.dead = brI[5];

            bool stuck = brI[6];

            if (d.currentPlusOne == null && brI[7])
            {
                PlusOne pls = new PlusOne(0, 0, d.profile, false, true);
                pls._duck = d;
                pls.anchor = d;
                pls.anchor.offset = new Vec2(0f, -16f);
                pls.depth = (Depth)0.95f;
                d.currentPlusOne = pls;
                Level.Add(pls);
            }


            d.tounge = CompressedVec2Binding.GetUncompressedVec2((int)valOf("tongue"), 10000);

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

            //d._spriteArms.imageIndex = armframe;

            d._sprite.ClearAnimations();
            d.velocity = Vec2.Zero;
            d.updatePhysics = false;


            BitArray b_ARR = new BitArray(new byte[] { b });
            int divide = 16;
            int frame = 0;
            for (int i = 0; i < 5; i++)
            {
                if (b_ARR[i]) frame += divide;
                divide /= 2;
            }


            d._hovering = b_ARR[5];
            d.spriteImageIndex = (byte)frame;
            //d._spriteArms.imageIndex = d._sprite.imageIndex;
            d.offDir = (sbyte)(b_ARR[6]?1:-1);

            //(ushort)valOf("hold") - 1

            //look into the future to see if the holdogjbecfojsdghsdfidf
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

            Vec2 r1 = new Vec2(v6.a, v6.b);
            Vec2 r2 = new Vec2(v6.c, v6.d);
            Vec2 r3 = new Vec2(v6.e, v6.f);
            if (r1.y > -1500)
            {
                d.GoRagdoll();
                d.ragdoll.active = false;
            }
            else if (d.ragdoll != null) d.ragdoll.Unragdoll();

            d.visible = b_ARR[7];
            if (d._trapped != null && d._trapped.active)
            {
                d.visible = true;
            }

            if (d.ragdoll != null)
            {
                d.ragdoll.inSleepingBag = (z & 1024) > 0;
                d.ragdoll.part1.enablePhysics = false;
                d.ragdoll.part2.enablePhysics = false;
                d.ragdoll.part3.enablePhysics = false;
                d.ragdoll.part1.position = r1;
                d.ragdoll.part2.position = r2;
                d.ragdoll.part3.position = r3;
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


            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Duck d = (Duck)t;
            if (d == null)
            {
                //theres a null crash on the duck vessel class that happens once in a blue moon
                //so now i have to null check fucking everything
                DevConsole.Log("|RED|RECORDERATOR WENT INCREDIBLY WRONG!!");
                return;
            }
            addVal("position", CompressedVec2Binding.GetCompressedVec2(d.position, 10000));
            BitArray b_ARR = new BitArray(8);
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

            Main.SpecialCode = "coded 1-weird";
            addVal("infoed", BitCrusher.BitArrayToByte(b_ARR));
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

            BitArray brI = new BitArray(8);
            brI[0] = (b & 8) > 0;
            brI[1] = (b & 4) > 0;
            brI[2] = (b & 2) > 0;
            brI[3] = (b & 1) > 0;
            brI[4] = d.quack > 0;
            brI[5] = d.dead;
            brI[6] = (d.ragdoll != null && d.ragdoll.tongueStuckThing != null);
            brI[7] = d.currentPlusOne != null;

            addVal("infoed_2", BitCrusher.BitArrayToByte(brI));

            if (d._ragdollInstance != null)
            {
                Main.SpecialCode = "coded 1-ragdoll";
                Vec6 v = new Vec6();
                v.a = d._ragdollInstance.part1.x;
                v.b = d._ragdollInstance.part1.y;
                v.c = d._ragdollInstance.part2.x;
                v.d = d._ragdollInstance.part2.y;
                v.e = d._ragdollInstance.part3.x;
                v.f = d._ragdollInstance.part3.y;
                addVal("rpos", v);
            }
            else
            {
                addVal("rpos", new Vec6(0, -2000, 0, -2000, 0, -2000));   
            }
            /*if (d._trapped != null)
            {
                Main.SpecialCode = "coded 1-trapped";
                addVal("trappedpos", d._trapped.position);
                Main.SpecialCode = "coded 2-trapped";
                if (d._trapped.owner != null && Corderator.instance.somethingMap.Contains(d._trapped.owner))
                {
                    Main.SpecialCode = "coded 3-trapped";
                    addVal("trappedowner", Corderator.instance.somethingMap[d._trapped.owner]);
                }
                else addVal("trappedowner", -1);
            }
            else
            {
                addVal("trappedpos", new Vec2(0, -2000));
                addVal("trappedowner", -1);
            }*/
            Main.SpecialCode = "coded 1-hold";
            if (d.holdObject != null && Corderator.instance.somethingMap.Contains(d.holdObject))
            {
                Main.SpecialCode = "coded 2-hold";
                addVal("hold", (ushort)(Corderator.instance.somethingMap[d.holdObject] + 1));
            }
            else addVal("hold", (ushort)0);

            Main.SpecialCode = "coded 1-comic";
            float f = Maths.RadToDeg(d.holdAngleOff) % 360;
            addVal("holdang", BitCrusher.FloatToUShort(f, 360));

            Main.SpecialCode = "coded 2-comic";
            ushort value = 0;
            if (d._ragdollInstance != null && d._ragdollInstance.inSleepingBag) value |= 1024;
            Main.SpecialCode = "coded 3-comic";
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
        }
    }
}
