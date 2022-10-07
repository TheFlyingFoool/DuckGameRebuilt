using System;
using System.Collections.Generic;
using DuckGame.AddedContent.Drake.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame.AddedContent.Drake.PolyRender
{

    public sealed class PolygonBatcher : IDisposable
    {
        private readonly GraphicsDevice _device;
        private readonly GraphicsDeviceManager _manager;
        private readonly BasicEffect _effect;
        private readonly VertexPositionColorTexture[] _vertices;
        private readonly Stack<Rectangle> _scissorStack;

        private Vector3 _currentOffset;
        private int _bufferPos;
        private bool _isDisposed;

        public PolygonBatcher(GraphicsDeviceManager graphics, int bufferSize = 128)
        {
            _manager = graphics;
            _device = _manager?.GraphicsDevice ?? throw new InvalidOperationException("Cannot create a polygon batcher will a null graphics device!!");
            _vertices = new VertexPositionColorTexture[bufferSize];
            _scissorStack = new Stack<Rectangle>();
            _effect = new BasicEffect(_device);
            _effect.LightingEnabled = false;
            _effect.VertexColorEnabled = true;
            _manager.ApplyChanges();
            ResetDrawParams();
            UpdateMatricesForCurrentLayer();
            ResetBuffer();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing || _isDisposed)
                return;
            _effect?.Dispose();
            _isDisposed = true;
        }

        public void UpdateMatricesForCurrentLayer()
        {
            _effect.View = Graphics.currentLayer?.camera?.getMatrix() ?? Matrix.Identity;
            _effect.Projection = Matrix.CreateOrthographicOffCenter(0f, _device.Viewport.Width, _device.Viewport.Height, 0f, 100f, -100f);
        }
        
        public void SetOrthographicProjection() => _effect.Projection = Matrix.CreateOrthographicOffCenter(0f, _device.Viewport.Width, _device.Viewport.Height, 0f, 100f, -100f);

        public void SetPerspectiveProjection() => _effect.Projection = Matrix.CreatePerspectiveOffCenter(0f, _device.Viewport.Width, _device.Viewport.Height, 0f, 100f, -100f);

        public void SetScreenView() => _effect.View = Matrix.Identity;

        public void SetCurrentLayerView() => _effect.View = Graphics.currentLayer?.camera?.getMatrix() ?? Matrix.Identity;

        public void ResetMatrices() => UpdateMatricesForCurrentLayer();

        public Vector3 TransformVector(Vector3 vec) => Vector3.Transform(vec, Matrix.Invert(_effect.View));
        public Vector2 TransformVector(Vector2 vec) => Vector2.Transform(vec, Matrix.Invert(_effect.View));
        
        public void ResetDrawParams()
        {
            ClearScissorStack();
            ScissorMode = ScissorStackMode.Intersect;
            CullMode = CullMode.None;
            FillMode = FillMode.Solid;
            GlobalAlpha = 1.0f;
            AntiAlias = false;
            Texture = null;
            BlendState = BlendState.Opaque;
        }
        
        public Viewport Viewport
        {
            get => _device.Viewport;
        }

        public ScissorStackMode ScissorMode { get; set; }
        public CullMode CullMode { set => _device.RasterizerState.CullMode = value; }
        public FillMode FillMode { set => _device.RasterizerState.FillMode = value; }
        public float DepthBias { set => _device.RasterizerState.DepthBias = value; }
        public float GlobalAlpha { set => _effect.Alpha = value; }
        
        
        /// <summary>
        /// Useful key for blend states:
        /// BlendState.Opaque => Alpha is ignored, colours overwrite.
        /// BlendState.Additive => Alpha is ignored, colours are added.
        /// BlendState.AlphaBlend => Alpha is used, pre-multiplied assumed. (any opacity white = white)
        /// BlendState.NonPreMultiplied => This will do the kind of alpha blending you'd normally expect. (half opacity white = gray)
        ///
        /// Pre-Multiplied alpha is where the colour channels are multiplied by the alpha channel before being drawn,
        /// which allows the rasterizer to skip some of the calculations needed for blending
        /// </summary>
        public BlendState BlendState { set => _device.BlendState = value; }
        public Texture2D Texture
        {
            set
            {
                _effect.Texture = value;
                _effect.TextureEnabled = !(value is null);
            }
        }

        //WARNING: Enabling/Disabling of AA will cause a screen flicker
        public bool AntiAlias
        {
            set
            {
                _device.RasterizerState.MultiSampleAntiAlias = value;
                _manager.PreferMultiSampling = value;
                _manager.ApplyChanges();
            }
        }

        //WARNING: Changing AA multisample count while AA is enabled will cause a screen flicker
        public int MSAASamples
        {
            set
            {
                _device.PresentationParameters.MultiSampleCount = value;
                if(_device.RasterizerState.MultiSampleAntiAlias) _manager.ApplyChanges();
            }
        }

        public void ClearScissorStack()
        {
            _scissorStack.Clear();
            _device.RasterizerState.ScissorTestEnable = false;
        }
        
        public void PushScissor(Rectangle scissorRect)
        {
            Vec2 offset = new Vec2(Viewport.X, Viewport.Y);
            Vec3 tl = Vec3.Transform(new Vec3(scissorRect.tl + offset, 0f), _effect.View);
            Vec3 br = Vec3.Transform(new Vec3(scissorRect.br + offset, 0f), _effect.View);
            Rectangle rect = new Rectangle(tl.XY(), br.XY());

            if (_scissorStack.Count > 0)
            {
                switch (ScissorMode)
                {
                    case ScissorStackMode.Intersect:
                        rect = Microsoft.Xna.Framework.Rectangle.Intersect(rect, _scissorStack.Peek());
                        break;
                    case ScissorStackMode.Union:
                        rect = Microsoft.Xna.Framework.Rectangle.Union(rect, _scissorStack.Peek());
                        break;
                }
            }
            
            _scissorStack.Push(rect);
            _device.ScissorRectangle = rect;
            _device.RasterizerState.ScissorTestEnable = true;
        }

        public void PopScissor()
        {
            if(_scissorStack.Count > 0) _scissorStack.Pop();
            if (_scissorStack.Count <= 0) ClearScissorStack();
            else _device.ScissorRectangle = _scissorStack.Peek();
        }
        
        //Reset buffer is automatically called after each draw, so you should only need it if a draw has been left in progress for some reason.
        public void ResetBuffer()
        {
            _bufferPos = -1;
            _currentOffset = Vector3.Zero;
        }

        public void DrawArrays(Vector3[] positions, Color[] colors, Vector2[] texCoords, PrimitiveType type)
        {
            if (positions.Length != texCoords.Length || positions.Length != colors.Length ||
                texCoords.Length != colors.Length)
                throw new ArgumentException("Primitive arrays are not all the same length!");

            ResetBuffer();
            for(int i = 0; i < positions.Length; i++)
            {
                Vert(positions[i]).Col(colors[i]).Tex(texCoords[i]);
            }
            Draw(type);
        }

        public void DrawArrays(Vector3[] positions, Color[] colors, PrimitiveType type)
        {
            if (positions.Length != colors.Length)
                throw new ArgumentException("Primitive arrays are not all the same length!");

            ResetBuffer();
            for (int i = 0; i < positions.Length; i++)
            {
                Vert(positions[i]).Col(colors[i]);
            }

            Draw(type);
        }

        public void DrawArrays(Vector3[] positions, Vector2[] texCoords, PrimitiveType type)
        {
            if (positions.Length != texCoords.Length)
                throw new ArgumentException("Primitive arrays are not all the same length!");

            ResetBuffer();
            for (int i = 0; i < positions.Length; i++)
            {
                Vert(positions[i]).Tex(texCoords[i]);
            }
            Draw(type);
        }

        public void DrawArrays(Vector3[] positions, Color color, PrimitiveType type)
        {
            ResetBuffer();
            foreach (Vector3 t in positions) Vert(t).Col(color);
            Draw(type);
        }

        public void DrawArrays(Vector3[] positions, PrimitiveType type)
        {
            ResetBuffer();
            foreach (Vector3 t in positions) Vert(t);
            Draw(type);
        }

        public PolygonBatcher Vert(Vector3 pos)
        {
            _bufferPos++;
            if (_bufferPos >= _vertices.Length) throw new InvalidOperationException("Primitive vertex buffer exceeded! Capacity is " + _vertices.Length + " vertices.");
            _vertices[_bufferPos].Position = pos + _currentOffset;
            _vertices[_bufferPos].Color = _bufferPos > 0 ? _vertices[_bufferPos - 1].Color : Color.White;
            return this;
        }

        public PolygonBatcher Vert(Vector2 pos, float z = 0f)
        {
            return Vert(new Vector3(pos.X, pos.Y, z));
        }

        public PolygonBatcher Tex(Vector2 texCoord)
        {
            _vertices[_bufferPos].TextureCoordinate = texCoord;
            return this;
        }

        public PolygonBatcher Col(Color col)
        {
            _vertices[_bufferPos].Color = col;
            return this;
        }

        //Sets an offset to apply to all future vertices (Resets each draw, same as all other draw data)
        public PolygonBatcher Off(Vector3 offset)
        {
            _currentOffset = offset;
            return this;
        }
        
        public void Draw(PrimitiveType type)
        {
            if (_bufferPos == 0 || _isDisposed) return;
            
            _effect.CurrentTechnique.Passes[0].Apply();

            _bufferPos++;
            
            int primitiveCount = 0;

            switch (type)
            {
                case PrimitiveType.TriangleList:
                    primitiveCount = _bufferPos / 3;
                    break;
                case PrimitiveType.TriangleStrip:
                    primitiveCount = _bufferPos - 2;
                    break;
                case PrimitiveType.LineList:
                    primitiveCount = _bufferPos / 2;
                    break;
                case PrimitiveType.LineStrip:
                    primitiveCount = _bufferPos - 1;
                    break;
            }

            _device.DrawUserPrimitives(type, _vertices, 0, primitiveCount);

            ResetBuffer();
        }
        
        public void DrawTriStrip()
        {
            Draw(PrimitiveType.TriangleStrip);
        }

        public void DrawTriList()
        {
            Draw(PrimitiveType.TriangleList);
        }

        public void DrawLineStrip()
        {
            Draw(PrimitiveType.LineStrip);
        }

        public void DrawLineList()
        {
            Draw(PrimitiveType.LineList);
        }
    }

    public enum ScissorStackMode
    {
        Replace, Intersect, Union,
    }
}
