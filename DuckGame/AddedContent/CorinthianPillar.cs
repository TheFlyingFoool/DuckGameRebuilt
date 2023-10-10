namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Rebuilt|Stuff", EditorItemType.Normal)]
    public class CorinthianPillar : Platform, IDontMove
    {
        public EditorProperty<int> height;
        public EditorProperty<int> style;
        private SpriteMap _pillar;
        private SpriteMap _pillarBase;
        public bool _stretchTop = false;
        public bool _updatedTops = false;
        override public void EditorPropertyChanged(object property)
        {
            UpdateSize();
        }
        public void UpdateSize()
        {
            _stretchTop = false;
            collisionOffset = new Vec2(-8, -height.value * 8);
            collisionSize = new Vec2(16, height.value * 8);

            if (Level.current != null && Level.CheckPoint<AutoBlock>(new Vec2(x, top - 4)) != null) _stretchTop = true;
        }
        public CorinthianPillar(float xpos, float ypos) : base(xpos, ypos)
        {
            height = new EditorProperty<int>(2, this, 2, 32, 1);
            style = new EditorProperty<int>(0, this, 0, 1, 1);

            graphic = _pillar = new SpriteMap("corinthianPillar", 16, 16);
            _pillarBase = new SpriteMap("corinthianPillarDeepBase", 16, 16);
            center = new Vec2(8, 8);
            //physicsMaterial = PhysicsMaterial.Metal;


            _editorIcon = new SpriteMap("corinthianPillar", 16, 16);

            hugWalls = WallHug.Floor;
            UpdateSize();

            thickness = 0;
            depth = -0.05f;

            editorTooltip = "From Corinthia of course!";
            editorOffset = new Vec2(0, 8);

            placementLayerOverride = Layer.Blocks;
        }

        public override void Update()
        {
            if (_updatedTops == false)
            {
                _updatedTops = true;
                UpdateSize();
            }
            base.Update();
        }

        override public void EditorObjectsChanged()
        {
            UpdateSize();
        }


        public override void Draw()
        {
            _pillar.frame = style.value == 0 ? 0 : 1;
            _pillar.depth = depth;
            _pillar.alpha = alpha;
            _pillar.scale = new Vec2(1, 1);

            _pillarBase.frame = style.value == 0 ? 0 : 1;
            _pillarBase.depth = depth;
            _pillarBase.alpha = alpha;
            _pillarBase.scale = new Vec2(1, 1);

            if (_stretchTop) Graphics.Draw(_pillar, x - 8, (y - (height.value * 8)) - 3, new Rectangle(0, 0, 16, 9));
            else Graphics.Draw(_pillar, x - 8, y - (height.value * 8), new Rectangle(0, 0, 16, 6));


            _pillar.depth = depth - 10;
            _pillar.scale = new Vec2(1, (((height.value - 1) * 8) / 5.0f));
            Graphics.Draw(_pillar, x - 8, (y - (height.value * 8)) + 6, new Rectangle(0, 6, 16, 5));


            _pillar.depth = depth;
            _pillar.scale = new Vec2(1, 1);
            Graphics.Draw(_pillar, x - 8, y - 3, new Rectangle(0, 16 - 5, 16, 5));
        }
    }
}
