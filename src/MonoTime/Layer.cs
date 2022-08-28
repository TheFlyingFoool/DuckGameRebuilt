// Decompiled with JetBrains decompiler
// Type: DuckGame.Layer
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DuckGame
{
    public class Layer : DrawList
    {
        public bool enableCulling;
        public static bool lightingTwoPointOh = false;
        private static bool _lighting = false;
        private static LayerCore _core = new LayerCore();
        private static Layer _preDrawLayer = new Layer("PREDRAW");
        protected MTSpriteBatch _batch;
        private string _name;
        private int _depth;
        private Vec2 _depthSpan;
        private Effect _effect;
        private bool _visible = true;
        private bool _blurEffect;
        private bool _perspective;
        private BlendState _blend = BlendState.AlphaBlend;
        private BlendState _targetBlend = BlendState.AlphaBlend;
        private Color _targetClearColor = new Color(0, 0, 0, 0);
        private DepthStencilState _targetDepthStencil = DepthStencilState.Default;
        private RenderTarget2D _slaveTarget;
        private float _targetFade = 1f;
        protected Rectangle _scissor;
        protected float _fade = 1f;
        protected float _fadeAdd;
        protected Vec3 _colorAdd = Vec3.Zero;
        protected Vec3 _colorMul = Vec3.One;
        protected float _darken;
        public Camera _tallCamera;
        protected Camera _camera;
        protected RasterizerState _state;
        private Sprite _dropShadow = new Sprite("dropShadow");
        public RenderTarget2D _target;
        private Layer _shareDrawObjects;
        private bool _targetOnly;
        public bool aspectReliesOnGameLayer;
        private bool _allowTallAspect;
        public float flashAddInfluence;
        public float flashAddClearInfluence;
        private Viewport _oldViewport;
        private RenderTarget2D _oldRenderTarget;
        private Camera _targetCamera = new Camera();
        public static Vec3 kGameLayerFade;
        public static Vec3 kGameLayerAdd;
        public static bool blurry = false;
        public static bool ignoreTransparent = false;
        public static bool skipDrawing = false;
        public float currentSpanOffset;

        public static bool lighting
        {
            get => Options.Data.lighting && Layer._lighting && !(Level.current is Editor);
            set => Layer._lighting = value;
        }

        public static LayerCore core
        {
            get => Layer._core;
            set => Layer._core = value;
        }

        public static Layer PreDrawLayer => Layer._preDrawLayer;

        public static Layer Parallax => Layer._core._parallax;

        public static Layer Virtual => Layer._core._virtual;

        public static Layer Background => Layer._core._background;

        public static Layer Game => Layer._core._game;

        public static Layer Blocks => Layer._core._blocks;

        public static Layer Glow => Layer._core._glow;

        public static Layer Lighting => Layer._core._lighting;

        public static Layer Foreground => Layer._core._foreground;

        public static Layer HUD
        {
            get => Layer._core._hud;
            set => Layer._core._hud = value;
        }

        public static Layer Console => Layer._core._console;

        public static bool doVirtualEffect
        {
            get => Layer._core.doVirtualEffect;
            set => Layer._core.doVirtualEffect = value;
        }

        public static MTEffect basicWireframeEffect => Layer._core.basicWireframeEffect;

        public static bool basicWireframeTex
        {
            get => Layer._core.basicWireframeTex;
            set => Layer._core.basicWireframeTex = value;
        }

        public static MTEffect itemSpawnEffect => Layer._core._itemSpawnEffect;

        public static bool allVisible
        {
            set => Layer._core.allVisible = value;
        }

        public static MTEffect basicLayerEffect => Layer._core._basicEffectFadeAdd;

        public static bool IsBasicLayerEffect(MTEffect e) => Layer._core.IsBasicLayerEffect(e);

        public static void InitializeLayers() => Layer._core.InitializeLayers();

        public static void ClearLayers() => Layer._core.ClearLayers();

        public static void DrawLayers() => Layer._core.DrawLayers();

        public static void DrawTargetLayers() => Layer._core.DrawTargetLayers();

        public static void UpdateLayers() => Layer._core.UpdateLayers();

        public static void ResetLayers() => Layer._core.ResetLayers();

        public static Layer Get(string layer) => Layer._core.Get(layer);

        public static void Add(Layer l) => Layer._core.Add(l);

        public static void Remove(Layer l) => Layer._core.Remove(l);

        public static bool Contains(Layer l) => Layer._core.Contains(l);

        public Matrix fullMatrix => _batch.fullMatrix;

        public string name => _name;

        public int depth
        {
            get => _depth;
            set => _depth = value;
        }

        public Vec2 depthSpan
        {
            get => _depthSpan;
            set => _depthSpan = value;
        }

        public Effect effect
        {
            get => _effect;
            set => _effect = value;
        }

        public bool visible
        {
            get => _visible;
            set => _visible = value;
        }

        public bool blurEffect
        {
            get => _blurEffect;
            set => _blurEffect = value;
        }

        public float barSize => (float)((camera.width * DuckGame.Graphics.aspect - camera.width * (9.0 / 16.0)) / 2.0);

        public Matrix projection { get; set; }

        public Matrix view { get; set; }

        public bool perspective
        {
            get => _perspective;
            set => _perspective = value;
        }

        public BlendState blend
        {
            get => _blend;
            set => _blend = value;
        }

        public BlendState targetBlend
        {
            get => _targetBlend;
            set => _targetBlend = value;
        }

        public Color targetClearColor
        {
            get => _targetClearColor;
            set => _targetClearColor = value;
        }

        public DepthStencilState targetDepthStencil
        {
            get => _targetDepthStencil;
            set => _targetDepthStencil = value;
        }

        public RenderTarget2D slaveTarget
        {
            get => _slaveTarget;
            set => _slaveTarget = value;
        }

        public float targetFade
        {
            get => _targetFade;
            set => _targetFade = value;
        }

        public Rectangle scissor
        {
            get => _scissor;
            set
            {
                if (_scissor.width == 0.0 && value.width != 0.0)
                {
                    _state = new RasterizerState
                    {
                        CullMode = CullMode.None,
                        ScissorTestEnable = true
                    };
                }
                _scissor = value;
            }
        }

        public void ClearScissor()
        {
            if (_scissor.width == 0.0)
                return;
            _scissor = new Rectangle(0f, 0f, 0f, 0f);
            _state = new RasterizerState
            {
                CullMode = CullMode.None
            };
        }

        public float fade
        {
            get => _fade;
            set => _fade = value;
        }

        public float fadeAdd
        {
            get => _fadeAdd;
            set => _fadeAdd = value;
        }

        public Vec3 colorAdd
        {
            get => _colorAdd;
            set => _colorAdd = value;
        }

        public Vec3 colorMul
        {
            get => _colorMul;
            set => _colorMul = value;
        }

        public float darken
        {
            get => _darken;
            set => _darken = value;
        }

        public Camera camera
        {
            get => _camera == null && Level.activeLevel != null ? Level.activeLevel.camera : _camera;
            set => _camera = value;
        }

        public float width => camera.width;

        public float height => camera.height;

        public RenderTarget2D target => _slaveTarget != null ? _slaveTarget : _target;

        public Layer shareDrawObjects
        {
            get => _shareDrawObjects;
            set => _shareDrawObjects = value;
        }

        public bool targetOnly
        {
            get => _targetOnly;
            set => _targetOnly = value;
        }

        public bool allowTallAspect
        {
            get => _allowTallAspect && !camera.sixteenNine;
            set => _allowTallAspect = value;
        }

        public bool isTargetLayer => target != null;

        public Layer(string nameval, int depthval = 0, Camera cam = null, bool targetLayer = false, Vec2 targetSize = default(Vec2))
        {
            _name = nameval;
            _depth = depthval;
            _batch = new MTSpriteBatch(DuckGame.Graphics.device);
            _state = new RasterizerState
            {
                CullMode = CullMode.None
            };
            _camera = cam;
            _dropShadow.CenterOrigin();
            _dropShadow.alpha = 0.5f;
            if (!targetLayer)
                return;
            if (targetSize == new Vec2())
                _target = new RenderTarget2D(DuckGame.Graphics.width, DuckGame.Graphics.height);
            else
                _target = new RenderTarget2D((int)targetSize.x, (int)targetSize.y);
        }

        public virtual void Update()
        {
            foreach (Thing thing in _transparentRemove)
                _transparent.Remove(thing);
            foreach (Thing thing in _opaqueRemove)
                _opaque.Remove(thing);
            _transparentRemove.Clear();
            _opaqueRemove.Clear();
        }

        public virtual void Begin(bool transparent, bool isTargetDraw = false)
        {
            int num1 = name == "LIGHTING" ? 1 : 0;
            if (aspectReliesOnGameLayer && this.camera != Layer.Game.camera)
            {
                this.camera.width = 320f;
                this.camera.height = 320f / Layer.Game.camera.aspect;
            }
            if (allowTallAspect)
                DuckGame.Graphics.SetFullViewport();
            try
            {
                if (isTargetDraw & transparent && _target != null)
                {
                    _oldRenderTarget = DuckGame.Graphics.GetRenderTarget();
                    _oldViewport = DuckGame.Graphics.viewport;
                    DuckGame.Graphics.SetRenderTarget(_target);
                    if (flashAddClearInfluence > 0.0)
                        DuckGame.Graphics.Clear(new Color((byte)Math.Min(_targetClearColor.r + (float)(flashAddClearInfluence * DuckGame.Graphics.flashAddRenderValue * byte.MaxValue), byte.MaxValue), (byte)Math.Min(_targetClearColor.g + (float)(flashAddClearInfluence * DuckGame.Graphics.flashAddRenderValue * byte.MaxValue), byte.MaxValue), (byte)Math.Min(_targetClearColor.b + (float)(flashAddClearInfluence * DuckGame.Graphics.flashAddRenderValue * byte.MaxValue), byte.MaxValue), _targetClearColor.a));
                    else
                        DuckGame.Graphics.Clear(_targetClearColor);
                }
                if (!isTargetDraw)
                {
                    if (Graphics.currentRenderTarget != null)
                    {
                        if (!Graphics.currentRenderTarget.depth)
                            goto label_14;
                    }
                    Graphics.device.Clear(ClearOptions.DepthBuffer, (Microsoft.Xna.Framework.Color)Color.Black, 1f, 0);
                }
            }
            catch (Exception ex)
            {
                DevConsole.Log("|DGRED|Layer.Begin exception: " + ex.Message);
            }
        label_14:
            Graphics.ResetSpanAdjust();
            Effect effect = (Effect)Layer._core._basicEffect;
            Vec3 vec3_1 = new Vec3((Graphics.fade * _fade * (1f - _darken))) * colorMul;
            Vec3 vec3_2 = _colorAdd + new Vec3(_fadeAdd) + new Vec3(Graphics.flashAddRenderValue) * flashAddInfluence + new Vec3(Graphics.fadeAddRenderValue) - new Vec3(darken);
            vec3_2 = new Vec3(Maths.Clamp(vec3_2.x, -1f, 1f), Maths.Clamp(vec3_2.y, -1f, 1f), Maths.Clamp(vec3_2.z, -1f, 1f));
            vec3_2 *= vec3_1;
            if (this == Layer.Game)
            {
                Layer.kGameLayerFade = vec3_1;
                Layer.kGameLayerAdd = vec3_2;
            }
            if (_darken > 0f)
                _darken -= 0.15f;
            else if (_darken < 0f)
                _darken += 0.15f;
            if (Math.Abs(_darken) < 0.16f)
                _darken = 0f;
            if (_effect != null)
            {
                effect = _effect;
                effect.Parameters["fade"]?.SetValue((Vector3)vec3_1);
                effect.Parameters["add"]?.SetValue((Vector3)vec3_2);
            }
            else
            {
                float num2 = vec3_2.LengthSquared();
                if (vec3_1 != Vec3.One && num2 > 1f / 1000f)
                {
                    effect = (Effect)Layer._core._basicEffectFadeAdd;
                    effect.Parameters["fade"].SetValue((Vector3)vec3_1);
                    effect.Parameters["add"].SetValue((Vector3)vec3_2);
                }
                else if (vec3_1 != Vec3.One)
                {
                    effect = (Effect)Layer._core._basicEffectFade;
                    effect.Parameters["fade"].SetValue((Vector3)vec3_1);
                }
                else if (num2 > 1f / 1000f)
                {
                    effect = (Effect)Layer._core._basicEffectAdd;
                    effect.Parameters["add"].SetValue((Vector3)vec3_2);
                }
                if (Layer.doVirtualEffect && (Layer.Game == this || Layer.Foreground == this || Layer.Blocks == this || Layer.Background == this))
                    effect = !Layer.basicWireframeTex ? (Effect)Layer._core._basicWireframeEffect : (Effect)Layer._core._basicWireframeEffectTex;
            }
            if (_state.ScissorTestEnable)
                DuckGame.Graphics.SetScissorRectangle(_scissor);
            DuckGame.Graphics.screen = _batch;
            Camera camera = this.camera;
            if (target != null & isTargetDraw && !targetOnly)
            {
                _targetCamera.x = (float)Math.Round(this.camera.x - 1f);
                _targetCamera.y = (float)Math.Round(this.camera.y - 1f);
                _targetCamera.width = Math.Max(this.camera.width, Graphics.width);
                _targetCamera.height = Math.Max(this.camera.height, Graphics.height);
                camera = _targetCamera;
            }
            BlendState blendState = _blend;
            if (isTargetDraw)
                blendState = _targetBlend;
            if (target != null & isTargetDraw)
            {
                Vec2 position1 = camera.position;
                position1.x = (float)Math.Floor(position1.x);
                position1.y = (float)Math.Floor(position1.y);
                Vec2 size1 = camera.size;
                size1.x = (float)Math.Floor(size1.x);
                size1.y = (float)Math.Floor(size1.y);
                Vec2 position2 = camera.position;
                Vec2 size2 = camera.size;
                _batch.Begin(SpriteSortMode.BackToFront, blendState, SamplerState.PointClamp, _targetDepthStencil, _state, (MTEffect)effect, camera.getMatrix());
                camera.position = position2;
                camera.size = size2;
            }
            else if (Layer.blurry || _blurEffect)
            {
                if (!transparent)
                    _batch.Begin(SpriteSortMode.FrontToBack, blendState, SamplerState.LinearClamp, DepthStencilState.Default, _state, (MTEffect)effect, camera.getMatrix());
                else
                    _batch.Begin(SpriteSortMode.BackToFront, blendState, SamplerState.LinearClamp, DepthStencilState.DepthRead, _state, (MTEffect)effect, camera.getMatrix());
            }
            else if (!transparent)
                _batch.Begin(SpriteSortMode.FrontToBack, blendState, SamplerState.PointClamp, DepthStencilState.Default, _state, (MTEffect)effect, camera.getMatrix());
            else
                _batch.Begin(SpriteSortMode.BackToFront, blendState, SamplerState.PointClamp, DepthStencilState.DepthRead, _state, (MTEffect)effect, camera.getMatrix());
        }

        public void End(bool transparent, bool isTargetDraw = false)
        {
            _batch.End();
            Graphics.screen = null;
            Graphics.currentLayer = null;
            if (isTargetDraw & transparent && _target != null)
            {
                Graphics.SetRenderTarget(_oldRenderTarget);
                Graphics.viewport = _oldViewport;
            }
            if (!allowTallAspect)
                return;
            Graphics.RestoreOldViewport();
        }

        public virtual void Draw(bool transparent, bool isTargetDraw = false)
        {
            if (currentSpanOffset > 10000f)
                currentSpanOffset = 0f;
            if (!transparent && Layer.ignoreTransparent || isTargetDraw && slaveTarget != null || target != null && !isTargetDraw && targetOnly)
                return;
            if (Network.isActive && this == Layer.Game)
                DuckGame.Graphics.currentFrameCalls = new List<DrawCall>();
            Level.activeLevel.InitializeDraw(this);
            DuckGame.Graphics.currentLayer = this;
            
            Graphics.polyBatcher.UpdateMatricesForCurrentLayer();
            
            
            Begin(transparent, isTargetDraw);
            if (target != null && !isTargetDraw)
            {
                Vec2 position = Level.activeLevel.camera.position - new Vec2(1f, 1f);
                position.x = (float)Math.Round(position.x);
                position.y = (float)Math.Round(position.y);
                Color color = new Color(1f * _targetFade, 1f * _targetFade, 1f * _targetFade, 1f);
                Vec2 vec2 = new Vec2(Math.Max(camera.width, Graphics.width), Math.Max(camera.height, Graphics.height));
                DuckGame.Graphics.skipReplayRender = true;
                DuckGame.Graphics.Draw(target, position, new Rectangle?(), color, 0f, Vec2.Zero, new Vec2(vec2.x / target.width, vec2.y / target.height), SpriteEffects.None, (Depth)1f);
                if (name == "LIGHTING")
                {
                    if (VirtualTransition.core._scanStage == 1)
                        targetClearColor = Lerp.ColorSmooth(new Color(120, 120, 120, (int)byte.MaxValue), Color.White, VirtualTransition.core._stick);
                    else if (VirtualTransition.core._scanStage == 3)
                        targetClearColor = new Color(120, 120, 120, (int)byte.MaxValue);
                }
                DuckGame.Graphics.skipReplayRender = false;
            }
            else
            {
                if (transparent)
                    Level.activeLevel.PreDrawLayer(this);
                HashSet<Thing> transparent1 = _transparent;
                HashSet<Thing> opaque = _opaque;
                if (_shareDrawObjects != null)
                {
                    transparent1 = _shareDrawObjects._transparent;
                    opaque = _shareDrawObjects._opaque;
                }
                if (!Layer.skipDrawing)
                {
                    if (transparent)
                    {
                        if (Network.isActive)
                        {
                            if (this != Layer.Parallax && DevConsoleCommands.GraphicsCulling && !(Level.current is RockScoreboard))
                            {
                                Vec2 Topleft = this.camera.transformInverse(new Vec2(0f, 0f));
                                Vec2 Bottomright = this.camera.transformInverse(new Vec2(DuckGame.Graphics.viewport.Width, DuckGame.Graphics.viewport.Height));
                                int top = (int)((Bottomright.y + QuadTreeObjectList.offset) / QuadTreeObjectList.cellsize);
                                int left = (int)((Topleft.x + QuadTreeObjectList.offset) / QuadTreeObjectList.cellsize);
                                int bottom = (int)((Topleft.y + QuadTreeObjectList.offset) / QuadTreeObjectList.cellsize);
                                int right = (int)((Bottomright.x + QuadTreeObjectList.offset) / QuadTreeObjectList.cellsize);
                                top += 1;
                                bottom -= 1;
                                right += 1;
                                left -= 1;
                                foreach (Thing thing in transparent1)
                                {
                                    bool flag = false;
                                    foreach (Vec2 vec2 in thing.Buckets)
                                    {
                                        flag = vec2.x <= right && vec2.x >= left && vec2.y <= top && vec2.y >= bottom;
                                        if (flag)
                                        {
                                            break;
                                        }
                                    }
                                    if ((flag || thing.Buckets.Length == 0 || thing.owner != null || thing is FunBeam || thing is Door) && thing.visible && (thing.ghostObject == null || thing.ghostObject.IsInitialized()))
                                    {
                                        if (_perspective)
                                        {
                                            Vec2 position = thing.position;
                                            Vec3 source = new Vec3(position.x, thing.z, thing.bottom);
                                            Viewport viewport = new Viewport(0, 0, 320, 180);
                                            source = (Vec3)viewport.Project((Vector3)source, (Microsoft.Xna.Framework.Matrix)projection, (Microsoft.Xna.Framework.Matrix)view, (Microsoft.Xna.Framework.Matrix)Matrix.Identity);
                                            thing.position = new Vec2(source.x, source.y - thing.centery);
                                            thing.DoDraw();
                                            Graphics.material = null;
                                            thing.position = position;
                                            if (thing is PhysicsObject)
                                            {
                                                float num = Maths.NormalizeSection(-thing.y, 8f, 64f);
                                                _dropShadow.alpha = (float)(0.5 - 0.5 * num);
                                                _dropShadow.scale = new Vec2(1f - num, 1f - num);
                                                _dropShadow.depth = thing.depth - 10;
                                                source = new Vec3(position.x, thing.z, 0f);
                                                source = (Vec3)viewport.Project((Vector3)source, (Microsoft.Xna.Framework.Matrix)projection, (Microsoft.Xna.Framework.Matrix)view, (Microsoft.Xna.Framework.Matrix)Matrix.Identity);
                                                Graphics.Draw(_dropShadow, source.x - 1f, source.y - 1f);
                                            }
                                        }
                                        else
                                            thing.DoDraw();
                                        Graphics.material = null;
                                    }
                                    else
                                    {

                                    }
                                }
                            }
                            else
                            {
                                foreach (Thing thing in transparent1)
                                {
                                    if (thing.visible && (thing.ghostObject == null || thing.ghostObject.IsInitialized()))
                                    {
                                        if (_perspective)
                                        {
                                            Vec2 position = thing.position;
                                            Vec3 source = new Vec3(position.x, thing.z, thing.bottom);
                                            Viewport viewport = new Viewport(0, 0, 320, 180);
                                            source = (Vec3)viewport.Project((Vector3)source, (Microsoft.Xna.Framework.Matrix)projection, (Microsoft.Xna.Framework.Matrix)view, (Microsoft.Xna.Framework.Matrix)Matrix.Identity);
                                            thing.position = new Vec2(source.x, source.y - thing.centery);
                                            thing.DoDraw();
                                            Graphics.material = null;
                                            thing.position = position;
                                            if (thing is PhysicsObject)
                                            {
                                                float num = Maths.NormalizeSection(-thing.y, 8f, 64f);
                                                _dropShadow.alpha = (float)(0.5 - 0.5 * num);
                                                _dropShadow.scale = new Vec2(1f - num, 1f - num);
                                                _dropShadow.depth = thing.depth - 10;
                                                source = new Vec3(position.x, thing.z, 0f);
                                                source = (Vec3)viewport.Project((Vector3)source, (Microsoft.Xna.Framework.Matrix)projection, (Microsoft.Xna.Framework.Matrix)view, (Microsoft.Xna.Framework.Matrix)Matrix.Identity);
                                                Graphics.Draw(_dropShadow, source.x - 1f, source.y - 1f);
                                            }
                                        }
                                        else
                                            thing.DoDraw();
                                        Graphics.material = null;
                                    }
                                }
                            }
                            if (DevConsole.showCollision)
                            {
                                foreach (Thing thing in transparent1)
                                {
                                    if (thing.visible)
                                        thing.DrawCollision();
                                }
                            }
                        }
                        else if (this == Layer.Lighting)
                        {
                            foreach (Thing thing in transparent1)
                            {
                                if (thing.visible)
                                {
                                    thing.DoDraw();
                                    Graphics.material = null;
                                }
                            }
                        }
                        else
                        {
                            if (this != Layer.Parallax && DevConsoleCommands.GraphicsCulling && !(Level.current is RockScoreboard))
                            {
                                Vec2 Topleft = this.camera.transformInverse(new Vec2(0f, 0f));
                                Vec2 Bottomright = this.camera.transformInverse(new Vec2(DuckGame.Graphics.viewport.Width, DuckGame.Graphics.viewport.Height));
                                int top = (int)((Bottomright.y + QuadTreeObjectList.offset) / QuadTreeObjectList.cellsize);
                                int left = (int)((Topleft.x + QuadTreeObjectList.offset) / QuadTreeObjectList.cellsize);
                                int bottom = (int)((Topleft.y + QuadTreeObjectList.offset) / QuadTreeObjectList.cellsize);
                                int right = (int)((Bottomright.x + QuadTreeObjectList.offset) / QuadTreeObjectList.cellsize);
                                top += 1;
                                bottom -= 1;
                                right += 1;
                                left -= 1;
                                foreach (Thing thing in transparent1)
                                {
                                    bool flag = false;
                                    foreach (Vec2 vec2 in thing.Buckets)
                                    {
                                        flag = vec2.x <= right && vec2.x >= left && vec2.y <= top && vec2.y >= bottom;
                                        if (flag)
                                        {
                                            break;
                                        }
                                    }
                                    if ((flag || thing.Buckets.Length == 0 || thing.owner != null || thing is FunBeam || thing is Door) && thing.visible)
                                    {
                                        if (_perspective)
                                        {
                                            Vec2 position = thing.position;
                                            Vec3 source = new Vec3(position.x, thing.z, thing.bottom);
                                            Viewport viewport = new Viewport(0, 0, 320, 180);
                                            source = (Vec3)viewport.Project((Vector3)source, (Microsoft.Xna.Framework.Matrix)projection, (Microsoft.Xna.Framework.Matrix)view, (Microsoft.Xna.Framework.Matrix)Matrix.Identity);
                                            thing.position = new Vec2(source.x, source.y - thing.centery);
                                            thing.DoDraw();
                                            Graphics.material = null;
                                            thing.position = position;
                                            if (thing is PhysicsObject)
                                            {
                                                float num = Maths.NormalizeSection(-thing.y, 8f, 64f);
                                                _dropShadow.alpha = (float)(0.5 - 0.5 * num);
                                                _dropShadow.scale = new Vec2(1f - num, 1f - num);
                                                _dropShadow.depth = thing.depth - 10;
                                                source = new Vec3(position.x, thing.z, 0f);
                                                source = (Vec3)viewport.Project((Vector3)source, (Microsoft.Xna.Framework.Matrix)projection, (Microsoft.Xna.Framework.Matrix)view, (Microsoft.Xna.Framework.Matrix)Matrix.Identity);
                                                Graphics.Draw(_dropShadow, source.x - 1f, source.y - 1f);
                                            }
                                        }
                                        else
                                            thing.DoDraw();
                                        Graphics.material = null;
                                    }

                                }
                            }
                            else
                            {
                                foreach (Thing thing in transparent1)
                                {
                                    if (thing.visible)
                                    {
                                        if (_perspective)
                                        {
                                            Vec2 position = thing.position;
                                            Vec3 source = new Vec3(position.x, thing.z, thing.bottom);
                                            Viewport viewport = new Viewport(0, 0, 320, 180);
                                            source = (Vec3)viewport.Project((Vector3)source, (Microsoft.Xna.Framework.Matrix)projection, (Microsoft.Xna.Framework.Matrix)view, (Microsoft.Xna.Framework.Matrix)Matrix.Identity);
                                            thing.position = new Vec2(source.x, source.y - thing.centery);
                                            thing.DoDraw();
                                            Graphics.material = null;
                                            thing.position = position;
                                            if (thing is PhysicsObject)
                                            {
                                                float num = Maths.NormalizeSection(-thing.y, 8f, 64f);
                                                _dropShadow.alpha = (float)(0.5 - 0.5 * num);
                                                _dropShadow.scale = new Vec2(1f - num, 1f - num);
                                                _dropShadow.depth = thing.depth - 10;
                                                source = new Vec3(position.x, thing.z, 0f);
                                                source = (Vec3)viewport.Project((Vector3)source, (Microsoft.Xna.Framework.Matrix)projection, (Microsoft.Xna.Framework.Matrix)view, (Microsoft.Xna.Framework.Matrix)Matrix.Identity);
                                                Graphics.Draw(_dropShadow, source.x - 1f, source.y - 1f);
                                            }
                                        }
                                        else
                                            thing.DoDraw();
                                        Graphics.material = null;
                                    }
                                }
                            }
                            if (this == Layer.Game)
                            {
                                DevConsoleCommands.drawthething();
                            }
                            if (DevConsole.showCollision)
                            {
                                foreach (Thing thing in transparent1)
                                {
                                    if (thing.visible)
                                        thing.DrawCollision();
                                }
                            }
                        }
                        if (ignoreTransparent)
                        {
                            foreach (Thing thing in opaque)
                            {
                                if (thing.visible)
                                    thing.DoDraw();
                                Graphics.material = null;
                            }
                            StaticRenderer.RenderLayer(this);
                        }
                    }
                    else
                    {
                        foreach (Thing thing in opaque)
                        {
                            if (thing.visible)
                                thing.DoDraw();
                        }
                        StaticRenderer.RenderLayer(this);
                    }
                }
                if (transparent)
                    Level.activeLevel.PostDrawLayer(this);
            }
            if (Network.isActive && Network.inputDelayFrames > 0 && this == Layer.Game)
            {
                Graphics.drawCalls.Enqueue(Graphics.currentFrameCalls);
                if (Graphics.drawCalls.Count > 0)
                {
                    List<DrawCall> drawCallList = Graphics.drawCalls.Peek();
                    if (Graphics.drawCalls.Count > Network.inputDelayFrames)
                        Graphics.drawCalls.Dequeue();
                    foreach (DrawCall drawCall in drawCallList)
                    {
                        if (drawCall.material != null)
                            Graphics.screen.DrawWithMaterial(drawCall.texture, drawCall.position, drawCall.sourceRect, drawCall.color, drawCall.rotation, drawCall.origin, drawCall.scale, drawCall.effects, drawCall.depth, drawCall.material);
                        else
                            Graphics.screen.Draw(drawCall.texture, drawCall.position, drawCall.sourceRect, drawCall.color, drawCall.rotation, drawCall.origin, drawCall.scale, drawCall.effects, drawCall.depth);
                    }
                }
            }
            End(transparent, isTargetDraw);
        }
    }
}
