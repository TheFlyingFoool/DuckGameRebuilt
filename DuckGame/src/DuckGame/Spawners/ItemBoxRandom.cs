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
            List<System.Type> physicsObjects = GetPhysicsObjects(Editor.Placeables);
            physicsObjects.RemoveAll(t => t == typeof(LavaBarrel) || t == typeof(Grapple) || t == typeof(Slag) || t == typeof(Holster));
            System.Type t1;
            
            if (Rando.Int(10000) == 0)
            {
                t1 = typeof(PositronShooter);
                Options.Data.specialTimes = 100;
            }
            else if (Editor.clientonlycontent && Rando.Int(2000) == 1)
            {
                if (Rando.Int(50) == 0) // 1/100000 lmao
                    t1 = typeof(SohRock);
                else
                {
                    t1 = DGRDevs.AllWithGuns.ChooseRandom().DevItem;

                    // to be removed when all devs get their gun
                    if (t1 == typeof(PositronShooter))
                        t1 = typeof(DanGun);
                }
            }
            else
            {
                if (Options.Data.specialTimes-- > 0)
                {
                    physicsObjects.Add(typeof(PositronShooter));
                    physicsObjects.Add(typeof(PositronShooter));
                }
                
                t1 = physicsObjects.ChooseRandom();
            }

            if (t1 == typeof(Rock) && Rando.Int(1000000) == 0)
                t1 = typeof(SpawnedGoldRock);
            
            PhysicsObject thing = Editor.CreateThing(t1) as PhysicsObject;
            
            if (thing is Gun gun && gun.CanSpawnInfinite() && Rando.Int(1000) == 1)
            {
                gun.infiniteAmmoVal = true;
                gun.infinite.value = true;
            }
            
            return thing;
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
