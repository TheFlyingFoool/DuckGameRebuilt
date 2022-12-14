// Decompiled with JetBrains decompiler
// Type: DuckGame.SpawnCannon
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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

        public float direction => fireDirection + (flipHorizontal ? 180f : 0f);

        public override void EditorPropertyChanged(object property)
        {
            if (showClock.value)
                _sprite = new SpriteMap("cannonTimer", 18, 18);
            else
                _sprite = new SpriteMap("cannon", 18, 18);
            graphic = _sprite;
        }

        public SpawnCannon(float xpos, float ypos, Type c = null)
          : base(xpos, ypos)
        {
            bing = new EditorProperty<int>(0, this, max: 240f, increment: 1f, minSpecial: "none")
            {
                _tooltip = "If set, this cannon will BING this many frames before it activates."
            };
            showClock = new EditorProperty<bool>(false, this);
            cannonColor = new EditorProperty<int>(0, this, max: 3f, increment: 1f);
            _arrowHead = new Sprite("arrowHead", new Vec2(3.5f, 8f));
            _sprite = new SpriteMap("cannon", 18, 18);
            graphic = _sprite;
            center = new Vec2(7f, 9f);
            collisionSize = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-6f, -6f);
            depth = (Depth)0.8f;
            contains = c;
            hugWalls = WallHug.None;
            _placementCost += 4;
            editorTooltip = "Shoots the specified item in the specified direction after the specified delay.";
            sequence = new SequenceItem(this)
            {
                type = SequenceItemType.Activator
            };
        }

        public override void OnSequenceActivate()
        {
            SFX.Play("basketball", 0.8f, Rando.Float(0.2f, 0.4f));
            scale = new Vec2(2f, 2f);
            _running = true;
        }

        public override void Initialize()
        {
            if (spawnOnStart)
                _spawnWait = spawnTime;
            _startupDelay = initialDelay;
        }

        public void Spawn()
        {
            if (!Network.isServer && !wasPulse)
            {
                _spawnWait = 0f;
                ++_numSpawned;
            }
            else
            {
                if (sequence != null)
                {
                    if (sequence.waitTillOrder)
                        _running = false;
                    sequence.Finished();
                }
                xscale = 2f;
                yscale = 0.5f;
                if (!initializedWired)
                {
                    WireTileset wireTileset = Level.current.NearestThing<WireTileset>(position);
                    if (wireTileset != null && (wireTileset.position - position).length < 1.0)
                        wired = true;
                    initializedWired = true;
                }
                if (!wasPulse && wired)
                    return;
                if (randomSpawn && keepRandom)
                {
                    List<Type> physicsObjects = ItemBox.GetPhysicsObjects(Editor.Placeables);
                    contains = physicsObjects[Rando.Int(physicsObjects.Count - 1)];
                }
                else if (possible.Count > 0)
                {
                    Type type = MysteryGun.PickType(chanceGroup, possible);
                    if (type != null)
                        contains = type;
                }
                _spawnWait = 0f;
                ++_numSpawned;
                if (contains == null || !(Editor.CreateThing(contains) is PhysicsObject thing))
                    return;
                Vec2 vec2 = Maths.AngleToVec(Maths.DegToRad(direction)) * firePower;
                thing.position = position + vec2.normalized * 8f;
                thing.hSpeed = vec2.x;
                thing.vSpeed = vec2.y;
                Level.Add(thing);
                thing.Ejected(this);
                if (DGRSettings.S_ParticleMultiplier != 0)
                {
                    Level.Add(SmallSmoke.New(thing.x, thing.y));
                    Level.Add(SmallSmoke.New(thing.x, thing.y));
                }
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
            scale = Lerp.Vec2Smooth(scale, Vec2.One, 0.2f);
            if ((sequence == null || !sequence.waitTillOrder || _running) && (_numSpawned < spawnNum || spawnNum == -1))
            {
                if (Level.current.simulatePhysics)
                    _spawnWait += 0.0166666f;
                if (bing.value > 0)
                {
                    float num1 = Math.Max(spawnTime - _spawnWait, 0f) + initialDelay;
                    float num2 = bing.value * Maths.IncFrameTimer();
                    float num3 = (num2 - num1) / num2;
                    if (beeps == 0 && num3 > 0.0)
                    {
                        SFX.Play("singleBeep");
                        ++beeps;
                    }
                    if (beeps == 1 && num3 > 0.33333334f)
                    {
                        SFX.Play("singleBeep");
                        ++beeps;
                    }
                    if (beeps == 2 && num3 > 0.6666667f)
                    {
                        SFX.Play("singleBeep");
                        ++beeps;
                    }
                }
                if (Level.current.simulatePhysics && _spawnWait >= spawnTime)
                {
                    if (initialDelay > 0.0)
                    {
                        initialDelay -= 0.0166666f;
                    }
                    else
                    {
                        if (bing.value > 0)
                            SFX.Play("bing");
                        beeps = 0;
                        Spawn();
                        _startupDelay = 0f;
                        initialDelay = 0f;
                    }
                }
            }
            angleDegrees = -direction;
        }

        public void Pulse(int type, WireTileset wire)
        {
            wasPulse = true;
            Spawn();
            wasPulse = false;
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
            fireDirection = node.GetProperty<float>("fireDirection");
            firePower = node.GetProperty<float>("firePower");
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
                fireDirection = Convert.ToSingle(dxmlNode1.Value);
            DXMLNode dxmlNode2 = node.Element("firePower");
            if (dxmlNode2 != null)
                firePower = Convert.ToSingle(dxmlNode2.Value);
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
            if (contains != null)
                text = contains.Name;
            Graphics.DrawString(text, this.position + new Vec2((float)(-Graphics.GetStringWidth(text) / 2.0), -16f), Color.White, (Depth)0.9f);
            if (!(contains != null))
                return;
            if (_hoverThing == null || _hoverThing.GetType() != contains)
                _hoverThing = Editor.CreateThing(contains) as PhysicsObject;
            if (_hoverThing == null)
                return;
            Vec2 vec2 = Maths.AngleToVec(Maths.DegToRad(direction)) * firePower;
            _hoverThing.position = this.position + vec2.normalized * 8f;
            _hoverThing.hSpeed = vec2.x;
            _hoverThing.vSpeed = vec2.y;
            SFX.enabled = false;
            Vec2 position = _hoverThing.position;
            for (int index = 0; index < 100; ++index)
            {
                _hoverThing.UpdatePhysics();
                Graphics.DrawLine(position, _hoverThing.position, Color.Red, 2f, (Depth)1f);
                position = _hoverThing.position;
            }
            SFX.enabled = true;
        }

        public override void Draw()
        {
            _sprite.frame = cannonColor.value;
            xscale = 1f;
            yscale = 1f;
            float val = _spawnWait / (spawnTime + _startupDelay);
            if (showClock.value)
            {
                float radians = (float)((flipHorizontal ? val : -val) * 6.2831855f);
                if (flipHorizontal)
                    radians += 3.141593f;
                Graphics.DrawLine(Offset(new Vec2(0f, 0f)), Offset(Maths.AngleToVec(radians) * 3f), Color.Black, depth: (depth + 2));
                Vec2 vec2 = Offset(Maths.AngleToVec(radians) * 2f);
                _arrowHead.angle = (float)((flipHorizontal ? radians : -radians) + angle + 3.1415927f * (flipHorizontal ? -0.5 : 0.5));
                _arrowHead.scale = new Vec2(0.5f, 0.5f);
                Graphics.Draw(_arrowHead, vec2.x, vec2.y, depth + 2);
            }
            float num = Maths.Clamp(val, 0f, 1f);
            if (num > 0.8f && !(Level.current is Editor))
            {
                xscale = (float)(1.0 - (num - 0.8f) * 2.0);
                yscale = (float)(1.0 + (num - 0.8f) * 4.0);
            }
            angleDegrees = -direction;
            base.Draw();
        }
    }
}
