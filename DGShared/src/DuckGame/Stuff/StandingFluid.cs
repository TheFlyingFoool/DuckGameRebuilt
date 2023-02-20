// Decompiled with JetBrains decompiler
// Type: DuckGame.StandingFluid
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Stuff")]
    public class StandingFluid : Thing
    {
        public EditorProperty<int> deep = new EditorProperty<int>(1, min: 1f, max: 100f, increment: 1f);
        public EditorProperty<int> fluidType = new EditorProperty<int>(0, max: 2f, increment: 1f);
        private Vec2 _prevPos = Vec2.Zero;
        private Vec2 _leftSide;
        private Vec2 _rightSide;
        private float _floor;
        private bool _isValid;
        private bool _filled;
        private int w8;

        public StandingFluid(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            _editorIcon = new Sprite("standingFluidIcon");
            _editorName = "Liquid";
            editorTooltip = "Place a liquid near the floor in a contained space and you've got yourself a pool party.";
            hugWalls = WallHug.Floor;
        }

        public override void Initialize() => base.Initialize();

        private FluidData GetFluidType()
        {
            switch ((int)fluidType)
            {
                case 0:
                    return Fluid.Water;
                case 1:
                    return Fluid.Gas;
                case 2:
                    return Fluid.Lava;
                default:
                    return Fluid.Water;
            }
        }

        public override void Update()
        {
            ++w8;
            if (!_filled && w8 > 2)
            {
                _filled = true;
                Block b = Level.CheckRay<Block>(new Vec2(x, y), new Vec2(x, y + 64f));
                if (b != null)
                {
                    FluidPuddle fluidPuddle = new FluidPuddle(x, b.top, b);
                    Level.Add(fluidPuddle);
                    float num = 0f;
                    while (fluidPuddle.CalculateDepth() < (int)deep * 8)
                    {
                        FluidData fluidType = GetFluidType();
                        fluidType.amount = 0.5f;
                        fluidPuddle.Feed(fluidType);
                        float depth = fluidPuddle.CalculateDepth();
                        if (Math.Abs(num - depth) < 1.0 / 1000.0)
                        {
                            Level.Remove(this);
                            break;
                        }
                        num = depth;
                    }
                    fluidPuddle.Update();
                    fluidPuddle.PrepareFloaters();
                }
            }
            base.Update();
        }

        public override void Draw()
        {
            if (Level.current is Editor)
            {
                Graphics.Draw(_editorIcon, x - 8f, y - 8f);
                if (_prevPos != position)
                {
                    _isValid = false;
                    _prevPos = position;
                    Vec2 hitPos1;
                    Vec2 hitPos2;
                    Vec2 hitPos3;
                    if (Level.CheckRay<Block>(position, position - new Vec2(1000f, 0f), out hitPos1) != null && Level.CheckRay<Block>(position, position + new Vec2(1000f, 0f), out hitPos2) != null && Level.CheckRay<Block>(position, position + new Vec2(0f, 64f), out hitPos3) != null)
                    {
                        _floor = hitPos3.y;
                        _leftSide = hitPos1;
                        _rightSide = hitPos2;
                        _isValid = true;
                    }
                }
                if (_isValid)
                    Graphics.DrawRect(new Vec2(_leftSide.x, _floor - (int)deep * 8), new Vec2(_rightSide.x, _floor), new Color(GetFluidType().color) * 0.5f, (Depth)0.9f);
            }
            base.Draw();
        }
    }
}
