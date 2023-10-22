
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

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
        private Rectangle _tempRect = new Rectangle(0f, 0f, 0f, 0f);
        private Vec2 _texCoordTL = new Vec2(0f, 0f);
        private Vec2 _texCoordBR = new Vec2(0f, 0f);
        private Matrix _projMatrix;
        public Matrix fullMatrix;
        public static float edgeBias = 1E-05f;
        private RasterizerState _prevRast;

        public IEnumerable<MTSpriteBatchItem> StealSpriteBatchItems()
        {
            return _batcher.StealSpriteBatchItems();
        }
        public int batchlistCount
        {
            get
            {
                return _batcher.batchlistCount;
            }
        }
        public MTSpriteBatchItem StealLastSpriteBatchItem() => _batcher.StealLastBatchItem();

        public bool transitionEffect => Layer.basicWireframeEffect != null && _effect == Layer.basicWireframeEffect.effect;

        public MTEffect SpriteEffect => _spriteEffect;

        public MTEffect SimpleEffect => _simpleEffect;

        public MTSpriteBatch(GraphicsDevice graphicsDevice)
          : base(graphicsDevice)
        {
            if (graphicsDevice == null)
                throw new ArgumentException(nameof(graphicsDevice));
            _spriteEffect = Content.Load<MTEffect>("Shaders/SpriteEffect");
            _matrixTransformSprite = _spriteEffect.effect.Parameters["MatrixTransform"];
            _simpleEffect = Content.Load<MTEffect>("Shaders/SpriteEffectSimple");
            _matrixTransformSimple = _simpleEffect.effect.Parameters["MatrixTransform"];
            _batcher = new MTSpriteBatcher(graphicsDevice, this);
            _beginCalled = false;
        }

        public new void Begin() => Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Matrix.Identity);

        public void Begin(
          SpriteSortMode sortMode,
          BlendState blendState,
          SamplerState samplerState,
          DepthStencilState depthStencilState,
          RasterizerState rasterizerState,
          MTEffect effect,
          Matrix transformMatrix)
        {
            if (sortMode == SpriteSortMode.BackToFront)
            {
                _batcher.depthmod = -1f;
            }
            Graphics.currentStateIndex = _globalIndex;
            if (_beginCalled)
                throw new InvalidOperationException("Begin cannot be called again until End has been successfully called.");
            base.Begin();
            if (Recorder.currentRecording != null)
                Recorder.currentRecording.StateChange(sortMode, blendState, samplerState, depthStencilState, rasterizerState, Layer.IsBasicLayerEffect(effect) ? Layer.basicLayerEffect : effect, transformMatrix, (Rectangle)GraphicsDevice.ScissorRectangle);
            _sortMode = sortMode;
            _blendState = blendState ?? BlendState.AlphaBlend;
            _samplerState = samplerState ?? SamplerState.LinearClamp;
            _depthStencilState = depthStencilState ?? DepthStencilState.None;
            _rasterizerState = rasterizerState ?? RasterizerState.CullCounterClockwise;
            _effect = (Effect)effect;
            _matrix = transformMatrix;
            if (sortMode == SpriteSortMode.Immediate)
                Setup();
            _beginCalled = true;
        }

        public new void Begin(SpriteSortMode sortMode, BlendState blendState) => Begin(sortMode, blendState, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Matrix.Identity);

        public new void Begin(
          SpriteSortMode sortMode,
          BlendState blendState,
          SamplerState samplerState,
          DepthStencilState depthStencilState,
          RasterizerState rasterizerState)
        {
            Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, null, Matrix.Identity);
        }

        public void Begin(
          SpriteSortMode sortMode,
          BlendState blendState,
          SamplerState samplerState,
          DepthStencilState depthStencilState,
          RasterizerState rasterizerState,
          MTEffect effect)
        {
            Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, Matrix.Identity);
        }

        public new void End()
        {
            _beginCalled = false;
            base.End();
            if (Graphics.recordOnly)
                return;
            if (_batcher.hasSimpleItems)
            {
                if (_sortMode != SpriteSortMode.Immediate)
                    Setup(true);
                _batcher.DrawSimpleBatch(_sortMode);
            }
            if (_batcher.hasGeometryItems)
            {
                if (_sortMode != SpriteSortMode.Immediate)
                    Setup(true);
                _batcher.DrawGeometryBatch(_sortMode);
            }
            if (_sortMode != SpriteSortMode.Immediate)
                Setup();
            _batcher.DrawBatch(_sortMode);
            if (!_batcher.hasTexturedGeometryItems)
                return;
            if (_sortMode != SpriteSortMode.Immediate)
                Setup();
            _batcher.DrawTexturedGeometryBatch(_sortMode);
        }

        public void ReapplyEffect(bool simple = false)
        {
            GraphicsDevice graphicsDevice = GraphicsDevice;
            graphicsDevice.BlendState = _blendState;
            graphicsDevice.DepthStencilState = _depthStencilState;
            graphicsDevice.RasterizerState = _rasterizerState;
            graphicsDevice.SamplerStates[0] = _samplerState;
            if (simple)
                _simpleEffect.effect.CurrentTechnique.Passes[0].Apply();
            else
                _spriteEffect.effect.CurrentTechnique.Passes[0].Apply();
        }

        public Matrix viewMatrix => _matrix;

        public Matrix projMatrix => _projMatrix;

        public void Setup(bool simple = false)
        {
            GraphicsDevice graphicsDevice = GraphicsDevice;
            graphicsDevice.BlendState = _blendState;
            graphicsDevice.DepthStencilState = _depthStencilState;
            graphicsDevice.RasterizerState = _rasterizerState;
            graphicsDevice.SamplerStates[0] = _samplerState;
            Viewport viewport = graphicsDevice.Viewport;
            Matrix.CreateOrthographicOffCenter(0f, viewport.Width, viewport.Height, 0f, 1f, -1f, out _projMatrix);
            //if (!Program.isLinux)
            //{
            //    _projMatrix.M41 += -0.5f * _projMatrix.M11;
            //    _projMatrix.M42 += -0.5f * _projMatrix.M22;
            //}
            Matrix result;
            Matrix.Multiply(ref _matrix, ref _projMatrix, out result);
            fullMatrix = result;
            if (simple)
            {
                _matrixTransformSimple.SetValue((Microsoft.Xna.Framework.Matrix)result);
                _simpleEffect.effect.CurrentTechnique.Passes[0].Apply();
            }
            else
            {
                _matrixTransformSprite.SetValue((Microsoft.Xna.Framework.Matrix)result);
                _spriteEffect.effect.CurrentTechnique.Passes[0].Apply();
            }
            if (_effect == null)
                return;
            _effect.CurrentTechnique = !simple || _effect.Techniques.Count <= 1 || !(_effect.Techniques[1].Name == "BasicSimple") ? _effect.Techniques[0] : _effect.Techniques[1];
            _effect.Parameters["MatrixTransform"]?.SetValue((Microsoft.Xna.Framework.Matrix)result);
            _effect.CurrentTechnique.Passes[0].Apply();
        }

        private void CheckValid(Tex2D texture)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));
            if (!_beginCalled)
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

        public GeometryItem GetGeometryItem() => _batcher.GetGeometryItem();

        public static GeometryItem CreateGeometryItem() => MTSpriteBatcher.CreateGeometryItem();

        public void SubmitGeometry(GeometryItem geo) => _batcher.SubmitGeometryItem(geo);

        public static GeometryItemTexture CreateTexturedGeometryItem() => MTSpriteBatcher.CreateTexturedGeometryItem();

        public void SubmitTexturedGeometry(GeometryItemTexture geo) => _batcher.SubmitTexturedGeometryItem(geo);

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
          float rotation = 0f,
          Vec2? scale = null,
          Color? color = null,
          SpriteEffects effect = SpriteEffects.None,
          float depth = 0f)
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
                Draw(texture, position.Value, sourceRectangle, color.Value, rotation, origin.Value, scale.Value, effect, depth);
            else
                Draw(texture, drawRectangle.Value, sourceRectangle, color.Value, rotation, origin.Value, effect, depth);
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
            //CheckValid(texture);
            float z = texture.width * scale.x;
            float w = texture.height * scale.y;
            if (sourceRectangle.HasValue)
            {
                z = sourceRectangle.Value.width * scale.x;
                w = sourceRectangle.Value.height * scale.y;
            }
            DoDrawInternal(texture, new Vec4(position.x, position.y, z, w), sourceRectangle, color, rotation, origin * scale, effect, depth, true, null);
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
            //CheckValid(texture);
            float z = texture.width * scale.x;
            float w = texture.height * scale.y;
            if (sourceRectangle.HasValue)
            {
                z = sourceRectangle.Value.width * scale.x;
                w = sourceRectangle.Value.height * scale.y;
            }
            DoDrawInternal(texture, new Vec4(position.x, position.y, z, w), sourceRectangle, color, rotation, origin * scale, effect, depth, true, fx);
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
            //CheckValid(texture);
            float z = texture.width * scale;
            float w = texture.height * scale;
            if (sourceRectangle.HasValue)
            {
                z = sourceRectangle.Value.width * scale;
                w = sourceRectangle.Value.height * scale;
            }
            DoDrawInternal(texture, new Vec4(position.x, position.y, z, w), sourceRectangle, color, rotation, origin * scale, effect, depth, true, null);
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
            //CheckValid(texture);
            DoDrawInternal(texture, new Vec4(destinationRectangle.x, destinationRectangle.y, destinationRectangle.width, destinationRectangle.height), sourceRectangle, color, rotation, new Vec2(origin.x * (destinationRectangle.width / (!sourceRectangle.HasValue || sourceRectangle.Value.width == 0f ? texture.width : sourceRectangle.Value.width)), (float)(origin.y * destinationRectangle.height / (!sourceRectangle.HasValue || sourceRectangle.Value.height == 0f ? texture.height : sourceRectangle.Value.height))), effect, depth, true, null);
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
          Tex2D texture,
          Color c)
        {
            ++Graphics.currentDrawIndex;
            MTSpriteBatchItem batchItem = _batcher.CreateBatchItemDepth(depth);
            batchItem.Depth = depth;

            if (texture.Texbase != null && texture.Texbase.Name != null && Content.offests.TryGetValue(texture.Texbase.Name, out Microsoft.Xna.Framework.Rectangle offset))
            {
                batchItem.usingspriteatlas = true;
                batchItem.NormalTexture = texture;
                batchItem.Texture = Content.SpriteAtlasTex2D.Texbase;

                //float sizeoffsetx = (float)offset.Left / (float)Content.Thick.width;
                //float sizeoffsety = (float)offset.Top / (float)Content.Thick.height;
                t1.x = ((t1.x * texture.width) + offset.Left) / Content.SpriteAtlasTex2D.height;
                t2.x = ((t2.x * texture.width) + offset.Left) / Content.SpriteAtlasTex2D.height;
                t3.x = ((t3.x * texture.width) + offset.Left) / Content.SpriteAtlasTex2D.height;
                t4.x = ((t4.x * texture.width) + offset.Left) / Content.SpriteAtlasTex2D.height;

                t1.y = ((t1.y * texture.height) + offset.Top) / Content.SpriteAtlasTex2D.height;
                t2.y = ((t1.y * texture.height) + offset.Top) / Content.SpriteAtlasTex2D.height;
                t3.y = ((t1.y * texture.height) + offset.Top) / Content.SpriteAtlasTex2D.height;
                t4.y = ((t1.y * texture.height) + offset.Top) / Content.SpriteAtlasTex2D.height;
                //offset
                //_tempRect.x += offset.Left;
                //_tempRect.y += offset.Top;
                //_texCoordTL.x = _tempRect.x / Content.Thick.width + MTSpriteBatch.edgeBias;
                //_texCoordTL.y = _tempRect.y / Content.Thick.height + MTSpriteBatch.edgeBias;
                //_texCoordBR.x = (_tempRect.x + _tempRect.width) / Content.Thick.width - MTSpriteBatch.edgeBias;
                //_texCoordBR.y = (_tempRect.y + _tempRect.height) / Content.Thick.height - MTSpriteBatch.edgeBias;
            }
            else
            {
                batchItem.NormalTexture = texture;
                batchItem.Texture = texture.Texbase;//texture.nativeObject as Texture2D;
            }
            //if (!MTSpriteBatcher.Texidonthave.Contains(batchItem.Texture) && (batchItem.Texture == null || batchItem.Texture.Name == null || batchItem.Texture.Name != "SpriteAtlas"))
            //{
            //    if (batchItem.Texture != null && batchItem.Texture.Name != null)
            //    {
            //        DevConsole.Log("DrawQuad " + batchItem.Texture.Name);
            //    }
            //    MTSpriteBatcher.Texidonthave.Add(batchItem.Texture);
            //}



            batchItem.Material = null;
            batchItem.Set(p1, p2, p3, p4, t1, t2, t3, t4, c);
        }
        private static List<string> usednames = new List<string>();
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
            ++Graphics.currentDrawIndex;
            MTSpriteBatchItem batchItem = _batcher.CreateBatchItemDepth(depth);
            batchItem.Depth = depth;

            if (sourceRectangle.HasValue)
            {
                _tempRect = sourceRectangle.Value;
            }
            else
            {
                _tempRect.x = 0f;
                _tempRect.y = 0f;
                _tempRect.width = texture.width;
                _tempRect.height = texture.height;
            }
            if ((fx == null || fx.spsupport) && !texture.skipSpriteAtlas && texture.Texbase != null && texture.Texbase.Name != null && Content.offests.TryGetValue(texture.Texbase.Name, out Microsoft.Xna.Framework.Rectangle offset))
            {
                batchItem.NormalTexture = texture;
                batchItem.usingspriteatlas = true;
                batchItem.Texture = Content.SpriteAtlasTex2D.Texbase;
                batchItem.Material = fx;
                //offset
                _tempRect.x += offset.Left;
                _tempRect.y += offset.Top;
                _texCoordTL.x = _tempRect.x / Content.SpriteAtlasTex2D.width + edgeBias;
                _texCoordTL.y = _tempRect.y / Content.SpriteAtlasTex2D.height + edgeBias;
                _texCoordBR.x = (_tempRect.x + _tempRect.width) / Content.SpriteAtlasTex2D.width - edgeBias;
                _texCoordBR.y = (_tempRect.y + _tempRect.height) / Content.SpriteAtlasTex2D.height - edgeBias;
                if ((effect & SpriteEffects.FlipVertically) != SpriteEffects.None)
                {
                    float y = _texCoordBR.y;
                    _texCoordBR.y = _texCoordTL.y;
                    _texCoordTL.y = y;
                }
                if ((effect & SpriteEffects.FlipHorizontally) != SpriteEffects.None)
                {
                    float x = _texCoordBR.x;
                    _texCoordBR.x = _texCoordTL.x;
                    _texCoordTL.x = x;
                }
                batchItem.Set(destinationRectangle.x, destinationRectangle.y, -origin.x, -origin.y, destinationRectangle.z, destinationRectangle.w, (float)Math.Sin(rotation), (float)Math.Cos(rotation), color, _texCoordTL, _texCoordBR);
                if (Graphics.recordMetadata)
                {
                    batchItem.MetaData = new MTSpriteBatchItemMetaData
                    {
                        texture = Content.SpriteAtlasTex2D,
                        rotation = rotation,
                        color = color,
                        tempRect = _tempRect,
                        effect = effect,
                        depth = depth
                    };
                }
                if (!Graphics.skipReplayRender && Recorder.currentRecording != null && Graphics.currentRenderTarget == null)
                    Recorder.currentRecording.LogDraw(Content.SpriteAtlasTex2D.textureIndex, new Vec2(batchItem.vertexTL.Position.X, batchItem.vertexTL.Position.Y), new Vec2(batchItem.vertexBR.Position.X, batchItem.vertexBR.Position.Y), rotation, color, (short)_tempRect.x, (short)_tempRect.y, (short)(_tempRect.width * ((effect & SpriteEffects.FlipHorizontally) != SpriteEffects.None ? -1f : 1f)), (short)(_tempRect.height * ((effect & SpriteEffects.FlipVertically) != SpriteEffects.None ? -1f : 1f)), depth);
                if (!autoFlush)
                    return;
                FlushIfNeeded();
                return;

            }
            batchItem.NormalTexture = texture;
            batchItem.Texture = texture.Texbase;
            batchItem.Material = fx;
            _texCoordTL.x = _tempRect.x / texture.width + edgeBias;
            _texCoordTL.y = _tempRect.y / texture.height + edgeBias;
            _texCoordBR.x = (_tempRect.x + _tempRect.width) / texture.width - edgeBias;
            _texCoordBR.y = (_tempRect.y + _tempRect.height) / texture.height - edgeBias;
            if ((effect & SpriteEffects.FlipVertically) != SpriteEffects.None)
            {
                float y = _texCoordBR.y;
                _texCoordBR.y = _texCoordTL.y;
                _texCoordTL.y = y;
            }
            if ((effect & SpriteEffects.FlipHorizontally) != SpriteEffects.None)
            {
                float x = _texCoordBR.x;
                _texCoordBR.x = _texCoordTL.x;
                _texCoordTL.x = x;
            }
            //if (!MTSpriteBatcher.Texidonthave.Contains(batchItem.Texture) && (batchItem.Texture == null || batchItem.Texture.Name == null || batchItem.Texture.Name != "SpriteAtlas"))
            //{
            //    StackTrace st = new StackTrace(true);
            //    if (batchItem.Texture != null && batchItem.Texture.Name != null)
            //    {
            //        DevConsole.Log("DoDrawInternal " + batchItem.Texture.Name);
            //    }
            //    string stacktracepath = "";
            //    List<string> keepgoingback = new List<string>() { "MTSpriteBatch", "Graphics", "SpriteMap", "Sprite", "Thing" };
            //    for (int i = 0; i < st.FrameCount; i++)
            //    {
            //        string classname = st.GetFrame(i).GetMethod().DeclaringType.Name;
            //        if (classname == "HatSelector")
            //        {
            //            DevConsole.Log("fuck");
            //        }
            //        if (!keepgoingback.Contains(classname))
            //        {
            //            string path = st.GetFrame(i).GetMethod().GetFullName();
            //            int index = 1;
            //            while (usednames.Contains(path + " " + index.ToString()))
            //            {
            //                index += 1;
            //            }
            //            path += " " + index.ToString();
            //            usednames.Add(path);
            //            if (batchItem.Texture.Name == null)
            //            {
            //                batchItem.Texture.Name = "unnamed" + " " + stacktracepath + path.Replace("DuckGame.", "").Replace(":", ".");
            //            }
            //            break;
            //        }
            //        stacktracepath += st.GetFrame(i).GetMethod().GetFullName().Replace("DuckGame.", "").Replace(":", ".") + "*";
            //    }
            //    //st.GetFrame(0).GetMethod().DeclaringType.Name
            //    MTSpriteBatcher.Texidonthave.Add(batchItem.Texture);
            //}
            batchItem.Set(destinationRectangle.x, destinationRectangle.y, -origin.x, -origin.y, destinationRectangle.z, destinationRectangle.w, (float)Math.Sin(rotation), (float)Math.Cos(rotation), color, _texCoordTL, _texCoordBR);
            if (Graphics.recordMetadata)
            {
                batchItem.MetaData = new MTSpriteBatchItemMetaData
                {
                    texture = texture,
                    rotation = rotation,
                    color = color,
                    tempRect = _tempRect,
                    effect = effect,
                    depth = depth
                };
            }
            if (!Graphics.skipReplayRender && Recorder.currentRecording != null && Graphics.currentRenderTarget == null)
                Recorder.currentRecording.LogDraw(texture.textureIndex, new Vec2(batchItem.vertexTL.Position.X, batchItem.vertexTL.Position.Y), new Vec2(batchItem.vertexBR.Position.X, batchItem.vertexBR.Position.Y), rotation, color, (short)_tempRect.x, (short)_tempRect.y, (short)(_tempRect.width * ((effect & SpriteEffects.FlipHorizontally) != SpriteEffects.None ? -1f : 1f)), (short)(_tempRect.height * ((effect & SpriteEffects.FlipVertically) != SpriteEffects.None ? -1f : 1f)), depth);
            if (!autoFlush)
                return;
            FlushIfNeeded();
            //else
            //{
            //    string name = "null";
            //    if (batchItem.Texture != null && batchItem.Texture.Name != null)
            //    {
            //        name = batchItem.Texture.Name;
            //    }
            //    //DevConsole.Log(name);
            //}


        }

        public void DrawExistingBatchItem(MTSpriteBatchItem item)
        {
            ++Graphics.currentDrawIndex;
            _batcher.SqueezeInItem(item);
            //if (!MTSpriteBatcher.Texidonthave.Contains(item.Texture) && (item.Texture == null || item.Texture.Name == null || item.Texture.Name != "SpriteAtlas"))
            //{
            //    if (item.Texture != null && item.Texture.Name != null)
            //    {
            //        DevConsole.Log("DrawExistingBatchItem " + item.Texture.Name);
            //    }
            //    MTSpriteBatcher.Texidonthave.Add(item.Texture);
            //}
            //  if (Recorder.currentRecording == null)
            //      return;
            //  Recorder.currentRecording.LogDraw(item.MetaData.texture.textureIndex, new Vec2(item.vertexTL.Position.X, item.vertexTL.Position.Y), new Vec2(item.vertexBR.Position.X, item.vertexBR.Position.Y), item.MetaData.rotation, item.MetaData.color, (short)item.MetaData.tempRect.x, (short)item.MetaData.tempRect.y, (short)(item.MetaData.tempRect.width * ((item.MetaData.effect & SpriteEffects.FlipHorizontally) != SpriteEffects.None ? -1f : 1f)), (short)(item.MetaData.tempRect.height * ((item.MetaData.effect & SpriteEffects.FlipVertically) != SpriteEffects.None ? -1f : 1f)), item.MetaData.depth);
        }

        public void DrawRecorderItem(ref RecorderFrameItem frame)
        {
            MTSpriteBatchItem batchItem = _batcher.CreateBatchItemDepth(frame.depth);
            batchItem.Depth = frame.depth;
            if (frame.texture == -1)
            {
                batchItem.Texture = Graphics.blankWhiteSquare.nativeObject as Texture2D;
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
            _texCoordTL.x = frame.texX / (float)batchItem.Texture.Width + edgeBias;
            _texCoordTL.y = frame.texY / (float)batchItem.Texture.Height + edgeBias;
            _texCoordBR.x = (frame.texX + num1) / batchItem.Texture.Width - edgeBias;
            _texCoordBR.y = (frame.texY + num2) / batchItem.Texture.Height - edgeBias;
            if (frame.texH < 0)
            {
                float y = _texCoordBR.y;
                _texCoordBR.y = _texCoordTL.y;
                _texCoordTL.y = y;
            }
            if (frame.texW < 0)
            {
                float x = _texCoordBR.x;
                _texCoordBR.x = _texCoordTL.x;
                _texCoordTL.x = x;
            }
            Vec2 vec2 = frame.bottomRight.Rotate(-frame.rotation, frame.topLeft);
            batchItem.Set(frame.topLeft.x, frame.topLeft.y, 0f, 0f, vec2.x - frame.topLeft.x, vec2.y - frame.topLeft.y, (float)Math.Sin(frame.rotation), (float)Math.Cos(frame.rotation), frame.color, _texCoordTL, _texCoordBR);
        }

        public void Flush(bool doSetup)
        {
            if (doSetup)
                Setup();
            _batcher.DrawBatch(_sortMode);
        }

        public void FlushSettingScissor()
        {
            Setup();
            _batcher.DrawBatch(_sortMode);
            _prevRast = GraphicsDevice.RasterizerState;
            GraphicsDevice.RasterizerState = new RasterizerState()
            {
                CullMode = _rasterizerState.CullMode,
                FillMode = _rasterizerState.FillMode,
                SlopeScaleDepthBias = _rasterizerState.SlopeScaleDepthBias,
                MultiSampleAntiAlias = _rasterizerState.MultiSampleAntiAlias,
                ScissorTestEnable = true
            };
        }

        public void FlushAndClearScissor()
        {
            _batcher.DrawBatch(_sortMode);
            GraphicsDevice.RasterizerState = _prevRast;
        }

        internal void FlushIfNeeded()
        {
            if (_sortMode != SpriteSortMode.Immediate)
                return;
            _batcher.DrawBatch(_sortMode);
        }

        public void Draw(Tex2D texture, Vec2 position, Rectangle? sourceRectangle, Color color) => Draw(texture, position, sourceRectangle, color, 0f, Vec2.Zero, 1f, SpriteEffects.None, 0f);

        public void Draw(
          Tex2D texture,
          Rectangle destinationRectangle,
          Rectangle? sourceRectangle,
          Color color)
        {
            Draw(texture, destinationRectangle, sourceRectangle, color, 0f, Vec2.Zero, SpriteEffects.None, 0f);
        }

        public void Draw(Tex2D texture, Vec2 position, Color color) => Draw(texture, position, new Rectangle?(), color);

        public void Draw(Tex2D texture, Rectangle rectangle, Color color) => Draw(texture, rectangle, new Rectangle?(), color);

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing && _spriteEffect != null)
            {
                _spriteEffect.effect.Dispose();
                _spriteEffect = null;
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
            DoDrawInternal(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effect, depth, autoFlush, fx);
        }
    }
}