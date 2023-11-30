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
