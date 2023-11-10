using System.Collections.Generic;
using System.Security.Principal;

namespace DuckGame
{
    [EditorGroup("Details|Lights", EditorItemType.Normal)]
    [BaggedProperty("isInDemo", true)]
    public class HangingCityLight : Thing
    {
        //private SpriteThing _shade;
        private List<LightOccluder> _occluders = new List<LightOccluder>();

        public HangingCityLight(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("hangingCityLight");
            center = new Vec2(8f, 5f);
            _collisionSize = new Vec2(8f, 8f);
            _collisionOffset = new Vec2(-4f, -5f);
            depth = (Depth)0.9f;
            hugWalls = WallHug.Ceiling;
            layer = Layer.Game;
            editorCycleType = typeof(Lamp);
            shouldbeinupdateloop = DGRSettings.AmbientParticles;
        }
        public override void Update()
        {
            if (tim > 0)
            {
                pl._range = Rando.Float(180f);
                if (tim <= 1)
                {
                    pl._range = 180;
                }
                pl.forceRefresh = true;
                tim--; 
            }
            else
            {
                if (Rando.Int(300) == 0)
                {
                    tim = Rando.Int(10);
                }
            }
        }
        public int tim;
        public PointLight pl;
        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            Vec2 vec2 = new Vec2(x, y);
            _occluders.Add(new LightOccluder(vec2 + new Vec2(-8f, 5f), vec2 + new Vec2(1f, -4f), new Color(0.4f, 0.4f, 0.4f)));
            _occluders.Add(new LightOccluder(vec2 + new Vec2(-1f, -4f), vec2 + new Vec2(8f, 5f), new Color(0.4f, 0.4f, 0.4f)));
            pl = new PointLight(vec2.x, vec2.y, new Color(247, 198, 120), 180f, _occluders);
            Level.Add(pl);
        }
    }
}
