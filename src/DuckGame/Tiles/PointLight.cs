using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class PointLight : Thing, ILight
    {
        public PointLight(float xpos, float ypos, Color c, float range, List<LightOccluder> occluders = null, bool strangeFalloff = false) : base(xpos, ypos, null)
        {
            base.layer = Layer.Lighting;
            this._occluders = occluders;
            this._lightColor = c;
            if (this._occluders == null)
            {
                this._occluders = new List<LightOccluder>();
            }
            this._range = range;
            this._strangeFalloff = strangeFalloff;
        }

        public override void Initialize()
        {
            if (NetworkDebugger.enabled)
            {
                return;
            }
            Layer.lighting = true;
        }

        public override void Update()
        {
            if (NetworkDebugger.enabled)
            {
                return;
            }
            Layer.lighting = true;
            if (!this._initialized)
            {
                this.DrawLightNew();
                foreach (Door d in Level.CheckCircleAll<Door>(this.position, this._range * 0.8f))
                {
                    if (Level.CheckLine<Block>(this.position, d.position, d) == null || Level.CheckLine<Block>(this.position, d.topLeft, d) == null || Level.CheckLine<Block>(this.position, d.bottomRight, d) == null)
                    {
                        this._doors[d] = false;
                        this._doorList.Add(d);
                    }
                }
                foreach (VerticalDoor d2 in Level.CheckCircleAll<VerticalDoor>(this.position, this._range * 0.8f))
                {
                    if (Level.CheckLine<Block>(this.position, d2.position, d2) == null || Level.CheckLine<Block>(this.position, d2.topLeft, d2) == null || Level.CheckLine<Block>(this.position, d2.bottomRight, d2) == null)
                    {
                        this._verticalDoors[d2] = false;
                        this._verticalDoorList.Add(d2);
                    }
                }
                this._initialized = true;
            }
            bool refresh = false;
            foreach (Door door in this._doorList)
            {
                if (!this._doors[door] && Math.Abs(door._open) > 0.8f)
                {
                    this._doors[door] = true;
                    refresh = true;
                }
                else if (this._doors[door] && Math.Abs(door._open) < 0.2f)
                {
                    this._doors[door] = false;
                    refresh = true;
                }
            }
            foreach (VerticalDoor door2 in this._verticalDoorList)
            {
                if (!this._verticalDoors[door2] && Math.Abs(door2._open) > 0.8f)
                {
                    this._verticalDoors[door2] = true;
                    refresh = true;
                }
                else if (this._verticalDoors[door2] && Math.Abs(door2._open) < 0.2f)
                {
                    this._verticalDoors[door2] = false;
                    refresh = true;
                }
            }
            if (this.fullRefreshCountdown > 0)
            {
                if (this.fullRefreshCountdown == 1)
                {
                    this._objectsInRange = null;
                    this.DrawLightNew();
                }
                this.fullRefreshCountdown--;
                return;
            }
            if (refresh || this._geo == null)
            {
                this.DrawLightNew();
            }
        }

        private void DrawLightNew()
        {
            if (NetworkDebugger.enabled)
            {
                return;
            }
            this._geo = MTSpriteBatch.CreateGeometryItem();
            Vec2 prevPos = Vec2.Zero;
            Color farColPrev = Color.White;
            bool hasPrev = false;
            if (this._objectsInRange == null)
            {
                this._objectsInRange = Level.CheckCircleAll<Block>(this.position, this._range).ToList<Block>();
            }
            int loops = 64;
            for (int i = 0; i <= loops; i++)
            {
                Color farColor = Color.Black;
                float a = (float)i / (float)loops * 360f;
                Vec2 dir = new Vec2((float)Math.Cos((double)Maths.DegToRad(a)), -(float)Math.Sin((double)Maths.DegToRad(a)));
                Vec2 rayPos = Vec2.Zero;
                Vec2 castTo = this.position + dir * this._range;
                if (this._strangeFalloff)
                {
                    rayPos = castTo;
                }
                else
                {
                    rayPos = new Vec2(999999f, 999999f);
                    float nearestRay = 9999999f;
                    for (int iBlock = 0; iBlock < this._objectsInRange.Count; iBlock++)
                    {
                        if (!(this._objectsInRange[iBlock] is Window) && this._objectsInRange[iBlock].solid && Collision.Line(this.position, castTo, this._objectsInRange[iBlock]))
                        {
                            Vec2 point = Collision.LinePoint(this.position, castTo, this._objectsInRange[iBlock]);
                            if (point != Vec2.Zero)
                            {
                                float len = (point - this.position).lengthSq;
                                if (len < nearestRay)
                                {
                                    rayPos = point;
                                    nearestRay = len;
                                }
                            }
                        }
                    }
                    if (nearestRay > 99999f)
                    {
                        rayPos = castTo;
                    }
                }
                Color nearColor = this._lightColor;
                float lightLength = (rayPos - this.position).length;
                if (this._strangeFalloff)
                {
                    lightLength += 30f;
                }
                float fade = 0f;
                if (this._strangeFalloff)
                {
                    float val = Math.Max(lightLength - 30f, 0f) / this._range;
                    fade = 1f - val;
                    fade *= fade;
                }
                else
                {
                    fade = 1f - lightLength / this._range;
                }
                bool dark = false;
                Color darkOccluder = Color.White;
                foreach (LightOccluder occluder in this._occluders)
                {
                    if (Collision.LineIntersect(occluder.p1, occluder.p2, this.position, rayPos) && (!hasPrev || Collision.LineIntersect(occluder.p1, occluder.p2, this.position, prevPos)))
                    {
                        Vec3 nc = (nearColor * 0.5f).ToVector3();
                        darkOccluder = occluder.color;
                        nearColor = new Color(nc * occluder.color.ToVector3());
                        dark = true;
                        break;
                    }
                }
                farColor = this._lightColor * fade;
                if (dark)
                {
                    Vec3 nc2 = (farColor * 0.5f).ToVector3();
                    farColor = new Color(nc2 * darkOccluder.ToVector3());
                }
                farColor.a = 0;
                nearColor.a = 0;
                if (hasPrev)
                {
                    if (!Layer.lightingTwoPointOh)
                    {
                        rayPos.x = (float)Math.Round((double)rayPos.x);
                        rayPos.y = (float)Math.Round((double)rayPos.y);
                    }
                    this._geo.AddTriangle(this.position, rayPos, prevPos, nearColor, farColor, farColPrev);
                }
                hasPrev = true;
                prevPos = rayPos;
                farColPrev = farColor;
            }
        }

        public override void Draw()
        {
            if (this._geo != null)
            {
                Graphics.screen.SubmitGeometry(this._geo);
            }
        }

        public void Refresh()
        {
            this.fullRefreshCountdown = 3;
        }

        public new Material material;

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
    }
}
