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
            graphic = new Sprite("explosiveBarrel");
            center = new Vec2(7f, 8f);
            collisionOffset = new Vec2(-7f, -8f);
            collisionSize = new Vec2(14f, 16f);
            depth = -0.1f;
            _editorName = "Barrel (Explosive)";
            editorTooltip = "Nobody knows what's in these things or why everyone just leaves them around.";
            thickness = 4f;
            weight = 10f;
            physicsMaterial = PhysicsMaterial.Metal;
            collideSounds.Clear();
            collideSounds.Add("barrelThud");
            _holdOffset = new Vec2(1f, 0f);
            flammable = 0.3f;
            _placementCost += 10;
            baseExplosionRange = 70f;
        }

        public override void DoBlockDestruction()
        {
        }
    }
}
