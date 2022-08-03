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
            graphic = new SpriteMap("levelJetIdle", 32, 13);
            _leftJet = new SpriteMap("jet", 16, 16);
            _leftJet.AddAnimation("idle", 0.4f, true, 0, 1, 2);
            _leftJet.SetAnimation("idle");
            _leftJet.center = new Vec2(8f, 0f);
            _leftJet.alpha = 0.7f;
            _rightJet = new SpriteMap("jet", 16, 16);
            _rightJet.AddAnimation("idle", 0.4f, true, 1, 2, 0);
            _rightJet.SetAnimation("idle");
            _rightJet.center = new Vec2(8f, 0f);
            _rightJet.alpha = 0.7f;
            center = new Vec2(16f, 8f);
            _collisionSize = new Vec2(16f, 14f);
            _collisionOffset = new Vec2(-8f, -8f);
            editorTooltip = "Things gotta float somehow.";
            hugWalls = WallHug.Ceiling;
            _canFlip = false;
        }

        public override void Update()
        {
            _leftAlternate = !_leftAlternate;
            _rightAlternate = !_rightAlternate;
        }

        public override void Draw()
        {
            base.Draw();
            Graphics.Draw(_leftJet, x - 8f, y + 5f);
            Graphics.Draw(_rightJet, x + 8f, y + 5f);
        }
    }
}
