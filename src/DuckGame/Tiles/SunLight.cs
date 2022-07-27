// Decompiled with JetBrains decompiler
// Type: DuckGame.SunLight
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.layer = Layer.Lighting;
            this._lightColor = c;
            this._range = range;
            this._vertical = vertical;
            this._strangeFalloff = strangeFalloff;
        }

        public override void Initialize() => Layer.lighting = true;

        public override void Update()
        {
            Layer.lighting = true;
            if (!this._initialized)
            {
                this.DrawLight();
                this._initialized = true;
            }
            foreach (SunLight.Section section in this._sections)
                section.RefreshDoors();
            if (this.needsRefresh <= 0)
                return;
            if (this.needsRefresh == 1)
            {
                foreach (SunLight.Section section in this._sections)
                {
                    if (section.NeedsRefresh())
                        section.Refresh();
                }
            }
            --this.needsRefresh;
        }

        public void Refresh() => this.needsRefresh = 3;

        private void DrawLight()
        {
            Vec2 vec2 = Maths.Snap(Level.current.topLeft, 16f, 16f) + new Vec2(-1024f, -256f);
            for (int index = 0; index < 42; ++index)
            {
                SunLight.Section section = new SunLight.Section()
                {
                    start = vec2,
                    lightColor = this._lightColor
                };
                section.Refresh();
                this._sections.Add(section);
                vec2.x += 64f;
            }
        }

        public override void Draw()
        {
            foreach (SunLight.Section section in this._sections)
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
                if (!flag)
                    return;
                this.Refresh();
            }

            public void Refresh()
            {
                this.affectors.Clear();
                this.geo = MTSpriteBatch.CreateGeometryItem();
                this.lightColor.a = 0;
                Vec2 start = this.start;
                float num = 0.25f;
                Vec2 vec2_1 = new Vec2(3000f, 5000f);
                for (int index = 0; index < 8; ++index)
                {
                    Vec2 vec2_2 = start + new Vec2(32f * num, 0.0f);
                    start += new Vec2(8f, 0.0f);
                    Vec2 hitPos = Vec2.Zero;
                    Block key = Level.CheckRay<Block>(vec2_2, vec2_2 + vec2_1, out hitPos);
                    if (key is Window)
                        key = null;
                    if (key == null)
                    {
                        this.geo.AddTriangle(vec2_2, vec2_2 + new Vec2(9f, 0.0f), vec2_2 + vec2_1, this.lightColor, this.lightColor, this.lightColor);
                        this.geo.AddTriangle(vec2_2, vec2_2 + vec2_1, vec2_2 + new Vec2(9f, 0.0f) + vec2_1, this.lightColor, this.lightColor, this.lightColor);
                    }
                    else
                    {
                        if (Level.CheckPoint<Block>(hitPos + new Vec2(0.0f, -9f) + new Vec2(1f, 0.0f)) != null)
                        {
                            this.geo.AddTriangle(vec2_2, vec2_2 + new Vec2(8f, 0.0f), hitPos + new Vec2(0.0f, -18f), this.lightColor, this.lightColor, this.lightColor);
                            this.geo.AddTriangle(vec2_2, hitPos, hitPos + new Vec2(0.0f, -18f), this.lightColor, this.lightColor, this.lightColor);
                        }
                        else
                        {
                            this.geo.AddTriangle(vec2_2, vec2_2 + new Vec2(12f, 0.0f), hitPos + new Vec2(8f, 0.0f), this.lightColor, this.lightColor, this.lightColor);
                            this.geo.AddTriangle(vec2_2, hitPos, hitPos + new Vec2(12f, 0.0f), this.lightColor, this.lightColor, this.lightColor);
                        }
                        this.affectors.Add(key);
                        if (key is Door)
                        {
                            this._doorList.Add(key as Door);
                            this._doors[key as Door] = false;
                        }
                        if (key is VerticalDoor)
                        {
                            this._verticalDoorList.Add(key as VerticalDoor);
                            this._verticalDoors[key as VerticalDoor] = false;
                        }
                    }
                }
            }

            public bool NeedsRefresh()
            {
                foreach (Thing affector in this.affectors)
                {
                    if (affector.removeFromLevel || affector.level == null)
                        return true;
                }
                return false;
            }
        }
    }
}
