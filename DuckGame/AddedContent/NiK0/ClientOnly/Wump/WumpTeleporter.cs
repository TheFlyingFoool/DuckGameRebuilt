// Decompiled with JetBrains decompiler
// Type: DuckGame.Teleporter
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Rebuilt|Stuff")]
    public class WumpTeleporter : Teleporter
    {
        public EditorProperty<float> chargeTime = new EditorProperty<float>(1, null, 0, 60, 0.1f);
        public StateBinding _chargeTime = new StateBinding("charge");
        public WumpTeleporter(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _editorIcon.color = Color.Blue;
            _editorName = "Wump Teleporter";
            editorTooltip = "Place 2 teleporters pointing toward each other and Ducks can transport between them sometimes.";
            bloo = new Color(0, 100, 255);
            _bottom = new Sprite("wumpTeleBottom");
            _bottom.CenterOrigin();
            _top = new Sprite("wumpTeleTop");
            _top.CenterOrigin();
        }
        public Color bloo;
        public override void EditorPropertyChanged(object property)
        {
            UpdateHeight();
            if (noduck.value)
            {
                _bottom = new Sprite("WumplittleTeleBottom");
                _bottom.CenterOrigin();
                _top = new Sprite("WumplittleTeleTop");
                _top.CenterOrigin();
            }
            else
            {
                _bottom = new Sprite("wumpTeleBottom");
                _bottom.CenterOrigin();
                _top = new Sprite("wumpTeleTop");
                _top.CenterOrigin();
            }
        }
        public override void Update()
        {
            _pulse.Update();
            _float.Update();
            if (!_initLinks)
                InitLinks();
            if (_link == null)
                return;
            if (charge <= 0)
            {
                IEnumerable<ITeleport> source = Level.CheckRectAll<ITeleport>(topLeft, bottomRight);
                for (int index = 0; index < _teleported.Count; ++index)
                {
                    ITeleport teleport = _teleported[index];
                    if (!source.Contains(teleport))
                    {
                        _teleported.RemoveAt(index);
                        --index;
                    }
                }
                foreach (ITeleport teleport1 in source)
                {
                    if (noduck.value)
                    {
                        switch (teleport1)
                        {
                            case Duck _:
                            case Ragdoll _:
                            case RagdollPart _:
                            case TrappedDuck _:
                                continue;
                        }
                    }
                    ITeleport teleport2 = teleport1;
                    if ((teleport2 as Thing).owner == null && (teleport2 as Thing).isServerForObject && !_teleported.Contains(teleport2) && !_teleporting.Contains(teleport2))
                        _teleporting.Add(teleport2);
                }
                int num1;
                for (int index1 = 0; index1 < _teleporting.Count; index1 = num1 + 1)
                {
                    Thing thing1 = _teleporting[index1] as Thing;
                    _teleporting.RemoveAt(index1);
                    for (int index2 = 0; index2 < DGRSettings.ActualParticleMultiplier * 2; ++index2)
                        Level.Add(SmallSmoke.New(thing1.position.x + Rando.Float(-8f, 8f), thing1.position.y + Rando.Float(-8f, 8f)));
                    Vec2 position1 = thing1.position;
                    float num2 = 0f;
                    if (thing1 is RagdollPart)
                        num2 = 8f;
                    _link._teleported.Add(thing1 as ITeleport);
                    if ((int)teleHeight != 2 || (int)_link.teleHeight != 2)
                    {
                        if (_dir.y == 0.0)
                        {
                            thing1.x = _link.x - (x - thing1.x);
                            if (!horizontal.value)
                            {
                                if (thing1 is PhysicsObject)
                                {
                                    if (thing1.hSpeed > 0.0)
                                        thing1.position.x = _link.position.x + 8f;
                                    else
                                        thing1.position.x = _link.position.x - 8f;
                                }
                            }
                            else if (thing1 is PhysicsObject)
                            {
                                if (thing1.vSpeed > 0.0)
                                    thing1.position.y = _link.position.y + ((float)(thing1.height / 2.0 + 6.0) + num2);
                                else
                                    thing1.position.y = _link.position.y - ((float)(thing1.height / 2.0 + 6.0) + num2);
                            }
                        }
                        else
                        {
                            thing1.y = _link.y - (y - thing1.y);
                            if (!horizontal.value)
                            {
                                if (thing1 is PhysicsObject)
                                {
                                    if (thing1.hSpeed > 0.0)
                                        thing1.position.x = _link.position.x + 8f;
                                    else
                                        thing1.position.x = _link.position.x - 8f;
                                }
                            }
                            else if (thing1 is PhysicsObject)
                            {
                                if (thing1.vSpeed > 0.0)
                                    thing1.position.y = _link.position.y + ((float)(thing1.height / 2.0 + 6.0) + num2);
                                else
                                    thing1.position.y = _link.position.y - ((float)(thing1.height / 2.0 + 6.0) + num2);
                            }
                        }
                        if (!horizontal.value)
                        {
                            if (thing1.bottom > _link.bottom)
                                thing1.bottom = _link.bottom;
                            if (thing1.top < _link.top)
                                thing1.top = _link.top;
                        }
                        else
                        {
                            if (thing1.right > _link.right)
                                thing1.right = _link.right;
                            if (thing1.left < _link.left)
                                thing1.left = _link.left;
                        }
                    }
                    else
                    {
                        Thing thing2 = thing1;
                        Rectangle rectangle = _link.rectangle;
                        Vec2 center = rectangle.Center;
                        rectangle = this.rectangle;
                        Vec2 vec2_1 = rectangle.Center - thing1.position;
                        Vec2 vec2_2 = center - vec2_1;
                        thing2.position = vec2_2;
                        if (!horizontal.value)
                        {
                            if (thing1 is RagdollPart)
                                thing1.position.y = _link.position.y;
                            if (thing1 is PhysicsObject)
                            {
                                if (thing1.hSpeed > 0.0)
                                    thing1.position.x = _link.position.x + 8f;
                                else
                                    thing1.position.x = _link.position.x - 8f;
                            }
                        }
                        else
                        {
                            if (thing1 is RagdollPart)
                                thing1.position.x = _link.position.x;
                            if (thing1 is PhysicsObject)
                            {
                                if (thing1.vSpeed > 0.0)
                                    thing1.position.y = _link.position.y + 8f;
                                else
                                    thing1.position.y = _link.position.y - 8f;
                            }
                        }
                    }
                    for (int index3 = 0; index3 < DGRSettings.ActualParticleMultiplier * 2; ++index3)
                        Level.Add(SmallSmoke.New(thing1.position.x + Rando.Float(-8f, 8f), thing1.position.y + Rando.Float(-8f, 8f)));
                    num1 = index1 - 1;
                    Vec2 vec2 = position1;
                    Vec2 position2 = thing1.position;
                    if (thing1 is Duck && (thing1 as Duck).sliding)
                    {
                        vec2.y += 9f;
                        position2.y += 9f;
                    }
                    if (_dir.y != 0.0 && !horizontal.value)
                    {
                        vec2.x = position.x;
                        position2.x = _link.position.x;
                    }
                    float num3 = Math.Max(_dir.x != 0.0 ? thing1.height : thing1.width, 8f);
                    warpLines.Add(new WarpLine()
                    {
                        start = vec2,
                        end = position2,
                        lerp = 0.6f,
                        wide = num3
                    });
                    thing1.OnTeleport();
                    Fondle(this);
                    Fondle(_link);
                    if (_link is WumpTeleporter wt) wt.charge = chargeTime;
                    charge = chargeTime;
                }
            }
            else
            {
                charge -= 0.01666667f;
            }
        }
        public override void Initialize()
        {
            if (noduck.value)
            {
                _bottom = new Sprite("WumplittleTeleBottom");
                _bottom.CenterOrigin();
                _top = new Sprite("WumplittleTeleTop");
                _top.CenterOrigin();
            }
            _warpLine = new Sprite("warpLine2");
            UpdateHeight();
        }
        public override void Draw()
        {
            BasedDraw();
            if ((bool)horizontal)
            {
                if (charge > 0)
                {
                    Graphics.DrawRect(new Vec2(x + ((int)teleHeight * 16 - 9), y - 5f), new Vec2(x - 5f, y + 5f), Color.Gray * (float)(_pulse.normalized * 0.3f + 0.2f), depth);
                    _top.color = Color.Gray;
                }
                else
                {
                    if ((bool)noduck)
                        Graphics.DrawRect(new Vec2(x + ((int)teleHeight * 16 - 9), y - 2f), new Vec2(x - 5f, y + 2f), Color.DarkCyan * (float)(_pulse.normalized * 0.3f + 0.2f), depth);
                    else
                        Graphics.DrawRect(new Vec2(x + ((int)teleHeight * 16 - 9), y - 4f), new Vec2(x - 5f, y + 4f), bloo * (float)(_pulse.normalized * 0.3f + 0.2f), depth);
                }
                _top.angleDegrees = 90f;
                _bottom.angleDegrees = 90f;
                _top.depth = depth + 1;
                _bottom.depth = depth + 1;
                Graphics.Draw(_top, x + ((int)teleHeight * 16 - 9), y);
                Graphics.Draw(_bottom, x - 5f, y);
                _arrow.depth = depth + 2;
                _arrow.alpha = 0.5f;
                if (direction == 0)
                    _arrow.angleDegrees = 0f;
                else if (direction == 1)
                    _arrow.angleDegrees = 180f;
                else if (direction == 2)
                    _arrow.angleDegrees = -90f;
                else if (direction == 3)
                    _arrow.angleDegrees = 90f;
                Graphics.Draw(_arrow, (float)(x - 8.0 + (int)teleHeight * 16 / 2 + (float)_float * 2.0), y);
            }
            else
            {
                if (charge > 0)
                {
                    Graphics.DrawRect(new Vec2(x - 5f, y - ((int)teleHeight * 16 - 9)), new Vec2(x + 5f, y + 5f), Color.Gray * (float)(_pulse.normalized * 0.3f + 0.2f), depth);
                }
                else
                {
                    if ((bool)noduck)
                        Graphics.DrawRect(new Vec2(x - 2f, y - ((int)teleHeight * 16 - 9)), new Vec2(x + 2f, y + 5f), Color.DarkCyan * (float)(_pulse.normalized * 0.3f + 0.2f), depth);
                    else
                        Graphics.DrawRect(new Vec2(x - 4f, y - ((int)teleHeight * 16 - 9)), new Vec2(x + 4f, y + 5f), bloo * (float)(_pulse.normalized * 0.3f + 0.2f), depth);
                }
                _top.angle = 0f;
                _bottom.angle = 0f;
                _top.depth = depth + 1;
                _bottom.depth = depth + 1;
                Graphics.Draw(_top, x, y - ((int)teleHeight * 16 - 9));
                Graphics.Draw(_bottom, x, y + 5f);
                _arrow.depth = depth + 2;
                _arrow.alpha = 0.5f;
                if (direction == 0)
                    _arrow.angleDegrees = 0f;
                else if (direction == 1)
                    _arrow.angleDegrees = 180f;
                else if (direction == 2)
                    _arrow.angleDegrees = -90f;
                else if (direction == 3)
                    _arrow.angleDegrees = 90f;
                Graphics.Draw(_arrow, x, (float)(y + 8.0 - (int)teleHeight * 16 / 2 + (float)_float * 2.0));
            }
        }
        public float charge;
        public override void DrawGlow()
        {
            Color color = bloo;
            if ((bool)noduck)
                color = Color.Red;
            foreach (WarpLine warpLine in warpLines)
            {
                Vec2 vec2_1 = warpLine.start - warpLine.end;
                Vec2 vec2_2 = warpLine.end - warpLine.start;
                Graphics.DrawTexturedLine(_warpLine.texture, warpLine.end, warpLine.end + vec2_1 * (1f - warpLine.lerp), color * 0.8f, warpLine.wide / 32f, (Depth)0.9f);
                Graphics.DrawTexturedLine(_warpLine.texture, warpLine.start + vec2_2 * warpLine.lerp, warpLine.start, color * 0.8f, warpLine.wide / 32f, (Depth)0.9f);
                warpLine.lerp += 0.1f;
            }
            warpLines.RemoveAll(v => v.lerp >= 1.0);
        }
    }
}
