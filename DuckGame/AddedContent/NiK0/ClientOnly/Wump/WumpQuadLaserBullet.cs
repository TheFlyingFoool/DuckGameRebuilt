using System.Diagnostics.SymbolStore;
using System.Security.Cryptography.X509Certificates;

namespace DuckGame
{
    [ClientOnly]
    public class WumpQuadLaserBullet : Thing, ITeleport, IDrawToDifferentLayers
    {
        public Vec2 travel
        {
            get
            {
                return _travel;
            }
            set
            {
                _travel = value;
            }
        }

        public WumpQuadLaserBullet(float xpos, float ypos, Vec2 travel) : base(xpos, ypos, null)
        {
            _travel = travel;
            collisionOffset = new Vec2(-14);
            _collisionSize = new Vec2(28);
        }
        public bool ShouldDrawIcon()
        {
            return level != null && level.camera != null && Level.current.simulatePhysics && (position.x < level.camera.left - 6 || position.x > level.camera.right + 6 || position.y < level.camera.top - 6 || position.y > level.camera.bottom + 6);
        }
        public override void Update()
        {
            _wave.Update();
            _wave2.Update();
            position += _travel * 0.5f;
            if (isServerForObject)
            {
                if (x > Level.current.bottomRight.x + 200) x = Level.current.topLeft.x - 200;
                if (x < Level.current.topLeft.x - 200) x = Level.current.bottomRight.x + 200;
                if (y > Level.current.bottomRight.y + 200) y = Level.current.topLeft.y - 200;
                if (y < Level.current.topLeft.y - 200) y = Level.current.bottomRight.y + 200;
                if (theholysee && safeFrames <= 0)
                {
                    Duck d = Level.CheckCircle<Duck>(position, 128);
                    if (d != null)
                    {
                        float f = Maths.PointDirectionRad(Vec2.Zero, travel);
                        float f2 = Maths.PointDirectionRad(position, d.position);
                        f = Lerp.Float(f, f2, 0.05f);
                        _travel = Maths.AngleToVec(f) * 16;
                    }
                }
            }
            foreach (MaterialThing materialThing in Level.CheckRectAll<MaterialThing>(topLeft, bottomRight))
            {
                if ((safeFrames <= 0 || materialThing != safeDuck) && materialThing.isServerForObject)
                {
                    bool destroyed = materialThing.destroyed;
                    materialThing.Destroy(new DTIncinerate(this));
                    if (materialThing.destroyed && !destroyed)
                    {
                        if (Recorder.currentRecording != null)
                        {
                            Recorder.currentRecording.LogAction(2);
                        }
                        if (materialThing is Duck && !(materialThing as Duck).dead)
                        {
                            Recorder.currentRecording.LogBonus();
                        }
                    }
                }
            }
            if (safeFrames > 0)
            {
                safeFrames--;
            }
            base.Update();
        }
        public void OnDrawLayer(Layer l)
        {
            if (l == Layer.HUD && ShouldDrawIcon())
            {
                Vec2 vec = position;
                if ((vec - Level.current.camera.position).length > Level.current.camera.width * 2f)
                {
                    return;
                }
                float num = 14f;
                if (vec.x < Level.current.camera.left + num)
                {
                    vec.x = Level.current.camera.left + num;
                }
                if (vec.x > Level.current.camera.right - num)
                {
                    vec.x = Level.current.camera.right - num;
                }
                if (vec.y < Level.current.camera.top + num)
                {
                    vec.y = Level.current.camera.top + num;
                }
                if (vec.y > Level.current.camera.bottom - num)
                {
                    vec.y = Level.current.camera.bottom - num;
                }
                vec = Level.current.camera.transform(vec);
                vec = Layer.HUD.camera.transformInverse(vec);
                DrawIcon(vec);
            }
        }
        public void DrawIcon(Vec2 vec)
        {
            Color c = new Color(0, 200 + (int)(_wave.normalized * 50f), 200 - (int)(_wave.normalized * 50));
            Graphics.DrawRect(vec + new Vec2(-5f, -5f), vec + new Vec2(5f, 5f), Color.Black, 0.8f, true, 1f);
            Graphics.DrawRect(vec + new Vec2(-5f, -5f), vec + new Vec2(5f, 5f), c, 0.81f, false, 1f);
            Graphics.DrawString("!", vec - new Vec2(3.5f), c, 1);
        }
        public override void Draw()
        {
            if (theholysee)
            {
                Graphics.DrawCircle(position, 128, Color.LightBlue * 0.5f);
                Graphics.DrawCircle(position, 100 + sw * 28, Color.LightBlue * 0.3f);
                Graphics.DrawCircle(position, 100 + sw * -17, Color.LightBlue * 0.3f);
                Graphics.DrawCircle(position, 64 + swr * -64, Color.LightBlue * 0.1f);
                Graphics.DrawCircle(position, 76 + swr * 45, Color.LightBlue * 0.1f);
            }
            Graphics.DrawRect(position + new Vec2(-16), position + new Vec2(16), new Color(0, 200 + (int)(_wave.normalized * 50f), 200 - (int)(_wave.normalized * 50)), depth, true, 4f);
            Graphics.DrawRect(position + new Vec2(-16), position + new Vec2(16), new Color(0, 255 - (int)(_wave2.normalized * 50), 200 + (int)(_wave2.normalized * 50f)), depth + 1, false, 4f);
            base.Draw();
        }
        public SinWave sw = new SinWave(0.1f);
        public SinWave swr = new SinWave(0.05f);
        public bool theholysee;
        public StateBinding _positionBinding = new CompressedVec2Binding("position", int.MaxValue, false, true);
        public StateBinding _travelBinding = new CompressedVec2Binding("travel", 20);
        public StateBinding _theholysaw = new CompressedVec2Binding("theholysee", 20);
        private Vec2 _travel;
        private SinWaveManualUpdate _wave = 0.5f;
        private SinWaveManualUpdate _wave2 = 1f;
        public int safeFrames;
        public Duck safeDuck;
    }
}
