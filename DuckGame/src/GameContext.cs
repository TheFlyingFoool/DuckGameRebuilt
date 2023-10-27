namespace DuckGame
{
    public class GameContext
    {
        public LayerCore layerCore;
        private LayerCore _oldLayerCore;
        public LevelCore levelCore;
        private LevelCore _oldLevelCore;

        public GameContext()
        {
            layerCore = new LayerCore();
            layerCore.InitializeLayers();
            levelCore = new LevelCore();
        }

        public void ApplyStates()
        {
            _oldLayerCore = Layer.core;
            Layer.core = layerCore;
            _oldLevelCore = Level.core;
            Level.core = levelCore;
        }

        public void RevertStates()
        {
            Layer.core = _oldLayerCore;
            Level.core = _oldLevelCore;
        }

        public void Update()
        {
            ApplyStates();
            Level.UpdateLevelChange();
            Level.UpdateCurrentLevel();
            RevertStates();
        }

        public void Draw(RenderTarget2D target = null, Camera c = null, Vec2 offset = default(Vec2))
        {
            ApplyStates();
            c.position += offset;
            if (c != null)
                Level.current.camera = c;
            RenderTarget2D t = null;
            if (target != null)
            {
                t = Graphics.GetRenderTarget();
                Graphics.SetRenderTarget(target);
            }
            Level.DrawCurrentLevel();
            if (target != null)
                Graphics.SetRenderTarget(t);
            RevertStates();
            c.position -= offset;
        }
    }
}
