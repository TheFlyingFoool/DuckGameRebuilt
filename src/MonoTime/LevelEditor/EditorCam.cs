// Decompiled with JetBrains decompiler
// Type: DuckGame.EditorCam
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class EditorCam : Camera
    {
        protected new Vec2 _zoomPoint;
        protected float _zoomInc;
        protected float _zoom = 1f;

        public new Vec2 zoomPoint
        {
            get => _zoomPoint;
            set
            {
                if (!(_zoomPoint != value))
                    return;
                _zoomPoint = value;
                _dirty = true;
            }
        }

        public float zoomInc
        {
            get => _zoomInc;
            set
            {
                if (_zoomInc == value)
                    return;
                _zoomInc = value;
                _dirty = true;
            }
        }

        public float zoom => _zoom;

        public override void Update()
        {
        }
    }
}
