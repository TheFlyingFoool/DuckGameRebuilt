// Decompiled with JetBrains decompiler
// Type: DuckGame.WaterFlow
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [EditorGroup("Details|Terrain")]
    public class WaterFlow : Thing
    {
        protected HashSet<WaterFlow> _extraWater = new HashSet<WaterFlow>();
        public static int waterFrame;
        public static int waterFrameInc;
        public static bool updatedWaterFrame;
        private new bool _initialized;
        private bool _wallLeft;
        private bool _wallRight;
        public bool processed;
        private HashSet<PhysicsObject> _held = new HashSet<PhysicsObject>();

        public WaterFlow(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("waterFlow", 16, 16);
            center = new Vec2(8f, 14f);
            _collisionSize = new Vec2(16f, 4f);
            _collisionOffset = new Vec2(-8f, -2f);
            layer = Layer.Blocks;
            depth = (Depth)0.3f;
            alpha = 0.8f;
            hugWalls = WallHug.Floor;
        }

        public Rectangle ProcessGroupRect(Rectangle rect)
        {
            if (processed)
                return rect;
            if (left < rect.x)
            {
                rect.width += (int)(rect.x - left);
                rect.x = (int)left;
            }
            if (right > rect.x + rect.width)
                rect.width += (int)(right - (rect.x + rect.width));
            processed = true;
            if (!_wallLeft)
            {
                WaterFlow waterFlow1 = Level.CheckPoint<WaterFlow>(new Vec2(x - 16f, y));
                if (waterFlow1 != null && waterFlow1 != this && waterFlow1.flipHorizontal == flipHorizontal)
                {
                    rect = waterFlow1.ProcessGroupRect(rect);
                    _extraWater.Add(waterFlow1);
                    foreach (WaterFlow waterFlow2 in waterFlow1._extraWater)
                        _extraWater.Add(waterFlow2);
                }
            }
            if (!_wallRight)
            {
                WaterFlow waterFlow3 = Level.CheckPoint<WaterFlow>(new Vec2(x + 16f, y));
                if (waterFlow3 != null && waterFlow3 != this && waterFlow3.flipHorizontal == flipHorizontal)
                {
                    rect = waterFlow3.ProcessGroupRect(rect);
                    _extraWater.Add(waterFlow3);
                    foreach (WaterFlow waterFlow4 in waterFlow3._extraWater)
                        _extraWater.Add(waterFlow4);
                }
            }
            return rect;
        }

        public override void Update()
        {
            if (!_initialized)
            {
                _initialized = true;
                if (Level.CheckPoint<Block>(new Vec2(x - 16f, y)) != null)
                    _wallLeft = true;
                else if (Level.CheckPoint<Block>(new Vec2(x + 16f, y)) != null)
                    _wallRight = true;
                if (!processed)
                {
                    Rectangle rectangle = ProcessGroupRect(this.rectangle);
                    if (_extraWater.Count > 0)
                    {
                        _extraWater.Remove(this);
                        foreach (WaterFlow waterFlow in _extraWater)
                        {
                            Level.Remove(waterFlow);
                            waterFlow._extraWater.Clear();
                        }
                        _collisionSize = new Vec2(rectangle.width, rectangle.height);
                        _collisionOffset = new Vec2(rectangle.x - x, _collisionOffset.y);
                    }
                }
            }
            bool flipHorizontal = this.flipHorizontal;
            this.flipHorizontal = false;
            IEnumerable<PhysicsObject> source = Level.CheckRectAll<PhysicsObject>(topLeft, bottomRight);
            foreach (PhysicsObject physicsObject in source)
            {
                if (flipHorizontal && physicsObject.hSpeed > -2.0)
                    physicsObject.hSpeed -= 0.3f;
                else if (!flipHorizontal && physicsObject.hSpeed < 2.0)
                    physicsObject.hSpeed += 0.3f;
                physicsObject.sleeping = false;
                physicsObject.frictionMult = 0.3f;
                _held.Add(physicsObject);
            }
            List<PhysicsObject> physicsObjectList = new List<PhysicsObject>();
            foreach (PhysicsObject physicsObject in _held)
            {
                if (!source.Contains(physicsObject))
                {
                    physicsObjectList.Add(physicsObject);
                    physicsObject.frictionMult = 1f;
                }
            }
            foreach (PhysicsObject physicsObject in physicsObjectList)
                _held.Remove(physicsObject);
            this.flipHorizontal = flipHorizontal;
            base.Update();
        }

        public override void Draw()
        {
            (graphic as SpriteMap).frame = (int)(Graphics.frame / 3.0 % 4.0);
            foreach (Thing thing in _extraWater)
                (thing.graphic as SpriteMap).frame = (int)(Graphics.frame / 3.0 % 4.0);
            graphic.flipH = offDir <= 0;
            base.Draw();
            if (!flipHorizontal)
            {
                if (_wallLeft)
                    Graphics.Draw(graphic, x - 4f, y, new Rectangle(graphic.w - 4, 0f, 4f, graphic.h));
                if (_wallRight)
                    Graphics.Draw(graphic, x + 16f, y, new Rectangle(0f, 0f, 4f, graphic.h));
            }
            else
            {
                if (_wallRight)
                    Graphics.Draw(graphic, x + 4f, y, new Rectangle(graphic.w - 4, 0f, 4f, graphic.h));
                if (_wallLeft)
                    Graphics.Draw(graphic, x - 16f, y, new Rectangle(0f, 0f, 4f, graphic.h));
            }
            foreach (WaterFlow waterFlow in _extraWater)
            {
                if (waterFlow != this && waterFlow._extraWater.Count == 0)
                    waterFlow.Draw();
            }
        }
    }
}
