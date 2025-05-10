using System;

namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Details|Weather")]
    public class RainTile : Thing
    {
        public EditorProperty<float> thunder = new EditorProperty<float>(0, null, 0, 16, 0.1f);
        public EditorProperty<bool> dark = new EditorProperty<bool>(false);
        public EditorProperty<float> strength = new EditorProperty<float>(1, null, 0, 2, 0.01f);
        public RainTile(float xpos, float ypos) : base(xpos, ypos)
        {
            graphic = new Sprite("rain");
            center = new Vec2(8);
            collisionSize = new Vec2(16);
            _collisionOffset = new Vec2(-8);
            _visibleInGame = false;
            _editorName = "Rain";
            maxPlaceable = 1;
            editorTooltip = "Adds some nice rain to the level. You can also summon zeus.";
        }
        public float rainTimer;
        public float rainDarken;
        public LoopingSound rainSound;
        public override void Initialize()
        {
            if (Level.current is not Editor)
            {
                rainSound = new LoopingSound("sizzle", 1, -3)
                {
                    volume = 0.2f
                };
                rainDarken = dark ? 0.8f : 1;
            }
            base.Initialize();
        }
        public override void Update()
        {
            if (rainSound != null)
            {
                if (dark.value) rainSound.volume = Lerp.Float(rainSound.volume, 0.5f, 0.01f);
                else rainSound.volume = Lerp.Float(rainSound.volume, 0.2f, 0.01f);

                rainSound.pitch = Rando.Float(-3f, -3.1f);

                if (rainSound._effect != null && rainSound._effect._instance != null && rainSound._effect._instance.Platform_GetProgress() > 0.5f) rainSound._effect._instance._position = 0;
            }
            if (thunder > 0 && Rando.Int((int)(4000 / thunder)) == 0)
            {
                rainDarken = 1.2f;
                Level.Add(new BGLightning(Rando.Float(-30, 270), 0));
                SFX.Play("balloonPop", 1, Rando.Float(-3, -4));
            }
            rainDarken = Lerp.Float(rainDarken, dark?0.8f:1, 0.005f);

            rainTimer += (dark ? 2 : 1.5f) * strength;
            if (rainTimer > 1)
            {
                for (int i = 0; i < rainTimer; i++)
                {
                    rainTimer -= 1;
                    Level.Add(new RainParticel(new Vec2(Rando.Float(Level.current.topLeft.x - 400, Level.current.bottomRight.x + 400), Level.current.topLeft.y - 200), GameLevel.rainwind));
                }
            }
            Layer.Game.fade = rainDarken;
            Layer.Glow.fade = rainDarken;
            Layer.Blocks.fade = rainDarken;
            Layer.Virtual.fade = rainDarken;
            Layer.Parallax.fade = rainDarken;
            Layer.Foreground.fade = rainDarken;
            Layer.Background.fade = rainDarken;
        }
    }
}
