// Decompiled with JetBrains decompiler
// Type: DuckGame.HostTable
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class HostTable : Thing
    {
        private Sprite _crown;
        private Sprite _chair;
        private SpriteMap _beverage;
        private LevelCore _fakeLevelCore;
        public static bool loop;
        private List<Profile> spectators = new List<Profile>();
        private Dictionary<Profile, HostTable.MemberData> _data = new Dictionary<Profile, HostTable.MemberData>();
        private List<Profile> remove = new List<Profile>();

        public HostTable(float pX, float pY)
          : base(pX, pY, new Sprite("hostTable"))
        {
            this.layer = Layer.HUD;
            this.graphic.CenterOrigin();
            this.center = new Vec2(this.graphic.w / 2, 0f);
            this._chair = new Sprite("hostChair");
            this._beverage = new SpriteMap("beverages", 16, 18);
            this._chair.CenterOrigin();
            this._crown = new Sprite("hostCrown");
            this._crown.CenterOrigin();
            this._fakeLevelCore = new LevelCore
            {
                currentLevel = new Level()
            };
        }

        public override void Update()
        {
            Thing.skipLayerAdding = true;
            HostTable.loop = true;
            bool networkActive = Network.activeNetwork._networkActive;
            Network.activeNetwork._networkActive = false;
            LevelCore core = Level.core;
            Level.core = this._fakeLevelCore;
            foreach (Profile profile in DuckNetwork.profiles)
            {
                if (profile.slotType == SlotType.Spectator && profile.connection != null && !this.spectators.Contains(profile) && profile.team != null)
                    this.spectators.Add(profile);
            }
            foreach (Profile spectator in this.spectators)
            {
                if (spectator.connection == null || spectator.slotType != SlotType.Spectator || spectator.team == null)
                {
                    this.remove.Add(spectator);
                }
                else
                {
                    HostTable.MemberData data = this.GetData(spectator);
                    if (data.duck == null)
                    {
                        InputProfile inputProfile = spectator.inputProfile;
                        data.duck = new Duck(100f, 0f, spectator)
                        {
                            enablePhysics = false
                        };
                        Level.Add(data.duck);
                        data.duck.mindControl = data.ai = new DuckAI();
                        data.duck.derpMindControl = false;
                        data.duck.depth = this.depth - 20;
                        data.ai.virtualDevice = new VirtualInput(0);
                        data.ai.virtualQuack = true;
                        data.duck.connection = spectator.connection;
                        spectator.duck = null;
                        spectator.inputProfile = inputProfile;
                    }
                    bool flag = spectator.netData.Get<bool>("quack", false);
                    if (flag && !data.quack)
                        data.ai.Press("QUACK");
                    else if (flag && data.quack)
                        data.ai.HoldDown("QUACK");
                    else if (!flag && data.quack)
                        data.ai.Release("QUACK");
                    data.ai.virtualDevice.rightStick = spectator.netData.Get<Vec2>("spectatorTongue", Vec2.Zero);
                    data.ai.virtualDevice.leftTrigger = spectator.netData.Get<float>("quackPitch", 0f);
                    if (spectator.team.hasHat && data.duck.hat == null)
                    {
                        TeamHat e = new TeamHat(0f, 0f, spectator.team);
                        Level.Add(e);
                        data.duck.Equip(e, false);
                    }
                    data.quack = flag;
                }
            }
            foreach (Profile profile in this.remove)
            {
                HostTable.MemberData data = this.GetData(profile);
                if (data.duck != null)
                {
                    if (data.duck.hat != null)
                        Level.Remove(data.duck.hat);
                    Level.Remove(data.duck);
                    this._data.Remove(profile);
                }
                this.spectators.Remove(profile);
            }
            this.remove.Clear();
            Level.current.things.RefreshState();
            Level.current.UpdateThings();
            Level.core = core;
            Network.activeNetwork._networkActive = networkActive;
            HostTable.loop = false;
            Thing.skipLayerAdding = false;
        }

        private HostTable.MemberData GetData(Profile pProfile)
        {
            MemberData data;
            if (!this._data.TryGetValue(pProfile, out data))
                this._data[pProfile] = data = new HostTable.MemberData();
            return data;
        }

        public override void Draw()
        {
            bool networkActive = Network.activeNetwork._networkActive;
            Network.activeNetwork._networkActive = false;
            LevelCore core = Level.core;
            Level.core = this._fakeLevelCore;
            foreach (Thing thing in Level.current.things)
            {
                if (thing is Duck)
                {
                    (thing as Duck).UpdateConnectionIndicators();
                    (thing as Duck).DrawConnectionIndicators();
                }
                thing.DoDraw();
            }
            float num1 = this.spectators.Count * 22;
            float x = (float)((double)this.x - (double)num1 / 2.0 + 10.0);
            int num2 = 0;
            foreach (Profile spectator in this.spectators)
            {
                HostTable.MemberData data = this.GetData(spectator);
                sbyte num3 = spectator.netData.Get<sbyte>("spectatorBeverage", -1);
                if (data.beverage != num3)
                {
                    data.beverageLerp = Lerp.FloatSmooth(data.beverageLerp, 1f, 0.2f, 1.1f);
                    if (data.beverageLerp >= 1f)
                        data.beverage = num3;
                }
                else
                {
                    data.beverageLerp = Lerp.FloatSmooth(data.beverageLerp, 0f, 0.2f, 1.1f);
                    if (data.beverageLerp < 0.05f)
                        data.beverageLerp = 0f;
                }
                bool flag = num2 >= this.spectators.Count / 2;
                if (spectator.netData.Get<bool>("spectatorFlip", false))
                    flag = !flag;
                if (data.beverage != -1)
                {
                    this._beverage.frame = data.beverage;
                    float num4 = (num2 * num2 * 5.4041653f % 1f * 7f);
                    int num5 = 1;
                    if (num2 == 1 || num2 == 2)
                        num5 = 0;
                    Graphics.Draw(_beverage, (x - num4 + (num2 >= this.spectators.Count / 2 ? 5f : -16f)), (this.y - 15f + 16f * data.beverageLerp) + num5, data.beverageLerp < 0.05f ? this.depth + 1 : this.depth - 1);
                }
                if (spectator == DuckNetwork.hostProfile)
                    Graphics.Draw(this._crown, x, this.y + 2f, this.depth + 2);
                Vec2 vec2_1 = new Vec2(x, this.y - 2f);
                vec2_1 += new Vec2(data.tilt.x, (-data.tilt.y * 0.25f)) * 4f;
                Vec2 vec2_2 = vec2_1;
                Vec2 bob = data.bob;
                if (bob.y < 0f)
                    bob.y *= 1.6f;
                Vec2 vec2_3 = vec2_2 + new Vec2(bob.x, (float)(-(double)bob.y * 1.5)) * 4f;
                data.tilt = Lerp.Vec2Smooth(data.tilt, spectator.netData.Get<Vec2>("spectatorTilt", Vec2.Zero), 0.15f);
                data.bob = Lerp.Vec2Smooth(data.bob, spectator.netData.Get<Vec2>("spectatorBob", Vec2.Zero), 0.15f);
                spectator.netData.Get<bool>("quack", false);
                if (data.duck != null)
                {
                    data.duck.position = vec2_3 + new Vec2(0f, -5f);
                    data.duck.offDir = flag ? (sbyte)-1 : (sbyte)1;
                }
                this._chair.flipH = flag;
                Graphics.Draw(this._chair, vec2_1.x - (flag ? -4f : 4f), vec2_1.y - 4f, this.depth - 40);
                x += num1 / spectators.Count;
                ++num2;
            }
            Level.core = core;
            Network.activeNetwork._networkActive = networkActive;
            if (this.spectators.Count <= 0)
                return;
            base.Draw();
        }

        private class MemberData
        {
            public bool quack;
            public Vec2 tongueLerp;
            public Vec2 tongueSlowLerp;
            public Vec2 tilt;
            public Vec2 bob;
            public Duck duck;
            public DuckAI ai;
            public int beverage;
            public float beverageLerp;
        }
    }
}
