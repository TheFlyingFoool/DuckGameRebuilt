// Decompiled with JetBrains decompiler
// Type: DuckGame.LayerCore
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class LayerCore
    {
        private const string NameParallax = "PARALLAX";
        private const string NameVirtual = "VIRTUAL";
        private const string NameBackground = "BACKGROUND";
        private const string NameGame = "GAME";
        private const string NameBlocks = "BLOCKS";
        private const string NameGlow = "GLOW";
        private const string NameLighting = "LIGHTING";
        private const string NameForeground = "FOREGROUND";
        private const string NameHUD = "HUD";
        private const string NameConsole = "CONSOLE";
        public bool doVirtualEffect;
        public Layer _parallax;
        public Layer _virtual;
        public Layer _background;
        public Layer _game;
        public Layer _blocks;
        public Layer _glow;
        public Layer _lighting;
        public Layer _foreground;
        public Layer _hud;
        public Layer _console;
        public List<Layer> _layers = new List<Layer>();
        public List<Layer> _extraLayers = new List<Layer>();
        public List<Layer> _hybridList = new List<Layer>();
        public MTEffect _basicEffectFadeAdd;
        public MTEffect _basicEffectAdd;
        public MTEffect _basicEffectFade;
        public MTEffect _basicEffect;
        public MTEffect _basicWireframeEffect;
        public MTEffect _basicWireframeEffectTex;
        public MTEffect _itemSpawnEffect;
        public bool basicWireframeTex;
        private LayerCore.MapEntry[] _layerMap;
        private int _lastDrawIndexCount;

        public bool allVisible
        {
            set
            {
                foreach (Layer layer in _layers)
                    layer.visible = value;
                foreach (Layer extraLayer in _extraLayers)
                    extraLayer.visible = value;
            }
        }

        public MTEffect basicWireframeEffect => !basicWireframeTex ? _basicWireframeEffect : _basicWireframeEffectTex;

        public MTEffect basicLayerEffect => _basicEffectFadeAdd;

        public bool IsBasicLayerEffect(MTEffect e)
        {
            if (e == null)
                return false;
            return e.effectIndex == _basicEffect.effectIndex || e.effectIndex == _basicEffectAdd.effectIndex || e.effectIndex == _basicEffectFade.effectIndex || e.effectIndex == _basicEffectFadeAdd.effectIndex;
        }

        public void InitializeLayers()
        {
            Layer.lightingTwoPointOh = false;
            _layers.Add(new Layer("PARALLAX", 100));
            _parallax = _layers[_layers.Count - 1];
            _parallax.allowTallAspect = true;
            _parallax.aspectReliesOnGameLayer = true;
            _layers.Add(new Layer("VIRTUAL", 95));
            _virtual = _layers[_layers.Count - 1];
            _virtual.allowTallAspect = true;
            _virtual.aspectReliesOnGameLayer = true;
            _layers.Add(new Layer("BACKGROUND", 90));
            _background = _layers[_layers.Count - 1];
            _background.enableCulling = true;
            _background.allowTallAspect = true;
            _layers.Add(new Layer("GAME"));
            _game = _layers[_layers.Count - 1];
            _game.enableCulling = false;
            _game.allowTallAspect = true;
            _layers.Add(new Layer("BLOCKS", -18));
            _blocks = _layers[_layers.Count - 1];
            _blocks.enableCulling = true;
            _blocks.allowTallAspect = true;
            _layers.Add(new Layer("FOREGROUND", -19));
            _foreground = _layers[_layers.Count - 1];
            _foreground.allowTallAspect = true;
            _layers.Add(new Layer("HUD", -90));
            _hud = _layers[_layers.Count - 1];
            _layers.Add(new Layer("CONSOLE", -100, new Camera(Resolution.current.x / 2, Resolution.current.y / 2)));
            _console = _layers[_layers.Count - 1];
            _console.allowTallAspect = true;
            _layers.Add(new Layer("GLOW", -21));
            _glow = _layers[_layers.Count - 1];
            _glow.allowTallAspect = true;
            _layers.Add(new Layer("LIGHTING", Layer.lightingTwoPointOh ? -20 : -10, targetLayer: true, targetSize: new Vec2(Graphics.width, Graphics.height)));
            _lighting = _layers[_layers.Count - 1];
            _lighting.allowTallAspect = true;
            BlendState blendState1 = new BlendState
            {
                ColorSourceBlend = Blend.Zero,
                ColorDestinationBlend = Blend.SourceColor,
                ColorBlendFunction = BlendFunction.Add,
                AlphaSourceBlend = Blend.Zero,
                AlphaDestinationBlend = Blend.SourceAlpha,
                AlphaBlendFunction = BlendFunction.Add
            };
            _glow.blend = BlendState.Additive;
            _lighting.targetBlend = BlendState.Additive;
            _lighting.targetBlend = new BlendState()
            {
                ColorSourceBlend = Blend.One,
                ColorDestinationBlend = Blend.One,
                ColorBlendFunction = BlendFunction.Add,
                AlphaSourceBlend = Blend.One,
                AlphaDestinationBlend = Blend.One,
                AlphaBlendFunction = BlendFunction.Add
            };
            _lighting.blend = blendState1;
            _lighting.targetClearColor = new Color(120, 120, 120, (int)byte.MaxValue);
            _lighting.targetDepthStencil = DepthStencilState.None;
            _lighting.flashAddClearInfluence = 1f;
            BlendState blendState2 = new BlendState()
            {
                ColorBlendFunction = BlendFunction.Add,
                ColorSourceBlend = Blend.DestinationColor,
                ColorDestinationBlend = Blend.Zero,
                AlphaBlendFunction = BlendFunction.Add,
                AlphaSourceBlend = Blend.DestinationColor,
                AlphaDestinationBlend = Blend.Zero
            };
            _layers = _layers.OrderBy<Layer, int>(l => -l.depth).ToList<Layer>();
            Layer.Parallax.flashAddInfluence = 1f;
            Layer.HUD.flashAddInfluence = 1f;
            if (_basicEffect == null)
            {
                _itemSpawnEffect = Content.Load<MTEffect>("Shaders/wireframeTex");
                _basicWireframeEffect = Content.Load<MTEffect>("Shaders/wireframe");
                _basicWireframeEffectTex = Content.Load<MTEffect>("Shaders/wireframeTex");
                _basicEffect = Content.Load<MTEffect>("Shaders/basic");
                _basicEffect.effect.Name = "Shaders/basic";
                _basicEffectFade = Content.Load<MTEffect>("Shaders/basicFade");
                _basicEffectFade.effect.Name = "Shaders/basicFade";
                _basicEffectAdd = Content.Load<MTEffect>("Shaders/basicAdd");
                _basicEffectAdd.effect.Name = "Shaders/basicAdd";
                _basicEffectFadeAdd = Content.Load<MTEffect>("Shaders/basicFadeAdd");
                _basicEffectFadeAdd.effect.Name = "Shaders/basicFadeAdd";
            }
            LayerCore.ReinitializeLightingTargets();
            ResetLayers();
        }

        public static void ReinitializeLightingTargets()
        {
            if (Layer.core._lighting == null)
                return;
            Layer.core._lighting._target = new RenderTarget2D(Resolution.current.x, Resolution.current.y);
            Layer.core._console.camera = new Camera(0f, 0f, DevConsole.size.x, DevConsole.size.y);
        }

        public void ClearLayers()
        {
            foreach (DrawList hybrid in _hybridList)
                hybrid.Clear();
        }

        private void SortLayers()
        {
            if (_layerMap == null || _layerMap.Length != _hybridList.Count)
                _layerMap = new LayerCore.MapEntry[_hybridList.Count];
            bool flag = true;
            int index = 0;
            int num1 = int.MinValue;
            foreach (Layer hybrid in _hybridList)
            {
                int num2 = -hybrid.depth;
                _layerMap[index].index = index;
                _layerMap[index].order = num2;
                if (num2 < num1)
                    flag = false;
                else
                    num1 = num2;
                ++index;
            }
            if (flag)
                return;
            Array.Sort<LayerCore.MapEntry>(_layerMap, (x, y) => x.order.CompareTo(y.order));
        }

        public void DrawTargetLayers()
        {
            SortLayers();
            uint num1 = 0;
            for (int index = 0; index < _hybridList.Count; ++index)
            {
                Layer hybrid = _hybridList[_layerMap[index].index];
                if (hybrid.visible && hybrid.isTargetLayer && (Layer.lighting && !NetworkDebugger.enabled || hybrid != _lighting))
                {
                    double num2 = 2.0 / _hybridList.Count * index / 2.0;
                    double num3 = 2.0 / _hybridList.Count * (index + 1) / 2.0;
                    hybrid.Draw(true, true);
                    ++num1;
                }
            }
        }

        public void DrawLayers()
        {
            SortLayers();
            if (_lastDrawIndexCount == 0)
                _lastDrawIndexCount = _hybridList.Count;
            int num1 = 0;
            for (int index = 0; index < _hybridList.Count; ++index)
            {
                Layer hybrid = _hybridList[_layerMap[index].index];
                if (hybrid.visible && (Layer.lighting || hybrid != _lighting))
                {
                    int num2 = 1;
                    if (hybrid == Layer.Game)
                        num2 = 3;
                    double num3 = 2.0 / _lastDrawIndexCount * num1 / 2.0;
                    double num4 = 2.0 / _lastDrawIndexCount * (num1 + num2) / 2.0;
                    hybrid.Draw(true);
                    num1 += num2;
                }
            }
            _lastDrawIndexCount = num1;
        }

        public void UpdateLayers()
        {
            foreach (Layer hybrid in _hybridList)
                hybrid.Update();
        }

        public void ResetLayers()
        {
            Layer.lightingTwoPointOh = false;
            foreach (Layer layer in _layers)
            {
                layer.fade = 1f;
                layer.effect = null;
                layer.camera = null;
                layer.perspective = false;
                layer.fadeAdd = 0f;
                layer.colorAdd = Vec3.Zero;
                layer.colorMul = Vec3.One;
                if (layer != _glow && layer != _lighting)
                {
                    layer.blend = BlendState.AlphaBlend;
                    layer.targetBlend = BlendState.AlphaBlend;
                }
                layer.ClearScissor();
                layer.Clear();
            }
            _extraLayers.Clear();
            _parallax.camera = new Camera(0f, 0f, 320f, 320f * Resolution.current.aspect);
            _virtual.camera = new Camera(0f, 0f, 320f, 320f * Resolution.current.aspect);
            _hud.camera = new Camera();
            _hud.allowTallAspect = false;
            _console.camera = new Camera(0f, 0f, Resolution.current.x / 2, Resolution.current.y / 2);
            _hybridList.Clear();
            _hybridList.AddRange(_layers);
        }

        public Layer Get(string layer) => _layers.FirstOrDefault<Layer>(x => x.name == layer);

        public void Add(Layer l)
        {
            if (_extraLayers.Contains(l))
                return;
            _extraLayers.Add(l);
            _hybridList.Add(l);
        }

        public void Remove(Layer l)
        {
            _extraLayers.Remove(l);
            _hybridList.Remove(l);
        }

        public bool Contains(Layer l) => _hybridList.Contains(l);

        private struct MapEntry
        {
            public int index;
            public int order;
        }
    }
}
