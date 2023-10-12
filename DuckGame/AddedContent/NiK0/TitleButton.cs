using NAudio.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    public class TitleButton : MaterialThing, IPlatform
    {
        public SpriteMap sprite;
        public bool fashion;
        public TitleButton(float xpos, float ypos, bool ff) : base(xpos, ypos)
        {
            fashion = ff;
            sprite = new SpriteMap("titleButtons", 13, 4);
            graphic = sprite;
            if (ff)
            {
                sprite.imageIndex = 1;
                sprite.frame = 1;
            }
            center = new Vec2(6.5f, 2);
            collisionSize = new Vec2(13, 4);
            _collisionOffset = new Vec2(-6.5f, -2f);
            depth = -1;
        }
        public CorinthianPillar drag;
        public MaterialThing lt;
        public override void Update()
        {
            if (lt != null)
            {
                y++;
                drag.y++;
                if (lt.impacting.Contains(this))
                {
                    lt.vSpeed += 1;
                }
                lt.vSpeed += 0.01f;
                if (y > 170)
                {
                    Level.Remove(this);
                    Level.Remove(drag);
                }
            }
            base.Update();
        }
        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with is Duck && from == ImpactedFrom.Top)
            {
                if (Level.current is TitleScreen ts && lt == null)
                {
                    lt = with;
                    SFX.Play("click");
                    if (fashion)
                    {
                        ts._featherFashionCircle.position = position - new Vec2(-12, 48);
                    }
                    else
                    {
                        ts._recorderatorCircle.position = position - new Vec2(12, 48);
                    }
                }
            }
            base.OnSolidImpact(with, from);
        }
    }
}
