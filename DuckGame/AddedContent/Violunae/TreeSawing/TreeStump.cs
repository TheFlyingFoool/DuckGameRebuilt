using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class TreeStump : Thing
    {
        public TreeStump(float xpos, float ypos, AutoPlatform sourcePlatform)
            : base(xpos, ypos)
        {
            this._center = new Vec2(8f, 8f);

            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);

            _sourcePlatform = sourcePlatform;
        }

        public AutoPlatform _sourcePlatform;

        public void Saw(bool flipped = false, bool energy = false)
        {
            if (energy) Explode();

            string tex = "treeStump";
            if (_sourcePlatform is CityTreeTileset) tex = "cityTreeStump";
            if (_sourcePlatform is PineTrunkTileset) tex = "pineTreeStump";
            this.graphic = new SpriteMap(tex, 16, 16);
            (this.graphic as SpriteMap).frame = energy ? 2 : 0;

            Level.Remove(_sourcePlatform);
            TreeData td = new TreeData(_sourcePlatform);
            GroupTree(_sourcePlatform, td);
            SFX.Play(td.failedToSaw ? "doorBreak" : "treeBreak", 1, 0.5f);
            if (td.failedToSaw)
            {
                Level.Add(new TreeEnd(_sourcePlatform.x, _sourcePlatform.y, _sourcePlatform is CityTreeTileset, energy));
            }
            else
            {
                foreach (PhysicsObject thing in td.affectedThings)
                {
                    if (thing.Distance(this) >= 32)
                    {
                        thing.vSpeed -= energy ? 1f : 0.2f;
                        if (thing is Duck duck) duck.GoRagdoll(); 
                    }
                }
                td.treeList.Remove(_sourcePlatform);
                foreach (Thing tree in td.treeList)
                {
                    Level.Remove(tree);
                    if (DGRSettings.ActualParticleMultiplier >= 1)
                    {
                        for (int i = 0; i < DGRSettings.ActualParticleMultiplier; i++)
                        {
                            WoodDebris woodDebris = WoodDebris.New(tree.x, tree.y);
                            woodDebris.hSpeed = Rando.Float(-1f, 1f);
                            woodDebris.vSpeed = Rando.Float(-1f, 1f);
                            Level.Add(woodDebris);
                        }
                    }
                    else if (Rando.Float(1) < DGRSettings.ActualParticleMultiplier)
                    {
                        WoodDebris woodDebris = WoodDebris.New(tree.x, tree.y);
                        woodDebris.hSpeed = Rando.Float(-1f, 1f);
                        woodDebris.vSpeed = Rando.Float(-1f, 1f);
                        Level.Add(woodDebris);
                    }
                }
                td.treeList.Add(new TreeEnd(_sourcePlatform.x, _sourcePlatform.y, _sourcePlatform is CityTreeTileset, energy));
                Level.Add(new FallingTree(_sourcePlatform.x, _sourcePlatform.y, td.treeList, flipped, energy));
            }
        }

        private void Explode()
        {
            if (DGRSettings.ActualParticleMultiplier > 0)
            {
                Level.Add(new ExplosionPart(position.x, position.y));
                int num1 = 6;
                if (Graphics.effectsLevel < 2)
                    num1 = 3;
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * num1; ++index)
                {
                    float deg = index * 60f + Rando.Float(-10f, 10f);
                    float num2 = Rando.Float(12f, 20f);
                    Level.Add(new ExplosionPart(position.x + (float)Math.Cos(Maths.DegToRad(deg)) * num2, position.y - (float)Math.Sin(Maths.DegToRad(deg)) * num2));
                }
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 5; ++index)
                {
                    SmallSmoke smallSmoke = SmallSmoke.New(position.x + Rando.Float(-6f, 6f), position.y + Rando.Float(-6f, 6f));
                    smallSmoke.hSpeed += Rando.Float(-0.3f, 0.3f);
                    smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                    Level.Add(smallSmoke);
                }
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 3; ++index)
                {
                    Level.Add(new CampingSmoke(position.x - 5f + Rando.Float(10f), (float)(position.y + 6 - 3 + Rando.Float(6f) - index * 1))
                    {
                        move = {
                            x = (Rando.Float(0.6f) - 0.3f),
                            y = (Rando.Float(1f) - 0.5f)
                        }
                    });
                }
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 6; ++index)
                {
                    WoodDebris woodDebris = WoodDebris.New(position.x - 8f + Rando.Float(16f), position.y - 8f + Rando.Float(16f));
                    woodDebris.hSpeed = (float)((Rando.Float(1f) > 0.5f ? 1 : -1) * Rando.Float(3f));
                    woodDebris.vSpeed = -Rando.Float(1f);
                    Level.Add(woodDebris);
                }
            }
            foreach (Window ignore in Level.CheckCircleAll<Window>(position, 40f))
            {
                if (Level.CheckLine<Block>(position, ignore.position, ignore) == null)
                    ignore.Destroy(new DTImpact(this));
            }
            SFX.Play("explode", pitch: Rando.Float(0.1f, 0.3f));
            RumbleManager.AddRumbleEvent(position, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium));
        }

        public static void GroupTree(AutoPlatform source, TreeData td)
        {
            if ((source.frame == 44 && source != td._sourceStump) || (source.frame == 26) || (source.frame == 27) || (source.frame == 28))
            {
                td.failedToSaw = true;
            }
            if (td.failedToSaw) return;

            td.treeList.Add(source);

            foreach (Thing thing in Level.CheckRectAll<Thing>(source.position - new Vec2(16, 16), source.position + new Vec2(16, 16)))
            {
                if (!td.treeList.Contains(thing))
                {
                    if ((source.GetType() == thing.GetType()) || (source is PineTrunkTileset && thing is PineTree))
                    {
                        GroupTree(thing as AutoPlatform, td);
                    }
                    else if ((thing is TreeEnd) || (thing is TreeTop) || (thing is TreeTopDead) || (thing is GoldenTreeTop) || (thing is SummerTreeTop) || (thing is WinterTreeTop))
                    {
                        td.treeList.Add(thing);
                    }
                    else if (thing is PhysicsObject)
                    {
                        td.affectedThings.Add(thing as PhysicsObject);
                    }
                }
            }
        }

        public class TreeData
        {
            public TreeData(AutoPlatform sourceStump)
            {
                _sourceStump = sourceStump;
            }

            public AutoPlatform _sourceStump;
            public List<Thing> treeList = new List<Thing>();
            public List<PhysicsObject> affectedThings = new List<PhysicsObject>();
            public bool failedToSaw = false;
        }
    }
}
