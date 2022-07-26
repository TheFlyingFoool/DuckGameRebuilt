// Decompiled with JetBrains decompiler
// Type: DuckGame.WallDoor
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Stuff|Doors", EditorItemType.Debug)]
    public class WallDoor : Thing
    {
        protected SpriteMap _sprite;
        private List<Duck> _transportingDucks = new List<Duck>();

        public WallDoor(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("wallDoor", 21, 30);
            this._sprite.AddAnimation("opening", 1f, false, 1, 2, 3, 4, 5, 6, 6, 6, 6, 6);
            this._sprite.AddAnimation("closing", 1f, false, 5, 4, 3, 2, 1);
            this._sprite.AddAnimation("open", 1f, false, 6);
            this._sprite.AddAnimation("closed", 1f, false, new int[1]);
            this._sprite.SetAnimation("closed");
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(10f, 22f);
            this.collisionSize = new Vec2(21f, 30f);
            this.collisionOffset = new Vec2(-10f, -20f);
            this.depth = - 0.5f;
            this._editorName = "Wall Door";
            this._canFlip = false;
        }

        public void AddDuck(Duck d)
        {
            d.autoExitDoor = true;
            d.autoExitDoorFrames = 5;
            this._transportingDucks.Add(d);
            this._sprite.SetAnimation("open");
            if (d.spriteImageIndex >= (byte)4)
                return;
            SFX.Play("doorOpen", Rando.Float(0.8f, 0.9f), Rando.Float(-0.1f, 0.1f));
        }

        public override void Update()
        {
            foreach (Duck duck in Level.CheckRectAll<Duck>(this.topLeft, this.bottomRight))
            {
                if (duck.grounded && duck.inputProfile.Pressed("UP") && !duck.enteringWalldoor && !duck.exitingWalldoor && !this._transportingDucks.Contains(duck))
                {
                    this._transportingDucks.Add(duck);
                    duck.wallDoorAI = new DuckAI(duck.inputProfile);
                    duck.autoExitDoorFrames = 0;
                    duck.enterDoorSpeed = duck.hSpeed;
                    if (duck.spriteImageIndex < (byte)4)
                        SFX.Play("doorOpen", Rando.Float(0.8f, 0.9f), Rando.Float(-0.1f, 0.1f));
                    this._sprite.SetAnimation("opening");
                }
            }
            if (this._sprite.currentAnimation == "opening" && this._sprite.finished)
                this._sprite.SetAnimation("open");
            if (this._sprite.currentAnimation == "closing" && this._sprite.finished)
            {
                this._sprite.SetAnimation("closed");
                SFX.Play("doorClose", Rando.Float(0.5f, 0.6f), Rando.Float(-0.1f, 0.1f));
            }
            if (this._transportingDucks.Count == 0 && this._sprite.currentAnimation != "closing" && this._sprite.currentAnimation != "closed")
                this._sprite.SetAnimation("closing");
            for (int index = 0; index < this._transportingDucks.Count; ++index)
            {
                Duck transportingDuck = this._transportingDucks[index];
                if (transportingDuck.wallDoorAI == null && !transportingDuck.autoExitDoor && !transportingDuck.exitingWalldoor)
                {
                    WallDoor wallDoor = (WallDoor)null;
                    if (transportingDuck.inputProfile.Pressed("LEFT") || transportingDuck.inputProfile.Down("LEFT") && transportingDuck.autoExitDoorFrames > 5)
                        wallDoor = Level.CheckRay<WallDoor>(this.position, this.position + new Vec2(-10000f, 0.0f), (Thing)this, out Vec2 _);
                    if (transportingDuck.inputProfile.Pressed("RIGHT") || transportingDuck.inputProfile.Down("RIGHT") && transportingDuck.autoExitDoorFrames > 5)
                        wallDoor = Level.CheckRay<WallDoor>(this.position, this.position + new Vec2(10000f, 0.0f), (Thing)this, out Vec2 _);
                    if (transportingDuck.inputProfile.Pressed("UP") || transportingDuck.inputProfile.Down("UP") && transportingDuck.autoExitDoorFrames > 10)
                        wallDoor = Level.CheckRay<WallDoor>(this.position, this.position + new Vec2(0.0f, -10000f), (Thing)this, out Vec2 _);
                    if (transportingDuck.inputProfile.Pressed("DOWN") || transportingDuck.inputProfile.Down("DOWN") && transportingDuck.autoExitDoorFrames > 5)
                        wallDoor = Level.CheckRay<WallDoor>(this.position, this.position + new Vec2(0.0f, 10000f), (Thing)this, out Vec2 _);
                    if (wallDoor != null)
                    {
                        transportingDuck.enteringWalldoor = true;
                        transportingDuck.transportDoor = wallDoor;
                        transportingDuck.autoExitDoorFrames = 0;
                    }
                }
                if ((double)Math.Abs(transportingDuck.x - this.x) < 3.0 && transportingDuck.wallDoorAI != null)
                {
                    transportingDuck.hSpeed *= 0.5f;
                    transportingDuck.moveLock = true;
                    transportingDuck.wallDoorAI = (DuckAI)null;
                }
                else if (transportingDuck.wallDoorAI != null)
                {
                    if ((double)transportingDuck.x > (double)this.x + 2.0)
                        transportingDuck.wallDoorAI.Press("LEFT");
                    if ((double)transportingDuck.x < (double)this.x - 2.0)
                        transportingDuck.wallDoorAI.Press("RIGHT");
                }
                if (transportingDuck.transportDoor != null)
                    --transportingDuck.autoExitDoorFrames;
                else if (transportingDuck.autoExitDoor && transportingDuck.autoExitDoorFrames > 0)
                    --transportingDuck.autoExitDoorFrames;
                else if (!transportingDuck.autoExitDoor)
                    ++transportingDuck.autoExitDoorFrames;
                if (transportingDuck.inputProfile.Pressed("JUMP") && !transportingDuck.autoExitDoor || transportingDuck.autoExitDoor && transportingDuck.autoExitDoorFrames == 0)
                {
                    transportingDuck.exitingWalldoor = true;
                    transportingDuck.autoExitDoor = false;
                }
                if (transportingDuck.transportDoor != null && transportingDuck.autoExitDoorFrames <= 0)
                {
                    transportingDuck.position = transportingDuck.transportDoor.position + new Vec2(0.0f, -6f);
                    transportingDuck.transportDoor.AddDuck(transportingDuck);
                    transportingDuck.transportDoor = (WallDoor)null;
                    this._transportingDucks.RemoveAt(index);
                    --index;
                }
                else if (transportingDuck.exitingWalldoor)
                {
                    transportingDuck.moveLock = false;
                    transportingDuck.enteringWalldoor = false;
                    transportingDuck.exitingWalldoor = false;
                    transportingDuck.wallDoorAI = (DuckAI)null;
                    this._transportingDucks.RemoveAt(index);
                    transportingDuck.autoExitDoor = false;
                    transportingDuck.transportDoor = (WallDoor)null;
                    transportingDuck.hSpeed = transportingDuck.enterDoorSpeed;
                    --index;
                }
            }
            base.Update();
        }

        public override void Draw()
        {
            Graphics.DrawRect(this.topLeft, this.bottomRight, new Color(18, 25, 33), - 0.6f);
            base.Draw();
        }
    }
}
