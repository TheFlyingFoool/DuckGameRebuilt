// Decompiled with JetBrains decompiler
// Type: DuckGame.SunLight
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class SunLight : Thing, ILight
    {
        private Color _lightColor;
        private float _range;
        private bool _strangeFalloff;
        private bool _vertical;
        private Dictionary<Door, bool> _doors = new Dictionary<Door, bool>();
        private List<Door> _doorList = new List<Door>();
        private new bool _initialized;
        private int needsRefresh;
        private List<SunLight.Section> _sections = new List<SunLight.Section>();

        public SunLight(
          float xpos,
          float ypos,
          Color c,
          float range,
          List<LightOccluder> occluders = null,
          bool strangeFalloff = false,
          bool vertical = false)
          : base(xpos, ypos)
        {
            layer = Layer.Lighting;
            _lightColor = c;
            _range = range;
            _vertical = vertical;
            _strangeFalloff = strangeFalloff;
        }

        public override void Initialize() => Layer.lighting = true;

        public override void Update()
        {
            Layer.lighting = true;
            if (!_initialized)
            {
                DrawLight();
                _initialized = true;
            }
            foreach (SunLight.Section section in _sections)
                section.RefreshDoors();
            if (needsRefresh <= 0)
                return;
            if (needsRefresh == 1)
            {
                foreach (SunLight.Section section in _sections)
                {
                    if (section.NeedsRefresh())
                        section.Refresh();
                }
            }
            --needsRefresh;
        }

        public void Refresh() => needsRefresh = 3;

        private void DrawLight()
        {
            Vec2 vec2 = Maths.Snap(Level.current.topLeft, 16f, 16f) + new Vec2(-1024f, -256f);
            for (int index = 0; index < 42; ++index)
            {
                SunLight.Section section = new SunLight.Section()
                {
                    start = vec2,
                    lightColor = _lightColor
                };
                section.Refresh();
                _sections.Add(section);
                vec2.x += 64f;
            }
        }

        public override void Draw()
        {
            foreach (SunLight.Section section in _sections)
                Graphics.screen.SubmitGeometry(section.geo);
        }

        public class Section
        {
            private List<Thing> affectors = new List<Thing>();
            private Dictionary<Door, bool> _doors = new Dictionary<Door, bool>();
            private List<Door> _doorList = new List<Door>();
            private Dictionary<VerticalDoor, bool> _verticalDoors = new Dictionary<VerticalDoor, bool>();
            private List<VerticalDoor> _verticalDoorList = new List<VerticalDoor>();
            public Vec2 start;
            public GeometryItem geo;
            public Color lightColor;

            public void RefreshDoors()
            {
                bool flag = false;
                foreach (Door door in _doorList)
                {
                    if (!_doors[door] && Math.Abs(door._open) > 0.8f)
                    {
                        _doors[door] = true;
                        flag = true;
                    }
                    else if (_doors[door] && Math.Abs(door._open) < 0.2f)
                    {
                        _doors[door] = false;
                        flag = true;
                    }
                }
                foreach (VerticalDoor verticalDoor in _verticalDoorList)
                {
                    if (!_verticalDoors[verticalDoor] && Math.Abs(verticalDoor._open) > 0.8f)
                    {
                        _verticalDoors[verticalDoor] = true;
                        flag = true;
                    }
                    else if (_verticalDoors[verticalDoor] && Math.Abs(verticalDoor._open) < 0.2f)
                    {
                        _verticalDoors[verticalDoor] = false;
                        flag = true;
                    }
                }
                if (!flag)
                    return;
                Refresh();
            }

            public void Refresh()
            {
                affectors.Clear();
                geo = MTSpriteBatch.CreateGeometryItem();
                lightColor.a = 0;
                Vec2 start = this.start;
                float num = 0.25f;
                Vec2 vec2_1 = new Vec2(3000f, 5000f);
                for (int index = 0; index < 8; ++index)
                {
                    Vec2 vec2_2 = start + new Vec2(32f * num, 0f);
                    start += new Vec2(8f, 0f);
                    Vec2 hitPos = Vec2.Zero;
                    Block key = Level.CheckRay<Block>(vec2_2, vec2_2 + vec2_1, out hitPos);
                    if (key is Window)
                        key = null;
                    if (key == null)
                    {
                        geo.AddTriangle(vec2_2, vec2_2 + new Vec2(9f, 0f), vec2_2 + vec2_1, lightColor, lightColor, lightColor);
                        geo.AddTriangle(vec2_2, vec2_2 + vec2_1, vec2_2 + new Vec2(9f, 0f) + vec2_1, lightColor, lightColor, lightColor);
                    }
                    else
                    {
                        if (Level.CheckPoint<Block>(hitPos + new Vec2(0f, -9f) + new Vec2(1f, 0f)) != null)
                        {
                            geo.AddTriangle(vec2_2, vec2_2 + new Vec2(8f, 0f), hitPos + new Vec2(0f, -18f), lightColor, lightColor, lightColor);
                            geo.AddTriangle(vec2_2, hitPos, hitPos + new Vec2(0f, -18f), lightColor, lightColor, lightColor);
                        }
                        else
                        {
                            geo.AddTriangle(vec2_2, vec2_2 + new Vec2(12f, 0f), hitPos + new Vec2(8f, 0f), lightColor, lightColor, lightColor);
                            geo.AddTriangle(vec2_2, hitPos, hitPos + new Vec2(12f, 0f), lightColor, lightColor, lightColor);
                        }
                        affectors.Add(key);
                        if (key is Door)
                        {
                            _doorList.Add(key as Door);
                            _doors[key as Door] = false;
                        }
                        if (key is VerticalDoor)
                        {
                            _verticalDoorList.Add(key as VerticalDoor);
                            _verticalDoors[key as VerticalDoor] = false;
                        }
                    }
                }
            }

            public bool NeedsRefresh()
            {
                foreach (Thing affector in affectors)
                {
                    if (affector.removeFromLevel || affector.level == null)
                        return true;
                }
                return false;
            }
        }
    }
}
