using System;
using System.Collections.Generic;
using AddedContent.Hyeve.DebugUI.Basic;
using AddedContent.Hyeve.DebugUI.Groups;
using AddedContent.Hyeve.PolyRender;
using AddedContent.Hyeve.Utils;
using DuckGame;
using Microsoft.Xna.Framework;
using Color = DuckGame.Color;
using Rectangle = DuckGame.Rectangle;

namespace AddedContent.Hyeve.DebugUI
{
    public class UiTabber : UiList
    {
        protected IAmUi CurrentTab;

        protected int CharSize;

        public override Vector4 Expansion => new Vector4(0f, InteractBarSize.Y + _accentLineWidth, 0f, 0f);

        public UiTabber(Vector2 position, Vector2 size, List<IAmUi> content, string name = "UiList", float scale = 1f) : base(position, size, content, name, scale)
        {
            CurrentTab = content.Count > 0 ? content[0] : null;
            CharSize = (int)(18 * scale);
        }

        protected override void ArrangeContent()
        {
            if (SubContent.Count <= 0) CurrentTab = null;
            if (CurrentTab == null && SubContent.Count > 0) CurrentTab = SubContent[0];
            if (CurrentTab == null) return;

            Vector2 subContentPosition = Position + InteractBarSize.ZeroX() + Padding + new Vector2(0f, -_scrollOffset);
            foreach (IAmUi ui in SubContent)
            {
                ui.Position = subContentPosition + ui.Expansion.XY();
                ui.Size = ui.Size.ReplaceX(Size.X - Padding.X * 2 - ui.Expansion.Y - ui.Expansion.Z).ReplaceY(Size.Y - Padding.Y * 2 - ui.Expansion.X - ui.Expansion.W - InteractBarSize.Y);
            }
            _maxScrollOffset = Math.Max(CurrentTab.Size.Y + CurrentTab.Expansion.Y + CurrentTab.Expansion.W + Padding.Y * 2f - Size.Y, _scrollOffset);
            ContentChanged = false;
        }

        public override void DrawContent()
        {
            base.DrawContent();
            DrawTabs();
        }

        protected override void DrawSubContent()
        {
            if (!Visible()) return;
            Graphics.polyBatcher.PushScissor(CalcScissor());
            CurrentTab?.DrawContent();
            Graphics.polyBatcher.PopScissor();
        }

        protected virtual void DrawTabs()
        {
            float width = Size.X / SubContent.Count;
            Vector2 pos = Position - new Vector2(0f, _accentLineWidth);
            foreach (IAmUi content in SubContent)
            {
                DrawTab(pos, width, content);
                pos.X += width;
            }
        }

        protected virtual void DrawTab(Vector2 pos, float width, IAmUi ui)
        {
            Vector2 off = new(InteractBarSize.Y / 5f, 0f);
            Vector2 size = new(width, InteractBarSize.Y);
            Color col = Colors.Main;
            if (HoveredTab() == ui) col = col.Brighter(.4f);
            if (ui == CurrentTab) col = Colors.Accent;
            PolyRenderer.Quad(pos - size.ZeroX() + off, pos + size.NegateY() - off, pos, pos + size.ZeroY(), col);

            Graphics.polyBatcher.PushScissor(new Rectangle(pos - size.ZeroX() + off, pos + size.ZeroY() - off));
            FontDatabase.DrawString("$B" + ui.Name, pos + off, Colors.Text, CharSize);
            Graphics.polyBatcher.PopScissor();
        }

        protected override void HandleClicked(MouseAction action)
        {
            base.HandleClicked(action);
            if (SubContent.Count == 0) return;
            IAmUi ui = HoveredTab();
            if (ui != null) CurrentTab = ui;
            ArrangeContent();
        }

        protected virtual IAmUi HoveredTab()
        {
            if (!InputData.MouseProjectedPosition.IsInsideRect(Position - new Vector2(0f, InteractBarSize.Y), new Vector2(Size.X, InteractBarSize.Y))) return null;
            float offset = InputData.MouseProjectedPosition.X - Position.X;
            int index = (int)((offset / Size.X) * SubContent.Count);
            return SubContent[index];
        }

        protected override void SendSubContentMouseAction(MouseAction action, float scroll = 0)
        {
            if ((action & MouseAction.AnyClick) != 0 || action == MouseAction.Scrolled && CurrentTab.Hovered()) CurrentTab?.OnMouseAction(action, scroll);
            else CurrentTab?.OnMouseAction(action, scroll);
        }

        protected override void SendSubContentKeyPressed(Keys keycode, char value)
        {
            CurrentTab?.OnKeyPressed(keycode, value);
        }

        protected override void OnSubContentDestroyed(IAmUi subContent)
        {
            base.OnSubContentDestroyed(subContent);
            CurrentTab = SubContent.Count > 0 ? SubContent[0] : null;
        }
    }
}
