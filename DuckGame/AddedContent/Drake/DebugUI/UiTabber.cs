using System;
using System.Collections.Generic;
using DuckGame.AddedContent.Drake.PolyRender;
using DuckGame.AddedContent.Drake.Utils;
using Microsoft.Xna.Framework;

namespace DuckGame.AddedContent.Drake.DebugUI
{
    public class UiTabber : UiList
{
    protected IAmUi _currentTab;

    public override Vector4 Expansion => new Vector4(0f, InteractBarSize.Y + _accentLineWidth, 0f, 0f);
    
    public UiTabber(Vector2 position, Vector2 size, Color color, List<IAmUi> content, string name = "UiList") : base(position, size, color, content, name)
    {
        _currentTab = content.Count > 0 ? content[0] : null;
    }
    
    protected override void ArrangeContent()
    {
        if (SubContent.Count <= 0) _currentTab = null;
        if (_currentTab == null && SubContent.Count > 0) _currentTab = SubContent[0];
        if (_currentTab == null) return;
        
        var subContentPosition = Position + InteractBarSize.ZeroX() + Padding + new Vector2(0f, -_scrollOffset);
        foreach (var ui in SubContent)
        {
            ui.Position = subContentPosition + ui.Expansion.XY();
            ui.Size = ui.Size.ReplaceX(Size.X - Padding.X * 2 - ui.Expansion.Y - ui.Expansion.Z);
        }
        _maxScrollOffset = Math.Max(_currentTab.Size.Y + _currentTab.Expansion.Y + _currentTab.Expansion.W + Padding.Y * 2f - Size.Y, _scrollOffset);
        ContentChanged = false;
    }
    
    public override void DrawContent()
    {
        base.DrawContent();
        DrawTabs();
    }

    protected override void DrawSubContent()
    {
        Graphics.polyBatcher.PushScissor(CalcScissor());
        _currentTab?.DrawContent();
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
        Vector2 off = new Vector2(InteractBarSize.Y / 5f, 0f);
        Vector2 size = new Vector2(width, InteractBarSize.Y);
        Color col = ui.GetCol(UiCols.Main);
        if (ui == _currentTab) col = col.Brighter();
        PolyRenderer.Quad(pos - size.ZeroX() + off, pos + size.NegateY() - off, pos, pos + size.ZeroY(), col);
    
        StaticFont.DrawStringSized("$B"+ui.Name, pos - size.ZeroX() + off, Color.Black, 4);
    }
    
    protected override void HandleClicked(MouseAction action)
    {
        base.HandleClicked(action);
        if(SubContent.Count == 0) return;
        if (!InputData.MouseProjectedPosition.IsInsideRect(Position - new Vector2(0f, InteractBarSize.Y), new Vector2(Size.X, InteractBarSize.Y))) return;
        float offset = InputData.MouseProjectedPosition.X - Position.X;
        int index = (int)((offset / Size.X) * SubContent.Count);
        _currentTab = SubContent[index];
        ArrangeContent();
    }
    
    protected override void SendSubContentMouseAction(MouseAction action, float scroll = 0)
    {
        if ((action & MouseAction.AnyClick) != 0 || action == MouseAction.Scrolled && _currentTab.IsOverlapping(InputData.MouseProjectedPosition)) _currentTab?.OnMouseAction(action, scroll);
        else _currentTab?.OnMouseAction(action, scroll);
    }
    
    protected override void SendSubContentKeyPressed(Keys keycode, char value)
    {
        _currentTab?.OnKeyPressed(keycode, value);
    }
    
    protected override void OnSubContentKilled(IAmUi subContent)
    {
        base.OnSubContentKilled(subContent);
        _currentTab = SubContent.Count > 0 ? SubContent[0] : null;
    }
}
}
