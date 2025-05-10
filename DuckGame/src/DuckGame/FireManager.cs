﻿namespace DuckGame
{
    public class FireManager
    {
        private static int _curFireID;
        private static int _curUpdateID;
        private static int _curSmokeID;

        public static int GetFireID()
        {
            ++_curFireID;
            if (_curFireID > 20)
                _curFireID = 0;
            return _curFireID;
        }

        public static int GetSmokeID()
        {
            ++_curSmokeID;
            if (_curSmokeID > 20)
                _curSmokeID = 0;
            return _curSmokeID;
        }

        public static void Update()
        {
            foreach (SmallFire litBy in Level.current.things[typeof(SmallFire)])
            {
                if (litBy.y >= -2000f && litBy.fireID == _curUpdateID && litBy.alpha > 0.5f)
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
                                    if (fluidPuddle.data.flammable <= 0.5f && fluidPuddle.data.heat < 0.5f && fluidPuddle.data.douseFire > 0.5f)
                                    {
                                        Level.Remove(litBy);
                                        break;
                                    }
                                }
                                Duck duck = materialThing as Duck;
                                if (duck != null && (duck.slideBuildup > 0f && duck.sliding || duck.holdObject is Sword && (duck.holdObject as Sword)._slamStance))
                                {
                                    if (DGRSettings.S_ParticleMultiplier != 0)
                                    {
                                        SmallSmoke smallSmoke = SmallSmoke.New(litBy.x + Rando.Float(-1f, 1f), litBy.y + Rando.Float(-1f, 1f));
                                        smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                                        Level.Add(smallSmoke);
                                    }
                                    Level.Remove(litBy);
                                }
                                else if (Rando.Float(1000f) < materialThing.flammable * 1000f && (litBy.whoWait == null || duck != litBy.whoWait))
                                    materialThing.Burn(litBy.position + new Vec2(0f, 4f), litBy);
                            }
                        }
                    }
                }
            }
            foreach (FluidPuddle puddle in Level.current.things[typeof(FluidPuddle)])
            {
                if (puddle.data.flammable <= 0.5)
                    puddle.onFire = false;
                else if (puddle.onFire && puddle.fireID == _curUpdateID && puddle.alpha > 0.5)
                {
                    foreach (MaterialThing materialThing in Level.CheckRectAll<MaterialThing>(puddle.topLeft + new Vec2(0f, -4f), puddle.topRight + new Vec2(0f, 2f)))
                    {
                        if (materialThing != puddle && materialThing.isServerForObject)
                        {
                            if ((!(materialThing is Duck duck) || duck.slideBuildup <= 0f) && Rando.Float(1000f) < materialThing.flammable * 1000f)
                                materialThing.Burn(puddle.position + new Vec2(0f, 4f), puddle);
                            materialThing.DoHeatUp(0.05f, puddle.position);
                        }
                    }
                }
                else if (!puddle.onFire)
                {
                    Rectangle rectangle = puddle.rectangle;
                    foreach (Spark sparks in Level.current.things[typeof(Spark)])
                    {
                        if (sparks.x > rectangle.x && sparks.x < rectangle.x + rectangle.width && sparks.y > rectangle.y && sparks.y < rectangle.y + rectangle.height)
                        {
                            puddle.Burn(puddle.position, sparks);
                            break;
                        }
                    }
                }
            }
            foreach (ExtinguisherSmoke extinguisherSmoke in Level.current.things[typeof(ExtinguisherSmoke)])
            {
                if (extinguisherSmoke.smokeID == _curUpdateID)
                {
                    foreach (SmallFire smallFire in Level.CheckCircleAll<SmallFire>(extinguisherSmoke.position + new Vec2(0f, -8f), 12f))
                    {
                        if (extinguisherSmoke.scale.x > 1f)
                            smallFire.SuckLife(10f);
                    }
                    foreach (MaterialThing materialThing in Level.CheckCircleAll<MaterialThing>(extinguisherSmoke.position + new Vec2(0f, -8f), 4f))
                    {
                        if (extinguisherSmoke.scale.x > 1f)
                            materialThing.spreadExtinguisherSmoke = 1f;
                        if (materialThing.physicsMaterial == PhysicsMaterial.Metal)
                            materialThing.DoFreeze(0.03f, extinguisherSmoke.position);
                        if (materialThing.onFire && Rando.Float(1000f) > materialThing.flammable * 650f)
                            materialThing.Extinquish();
                    }
                }
            }
            ++_curUpdateID;
            if (_curUpdateID <= 20)
                return;
            _curUpdateID = 0;
        }
    }
}
