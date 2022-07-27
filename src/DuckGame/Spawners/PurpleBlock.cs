// Decompiled with JetBrains decompiler
// Type: DuckGame.PurpleBlock
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Spawns")]
    [BaggedProperty("previewPriority", true)]
    public class PurpleBlock : ItemBox, IDrawToDifferentLayers
    {
        private Sprite _scanner;
        private Sprite _projector;
        private Sprite _none;
        private Sprite _projectorGlitch;
        private Thing _currentProjection;
        private SinWave _wave = (SinWave)1f;
        private SinWave _projectionWave = (SinWave)0.04f;
        private SinWave _projectionWave2 = (SinWave)0.05f;
        private SinWave _projectionFlashWave = (SinWave)0.8f;
        private bool _useWave;
        private bool _alternate;
        private float _double;
        private float _glitch;
        public static Material _grayscale = new Material("Shaders/greyscale");
        public List<Profile> _served = new List<Profile>();
        private List<Profile> _close = new List<Profile>();
        private float _closeWait;
        private int _closeIndex;
        private float _projectorAlpha;
        private bool _closeGlitch;
        private Holdable _hoverItem;
        private float hitWait;

        public static void Reset() => PurpleBlock._storedItems.Clear();

        private static Dictionary<Profile, StoredItem> _storedItems => Level.core._storedItems;

        public PurpleBlock(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite("purpleBlock");
            this.graphic.center = new Vec2(8f, 8f);
            this._scanner = new Sprite("purpleScanner");
            this._scanner.center = new Vec2(4f, 1f);
            this._scanner.alpha = 0.7f;
            this._scanner.depth = (Depth)0.9f;
            this._projector = new Sprite("purpleProjector");
            this._projector.center = new Vec2(8f, 16f);
            this._projector.alpha = 0.7f;
            this._projector.depth = (Depth)0.9f;
            this._none = new Sprite("none");
            this._none.center = new Vec2(8f, 8f);
            this._none.alpha = 0.7f;
            this._projectorGlitch = new Sprite("projectorGlitch");
            this._projectorGlitch.center = new Vec2(8f, 8f);
            this._projectorGlitch.alpha = 0.7f;
            this._projectorGlitch.depth = (Depth)0.91f;
            this.impactThreshold = 0.2f;
            this._placementCost += 4;
            this.editorTooltip = "Makes a copy of a Duck's weapon when used. Spawns a new copy when used again.";
        }

        public override void Initialize()
        {
        }

        public static StoredItem GetStoredItem(Profile p)
        {
            StoredItem storedItem = (StoredItem)null;
            if (!PurpleBlock._storedItems.TryGetValue(p, out storedItem))
                storedItem = PurpleBlock._storedItems[p] = new StoredItem();
            return storedItem;
        }

        public static void StoreItem(Profile p, Thing t)
        {
            switch (t)
            {
                case RagdollPart _:
                case TrappedDuck _:
                case Holdable _ when !(t as Holdable).canStore:
                    return;
                case WeightBall _:
                    t = (Thing)(t as WeightBall).collar;
                    break;
            }
            StoredItem storedItem = PurpleBlock.GetStoredItem(p);
            t.GetType();
            try
            {
                storedItem.serializedData = t.Serialize();
                storedItem.thing = Thing.LoadThing(storedItem.serializedData);
            }
            catch (Exception)
            {
                PurpleBlock._storedItems.Clear();
            }
            SFX.Play("scanBeep");
            if (!Network.isActive || p.connection != DuckNetwork.localConnection || p.duck == null)
                return;
            Send.Message((NetMessage)new NMPurpleBoxStoreItem(p.duck, t as PhysicsObject));
        }

        private void BreakHoverBond()
        {
            this._hoverItem.gravMultiplier = 1f;
            this._hoverItem = (Holdable)null;
        }

        public override void Update()
        {
            if ((double)this.hitWait > 0.0)
                this.hitWait -= 0.1f;
            else
                this.hitWait = 0.0f;
            this._alternate = !this._alternate;
            this._scanner.alpha = (float)(0.400000005960464 + (double)this._wave.normalized * 0.600000023841858);
            this._projector.alpha = (float)(0.400000005960464 + (double)this._wave.normalized * 0.600000023841858) * this._projectorAlpha;
            this._double = Maths.CountDown(this._double, 0.15f);
            this._glitch = Maths.CountDown(this._glitch, 0.1f);
            if ((double)Rando.Float(1f) < 0.00999999977648258)
            {
                this._glitch = 0.3f;
                this._projectorGlitch.xscale = 0.8f + Rando.Float(0.7f);
                this._projectorGlitch.yscale = 0.6f + Rando.Float(0.5f);
                this._projectorGlitch.flipH = (double)Rando.Float(1f) > 0.5;
            }
            if ((double)Rando.Float(1f) < 0.00499999988824129)
            {
                this._glitch = 0.3f;
                this._projectorGlitch.xscale = 0.8f + Rando.Float(0.7f);
                this._projectorGlitch.yscale = 0.6f + Rando.Float(0.5f);
                this._projectorGlitch.flipH = (double)Rando.Float(1f) > 0.5;
                this._useWave = !this._useWave;
            }
            if ((double)Rando.Float(1f) < 0.00800000037997961)
            {
                this._glitch = 0.3f;
                this._projectorGlitch.xscale = 0.8f + Rando.Float(0.7f);
                this._projectorGlitch.yscale = 0.6f + Rando.Float(0.5f);
                this._projectorGlitch.flipH = (double)Rando.Float(1f) > 0.5;
                this._useWave = !this._useWave;
                this._double = 0.6f + Rando.Float(0.6f);
            }
            this._close.Clear();
            if (this._hoverItem != null && this._hoverItem.owner != null)
                this.BreakHoverBond();
            Vec2 vec2;
            if (this._hoverItem == null)
            {
                Holdable holdable = Level.Nearest<Holdable>(this.x, this.y);
                if (holdable != null && holdable.owner == null && holdable != null && holdable.canPickUp && (double)holdable.bottom <= (double)this.top && (double)Math.Abs(holdable.hSpeed) + (double)Math.Abs(holdable.vSpeed) < 2.0)
                {
                    float num = 999f;
                    if (holdable != null)
                    {
                        vec2 = this.position - holdable.position;
                        num = vec2.length;
                    }
                    if ((double)num < 24.0)
                        this._hoverItem = holdable;
                }
            }
            else if ((double)Math.Abs(this._hoverItem.hSpeed) + (double)Math.Abs(this._hoverItem.vSpeed) > 2.0 || (double)(this._hoverItem.position - this.position).length > 25.0)
            {
                this.BreakHoverBond();
            }
            else
            {
                this._hoverItem.position = Lerp.Vec2Smooth(this._hoverItem.position, this.position + new Vec2(0.0f, (float)(-12.0 - (double)this._hoverItem.collisionSize.y / 2.0 + (double)(float)this._projectionWave * 2.0)), 0.2f);
                this._hoverItem.vSpeed = 0.0f;
                this._hoverItem.gravMultiplier = 0.0f;
            }
            foreach (Duck duck in this._level.things[typeof(Duck)])
            {
                if (!duck.dead)
                {
                    vec2 = duck.position - this.position;
                    if ((double)vec2.length < 64.0)
                    {
                        this._close.Add(duck.profile);
                        this._closeGlitch = false;
                    }
                }
            }
            this._closeWait = Maths.CountDown(this._closeWait, 0.05f);
            for (int index = 0; index < this._close.Count; ++index)
            {
                if (this._close.Count == 1)
                    this._closeIndex = 0;
                else if (this._close.Count > 1 && index == this._closeIndex && (double)this._closeWait <= 0.0)
                {
                    this._closeIndex = (this._closeIndex + 1) % this._close.Count;
                    this._closeWait = 1f;
                    this._glitch = 0.3f;
                    this._projectorGlitch.xscale = 0.8f + Rando.Float(0.7f);
                    this._projectorGlitch.yscale = 0.6f + Rando.Float(0.5f);
                    this._projectorGlitch.flipH = (double)Rando.Float(1f) > 0.5;
                    this._useWave = !this._useWave;
                    this._double = 0.6f + Rando.Float(0.6f);
                    break;
                }
            }
            if (this._closeIndex >= this._close.Count)
                this._closeIndex = 0;
            if (this._close.Count == 0)
            {
                if (!this._closeGlitch)
                {
                    this._closeWait = 1f;
                    this._glitch = 0.3f;
                    this._projectorGlitch.xscale = 0.8f + Rando.Float(0.7f);
                    this._projectorGlitch.yscale = 0.6f + Rando.Float(0.5f);
                    this._projectorGlitch.flipH = (double)Rando.Float(1f) > 0.5;
                    this._useWave = !this._useWave;
                    this._double = 0.6f + Rando.Float(0.6f);
                    this._closeGlitch = true;
                }
                this._projectorAlpha = Maths.CountDown(this._projectorAlpha, 0.1f);
                this._currentProjection = (Thing)null;
            }
            else
            {
                this._currentProjection = PurpleBlock.GetStoredItem(this._close[this._closeIndex]).thing;
                this._projectorAlpha = Maths.CountUp(this._projectorAlpha, 0.1f);
            }
            this._projectorGlitch.alpha = this._glitch * this._projectorAlpha;
            base.Update();
        }

        public override void Draw() => base.Draw();

        public void OnDrawLayer(Layer pLayer)
        {
            if (pLayer != Layer.Background)
                return;
            if (this._alternate)
                Graphics.Draw(this._scanner, this.x, this.y + 9f);
            if (!this._alternate)
                Graphics.Draw(this._projector, this.x, this.y - 8f);
            float num = (float)(this._useWave ? this._projectionWave : this._projectionWave2);
            if ((double)this._double > 0.0)
            {
                if (this._currentProjection != null)
                {
                    Duck.renderingIcon = true;
                    Material material = Graphics.material;
                    Graphics.material = PurpleBlock._grayscale;
                    this._currentProjection.depth = this.depth - 5;
                    this._currentProjection.x = this.x - this._double * 2f;
                    this._currentProjection.y = this.y - 16f - num;
                    this._currentProjection.Draw();
                    this._currentProjection.x = this.x + this._double * 2f;
                    this._currentProjection.y = this.y - 16f - num;
                    this._currentProjection.Draw();
                    Graphics.material = material;
                    Duck.renderingIcon = false;
                }
                else
                {
                    this._none.alpha = (float)(0.200000002980232 + (double)this._projectionFlashWave.normalized * 0.200000002980232 + (double)this._glitch * 1.0) * this._projectorAlpha;
                    Graphics.Draw(this._none, this.x - this._double * 2f, this.y - 16f - num);
                    Graphics.Draw(this._none, this.x + this._double * 2f, this.y - 16f - num);
                }
            }
            else if (this._currentProjection != null)
            {
                Duck.renderingIcon = true;
                Material material = Graphics.material;
                Graphics.material = PurpleBlock._grayscale;
                this._currentProjection.depth = this.depth - 5;
                this._currentProjection.x = this.x;
                this._currentProjection.y = this.y - 16f - num;
                this._currentProjection.Draw();
                Graphics.material = material;
                Duck.renderingIcon = false;
            }
            else
                Graphics.Draw(this._none, this.x, this.y - 16f - num);
            if (this._currentProjection != null && this._served.Contains(this._close[this._closeIndex]))
            {
                this._none.alpha = (float)(0.200000002980232 + (double)this._projectionFlashWave.normalized * 0.200000002980232 + (double)this._glitch * 1.0) * this._projectorAlpha;
                Graphics.Draw(this._none, this.x, this.y - 16f - num, this.depth + 5);
            }
            if ((double)this._glitch <= 0.0)
                return;
            Graphics.Draw(this._projectorGlitch, this.x, this.y - 16f);
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (from == ImpactedFrom.Bottom && (double)this.hitWait == 0.0 && with.isServerForObject)
                with.Fondle((Thing)this);
            if (!this.isServerForObject || !with.isServerForObject || from != ImpactedFrom.Bottom || (double)this.hitWait != 0.0)
                return;
            this.hitWait = 1f;
            switch (with)
            {
                case Holdable holdable when holdable.lastThrownBy != null || holdable is RagdollPart && !Network.isActive:
                    Duck lastThrownBy = holdable.lastThrownBy as Duck;
                    if (holdable is RagdollPart)
                        break;
                    if (lastThrownBy != null)
                        PurpleBlock.StoreItem(lastThrownBy.profile, (Thing)with);
                    this.Bounce();
                    break;
                case Duck pDuck:
                    RumbleManager.AddRumbleEvent(pDuck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.None));
                    StoredItem storedItem = PurpleBlock.GetStoredItem(pDuck.profile);
                    if (storedItem.thing != null && !this._served.Contains(pDuck.profile))
                    {
                        this.containContext = storedItem.thing as PhysicsObject;
                        storedItem.thing = Thing.LoadThing(storedItem.serializedData);
                        this._hit = false;
                        this.Pop();
                        this._served.Add(pDuck.profile);
                        if (Network.isActive && pDuck.isServerForObject)
                            Send.Message((NetMessage)new NMPurpleBoxServed(pDuck, this));
                    }
                    else
                    {
                        if (this._served.Contains(pDuck.profile))
                            SFX.PlaySynchronized("scanFail");
                        this.Bounce();
                    }
                    if (pDuck.holdObject == null)
                        break;
                    Holdable holdObject = pDuck.holdObject;
                    if (holdObject == null)
                        break;
                    PurpleBlock.StoreItem(pDuck.profile, (Thing)holdObject);
                    break;
            }
        }
    }
}
