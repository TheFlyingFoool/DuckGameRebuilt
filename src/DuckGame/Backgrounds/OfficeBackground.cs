// Decompiled with JetBrains decompiler
// Type: DuckGame.OfficeBackground
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Background|Parallax")]
    public class OfficeBackground : BackgroundUpdater
    {
        public OfficeBackground(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new SpriteMap("backgroundIcons", 16, 16)
            {
                frame = 1
            };
            this.center = new Vec2(8f, 8f);
            this._collisionSize = new Vec2(16f, 16f);
            this._collisionOffset = new Vec2(-8f, -8f);
            this.depth = (Depth)0.9f;
            this.layer = Layer.Foreground;
            this._visibleInGame = false;
            this._editorName = "Office BG";
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            this.backgroundColor = new Color(25, 38, 41);
            Level.current.backgroundColor = this.backgroundColor;
            this._parallax = new ParallaxBackground("background/office", 0.0f, 0.0f, 3);
            if (this._parallax.definition == null)
            {
                float speed = 0.4f;
                this._parallax.AddZone(0, 0.0f, -speed, true);
                this._parallax.AddZone(1, 0.0f, -speed, true);
                this._parallax.AddZone(2, 0.0f, -speed, true);
                this._parallax.AddZone(3, 0.2f, -speed, true);
                this._parallax.AddZone(4, 0.2f, -speed, true);
                this._parallax.AddZone(5, 0.4f, -speed, true);
                this._parallax.AddZone(6, 0.8f, speed);
                this._parallax.AddZone(7, 0.8f, speed);
                this._parallax.AddZone(8, 0.8f, speed);
                this._parallax.AddZone(9, 0.8f, speed);
                Sprite s1 = new Sprite("background/officeBuilding01")
                {
                    depth = -0.9f,
                    position = new Vec2(100f, 100f)
                };
                this._parallax.AddZoneSprite(s1, 15, 0.6f, speed);
                Sprite s2 = new Sprite("background/officeBuilding01Porch")
                {
                    depth = -0.9f,
                    position = new Vec2(84f, 160f)
                };
                this._parallax.AddZoneSprite(s2, 16, 0.6f, speed);
                Sprite s3 = new Sprite("background/officeBuilding02")
                {
                    depth = -0.9f,
                    position = new Vec2(300f, 120f)
                };
                this._parallax.AddZoneSprite(s3, 17, 0.6f, speed);
                this._parallax.AddZone(19, 0.6f, speed);
                this._parallax.AddZone(20, 0.6f, speed);
                this._parallax.AddZone(21, 0.6f, speed);
                this._parallax.AddZone(22, 0.6f, speed);
                this._parallax.AddZone(23, 0.6f, speed);
                this._parallax.AddZone(24, 0.5f, speed);
                this._parallax.AddZone(25, 0.4f, speed);
                this._parallax.AddZone(26, 0.3f, speed);
                this._parallax.AddZone(27, 0.2f, speed);
                this._parallax.AddZone(28, 0.1f, speed);
                this._parallax.AddZone(29, 0.0f, speed);
            }
            Level.Add(_parallax);
        }

        public override void Update() => base.Update();

        public override void Terminate() => Level.Remove(_parallax);
    }
}
