// Decompiled with JetBrains decompiler
// Type: DuckGame.SpawnCannon
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Spawns")]
    [BaggedProperty("isInDemo", false)]
    [BaggedProperty("previewPriority", false)]
    public class SpawnCannon : ItemSpawner, IWirePeripheral, ISequenceItem
    {
        public EditorProperty<int> bing;
        public EditorProperty<bool> showClock;
        public EditorProperty<int> cannonColor;
        public float fireDirection;
        public float firePower = 5f;
        private float _startupDelay;
        private Sprite _arrowHead;
        private bool initializedWired;
        private bool wired;
        private bool wasPulse;
        private bool _running;
        private int beeps;
        private PhysicsObject _hoverThing;
        private Vec2 _scaleLerp = Vec2.One;

        public float direction => this.fireDirection + (this.flipHorizontal ? 180f : 0f);

        public override void EditorPropertyChanged(object property)
        {
            if (this.showClock.value)
                this._sprite = new SpriteMap("cannonTimer", 18, 18);
            else
                this._sprite = new SpriteMap("cannon", 18, 18);
            this.graphic = _sprite;
        }

        public SpawnCannon(float xpos, float ypos, System.Type c = null)
          : base(xpos, ypos)
        {
            this.bing = new EditorProperty<int>(0, this, max: 240f, increment: 1f, minSpecial: "none")
            {
                _tooltip = "If set, this cannon will BING this many frames before it activates."
            };
            this.showClock = new EditorProperty<bool>(false, this);
            this.cannonColor = new EditorProperty<int>(0, this, max: 3f, increment: 1f);
            this._arrowHead = new Sprite("arrowHead", new Vec2(3.5f, 8f));
            this._sprite = new SpriteMap("cannon", 18, 18);
            this.graphic = _sprite;
            this.center = new Vec2(7f, 9f);
            this.collisionSize = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-6f, -6f);
            this.depth = (Depth)0.8f;
            this.contains = c;
            this.hugWalls = WallHug.None;
            this._placementCost += 4;
            this.editorTooltip = "Shoots the specified item in the specified direction after the specified delay.";
            this.sequence = new SequenceItem(this)
            {
                type = SequenceItemType.Activator
            };
        }

        public override void OnSequenceActivate()
        {
            SFX.Play("basketball", 0.8f, Rando.Float(0.2f, 0.4f));
            this.scale = new Vec2(2f, 2f);
            this._running = true;
        }

        public override void Initialize()
        {
            if (this.spawnOnStart)
                this._spawnWait = this.spawnTime;
            this._startupDelay = this.initialDelay;
        }

        public void Spawn()
        {
            if (!Network.isServer && !this.wasPulse)
            {
                this._spawnWait = 0f;
                ++this._numSpawned;
            }
            else
            {
                if (this.sequence != null)
                {
                    if (this.sequence.waitTillOrder)
                        this._running = false;
                    this.sequence.Finished();
                }
                this.xscale = 2f;
                this.yscale = 0.5f;
                if (!this.initializedWired)
                {
                    WireTileset wireTileset = Level.current.NearestThing<WireTileset>(this.position);
                    if (wireTileset != null && (double)(wireTileset.position - this.position).length < 1.0)
                        this.wired = true;
                    this.initializedWired = true;
                }
                if (!this.wasPulse && this.wired)
                    return;
                if (this.randomSpawn && this.keepRandom)
                {
                    List<System.Type> physicsObjects = ItemBox.GetPhysicsObjects(Editor.Placeables);
                    this.contains = physicsObjects[Rando.Int(physicsObjects.Count - 1)];
                }
                else if (this.possible.Count > 0)
                {
                    System.Type type = MysteryGun.PickType(this.chanceGroup, this.possible);
                    if (type != null)
                        this.contains = type;
                }
                this._spawnWait = 0f;
                ++this._numSpawned;
                if (this.contains == null || !(Editor.CreateThing(this.contains) is PhysicsObject thing))
                    return;
                Vec2 vec2 = Maths.AngleToVec(Maths.DegToRad(this.direction)) * this.firePower;
                thing.position = this.position + vec2.normalized * 8f;
                thing.hSpeed = vec2.x;
                thing.vSpeed = vec2.y;
                Level.Add(thing);
                thing.Ejected(this);
                Level.Add(SmallSmoke.New(thing.x, thing.y));
                Level.Add(SmallSmoke.New(thing.x, thing.y));
                SFX.Play("netGunFire", Rando.Float(0.9f, 1f), Rando.Float(-0.1f, 0.1f));
                if (thing is Equipment)
                    (thing as Equipment).autoEquipTime = 0.5f;
                if (thing is ChokeCollar)
                {
                    (thing as ChokeCollar).ball.hSpeed = thing.hSpeed;
                    (thing as ChokeCollar).ball.vSpeed = thing.vSpeed;
                }
                if (!(thing is Sword))
                    return;
                (thing as Sword)._wasLifted = true;
                (thing as Sword)._framesExisting = 16;
            }
        }

        public override void Update()
        {
            this.scale = Lerp.Vec2Smooth(this.scale, Vec2.One, 0.2f);
            if ((this.sequence == null || !this.sequence.waitTillOrder || this._running) && (this._numSpawned < this.spawnNum || this.spawnNum == -1))
            {
                if (Level.current.simulatePhysics)
                    this._spawnWait += 0.0166666f;
                if (this.bing.value > 0)
                {
                    float num1 = Math.Max(this.spawnTime - this._spawnWait, 0f) + this.initialDelay;
                    float num2 = bing.value * Maths.IncFrameTimer();
                    float num3 = (num2 - num1) / num2;
                    if (this.beeps == 0 && (double)num3 > 0.0)
                    {
                        SFX.Play("singleBeep");
                        ++this.beeps;
                    }
                    if (this.beeps == 1 && (double)num3 > 0.333333343267441)
                    {
                        SFX.Play("singleBeep");
                        ++this.beeps;
                    }
                    if (this.beeps == 2 && (double)num3 > 0.666666686534882)
                    {
                        SFX.Play("singleBeep");
                        ++this.beeps;
                    }
                }
                if (Level.current.simulatePhysics && _spawnWait >= (double)this.spawnTime)
                {
                    if (initialDelay > 0.0)
                    {
                        this.initialDelay -= 0.0166666f;
                    }
                    else
                    {
                        if (this.bing.value > 0)
                            SFX.Play("bing");
                        this.beeps = 0;
                        this.Spawn();
                        this._startupDelay = 0f;
                        this.initialDelay = 0f;
                    }
                }
            }
            this.angleDegrees = -this.direction;
        }

        public void Pulse(int type, WireTileset wire)
        {
            this.wasPulse = true;
            this.Spawn();
            this.wasPulse = false;
            SFX.Play("click");
        }

        public override void Terminate()
        {
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("fireDirection", fireDirection);
            binaryClassChunk.AddProperty("firePower", firePower);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            this.fireDirection = node.GetProperty<float>("fireDirection");
            this.firePower = node.GetProperty<float>("firePower");
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = base.LegacySerialize();
            dxmlNode.Add(new DXMLNode("fireDirection", Change.ToString(fireDirection)));
            dxmlNode.Add(new DXMLNode("firePower", Change.ToString(firePower)));
            return dxmlNode;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            base.LegacyDeserialize(node);
            DXMLNode dxmlNode1 = node.Element("fireDirection");
            if (dxmlNode1 != null)
                this.fireDirection = Convert.ToSingle(dxmlNode1.Value);
            DXMLNode dxmlNode2 = node.Element("firePower");
            if (dxmlNode2 != null)
                this.firePower = Convert.ToSingle(dxmlNode2.Value);
            return true;
        }

        public override ContextMenu GetContextMenu()
        {
            EditorGroupMenu contextMenu = base.GetContextMenu() as EditorGroupMenu;
            contextMenu.AddItem(new ContextSlider("Angle", null, new FieldBinding(this, "fireDirection", max: 360f), 1f));
            contextMenu.AddItem(new ContextSlider("Power", null, new FieldBinding(this, "firePower", 1f, 20f)));
            return contextMenu;
        }

        public override void DrawHoverInfo()
        {
            string text = "EMPTY";
            if (this.contains != null)
                text = this.contains.Name;
            Graphics.DrawString(text, this.position + new Vec2((float)(-(double)Graphics.GetStringWidth(text) / 2.0), -16f), Color.White, (Depth)0.9f);
            if (!(this.contains != null))
                return;
            if (this._hoverThing == null || this._hoverThing.GetType() != this.contains)
                this._hoverThing = Editor.CreateThing(this.contains) as PhysicsObject;
            if (this._hoverThing == null)
                return;
            Vec2 vec2 = Maths.AngleToVec(Maths.DegToRad(this.direction)) * this.firePower;
            this._hoverThing.position = this.position + vec2.normalized * 8f;
            this._hoverThing.hSpeed = vec2.x;
            this._hoverThing.vSpeed = vec2.y;
            SFX.enabled = false;
            Vec2 position = this._hoverThing.position;
            for (int index = 0; index < 100; ++index)
            {
                this._hoverThing.UpdatePhysics();
                Graphics.DrawLine(position, this._hoverThing.position, Color.Red, 2f, (Depth)1f);
                position = this._hoverThing.position;
            }
            SFX.enabled = true;
        }

        public override void Draw()
        {
            this._sprite.frame = this.cannonColor.value;
            this.xscale = 1f;
            this.yscale = 1f;
            float val = this._spawnWait / (this.spawnTime + this._startupDelay);
            if (this.showClock.value)
            {
                float radians = (float)((this.flipHorizontal ? (double)val : -(double)val) * 6.28318548202515);
                if (this.flipHorizontal)
                    radians += 3.141593f;
                Graphics.DrawLine(this.Offset(new Vec2(0f, 0f)), this.Offset(Maths.AngleToVec(radians) * 3f), Color.Black, depth: (this.depth + 2));
                Vec2 vec2 = this.Offset(Maths.AngleToVec(radians) * 2f);
                this._arrowHead.angle = (float)((this.flipHorizontal ? (double)radians : -(double)radians) + (double)this.angle + 3.14159274101257 * (this.flipHorizontal ? -0.5 : 0.5));
                this._arrowHead.scale = new Vec2(0.5f, 0.5f);
                Graphics.Draw(this._arrowHead, vec2.x, vec2.y, this.depth + 2);
            }
            float num = Maths.Clamp(val, 0f, 1f);
            if ((double)num > 0.800000011920929 && !(Level.current is Editor))
            {
                this.xscale = (float)(1.0 - ((double)num - 0.800000011920929) * 2.0);
                this.yscale = (float)(1.0 + ((double)num - 0.800000011920929) * 4.0);
            }
            this.angleDegrees = -this.direction;
            base.Draw();
        }
    }
}
