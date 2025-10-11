﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [EditorGroup("Stuff")]
    public class Teleporter : MaterialThing
    {
        public List<WarpLine> warpLines = new List<WarpLine>();
        protected Sprite _bottom;
        protected Sprite _top;
        protected SinWaveManualUpdate _pulse = (SinWaveManualUpdate)0.1f;
        protected SinWaveManualUpdate _float = (SinWaveManualUpdate)0.2f;
        protected Sprite _arrow;
        public Teleporter _link;
        public EditorProperty<bool> noduck;
        public EditorProperty<int> teleHeight;
        public EditorProperty<bool> horizontal;
        public Sprite _warpLine;
        protected bool _initLinks;
        protected List<ITeleport> _teleporting = new List<ITeleport>();
        public List<ITeleport> _teleported = new List<ITeleport>();
        public Vec2 _dir;
        public int direction;
        public bool newVersion = true;

        public Teleporter link => _link;

        public Teleporter(float xpos, float ypos)
          : base(xpos, ypos)
        {
            teleHeight = new EditorProperty<int>(2, this, 1f, 16f, 1f)
            {
                name = "height"
            };
            noduck = new EditorProperty<bool>(false, this);
            horizontal = new EditorProperty<bool>(false, this);
            center = new Vec2(8f, 24f);
            collisionSize = new Vec2(6f, 32f);
            collisionOffset = new Vec2(-3f, -24f);
            depth = -0.5f;
            _editorName = nameof(Teleporter);
            editorTooltip = "Place 2 teleporters pointing toward each other and Ducks can transport between them.";
            _editorIcon = new Sprite("teleporterIcon");
            _bottom = new Sprite("teleporterBottom");
            _bottom.CenterOrigin();
            _top = new Sprite("teleporterTop");
            _top.CenterOrigin();
            _arrow = new Sprite("upArrow");
            _arrow.CenterOrigin();
            thickness = 99f;
            _placementCost += 2;
        }

        public override void EditorPropertyChanged(object property)
        {
            UpdateHeight();
            if (noduck.value)
            {
                _bottom = new Sprite("littleTeleBottom");
                _bottom.CenterOrigin();
                _top = new Sprite("littleTeleTop");
                _top.CenterOrigin();
            }
            else
            {
                _bottom = new Sprite("teleporterBottom");
                _bottom.CenterOrigin();
                _top = new Sprite("teleporterTop");
                _top.CenterOrigin();
            }
        }

        protected void UpdateHeight()
        {
            if ((bool)horizontal)
            {
                center = new Vec2(24f, 8f);
                collisionSize = new Vec2((int)teleHeight * 16, 6f);
                collisionOffset = new Vec2(-8f, -3f);
            }
            else
            {
                center = new Vec2(8f, 24f);
                collisionSize = new Vec2(6f, (int)teleHeight * 16);
                collisionOffset = new Vec2(-3f, -((int)teleHeight * 16 - 8));
            }
        }

        public override void Initialize()
        {
            if (noduck.value)
            {
                _bottom = new Sprite("littleTeleBottom");
                _bottom.CenterOrigin();
                _top = new Sprite("littleTeleTop");
                _top.CenterOrigin();
            }
            _warpLine = new Sprite("warpLine2");
            UpdateHeight();
            base.Initialize();
        }

        public override void TabRotate()
        {
            if (Keyboard.control)
            {
                direction += 4;
                horizontal.value = !horizontal.value;
            }
            else
            {
                switch (direction)
                {
                    case 0:
                        direction = 3;
                        break;
                    case 1:
                        direction = 2;
                        break;
                    case 2:
                        direction = 0;
                        break;
                    case 3:
                        direction = 1;
                        break;
                    default:
                        direction = 0;
                        break;
                }
            }
        }

        public void InitLinks()
        {
            _initLinks = true;
            Vec2 vec2_1 = new Vec2(0f, -1f);
            if (direction == 1)
                vec2_1 = new Vec2(0f, 1f);
            else if (direction == 2)
                vec2_1 = new Vec2(-1f, 0f);
            else if (direction == 3)
                vec2_1 = new Vec2(1f, 0f);
            Vec2 hitPos;
            if (horizontal.value)
            {
                Vec2 center = rectangle.Center;
                Teleporter teleporter = Level.CheckRay<Teleporter>(center + vec2_1 * 20f, center + vec2_1 * 5000f, this, out hitPos);
                if (teleporter != null)
                    _link = teleporter;
            }
            else
            {
                Vec2 vec2_2 = position + new Vec2(0f, (float)-((int)teleHeight * 16f / 2f - 8f));
                Teleporter teleporter = Level.CheckRay<Teleporter>(vec2_2 + vec2_1 * 20f, vec2_2 + vec2_1 * 5000f, this, out hitPos);
                if (teleporter != null)
                    _link = teleporter;
            }
            _dir = vec2_1;
        }

        public void Shine()
        {
            if (DGRSettings.EnhancedTextures)
            {
                if (noduck) shine = 0.8f;
                else shine = 1.2f;
                if (_link != null)
                {
                    if (_link.noduck) _link.shine = 0.8f;
                    else _link.shine = 1.2f;
                }
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

            //reverted this back to optimized dgr's collision system, exact fix is in Thing.OnTeleport() -Lucky
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
                if ((teleport2 as Thing).owner == null && !_teleported.Contains(teleport2) && !_teleporting.Contains(teleport2))
                {
                    Shine();
                    if ((teleport2 as Thing).isServerForObject) _teleporting.Add(teleport2);
                }
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
                    if (_dir.y == 0)
                    {
                        thing1.x = _link.x - (x - thing1.x);
                        if (!horizontal.value)
                        {
                            if (thing1 is PhysicsObject)
                            {
                                if (thing1.hSpeed > 0)
                                    thing1.position.x = _link.position.x + 8f;
                                else
                                    thing1.position.x = _link.position.x - 8f;
                            }
                        }
                        else if (thing1 is PhysicsObject)
                        {
                            if (thing1.vSpeed > 0)
                                thing1.position.y = _link.position.y + ((float)(thing1.height / 2 + 6) + num2);
                            else
                                thing1.position.y = _link.position.y - ((float)(thing1.height / 2 + 6) + num2);
                        }
                    }
                    else
                    {
                        thing1.y = _link.y - (y - thing1.y);
                        if (!horizontal.value)
                        {
                            if (thing1 is PhysicsObject)
                            {
                                if (thing1.hSpeed > 0)
                                    thing1.position.x = _link.position.x + 8f;
                                else
                                    thing1.position.x = _link.position.x - 8f;
                            }
                        }
                        else if (thing1 is PhysicsObject)
                        {
                            if (thing1.vSpeed > 0)
                                thing1.position.y = _link.position.y + ((float)(thing1.height / 2 + 6) + num2);
                            else
                                thing1.position.y = _link.position.y - ((float)(thing1.height / 2 + 6) + num2);
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
                            if (thing1.hSpeed > 0)
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
                            if (thing1.vSpeed > 0)
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
                if (_dir.y != 0 && !horizontal.value)
                {
                    vec2.x = position.x;
                    position2.x = _link.position.x;
                }
                float num3 = Math.Max(_dir.x != 0 ? thing1.height : thing1.width, 8f);
                warpLines.Add(new WarpLine()
                {
                    start = vec2,
                    end = position2,
                    lerp = 0.6f,
                    wide = num3
                });
                thing1.OnTeleport();
            }
        }

        public override void DrawGlow()
        {
            Color color = Color.Purple;
            if ((bool)noduck)
                color = Color.Yellow;
            foreach (WarpLine warpLine in warpLines)
            {
                Vec2 vec2_1 = warpLine.start - warpLine.end;
                Vec2 vec2_2 = warpLine.end - warpLine.start;
                Graphics.DrawTexturedLine(_warpLine.texture, warpLine.end, warpLine.end + vec2_1 * (1f - warpLine.lerp), color * 0.8f, warpLine.wide / 32f, (Depth)0.9f);
                Graphics.DrawTexturedLine(_warpLine.texture, warpLine.start + vec2_2 * warpLine.lerp, warpLine.start, color * 0.8f, warpLine.wide / 32f, (Depth)0.9f);
                if (MonoMain.UpdateLerpState) warpLine.lerp += 0.1f;
            }
            warpLines.RemoveAll(v => v.lerp >= 1);
        }
        //REBUILT STUFF
        public void BasedDraw()
        {
            base.Draw();
        }
        public float shine;
        public override void Draw()
        {
            base.Draw();
            if (MonoMain.UpdateLerpState) shine = Lerp.Float(shine, 0, 0.05f);
            if ((bool)horizontal)
            {
                Color purple = Color.Purple;
                if ((bool)noduck)
                    Graphics.DrawRect(new Vec2(x + ((int)teleHeight * 16 - 9), y - 2f), new Vec2(x - 5f, y + 2f), Color.Yellow * (float)(_pulse.normalized * 0.3f + 0.2f + shine), depth);
                else
                    Graphics.DrawRect(new Vec2(x + ((int)teleHeight * 16 - 9), y - 4f), new Vec2(x - 5f, y + 4f), purple * (float)(_pulse.normalized * 0.3f + 0.2f + shine), depth);
                _top.angleDegrees = 90f;
                _bottom.angleDegrees = 90f;
                _top.depth = depth + 1;
                _bottom.depth = depth + 1;
                Graphics.Draw(ref _top, x + ((int)teleHeight * 16 - 9), y);
                Graphics.Draw(ref _bottom, x - 5f, y);
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
                Graphics.Draw(ref _arrow, (float)(x - 8 + (int)teleHeight * 16f / 2f + (float)_float * 2), y);
            }
            else
            {
                Color purple = Color.Purple;
                if ((bool)noduck)
                    Graphics.DrawRect(new Vec2(x - 2f, y - ((int)teleHeight * 16 - 9)), new Vec2(x + 2f, y + 5f), Color.Yellow * (float)(_pulse.normalized * 0.3f + 0.2f + shine), depth);
                else
                    Graphics.DrawRect(new Vec2(x - 4f, y - ((int)teleHeight * 16 - 9)), new Vec2(x + 4f, y + 5f), purple * (float)(_pulse.normalized * 0.3f + 0.2f + shine), depth);
                _top.angle = 0f;
                _bottom.angle = 0f;
                _top.depth = depth + 1;
                _bottom.depth = depth + 1;
                Graphics.Draw(ref _top, x, y - ((int)teleHeight * 16 - 9));
                Graphics.Draw(ref _bottom, x, y + 5f);
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
                Graphics.Draw(ref _arrow, x, (float)(y + 8f - (int)teleHeight * 16f / 2f + (float)_float * 2f));
            }
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("direction", direction);
            binaryClassChunk.AddProperty("newVersion", true);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            newVersion = node.GetProperty<bool>("newVersion");
            if (!newVersion)
                teleHeight.value = 2;
            newVersion = true;
            direction = node.GetProperty<int>("direction");
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = base.LegacySerialize();
            dxmlNode.Add(new DXMLNode("direction", Change.ToString(direction)));
            return dxmlNode;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            base.LegacyDeserialize(node);
            teleHeight.value = 2;
            DXMLNode dxmlNode = node.Element("direction");
            if (dxmlNode != null)
                direction = Convert.ToInt32(dxmlNode.Value);
            return true;
        }

        public override ContextMenu GetContextMenu()
        {
            EditorGroupMenu contextMenu = base.GetContextMenu() as EditorGroupMenu;
            contextMenu.AddItem(new ContextRadio("Up", direction == 0, 0, null, new FieldBinding(this, "direction")));
            contextMenu.AddItem(new ContextRadio("Down", direction == 1, 1, null, new FieldBinding(this, "direction")));
            contextMenu.AddItem(new ContextRadio("Left", direction == 2, 2, null, new FieldBinding(this, "direction")));
            contextMenu.AddItem(new ContextRadio("Right", direction == 3, 3, null, new FieldBinding(this, "direction")));
            return contextMenu;
        }

        public override void EditorUpdate()
        {
            _pulse.Update();
            _float.Update();
            base.EditorUpdate();
        }
    }
}