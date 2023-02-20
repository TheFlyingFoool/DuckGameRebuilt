// Decompiled with JetBrains decompiler
// Type: DuckGame.CameraFixed
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Arcade|Cameras", EditorItemType.ArcadeNew)]
    public class CameraFixed : CustomCamera
    {
        public EditorProperty<int> Size = new EditorProperty<int>(320, min: 60f, max: 1920f, increment: 1f); // what
        public EditorProperty<float> MoveX = new EditorProperty<float>(0f, min: -10f, max: 10f, increment: 0.25f);
        public EditorProperty<float> MoveY = new EditorProperty<float>(0f, min: -10f, max: 10f, increment: 0.25f);
        public EditorProperty<float> MoveDelay = new EditorProperty<float>(1f, max: 120f, increment: 0.25f);
        //private bool moving;
        //private int inc;
        private CameraMover curMover;

        public CameraFixed()
        {
            _contextMenuFilter.Add("wide");
            _editorName = "Camera Fixed";
            editorTooltip = "A fixed Camera that stays in one place.";
            Size._tooltip = "The size of the camera view (In pixels).";
            graphic = new Sprite("cameraIcon");
            collisionSize = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-4f, -4f);
            _visibleInGame = false;
        }

        public override void Initialize()
        {
            wide.value = Size.value;
            base.Initialize();
        }

        public override void Update()
        {
            if (!(Level.current is GameLevel) || GameMode.started)
            {
                if (MoveDelay.value > 0f)
                {
                    MoveDelay.value -= Maths.IncFrameTimer();
                }
                else
                {
                    Level.current.camera.x += MoveX.value;
                    Level.current.camera.y += MoveY.value;
                    position = Level.current.camera.center;
                    if (MoveX.value != 0f || MoveY.value != 0f)
                    {
                        CameraMover cameraMover = Level.CheckLine<CameraMover>(position, position + new Vec2(MoveX.value, MoveY.value));
                        if (cameraMover != null && cameraMover != curMover && ((cameraMover.position - position).length < 0.5f || MoveX.value != 0f && Math.Sign(cameraMover.position.x - position.x) != Math.Sign(MoveX.value) || MoveY.value != 0f && Math.Sign(cameraMover.position.y - position.y) != Math.Sign(MoveY.value)))
                        {
                            position = cameraMover.position;
                            MoveX.value = cameraMover.SpeedX.value;
                            MoveY.value = cameraMover.SpeedY.value;
                            MoveDelay.value = cameraMover.MoveDelay.value;
                            curMover = cameraMover;
                        }
                    }
                }
            }
            base.Update();
        }

        public override void Draw()
        {
            wide.value = Size.value;
            base.Draw();
        }
    }
}
