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
            allowHints = canHint;
            _team = t;
            _persona = Persona.all.ElementAt<DuckPersona>(Rando.Int(3));
            _showScreen = true;
        }

        public override void Initialize()
        {
            if (_team == null)
                return;
            _team.locked = true;
        }

        protected override void Unlock()
        {
            if (_team == null)
                return;
            _team.locked = false;
        }

        protected override void Lock()
        {
            if (_team == null)
                return;
            _team.locked = true;
        }

        public override void Draw(float x, float y, Depth depth)
        {
            if (_team == null)
                return;
            y -= 9f;
            float x1 = x;
            float y1 = y + 8f;
            _persona.sprite.depth = depth;
            _persona.sprite.color = Color.White;
            DuckGame.Graphics.Draw(_persona.sprite, 0, x1, y1);
            _persona.armSprite.frame = _persona.sprite.imageIndex;
            _persona.armSprite.scale = new Vec2(1f, 1f);
            _persona.armSprite.depth = depth + 4;
            DuckGame.Graphics.Draw(_persona.armSprite, x1 - 3f, y1 + 6f);
            Vec2 hatPoint = DuckRig.GetHatPoint(_persona.sprite.imageIndex);
            _team.hat.depth = depth + 2;
            _team.hat.center = new Vec2(16f, 16f) + _team.hatOffset;
            DuckGame.Graphics.Draw(_team.hat, _team.hat.frame, x1 + hatPoint.x, y1 + hatPoint.y);
            if (_team.hat.texture.textureName == "hats/devhat" && _cape == null)
            {
                _hat = new TeamHat(x1 + hatPoint.x, (float)(y1 + hatPoint.y + 5.0), Teams.GetTeam("CAPTAIN"));
                _cape = new Cape(x1 + hatPoint.x, y1 + hatPoint.y, _hat);
                _cape.SetCapeTexture((Texture2D)Content.Load<Tex2D>("hats/devCape"));
            }
            if (_team.hat.texture.textureName == "hats/moonwalker" && _cape == null)
            {
                _hat = new TeamHat(x1 + hatPoint.x, (float)(y1 + hatPoint.y + 5.0), Teams.GetTeam("MOONWALK"));
                _cape = new Cape(x1 + hatPoint.x, y1 + hatPoint.y, _hat);
                _cape.SetCapeTexture((Texture2D)Content.Load<Tex2D>("hats/moonCape"));
            }
            if (_team.hat.texture.textureName == "hats/royalty" && _cape == null)
            {
                _hat = new TeamHat(x1 + hatPoint.x, (float)(y1 + hatPoint.y + 5.0), Teams.GetTeam("MAJESTY"));
                _cape = new Cape(x1 + hatPoint.x, y1 + hatPoint.y, _hat);
                _cape.SetCapeTexture((Texture2D)Content.Load<Tex2D>("hats/royalCape"));
            }
            if (_cape == null)
                return;
            _hat.position = new Vec2(x1 + hatPoint.x, (float)(y1 + hatPoint.y + 5.0));
            _cape.depth = depth + 2;
            _cape.Update();
            _cape.Draw();
        }
    }
}
