// Decompiled with JetBrains decompiler
// Type: DuckGame.Spark
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class Spark : PhysicsParticle, IFactory
    {
        private static int kMaxSparks = 64;
        private static Spark[] _sparks = new Spark[Spark.kMaxSparks];
        private static int _lastActiveSpark = 0;
        private float _killSpeed = 0.03f;
        public Color _color;
        public float _width = 0.5f;

        public static Spark New(float xpos, float ypos, Vec2 hitAngle, float killSpeed = 0.02f)
        {
            Spark spark;
            if (Spark._sparks[Spark._lastActiveSpark] == null)
            {
                spark = new Spark();
                Spark._sparks[Spark._lastActiveSpark] = spark;
            }
            else
                spark = Spark._sparks[Spark._lastActiveSpark];
            Spark._lastActiveSpark = (Spark._lastActiveSpark + 1) % Spark.kMaxSparks;
            spark.ResetProperties();
            spark.Init(xpos, ypos, hitAngle, killSpeed);
            spark.globalIndex = Thing.GetGlobalIndex();
            return spark;
        }

        private Spark()
          : base(0.0f, 0.0f)
        {
        }

        private void Init(float xpos, float ypos, Vec2 hitAngle, float killSpeed = 0.02f)
        {
            this.position.x = xpos;
            this.position.y = ypos;
            this.hSpeed = (float)(-(double)hitAngle.x * 2.0 * ((double)Rando.Float(1f) + 0.300000011920929)) - Rando.Float(-1f, 1f);
            this.vSpeed = (float)(-(double)hitAngle.y * 2.0 * ((double)Rando.Float(1f) + 0.300000011920929)) - Rando.Float(2f);
            this._bounceEfficiency = 0.6f;
            this.depth = (Depth)0.9f;
            this._killSpeed = killSpeed;
            this._color = new Color(byte.MaxValue, (byte)Rando.Int(180, byte.MaxValue), (byte)0);
            this._width = 0.5f;
        }

        public override void Update()
        {
            this.alpha -= this._killSpeed;
            if ((double)this.alpha < 0.0)
                Level.Remove(this);
            base.Update();
        }

        public override void Draw()
        {
            Vec2 p2 = this.position + this.velocity.normalized * (this.velocity.length * 2f);
            Vec2 position;
            Graphics.DrawLine(this.position, Level.CheckLine<Block>(this.position, p2, out position) != null ? position : p2, this._color * this.alpha, this._width, this.depth);
        }
    }
}
