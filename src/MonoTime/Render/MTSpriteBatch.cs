// Decompiled with JetBrains decompiler
// Type: DuckGame.MTSpriteBatch
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;

namespace DuckGame
{
    public class MTSpriteBatch : SpriteBatch
    {
        private int _globalIndex = Thing.GetGlobalIndex();
        private readonly MTSpriteBatcher _batcher;
        private SpriteSortMode _sortMode;
        private BlendState _blendState;
        private SamplerState _samplerState;
        private DepthStencilState _depthStencilState;
        private RasterizerState _rasterizerState;
        private Effect _effect;
        private bool _beginCalled;
        private MTEffect _spriteEffect;
        private MTEffect _simpleEffect;
        private readonly EffectParameter _matrixTransformSprite;
        private readonly EffectParameter _matrixTransformSimple;
        private Matrix _matrix;
        private Rectangle _tempRect = new Rectangle(0.0f, 0.0f, 0.0f, 0.0f);
        private Vec2 _texCoordTL = new Vec2(0.0f, 0.0f);
        private Vec2 _texCoordBR = new Vec2(0.0f, 0.0f);
        private Matrix _projMatrix;
        public Matrix fullMatrix;
        public static float edgeBias = 1E-05f;
        private RasterizerState _prevRast;

        public MTSpriteBatchItem StealLastSpriteBatchItem() => this._batcher.StealLastBatchItem();

        public bool transitionEffect => Layer.basicWireframeEffect != null && this._effect == Layer.basicWireframeEffect.effect;

        public MTEffect SpriteEffect => this._spriteEffect;

        public MTEffect SimpleEffect => this._simpleEffect;

        public MTSpriteBatch(GraphicsDevice graphicsDevice)
          : base(graphicsDevice)
        {
            if (graphicsDevice == null)
                throw new ArgumentException(nameof(graphicsDevice));
            this._spriteEffect = Content.Load<MTEffect>("Shaders/SpriteEffect");
            this._matrixTransformSprite = this._spriteEffect.effect.Parameters["MatrixTransform"];
            this._simpleEffect = Content.Load<MTEffect>("Shaders/SpriteEffectSimple");
            this._matrixTransformSimple = this._simpleEffect.effect.Parameters["MatrixTransform"];
            this._batcher = new MTSpriteBatcher(graphicsDevice, this);
            this._beginCalled = false;
        }

        public new void Begin() => this.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Matrix.Identity);

        public void Begin(
          SpriteSortMode sortMode,
          BlendState blendState,
          SamplerState samplerState,
          DepthStencilState depthStencilState,
          RasterizerState rasterizerState,
          MTEffect effect,
          Matrix transformMatrix)
        {
            GraphicsDevice device = DuckGame.Graphics.device;
            GraphicsDevice graphicsDevice = this.GraphicsDevice;
            DuckGame.Graphics.currentStateIndex = this._globalIndex;
            if (this._beginCalled)
                throw new InvalidOperationException("Begin cannot be called again until End has been successfully called.");
            base.Begin();
            if (Recorder.currentRecording != null)
                Recorder.currentRecording.StateChange(sortMode, blendState, samplerState, depthStencilState, rasterizerState, Layer.IsBasicLayerEffect(effect) ? Layer.basicLayerEffect : effect, transformMatrix, (Rectangle)this.GraphicsDevice.ScissorRectangle);
            this._sortMode = sortMode;
            this._blendState = blendState ?? BlendState.AlphaBlend;
            this._samplerState = samplerState ?? SamplerState.LinearClamp;
            this._depthStencilState = depthStencilState ?? DepthStencilState.None;
            this._rasterizerState = rasterizerState ?? RasterizerState.CullCounterClockwise;
            this._effect = (Effect)effect;
            this._matrix = transformMatrix;
            if (sortMode == SpriteSortMode.Immediate)
                this.Setup();
            this._beginCalled = true;
        }

        public new void Begin(SpriteSortMode sortMode, BlendState blendState) => this.Begin(sortMode, blendState, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Matrix.Identity);

