// Decompiled with JetBrains decompiler
// Type: DuckGame.PurpleBlock
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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

        public static void Reset() => _storedItems.Clear();

        private static Dictionary<Profile, StoredItem> _storedItems => Level.core._storedItems;

        public PurpleBlock(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("purpleBlock")
            {
                center = new Vec2(8f, 8f)
            };
            _scanner = new Sprite("purpleScanner")
            {
                center = new Vec2(4f, 1f),
                alpha = 0.7f,
                depth = (Depth)0.9f
            };
            _projector = new Sprite("purpleProjector")
            {
                center = new Vec2(8f, 16f),
                alpha = 0.7f,
                depth = (Depth)0.9f
            };
            _none = new Sprite("none")
            {
                center = new Vec2(8f, 8f),
                alpha = 0.7f
            };
            _projectorGlitch = new Sprite("projectorGlitch")
            {
                center = new Vec2(8f, 8f),
                alpha = 0.7f,
                depth = (Depth)0.91f
            };
            impactThreshold = 0.2f;
            _placementCost += 4;
            editorTooltip = "Makes a copy of a Duck's weapon when used. Spawns a new copy when used again.";
        }

        public override void Initialize()
        {
        }

        public static StoredItem GetStoredItem(Profile p)
        {
            StoredItem storedItem;
            if (!_storedItems.TryGetValue(p, out storedItem))
                storedItem = _storedItems[p] = new StoredItem();
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
            StoredItem storedItem = GetStoredItem(p);
            t.GetType();
            try
            {
                storedItem.serializedData = t.Serialize();
                storedItem.thing = LoadThing(storedItem.serializedData);
            }
            catch (Exception)
            {
                _storedItems.Clear();
            }
            SFX.Play("scanBeep");
            if (!Network.isActive || p.connection != DuckNetwork.localConnection || p.duck == null)
                return;
            Send.Message(new NMPurpleBoxStoreItem(p.duck, t as PhysicsObject));
        }

        private void BreakHoverBond()
        {
            _hoverItem.gravMultiplier = 1f;
            _hoverItem = null;
        }

        public override void Update()
        {
            if (hitWait > 0f)
                hitWait -= 0.1f;
            else
                hitWait = 0f;
            _alternate = !_alternate;
            _scanner.alpha = (float)(0.4f + _wave.normalized * 0.6f);
            _projector.alpha = (float)(0.4f + _wave.normalized * 0.6f) * _projectorAlpha;
            _double = Maths.CountDown(_double, 0.15f);
            _glitch = Maths.CountDown(_glitch, 0.1f);
            if (Rando.Float(1f) < 0.01f)
            {
                _glitch = 0.3f;
                _projectorGlitch.xscale = 0.8f + Rando.Float(0.7f);
                _projectorGlitch.yscale = 0.6f + Rando.Float(0.5f);
                _projectorGlitch.flipH = Rando.Float(1f) > 0.5f;
            }
            if (Rando.Float(1f) < 0.005f)
            {
                _glitch = 0.3f;
                _projectorGlitch.xscale = 0.8f + Rando.Float(0.7f);
                _projectorGlitch.yscale = 0.6f + Rando.Float(0.5f);
                _projectorGlitch.flipH = Rando.Float(1f) > 0.5f;
                _useWave = !_useWave;
            }
            if (Rando.Float(1f) < 0.008f)
            {
                _glitch = 0.3f;
                _projectorGlitch.xscale = 0.8f + Rando.Float(0.7f);
                _projectorGlitch.yscale = 0.6f + Rando.Float(0.5f);
                _projectorGlitch.flipH = Rando.Float(1f) > 0.5f;
                _useWave = !_useWave;
                _double = 0.6f + Rando.Float(0.6f);
            }
            _close.Clear();
            if (_hoverItem != null && _hoverItem.owner != null)
                BreakHoverBond();
            Vec2 vec2;
            if (_hoverItem == null)
            {
                Holdable holdable = Level.Nearest<Holdable>(position, 24f);
                if (holdable != null && holdable.owner == null && holdable != null && holdable.canPickUp && holdable.bottom <= top && Math.Abs(holdable.hSpeed) + Math.Abs(holdable.vSpeed) < 2.0)
                {
                    float num = 999f;
                    if (holdable != null)
                    {
                        vec2 = position - holdable.position;
                        num = vec2.length;
                    }
                    if (num < 24f)
                        _hoverItem = holdable;
                }
            }
            else if (Math.Abs(_hoverItem.hSpeed) + Math.Abs(_hoverItem.vSpeed) > 2f || (_hoverItem.position - position).length > 25f)
            {
                BreakHoverBond();
            }
            else
            {
                _hoverItem.position = Lerp.Vec2Smooth(_hoverItem.position, position + new Vec2(0f, (float)(-12f - _hoverItem.collisionSize.y / 2f + (float)_projectionWave * 2f)), 0.2f);
                _hoverItem.vSpeed = 0f;
                _hoverItem.gravMultiplier = 0f;
            }
            foreach (Duck duck in _level.things[typeof(Duck)])
            {
                if (!duck.dead)
                {
                    vec2 = duck.position - position;
                    if (vec2.length < 64f)
                    {
                        _close.Add(duck.profile);
                        _closeGlitch = false;
                    }
                }
            }
            _closeWait = Maths.CountDown(_closeWait, 0.05f);
            for (int index = 0; index < _close.Count; ++index)
            {
                if (_close.Count == 1)
                    _closeIndex = 0;
                else if (_close.Count > 1 && index == _closeIndex && _closeWait <= 0f)
                {
                    _closeIndex = (_closeIndex + 1) % _close.Count;
                    _closeWait = 1f;
                    _glitch = 0.3f;
                    _projectorGlitch.xscale = 0.8f + Rando.Float(0.7f);
                    _projectorGlitch.yscale = 0.6f + Rando.Float(0.5f);
                    _projectorGlitch.flipH = Rando.Float(1f) > 0.5f;
                    _useWave = !_useWave;
                    _double = 0.6f + Rando.Float(0.6f);
                    break;
                }
            }
            if (_closeIndex >= _close.Count)
                _closeIndex = 0;
            if (_close.Count == 0)
            {
                if (!_closeGlitch)
                {
                    _closeWait = 1f;
                    _glitch = 0.3f;
                    _projectorGlitch.xscale = 0.8f + Rando.Float(0.7f);
                    _projectorGlitch.yscale = 0.6f + Rando.Float(0.5f);
                    _projectorGlitch.flipH = Rando.Float(1f) > 0.5f;
                    _useWave = !_useWave;
                    _double = 0.6f + Rando.Float(0.6f);
                    _closeGlitch = true;
                }
                _projectorAlpha = Maths.CountDown(_projectorAlpha, 0.1f);
                _currentProjection = null;
            }
            else
            {
                _currentProjection = GetStoredItem(_close[_closeIndex]).thing;
                _projectorAlpha = Maths.CountUp(_projectorAlpha, 0.1f);
            }
            _projectorGlitch.alpha = _glitch * _projectorAlpha;
            base.Update();
        }

        public override void Draw() => base.Draw();

        public void OnDrawLayer(Layer pLayer)
        {
            if (pLayer != Layer.Background)
                return;
            if (_alternate)
                Graphics.Draw(_scanner, x, y + 9f);
            if (!_alternate)
                Graphics.Draw(_projector, x, y - 8f);
            float num = (float)(_useWave ? _projectionWave : _projectionWave2);
            if (_double > 0.0)
            {
                if (_currentProjection != null)
                {
                    Duck.renderingIcon = true;
                    Material material = Graphics.material;
                    Graphics.material = _grayscale;
                    _currentProjection.depth = depth - 5;
                    _currentProjection.x = x - _double * 2f;
                    _currentProjection.y = y - 16f - num;
                    _currentProjection.Draw();
                    _currentProjection.x = x + _double * 2f;
                    _currentProjection.y = y - 16f - num;
                    _currentProjection.Draw();
                    Graphics.material = material;
                    Duck.renderingIcon = false;
                }
                else
                {
                    _none.alpha = (0.2f + _projectionFlashWave.normalized * 0.2f + _glitch * 1f) * _projectorAlpha;
                    Graphics.Draw(_none, x - _double * 2f, y - 16f - num);
                    Graphics.Draw(_none, x + _double * 2f, y - 16f - num);
                }
            }
            else if (_currentProjection != null)
            {
                Duck.renderingIcon = true;
                Material material = Graphics.material;
                Graphics.material = _grayscale;
                _currentProjection.depth = depth - 5;
                _currentProjection.x = x;
                _currentProjection.y = y - 16f - num;
                _currentProjection.Draw();
                Graphics.material = material;
                Duck.renderingIcon = false;
            }
            else
                Graphics.Draw(_none, x, y - 16f - num);
            if (_currentProjection != null && _served.Contains(_close[_closeIndex]))
            {
                _none.alpha = (float)(0.2f + _projectionFlashWave.normalized * 0.2f + _glitch * 1f) * _projectorAlpha;
                Graphics.Draw(_none, x, y - 16f - num, depth + 5);
            }
            if (_glitch <= 0f)
                return;
            Graphics.Draw(_projectorGlitch, x, y - 16f);
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (from == ImpactedFrom.Bottom && hitWait == 0f && with.isServerForObject)
                with.Fondle(this);
            if (!isServerForObject || !with.isServerForObject || from != ImpactedFrom.Bottom || hitWait != 0f)
                return;
            hitWait = 1f;
            switch (with)
            {
                case Holdable holdable when holdable.lastThrownBy != null || holdable is RagdollPart && !Network.isActive:
                    Duck lastThrownBy = holdable.lastThrownBy as Duck;
                    if (holdable is RagdollPart)
                        break;
                    if (lastThrownBy != null)
                        StoreItem(lastThrownBy.profile, with);
                    Bounce();
                    break;
                case Duck pDuck:
                    RumbleManager.AddRumbleEvent(pDuck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.None));
                    StoredItem storedItem = GetStoredItem(pDuck.profile);
                    if (storedItem.thing != null && !_served.Contains(pDuck.profile))
                    {
                        containContext = storedItem.thing as PhysicsObject;
                        storedItem.thing = LoadThing(storedItem.serializedData);
                        _hit = false;
                        Pop();
                        _served.Add(pDuck.profile);
                        if (Network.isActive && pDuck.isServerForObject)
                            Send.Message(new NMPurpleBoxServed(pDuck, this));
                    }
                    else
                    {
                        if (_served.Contains(pDuck.profile))
                            SFX.PlaySynchronized("scanFail");
                        Bounce();
                    }
                    if (pDuck.holdObject == null)
                        break;
                    Holdable holdObject = pDuck.holdObject;
                    if (holdObject == null)
                        break;
                    StoreItem(pDuck.profile, holdObject);
                    break;
            }
        }
    }
}
