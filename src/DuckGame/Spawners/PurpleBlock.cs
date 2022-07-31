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
            this.graphic = new Sprite("purpleBlock")
            {
                center = new Vec2(8f, 8f)
            };
            this._scanner = new Sprite("purpleScanner")
            {
                center = new Vec2(4f, 1f),
                alpha = 0.7f,
                depth = (Depth)0.9f
            };
            this._projector = new Sprite("purpleProjector")
            {
                center = new Vec2(8f, 16f),
                alpha = 0.7f,
                depth = (Depth)0.9f
            };
            this._none = new Sprite("none")
            {
                center = new Vec2(8f, 8f),
                alpha = 0.7f
            };
            this._projectorGlitch = new Sprite("projectorGlitch")
            {
                center = new Vec2(8f, 8f),
                alpha = 0.7f,
                depth = (Depth)0.91f
            };
            this.impactThreshold = 0.2f;
            this._placementCost += 4;
            this.editorTooltip = "Makes a copy of a Duck's weapon when used. Spawns a new copy when used again.";
        }

        public override void Initialize()
        {
        }

        public static StoredItem GetStoredItem(Profile p)
        {
            StoredItem storedItem;
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
                    t = (t as WeightBall).collar;
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
            Send.Message(new NMPurpleBoxStoreItem(p.duck, t as PhysicsObject));
        }

        private void BreakHoverBond()
        {
            this._hoverItem.gravMultiplier = 1f;
            this._hoverItem = null;
        }

        public override void Update()
        {
            if (hitWait > 0f)
                this.hitWait -= 0.1f;
            else
                this.hitWait = 0f;
            this._alternate = !this._alternate;
            this._scanner.alpha = (float)(0.4f + (double)this._wave.normalized * 0.6f);
            this._projector.alpha = (float)(0.4f + (double)this._wave.normalized * 0.6f) * this._projectorAlpha;
            this._double = Maths.CountDown(this._double, 0.15f);
            this._glitch = Maths.CountDown(this._glitch, 0.1f);
            if ((double)Rando.Float(1f) < 0.01f)
            {
                this._glitch = 0.3f;
                this._projectorGlitch.xscale = 0.8f + Rando.Float(0.7f);
                this._projectorGlitch.yscale = 0.6f + Rando.Float(0.5f);
                this._projectorGlitch.flipH = (double)Rando.Float(1f) > 0.5f;
            }
            if ((double)Rando.Float(1f) < 0.005f)
            {
                this._glitch = 0.3f;
                this._projectorGlitch.xscale = 0.8f + Rando.Float(0.7f);
                this._projectorGlitch.yscale = 0.6f + Rando.Float(0.5f);
                this._projectorGlitch.flipH = (double)Rando.Float(1f) > 0.5f;
                this._useWave = !this._useWave;
            }
            if (Rando.Float(1f) < 0.008f)
            {
                this._glitch = 0.3f;
                this._projectorGlitch.xscale = 0.8f + Rando.Float(0.7f);
                this._projectorGlitch.yscale = 0.6f + Rando.Float(0.5f);
                this._projectorGlitch.flipH = (double)Rando.Float(1f) > 0.5f;
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
                    if (num < 24f)
                        this._hoverItem = holdable;
                }
            }
            else if (Math.Abs(this._hoverItem.hSpeed) + Math.Abs(this._hoverItem.vSpeed) > 2f || (this._hoverItem.position - this.position).length > 25f)
            {
                this.BreakHoverBond();
            }
            else
            {
                this._hoverItem.position = Lerp.Vec2Smooth(this._hoverItem.position, this.position + new Vec2(0f, (float)(-12f - _hoverItem.collisionSize.y / 2f + (double)(float)this._projectionWave * 2f)), 0.2f);
                this._hoverItem.vSpeed = 0f;
                this._hoverItem.gravMultiplier = 0f;
            }
            foreach (Duck duck in this._level.things[typeof(Duck)])
            {
                if (!duck.dead)
                {
                    vec2 = duck.position - this.position;
                    if ((double)vec2.length < 64f)
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
                else if (this._close.Count > 1 && index == this._closeIndex && _closeWait <= 0f)
                {
                    this._closeIndex = (this._closeIndex + 1) % this._close.Count;
                    this._closeWait = 1f;
                    this._glitch = 0.3f;
                    this._projectorGlitch.xscale = 0.8f + Rando.Float(0.7f);
                    this._projectorGlitch.yscale = 0.6f + Rando.Float(0.5f);
                    this._projectorGlitch.flipH = Rando.Float(1f) > 0.5f;
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
                    this._projectorGlitch.flipH = Rando.Float(1f) > 0.5f;
                    this._useWave = !this._useWave;
                    this._double = 0.6f + Rando.Float(0.6f);
                    this._closeGlitch = true;
                }
                this._projectorAlpha = Maths.CountDown(this._projectorAlpha, 0.1f);
                this._currentProjection = null;
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
            if (_double > 0.0)
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
                    this._none.alpha = (0.2f + this._projectionFlashWave.normalized * 0.2f + _glitch * 1f) * this._projectorAlpha;
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
                this._none.alpha = (float)(0.2f + (double)this._projectionFlashWave.normalized * 0.2f + _glitch * 1f) * this._projectorAlpha;
                Graphics.Draw(this._none, this.x, this.y - 16f - num, this.depth + 5);
            }
            if (_glitch <= 0f)
                return;
            Graphics.Draw(this._projectorGlitch, this.x, this.y - 16f);
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (from == ImpactedFrom.Bottom && hitWait == 0f && with.isServerForObject)
                with.Fondle(this);
            if (!this.isServerForObject || !with.isServerForObject || from != ImpactedFrom.Bottom || hitWait != 0f)
                return;
            this.hitWait = 1f;
            switch (with)
            {
                case Holdable holdable when holdable.lastThrownBy != null || holdable is RagdollPart && !Network.isActive:
                    Duck lastThrownBy = holdable.lastThrownBy as Duck;
                    if (holdable is RagdollPart)
                        break;
                    if (lastThrownBy != null)
                        PurpleBlock.StoreItem(lastThrownBy.profile, with);
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
                            Send.Message(new NMPurpleBoxServed(pDuck, this));
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
                    PurpleBlock.StoreItem(pDuck.profile, holdObject);
                    break;
            }
        }
    }
}
