// Decompiled with JetBrains decompiler
// Type: DuckGame.Spark
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class Spark : PhysicsParticle, IFactory
    {
        public static int kMaxSparks = 64;
        public static Spark[] _sparks = new Spark[kMaxSparks];
        public static int _lastActiveSpark = 0;
        private float _killSpeed = 0.03f;
        public Color _color;
        public float _width = 0.5f;

        public static Spark New(float xpos, float ypos, Vec2 hitAngle, float killSpeed = 0.02f)
        {
            Spark spark;
            if (_sparks[_lastActiveSpark] == null)
            {
                spark = new Spark();
                _sparks[_lastActiveSpark] = spark;
            }
            else
                spark = _sparks[_lastActiveSpark];
            _lastActiveSpark = (_lastActiveSpark + 1) % kMaxSparks;
            spark.ResetProperties();
            spark.Init(xpos, ypos, hitAngle, killSpeed);
            spark.globalIndex = GetGlobalIndex();
            return spark;
        }

        private Spark()
          : base(0f, 0f)
        {
        }

        private void Init(float xpos, float ypos, Vec2 hitAngle, float killSpeed = 0.02f)
        {
            position.x = xpos;
            position.y = ypos;
            hSpeed = -hitAngle.x * 2f * (Rando.Float(1f) + 0.3f) - Rando.Float(-1f, 1f);
            vSpeed = -hitAngle.y * 2f * (Rando.Float(1f) + 0.3f) - Rando.Float(2f);
            _bounceEfficiency = 0.6f;
            depth = 0.9f;
            _killSpeed = killSpeed;
            _color = new Color(byte.MaxValue, (byte)Rando.Int(180, byte.MaxValue), (byte)0);
            _width = 0.5f;
        }

        public override void Update()
        {
            alpha -= _killSpeed;
            if (alpha < 0.0)
                Level.Remove(this);
            base.Update();
        }

        public override void Draw()
        {
            Vec2 p2 = this.position + velocity.normalized * (velocity.length * 2f);
            Vec2 position;
            Graphics.DrawLine(this.position, Level.CheckLine<Block>(this.position, p2, out position) != null ? position : p2, _color * alpha, _width, depth);
        }
    }
}
