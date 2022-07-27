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
                foreach (Layer layer in this._layers)
                    layer.visible = value;
                foreach (Layer extraLayer in this._extraLayers)
                    extraLayer.visible = value;
            }
        }

        public MTEffect basicWireframeEffect => !this.basicWireframeTex ? this._basicWireframeEffect : this._basicWireframeEffectTex;

        public MTEffect basicLayerEffect => this._basicEffectFadeAdd;

        public bool IsBasicLayerEffect(MTEffect e)
        {
            if (e == null)
                return false;
            return e.effectIndex == _basicEffect.effectIndex || e.effectIndex == _basicEffectAdd.effectIndex || e.effectIndex == _basicEffectFade.effectIndex || e.effectIndex == _basicEffectFadeAdd.effectIndex;
        }

        public void InitializeLayers()
        {
            Layer.lightingTwoPointOh = false;
            this._layers.Add(new Layer("PARALLAX", 100));
            this._parallax = this._layers[this._layers.Count - 1];
            this._parallax.allowTallAspect = true;
            this._parallax.aspectReliesOnGameLayer = true;
            this._layers.Add(new Layer("VIRTUAL", 95));
            this._virtual = this._layers[this._layers.Count - 1];
            this._virtual.allowTallAspect = true;
            this._virtual.aspectReliesOnGameLayer = true;
            this._layers.Add(new Layer("BACKGROUND", 90));
            this._background = this._layers[this._layers.Count - 1];
            this._background.enableCulling = true;
            this._background.allowTallAspect = true;
            this._layers.Add(new Layer("GAME"));
            this._game = this._layers[this._layers.Count - 1];
            this._game.enableCulling = false;
            this._game.allowTallAspect = true;
            this._layers.Add(new Layer("BLOCKS", -18));
            this._blocks = this._layers[this._layers.Count - 1];
            this._blocks.enableCulling = true;
            this._blocks.allowTallAspect = true;
            this._layers.Add(new Layer("FOREGROUND", -19));
            this._foreground = this._layers[this._layers.Count - 1];
            this._foreground.allowTallAspect = true;
            this._layers.Add(new Layer("HUD", -90));
            this._hud = this._layers[this._layers.Count - 1];
            this._layers.Add(new Layer("CONSOLE", -100, new Camera(Resolution.current.x / 2, Resolution.current.y / 2)));
            this._console = this._layers[this._layers.Count - 1];
            this._console.allowTallAspect = true;
            this._layers.Add(new Layer("GLOW", -21));
            this._glow = this._layers[this._layers.Count - 1];
            this._glow.allowTallAspect = true;
            this._layers.Add(new Layer("LIGHTING", Layer.lightingTwoPointOh ? -20 : -10, targetLayer: true, targetSize: new Vec2(Graphics.width, Graphics.height)));
            this._lighting = this._layers[this._layers.Count - 1];
            this._lighting.allowTallAspect = true;
            BlendState blendState1 = new BlendState
            {
                ColorSourceBlend = Blend.Zero,
                ColorDestinationBlend = Blend.SourceColor,
                ColorBlendFunction = BlendFunction.Add,
                AlphaSourceBlend = Blend.Zero,
                AlphaDestinationBlend = Blend.SourceAlpha,
                AlphaBlendFunction = BlendFunction.Add
            };
            this._glow.blend = BlendState.Additive;
            this._lighting.targetBlend = BlendState.Additive;
            this._lighting.targetBlend = new BlendState()
            {
                ColorSourceBlend = Blend.One,
                ColorDestinationBlend = Blend.One,
                ColorBlendFunction = BlendFunction.Add,
                AlphaSourceBlend = Blend.One,
                AlphaDestinationBlend = Blend.One,
                AlphaBlendFunction = BlendFunction.Add
            };
            this._lighting.blend = blendState1;
            this._lighting.targetClearColor = new Color(120, 120, 120, (int)byte.MaxValue);
            this._lighting.targetDepthStencil = DepthStencilState.None;
            this._lighting.flashAddClearInfluence = 1f;
            BlendState blendState2 = new BlendState()
            {
                ColorBlendFunction = BlendFunction.Add,
                ColorSourceBlend = Blend.DestinationColor,
                ColorDestinationBlend = Blend.Zero,
                AlphaBlendFunction = BlendFunction.Add,
                AlphaSourceBlend = Blend.DestinationColor,
                AlphaDestinationBlend = Blend.Zero
            };
            this._layers = this._layers.OrderBy<Layer, int>(l => -l.depth).ToList<Layer>();
            Layer.Parallax.flashAddInfluence = 1f;
            Layer.HUD.flashAddInfluence = 1f;
            if (this._basicEffect == null)
            {
                this._itemSpawnEffect = Content.Load<MTEffect>("Shaders/wireframeTex");
                this._basicWireframeEffect = Content.Load<MTEffect>("Shaders/wireframe");
                this._basicWireframeEffectTex = Content.Load<MTEffect>("Shaders/wireframeTex");
                this._basicEffect = Content.Load<MTEffect>("Shaders/basic");
                this._basicEffect.effect.Name = "Shaders/basic";
                this._basicEffectFade = Content.Load<MTEffect>("Shaders/basicFade");
                this._basicEffectFade.effect.Name = "Shaders/basicFade";
                this._basicEffectAdd = Content.Load<MTEffect>("Shaders/basicAdd");
                this._basicEffectAdd.effect.Name = "Shaders/basicAdd";
                this._basicEffectFadeAdd = Content.Load<MTEffect>("Shaders/basicFadeAdd");
                this._basicEffectFadeAdd.effect.Name = "Shaders/basicFadeAdd";
            }
            LayerCore.ReinitializeLightingTargets();
            this.ResetLayers();
        }

        public static void ReinitializeLightingTargets()
        {
            if (Layer.core._lighting == null)
                return;
            Layer.core._lighting._target = new RenderTarget2D(Resolution.current.x, Resolution.current.y);
            Layer.core._console.camera = new Camera(0.0f, 0.0f, DevConsole.size.x, DevConsole.size.y);
        }

        public void ClearLayers()
        {
            foreach (DrawList hybrid in this._hybridList)
                hybrid.Clear();
        }

        private void SortLayers()
        {
            if (this._layerMap == null || this._layerMap.Length != this._hybridList.Count)
                this._layerMap = new LayerCore.MapEntry[this._hybridList.Count];
            bool flag = true;
            int index = 0;
            int num1 = int.MinValue;
            foreach (Layer hybrid in this._hybridList)
            {
                int num2 = -hybrid.depth;
                this._layerMap[index].index = index;
                this._layerMap[index].order = num2;
                if (num2 < num1)
                    flag = false;
                else
                    num1 = num2;
                ++index;
            }
            if (flag)
                return;
            Array.Sort<LayerCore.MapEntry>(this._layerMap, (x, y) => x.order.CompareTo(y.order));
        }

        public void DrawTargetLayers()
        {
            this.SortLayers();
            uint num1 = 0;
            for (int index = 0; index < this._hybridList.Count; ++index)
            {
                Layer hybrid = this._hybridList[this._layerMap[index].index];
                if (hybrid.visible && hybrid.isTargetLayer && (Layer.lighting && !NetworkDebugger.enabled || hybrid != this._lighting))
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
            this.SortLayers();
            if (this._lastDrawIndexCount == 0)
                this._lastDrawIndexCount = this._hybridList.Count;
            int num1 = 0;
            for (int index = 0; index < this._hybridList.Count; ++index)
            {
                Layer hybrid = this._hybridList[this._layerMap[index].index];
                if (hybrid.visible && (Layer.lighting || hybrid != this._lighting))
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
            this._lastDrawIndexCount = num1;
        }

        public void UpdateLayers()
        {
            foreach (Layer hybrid in this._hybridList)
                hybrid.Update();
        }

        public void ResetLayers()
        {
            Layer.lightingTwoPointOh = false;
            foreach (Layer layer in this._layers)
            {
                layer.fade = 1f;
                layer.effect = null;
                layer.camera = null;
                layer.perspective = false;
                layer.fadeAdd = 0.0f;
                layer.colorAdd = Vec3.Zero;
                layer.colorMul = Vec3.One;
                if (layer != this._glow && layer != this._lighting)
                {
                    layer.blend = BlendState.AlphaBlend;
                    layer.targetBlend = BlendState.AlphaBlend;
                }
                layer.ClearScissor();
                layer.Clear();
            }
            this._extraLayers.Clear();
            this._parallax.camera = new Camera(0.0f, 0.0f, 320f, 320f * Resolution.current.aspect);
            this._virtual.camera = new Camera(0.0f, 0.0f, 320f, 320f * Resolution.current.aspect);
            this._hud.camera = new Camera();
            this._hud.allowTallAspect = false;
            this._console.camera = new Camera(0.0f, 0.0f, Resolution.current.x / 2, Resolution.current.y / 2);
            this._hybridList.Clear();
            this._hybridList.AddRange(_layers);
        }

        public Layer Get(string layer) => this._layers.FirstOrDefault<Layer>(x => x.name == layer);

        public void Add(Layer l)
        {
            if (this._extraLayers.Contains(l))
                return;
            this._extraLayers.Add(l);
            this._hybridList.Add(l);
        }

        public void Remove(Layer l)
        {
            this._extraLayers.Remove(l);
            this._hybridList.Remove(l);
        }

        public bool Contains(Layer l) => this._hybridList.Contains(l);

        private struct MapEntry
        {
            public int index;
            public int order;
        }
    }
}
