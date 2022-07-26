// Decompiled with JetBrains decompiler
// Type: DuckGame.WallBoots
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Equipment")]
    [BaggedProperty("canSpawn", false)]
    public class WallBoots : Boots
    {
        public WallBoots(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._pickupSprite = new Sprite("walljumpBootsPickup");
            this._sprite = new SpriteMap("walljumpBoots", 32, 32);
            this.graphic = this._pickupSprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-6f, -6f);
            this.collisionSize = new Vec2(12f, 13f);
            this._equippedDepth = 3;
            this.editorTooltip = "Allows you to jump from walls. Why would you want to do this? Who can say.";
        }
    }
}
