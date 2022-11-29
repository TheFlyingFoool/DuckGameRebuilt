﻿// Decompiled with JetBrains decompiler
// Type: DuckGame.FireManager
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class FireManager
    {
        private static int _curFireID;
        private static int _curUpdateID;
        private static int _curSmokeID;

        public static int GetFireID()
        {
            ++FireManager._curFireID;
            if (FireManager._curFireID > 20)
                FireManager._curFireID = 0;
            return FireManager._curFireID;
        }

        public static int GetSmokeID()
        {
            ++FireManager._curSmokeID;
            if (FireManager._curSmokeID > 20)
                FireManager._curSmokeID = 0;
            return FireManager._curSmokeID;
        }

        public static void Update()
        {
            try
            {
                foreach (SmallFire litBy in Level.current.things[typeof(SmallFire)])
                {
                    if (litBy.y >= -2000.0 && litBy.fireID == FireManager._curUpdateID && litBy.alpha > 0.5)
                    {
                        Thing thing = null;
                        if (litBy.stick != null && (litBy.stick is DartGun || litBy.stick is Chaindart))
                            thing = litBy.stick.owner;
                        litBy.doFloat = false;
                        foreach (MaterialThing materialThing in Level.CheckCircleAll<MaterialThing>(litBy.position + new Vec2(0f, -4f), 6f))
                        {
                            if (materialThing != thing)
                            {
                                materialThing.DoHeatUp(0.05f, litBy.position);
                                if (materialThing.isServerForObject)
                                {
                                    if (materialThing is FluidPuddle)
                                    {
                                        litBy.doFloat = true;
                                        FluidPuddle fluidPuddle = materialThing as FluidPuddle;
                                        if (fluidPuddle.data.flammable <= 0.5 && fluidPuddle.data.heat < 0.5 && fluidPuddle.data.douseFire > 0.5)
                                        {
                                            Level.Remove(litBy);
                                            break;
                                        }
                                    }
                                    Duck duck = materialThing as Duck;
                                    if (duck != null && (duck.slideBuildup > 0.0 && duck.sliding || duck.holdObject is Sword && (duck.holdObject as Sword)._slamStance))
                                    {
                                        if (DGRSettings.S_ParticleMultiplier != 0)
                                        {
                                            SmallSmoke smallSmoke = SmallSmoke.New(litBy.x + Rando.Float(-1f, 1f), litBy.y + Rando.Float(-1f, 1f));
                                            smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                                            Level.Add(smallSmoke);
                                        }
                                        Level.Remove(litBy);
                                    }
                                    else if (Rando.Float(1000f) < materialThing.flammable * 1000.0 && (litBy.whoWait == null || duck != litBy.whoWait))
                                        materialThing.Burn(litBy.position + new Vec2(0f, 4f), litBy);
                                }
                            }
                        }
                    }
                }
                foreach (FluidPuddle litBy1 in Level.current.things[typeof(FluidPuddle)])
                {
                    if (litBy1.data.flammable <= 0.5)
                        litBy1.onFire = false;
                    else if (litBy1.onFire && litBy1.fireID == _curUpdateID && litBy1.alpha > 0.5)
                    {
                        foreach (MaterialThing materialThing in Level.CheckRectAll<MaterialThing>(litBy1.topLeft + new Vec2(0f, -4f), litBy1.topRight + new Vec2(0f, 2f)))
                        {
                            if (materialThing != litBy1 && materialThing.isServerForObject)
                            {
                                if ((!(materialThing is Duck duck) || duck.slideBuildup <= 0.0) && Rando.Float(1000f) < materialThing.flammable * 1000.0)
                                    materialThing.Burn(litBy1.position + new Vec2(0f, 4f), litBy1);
                                materialThing.DoHeatUp(0.05f, litBy1.position);
                            }
                        }
                    }
                    else if (!litBy1.onFire)
                    {
                        Rectangle rectangle = litBy1.rectangle;
                        foreach (Spark litBy2 in Level.current.things[typeof(Spark)])
                        {
                            if (litBy2.x > rectangle.x && litBy2.x < rectangle.x + rectangle.width && litBy2.y > rectangle.y && litBy2.y < rectangle.y + rectangle.height)
                            {
                                litBy1.Burn(litBy1.position, litBy2);
                                break;
                            }
                        }
                    }
                }
                foreach (ExtinguisherSmoke extinguisherSmoke in Level.current.things[typeof(ExtinguisherSmoke)])
                {
                    if (extinguisherSmoke.smokeID == FireManager._curUpdateID)
                    {
                        foreach (SmallFire smallFire in Level.CheckCircleAll<SmallFire>(extinguisherSmoke.position + new Vec2(0f, -8f), 12f))
                        {
                            if (extinguisherSmoke.scale.x > 1.0)
                                smallFire.SuckLife(10f);
                        }
                        foreach (MaterialThing materialThing in Level.CheckCircleAll<MaterialThing>(extinguisherSmoke.position + new Vec2(0f, -8f), 4f))
                        {
                            if (extinguisherSmoke.scale.x > 1.0)
                                materialThing.spreadExtinguisherSmoke = 1f;
                            if (materialThing.physicsMaterial == PhysicsMaterial.Metal)
                                materialThing.DoFreeze(0.03f, extinguisherSmoke.position);
                            if (materialThing.onFire && Rando.Float(1000f) > materialThing.flammable * 650.0)
                                materialThing.Extinquish();
                        }
                    }
                }
                ++FireManager._curUpdateID;
                if (FireManager._curUpdateID <= 20)
                    return;
                FireManager._curUpdateID = 0;
            }
            catch
            {
                DevConsole.Log("FireManager Update catch", Color.Green, 2f, -1);
            }
        }
    }
 }