// Decompiled with JetBrains decompiler
// Type: DuckGame.LavaBarrel
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Stuff|Props|Barrels")]
    [BaggedProperty("noRandomSpawningOnline", true)]
    public class LavaBarrel : YellowBarrel
    {
        public LavaBarrel(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite("lavaBarrel");
            this.center = new Vec2(7f, 8f);
            this._melting = new Sprite("blueBarrelMelting");
            this._editorName = "Barrel (Lava)";
            this.editorTooltip = "Your standard lava barrel - for carrying delicious, refreshing lava. Choose lava!";
            this.flammable = 0.0f;
            this._fluid = Fluid.Lava;
            this._toreUp = new SpriteMap("blueBarrelToreUp", 14, 17);
            this._toreUp.frame = 1;
            this._toreUp.center = new Vec2(0.0f, -6f);
        }
    }
}
