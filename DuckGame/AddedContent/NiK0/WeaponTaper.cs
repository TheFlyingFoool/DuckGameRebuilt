using NAudio.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Equipment")]
    public class WeaponTaper : Thing
    {
        public WeaponTaper()
        {
            graphic = new SpriteMap("backgroundIcons", 16, 16)
            {
                frame = 6
            };
            center = new Vec2(8f, 8f);

            editorTooltip = "Put 2 props in the level and place this on top to tape them";
        }
        public override void Draw()
        {
            if (frames < 2)
            {
                Graphics.DrawDottedRect(position - new Vec2(32), position + new Vec2(32), Color.Gray * 0.8f, 1);
            }
            base.Draw();
        }
        public int frames = 0;
        public override void EditorUpdate()
        {
            if (frames == 0)
            {
                Holdable h1 = Level.CheckRect<Holdable>(position - new Vec2(32), position + new Vec2(32));
                if (h1 != null && h1 is not TapedGun && h1.tapeable)
                {
                    Holdable h2 = Level.CheckRect<Holdable>(position - new Vec2(32), position + new Vec2(32), h1);
                    if (h2 is TapedGun || (h2 != null && !h2.tapeable)) h2 = null;
                    TapedGun tp = new TapedGun(x, y);
                    tp.gun1 = h1;
                    tp.gun2 = h2;
                    Holdable tms = tp.gun1.BecomeTapedMonster(tp);
                    if (h1 != null)
                    {
                        ((Editor)Level.current).levelThings.Remove(h1);
                        h1.tapeable = false;
                    }
                    if (h2 != null)
                    {
                        ((Editor)Level.current).levelThings.Remove(h2);
                        h2.tapeable = false;
                    }
                    if (tms != null)
                    {
                        ((Editor)Level.current).AddObject(tms);
                        tms.enablePhysics = false;
                    }
                    else
                    {
                        ((Editor)Level.current).AddObject(tp);
                        tp.enablePhysics = false;
                    }
                }
            }
            frames++;
            if (frames > 10)
            {
                ((Editor)Level.current).RemoveObject(this);
            }
            base.EditorUpdate();
        }
    }
}
