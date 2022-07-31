// Decompiled with JetBrains decompiler
// Type: DuckGame.QuadLaserBullet
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class QuadLaserBullet : Thing, ITeleport
    {
        public StateBinding _positionBinding = new CompressedVec2Binding("position", doLerp: true);
        public StateBinding _travelBinding = new CompressedVec2Binding(nameof(travel), 20);
        private Vec2 _travel;
        private SinWaveManualUpdate _wave = (SinWaveManualUpdate)0.5f;
        private SinWaveManualUpdate _wave2 = (SinWaveManualUpdate)1f;
        public int safeFrames;
        public Duck safeDuck;
        public float timeAlive;

        public Vec2 travel
        {
            get => this._travel;
            set => this._travel = value;
        }

        public QuadLaserBullet(float xpos, float ypos, Vec2 travel)
          : base(xpos, ypos)
        {
            this._travel = travel;
            this.collisionOffset = new Vec2(-1f, -1f);
            this._collisionSize = new Vec2(2f, 2f);
        }

        public override void Update()
        {
            this._wave.Update();
            this._wave2.Update();
            this.timeAlive += 0.016f;
            this.position += this._travel * 0.5f;
            if (this.isServerForObject && (this.x > Level.current.bottomRight.x + 200.0 || this.x < Level.current.topLeft.x - 200.0))
                Level.Remove(this);
            foreach (MaterialThing materialThing in Level.CheckRectAll<MaterialThing>(this.topLeft, this.bottomRight))
            {
                if ((this.safeFrames <= 0 || materialThing != this.safeDuck) && materialThing.isServerForObject)
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
            if (this.safeFrames > 0)
                --this.safeFrames;
            base.Update();
        }

        public override void Draw()
        {
            Graphics.DrawRect(this.position + new Vec2(-4f, -4f), this.position + new Vec2(4f, 4f), new Color(byte.MaxValue - (int)(this._wave.normalized * 90.0), 137 + (int)(this._wave.normalized * 50.0), 31 + (int)(this._wave.normalized * 30.0)), this.depth);
            Graphics.DrawRect(this.position + new Vec2(-4f, -4f), this.position + new Vec2(4f, 4f), new Color(byte.MaxValue, 224 - (int)(this._wave2.normalized * 150.0), 90 + (int)(this._wave2.normalized * 50.0)), this.depth + 1, false);
            base.Draw();
        }
    }
}
