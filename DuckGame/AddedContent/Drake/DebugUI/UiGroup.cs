using System;
using System.Collections.Generic;
using DuckGame.AddedContent.Drake.Utils;
using Microsoft.Xna.Framework;

namespace DuckGame.AddedContent.Drake.DebugUI;

public class UiGroup : UiBasic
{
    public readonly List<IAmUi> SubContent = new();
    protected readonly List<IAmUi> _contentToRemove = new();
    protected readonly List<IAmUi> _contentToAdd = new();

    protected bool ContentChanged = false;

    protected virtual bool DrawSelf { get => false; }
    
    public UiGroup(Vector2 position, Vector2 size, Color color, List<IAmUi> content, string name = "UiGroup") : base(position, size, color, name)
    {
        foreach (IAmUi ui in content) AddContent(ui);
    }

    public override void DrawContent()
    {
        if (DrawSelf) base.DrawContent();
        DrawSubContent();
    }

    public override void UpdateContent()
    {
        base.UpdateContent();
        if (_contentToAdd.Count > 0)
        {
            SubContent.AddRange(_contentToAdd);
            _contentToAdd.Clear();
            ContentChanged = true;
        }

        if (_contentToRemove.Count > 0)
        {
            _contentToRemove.ForEach(content => SubContent.Remove(content));
            _contentToRemove.Clear();
            ContentChanged = true;
        }
        UpdateSubContent();
    }


    protected virtual void DrawSubContent()
    {
        Graphics.polyBatcher.PushScissor(CalcScissor());
        for (int i = SubContent.Count - 1; i >= 0; i--)
            SubContent[i].DrawContent(); //Draw in reverse order so that click events always hit the top UI
        Graphics.polyBatcher.PopScissor();
    }

    protected virtual void UpdateSubContent()
    {
        foreach (var ui in SubContent) ui.UpdateContent();
    }

    public virtual void AddContent(IAmUi content)
    {
        _contentToAdd.Add(content);
        content.OnKilled += OnSubContentKilled;
        content.OnResized += OnSubContentResized;
        content.OnPositioned += OnSubContentPositioned;
        content.OnColoured += OnSubContentColoured;
    }

    public virtual void RemoveContent(IAmUi content) => _contentToRemove.Add(content);

    public override void OnMouseAction(MouseAction action, float scroll = 0)
    {
        base.OnMouseAction(action, scroll);
        SendSubContentMouseAction(action, scroll);
    }

    protected virtual void SendSubContentMouseAction(MouseAction action, float scroll = 0)
    {
        if ((action & MouseAction.AnyClick) != 0 || action == MouseAction.Scrolled)
            SubContent.Find(content => content.IsOverlapping(InputData.MouseProjectedPosition))?.OnMouseAction(action, scroll);
        else
            foreach (var ui in SubContent)
                ui.OnMouseAction(action, scroll);
    }
    
    public override void OnKeyPressed(Keys keycode, char value)
    {
        base.OnKeyPressed(keycode, value);
        SendSubContentKeyPressed(keycode, value);
    }

    protected virtual void SendSubContentKeyPressed(Keys keycode, char value)
    {
        foreach (var ui in SubContent) ui.OnKeyPressed(keycode, value);
    }

    protected virtual Rectangle CalcScissor()
    {
        return new Rectangle(Position, Position + Size);
    }
    
    protected virtual void OnSubContentResized(IAmUi subContent, Vector2 old) { }

    protected virtual void OnSubContentPositioned(IAmUi subContent, Vector2 old)
    { }

    protected virtual void OnSubContentColoured(IAmUi subContent, UiCols type, Color old)
    { }

    protected virtual void OnSubContentKilled(IAmUi subContent)
    {
        RemoveContent(subContent);
    }
}