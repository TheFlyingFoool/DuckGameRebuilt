// Decompiled with JetBrains decompiler
// Type: DuckGame.UnlockableHat
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace DuckGame
{
    public class UnlockableHat : Unlockable
    {
        private Team _team;
        private TeamHat _hat;
        private DuckPersona _persona;
        private Cape _cape;

        public UnlockableHat(
          string identifier,
          Team t,
          Func<bool> condition,
          string nam,
          string desc,
          string achieve = "")
          : this(true, identifier, t, condition, nam, desc, achieve)
        {
        }

        public UnlockableHat(
          bool canHint,
          string identifier,
          Team t,
          Func<bool> condition,
          string nam,
          string desc,
          string achieve = "")
          : base(identifier, condition, nam, desc, achieve)
        {
            this.allowHints = canHint;
            this._team = t;
            this._persona = Persona.all.ElementAt<DuckPersona>(Rando.Int(3));
            this._showScreen = true;
        }

        public override void Initialize()
        {
            if (this._team == null)
                return;
            this._team.locked = true;
        }

        protected override void Unlock()
        {
            if (this._team == null)
                return;
            this._team.locked = false;
        }

        protected override void Lock()
        {
            if (this._team == null)
                return;
            this._team.locked = true;
        }

        public override void Draw(float x, float y, Depth depth)
        {
            if (this._team == null)
                return;
            y -= 9f;
            float x1 = x;
            float y1 = y + 8f;
            this._persona.sprite.depth = depth;
            this._persona.sprite.color = Color.White;
            DuckGame.Graphics.Draw(this._persona.sprite, 0, x1, y1);
            this._persona.armSprite.frame = this._persona.sprite.imageIndex;
            this._persona.armSprite.scale = new Vec2(1f, 1f);
            this._persona.armSprite.depth = depth + 4;
            DuckGame.Graphics.Draw((Sprite)this._persona.armSprite, x1 - 3f, y1 + 6f);
            Vec2 hatPoint = DuckRig.GetHatPoint(this._persona.sprite.imageIndex);
            this._team.hat.depth = depth + 2;
            this._team.hat.center = new Vec2(16f, 16f) + this._team.hatOffset;
            DuckGame.Graphics.Draw(this._team.hat, this._team.hat.frame, x1 + hatPoint.x, y1 + hatPoint.y);
            if (this._team.hat.texture.textureName == "hats/devhat" && this._cape == null)
            {
                this._hat = new TeamHat(x1 + hatPoint.x, (float)((double)y1 + (double)hatPoint.y + 5.0), Teams.GetTeam("CAPTAIN"));
                this._cape = new Cape(x1 + hatPoint.x, y1 + hatPoint.y, (PhysicsObject)this._hat);
                this._cape.SetCapeTexture((Texture2D)Content.Load<Tex2D>("hats/devCape"));
            }
            if (this._team.hat.texture.textureName == "hats/moonwalker" && this._cape == null)
            {
                this._hat = new TeamHat(x1 + hatPoint.x, (float)((double)y1 + (double)hatPoint.y + 5.0), Teams.GetTeam("MOONWALK"));
                this._cape = new Cape(x1 + hatPoint.x, y1 + hatPoint.y, (PhysicsObject)this._hat);
                this._cape.SetCapeTexture((Texture2D)Content.Load<Tex2D>("hats/moonCape"));
            }
            if (this._team.hat.texture.textureName == "hats/royalty" && this._cape == null)
            {
                this._hat = new TeamHat(x1 + hatPoint.x, (float)((double)y1 + (double)hatPoint.y + 5.0), Teams.GetTeam("MAJESTY"));
                this._cape = new Cape(x1 + hatPoint.x, y1 + hatPoint.y, (PhysicsObject)this._hat);
                this._cape.SetCapeTexture((Texture2D)Content.Load<Tex2D>("hats/royalCape"));
            }
            if (this._cape == null)
                return;
            this._hat.position = new Vec2(x1 + hatPoint.x, (float)((double)y1 + (double)hatPoint.y + 5.0));
            this._cape.depth = depth + 2;
            this._cape.Update();
            this._cape.Draw();
        }
    }
}
