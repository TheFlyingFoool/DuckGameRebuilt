using System;
using AddedContent.Hyeve.DebugUI.Basic;
using AddedContent.Hyeve.PolyRender;
using AddedContent.Hyeve.Utils;
using DuckGame;
using Microsoft.Xna.Framework;
using Color = DuckGame.Color;

namespace AddedContent.Hyeve.DebugUI.Values
{
    public class UiNumberBar : UiSeamless
    {
        protected int CharSize;

        protected bool DraggingValue;

        protected float DragMultiplier;

        protected string Formatter = "F2";
        
        private Vector2 _mouseStart;
        private double _oldValue;
        private double _oldValueOffset;
        private double _valueOffset;

        public virtual double Value { get; set; }
        
        public UiNumberBar(Vector2 position, Vector2 size, string name = "UiSeamless", float scale = 1) : base(position, size, name, scale)
        {
            CharSize = (int)(20 * scale);
        }

        public override void DrawContent()
        {
            if (!Visible()) return;
            
            base.DrawContent();
            Color main = Colors.Main;

            if (Hovered() || DraggingValue) main = main.Brighter(0.5f);
            
            Vector2 offset = new Vector2(_accentLineWidth);
            PolyRenderer.Rect(Position - offset, Position + Size + offset, Colors.Accent);
            PolyRenderer.Rect(Position, Position + Size, main);
            if (DraggingValue)
            {
                Vector2 pos = InputData.MouseProjectedPosition.ReplaceY(Position.Y);
                PolyRenderer.Line(pos, pos + Size.ZeroX(), _accentLineWidth, Colors.Data);
            }
            FontDatabase.DrawString(Name + $" : $C{Colors.Data.ToHexString()}{Value.ToString(Formatter)}",Position + Size.ZeroX() / 2f + Vector2.UnitY * CharSize / 2f, Colors.Text, CharSize);
        }

        public override void UpdateContent()
        {
            base.UpdateContent();
            if (DraggingValue) DoValueDragging();
        }

        protected virtual void DoValueDragging()
        {
            Vector2 pos = InputData.MouseProjectedPosition;
            _valueOffset = _oldValueOffset + (pos.X + pos.Y - _mouseStart.X - _mouseStart.Y) * DragMultiplier;
            Value = _oldValue + _valueOffset - _oldValueOffset;
        }

        protected override void HandleClicked(MouseAction action)
        {
            if (!Hovered()) return;

            if (InputData.KeyDown(Keys.LeftAlt))
            {
                Value -= _valueOffset;
                _valueOffset = 0;
                return;
            }
            
            DragMultiplier = action switch
            {
                MouseAction.LeftClick => 0.05f,
                MouseAction.RightClick => 0.5f,
                _ => DragMultiplier
            };
            
            _mouseStart = InputData.MouseProjectedPosition;
            if (Math.Abs(Value - (_oldValue + _valueOffset - _oldValueOffset)) > 0.01f) _valueOffset = 0;
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