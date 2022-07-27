// Decompiled with JetBrains decompiler
// Type: DuckGame.Conveyor
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [BaggedProperty("isInDemo", true)]
    public class Conveyor : Block
    {
        private SpriteMap _sprite;
        public bool up = true;
        protected ImpactedFrom _killImpact;

        public Conveyor(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("stuff/conveyor", 14, 10);
            this._sprite.AddAnimation("convey", 0.1f, true, 0, 1, 2, 3, 4, 5, 6, 7);
            this._sprite.frame = Rando.Int(0, 7);
            this._sprite.SetAnimation("convey");
            this.graphic = _sprite;
            this.center = new Vec2(7f, 5f);
            this.collisionOffset = new Vec2(-7f, -4f);
            this.collisionSize = new Vec2(14f, 8f);
            this.depth = (Depth)0.5f;
            this._editorName = nameof(Conveyor);
            this.thickness = 100f;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.editorOffset = new Vec2(0.0f, 6f);
            this.hugWalls = WallHug.Floor;
            this._editorImageCenter = true;
            this._killImpact = ImpactedFrom.Top;
        }

        public override void Update() => base.Update();

        public override void Draw() => base.Draw();
    }
}
