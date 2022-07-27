// Decompiled with JetBrains decompiler
// Type: DuckGame.ExplosiveBarrel
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Stuff|Props|Barrels")]
    public class ExplosiveBarrel : DemoCrate, IPlatform
    {
        public ExplosiveBarrel(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite("explosiveBarrel");
            this.center = new Vec2(7f, 8f);
            this.collisionOffset = new Vec2(-7f, -8f);
            this.collisionSize = new Vec2(14f, 16f);
            this.depth = -0.1f;
            this._editorName = "Barrel (Explosive)";
            this.editorTooltip = "Nobody knows what's in these things or why everyone just leaves them around.";
            this.thickness = 4f;
            this.weight = 10f;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.collideSounds.Clear();
            this.collideSounds.Add("barrelThud");
            this._holdOffset = new Vec2(1f, 0.0f);
            this.flammable = 0.3f;
            this._placementCost += 10;
            this.baseExplosionRange = 70f;
        }

        public override void DoBlockDestruction()
        {
        }
    }
}
