using System;
using AddedContent.Hyeve.DebugUI.Components;
using AddedContent.Hyeve.PolyRender;
using AddedContent.Hyeve.Utils;
using DuckGame;
using Microsoft.Xna.Framework;
using Color = DuckGame.Color;
using Rectangle = DuckGame.Rectangle;

namespace AddedContent.Hyeve.DebugUI.Basic
{
    public class UiBasic : IAmUi
    {
        public event Action<IAmUi, Vector2> OnPositioned;

        public event Action<IAmUi, Vector2> OnResized;

        public event Action<IAmUi> OnDestroyed;

        public virtual Vector2 Position
        {
            get => _position;
            set
            {
                Vector2 old = _position;
                _position = value;
                HandlePositioned(old);
            }
        }

        protected virtual Vector2 PositionInternal
        {
            set
            {
                Vector2 old = _position;
                _position = value;
                OnPositioned?.Invoke(this, old);
                HandlePositioned(old);
            }
        }

        public virtual Vector2 Size
        {
            get => _size;
            set
            {
                Vector2 old = _size;
                _size = Vector2.Clamp(value, MinSize, MaxSize);
                HandleResized(old);
            }
        }

        public virtual Vector4 Expansion => Vector4.Zero;
        public string Name { get; set; }
        protected virtual Vector2 SizeInternal
        {
            set
            {
                Vector2 old = _size;
                _size = Vector2.Clamp(value, MinSize, MaxSize);
                OnResized?.Invoke(this, old);
                HandleResized(old);
            }
        }
        
        public virtual UiColourHolder Colors { get; set; }
        public virtual Vector2 MinSize { get; set; }
        public virtual Vector2 MaxSize { get; set; }
        public virtual Vector2 InteractBarSize { get; set; }
        public virtual bool Draggable { get; set; }
        public virtual bool Resizeable { get; set; }
        public virtual bool Closeable { get; set; }


        protected Vector2 _position;
        protected Vector2 _size;
        protected AtlasedFont _font;


        private bool _dragging;
        private bool _resizing;
        private Vector2 _originalSize;
        private Vector2 _originalPosition;
        private Vector2 _mouseOffset;
        protected float _accentLineWidth;

        public UiBasic(Vector2 position, Vector2 size, string name = "UiBasic", float scale = 1f)
        {
            size *= scale;
            PositionInternal = position;
            InteractBarSize = new Vector2(size.X, 15f * scale);
            MinSize = InteractBarSize.YY() * 5f;
            MaxSize = new Vector2(float.PositiveInfinity);
            SizeInternal = size;
            _accentLineWidth = 2f * scale;
            Draggable = true;
            Resizeable = true;
            Closeable = true;
            Name = name;
            Colors = new UiColourHolder();
        }

        public virtual void DrawContent()
        {
            if (!Visible()) return;
            
            if (Draggable || Closeable || Resizeable)
            {
                Vector2 offset = new Vector2(_accentLineWidth);
                PolyRenderer.Rect(Position - offset, Position + Size + offset, Colors.Accent);
                PolyRenderer.Rect(Position, Position + Size, Colors.Main);
                DrawInteractBar();
            }
            else
            {
                PolyRenderer.Rect(Position, Position + Size, Colors.Main);
            }
        }

        protected virtual void DrawInteractBar()
        {
            Graphics.polyBatcher.PushScissor(new Rectangle(Position, Position + Size));
            Vector2 crossSize = new Vector2(InteractBarSize.Y);
            Vector2 upRightPosition = Position + Size.ZeroY();
            Vector2 crossZeroY = crossSize.ZeroY();
            Vector2 crossZeroX = crossSize.ZeroX();
            Vector2 crossNegateX = crossSize.NegateX();

            if (Draggable)
            {
                Color col = Colors.Accent;
                if (_dragging || InputData.MouseProjectedPosition.IsInsideRect(Position + InteractBarSize.MultiplyX(0.3f).ZeroY(), InteractBarSize.MultiplyX(0.4f))) col = col.Brighter(0.4f);
                PolyRenderer.Line(Position + InteractBarSize.MultiplyX(0.3f), Position + InteractBarSize.MultiplyX(0.7f), _accentLineWidth, col);
            }

            if (Closeable)
            {
                Color col = Colors.Accent;
                if (CloseButtonHovered()) col = col.Brighter(0.4f);
                PolyRenderer.Line(Position + crossZeroX, Position + crossZeroY, _accentLineWidth, col);
                PolyRenderer.Line(Position + new Vector2(0, crossSize.Y * 0.5f),
                    Position + new Vector2(crossSize.X * 0.5f, 0), _accentLineWidth, col);
            }

            if (Resizeable)
            {
                Color col = Colors.Accent;
                if ( _resizing || ResizeButtonHovered()) col = col.Brighter(0.4f);
                PolyRenderer.Line(upRightPosition + crossNegateX + new Vector2(0f, _accentLineWidth * 0.5f), upRightPosition - crossZeroY, _accentLineWidth,
                    col);
                PolyRenderer.Line(upRightPosition + crossNegateX, upRightPosition + crossZeroX, _accentLineWidth,
                    col);
                PolyRenderer.Line(upRightPosition + crossNegateX * 0.5f + new Vector2(0f, _accentLineWidth * 0.5f), upRightPosition - crossZeroY * 0.5f,
                    _accentLineWidth, col);
                PolyRenderer.Line(upRightPosition + crossNegateX * 0.5f, upRightPosition + crossZeroX * 0.5f,
                    _accentLineWidth, col);
            }
            Graphics.polyBatcher.PopScissor();
        }

