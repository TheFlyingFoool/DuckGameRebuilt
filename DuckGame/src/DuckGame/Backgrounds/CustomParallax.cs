namespace DuckGame
{
    [EditorGroup("Background|Parallax|custom", EditorItemType.Custom)]
    public class CustomParallax : BackgroundUpdater
    {
        public bool didInit;

        public CustomParallax(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("backgroundIcons", 16, 16)
            {
                frame = 6
            };
            center = new Vec2(8f, 8f);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            depth = (Depth)0.9f;
            layer = Layer.Foreground;
            _visibleInGame = false;
            _editorName = "Custom Parallax";
        }

        public override void Initialize()
        {
            didInit = true;
            if (Level.current is Editor)
                return;
            backgroundColor = new Color(25, 38, 41);
            Level.current.backgroundColor = backgroundColor;
            CustomTileData data = Custom.GetData(0, CustomType.Parallax);
            if (data != null && data.texture != null)
            {
                _parallax = new ParallaxBackground(data.texture);
                for (int yPos = 0; yPos < 40; ++yPos)
                    _parallax.AddZone(yPos, 0f, 0f, true);
                Level.Add(_parallax);
            }
            else
            {
                _parallax = new ParallaxBackground("background/office", 0f, 0f, 3);
                Level.Add(_parallax);
            }
        }

        public override void Update() => base.Update();

        public override void Terminate() => Level.Remove(_parallax);

        public static string customParallax
        {
            get => Custom.data[CustomType.Parallax][0];
            set
            {
                Custom.data[CustomType.Parallax][0] = value;
                Custom.Clear(CustomType.Block, value);
            }
        }

        public override ContextMenu GetContextMenu()
        {
            EditorGroupMenu contextMenu = new EditorGroupMenu(null, true);
            contextMenu.AddItem(new ContextFile("style", null, new FieldBinding(this, "customParallax"), ContextFileType.Parallax));
            return contextMenu;
        }
    }
}
