namespace DuckGame
{
    public class HatPreviewLevel : Level
    {
        public HatPreviewLevel()
        {
            backgroundColor = Color.SlateGray;
        }

        public override void Initialize()
        {
            camera.position = camera.size / -2;
            
            Vec2 rectPos = camera.position + camera.size / 2;
            Rectangle bounds = new(rectPos - new Vec2(60, 24), rectPos + new Vec2(60, 24));
            
            Add(new Block(bounds.x, bounds.y - 16, bounds.width, 16));
            Add(new Block(bounds.x + bounds.width, bounds.y, 16, bounds.height));
            Add(new Block(bounds.x - 16, bounds.y, 16, bounds.height));
            Add(new Block(bounds.x, bounds.y + bounds.height, bounds.width, 16));
            
            Add(new Duck(bounds.Center.x, bounds.Bottom, Profiles.DefaultPlayer1));
            
            base.Initialize();
        }

        public override void Update()
        {
            foreach (Duck d in things[typeof(Duck)])
            {
                // d.x = 0;
            }
            
            base.Update();
        }
    }
}