using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DuckGame
{
    [EditorGroup("Details|Pyramid", EditorItemType.Pyramid)]
    [BaggedProperty("isInDemo", true)]
    public class PyramidBLight : Thing
    {
        //private SpriteThing _shade;
        private List<LightOccluder> _occluders = new List<LightOccluder>();
        private SpriteMap _sprite;

        public PyramidBLight(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("pyramidBackgroundLight", 14, 12);
            _sprite.AddAnimation("go", 0.2f, true, 0, 1, 2, 3, 4);
            _sprite.SetAnimation("go");
            graphic = _sprite;
            center = new Vec2(7f, 8f);
            _collisionSize = new Vec2(8f, 8f);
            _collisionOffset = new Vec2(-4f, -4f);
            depth = -0.9f;
            alpha = 0.7f;
            layer = Layer.Game;
            placementLayerOverride = Layer.Blocks;
            editorCycleType = typeof(PyramidLightRoof);
            shouldbeinupdateloop = DGRSettings.AmbientParticles;
        }
        public float timer;
        public override void Update()
        {
            timer += 0.02f * DGRSettings.ActualParticleMultiplier;
            if (timer >= 1)
            {
                timer = Rando.Float(0.1f, 0.2f);
                Level.Add(new Ember(x + Rando.Float(-4, 4), y - Rando.Float(3.5f, 6)));
            }
        }
        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            Level.Add(new PointLight(x, y - 1f, PyramidWallLight.lightColor, 120f, strangeFalloff: true));
        }
    }
}
