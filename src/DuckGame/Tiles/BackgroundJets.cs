// Decompiled with JetBrains decompiler
// Type: DuckGame.BackgroundJets
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Details")]
    public class BackgroundJets : Thing
    {
        public SpriteMap _leftJet;
        public SpriteMap _rightJet;
        private bool _leftAlternate;
        private bool _rightAlternate = true;

        public BackgroundJets(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new SpriteMap("levelJetIdle", 32, 13);
            this._leftJet = new SpriteMap("jet", 16, 16);
            this._leftJet.AddAnimation("idle", 0.4f, true, 0, 1, 2);
            this._leftJet.SetAnimation("idle");
            this._leftJet.center = new Vec2(8f, 0.0f);
            this._leftJet.alpha = 0.7f;
            this._rightJet = new SpriteMap("jet", 16, 16);
            this._rightJet.AddAnimation("idle", 0.4f, true, 1, 2, 0);
            this._rightJet.SetAnimation("idle");
            this._rightJet.center = new Vec2(8f, 0.0f);
            this._rightJet.alpha = 0.7f;
            this.center = new Vec2(16f, 8f);
            this._collisionSize = new Vec2(16f, 14f);
            this._collisionOffset = new Vec2(-8f, -8f);
            this.editorTooltip = "Things gotta float somehow.";
            this.hugWalls = WallHug.Ceiling;
            this._canFlip = false;
        }

        public override void Update()
        {
            this._leftAlternate = !this._leftAlternate;
            this._rightAlternate = !this._rightAlternate;
        }

        public override void Draw()
        {
            base.Draw();
            Graphics.Draw(_leftJet, this.x - 8f, this.y + 5f);
            Graphics.Draw(_rightJet, this.x + 8f, this.y + 5f);
        }
    }
}
