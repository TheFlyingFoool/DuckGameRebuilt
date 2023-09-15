using System;

namespace DuckGame
{
    public class ModDebug : Thing
    {
        public ModDebug() : base(0, 0)
        {
            shouldbegraphicculled = false;
        }

        [DevConsoleCommand(Name = "moddebug", IsCheat = true)]
        public static void MDDebug()
        {
            if (instance != null && instance.level == Level.current)
            {
                Level.Remove(instance);
                instance = null;
            }
            else
            {
                instance = new ModDebug();
                Level.Add(instance);
            }
        }
        public static ModDebug instance;

        public Vec2 initial;
        public Vec2 initialOFF;
        public Vec2 initialOFF2;
        public Vec2 initialOFF3;

        public bool OPbarrelOffset;
        public bool OPlaserOffset;
        public bool OPcollisionOffset;
        public bool OPcenter;
        public int OPcollisionSized = -1;

        public void ColOP(Holdable h)
        {
            if (OPcollisionSized != 0)
            {
                if (OPcollisionSized == 1)
                {
                    Vec2 V = Mouse.positionScreen - initial + initialOFF;

                    Vec2 additive = initialOFF2 - h.bottomRight;

                    additive.x = (float)Math.Round(additive.x * 2);
                    additive.y = (float)Math.Round(additive.y * 2);
                    additive /= 2;
                    h.collisionSize += additive;

                    V.x = (float)Math.Round(V.x * 2);
                    V.y = (float)Math.Round(V.y * 2);
                    V /= 2;
                    h.SetPrivateFieldValue("_collisionOffset", V);

                }
                else if (OPcollisionSized == 2)
                {

                    Vec2 rV = Mouse.positionScreen - initial + initialOFF;

                    float alg = rV.y;

                    rV.x = initialOFF.x;
                    rV.y = (float)Math.Round(rV.y * 2);
                    rV.y /= 2;


                    Vec2 V = Mouse.positionScreen - initial + h.topRight;

                    V.x = (float)Math.Round(V.x * 2);
                    V.y = (float)Math.Round(V.y * 2);
                    V /= 2;

                    h.top = V.y;
                    h.right = V.x;

                    Vec2 additive = h.bottomLeft - initialOFF2;
                    additive.y = alg - h.collisionOffset.y;
                    h.SetPrivateFieldValue("_collisionOffset", rV);

                    float yarg = (h.bottomLeft - initialOFF2).y;
                    additive.y -= yarg;


                    additive.x = (float)Math.Round(additive.x * 2);
                    additive.y = (float)Math.Round(additive.y * 2);
                    additive /= 2;

                    h.collisionSize += additive;
                    initialOFF2 = h.bottomLeft;
                }
                else if (OPcollisionSized == 3)
                {
                    Vec2 rV = Mouse.positionScreen - initial + initialOFF;

                    float alg = rV.x;

                    rV.x = (float)Math.Round(rV.x * 2);
                    rV.x /= 2;
                    rV.y = initialOFF.y;


                    Vec2 V = Mouse.positionScreen - initial + initialOFF3;

                    V.x = (float)Math.Round(V.x * 2);
                    V.y = (float)Math.Round(V.y * 2);
                    V /= 2;

                    h.top = V.y;
                    h.left = V.x;

                    h.SetPrivateFieldValue("_collisionOffset", rV);

                    Vec2 additive = h.topRight - initialOFF2;
                    additive.x = alg - h.collisionOffset.x;

                    Vec2 yarg = (h.topRight - initialOFF2);
                    additive.x -= yarg.x;
                    if (additive.y > h.collisionSize.y - 1)

                    {
                        additive.y = 0; //fucking hell
                    }

                    additive.x = (float)Math.Round(additive.x * 2);
                    additive.y = (float)Math.Round(additive.y * 2);

                    additive /= 2;


                    h.collisionSize += additive;
                    initialOFF2 = h.topRight;
                }
                else if (OPcollisionSized == 4)
                {
                    Vec2 V = Mouse.positionScreen - initial + h.bottomRight;

                    V.x = (float)Math.Round(V.x * 2);
                    V.y = (float)Math.Round(V.y * 2);
                    V /= 2;

                    h.right = V.x;
                    h.bottom = V.y;

                    Vec2 additive = h.bottomRight - initialOFF;

                    //Vec2 yarg = (h.bottomRight - initialOFF);
                    //additive -= yarg;


                    additive.x = (float)Math.Round(additive.x * 2);
                    additive.y = (float)Math.Round(additive.y * 2);
                    additive /= 2;

                    h.collisionSize += additive;
                    initialOFF = h.bottomRight;
                }
            }
        }
        public override void Draw()
        {
            if (Profiles.DefaultPlayer1.duck == null) return;
            Duck d = Profiles.DefaultPlayer1.duck;
            if (d != null && d.holdObject != null)
            {
                Graphics.DrawLine(Mouse.positionScreen - new Vec2(8, 0), Mouse.positionScreen + new Vec2(8, 0), Color.White * 0.5f, 2, 1);
                Graphics.DrawLine(Mouse.positionScreen - new Vec2(0, 8), Mouse.positionScreen + new Vec2(0, 8), Color.White * 0.5f, 2, 1);

                Rectangle mRect = new Rectangle(Mouse.positionScreen - new Vec2(0.5f), Mouse.positionScreen + new Vec2(0.5f));
                Holdable h = d.holdObject;
                Graphics.DrawRect(h.position + h.collisionOffset, h.position + h.collisionSize + h.collisionOffset, Color.White * 0.4f, h.depth + 1, false);
                Graphics.DrawRect(h.topLeft, h.bottomRight, Color.Orange * 0.4f, h.depth + 1, false);
                Graphics.DrawLine(h.position + h.collisionOffset, h.position, Color.Blue * 0.5f, 1, h.depth + 2);


                if (Collision.Rect(new Rectangle(h.topLeft - new Vec2(2), h.topLeft - new Vec2(4)), mRect) || OPcollisionSized == 1)
                {
                    Graphics.DrawRect(h.topLeft - new Vec2(2), h.topLeft - new Vec2(4), Color.White, h.depth + 1, false);
                    if (Mouse.left == InputState.Pressed)
                    {
                        OPcollisionSized = 1;
                        initial = Mouse.positionScreen;
                        initialOFF = h.collisionOffset;
                        initialOFF2 = h.bottomRight;
                    }
                    else if (Mouse.left == InputState.Released)
                    {
                        OPcollisionSized = 0;
                        OPbarrelOffset = false;
                        DevConsole.Log($"Final COLSIZ {h.collisionSize}", Color.Cyan, 6);
                        DevConsole.Log($"Final COLOFF {h.collisionOffset}", Color.Cyan, 6);
                    }
                }
                else Graphics.DrawRect(h.topLeft - new Vec2(2), h.topLeft - new Vec2(4), Color.Orange, h.depth + 1, false);

                if (Collision.Rect(new Rectangle(h.topRight + new Vec2(4, -2), h.topRight + new Vec2(2, -4)), mRect) || OPcollisionSized == 2)
                {
                    Graphics.DrawRect(h.topRight + new Vec2(4, -2), h.topRight + new Vec2(2, -4), Color.White, h.depth + 1, false);
                    if (Mouse.left == InputState.Pressed)
                    {
                        OPcollisionSized = 2;
                        initial = Mouse.positionScreen;
                        initialOFF = h.collisionOffset;
                        initialOFF2 = h.bottomLeft;
                    }
                    else if (Mouse.left == InputState.Released)
                    {
                        OPcollisionSized = 0;
                        OPbarrelOffset = false;
                        DevConsole.Log($"Final COLSIZ {h.collisionSize}", Color.Cyan, 6);
                        DevConsole.Log($"Final COLOFF {h.collisionOffset}", Color.Cyan, 6);
                    }
                }
                else Graphics.DrawRect(h.topRight + new Vec2(4, -2), h.topRight + new Vec2(2, -4), Color.Orange, h.depth + 1, false);

                if (Collision.Rect(new Rectangle(h.bottomLeft + new Vec2(-2, 4), h.bottomLeft + new Vec2(-4, 2)), mRect) || OPcollisionSized == 3)
                {
                    Graphics.DrawRect(h.bottomLeft + new Vec2(-2, 4), h.bottomLeft + new Vec2(-4, 2), Color.White, h.depth + 1, false);
                    if (Mouse.left == InputState.Pressed)
                    {
                        OPcollisionSized = 3;
                        initial = Mouse.positionScreen;
                        initialOFF = h.collisionOffset;
                        initialOFF2 = h.topRight;
                        initialOFF3 = h.bottomLeft;
                    }
                    else if (Mouse.left == InputState.Released)
                    {
                        OPcollisionSized = 0;
                        OPbarrelOffset = false;
                        DevConsole.Log($"Final COLSIZ {h.collisionSize}", Color.Cyan, 6);
                        DevConsole.Log($"Final COLOFF {h.collisionOffset}", Color.Cyan, 6);
                    }
                }
                else Graphics.DrawRect(h.bottomLeft + new Vec2(-2, 4), h.bottomLeft + new Vec2(-4, 2), Color.Orange, h.depth + 1, false);

                if (Collision.Rect(new Rectangle(h.bottomRight + new Vec2(4), h.bottomRight + new Vec2(2)), mRect) || OPcollisionSized == 4)
                {
                    Graphics.DrawRect(h.bottomRight + new Vec2(4), h.bottomRight + new Vec2(2), Color.White, h.depth + 1, false);
                    if (Mouse.left == InputState.Pressed)
                    {
                        OPcollisionSized = 4;
                        initial = Mouse.positionScreen;
                        initialOFF = h.bottomRight;
                    }
                    else if (Mouse.left == InputState.Released)
                    {
                        OPcollisionSized = 0;
                        OPbarrelOffset = false;
                        DevConsole.Log($"Final COLSIZ {h.collisionSize}", Color.Cyan, 6);
                    }
                }
                else Graphics.DrawRect(h.bottomRight + new Vec2(4), h.bottomRight + new Vec2(2), Color.Orange, h.depth + 1, false);


                if (Collision.Rect(new Rectangle(h.position + h.collisionOffset - new Vec2(2), h.position + h.collisionOffset + new Vec2(2)), mRect) || OPcollisionOffset)
                {
                    Graphics.DrawRect(h.position + h.collisionOffset - new Vec2(2), h.position + h.collisionOffset + new Vec2(2), Color.White, h.depth + 2, false);
                    if (Mouse.left == InputState.Pressed)
                    {
                        OPcollisionOffset = true;
                        initial = Mouse.positionScreen;
                        initialOFF = h.collisionOffset;
                    }
                    else if (Mouse.left == InputState.Released)
                    {
                        OPcollisionOffset = false;
                        DevConsole.Log($"Final COLOFF {h.collisionOffset}", Color.Cyan, 6);
                    }
                }
                else Graphics.DrawRect(h.position + h.collisionOffset - new Vec2(2), h.position + h.collisionOffset + new Vec2(2), Color.Blue, h.depth + 2, false);
                if (OPcollisionOffset)
                {
                    Vec2 V = Mouse.positionScreen - initial + initialOFF;

                    V.x = (float)Math.Round(V.x * 2);
                    V.y = (float)Math.Round(V.y * 2);
                    V.x /= 2;
                    V.y /= 2;

                    h.SetPrivateFieldValue<Vec2>("_collisionOffset", V);
                }

                if (h is Gun g)
                {

                    Vec2 brv = g.barrelVector;
                    if (g.ammoType != null) brv = brv.Rotate(Maths.DegToRad(g.ammoType.barrelAngleDegrees), Vec2.Zero);

                    //BARREL OFFSET
                    if (Collision.Rect(new Rectangle(g.barrelPosition - new Vec2(2), g.barrelPosition + new Vec2(2)), mRect) || OPbarrelOffset)
                    {
                        Graphics.DrawRect(g.barrelPosition - new Vec2(2), g.barrelPosition + new Vec2(2), Color.White, g.depth + 1, false);
                        if (Mouse.left == InputState.Pressed)
                        {
                            OPbarrelOffset = true;
                            initial = Mouse.positionScreen;
                            initialOFF = g.GetPrivateFieldValue<Vec2>("_barrelOffsetTL");
                        }
                        else if (Mouse.left == InputState.Released)
                        {
                            OPbarrelOffset = false;
                            DevConsole.Log($"Final TL {g.GetPrivateFieldValue<Vec2>("_barrelOffsetTL")}", Color.Cyan, 6);
                        }
                    }
                    else Graphics.DrawRect(g.barrelPosition - new Vec2(2), g.barrelPosition + new Vec2(2), Color.Red, g.depth + 1, false);
                    if (OPbarrelOffset)
                    {
                        Vec2 V = Mouse.positionScreen - initial + initialOFF;

                        V.x = (float)Math.Round(V.x * 2);
                        V.y = (float)Math.Round(V.y * 2);
                        V.x /= 2;
                        V.y /= 2;

                        g.SetPrivateFieldValue<Vec2>("_barrelOffsetTL", V);
                    }

                    if (g.laserSight)
                    {
                        //LASER OFFSET
                        Vec2 ls = h.Offset(g.laserOffset);
                        if (Collision.Rect(new Rectangle(ls - new Vec2(2), ls + new Vec2(2)), mRect) || OPlaserOffset)
                        {
                            Graphics.DrawRect(ls - new Vec2(2), ls + new Vec2(2), Color.White, g.depth + 1, false);
                            if (Mouse.left == InputState.Pressed)
                            {
                                OPlaserOffset = true;
                                initial = Mouse.positionScreen;
                                initialOFF = g.GetPrivateFieldValue<Vec2>("_laserOffsetTL");
                            }
                            else if (Mouse.left == InputState.Released)
                            {
                                OPlaserOffset = false;
                                DevConsole.Log($"Final LTL {g.GetPrivateFieldValue<Vec2>("_laserOffsetTL")}", Color.Cyan, 6);
                            }
                        }
                        else Graphics.DrawRect(ls - new Vec2(2), ls + new Vec2(2), Color.DarkRed, g.depth + 1, false);
                        if (OPlaserOffset)
                        {
                            Vec2 V = Mouse.positionScreen - initial + initialOFF;

                            V.x = (float)Math.Round(V.x * 2);
                            V.y = (float)Math.Round(V.y * 2);
                            V.x /= 2;
                            V.y /= 2;

                            g.SetPrivateFieldValue<Vec2>("_laserOffsetTL", V);
                        }
                    }


                    if (g.ammoType != null)
                    {
                        GeometryItem item = new GeometryItem();
                        item.depth = g.depth.value + 0.01f;


                        Vec2 inacc = brv.Rotate(1.5708f, Vec2.Zero);
                        Vec2 add = (inacc * (g.ammoType.range * (g.ammoType.accuracy - 1))) / 4;
                        if (g.ammoType.accuracy == 1)
                        {
                            Graphics.DrawLine(g.barrelPosition, g.barrelPosition + brv * g.ammoType.range, Color.DarkRed * 0.5f, 1, g.depth + 1);
                        }
                        else
                        {
                            item.AddTriangle(g.barrelPosition, g.barrelPosition + brv * g.ammoType.range + add, g.barrelPosition + brv * g.ammoType.range - add, Color.DarkRed * 0.5f);
                        }
                        Graphics.screen.SubmitGeometry(item);
                    }
                }
                Vec2 v = h.position;

                if (Collision.Rect(new Rectangle(v - new Vec2(2), v + new Vec2(2)), mRect) || OPcenter)
                {
                    Graphics.DrawRect(v - new Vec2(2), v + new Vec2(2), Color.White, h.depth + 1, false);
                    if (Mouse.left == InputState.Pressed)
                    {
                        OPcenter = true;
                        initial = Mouse.positionScreen;
                        initialOFF = h.center;
                    }
                    else if (Mouse.left == InputState.Released)
                    {
                        OPcenter = false;
                        DevConsole.Log($"Final center {h.center}", Color.Cyan, 6);
                    }
                }
                else Graphics.DrawRect(v - new Vec2(2), v + new Vec2(2), Color.Yellow, h.depth + 1, false);
                if (OPcenter)
                {
                    Vec2 V = Mouse.positionScreen - initial + initialOFF;

                    V.x = (float)Math.Round(V.x * 2);
                    V.y = (float)Math.Round(V.y * 2);
                    V.x /= 2;
                    V.y /= 2;

                    h.center = V;
                }

                Vec2 zx = new Vec2(h.graphic.width, h.graphic.height) / 2f;
                Graphics.DrawDottedRect(v - zx, v + zx, Color.Gray * 0.4f, h.depth + 1, 1, 6);

                ColOP(h);
            }
        }
    }
}
