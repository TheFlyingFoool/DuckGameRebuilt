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
