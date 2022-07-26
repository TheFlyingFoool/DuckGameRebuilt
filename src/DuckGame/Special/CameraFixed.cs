// Decompiled with JetBrains decompiler
// Type: DuckGame.CameraFixed
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Arcade|Cameras", EditorItemType.ArcadeNew)]
    public class CameraFixed : CustomCamera
    {
        public EditorProperty<int> Size = new EditorProperty<int>(320, min: 60f, max: 1920f, increment: 1f);
        public EditorProperty<float> MoveX = new EditorProperty<float>(0.0f, min: -10f, max: 10f, increment: 0.25f);
        public EditorProperty<float> MoveY = new EditorProperty<float>(0.0f, min: -10f, max: 10f, increment: 0.25f);
        public EditorProperty<float> MoveDelay = new EditorProperty<float>(1f, max: 120f, increment: 0.25f);
        private bool moving;
        private int inc;
        private CameraMover curMover;

        public CameraFixed()
        {
            this._contextMenuFilter.Add("wide");
            this._editorName = "Camera Fixed";
            this.editorTooltip = "A fixed Camera that stays in one place.";
            this.Size._tooltip = "The size of the camera view (In pixels).";
            this.graphic = new Sprite("cameraIcon");
            this.collisionSize = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-4f, -4f);
            this._visibleInGame = false;
        }

        public override void Initialize()
        {
            this.wide.value = this.Size.value;
            base.Initialize();
        }

        public override void Update()
        {
            if (!(Level.current is GameLevel) || GameMode.started)
            {
                if ((double)this.MoveDelay.value > 0.0)
                {
                    this.MoveDelay.value -= Maths.IncFrameTimer();
                }
                else
                {
                    Level.current.camera.x += this.MoveX.value;
                    Level.current.camera.y += this.MoveY.value;
                    this.position = Level.current.camera.center;
                    if ((double)this.MoveX.value != 0.0 || (double)this.MoveY.value != 0.0)
                    {
                        CameraMover cameraMover = Level.CheckLine<CameraMover>(this.position, this.position + new Vec2(this.MoveX.value, this.MoveY.value));
                        if (cameraMover != null && cameraMover != this.curMover && ((double)(cameraMover.position - this.position).length < 0.5 || (double)this.MoveX.value != 0.0 && Math.Sign(cameraMover.position.x - this.position.x) != Math.Sign(this.MoveX.value) || (double)this.MoveY.value != 0.0 && Math.Sign(cameraMover.position.y - this.position.y) != Math.Sign(this.MoveY.value)))
                        {
                            this.position = cameraMover.position;
                            this.MoveX.value = cameraMover.SpeedX.value;
                            this.MoveY.value = cameraMover.SpeedY.value;
                            this.MoveDelay.value = cameraMover.MoveDelay.value;
                            this.curMover = cameraMover;
                        }
                    }
                }
            }
            base.Update();
        }

        public override void Draw()
        {
            this.wide.value = this.Size.value;
            base.Draw();
        }
    }
}
