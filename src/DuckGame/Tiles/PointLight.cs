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
            layer = Layer.Lighting;
            _occluders = occluders;
            _lightColor = c;
            if (_occluders == null)
                _occluders = new List<LightOccluder>();
            _range = range;
            _strangeFalloff = strangeFalloff;
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
            if (!_initialized)
            {
                DrawLightNew();
                foreach (Door door in Level.CheckCircleAll<Door>(position, _range * 0.8f))
                {
                    if (Level.CheckLine<Block>(position, door.position, door) == null || Level.CheckLine<Block>(position, door.topLeft, door) == null || Level.CheckLine<Block>(position, door.bottomRight, door) == null)
                    {
                        _doors[door] = false;
                        _doorList.Add(door);
                    }
                }
                foreach (VerticalDoor verticalDoor in Level.CheckCircleAll<VerticalDoor>(position, _range * 0.8f))
                {
                    if (Level.CheckLine<Block>(position, verticalDoor.position, verticalDoor) == null || Level.CheckLine<Block>(position, verticalDoor.topLeft, verticalDoor) == null || Level.CheckLine<Block>(position, verticalDoor.bottomRight, verticalDoor) == null)
                    {
                        _verticalDoors[verticalDoor] = false;
                        _verticalDoorList.Add(verticalDoor);
                    }
                }
                _initialized = true;
            }
            bool flag = false;
            foreach (Door door in _doorList)
            {
                if (!_doors[door] && Math.Abs(door._open) > 0.800000011920929)
                {
                    _doors[door] = true;
                    flag = true;
                }
                else if (_doors[door] && Math.Abs(door._open) < 0.200000002980232)
                {
                    _doors[door] = false;
                    flag = true;
                }
            }
            foreach (VerticalDoor verticalDoor in _verticalDoorList)
            {
                if (!_verticalDoors[verticalDoor] && Math.Abs(verticalDoor._open) > 0.800000011920929)
                {
                    _verticalDoors[verticalDoor] = true;
                    flag = true;
                }
                else if (_verticalDoors[verticalDoor] && Math.Abs(verticalDoor._open) < 0.200000002980232)
                {
                    _verticalDoors[verticalDoor] = false;
                    flag = true;
                }
            }
            if (fullRefreshCountdown > 0)
            {
                if (fullRefreshCountdown == 1)
                {
                    _objectsInRange = null;
                    DrawLightNew();
                }
                --fullRefreshCountdown;
            }
            else
            {
                if (!flag && _geo != null)
                    return;
                DrawLightNew();
            }
        }

        private void DrawLightNew()
        {
            if (NetworkDebugger.enabled)
                return;
            _geo = MTSpriteBatch.CreateGeometryItem();
            Vec2 vec2_1 = Vec2.Zero;
            Color c3 = Color.White;
            bool flag1 = false;
            if (_objectsInRange == null)
                _objectsInRange = Level.CheckCircleAll<Block>(position, _range).ToList<Block>();
            int num1 = 64;
            for (int index1 = 0; index1 <= num1; ++index1)
            {
                float deg = (float)(index1 / num1 * 360.0);
                Vec2 vec2_2 = new Vec2((float)Math.Cos(Maths.DegToRad(deg)), -(float)Math.Sin(Maths.DegToRad(deg)));
                Vec2 vec2_3 = Vec2.Zero;
                Vec2 point2 = position + vec2_2 * _range;
                Vec2 vec2_4;
                if (_strangeFalloff)
                {
                    vec2_3 = point2;
                }
                else
                {
                    vec2_3 = new Vec2(999999f, 999999f);
                    float num2 = 9999999f;
                    for (int index2 = 0; index2 < _objectsInRange.Count; ++index2)
                    {
                        if (!(_objectsInRange[index2] is Window) && _objectsInRange[index2].solid && Collision.Line(position, point2, _objectsInRange[index2]))
                        {
                            Vec2 vec2_5 = Collision.LinePoint(position, point2, _objectsInRange[index2]);
                            if (vec2_5 != Vec2.Zero)
                            {
                                vec2_4 = vec2_5 - position;
                                float lengthSq = vec2_4.lengthSq;
                                if (lengthSq < num2)
                                {
                                    vec2_3 = vec2_5;
                                    num2 = lengthSq;
                                }
                            }
                        }
                    }
                    if (num2 > 99999.0)
                        vec2_3 = point2;
                }
                Color c = _lightColor;
                vec2_4 = vec2_3 - position;
                float length = vec2_4.length;
                if (_strangeFalloff)
                    length += 30f;
                float num3;
                if (_strangeFalloff)
                {
                    float num4 = 1f - Math.Max(length - 30f, 0f) / _range;
                    num3 = num4 * num4;
                }
                else
                    num3 = (float)(1.0 - length / _range);
                bool flag2 = false;
                Color color = Color.White;
                foreach (LightOccluder occluder in _occluders)
                {
                    if (Collision.LineIntersect(occluder.p1, occluder.p2, position, vec2_3) && (!flag1 || Collision.LineIntersect(occluder.p1, occluder.p2, position, vec2_1)))
                    {
                        Vec3 vector3 = (c * 0.5f).ToVector3();
                        color = occluder.color;
                        c = new Color(vector3 * occluder.color.ToVector3());
                        flag2 = true;
                        break;
                    }
                }
                Color c2 = _lightColor * num3;
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
                    _geo.AddTriangle(position, vec2_3, vec2_1, c, c2, c3);
                }
                flag1 = true;
                vec2_1 = vec2_3;
                c3 = c2;
            }
        }

        public override void Draw()
        {
            if (_geo == null)
                return;
            Graphics.screen.SubmitGeometry(_geo);
        }

        public void Refresh() => fullRefreshCountdown = 3;
    }
}
