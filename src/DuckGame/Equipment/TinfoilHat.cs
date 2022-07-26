// Decompiled with JetBrains decompiler
// Type: DuckGame.TinfoilHat
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Equipment")]
    public class TinfoilHat : Hat
    {
        public TinfoilHat(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._pickupSprite = new Sprite("tinfoilHatPickup");
            this._sprite = new SpriteMap("tinfoilHat", 32, 32);
            this.graphic = this._pickupSprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-6f, -4f);
            this.collisionSize = new Vec2(12f, 8f);
            this._sprite.CenterOrigin();
            this.thickness = 0.1f;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.editorTooltip = "Protects against the effects of mind control, spy satellites, and awkward social situations.";
        }

        public override void Update() => base.Update();
    }
}
