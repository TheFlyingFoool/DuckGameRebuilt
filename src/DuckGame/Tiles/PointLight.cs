// Decompiled with JetBrains decompiler
// Type: DuckGame.PointLight
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class PointLight : Thing, ILight
    {
        public Material material;
        private List<LightOccluder> _occluders = new List<LightOccluder>();
        private Color _lightColor;
        private float _range;
        private GeometryItem _geo;
        private bool _strangeFalloff;
        private Dictionary<Door, bool> _doors = new Dictionary<Door, bool>();
        private Dictionary<VerticalDoor, bool> _verticalDoors = new Dictionary<VerticalDoor, bool>();
        private List<Door> _doorList = new List<Door>();
        private List<VerticalDoor> _verticalDoorList = new List<VerticalDoor>();
        private new bool _initialized;
        private List<Block> _objectsInRange;
        private int fullRefreshCountdown;

        public PointLight(
          float xpos,
          float ypos,
          Color c,
          float range,
          List<LightOccluder> occluders = null,
          bool strangeFalloff = false)
          : base(xpos, ypos)
        {
            this.layer = Layer.Lighting;
            this._occluders = occluders;
            this._lightColor = c;
            if (this._occluders == null)
                this._occluders = new List<LightOccluder>();
            this._range = range;
            this._strangeFalloff = strangeFalloff;
        }

        public override void Initialize()
        {
            if (NetworkDebugger.enabled)
                return;
            Layer.lighting = true;
        }

        public override void Update()
        {
            if (NetworkDebugger.enabled)
                return;
            Layer.lighting = true;
            if (!this._initialized)
            {
                this.DrawLightNew();
                foreach (Door door in Level.CheckCircleAll<Door>(this.position, this._range * 0.8f))
                {
                    if (Level.CheckLine<Block>(this.position, door.position, door) == null || Level.CheckLine<Block>(this.position, door.topLeft, door) == null || Level.CheckLine<Block>(this.position, door.bottomRight, door) == null)
                    {
                        this._doors[door] = false;
                        this._doorList.Add(door);
                    }
                }
                foreach (VerticalDoor verticalDoor in Level.CheckCircleAll<VerticalDoor>(this.position, this._range * 0.8f))
                {
                    if (Level.CheckLine<Block>(this.position, verticalDoor.position, verticalDoor) == null || Level.CheckLine<Block>(this.position, verticalDoor.topLeft, verticalDoor) == null || Level.CheckLine<Block>(this.position, verticalDoor.bottomRight, verticalDoor) == null)
                    {
                        this._verticalDoors[verticalDoor] = false;
                        this._verticalDoorList.Add(verticalDoor);
                    }
                }
                this._initialized = true;
            }
            bool flag = false;
            foreach (Door door in this._doorList)
            {
                if (!this._doors[door] && (double)Math.Abs(door._open) > 0.800000011920929)
                {
                    this._doors[door] = true;
                    flag = true;
                }
                else if (this._doors[door] && (double)Math.Abs(door._open) < 0.200000002980232)
                {
                    this._doors[door] = false;
                    flag = true;
                }
            }
            foreach (VerticalDoor verticalDoor in this._verticalDoorList)
            {
                if (!this._verticalDoors[verticalDoor] && (double)Math.Abs(verticalDoor._open) > 0.800000011920929)
                {
                    this._verticalDoors[verticalDoor] = true;
                    flag = true;
                }
                else if (this._verticalDoors[verticalDoor] && (double)Math.Abs(verticalDoor._open) < 0.200000002980232)
                {
                    this._verticalDoors[verticalDoor] = false;
                    flag = true;
                }
            }
            if (this.fullRefreshCountdown > 0)
            {
                if (this.fullRefreshCountdown == 1)
                {
                    this._objectsInRange = null;
                    this.DrawLightNew();
                }
                --this.fullRefreshCountdown;
            }
            else
            {
                if (!flag && this._geo != null)
                    return;
                this.DrawLightNew();
            }
        }

        private void DrawLightNew()
        {
            if (NetworkDebugger.enabled)
                return;
            this._geo = MTSpriteBatch.CreateGeometryItem();
            Vec2 vec2_1 = Vec2.Zero;
            Color c3 = Color.White;
            bool flag1 = false;
            if (this._objectsInRange == null)
                this._objectsInRange = Level.CheckCircleAll<Block>(this.position, this._range).ToList<Block>();
            int num1 = 64;
            for (int index1 = 0; index1 <= num1; ++index1)
            {
                float deg = (float)(index1 / (double)num1 * 360.0);
                Vec2 vec2_2 = new Vec2((float)Math.Cos((double)Maths.DegToRad(deg)), -(float)Math.Sin((double)Maths.DegToRad(deg)));
                Vec2 vec2_3 = Vec2.Zero;
                Vec2 point2 = this.position + vec2_2 * this._range;
                Vec2 vec2_4;
                if (this._strangeFalloff)
                {
                    vec2_3 = point2;
                }
                else
                {
                    vec2_3 = new Vec2(999999f, 999999f);
                    float num2 = 9999999f;
                    for (int index2 = 0; index2 < this._objectsInRange.Count; ++index2)
                    {
                        if (!(this._objectsInRange[index2] is Window) && this._objectsInRange[index2].solid && Collision.Line(this.position, point2, this._objectsInRange[index2]))
                        {
                            Vec2 vec2_5 = Collision.LinePoint(this.position, point2, this._objectsInRange[index2]);
                            if (vec2_5 != Vec2.Zero)
                            {
                                vec2_4 = vec2_5 - this.position;
                                float lengthSq = vec2_4.lengthSq;
                                if ((double)lengthSq < (double)num2)
                                {
                                    vec2_3 = vec2_5;
                                    num2 = lengthSq;
                                }
                            }
                        }
                    }
                    if ((double)num2 > 99999.0)
                        vec2_3 = point2;
                }
                Color c = this._lightColor;
                vec2_4 = vec2_3 - this.position;
                float length = vec2_4.length;
                if (this._strangeFalloff)
                    length += 30f;
                float num3;
                if (this._strangeFalloff)
                {
                    float num4 = 1f - Math.Max(length - 30f, 0.0f) / this._range;
                    num3 = num4 * num4;
                }
                else
                    num3 = (float)(1.0 - (double)length / _range);
                bool flag2 = false;
                Color color = Color.White;
                foreach (LightOccluder occluder in this._occluders)
                {
                    if (Collision.LineIntersect(occluder.p1, occluder.p2, this.position, vec2_3) && (!flag1 || Collision.LineIntersect(occluder.p1, occluder.p2, this.position, vec2_1)))
                    {
                        Vec3 vector3 = (c * 0.5f).ToVector3();
                        color = occluder.color;
                        c = new Color(vector3 * occluder.color.ToVector3());
                        flag2 = true;
                        break;
                    }
                }
                Color c2 = this._lightColor * num3;
                if (flag2)
                    c2 = new Color((c2 * 0.5f).ToVector3() * color.ToVector3());
                c2.a = 0;
                c.a = 0;
                if (flag1)
                {
                    if (!Layer.lightingTwoPointOh)
                    {
                        vec2_3.x = (float)Math.Round(vec2_3.x);
                        vec2_3.y = (float)Math.Round(vec2_3.y);
                    }
                    this._geo.AddTriangle(this.position, vec2_3, vec2_1, c, c2, c3);
                }
                flag1 = true;
                vec2_1 = vec2_3;
                c3 = c2;
            }
        }

        public override void Draw()
        {
            if (this._geo == null)
                return;
            Graphics.screen.SubmitGeometry(this._geo);
        }

        public void Refresh() => this.fullRefreshCountdown = 3;
    }
}
