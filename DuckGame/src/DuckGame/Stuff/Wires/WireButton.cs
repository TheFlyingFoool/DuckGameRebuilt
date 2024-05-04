using System;
using System.Linq;

namespace DuckGame
{
    [EditorGroup("Stuff|Wires")]
    [BaggedProperty("isInDemo", true)]
    public class WireButton : Block, IWirePeripheral
    {
        public EditorProperty<bool> offSignal = new EditorProperty<bool>(false);
        public EditorProperty<float> holdTime = new EditorProperty<float>(0f);
        public EditorProperty<bool> releaseOnly = new EditorProperty<bool>(false);
        public EditorProperty<bool> invert = new EditorProperty<bool>(false);
        public EditorProperty<int> orientation = new EditorProperty<int>(0, max: 3f, increment: 1f);
        private WireButtonTop _top;
        private SpriteMap _sprite;
        private bool _initializedFrame;
        private float releaseHold;
        private PhysicsObject prevO;

        public WireButton(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("wireButton", 16, 19);
            graphic = _sprite;
            center = new Vec2(8f, 11f);
            collisionOffset = new Vec2(-8f, -8f);
            collisionSize = new Vec2(16f, 16f);
            depth = -0.5f;
            _editorName = "Wire Button";
            editorTooltip = "Stepping on a Button triggers the behavior of connected objects.";
            offSignal.name = "Hold Signal";
            offSignal._tooltip = "If true, the button continuously send a signal through the wire while pressed.";
            holdTime.name = "Hold Time";
            holdTime._tooltip = "How long the signal will be held after releasing the button.";
            releaseOnly.name = "Release";
            releaseOnly._tooltip = "If true, the button will send a signal only when released.";
            invert._tooltip = "If true, the button will send signals as long as it's not pressed.";
            thickness = 4f;
            physicsMaterial = PhysicsMaterial.Metal;
            layer = Layer.Foreground;
        }

        public override void TabRotate()
        {
            orientation = (EditorProperty<int>)((int)orientation + 1);
            if ((int)orientation <= 3)
                return;
            orientation = (EditorProperty<int>)0;
        }

        public override Type TabRotate(bool control)
        {
            if (control)
                editorCycleType = typeof(WireFlipper);
            else
                TabRotate();
            return editorCycleType;
        }

        public override void Initialize()
        {
            if (flipHorizontal)
            {
                if ((orientation.value & 1) == 1) // If odd, flip the bit. 1 -> 3, 3 -> 1
                    orientation.value ^= 2;
            }
            angleDegrees = orientation.value * 90f;
            if (!(Level.current is Editor))
            {
                if (orientation.value == 0)
                    _top = new WireButtonTop(x, y - 9f, this, orientation.value);
                else if (orientation.value == 1)
                    _top = new WireButtonTop(x + 9f, y, this, orientation.value);
                else if (orientation.value == 2)
                    _top = new WireButtonTop(x, y + 9f, this, orientation.value);
                else if (orientation.value == 3)
                    _top = new WireButtonTop(x - 9f, y, this, orientation.value);
                Level.Add(_top);
            }
            base.Initialize();
        }

        public override void Terminate()
        {
            Level.Remove(_top);
            base.Terminate();
        }

        public void Pulse(int type, WireTileset wire)
        {
        }

        public void ButtonPressed(PhysicsObject t)
        {
            if (_sprite.frame == 0)
            {
                SFX.Play("click");
                _sprite.frame = 1;
                if (invert.value)
                {
                    if (!releaseOnly.value && t.isServerForObject)
                        Level.CheckRect<WireTileset>(topLeft + new Vec2(2f, 2f), bottomRight + new Vec2(-2f, -2f))?.Emit(type: (offSignal.value ? 2 : 3));
                }
                else if (!releaseOnly.value && t.isServerForObject)
                    Level.CheckRect<WireTileset>(topLeft + new Vec2(2f, 2f), bottomRight + new Vec2(-2f, -2f))?.Emit(type: (offSignal.value ? 1 : 0));
            }
            prevO = t;
        }

        public override void Update()
        {
            if (!_initializedFrame)
            {
                if (Level.CheckRectAll<PhysicsObject>(_top.topLeft, _top.bottomRight).FirstOrDefault(x => !(x is TeamHat)) != null)
                    _sprite.frame = 1;
                _initializedFrame = true;
            }
            if (invert.value)
            {
                if (_sprite.frame == 0)
                    Level.CheckRect<WireTileset>(topLeft + new Vec2(2f, 2f), bottomRight + new Vec2(-2f, -2f))?.Emit(type: 1);
                if (_sprite.frame == 1)
                {
                    PhysicsObject physicsObject = Level.CheckRectAll<PhysicsObject>(_top.topLeft, _top.bottomRight).FirstOrDefault(x => !(x is TeamHat));
                    if (physicsObject == null)
                    {
                        SFX.Play("click");
                        _sprite.frame = 0;
                    }
                    prevO = physicsObject;
                }
            }
            else if (_sprite.frame == 1)
            {
                PhysicsObject physicsObject = Level.CheckRectAll<PhysicsObject>(_top.topLeft, _top.bottomRight).FirstOrDefault(x => !(x is TeamHat));
                if (physicsObject == null)
                {
                    releaseHold += Maths.IncFrameTimer();
                    if (releaseHold > holdTime.value)
                    {
                        SFX.Play("click");
                        _sprite.frame = 0;
                        if ((offSignal.value || releaseOnly.value) && (prevO == null || prevO.isServerForObject))
                            Level.CheckRect<WireTileset>(topLeft + new Vec2(2f, 2f), bottomRight + new Vec2(-2f, -2f))?.Emit(type: (releaseOnly.value ? 0 : 2));
                    }
                }
                prevO = physicsObject;
            }
            else
                releaseHold = 0f;
            base.Update();
        }

        public override void Draw()
        {
            if (Level.current is Editor)
            {
                angleDegrees = orientation.value * 90f;
                if (flipHorizontal && (orientation.value & 1) != 0)  // make consistent with actual outcome on play
                    angleDegrees -= 180f;
            }
            else
                angleDegrees = orientation.value * 90f;
            base.Draw();
        }
    }
}