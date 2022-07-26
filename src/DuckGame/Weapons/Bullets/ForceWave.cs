// Decompiled with JetBrains decompiler
// Type: DuckGame.ForceWave
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class ForceWave : Thing
    {
        public StateBinding _positionBinding = (StateBinding)new InterpolatedVec2Binding("netPosition");
        public StateBinding _offDirBinding = new StateBinding(GhostPriority.High, "_offDir");
        public StateBinding _alphaBinding = new StateBinding(GhostPriority.High, "alpha");
        public StateBinding _waveOwnerBinding = new StateBinding(GhostPriority.High, nameof(_waveOwner));
        private Thing _waveOwner;
        private float _alphaSub;
        private float _speed;
        private float _speedv;
        private List<Thing> _hits = new List<Thing>();

        public ForceWave(
          float xpos,
          float ypos,
          int dir,
          float alphaSub,
          float speed,
          float speedv,
          Duck own)
          : base(xpos, ypos)
        {
            this.offDir = (sbyte)dir;
            this.graphic = new Sprite("sledgeForce");
            this.center = new Vec2((float)this.graphic.w, (float)this.graphic.h);
            this._alphaSub = alphaSub;
            this._speed = speed;
            this._speedv = speedv;
            this._collisionSize = new Vec2(6f, 30f);
            this._collisionOffset = new Vec2(-3f, -15f);
            this.graphic.flipH = this.offDir <= (sbyte)0;
            this._waveOwner = (Thing)own;
            this.depth = - 0.7f;
        }

        public override void Update()
        {
            this.graphic.flipH = this.offDir <= (sbyte)0;
            if ((double)this.alpha > 0.100000001490116)
            {
                foreach (MaterialThing materialThing in Level.CheckRectAll<MaterialThing>(this.topLeft, this.bottomRight))
                {
                    if ((materialThing is PhysicsObject || materialThing is Icicles) && !this._hits.Contains((Thing)materialThing) && materialThing != this._waveOwner && materialThing.owner != this._waveOwner && Duck.GetAssociatedDuck((Thing)materialThing) != this._waveOwner)
                    {
                        if (materialThing.owner != null)
                        {
                            if (this.isServerForObject && !materialThing.isServerForObject)
                                continue;
                        }
                        else if (!this.isServerForObject)
                            continue;
                        if (this._waveOwner != null)
                            Thing.Fondle((Thing)materialThing, this._waveOwner.connection);
                        if (materialThing is Grenade grenade)
                            grenade.PressAction();
                        if (materialThing is PhysicsObject)
                        {
                            materialThing.hSpeed = (float)(((double)this._speed - 3.0) * (double)this.offDir * 1.5 + (double)this.offDir * 4.0) * this.alpha;
                            materialThing.vSpeed = (this._speedv - 4.5f) * this.alpha;
                            materialThing.clip.Add(this._waveOwner as MaterialThing);
                        }
                        if (!materialThing.destroyed && !(materialThing is Equipment))
                            materialThing.Destroy((DestroyType)new DTImpact((Thing)this));
                        this._hits.Add((Thing)materialThing);
                    }
                }
                if (this.isServerForObject)
                {
                    foreach (Door t in Level.CheckRectAll<Door>(this.topLeft, this.bottomRight))
                    {
                        if (this._waveOwner != null)
                            Thing.Fondle((Thing)t, this._waveOwner.connection);
                        if (!t.destroyed)
                            t.Destroy((DestroyType)new DTImpact((Thing)this));
                    }
                    foreach (Window t in Level.CheckRectAll<Window>(this.topLeft, this.bottomRight))
                    {
                        if (this._waveOwner != null)
                            Thing.Fondle((Thing)t, this._waveOwner.connection);
                        if (!t.destroyed)
                            t.Destroy((DestroyType)new DTImpact((Thing)this));
                    }
                }
            }
            if (!this.isServerForObject)
                return;
            this.x += (float)this.offDir * this._speed;
            this.y += this._speedv;
            this.alpha -= this._alphaSub;
            if ((double)this.alpha > 0.0)
                return;
            Level.Remove((Thing)this);
        }
    }
}