        public new void Begin(
          SpriteSortMode sortMode,
          BlendState blendState,
          SamplerState samplerState,
          DepthStencilState depthStencilState,
          RasterizerState rasterizerState)
        {
            this.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, null, Matrix.Identity);
        }

        public void Begin(
          SpriteSortMode sortMode,
          BlendState blendState,
          SamplerState samplerState,
          DepthStencilState depthStencilState,
          RasterizerState rasterizerState,
          MTEffect effect)
        {
            this.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, Matrix.Identity);
        }

        public new void End()
        {
            this._beginCalled = false;
            base.End();
            if (DuckGame.Graphics.recordOnly)
                return;
            if (this._batcher.hasSimpleItems)
            {
                if (this._sortMode != SpriteSortMode.Immediate)
                    this.Setup(true);
                this._batcher.DrawSimpleBatch(this._sortMode);
            }
            if (this._batcher.hasGeometryItems)
            {
                if (this._sortMode != SpriteSortMode.Immediate)
                    this.Setup(true);
                this._batcher.DrawGeometryBatch(this._sortMode);
            }
            if (this._sortMode != SpriteSortMode.Immediate)
                this.Setup();
            this._batcher.DrawBatch(this._sortMode);
            if (!this._batcher.hasTexturedGeometryItems)
                return;
            if (this._sortMode != SpriteSortMode.Immediate)
                this.Setup();
            this._batcher.DrawTexturedGeometryBatch(this._sortMode);
        }

        public void ReapplyEffect(bool simple = false)
        {
            GraphicsDevice graphicsDevice = this.GraphicsDevice;
            graphicsDevice.BlendState = this._blendState;
            graphicsDevice.DepthStencilState = this._depthStencilState;
            graphicsDevice.RasterizerState = this._rasterizerState;
            graphicsDevice.SamplerStates[0] = this._samplerState;
            if (simple)
                this._simpleEffect.effect.CurrentTechnique.Passes[0].Apply();
            else
                this._spriteEffect.effect.CurrentTechnique.Passes[0].Apply();
        }

        public Matrix viewMatrix => this._matrix;

        public Matrix projMatrix => this._projMatrix;

        public void Setup(bool simple = false)
        {
            GraphicsDevice graphicsDevice = this.GraphicsDevice;
            graphicsDevice.BlendState = this._blendState;
            graphicsDevice.DepthStencilState = this._depthStencilState;
            graphicsDevice.RasterizerState = this._rasterizerState;
            graphicsDevice.SamplerStates[0] = this._samplerState;
            Viewport viewport = graphicsDevice.Viewport;
            Matrix.CreateOrthographicOffCenter(0.0f, viewport.Width, viewport.Height, 0.0f, 1f, -1f, out this._projMatrix);
            if (!Program.isLinux)
            {
                this._projMatrix.M41 += -0.5f * this._projMatrix.M11;
                this._projMatrix.M42 += -0.5f * this._projMatrix.M22;
            }
            Matrix result;
            Matrix.Multiply(ref this._matrix, ref this._projMatrix, out result);
            this.fullMatrix = result;
            if (simple)
            {
                this._matrixTransformSimple.SetValue((Microsoft.Xna.Framework.Matrix)result);
                this._simpleEffect.effect.CurrentTechnique.Passes[0].Apply();
            }
            else
            {
                this._matrixTransformSprite.SetValue((Microsoft.Xna.Framework.Matrix)result);
                this._spriteEffect.effect.CurrentTechnique.Passes[0].Apply();
            }
            if (this._effect == null)
                return;
            this._effect.CurrentTechnique = !simple || this._effect.Techniques.Count <= 1 || !(this._effect.Techniques[1].Name == "BasicSimple") ? this._effect.Techniques[0] : this._effect.Techniques[1];
            this._effect.Parameters["MatrixTransform"]?.SetValue((Microsoft.Xna.Framework.Matrix)result);
            this._effect.CurrentTechnique.Passes[0].Apply();
        }

        private void CheckValid(Tex2D texture)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));
            if (!this._beginCalled)
                throw new InvalidOperationException("Draw was called, but Begin has not yet been called. Begin must be called successfully before you can call Draw.");
        }

        //private void CheckValid(SpriteFont spriteFont, string text)
        //{
        //    if (spriteFont == null)
        //        throw new ArgumentNullException(nameof(spriteFont));
        //    if (text == null)
        //        throw new ArgumentNullException(nameof(text));
        //    if (!this._beginCalled)
        //        throw new InvalidOperationException("DrawString was called, but Begin has not yet been called. Begin must be called successfully before you can call DrawString.");
        //}

        //private void CheckValid(SpriteFont spriteFont, StringBuilder text)
        //{
        //    if (spriteFont == null)
        //        throw new ArgumentNullException(nameof(spriteFont));
        //    if (text == null)
        //        throw new ArgumentNullException(nameof(text));
        //    if (!this._beginCalled)
        //        throw new InvalidOperationException("DrawString was called, but Begin has not yet been called. Begin must be called successfully before you can call DrawString.");
        //}

        public GeometryItem GetGeometryItem() => this._batcher.GetGeometryItem();

        public static GeometryItem CreateGeometryItem() => MTSpriteBatcher.CreateGeometryItem();

        public void SubmitGeometry(GeometryItem geo) => this._batcher.SubmitGeometryItem(geo);

        public static GeometryItemTexture CreateTexturedGeometryItem() => MTSpriteBatcher.CreateTexturedGeometryItem();

        public void SubmitTexturedGeometry(GeometryItemTexture geo) => this._batcher.SubmitTexturedGeometryItem(geo);

        /// <summary>
        /// This is a MonoGame Extension method for calling Draw() using named parameters.  It is not available in the standard XNA Framework.
        /// </summary>
        /// <param name="texture">The Texture2D to draw.  Required.</param>
        /// <param name="position">
        /// The position to draw at.  If left empty, the method will draw at drawRectangle instead.
        /// </param>
        /// <param name="drawRectangle">
        /// The rectangle to draw at.  If left empty, the method will draw at position instead.
        /// </param>
        /// <param name="sourceRectangle">
        /// The source rectangle of the texture.  Default is null
        /// </param>
        /// <param name="origin">
        /// Origin of the texture.  Default is Vector2.Zero
        /// </param>
        /// <param name="rotation">Rotation of the texture.  Default is 0f</param>
        /// <param name="scale">
        /// The scale of the texture as a Vector2.  Default is Vector2.One
        /// </param>
        /// <param name="color">
        /// Color of the texture.  Default is Color.White
        /// </param>
        /// <param name="effect">
        /// SpriteEffect to draw with.  Default is SpriteEffects.None
        /// </param>
        /// <param name="depth">Draw depth.  Default is 0f.</param>
        public void Draw(
          Tex2D texture,
          Vec2? position = null,
          Rectangle? drawRectangle = null,
          Rectangle? sourceRectangle = null,
          Vec2? origin = null,
          float rotation = 0.0f,
          Vec2? scale = null,
          Color? color = null,
          SpriteEffects effect = SpriteEffects.None,
          float depth = 0.0f)
        {
            if (!color.HasValue)
                color = new Color?(Color.White);
            if (!origin.HasValue)
                origin = new Vec2?(Vec2.Zero);
            if (!scale.HasValue)
                scale = new Vec2?(Vec2.One);
            if (drawRectangle.HasValue == position.HasValue)
                throw new InvalidOperationException("Expected drawRectangle or position, but received neither or both.");
            if (position.HasValue)
                this.Draw(texture, position.Value, sourceRectangle, color.Value, rotation, origin.Value, scale.Value, effect, depth);
            else
                this.Draw(texture, drawRectangle.Value, sourceRectangle, color.Value, rotation, origin.Value, effect, depth);
        }

        public void Draw(
          Tex2D texture,
          Vec2 position,
          Rectangle? sourceRectangle,
          Color color,
          float rotation,
          Vec2 origin,
          Vec2 scale,
          SpriteEffects effect,
          float depth)
        {
            this.CheckValid(texture);
            float z = texture.width * scale.x;
            float w = texture.height * scale.y;
            if (sourceRectangle.HasValue)
            {
                z = sourceRectangle.Value.width * scale.x;
                w = sourceRectangle.Value.height * scale.y;
            }
            this.DoDrawInternal(texture, new Vec4(position.x, position.y, z, w), sourceRectangle, color, rotation, origin * scale, effect, depth, true, null);
        }

        public void DrawWithMaterial(
          Tex2D texture,
          Vec2 position,
          Rectangle? sourceRectangle,
          Color color,
          float rotation,
          Vec2 origin,
          Vec2 scale,
          SpriteEffects effect,
          float depth,
          Material fx)
        {
            this.CheckValid(texture);
            float z = texture.width * scale.x;
            float w = texture.height * scale.y;
            if (sourceRectangle.HasValue)
            {
                z = sourceRectangle.Value.width * scale.x;
                w = sourceRectangle.Value.height * scale.y;
            }
            this.DoDrawInternal(texture, new Vec4(position.x, position.y, z, w), sourceRectangle, color, rotation, origin * scale, effect, depth, true, fx);
        }

        public void Draw(
          Tex2D texture,
          Vec2 position,
          Rectangle? sourceRectangle,
          Color color,
          float rotation,
          Vec2 origin,
          float scale,
          SpriteEffects effect,
          float depth)
        {
            this.CheckValid(texture);
            float z = texture.width * scale;
            float w = texture.height * scale;
            if (sourceRectangle.HasValue)
            {
                z = sourceRectangle.Value.width * scale;
                w = sourceRectangle.Value.height * scale;
            }
            this.DoDrawInternal(texture, new Vec4(position.x, position.y, z, w), sourceRectangle, color, rotation, origin * scale, effect, depth, true, null);
        }

        public void Draw(
          Tex2D texture,
          Rectangle destinationRectangle,
          Rectangle? sourceRectangle,
          Color color,
          float rotation,
          Vec2 origin,
          SpriteEffects effect,
          float depth)
        {
            this.CheckValid(texture);
            this.DoDrawInternal(texture, new Vec4(destinationRectangle.x, destinationRectangle.y, destinationRectangle.width, destinationRectangle.height), sourceRectangle, color, rotation, new Vec2(origin.x * (destinationRectangle.width / (!sourceRectangle.HasValue || sourceRectangle.Value.width == 0.0 ? texture.width : sourceRectangle.Value.width)), (float)(origin.y * (double)destinationRectangle.height / (!sourceRectangle.HasValue || sourceRectangle.Value.height == 0.0 ? texture.height : sourceRectangle.Value.height))), effect, depth, true, null);
        }

        public void DrawQuad(
          Vec2 p1,
          Vec2 p2,
          Vec2 p3,
          Vec2 p4,
          Vec2 t1,
          Vec2 t2,
          Vec2 t3,
          Vec2 t4,
          float depth,
          Tex2D tex,
          Color c)
        {
            ++DuckGame.Graphics.currentDrawIndex;
            MTSpriteBatchItem batchItem = this._batcher.CreateBatchItem();
            batchItem.Depth = depth;
            batchItem.Texture = tex.nativeObject as Texture2D;
            batchItem.Material = null;
            batchItem.Set(p1, p2, p3, p4, t1, t2, t3, t4, c);
        }

        internal void DoDrawInternal(
          Tex2D texture,
          Vec4 destinationRectangle,
          Rectangle? sourceRectangle,
          Color color,
          float rotation,
          Vec2 origin,
          SpriteEffects effect,
          float depth,
          bool autoFlush,
          Material fx)
        {
            ++DuckGame.Graphics.currentDrawIndex;
            MTSpriteBatchItem batchItem = this._batcher.CreateBatchItem();
            batchItem.Depth = depth;
            batchItem.Texture = texture.nativeObject as Texture2D;
            batchItem.Material = fx;
            if (sourceRectangle.HasValue)
            {
                this._tempRect = sourceRectangle.Value;
            }
            else
            {
                this._tempRect.x = 0.0f;
                this._tempRect.y = 0.0f;
                this._tempRect.width = texture.width;
                this._tempRect.height = texture.height;
            }
            this._texCoordTL.x = this._tempRect.x / texture.width + MTSpriteBatch.edgeBias;
            this._texCoordTL.y = this._tempRect.y / texture.height + MTSpriteBatch.edgeBias;
            this._texCoordBR.x = (this._tempRect.x + this._tempRect.width) / texture.width - MTSpriteBatch.edgeBias;
            this._texCoordBR.y = (this._tempRect.y + this._tempRect.height) / texture.height - MTSpriteBatch.edgeBias;
            if ((effect & SpriteEffects.FlipVertically) != SpriteEffects.None)
            {
                float y = this._texCoordBR.y;
                this._texCoordBR.y = this._texCoordTL.y;
                this._texCoordTL.y = y;
            }
            if ((effect & SpriteEffects.FlipHorizontally) != SpriteEffects.None)
            {
                float x = this._texCoordBR.x;
                this._texCoordBR.x = this._texCoordTL.x;
                this._texCoordTL.x = x;
            }
            batchItem.Set(destinationRectangle.x, destinationRectangle.y, -origin.x, -origin.y, destinationRectangle.z, destinationRectangle.w, (float)Math.Sin((double)rotation), (float)Math.Cos((double)rotation), color, this._texCoordTL, this._texCoordBR);
            if (DuckGame.Graphics.recordMetadata)
            {
                batchItem.MetaData = new MTSpriteBatchItemMetaData
                {
                    texture = texture,
                    rotation = rotation,
                    color = color,
                    tempRect = this._tempRect,
                    effect = effect,
                    depth = depth
                };
            }
            if (!DuckGame.Graphics.skipReplayRender && Recorder.currentRecording != null && DuckGame.Graphics.currentRenderTarget == null)
                Recorder.currentRecording.LogDraw(texture.textureIndex, new Vec2(batchItem.vertexTL.Position.X, batchItem.vertexTL.Position.Y), new Vec2(batchItem.vertexBR.Position.X, batchItem.vertexBR.Position.Y), rotation, color, (short)this._tempRect.x, (short)this._tempRect.y, (short)(_tempRect.width * ((effect & SpriteEffects.FlipHorizontally) != SpriteEffects.None ? -1.0 : 1.0)), (short)(_tempRect.height * ((effect & SpriteEffects.FlipVertically) != SpriteEffects.None ? -1.0 : 1.0)), depth);
            if (!autoFlush)
                return;
            this.FlushIfNeeded();
        }

        public void DrawExistingBatchItem(MTSpriteBatchItem item)
        {
            ++DuckGame.Graphics.currentDrawIndex;
            this._batcher.SqueezeInItem(item);
            if (Recorder.currentRecording == null)
                return;
            Recorder.currentRecording.LogDraw(item.MetaData.texture.textureIndex, new Vec2(item.vertexTL.Position.X, item.vertexTL.Position.Y), new Vec2(item.vertexBR.Position.X, item.vertexBR.Position.Y), item.MetaData.rotation, item.MetaData.color, (short)item.MetaData.tempRect.x, (short)item.MetaData.tempRect.y, (short)(item.MetaData.tempRect.width * ((item.MetaData.effect & SpriteEffects.FlipHorizontally) != SpriteEffects.None ? -1.0 : 1.0)), (short)(item.MetaData.tempRect.height * ((item.MetaData.effect & SpriteEffects.FlipVertically) != SpriteEffects.None ? -1.0 : 1.0)), item.MetaData.depth);
        }

        public void DrawRecorderItem(ref RecorderFrameItem frame)
        {
            MTSpriteBatchItem batchItem = this._batcher.CreateBatchItem();
            batchItem.Depth = frame.depth;
            if (frame.texture == -1)
            {
                batchItem.Texture = DuckGame.Graphics.blankWhiteSquare.nativeObject as Texture2D;
            }
            else
            {
                Tex2D tex2DfromIndex = Content.GetTex2DFromIndex(frame.texture);
                if (tex2DfromIndex == null)
                    return;
                batchItem.Texture = tex2DfromIndex.nativeObject as Texture2D;
            }
            if (batchItem.Texture == null)
                return;
            float num1 = Math.Abs(frame.texW);
            float num2 = Math.Abs(frame.texH);
            this._texCoordTL.x = frame.texX / (float)batchItem.Texture.Width + MTSpriteBatch.edgeBias;
            this._texCoordTL.y = frame.texY / (float)batchItem.Texture.Height + MTSpriteBatch.edgeBias;
            this._texCoordBR.x = (frame.texX + num1) / batchItem.Texture.Width - MTSpriteBatch.edgeBias;
            this._texCoordBR.y = (frame.texY + num2) / batchItem.Texture.Height - MTSpriteBatch.edgeBias;
            if (frame.texH < 0)
            {
                float y = this._texCoordBR.y;
                this._texCoordBR.y = this._texCoordTL.y;
                this._texCoordTL.y = y;
            }
            if (frame.texW < 0)
            {
                float x = this._texCoordBR.x;
                this._texCoordBR.x = this._texCoordTL.x;
                this._texCoordTL.x = x;
            }
            Vec2 vec2 = frame.bottomRight.Rotate(-frame.rotation, frame.topLeft);
            batchItem.Set(frame.topLeft.x, frame.topLeft.y, 0.0f, 0.0f, vec2.x - frame.topLeft.x, vec2.y - frame.topLeft.y, (float)Math.Sin(frame.rotation), (float)Math.Cos(frame.rotation), frame.color, this._texCoordTL, this._texCoordBR);
        }

        public void Flush(bool doSetup)
        {
            if (doSetup)
                this.Setup();
            this._batcher.DrawBatch(this._sortMode);
        }

        public void FlushSettingScissor()
        {
            this.Setup();
            this._batcher.DrawBatch(this._sortMode);
            this._prevRast = this.GraphicsDevice.RasterizerState;
            this.GraphicsDevice.RasterizerState = new RasterizerState()
            {
                CullMode = this._rasterizerState.CullMode,
                FillMode = this._rasterizerState.FillMode,
                SlopeScaleDepthBias = this._rasterizerState.SlopeScaleDepthBias,
                MultiSampleAntiAlias = this._rasterizerState.MultiSampleAntiAlias,
                ScissorTestEnable = true
            };
        }

        public void FlushAndClearScissor()
        {
            this._batcher.DrawBatch(this._sortMode);
            this.GraphicsDevice.RasterizerState = this._prevRast;
        }

        internal void FlushIfNeeded()
        {
            if (this._sortMode != SpriteSortMode.Immediate)
                return;
            this._batcher.DrawBatch(this._sortMode);
        }

        public void Draw(Tex2D texture, Vec2 position, Rectangle? sourceRectangle, Color color) => this.Draw(texture, position, sourceRectangle, color, 0.0f, Vec2.Zero, 1f, SpriteEffects.None, 0.0f);

        public void Draw(
          Tex2D texture,
          Rectangle destinationRectangle,
          Rectangle? sourceRectangle,
          Color color)
        {
            this.Draw(texture, destinationRectangle, sourceRectangle, color, 0.0f, Vec2.Zero, SpriteEffects.None, 0.0f);
        }

        public void Draw(Tex2D texture, Vec2 position, Color color) => this.Draw(texture, position, new Rectangle?(), color);

        public void Draw(Tex2D texture, Rectangle rectangle, Color color) => this.Draw(texture, rectangle, new Rectangle?(), color);

        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed && disposing && this._spriteEffect != null)
            {
                this._spriteEffect.effect.Dispose();
                this._spriteEffect = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>Obsolete, use DoDrawInternal()</summary>
        /// <param name="texture"></param>
        /// <param name="destinationRectangle"></param>
        /// <param name="sourceRectangle"></param>
        /// <param name="color"></param>
        /// <param name="rotation"></param>
        /// <param name="origin"></param>
        /// <param name="effect"></param>
        /// <param name="depth"></param>
        /// <param name="autoFlush"></param>
        /// <param name="fx"></param>
        internal void DoDrawInternalTex2D(
          Tex2D texture,
          Vec4 destinationRectangle,
          Rectangle? sourceRectangle,
          Color color,
          float rotation,
          Vec2 origin,
          SpriteEffects effect,
          float depth,
          bool autoFlush,
          Material fx)
        {
            this.DoDrawInternal(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effect, depth, autoFlush, fx);
        }
    }
}
