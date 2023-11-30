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
        private Dictionary<Profile, MemberData> _data = new Dictionary<Profile, MemberData>();
        private List<Profile> remove = new List<Profile>();

        public HostTable(float pX, float pY)
          : base(pX, pY, new Sprite("hostTable"))
        {
            layer = Layer.HUD;
            graphic.CenterOrigin();
            center = new Vec2(graphic.w / 2, 0f);
            _chair = new Sprite("hostChair");
            _beverage = new SpriteMap("beverages", 16, 18);
            _chair.CenterOrigin();
            _crown = new Sprite("hostCrown");
            _crown.CenterOrigin();
            _fakeLevelCore = new LevelCore
            {
                currentLevel = new Level()
            };
        }

        public override void Update()
        {
            skipLayerAdding = true;
            loop = true;
            bool networkActive = Network.activeNetwork._networkActive;
            Network.activeNetwork._networkActive = false;
            LevelCore core = Level.core;
            Level.core = _fakeLevelCore;
            foreach (Profile profile in DuckNetwork.profiles)
            {
                if (profile.slotType == SlotType.Spectator && profile.connection != null && !spectators.Contains(profile) && profile.team != null)
                    spectators.Add(profile);
            }
            foreach (Profile spectator in spectators)
            {
                if (spectator.connection == null || spectator.slotType != SlotType.Spectator || spectator.team == null)
                {
                    remove.Add(spectator);
                }
                else
                {
                    MemberData data = GetData(spectator);
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
                        data.duck.depth = depth - 20;
                        data.ai.virtualDevice = new VirtualInput(0);
                        data.ai.virtualQuack = true;
                        data.duck.connection = spectator.connection;
                        spectator.duck = null;
                        spectator.inputProfile = inputProfile;
                    }
                    bool flag = spectator.netData.Get("quack", false);
                    if (flag && !data.quack)
                        data.ai.Press(Triggers.Quack);
                    else if (flag && data.quack)
                        data.ai.HoldDown(Triggers.Quack);
                    else if (!flag && data.quack)
                        data.ai.Release(Triggers.Quack);
                    data.ai.virtualDevice.rightStick = spectator.netData.Get("spectatorTongue", Vec2.Zero);
                    data.ai.virtualDevice.leftTrigger = spectator.netData.Get("quackPitch", 0f);
                    if (spectator.team.hasHat && data.duck.hat == null)
                    {
                        TeamHat e = new TeamHat(0f, 0f, spectator.team);
                        Level.Add(e);
                        data.duck.Equip(e, false);
                    }
                    data.quack = flag;
                }
            }
            foreach (Profile profile in remove)
            {
                MemberData data = GetData(profile);
                if (data.duck != null)
                {
                    if (data.duck.hat != null)
                        Level.Remove(data.duck.hat);
                    Level.Remove(data.duck);
                    _data.Remove(profile);
                }
                spectators.Remove(profile);
            }
            remove.Clear();
            Level.current.things.RefreshState();
            Level.current.UpdateThings();
            Level.core = core;
            Network.activeNetwork._networkActive = networkActive;
            loop = false;
            skipLayerAdding = false;
        }

        private MemberData GetData(Profile pProfile)
        {
            MemberData data;
            if (!_data.TryGetValue(pProfile, out data))
                _data[pProfile] = data = new MemberData();
            return data;
        }

        public override void Draw()
        {
            bool networkActive = Network.activeNetwork._networkActive;
            Network.activeNetwork._networkActive = false;
            LevelCore core = Level.core;
            Level.core = _fakeLevelCore;
            foreach (Thing thing in Level.current.things)
            {
                if (thing is Duck)
                {
                    (thing as Duck).UpdateConnectionIndicators();
                    (thing as Duck).DrawConnectionIndicators();
                }
                thing.DoDraw();
            }
            float num1 = spectators.Count * 22;
            float x = (float)(this.x - num1 / 2f + 10f);
            int num2 = 0;
            foreach (Profile spectator in spectators)
            {
                MemberData data = GetData(spectator);
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
                bool flag = num2 >= spectators.Count / 2;
                if (spectator.netData.Get("spectatorFlip", false))
                    flag = !flag;
                if (data.beverage != -1)
                {
                    _beverage.frame = data.beverage;
                    float num4 = (num2 * num2 * 5.4041653f % 1f * 7f);
                    int num5 = 1;
                    if (num2 == 1 || num2 == 2)
                        num5 = 0;
                    Graphics.Draw(_beverage, (x - num4 + (num2 >= spectators.Count / 2 ? 5f : -16f)), (y - 15f + 16f * data.beverageLerp) + num5, data.beverageLerp < 0.05f ? depth + 1 : depth - 1);
                }
                if (spectator == DuckNetwork.hostProfile)
                    Graphics.Draw(_crown, x, y + 2f, depth + 2);
                Vec2 vec2_1 = new Vec2(x, y - 2f);
                vec2_1 += new Vec2(data.tilt.x, (-data.tilt.y * 0.25f)) * 4f;
                Vec2 vec2_2 = vec2_1;
                Vec2 bob = data.bob;
                if (bob.y < 0f)
                    bob.y *= 1.6f;
                Vec2 vec2_3 = vec2_2 + new Vec2(bob.x, (float)(-bob.y * 1.5)) * 4f;
                data.tilt = Lerp.Vec2Smooth(data.tilt, spectator.netData.Get("spectatorTilt", Vec2.Zero), 0.15f);
                data.bob = Lerp.Vec2Smooth(data.bob, spectator.netData.Get("spectatorBob", Vec2.Zero), 0.15f);
                spectator.netData.Get("quack", false);
                if (data.duck != null)
                {
                    data.duck.position = vec2_3 + new Vec2(0f, -5f);
                    data.duck.offDir = flag ? (sbyte)-1 : (sbyte)1;
                }
                _chair.flipH = flag;
                Graphics.Draw(_chair, vec2_1.x - (flag ? -4f : 4f), vec2_1.y - 4f, depth - 40);
                x += num1 / spectators.Count;
                ++num2;
            }
            Level.core = core;
            Network.activeNetwork._networkActive = networkActive;
            if (spectators.Count <= 0)
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
