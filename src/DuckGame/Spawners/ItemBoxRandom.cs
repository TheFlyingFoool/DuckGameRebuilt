// Decompiled with JetBrains decompiler
// Type: DuckGame.ItemBoxRandom
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

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
        }

        public override void Update() => base.Update();

        public override void Draw()
        {
            _sprite.frame += 2;
            base.Draw();
            _sprite.frame -= 2;
        }

        public static PhysicsObject GetRandomItem()
        {
            List<System.Type> physicsObjects = ItemBox.GetPhysicsObjects(Editor.Placeables);
            physicsObjects.RemoveAll(t => t == typeof(LavaBarrel) || t == typeof(Grapple) || t == typeof(Slag) || t == typeof(Holster));
            System.Type t1;
            if (Rando.Int(10000) == 0)
            {
                t1 = typeof(PositronShooter);
                Options.Data.specialTimes = 100;
            }
            else
            {
                if (Options.Data.specialTimes > 0)
                {
                    physicsObjects.Add(typeof(PositronShooter));
                    physicsObjects.Add(typeof(PositronShooter));
                    --Options.Data.specialTimes;
                }
                t1 = physicsObjects[Rando.Int(physicsObjects.Count - 1)];
            }
            PhysicsObject thing = Editor.CreateThing(t1) as PhysicsObject;
            if (Rando.Int(1000) == 1 && thing is Gun && (thing as Gun).CanSpawnInfinite())
            {
                (thing as Gun).infiniteAmmoVal = true;
                (thing as Gun).infinite.value = true;
            }
            if (thing is Rock && Rando.Int(1000000) == 0)
                thing = Editor.CreateThing(typeof(SpawnedGoldRock)) as PhysicsObject;
            return thing;
        }

        public override PhysicsObject GetSpawnItem()
        {
            PhysicsObject randomItem = ItemBoxRandom.GetRandomItem();
            contains = randomItem.GetType();
            return randomItem;
        }

        public override void DrawHoverInfo()
        {
        }
    }
}
