// Decompiled with JetBrains decompiler
// Type: DuckGame.QuadLaserBullet
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class QuadLaserBullet : Thing, ITeleport
    {
        public StateBinding _positionBinding = new CompressedVec2Binding("position", doLerp: true);
        public StateBinding _travelBinding = new CompressedVec2Binding(nameof(travel), 20);
        protected Vec2 _travel;
        protected SinWaveManualUpdate _wave = (SinWaveManualUpdate)0.5f;
        protected SinWaveManualUpdate _wave2 = (SinWaveManualUpdate)1f;
        public int safeFrames;
        public Duck safeDuck;
        public float timeAlive;

        public Vec2 travel
        {
            get => _travel;
            set => _travel = value;
        }

        public QuadLaserBullet(float xpos, float ypos, Vec2 travel)
          : base(xpos, ypos)
        {
            _travel = travel;
            collisionOffset = new Vec2(-1f, -1f);
            _collisionSize = new Vec2(2f, 2f);
        }

        public override void Update()
        {
            _wave.Update();
            _wave2.Update();
            timeAlive += 0.016f;
            position += _travel * 0.5f;
            if (isServerForObject && (x > Level.current.bottomRight.x + 200 || x < Level.current.topLeft.x - 200))
                Level.Remove(this);
            foreach (MaterialThing materialThing in Level.CheckRectAll<MaterialThing>(topLeft, bottomRight))
            {
                if ((safeFrames <= 0 || materialThing != safeDuck) && materialThing.isServerForObject)
                {
                    bool destroyed = materialThing.destroyed;
                    materialThing.Destroy(new DTIncinerate(this));
                    if (materialThing.destroyed && !destroyed)
                    {
                        if (Recorder.currentRecording != null)
                            Recorder.currentRecording.LogAction(2);
                        if (materialThing is Duck && !(materialThing as Duck).dead)
                            Recorder.currentRecording.LogBonus();
                    }
                }
            }
            if (safeFrames > 0)
                --safeFrames;
            base.Update();
        }

        public override void Draw()
        {
            Graphics.DrawRect(position + new Vec2(-4f, -4f), position + new Vec2(4f, 4f), new Color(byte.MaxValue - (int)(_wave.normalized * 90f), 137 + (int)(_wave.normalized * 50f), 31 + (int)(_wave.normalized * 30f)), depth);
            Graphics.DrawRect(position + new Vec2(-4f, -4f), position + new Vec2(4f, 4f), new Color(byte.MaxValue, 224 - (int)(_wave2.normalized * 150f), 90 + (int)(_wave2.normalized * 50f)), depth + 1, false);
            base.Draw();
        }
    }
}
