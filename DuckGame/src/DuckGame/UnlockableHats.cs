// Decompiled with JetBrains decompiler
// Type: DuckGame.UnlockableHats
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class UnlockableHats : Unlockable
    {
        private List<Team> _teams;
        private DuckPersona[] _persona = new DuckPersona[4];

        public UnlockableHats(
          string identifier,
          List<Team> t,
          Func<bool> condition,
          string nam,
          string desc,
          string achieve = "")
          : this(true, identifier, t, condition, nam, desc, achieve)
        {
        }

        public UnlockableHats(
          bool canHint,
          string identifier,
          List<Team> t,
          Func<bool> condition,
          string nam,
          string desc,
          string achieve = "")
          : base(identifier, condition, nam, desc, achieve)
        {
            allowHints = canHint;
            _teams = t;
            _showScreen = true;
            _persona[0] = Persona.alllist[Rando.Int(3)];
            _persona[1] = Persona.alllist[Rando.Int(3)];
            _persona[2] = Persona.alllist[Rando.Int(3)];
            _persona[3] = Persona.alllist[Rando.Int(3)];
        }

        public override void Initialize()
        {
            foreach (Team team in _teams)
            {
                if (team != null)
                    team.locked = true;
            }
        }

        protected override void Unlock()
        {
            foreach (Team team in _teams)
            {
                if (team != null)
                    team.locked = false;
            }
        }

        protected override void Lock()
        {
            foreach (Team team in _teams)
            {
                if (team != null)
                    team.locked = true;
            }
        }

        public override void Draw(float x, float y, Depth depth)
        {
            y -= 9f;
            float num1 = 9f;
            if (_teams.Count == 3)
                num1 = 18f;
            int index = 0;
            foreach (Team team in _teams)
            {
                if (team != null && index < 8)
                {
                    float num2 = x;
                    float y1 = y + 12f;
                    _persona[index].sprite.depth = depth;
                    _persona[index].sprite.color = Color.White;
                    SpriteMap g1 = _persona[index].sprite;

                    Graphics.Draw(ref g1, 0, num2 - num1 + index * 18, y1);
                    _persona[index].armSprite.frame = _persona[index].sprite.imageIndex;
                    _persona[index].armSprite.scale = new Vec2(1f, 1f);
                    _persona[index].armSprite.depth = depth + 4;
                    SpriteMap g2 = _persona[index].armSprite;

                    Graphics.Draw(ref g2, (float)(num2 - num1 + index * 18 - 3), y1 + 6f);
                    Vec2 hatPoint = DuckRig.GetHatPoint(_persona[index].sprite.imageIndex);
                    team.hat.depth = depth + 2;
                    team.hat.center = new Vec2(16f, 16f) + team.hatOffset;
                    Graphics.Draw(team.hat, team.hat.frame, num2 - num1 + index * 18 + hatPoint.x, y1 + hatPoint.y);
                }
                ++index;
            }
        }
    }
}
