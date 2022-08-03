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
            get => position.x;
            set => position.x = value;
        }

        public float y
        {
            get => position.y;
            set => position.y = value;
        }

        public float z
        {
            get => _z;
            set => _z = value;
        }

        public Depth depth
        {
            get => _depth;
            set => _depth = value;
        }

        public virtual Vec2 center
        {
            get => _center;
            set => _center = value;
        }

        public float centerx
        {
            get => _center.x;
            set => _center.x = value;
        }

        public float centery
        {
            get => _center.y;
            set => _center.y = value;
        }

        public virtual float angle
        {
            get => _angle;
            set => _angle = value;
        }

        public float angleDegrees
        {
            get => Maths.RadToDeg(angle);
            set => angle = Maths.DegToRad(value);
        }

        public Vec2 scale
        {
            get => _scale;
            set => _scale = value;
        }

        public float xscale
        {
            get => _scale.x;
            set => _scale.x = value;
        }

        public float yscale
        {
            get => _scale.y;
            set => _scale.y = value;
        }

        public float alpha
        {
            get => _alpha;
            set => _alpha = value;
        }

        public float alphaSub
        {
            get => _alphaSub;
            set => _alphaSub = value;
        }
    }
}
