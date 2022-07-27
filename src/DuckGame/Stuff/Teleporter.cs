// Decompiled with JetBrains decompiler
// Type: DuckGame.Teleporter
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [EditorGroup("Stuff")]
    public class Teleporter : MaterialThing
    {
        public List<WarpLine> warpLines = new List<WarpLine>();
        private Sprite _bottom;
        private Sprite _top;
        private SinWaveManualUpdate _pulse = (SinWaveManualUpdate)0.1f;
        private SinWaveManualUpdate _float = (SinWaveManualUpdate)0.2f;
        private Sprite _arrow;
        public Teleporter _link;
        public EditorProperty<bool> noduck = new EditorProperty<bool>(false);
        public EditorProperty<int> teleHeight;
        public EditorProperty<bool> horizontal;
        private Sprite _warpLine;
        private bool _initLinks;
        private List<ITeleport> _teleporting = new List<ITeleport>();
        private List<ITeleport> _teleported = new List<ITeleport>();
        public Vec2 _dir;
        public int direction;
        public bool newVersion = true;

        public Teleporter link => this._link;

        public Teleporter(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.teleHeight = new EditorProperty<int>(2, this, 1f, 16f, 1f)
            {
                name = "height"
            };
            this.horizontal = new EditorProperty<bool>(false, this);
            this.center = new Vec2(8f, 24f);
            this.collisionSize = new Vec2(6f, 32f);
            this.collisionOffset = new Vec2(-3f, -24f);
            this.depth = -0.5f;
            this._editorName = nameof(Teleporter);
            this.editorTooltip = "Place 2 teleporters pointing toward each other and Ducks can transport between them.";
            this._editorIcon = new Sprite("teleporterIcon");
            this._bottom = new Sprite("teleporterBottom");
            this._bottom.CenterOrigin();
            this._top = new Sprite("teleporterTop");
            this._top.CenterOrigin();
            this._arrow = new Sprite("upArrow");
            this._arrow.CenterOrigin();
            this.thickness = 99f;
            this._placementCost += 2;
        }

        public override void EditorPropertyChanged(object property) => this.UpdateHeight();

        private void UpdateHeight()
        {
            if ((bool)this.horizontal)
            {
                this.center = new Vec2(24f, 8f);
                this.collisionSize = new Vec2((int)this.teleHeight * 16, 6f);
                this.collisionOffset = new Vec2(-8f, -3f);
            }
            else
            {
                this.center = new Vec2(8f, 24f);
                this.collisionSize = new Vec2(6f, (int)this.teleHeight * 16);
                this.collisionOffset = new Vec2(-3f, -((int)this.teleHeight * 16 - 8));
            }
        }

        public override void Initialize()
        {
            if (this.noduck.value)
            {
                this._bottom = new Sprite("littleTeleBottom");
                this._bottom.CenterOrigin();
                this._top = new Sprite("littleTeleTop");
                this._top.CenterOrigin();
            }
            this._warpLine = new Sprite("warpLine2");
            this.UpdateHeight();
            base.Initialize();
        }

        public override void TabRotate()
        {
            if (Keyboard.control)
            {
                this.direction += 4;
                this.horizontal.value = !this.horizontal.value;
            }
            else
                ++this.direction;
            if (this.direction <= 3)
                return;
            this.direction = 0;
        }

        public void InitLinks()
        {
            this._initLinks = true;
            Vec2 vec2_1 = new Vec2(0.0f, -1f);
            if (this.direction == 1)
                vec2_1 = new Vec2(0.0f, 1f);
            else if (this.direction == 2)
                vec2_1 = new Vec2(-1f, 0.0f);
            else if (this.direction == 3)
                vec2_1 = new Vec2(1f, 0.0f);
            Vec2 hitPos;
            if (this.horizontal.value)
            {
                Vec2 center = this.rectangle.Center;
                Teleporter teleporter = Level.CheckRay<Teleporter>(center + vec2_1 * 20f, center + vec2_1 * 5000f, this, out hitPos);
                if (teleporter != null)
                    this._link = teleporter;
            }
            else
            {
                Vec2 vec2_2 = this.position + new Vec2(0.0f, (float)-((int)this.teleHeight * 16.0 / 2.0 - 8.0));
                Teleporter teleporter = Level.CheckRay<Teleporter>(vec2_2 + vec2_1 * 20f, vec2_2 + vec2_1 * 5000f, this, out hitPos);
                if (teleporter != null)
                    this._link = teleporter;
            }
            this._dir = vec2_1;
        }

        public override void Update()
        {
            this._pulse.Update();
            this._float.Update();
            if (!this._initLinks)
                this.InitLinks();
            if (this._link == null)
                return;
            IEnumerable<ITeleport> source = Level.CheckRectAll<ITeleport>(this.topLeft, this.bottomRight);
            for (int index = 0; index < this._teleported.Count; ++index)
            {
                ITeleport teleport = this._teleported[index];
                if (!source.Contains<ITeleport>(teleport))
                {
                    this._teleported.RemoveAt(index);
                    --index;
                }
            }
            foreach (ITeleport teleport1 in source)
            {
                if (this.noduck.value)
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
                if ((teleport2 as Thing).owner == null && (teleport2 as Thing).isServerForObject && !this._teleported.Contains(teleport2) && !this._teleporting.Contains(teleport2))
                    this._teleporting.Add(teleport2);
            }
            int num1;
            for (int index1 = 0; index1 < this._teleporting.Count; index1 = num1 + 1)
            {
                Thing thing1 = this._teleporting[index1] as Thing;
                this._teleporting.RemoveAt(index1);
                for (int index2 = 0; index2 < 2; ++index2)
                    Level.Add(SmallSmoke.New(thing1.position.x + Rando.Float(-8f, 8f), thing1.position.y + Rando.Float(-8f, 8f)));
                Vec2 position1 = thing1.position;
                float num2 = 0.0f;
                if (thing1 is RagdollPart)
                    num2 = 8f;
                this._link._teleported.Add(thing1 as ITeleport);
                if ((int)this.teleHeight != 2 || (int)this._link.teleHeight != 2)
                {
                    if (_dir.y == 0.0)
                    {
                        thing1.x = this._link.x - (this.x - thing1.x);
                        if (!this.horizontal.value)
                        {
                            if (thing1 is PhysicsObject)
                            {
                                if ((double)thing1.hSpeed > 0.0)
                                    thing1.position.x = this._link.position.x + 8f;
                                else
                                    thing1.position.x = this._link.position.x - 8f;
                            }
                        }
                        else if (thing1 is PhysicsObject)
                        {
                            if ((double)thing1.vSpeed > 0.0)
                                thing1.position.y = this._link.position.y + ((float)((double)thing1.height / 2.0 + 6.0) + num2);
                            else
                                thing1.position.y = this._link.position.y - ((float)((double)thing1.height / 2.0 + 6.0) + num2);
                        }
                    }
                    else
                    {
                        thing1.y = this._link.y - (this.y - thing1.y);
                        if (!this.horizontal.value)
                        {
                            if (thing1 is PhysicsObject)
                            {
                                if ((double)thing1.hSpeed > 0.0)
                                    thing1.position.x = this._link.position.x + 8f;
                                else
                                    thing1.position.x = this._link.position.x - 8f;
                            }
                        }
                        else if (thing1 is PhysicsObject)
                        {
                            if ((double)thing1.vSpeed > 0.0)
                                thing1.position.y = this._link.position.y + ((float)((double)thing1.height / 2.0 + 6.0) + num2);
                            else
                                thing1.position.y = this._link.position.y - ((float)((double)thing1.height / 2.0 + 6.0) + num2);
                        }
                    }
                    if (!this.horizontal.value)
                    {
                        if ((double)thing1.bottom > (double)this._link.bottom)
                            thing1.bottom = this._link.bottom;
                        if ((double)thing1.top < (double)this._link.top)
                            thing1.top = this._link.top;
                    }
                    else
                    {
                        if ((double)thing1.right > (double)this._link.right)
                            thing1.right = this._link.right;
                        if ((double)thing1.left < (double)this._link.left)
                            thing1.left = this._link.left;
                    }
                }
                else
                {
                    Thing thing2 = thing1;
                    Rectangle rectangle = this._link.rectangle;
                    Vec2 center = rectangle.Center;
                    rectangle = this.rectangle;
                    Vec2 vec2_1 = rectangle.Center - thing1.position;
                    Vec2 vec2_2 = center - vec2_1;
                    thing2.position = vec2_2;
                    if (!this.horizontal.value)
                    {
                        if (thing1 is RagdollPart)
                            thing1.position.y = this._link.position.y;
                        if (thing1 is PhysicsObject)
                        {
                            if ((double)thing1.hSpeed > 0.0)
                                thing1.position.x = this._link.position.x + 8f;
                            else
                                thing1.position.x = this._link.position.x - 8f;
                        }
                    }
                    else
                    {
                        if (thing1 is RagdollPart)
                            thing1.position.x = this._link.position.x;
                        if (thing1 is PhysicsObject)
                        {
                            if ((double)thing1.vSpeed > 0.0)
                                thing1.position.y = this._link.position.y + 8f;
                            else
                                thing1.position.y = this._link.position.y - 8f;
                        }
                    }
                }
                for (int index3 = 0; index3 < 2; ++index3)
                    Level.Add(SmallSmoke.New(thing1.position.x + Rando.Float(-8f, 8f), thing1.position.y + Rando.Float(-8f, 8f)));
                num1 = index1 - 1;
                Vec2 vec2 = position1;
                Vec2 position2 = thing1.position;
                if (thing1 is Duck && (thing1 as Duck).sliding)
                {
                    vec2.y += 9f;
                    position2.y += 9f;
                }
                if (_dir.y != 0.0 && !this.horizontal.value)
                {
                    vec2.x = this.position.x;
                    position2.x = this._link.position.x;
                }
                float num3 = Math.Max(_dir.x != 0.0 ? thing1.height : thing1.width, 8f);
                this.warpLines.Add(new WarpLine()
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
            if ((bool)this.noduck)
                color = Color.Yellow;
            foreach (WarpLine warpLine in this.warpLines)
            {
                Vec2 vec2_1 = warpLine.start - warpLine.end;
                Vec2 vec2_2 = warpLine.end - warpLine.start;
                Graphics.DrawTexturedLine(this._warpLine.texture, warpLine.end, warpLine.end + vec2_1 * (1f - warpLine.lerp), color * 0.8f, warpLine.wide / 32f, (Depth)0.9f);
                Graphics.DrawTexturedLine(this._warpLine.texture, warpLine.start + vec2_2 * warpLine.lerp, warpLine.start, color * 0.8f, warpLine.wide / 32f, (Depth)0.9f);
                warpLine.lerp += 0.1f;
            }
            this.warpLines.RemoveAll(v => v.lerp >= 1.0);
        }

        public override void Draw()
        {
            base.Draw();
            if ((bool)this.horizontal)
            {
                Color purple = Color.Purple;
                if ((bool)this.noduck)
                    Graphics.DrawRect(new Vec2(this.x + ((int)this.teleHeight * 16 - 9), this.y - 2f), new Vec2(this.x - 5f, this.y + 2f), Color.Yellow * (float)((double)this._pulse.normalized * 0.300000011920929 + 0.200000002980232), this.depth);
                else
                    Graphics.DrawRect(new Vec2(this.x + ((int)this.teleHeight * 16 - 9), this.y - 4f), new Vec2(this.x - 5f, this.y + 4f), purple * (float)((double)this._pulse.normalized * 0.300000011920929 + 0.200000002980232), this.depth);
                this._top.angleDegrees = 90f;
                this._bottom.angleDegrees = 90f;
                this._top.depth = this.depth + 1;
                this._bottom.depth = this.depth + 1;
                Graphics.Draw(this._top, this.x + ((int)this.teleHeight * 16 - 9), this.y);
                Graphics.Draw(this._bottom, this.x - 5f, this.y);
                this._arrow.depth = this.depth + 2;
                this._arrow.alpha = 0.5f;
                if (this.direction == 0)
                    this._arrow.angleDegrees = 0.0f;
                else if (this.direction == 1)
                    this._arrow.angleDegrees = 180f;
                else if (this.direction == 2)
                    this._arrow.angleDegrees = -90f;
                else if (this.direction == 3)
                    this._arrow.angleDegrees = 90f;
                Graphics.Draw(this._arrow, (float)((double)this.x - 8.0 + (int)this.teleHeight * 16 / 2 + (double)(float)this._float * 2.0), this.y);
            }
            else
            {
                Color purple = Color.Purple;
                if ((bool)this.noduck)
                    Graphics.DrawRect(new Vec2(this.x - 2f, this.y - ((int)this.teleHeight * 16 - 9)), new Vec2(this.x + 2f, this.y + 5f), Color.Yellow * (float)((double)this._pulse.normalized * 0.300000011920929 + 0.200000002980232), this.depth);
                else
                    Graphics.DrawRect(new Vec2(this.x - 4f, this.y - ((int)this.teleHeight * 16 - 9)), new Vec2(this.x + 4f, this.y + 5f), purple * (float)((double)this._pulse.normalized * 0.300000011920929 + 0.200000002980232), this.depth);
                this._top.angle = 0.0f;
                this._bottom.angle = 0.0f;
                this._top.depth = this.depth + 1;
                this._bottom.depth = this.depth + 1;
                Graphics.Draw(this._top, this.x, this.y - ((int)this.teleHeight * 16 - 9));
                Graphics.Draw(this._bottom, this.x, this.y + 5f);
                this._arrow.depth = this.depth + 2;
                this._arrow.alpha = 0.5f;
                if (this.direction == 0)
                    this._arrow.angleDegrees = 0.0f;
                else if (this.direction == 1)
                    this._arrow.angleDegrees = 180f;
                else if (this.direction == 2)
                    this._arrow.angleDegrees = -90f;
                else if (this.direction == 3)
                    this._arrow.angleDegrees = 90f;
                Graphics.Draw(this._arrow, this.x, (float)((double)this.y + 8.0 - (int)this.teleHeight * 16 / 2 + (double)(float)this._float * 2.0));
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
            this.newVersion = node.GetProperty<bool>("newVersion");
            if (!this.newVersion)
                this.teleHeight.value = 2;
            this.newVersion = true;
            this.direction = node.GetProperty<int>("direction");
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
            this.teleHeight.value = 2;
            DXMLNode dxmlNode = node.Element("direction");
            if (dxmlNode != null)
                this.direction = Convert.ToInt32(dxmlNode.Value);
            return true;
        }

        public override ContextMenu GetContextMenu()
        {
            EditorGroupMenu contextMenu = base.GetContextMenu() as EditorGroupMenu;
            contextMenu.AddItem(new ContextRadio("Up", this.direction == 0, 0, null, new FieldBinding(this, "direction")));
            contextMenu.AddItem(new ContextRadio("Down", this.direction == 1, 1, null, new FieldBinding(this, "direction")));
            contextMenu.AddItem(new ContextRadio("Left", this.direction == 2, 2, null, new FieldBinding(this, "direction")));
            contextMenu.AddItem(new ContextRadio("Right", this.direction == 3, 3, null, new FieldBinding(this, "direction")));
            return contextMenu;
        }

        public override void EditorUpdate()
        {
            this._pulse.Update();
            this._float.Update();
            base.EditorUpdate();
        }
    }
}
