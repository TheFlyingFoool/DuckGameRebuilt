// Decompiled with JetBrains decompiler
// Type: DuckGame.WaterFlow
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.graphic = new SpriteMap("waterFlow", 16, 16);
            this.center = new Vec2(8f, 14f);
            this._collisionSize = new Vec2(16f, 4f);
            this._collisionOffset = new Vec2(-8f, -2f);
            this.layer = Layer.Blocks;
            this.depth = (Depth)0.3f;
            this.alpha = 0.8f;
            this.hugWalls = WallHug.Floor;
        }

        public Rectangle ProcessGroupRect(Rectangle rect)
        {
            if (this.processed)
                return rect;
            if (this.left < rect.x)
            {
                rect.width += (int)(rect.x - this.left);
                rect.x = (int)this.left;
            }
            if (this.right > rect.x + rect.width)
                rect.width += (int)(this.right - (rect.x + rect.width));
            this.processed = true;
            if (!this._wallLeft)
            {
                WaterFlow waterFlow1 = Level.CheckPoint<WaterFlow>(new Vec2(this.x - 16f, this.y));
                if (waterFlow1 != null && waterFlow1 != this && waterFlow1.flipHorizontal == this.flipHorizontal)
                {
                    rect = waterFlow1.ProcessGroupRect(rect);
                    this._extraWater.Add(waterFlow1);
                    foreach (WaterFlow waterFlow2 in waterFlow1._extraWater)
                        this._extraWater.Add(waterFlow2);
                }
            }
            if (!this._wallRight)
            {
                WaterFlow waterFlow3 = Level.CheckPoint<WaterFlow>(new Vec2(this.x + 16f, this.y));
                if (waterFlow3 != null && waterFlow3 != this && waterFlow3.flipHorizontal == this.flipHorizontal)
                {
                    rect = waterFlow3.ProcessGroupRect(rect);
                    this._extraWater.Add(waterFlow3);
                    foreach (WaterFlow waterFlow4 in waterFlow3._extraWater)
                        this._extraWater.Add(waterFlow4);
                }
            }
            return rect;
        }

        public override void Update()
        {
            if (!this._initialized)
            {
                this._initialized = true;
                if (Level.CheckPoint<Block>(new Vec2(this.x - 16f, this.y)) != null)
                    this._wallLeft = true;
                else if (Level.CheckPoint<Block>(new Vec2(this.x + 16f, this.y)) != null)
                    this._wallRight = true;
                if (!this.processed)
                {
                    Rectangle rectangle = this.ProcessGroupRect(this.rectangle);
                    if (this._extraWater.Count > 0)
                    {
                        this._extraWater.Remove(this);
                        foreach (WaterFlow waterFlow in this._extraWater)
                        {
                            Level.Remove(waterFlow);
                            waterFlow._extraWater.Clear();
                        }
                        this._collisionSize = new Vec2(rectangle.width, rectangle.height);
                        this._collisionOffset = new Vec2(rectangle.x - this.x, this._collisionOffset.y);
                    }
                }
            }
            bool flipHorizontal = this.flipHorizontal;
            this.flipHorizontal = false;
            IEnumerable<PhysicsObject> source = Level.CheckRectAll<PhysicsObject>(this.topLeft, this.bottomRight);
            foreach (PhysicsObject physicsObject in source)
            {
                if (flipHorizontal && physicsObject.hSpeed > -2.0)
                    physicsObject.hSpeed -= 0.3f;
                else if (!flipHorizontal && physicsObject.hSpeed < 2.0)
                    physicsObject.hSpeed += 0.3f;
                physicsObject.sleeping = false;
                physicsObject.frictionMult = 0.3f;
                this._held.Add(physicsObject);
            }
            List<PhysicsObject> physicsObjectList = new List<PhysicsObject>();
            foreach (PhysicsObject physicsObject in this._held)
            {
                if (!source.Contains<PhysicsObject>(physicsObject))
                {
                    physicsObjectList.Add(physicsObject);
                    physicsObject.frictionMult = 1f;
                }
            }
            foreach (PhysicsObject physicsObject in physicsObjectList)
                this._held.Remove(physicsObject);
            this.flipHorizontal = flipHorizontal;
            base.Update();
        }

        public override void Draw()
        {
            (this.graphic as SpriteMap).frame = (int)(Graphics.frame / 3.0 % 4.0);
            foreach (Thing thing in this._extraWater)
                (thing.graphic as SpriteMap).frame = (int)(Graphics.frame / 3.0 % 4.0);
            this.graphic.flipH = this.offDir <= 0;
            base.Draw();
            if (!this.flipHorizontal)
            {
                if (this._wallLeft)
                    Graphics.Draw(this.graphic, this.x - 4f, this.y, new Rectangle(this.graphic.w - 4, 0f, 4f, graphic.h));
                if (this._wallRight)
                    Graphics.Draw(this.graphic, this.x + 16f, this.y, new Rectangle(0f, 0f, 4f, graphic.h));
            }
            else
            {
                if (this._wallRight)
                    Graphics.Draw(this.graphic, this.x + 4f, this.y, new Rectangle(this.graphic.w - 4, 0f, 4f, graphic.h));
                if (this._wallLeft)
                    Graphics.Draw(this.graphic, this.x - 16f, this.y, new Rectangle(0f, 0f, 4f, graphic.h));
            }
            foreach (WaterFlow waterFlow in this._extraWater)
            {
                if (waterFlow != this && waterFlow._extraWater.Count == 0)
                    waterFlow.Draw();
            }
        }
    }
}
