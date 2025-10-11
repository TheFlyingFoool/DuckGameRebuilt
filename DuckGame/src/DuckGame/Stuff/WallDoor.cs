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
            _sprite = new SpriteMap("wallDoor", 21, 30);
            _sprite.AddAnimation("opening", 1f, false, 1, 2, 3, 4, 5, 6, 6, 6, 6, 6);
            _sprite.AddAnimation("closing", 1f, false, 5, 4, 3, 2, 1);
            _sprite.AddAnimation("open", 1f, false, 6);
            _sprite.AddAnimation("closed", 1f, false, new int[1]);
            _sprite.SetAnimation("closed");
            graphic = _sprite;
            center = new Vec2(10f, 22f);
            collisionSize = new Vec2(21f, 30f);
            collisionOffset = new Vec2(-10f, -20f);
            depth = -0.5f;
            _editorName = "Wall Door";
            _canFlip = false;

            //a lot of stuff is tied to the sprite animation so this shall never be graphic culled -Lucky
            shouldbegraphicculled = false;
        }

        public void AddDuck(Duck d)
        {
            d.autoExitDoor = true;
            d.autoExitDoorFrames = 5;
            _transportingDucks.Add(d);
            _sprite.SetAnimation("open");
            if (d.spriteImageIndex >= 4)
                return;
            SFX.Play("doorOpen", Rando.Float(0.8f, 0.9f), Rando.Float(-0.1f, 0.1f));
        }

        public override void Update()
        {
            foreach (Duck duck in Level.CheckRectAll<Duck>(topLeft, bottomRight))
            {
                if (duck.grounded && duck.inputProfile.Pressed(Triggers.Up) && !duck.enteringWalldoor && !duck.exitingWalldoor && !_transportingDucks.Contains(duck))
                {
                    _transportingDucks.Add(duck);
                    duck.wallDoorAI = new DuckAI(duck.inputProfile);
                    duck.autoExitDoorFrames = 0;
                    duck.enterDoorSpeed = duck.hSpeed;
                    if (duck.spriteImageIndex < 4)
                        SFX.Play("doorOpen", Rando.Float(0.8f, 0.9f), Rando.Float(-0.1f, 0.1f));
                    _sprite.SetAnimation("opening");
                }
            }
            if (_sprite.currentAnimation == "opening" && _sprite.finished)
                _sprite.SetAnimation("open");
            if (_sprite.currentAnimation == "closing" && _sprite.finished)
            {
                _sprite.SetAnimation("closed");
                SFX.Play("doorClose", Rando.Float(0.5f, 0.6f), Rando.Float(-0.1f, 0.1f));
            }
            if (_transportingDucks.Count == 0 && _sprite.currentAnimation != "closing" && _sprite.currentAnimation != "closed")
                _sprite.SetAnimation("closing");
            for (int index = 0; index < _transportingDucks.Count; ++index)
            {
                Duck transportingDuck = _transportingDucks[index];
                if (transportingDuck.wallDoorAI == null && !transportingDuck.autoExitDoor && !transportingDuck.exitingWalldoor)
                {
                    WallDoor wallDoor = null;
                    if (transportingDuck.inputProfile.Pressed(Triggers.Left) || transportingDuck.inputProfile.Down(Triggers.Left) && transportingDuck.autoExitDoorFrames > 5)
                        wallDoor = Level.CheckRay<WallDoor>(position, position + new Vec2(-10000f, 0f), this, out Vec2 _);
                    if (transportingDuck.inputProfile.Pressed(Triggers.Right) || transportingDuck.inputProfile.Down(Triggers.Right) && transportingDuck.autoExitDoorFrames > 5)
                        wallDoor = Level.CheckRay<WallDoor>(position, position + new Vec2(10000f, 0f), this, out Vec2 _);
                    if (transportingDuck.inputProfile.Pressed(Triggers.Up) || transportingDuck.inputProfile.Down(Triggers.Up) && transportingDuck.autoExitDoorFrames > 10)
                        wallDoor = Level.CheckRay<WallDoor>(position, position + new Vec2(0f, -10000f), this, out Vec2 _);
                    if (transportingDuck.inputProfile.Pressed(Triggers.Down) || transportingDuck.inputProfile.Down(Triggers.Down) && transportingDuck.autoExitDoorFrames > 5)
                        wallDoor = Level.CheckRay<WallDoor>(position, position + new Vec2(0f, 10000f), this, out Vec2 _);
                    if (wallDoor != null)
                    {
                        transportingDuck.enteringWalldoor = true;
                        transportingDuck.transportDoor = wallDoor;
                        transportingDuck.autoExitDoorFrames = 0;
                    }
                }
                if (Math.Abs(transportingDuck.x - x) < 3 && transportingDuck.wallDoorAI != null)
                {
                    transportingDuck.hSpeed *= 0.5f;
                    transportingDuck.moveLock = true;
                    transportingDuck.wallDoorAI = null;
                }
                else if (transportingDuck.wallDoorAI != null)
                {
                    if (transportingDuck.x > x + 2)
                        transportingDuck.wallDoorAI.Press(Triggers.Left);
                    if (transportingDuck.x < x - 2)
                        transportingDuck.wallDoorAI.Press(Triggers.Right);
                }
                if (transportingDuck.transportDoor != null)
                    --transportingDuck.autoExitDoorFrames;
                else if (transportingDuck.autoExitDoor && transportingDuck.autoExitDoorFrames > 0)
                    --transportingDuck.autoExitDoorFrames;
                else if (!transportingDuck.autoExitDoor)
                    ++transportingDuck.autoExitDoorFrames;
                if (transportingDuck.inputProfile.Pressed(Triggers.Jump) && !transportingDuck.autoExitDoor || transportingDuck.autoExitDoor && transportingDuck.autoExitDoorFrames == 0)
                {
                    transportingDuck.exitingWalldoor = true;
                    transportingDuck.autoExitDoor = false;
                }
                if (transportingDuck.transportDoor != null && transportingDuck.autoExitDoorFrames <= 0)
                {
                    transportingDuck.position = transportingDuck.transportDoor.position + new Vec2(0f, -6f);
                    transportingDuck.transportDoor.AddDuck(transportingDuck);
                    transportingDuck.transportDoor = null;
                    _transportingDucks.RemoveAt(index);
                    --index;
                }
                else if (transportingDuck.exitingWalldoor)
                {
                    transportingDuck.moveLock = false;
                    transportingDuck.enteringWalldoor = false;
                    transportingDuck.exitingWalldoor = false;
                    transportingDuck.wallDoorAI = null;
                    _transportingDucks.RemoveAt(index);
                    transportingDuck.autoExitDoor = false;
                    transportingDuck.transportDoor = null;
                    transportingDuck.hSpeed = transportingDuck.enterDoorSpeed;
                    --index;
                }
            }
            base.Update();
        }

        public override void Draw()
        {
            Graphics.DrawRect(topLeft, bottomRight, new Color(18, 25, 33), -0.6f);
            base.Draw();
        }
    }
}
