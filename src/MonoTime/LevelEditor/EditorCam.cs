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
            get => this._zoomPoint;
            set
            {
                if (!(this._zoomPoint != value))
                    return;
                this._zoomPoint = value;
                this._dirty = true;
            }
        }

        public float zoomInc
        {
            get => this._zoomInc;
            set
            {
                if ((double)this._zoomInc == (double)value)
                    return;
                this._zoomInc = value;
                this._dirty = true;
            }
        }

        public float zoom => this._zoom;

        public override void Update()
        {
        }
    }
}
