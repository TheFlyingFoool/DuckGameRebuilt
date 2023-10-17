using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [EditorGroup("Spawns")]
    [BaggedProperty("isInDemo", false)]
    [BaggedProperty("previewPriority", false)]
    public class ItemBoxRandom : ItemBox
    {
        public ItemBoxRandom(float xpos, float ypos)
          : base(xpos, ypos)
        {
            editorTooltip = "Spawns a random object each time it's used. Recharges after a short duration.";
            editorCycleType = typeof(ItemBox);
        }

        public override void Draw()
        {
            _sprite.frame += 2;
            base.Draw();
            _sprite.frame -= 2;
        }

        public static PhysicsObject GetRandomItem()
        {
            return GetRandomItem<PhysicsObject>();
        }

        static bool _gettingTaped = false;
        public static T GetRandomItem<T>()
        {
            List<Type> things = GetPhysicsObjects(Editor.Placeables);
            things.RemoveAll(t => t == typeof(LavaBarrel) || t == typeof(Grapple) || t == typeof(Slag) || t == typeof(Holster) || typeof(T).IsAssignableFrom(t) == false);

            Type contains = things[Rando.Int(things.Count - 1)];
            if (Rando.Int(10000) == 0 && typeof(T).IsAssignableFrom(typeof(PositronShooter)))
            {
                contains = typeof(PositronShooter);
                Options.Data.specialTimes = 100;
            }
            else
            {
                //Allow PositronShooter to have a double chance to spawn 100 times
                //after the initial super rare spawning
                if (Options.Data.specialTimes > 0 && typeof(T).IsAssignableFrom(typeof(PositronShooter)))
                {
                    things.Add(typeof(PositronShooter));
                    things.Add(typeof(PositronShooter));
                    Options.Data.specialTimes--;
                }

                if (Editor.clientonlycontent)
                {
                    if (_gettingTaped == false && Rando.Int(1500) == 0 && typeof(T).IsAssignableFrom(typeof(TapedGun)))
                    {
                        //Chance for spawning two guns taped together
                        _gettingTaped = true;

                        TapedGun t = new TapedGun(0, 0);
                        t.gun1 = GetRandomItem<Gun>();
                        t.gun2 = GetRandomItem<Gun>();
                        t.ReorderTapedGunsByPreference();


                        _gettingTaped = false;
                        Holdable choldable = t.gun1.BecomeTapedMonster(t);
                        if (choldable != null)
                        {
                            return (T)(object)choldable;
                        }
                        return (T)(object)t;
                    }
                    else if (Rando.Int(2000) == 1 && typeof(T).IsAssignableFrom(typeof(Gun)))
                    {
                        if (Rando.Int(50) == 0) // 1/100000 lmao
                            contains = typeof(SohRock);
                        else
                        {
                            contains = DGRDevs.AllWithGuns.ChooseRandom().DevItem;

                            // to be removed when all devs get their gun
                            if (contains == typeof(PositronShooter)) contains = typeof(DanGun);
                        }
                    }
                }
            }

            T newThing = (T)(object)Editor.CreateThing(contains);
            if (Rando.Int(1000) == 1 && newThing is Gun && (newThing as Gun).CanSpawnInfinite())
            {
                (newThing as Gun).infiniteAmmoVal = true;
                (newThing as Gun).infinite.value = true;
            }

            if (newThing is Rock && Rando.Int(1000000) == 0) //The fabled golden stone
                newThing = (T)(object)Editor.CreateThing(typeof(SpawnedGoldRock));

            return newThing;
        }

        public override PhysicsObject GetSpawnItem()
        {
            PhysicsObject randomItem = GetRandomItem();
            contains = randomItem.GetType();
            return randomItem;
        }

        public override void DrawHoverInfo()
        {
        }
    }
}
