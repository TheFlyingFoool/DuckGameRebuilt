// Decompiled with JetBrains decompiler
// Type: DuckGame.DamageManager
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
                DamageManager._targets.Add(new RenderTarget2D(16, 16, true));
                DamageManager._damageMaps.Add(new DamageMap());
            }
            DamageManager._blendState = new BlendState
            {
                ColorSourceBlend = Blend.Zero,
                ColorDestinationBlend = Blend.SourceColor,
                ColorBlendFunction = BlendFunction.Add,
                AlphaSourceBlend = Blend.Zero,
                AlphaDestinationBlend = Blend.SourceColor,
                AlphaBlendFunction = BlendFunction.Add
            };
            DamageManager._subtractiveBlend = new BlendState()
            {
                ColorSourceBlend = Blend.SourceAlpha,
                ColorDestinationBlend = Blend.One,
                ColorBlendFunction = BlendFunction.ReverseSubtract,
                AlphaSourceBlend = Blend.SourceAlpha,
                AlphaDestinationBlend = Blend.One,
                AlphaBlendFunction = BlendFunction.ReverseSubtract
            };
            DamageManager._burns = new SpriteMap("scratches", 16, 16);
            DamageManager._burns.CenterOrigin();
            DamageManager._bulletHoles = new SpriteMap("bulletHoles", 8, 8);
            DamageManager._bulletHoles.CenterOrigin();
        }

        public static RenderTarget2D Get16x16Target()
        {
            DamageManager._nextTarget = (DamageManager._nextTarget + 1) % 256;
            return DamageManager._targets[DamageManager._nextTarget];
        }

        public static DamageMap GetDamageMap()
        {
            DamageManager._nextDamageMap = (DamageManager._nextDamageMap + 1) % 256;
            DamageManager._damageMaps[DamageManager._nextDamageMap].Clear();
            return DamageManager._damageMaps[DamageManager._nextDamageMap];
        }

        public static void RegisterHit(Vec2 pt, Thing t, DamageType tp)
        {
            bool flag = false;
            foreach (DamageHit hit in DamageManager._hits)
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
            DamageManager._hits.Add(new DamageHit()
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

        public static void ClearHits() => DamageManager._hits.Clear();

        public static void Update()
        {
            int targetsPerFrame = DamageManager._targetsPerFrame;
            int index = 0;
            while (targetsPerFrame > 0 && DamageManager._hits.Count > 0 && index < DamageManager._hits.Count)
            {
                DamageHit hit = DamageManager._hits[index];
                if (hit.thing.graphic.renderTexture == null)
                {
                    hit.thing.graphic = hit.thing.GetEditorImage(0, 0, true, target: DamageManager.Get16x16Target());
                    ++index;
                    --targetsPerFrame;
                }
                else
                {
                    DamageManager._hits.RemoveAt(index);
                    float num = hit.thing.graphic.width / (float)hit.thing.graphic.width;
                    Camera camera = new Camera(0.0f, 0.0f, hit.thing.graphic.width, hit.thing.graphic.height)
                    {
                        position = new Vec2(hit.thing.x - hit.thing.centerx * num, hit.thing.y - hit.thing.centery * num)
                    };
                    DuckGame.Graphics.SetRenderTarget(hit.thing.graphic.renderTexture);
                    DepthStencilState depthStencilState = new DepthStencilState()
                    {
                        StencilEnable = true,
                        StencilFunction = CompareFunction.Equal,
                        StencilPass = StencilOperation.Keep,
                        ReferenceStencil = 1,
                        DepthBufferEnable = false
                    };
                    DuckGame.Graphics.screen.Begin(SpriteSortMode.BackToFront, DamageManager._blendState, SamplerState.PointClamp, depthStencilState, RasterizerState.CullNone, null, camera.getMatrix());
                    foreach (Vec2 point in hit.points)
                    {
                        DamageManager._bulletHoles.depth = (Depth)1f;
                        DamageManager._bulletHoles.x = point.x + Rando.Float(-1f, 1f);
                        DamageManager._bulletHoles.y = point.y + Rando.Float(-1f, 1f);
                        DamageManager._bulletHoles.imageIndex = Rando.Int(4);
                        DamageManager._bulletHoles.Draw();
                    }
                    DuckGame.Graphics.screen.End();
                    DuckGame.Graphics.device.SetRenderTarget(null);
                    --targetsPerFrame;
                }
            }
        }
    }
}
