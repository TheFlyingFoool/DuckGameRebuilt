// Decompiled with JetBrains decompiler
// Type: DuckGame.ChallengeConfetti
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ChallengeConfetti : Thing
    {
        private float _fallSpeed;
        private float _horSpeed;

        public ChallengeConfetti(float xpos, float ypos)
          : base(xpos, ypos)
        {
            SpriteMap spriteMap = new SpriteMap("arcade/confetti", 8, 8);
            spriteMap.AddAnimation("idle", 0.1f, true, 0, 1);
            spriteMap.SetAnimation("idle");
            this.graphic = (Sprite)spriteMap;
            this._fallSpeed = Rando.Float(0.5f, 1.2f);
            this.layer = Layer.HUD;
            int num = Rando.ChooseInt(0, 1, 2, 3);
            if (num == 0)
                spriteMap.color = Color.Violet;
            if (num == 1)
                spriteMap.color = Color.SkyBlue;
            if (num == 2)
                spriteMap.color = Color.Wheat;
            if (num == 4)
                spriteMap.color = Color.GreenYellow;
            this.depth = (Depth)1f;
        }

        public override void Update()
        {
            this.alpha = ArcadeHUD.alphaVal + Chancy.alpha;
            this.y += this._fallSpeed;
            this._horSpeed += Rando.Float(-0.1f, 0.1f);
            if ((double)this._horSpeed < -0.300000011920929)
                this._horSpeed = -0.3f;
            else if ((double)this._horSpeed > 0.300000011920929)
                this._horSpeed = 0.3f;
            this.x += this._horSpeed;
            if ((double)this.y <= 200.0)
                return;
            Level.Remove((Thing)this);
        }
    }
}
