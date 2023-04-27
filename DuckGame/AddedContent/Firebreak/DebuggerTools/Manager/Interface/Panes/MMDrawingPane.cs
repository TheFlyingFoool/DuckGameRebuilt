using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace DuckGame.ConsoleInterface.Panes
{
    public class MMDrawingPane : MallardManagerPane
    {
        public override bool Borderless { get; } = false;
        
        private Dictionary<Point, Color> _pixelBuffer = new();
        public Size CanvasSize;
        public Tex2D OutputTexture = null;
        
        private Vec2 _cameraPosition = Vec2.Zero;
        private bool _movingCamera = false;
        private Vec2 _oldCameraPosition = Vec2.Zero;
        private Vec2 _cameraMovePivot = Vec2.Zero;
        
        private Vec2 _cursorScreenPosition => Mouse.positionConsole;
        private Vec2 _cursorWorldPosition => _cursorScreenPosition + _cameraPosition;
        
        private Rectangle _canvasBounds => new(
            Bounds.tl.x + _cameraPosition.x,
            Bounds.tl.y + _cameraPosition.y,
            CanvasSize.Width * MallardManager.Config.Zoom * 4,
            CanvasSize.Height * MallardManager.Config.Zoom * 4);

        public MMDrawingPane(Size canvasSize)
        {
            CanvasSize = canvasSize;
        }

        public MMDrawingPane(Tex2D texture)
        {
            CanvasSize = new Size(texture.w, texture.h);
            
            texture.Transform((pos, color) =>
            {
                if (color != Color.Transparent)
                    _pixelBuffer[pos] = color;
                
                return color;
            });
            
            ApplyTexture();
        }

        public void ApplyTexture()
        {
            OutputTexture = new Tex2D(CanvasSize.Width, CanvasSize.Height);
            OutputTexture.Transform((pos, _) =>
            {
                if (_pixelBuffer.TryGetValue(pos, out Color value))
                    return value;
                
                return Color.Transparent;
            });
        }

        public override void Update()
        {
            if (Mouse.middle == InputState.Pressed)
            {
                _movingCamera = true;
                _cameraMovePivot = _cursorWorldPosition;
                _oldCameraPosition = _cameraPosition;
            }
            else if (Mouse.middle == InputState.Released)
                _movingCamera = false;

            if (_movingCamera)
            {
                _cameraPosition = _oldCameraPosition + (_cursorScreenPosition + _oldCameraPosition - _cameraMovePivot);
                return;
            }

            float deltaUnit = MallardManager.Config.Zoom * 4;

            for (int y = 0; y < CanvasSize.Height; y++)
            {
                for (int x = 0; x < CanvasSize.Width; x++)
                {
                    Rectangle pixelBounds = new(
                        Bounds.tl.x + x * deltaUnit + _cameraPosition.x,
                        Bounds.tl.y + y * deltaUnit + _cameraPosition.y,
                        deltaUnit,
                        deltaUnit);
                    
                    if (Mouse.left == InputState.Down
                        && pixelBounds.Contains(_cursorScreenPosition))
                    {
                        _pixelBuffer[new Point(x, y)] = Color.White;
                    }
                }
            }
        }

        public override void DrawRaw(Depth depth, float deltaUnit)
        {
            deltaUnit *= 4;
            
            Graphics.DrawCircle(_cursorScreenPosition, 1, Color.Red, 1f, 2f);
            
            Graphics.DrawRect(Bounds, MallardManager.Colors.PrimaryBackground, depth);

            Graphics.DrawExternallyOutlinedRect(_canvasBounds, Color.Transparent, MallardManager.Colors.PrimarySub, depth + 0.5f, deltaUnit / 2);
            Tex2D backgroundCanvasTexture = new(CanvasSize.Width, CanvasSize.Height);
            backgroundCanvasTexture.Transform((pos, _) => pos.X % 2 == pos.Y % 2
                ? Color.DimGray
                : Color.LightGray);
            Graphics.Draw(backgroundCanvasTexture, _canvasBounds.tl, null, Color.White, 0, Vec2.Zero, new Vec2(deltaUnit), SpriteEffects.None, depth + 0.5f);

            foreach (KeyValuePair<Point,Color> pixel in _pixelBuffer)
            {
                Rectangle pixelBounds = new(
                    Bounds.tl.x + pixel.Key.X * deltaUnit + _cameraPosition.x,
                    Bounds.tl.y + pixel.Key.Y * deltaUnit + _cameraPosition.y,
                    deltaUnit,
                    deltaUnit);
                
                Graphics.DrawRect(pixelBounds, pixel.Value, depth + 1f);
            }
        }

        public override void OnFocus()
        {
        }
    }
}