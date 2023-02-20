// Decompiled with JetBrains decompiler
// Type: DuckGame.DamageManager
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DuckGame
{
    public static class DamageManager
    {
        private const int kNumTargets = 256;
        private static List<RenderTarget2D> _targets = new List<RenderTarget2D>();
        private static List<DamageMap> _damageMaps = new List<DamageMap>();
        private static int _nextTarget = 0;
        private static int _nextDamageMap = 0;
        private static int _targetsPerFrame = 1;
        private static List<DamageHit> _hits = new List<DamageHit>();
        private static BlendState _blendState;
        private static BlendState _subtractiveBlend;
        private static SpriteMap _burns;
        private static SpriteMap _bulletHoles;

        public static void Initialize()
        {
            for (int index = 0; index < 256; ++index)
            {
                _targets.Add(new RenderTarget2D(16, 16, true));
                _damageMaps.Add(new DamageMap());
            }
            _blendState = new BlendState
            {
                ColorSourceBlend = Blend.Zero,
                ColorDestinationBlend = Blend.SourceColor,
                ColorBlendFunction = BlendFunction.Add,
                AlphaSourceBlend = Blend.Zero,
                AlphaDestinationBlend = Blend.SourceColor,
                AlphaBlendFunction = BlendFunction.Add
            };
            _subtractiveBlend = new BlendState()
            {
                ColorSourceBlend = Blend.SourceAlpha,
                ColorDestinationBlend = Blend.One,
                ColorBlendFunction = BlendFunction.ReverseSubtract,
                AlphaSourceBlend = Blend.SourceAlpha,
                AlphaDestinationBlend = Blend.One,
                AlphaBlendFunction = BlendFunction.ReverseSubtract
            };
            _burns = new SpriteMap("scratches", 16, 16);
            _burns.CenterOrigin();
            _bulletHoles = new SpriteMap("bulletHoles", 8, 8);
            _bulletHoles.CenterOrigin();
        }

        public static RenderTarget2D Get16x16Target()
        {
            _nextTarget = (_nextTarget + 1) % 256;
            return _targets[_nextTarget];
        }

        public static DamageMap GetDamageMap()
        {
            _nextDamageMap = (_nextDamageMap + 1) % 256;
            _damageMaps[_nextDamageMap].Clear();
            return _damageMaps[_nextDamageMap];
        }

        public static void RegisterHit(Vec2 pt, Thing t, DamageType tp)
        {
            bool flag = false;
            foreach (DamageHit hit in _hits)
            {
                if (hit.thing == t)
                {
                    hit.points.Add(pt);
                    hit.types.Add(tp);
                    flag = true;
                    break;
                }
            }
            if (flag)
                return;
            _hits.Add(new DamageHit()
            {
                thing = t,
                points = {
          pt
        },
                types = {
          tp
        }
            });
        }

        public static void ClearHits() => _hits.Clear();

        public static void Update()
        {
            int targetsPerFrame = _targetsPerFrame;
            int index = 0;
            while (targetsPerFrame > 0 && _hits.Count > 0 && index < _hits.Count)
            {
                DamageHit hit = _hits[index];
                if (hit.thing.graphic.renderTexture == null)
                {
                    hit.thing.graphic = hit.thing.GetEditorImage(0, 0, true, target: Get16x16Target());
                    ++index;
                    --targetsPerFrame;
                }
                else
                {
                    _hits.RemoveAt(index);
                    float num = hit.thing.graphic.width / (float)hit.thing.graphic.width;
                    Camera camera = new Camera(0f, 0f, hit.thing.graphic.width, hit.thing.graphic.height)
                    {
                        position = new Vec2(hit.thing.x - hit.thing.centerx * num, hit.thing.y - hit.thing.centery * num)
                    };
                    Graphics.SetRenderTarget(hit.thing.graphic.renderTexture);
                    DepthStencilState depthStencilState = new DepthStencilState()
                    {
                        StencilEnable = true,
                        StencilFunction = CompareFunction.Equal,
                        StencilPass = StencilOperation.Keep,
                        ReferenceStencil = 1,
                        DepthBufferEnable = false
                    };
                    Graphics.screen.Begin(SpriteSortMode.BackToFront, _blendState, SamplerState.PointClamp, depthStencilState, RasterizerState.CullNone, null, camera.getMatrix());
                    foreach (Vec2 point in hit.points)
                    {
                        _bulletHoles.depth = (Depth)1f;
                        _bulletHoles.x = point.x + Rando.Float(-1f, 1f);
                        _bulletHoles.y = point.y + Rando.Float(-1f, 1f);
                        _bulletHoles.imageIndex = Rando.Int(4);
                        _bulletHoles.Draw();
                    }
                    Graphics.screen.End();
                    Graphics.device.SetRenderTarget(null);
                    --targetsPerFrame;
                }
            }
        }
    }
}
