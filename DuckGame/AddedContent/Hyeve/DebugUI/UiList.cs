using System;
using System.Collections.Generic;
using System.Linq;
using AddedContent.Hyeve.Utils;
using DuckGame;
using Microsoft.Xna.Framework;
using Color = DuckGame.Color;
using Rectangle = DuckGame.Rectangle;

namespace AddedContent.Hyeve.DebugUI
{
    public class UiList : UiGroup
    {
        public Vector2 Padding
        {
            get => _padding;
            set
            {
                _padding = value;
                ArrangeContent();
            }
        }

        protected override bool DrawSelf { get => true; }
        protected Vector2 _padding;

        protected float _maxScrollOffset;
        protected float _scrollOffset = 0f;

        public UiList(Vector2 position, Vector2 size, Color color, List<IAmUi> content, string name = "UiList", float scale = 1f) : base(position, size, color, content, name, scale)
        {
            Padding = new Vector2(2f) * scale;
            Resizeable = false;
            ArrangeContent();
            OnKilled += HandleDeath;
        }

        protected override void UpdateSubContent()
        {
            base.UpdateSubContent();
            if (ContentChanged) ArrangeContent();
        }

        protected virtual void ArrangeContent()
        {
            Vector2 subContentPosition = Position + InteractBarSize.ZeroX() + Padding + new Vector2(0f, -_scrollOffset);
            foreach (IAmUi ui in SubContent)
            {
                ui.Position = subContentPosition + ui.Expansion.XY();
                ui.Size = ui.Size.ReplaceX(Size.X - Padding.X * 2 - ui.Expansion.Y - ui.Expansion.Z);
                subContentPosition += ui.Size.ZeroX() + Padding.ZeroX() + ui.Expansion.ZW().ZeroX();
            }
            _maxScrollOffset = Math.Max(SubContent.Sum(content => content.Size.Y + content.Expansion.Y + content.Expansion.W + Padding.Y) + Padding.Y * 2f - Size.Y, _scrollOffset);
            ContentChanged = false;
        }

        protected override Rectangle CalcScissor()
        {
            return new Rectangle(Position + InteractBarSize.ZeroX() + new Vector2(0f, _accentLineWidth), Position + Size);
        }

        protected override void HandleColoured(UiCols type, Color old)
        {
            base.HandleColoured(type, old);
            foreach (IAmUi ui in SubContent) ui.SetCol(type, GetCol(type));
        }

        protected override void HandleResized(Vector2 old)
        {
            base.HandleResized(old);
            ArrangeContent();
        }
        protected override void HandlePositioned(Vector2 old)
        {
            base.HandlePositioned(old);
            Vector2 change = Position - old;
            foreach (IAmUi ui in SubContent) ui.Position += change;
        }

        protected override void OnSubContentResized(IAmUi subContent, Vector2 old)
        {
            Size.ReplaceX(SubContent.Max(ui => ui.Size.X) + Padding.X * 2f);
            ArrangeContent();
        }

        protected override void OnSubContentPositioned(IAmUi subContent, Vector2 old)
        {
            //TODO: UI Docking?? :D
            subContent.Position = old; //NO MOVING!!
        }

        protected override void HandleScrolled(float scroll)
        {
            if (!IsOverlapping(InputData.MouseProjectedPosition)) return;
            base.HandleScrolled(scroll);
            _scrollOffset = Maths.Clamp(_scrollOffset + scroll, 0, _maxScrollOffset);
            ArrangeContent();
        }

        protected virtual void HandleDeath(IAmUi self)
        {
            foreach (IAmUi ui in SubContent) ui.Kill();
        }
    }
}
