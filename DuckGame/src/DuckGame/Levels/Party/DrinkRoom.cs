// Decompiled with JetBrains decompiler
// Type: DuckGame.DrinkRoom
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class DrinkRoom : Level, IHaveAVirtualTransition
    {
        private Level _next;
        private bool _fade;

        public DrinkRoom(Level next)
        {
            transitionSpeedMultiplier = 4f;
            _centeredView = true;
            _next = next;
        }

        public override void Initialize()
        {
            HUD.AddCornerMessage(HUDCorner.BottomRight, "@MENU2@CONTINUE");
            base.Initialize();
        }

        public override void Update()
        {
            if (Input.Pressed(Triggers.Menu2))
                _fade = true;
            Graphics.fade = Lerp.Float(Graphics.fade, _fade ? 0f : 1f, 0.1f);
            if (_fade && Graphics.fade < 0.01f)
                current = _next;
            base.Update();
        }

        public override void Draw()
        {
            float y = 12f;
            foreach (Profile p in Profiles.active)
            {
                bool flag = false;
                int drinks = Party.GetDrinks(p);
                if (drinks > 0)
                {
                    string text = p.name + " |WHITE|drinks |RED|" + drinks.ToString();
                    Graphics.DrawString(text, new Vec2((Layer.HUD.camera.width / 2f - Graphics.GetStringWidth(text) / 2f), y), p.persona.colorUsable);
                    y += 9f;
                    flag = true;
                }
                foreach (PartyPerks perk in Party.GetPerks(p))
                {
                    string text = p.name + " |WHITE|gets |GREEN|" + perk.ToString();
                    Graphics.DrawString(text, new Vec2((Layer.HUD.camera.width / 2f - Graphics.GetStringWidth(text) / 2f), y), p.persona.colorUsable);
                    y += 9f;
                    flag = true;
                }
                if (flag)
                    y += 9f;
            }
        }
    }
}
