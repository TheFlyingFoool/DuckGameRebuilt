using System;
using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    //[EditorGroup("Rebuilt|Stuff")]
    public class BlizzardThing : Thing, IDrawToDifferentLayers
    {
        public EditorProperty<float> strength = new EditorProperty<float>(2, null, -4, 4, 0.1f);
        public EditorProperty<float> spawn = new EditorProperty<float>(3, null, 0, 30, 0.1f);
        public EditorProperty<float> flipInterval = new EditorProperty<float>(0, null, 0, 30, 0.1f, "NO FLIP");
        public BlizzardThing(float xpos, float ypos) : base(xpos, ypos)
        {
            graphic = new Sprite("blizzardThing");
            center = new Vec2(8);
            collisionSize = new Vec2(16);
            _collisionOffset = new Vec2(-8);
            _visibleInGame = false;
        }
        public float flipTimer;
        public float spawnTimer;

        public int queue;

        public float currentStrength;
        public float strengthTo = -500;
        public IEnumerable<Thing> allObjects = new List<Thing>();
        public float snowTimer;
        public override void Update()
        {
            if (strengthTo < -50) strengthTo = strength.value;
            spawnTimer += 0.017f;
            if (spawnTimer > spawn.value)
            {
                currentStrength = Lerp.Float(currentStrength, strengthTo, 0.1f);
                if (flipInterval > 0)
                {
                    flipTimer += 0.017f;
                    if (flipTimer > flipInterval)
                    {
                        strengthTo *= -1;
                        flipTimer = 0;
                    }
                }

                float multVal = Math.Abs(currentStrength) / 10f;
                if (multVal > highest) highest = multVal;

                queue += 1;
                if (queue > 10)
                {
                    queue = 0;
                    allObjects = Level.current.things[typeof(PhysicsObject)];
                }
                foreach (Thing t in allObjects)
                {
                    if (t.isServerForObject)
                    {
                        float mult = 0.5f;
                        if (t is Duck) mult = 0.93f;

                        float val = currentStrength * 0.1f * mult;

                        if ((val > 0 && t.hSpeed < val) || (val < 0 && t.hSpeed > val))
                        {
                            t.hSpeed += val;
                        }

                        ((PhysicsObject)t).specialFrictionMod = 0.7f;
                    }
                }

                GameLevel.rainwind = currentStrength;
                snowTimer += 0.1f * DGRSettings.WeatherMultiplier;
                if (snowTimer > 1)
                {
                    for (int i = 0; i < snowTimer; i++)
                    {
                        snowTimer -= 1;
                        Vec2 v = new Vec2(Rando.Float(Level.current.topLeft.x - 128, Level.current.bottomRight.x + 128), Level.current.topLeft.y - 100);
                        SnowFallParticle sn = new SnowFallParticle(v.x - currentStrength * 50, v.y, new Vec2(currentStrength * 0.5f, 2), Rando.Int(2) == 0);
                        sn.life = Rando.Float(1, 2);
                        sn._airFriction = 0;
                        sn._size = Rando.Float(1, 1.5f);
                        Level.Add(sn);
                    }
                }
            }
        }

        public float highest;
        public void OnDrawLayer(Layer l)
        {
            if (l == Layer.HUD && Level.current is not Editor)
            {
                Graphics.DrawRect(new Vec2(-100), new Vec2(500), Colors.Platinum * highest, 1);
            }
        }
    }
}