// Decompiled with JetBrains decompiler
// Type: DuckGame.DonutSpaceBackground
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Background|Parallax")]
    public class DonutSpaceBackground : BackgroundUpdater
    {
        private float _speedMult;
        private bool _moving;

        public DonutSpaceBackground(float xpos, float ypos, bool moving = false, float speedMult = 1f)
          : base(xpos, ypos)
        {
            this.graphic = new SpriteMap("backgroundIcons", 16, 16)
            {
                frame = 3
            };
            this.center = new Vec2(8f, 8f);
            this._collisionSize = new Vec2(16f, 16f);
            this._collisionOffset = new Vec2(-8f, -8f);
            this.depth = (Depth)0.9f;
            this.layer = Layer.Foreground;
            this._visibleInGame = false;
            this._speedMult = speedMult;
            this._moving = moving;
            this._editorName = "Donut BG";
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            this.backgroundColor = new Color(0, 0, 0);
            Level.current.backgroundColor = this.backgroundColor;
            this._parallax = new ParallaxBackground("background/space", 0f, 0f, 3);
            float speed = 0.4f * this._speedMult;
            Sprite sprite = new Sprite("background/donut")
            {
                depth = -0.9f,
                position = new Vec2(200f, 50f)
            };
            this._parallax.AddZoneThing(new SpaceDonut(200f, 50f), 19, 0.99f, speed);
            this._parallax.AddZone(20, 0.93f, speed, this._moving);
            this._parallax.AddZone(21, 0.9f, speed, this._moving);
            this._parallax.AddZone(22, 0.87f, speed, this._moving);
            this._parallax.AddZone(23, 0.84f, speed, this._moving);
            this._parallax.AddZone(24, 0.81f, speed, this._moving);
            this._parallax.AddZone(25, 0.81f, speed, this._moving);
            this._parallax.AddZone(26, 0.78f, speed, this._moving);
            this._parallax.AddZone(27, 0.78f, speed, this._moving);
            this._parallax.AddZone(28, 0.75f, speed, this._moving);
            this._parallax.AddZone(29, 0.75f, speed, this._moving);
            Level.Add(_parallax);
        }

        public override void Update() => base.Update();

        public override void Terminate() => Level.Remove(_parallax);
    }
}
