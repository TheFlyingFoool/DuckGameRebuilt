//namespace duk
namespace DuckGame
{
    //straight up dont use this at all, its pretty much hardcoded to just work for the match setting presets and is useless otherwise -NiK0
    public class UISideButton : UIComponent
    {
        public string text = "";
        public UISideButton(float xpos, float ypos, float wide, float high, string txt) : base(xpos, ypos, wide, high)
        {
            _sections = new SpriteMap("uiBox", 10, 10);
            text = txt;
        }
        private SpriteMap _sections;
        private bool _borderVisible = true;
        public override void Draw()
        {
            //DevConsole.Log("HELP");
            //jank ui component but it'll do -NiK0
            x = 52;
            collisionSize = new Vec2(width, 26);
            depth = -1;
            if (_borderVisible)
            {
                Color c = Colors.MenuOption;
                if (DGRSettings.mMatch != -1)
                {
                    if (text.Contains("P" + (DGRSettings.mMatch + 1))) c = Color.Yellow;
                }
                Graphics.DrawString(text, new Vec2(x - halfWidth + 8, y - 3.5f), c, depth + 1);
                _sections.scale = scale;
                _sections.alpha = alpha;
                _sections.depth = depth;
                _sections.frame = 0;
                Graphics.Draw(_sections, -halfWidth + x, -halfHeight + y);
                _sections.frame = 2;
                Graphics.Draw(_sections, (float)(halfWidth + x - _sections.w * scale.x), -halfHeight + y);
                _sections.frame = 1;
                _sections.xscale = (_collisionSize.x - _sections.w * 2) / _sections.w * xscale;
                Graphics.Draw(_sections, (float)(-halfWidth + x + _sections.w * scale.x), -halfHeight + y);
                _sections.xscale = xscale;
                _sections.frame = 3;
                _sections.yscale = (_collisionSize.y - _sections.h * 2) / _sections.h * yscale;
                Graphics.Draw(_sections, -halfWidth + x, (float)(-halfHeight + y + _sections.h * scale.y));
                _sections.frame = 5;
                Graphics.Draw(_sections, (float)(halfWidth + x - _sections.w * scale.x), (float)(-halfHeight + y + _sections.h * scale.y));
                _sections.frame = 4;
                _sections.xscale = (_collisionSize.x - _sections.w * 2) / _sections.w * xscale;
                Graphics.Draw(_sections, (float)(-halfWidth + x + _sections.w * scale.x), (float)(-halfHeight + y + _sections.h * scale.y));
                _sections.xscale = xscale;
                _sections.yscale = yscale;
                _sections.frame = 6;
                Graphics.Draw(_sections, -halfWidth + x, (float)(halfHeight + y - _sections.h * scale.y));
                _sections.frame = 8;
                Graphics.Draw(_sections, (float)(halfWidth + x - _sections.w * scale.x), (float)(halfHeight + y - _sections.h * scale.y));
                _sections.frame = 7;
                _sections.xscale = (_collisionSize.x - _sections.w * 2) / _sections.w * xscale;
                Graphics.Draw(_sections, (float)(-halfWidth + x + _sections.w * scale.x), (float)(halfHeight + y - _sections.h * scale.y));
            collisionSize = new Vec2(width, 32);
            }
            collisionSize = new Vec2(width, 0);
            base.Draw();
        }
    }
}
