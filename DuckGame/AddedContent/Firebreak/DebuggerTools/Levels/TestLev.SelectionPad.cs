using System;
using System.Collections.Generic;
using System.Linq;
using DuckGame.AddedContent.Drake.PolyRender;

namespace DuckGame
{
    public partial class TestLev
    {
        [ClientOnly]
        public class SelectionPad : SinkingBlock
        {
            public IEnumerable<Duck> CapturedDucks => CheckRectAll<Duck>(_collisionRect.tl, _collisionRect.br);
            protected override bool ShouldSink => CheckRectAll<PhysicsObject>(_collisionRect.tl, _collisionRect.br).Any();

            public SelectionPad(float x, float y, float width, float height, float sinkMultiplier = 2)
                : base(x, y, width, height, sinkMultiplier) { }

            public override void Update()
            {
                const float stepBoxWidth = 5f;
                Rectangle rect = new(x - stepBoxWidth, y + 0.2f, w + stepBoxWidth * 2, h - 0.2f);

                List<Duck> ducks = new();
                CheckRectAll(rect.tl, rect.br, ducks);

                for (int i = 0; i < ducks.Count; i++)
                {
                    Duck duck = ducks[i];
                    if (Math.Abs(duck.hSpeed) > 0.5f)
                        duck.y -= 1.2f;
                }

                if (CapturedDucks.Any())
                {
                    if (!HUD.core._cornerDisplays.Any() && Graphics.fade > 0.9f)
                        HUD.AddCornerMessage(HUDCorner.TopRight, $"SELECT @{Triggers.Shoot}@");

                    if (CapturedDucks.First().inputProfile.Pressed(Triggers.Shoot))
                    {
                        OnSelect();
                        HUD.CloseAllCorners();
                    }
                }
                else if (HUD.core._cornerDisplays.Any())
                {
                    HUD.CloseAllCorners();
                }

                base.Update();
            }

            public void OnSelect()
            {
                if (current is TestLev tl)
                    tl._transitionLevel = new TestLev();
            }

            public override void Draw()
            {
                if (_movementProgress > 0)
                    DrawShine();

                PolyRenderer.Rect(topLeft, bottomRight, Color.Black);
                PolyRenderer.Rect(topLeft, bottomRight, Color.Blue * Graphics.fade);

                base.Draw();
            }

            public void DrawShine()
            {
                const float shineHeight = 8;
                const float shineWidth = 4;

                Color lightColor = Color.Yellow * _movementProgress * 0.4f * Graphics.fade;

                PolyRenderer.Quad(
                    new Vec2(topLeft.x - shineWidth, topLeft.y - shineHeight),
                    new Vec2(topLeft.x - shineWidth, topLeft.y + shineHeight),
                    topLeft,
                    topRight,
                    Color.Transparent,
                    Color.Transparent,
                    lightColor,
                    lightColor
                );
            }
        }
    }
}