using System;
using AddedContent.Hyeve.DebugUI.Basic;
using AddedContent.Hyeve.PolyRender;
using AddedContent.Hyeve.Utils;
using DuckGame;
using Microsoft.Xna.Framework;
using Color = DuckGame.Color;
using Rectangle = DuckGame.Rectangle;

namespace AddedContent.Hyeve.DebugUI.Values
{
    public class UiVector2Bar : UiSeamless
    {
        protected int CharSize;

        protected bool DraggingValue;

        protected float DragMultiplier;

        private Vector2 _mouseStart;
        private Vector2 _oldValue;
        private Vector2 _oldValueOffset;
        private Vector2 _valueOffset;

        public virtual Vector2 Value { get; set; }
        
        public UiVector2Bar(Vector2 position, Vector2 size, string name = "UiVector2", float scale = 1) : base(position, size, name, scale)
        {
            CharSize = (int)(20 * scale);
        }

        public override void DrawContent()
        {
            if (!Visible()) return;
            base.DrawContent();
            Color main = Colors.Main;

            if (Hovered() || DraggingValue) main = main.Brighter(0.5f);
            
            Vector2 offset = new(_accentLineWidth);
            PolyRenderer.Rect(Position - offset, Position + Size + offset, Colors.Accent);
            PolyRenderer.Rect(Position, Position + Size, main);
            if (DraggingValue)
            {
                Vector2 posX = InputData.MouseProjectedPosition.ReplaceY(Position.Y);
                Vector2 posY = InputData.MouseProjectedPosition.ReplaceX(Position.X);
                Graphics.polyBatcher.PushScissor(new Rectangle(Position, Position + Size));
                PolyRenderer.Line(posX, posX + Size.ZeroX(), _accentLineWidth, Color.Red);
                PolyRenderer.Line(posY, posY + Size.ZeroY(), _accentLineWidth, Color.Lime);
                Graphics.polyBatcher.PopScissor();
            }
            FontDatabase.DrawString(Name + $" : $C{Color.Red.ToHexString()}X:{Value.X:F2} $C{Color.Lime.ToHexString()}Y:{Value.Y:F2}",Position + Size.ZeroX() / 2f + Vector2.UnitY * CharSize / 2f, Colors.Text, CharSize);
        }

        public override void UpdateContent()
        {
            base.UpdateContent();
            if (DraggingValue) DoValueDragging();
        }

        protected virtual void DoValueDragging()
        {
            Vector2 pos = InputData.MouseProjectedPosition;
            _valueOffset = _oldValueOffset + (pos - _mouseStart) * DragMultiplier;
            Value = _oldValue + _valueOffset - _oldValueOffset;
        }

        protected override void HandleClicked(MouseAction action)
        {
            if (!Hovered()) return;

            if (InputData.KeyDown(Keys.LeftAlt))
            {
                Value -= _valueOffset;
                _valueOffset = Vector2.Zero;
                return;
            }
            
            DragMultiplier = action switch
            {
                MouseAction.LeftClick => 0.05f,
                MouseAction.RightClick => 0.5f,
                _ => DragMultiplier
            };
            
            _mouseStart = InputData.MouseProjectedPosition;
            if ((Value - (_oldValue + _valueOffset - _oldValueOffset)).Length() > 0.01f) _valueOffset = Vector2.Zero;
            _oldValueOffset = _valueOffset;
            _oldValue = Value;
            DraggingValue = true;
        }

        protected override void HandleUnClicked(MouseAction action)
        {
            DraggingValue = false;
        }
    }
}