        public virtual void UpdateContent()
        {
            if (_dragging) DoDragging();
            if (_resizing) DoResizing();
        }


        public virtual void OnMouseAction(MouseAction action, float scroll = 0f)
        {
            switch (action)
            {
                case MouseAction.LeftClick:
                case MouseAction.RightClick:
                case MouseAction.MiddleClick:
                    HandleClicked(action);
                    break;
                case MouseAction.LeftRelease:
                case MouseAction.RightRelease:
                case MouseAction.MiddleRelease:
                    HandleUnClicked(action);
                    break;
                case MouseAction.Scrolled:
                    HandleScrolled(scroll);
                    break;
            }
        }
        public virtual void OnKeyPressed(Keys keycode, char value)
        {

        }


        public virtual bool IsOverlapping(Vector2 pos) => pos.IsInsideRect(Position, Size);

        public bool Visible()
        {
            Rectangle rect = Graphics.polyBatcher.GetCurrentScissor();
            Vector2 sbr = Position + Size;
            return rect.bl.x < sbr.X && rect.bl.y > Position.Y && rect.tr.x > Position.X && rect.tr.y < sbr.Y;
        }

        public virtual void Kill() => OnDestroyed?.Invoke(this);

        protected virtual void HandleClicked(MouseAction action)
        {
            if (Closeable && CloseButtonHovered())
            {
                Kill();
                return;
            }

            if (Resizeable && ResizeButtonHovered())
            {
                _resizing = true;
                _originalPosition = Position;
                _originalSize = Size;
                _mouseOffset = InputData.MouseProjectedPosition - Position;
                return;
            }

            if (Draggable && DragBarHovered())
            {
                _dragging = true;
                _originalPosition = Position;
                _mouseOffset = InputData.MouseProjectedPosition - Position;
                return;
            }
        }

        protected virtual bool CloseButtonHovered() => InputData.MouseProjectedPosition.IsInsideRect(Position, InteractBarSize.YY());

        protected virtual bool ResizeButtonHovered() => InputData.MouseProjectedPosition.IsInsideRect(Position + Size.ZeroY().SubtractX(InteractBarSize.Y), InteractBarSize.YY());

        protected virtual bool DragBarHovered() => InputData.MouseProjectedPosition.IsInsideRect(Position + InteractBarSize.YY().ZeroY(), Size.ReplaceY(InteractBarSize.Y).SubtractX(InteractBarSize.Y*2));
        
        protected virtual void HandleUnClicked(MouseAction action)
        {
            _dragging = false;
            _resizing = false;
        }

        protected virtual void DoDragging()
        {
            PositionInternal = Vector2.Clamp(InputData.MouseProjectedPosition, _mouseOffset, InputData.ViewportSize - (Size - _mouseOffset)) - _mouseOffset;
        }
        protected virtual void DoResizing()
        {
            Vector2 mouseDiff = Vector2.Clamp(
                                    InputData.MouseProjectedPosition,
                                    _mouseOffset.ZeroX(),
                                    InputData.ViewportSize - _originalSize.SubtractX(_mouseOffset.X).ReplaceY(_mouseOffset.Y)
                                    ) - _mouseOffset;

            PositionInternal = new Vector2(_originalPosition.X, Math.Min(_originalPosition.Y + _originalSize.Y - MinSize.Y, mouseDiff.Y));
            SizeInternal = new Vector2(mouseDiff.X - Position.X + _originalSize.X, _originalPosition.Y + _originalSize.Y - mouseDiff.Y);
        }



        protected virtual void HandlePositioned(Vector2 old)
        {

        }
        protected virtual void HandleResized(Vector2 old) => InteractBarSize = InteractBarSize.ReplaceX(Size.X);

        protected virtual void HandleScrolled(float scroll) { }

        public virtual bool Hovered() => IsOverlapping(InputData.MouseProjectedPosition);
    }
}
