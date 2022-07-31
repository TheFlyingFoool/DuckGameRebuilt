// Decompiled with JetBrains decompiler
// Type: DuckGame.Transform
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    /// <summary>Represents a transformable component.</summary>
    public abstract class Transform
    {
        public Vec2 position;
        protected float _z;
        protected Depth _depth = new Depth(0f);
        protected Vec2 _center;
        public float _angle;
        private Vec2 _scale = new Vec2(1f, 1f);
        private float _alpha = 1f;
        private float _alphaSub;

        public float x
        {
            get => this.position.x;
            set => this.position.x = value;
        }

        public float y
        {
            get => this.position.y;
            set => this.position.y = value;
        }

        public float z
        {
            get => this._z;
            set => this._z = value;
        }

        public Depth depth
        {
            get => this._depth;
            set => this._depth = value;
        }

        public virtual Vec2 center
        {
            get => this._center;
            set => this._center = value;
        }

        public float centerx
        {
            get => this._center.x;
            set => this._center.x = value;
        }

        public float centery
        {
            get => this._center.y;
            set => this._center.y = value;
        }

        public virtual float angle
        {
            get => this._angle;
            set => this._angle = value;
        }

        public float angleDegrees
        {
            get => Maths.RadToDeg(this.angle);
            set => this.angle = Maths.DegToRad(value);
        }

        public Vec2 scale
        {
            get => this._scale;
            set => this._scale = value;
        }

        public float xscale
        {
            get => this._scale.x;
            set => this._scale.x = value;
        }

        public float yscale
        {
            get => this._scale.y;
            set => this._scale.y = value;
        }

        public float alpha
        {
            get => this._alpha;
            set => this._alpha = value;
        }

        public float alphaSub
        {
            get => this._alphaSub;
            set => this._alphaSub = value;
        }
    }
}
