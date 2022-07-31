// Decompiled with JetBrains decompiler
// Type: DuckGame.Saws
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Stuff|Spikes")]
    public class Saws : MaterialThing, IDontMove
    {
        private SpriteMap _sprite;
        public bool up = true;

        public Saws(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("movingSpikes", 16, 21)
            {
                speed = 0.3f
            };
            this.graphic = _sprite;
            this.center = new Vec2(8f, 14f);
            this.collisionOffset = new Vec2(-6f, -2f);
            this.collisionSize = new Vec2(12f, 4f);
            this.depth = (Depth)0.28f;
            this._editorName = "Saws Up";
            this.editorTooltip = "Deadly hazards, able to cut through even the strongest of boots";
            this.thickness = 3f;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.editorOffset = new Vec2(0f, 6f);
            this.hugWalls = WallHug.Floor;
            this._editorImageCenter = true;
            this.editorCycleType = typeof(SawsRight);
            this.impactThreshold = 0.01f;
        }

        public override void Touch(MaterialThing with)
        {
            Duck duck = with as Duck;
            if (!with.isServerForObject || duck != null && duck.holdObject is Sword && (duck.holdObject as Sword)._slamStance || with.destroyed)
                return;
            with.Destroy(new DTImpale(this));
            with.vSpeed = -3f;
        }

        public override void Update() => base.Update();

        public override void Draw() => base.Draw();
    }
}
