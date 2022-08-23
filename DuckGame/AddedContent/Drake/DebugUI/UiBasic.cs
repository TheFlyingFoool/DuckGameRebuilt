using System;
using System.Collections.Generic;
using DuckGame.AddedContent.Drake.PolyRender;
using DuckGame.AddedContent.Drake.Utils;
using Microsoft.Xna.Framework;

namespace DuckGame.AddedContent.Drake.DebugUI;

public class UiBasic : IAmUi
{
    public event Action<IAmUi, Vector2> OnPositioned;

    public event Action<IAmUi, UiCols, Color> OnColoured;

    public event Action<IAmUi, Vector2> OnResized;

    public event Action<IAmUi> OnKilled;

    public virtual Vector2 Position
    {
        get => _position;
        set
        {
            var old = _position;
            _position = value;
            HandlePositioned(old);
        }
    }

    protected virtual Vector2 PositionInternal
    {
        set
        {
            var old = _position;
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
            var old = _size;
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
            var old = _size;
            _size = Vector2.Clamp(value, MinSize, MaxSize);
            OnResized?.Invoke(this, old);
            HandleResized(old);
        }
    }
    protected Color MainColor
    {
        get => GetCol(UiCols.Main);
        set => SetColInternal(UiCols.Main, value);
    }

    protected Color AccentColor
    {
        get => GetCol(UiCols.Accent);
        set => SetColInternal(UiCols.Accent, value);
    }

    protected Color AlternateColor
    {
        get => GetCol(UiCols.Alternate);
        set => SetColInternal(UiCols.Alternate, value);
    }

    protected Color TextColor
    {
        get => GetCol(UiCols.Text);
        set => SetColInternal(UiCols.Text, value);
    }

    protected Color DataColor
    {
        get => GetCol(UiCols.Data);
        set => SetColInternal(UiCols.Data, value);
    }
    
    public virtual Vector2 MinSize { get; set; }
    public virtual Vector2 MaxSize { get; set; }
    public virtual Vector2 InteractBarSize { get; set; }
    public virtual bool Draggable { get; set; }
    public virtual bool Resizeable { get; set; }
    public virtual bool Closeable { get; set; }


    protected Vector2 _position;
    protected Vector2 _size;


    private bool _dragging;
    private bool _resizing;
    private Vector2 _originalSize;
    private Vector2 _originalPosition;
    private Vector2 _mouseOffset;
    protected float _accentLineWidth = 0.4f;

    protected readonly Dictionary<UiCols, Color> Colors = new();
    
    public UiBasic(Vector2 position, Vector2 size, Color color, string name = "UiBasic")
    {
        PositionInternal = position;
        InteractBarSize = new Vector2(size.X, 3f);
        MinSize = InteractBarSize.YY() * 5f;
        MaxSize = new Vector2(float.PositiveInfinity);
        SizeInternal = size;
        SetColInternal(UiCols.Main, color);
        SetColInternal(UiCols.Alternate, new Color(30,30,30));
        Draggable = true;
        Resizeable = true;
        Closeable = true;
        Name = name;
    }
    
    public virtual void DrawContent()
    {
        if (Draggable || Closeable || Resizeable)
        {
            var offset = new Vector2(_accentLineWidth);
            PolyRenderer.Rect(Position - offset, Position + Size + offset, MainColor);
            PolyRenderer.Rect(Position, Position + Size, AlternateColor);
            DrawInteractBar();
        }
        else
        {
            PolyRenderer.Rect(Position, Position + Size, AlternateColor);
        }
    }

    protected virtual void DrawInteractBar()
    {
        Graphics.polyBatcher.PushScissor(new Rectangle(Position, Position + Size));
        var crossSize = new Vector2(InteractBarSize.Y);
        var upRightPosition = Position + Size.ZeroY();
        var crossZeroY = crossSize.ZeroY();
        var crossZeroX = crossSize.ZeroX();
        var crossNegateX = crossSize.NegateX();
        
        if (Draggable)
        {
            PolyRenderer.Line(Position + InteractBarSize.MultiplyX(0.3f), Position + InteractBarSize.MultiplyX(0.7f),
                _accentLineWidth, MainColor);
        }

        if (Closeable)
        {
            PolyRenderer.Line(Position + crossZeroX, Position + crossZeroY, _accentLineWidth, MainColor);
            PolyRenderer.Line(Position + new Vector2(0, crossSize.Y * 0.5f),
                Position + new Vector2(crossSize.X * 0.5f, 0), _accentLineWidth, MainColor);
        }

        if (Resizeable)
        {
            PolyRenderer.Line(upRightPosition + crossNegateX + new Vector2(0f, _accentLineWidth * 0.5f), upRightPosition - crossZeroY, _accentLineWidth,
                MainColor);
            PolyRenderer.Line(upRightPosition + crossNegateX, upRightPosition + crossZeroX, _accentLineWidth,
                MainColor);
            PolyRenderer.Line(upRightPosition + crossNegateX * 0.5f + new Vector2(0f, _accentLineWidth * 0.5f), upRightPosition - crossZeroY * 0.5f,
                _accentLineWidth, MainColor);
            PolyRenderer.Line(upRightPosition + crossNegateX * 0.5f, upRightPosition + crossZeroX * 0.5f,
                _accentLineWidth, MainColor);
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


    protected void SetColInternal(UiCols type, Color col)
    {
        if (Colors.ContainsKey(type)) Colors[type] = col;
        else Colors.Add(type, col);
        OnColoured?.Invoke(this, type, col);
    }
    public void SetCol(UiCols type, Color col)
    {
        var old = GetCol(type);
        if(Colors.ContainsKey(type)) Colors[type] = col;
        else Colors.Add(type, col);
        HandleColoured(type, old);
    }

    public Color GetCol(UiCols type) => Colors.ContainsKey(type) ? Colors[type] : Color.Transparent;
    
    public virtual void Kill() => OnKilled?.Invoke(this);
    
    protected virtual void HandleClicked(MouseAction action)
    {
        if (Closeable && InputChecker.MouseGamePos.IsInsideRect(Position, InteractBarSize.YY()))
        {
            Kill();
            return;
        }

        if (Resizeable && InputChecker.MouseGamePos.IsInsideRect(Position + Size.ZeroY().SubtractX(InteractBarSize.Y), InteractBarSize.YY()))
        {
            _resizing = true;
            _originalPosition = Position;
            _originalSize = Size;
            _mouseOffset = InputChecker.MouseGamePos - Position;
            return;
        }

        if (Draggable && InputChecker.MouseGamePos.IsInsideRect(Position, Size.ReplaceY(InteractBarSize.Y)))
        {
            _dragging = true;
            _originalPosition = Position;
            _mouseOffset = InputChecker.MouseGamePos - Position;
            return;
        }
    }
    protected virtual void HandleUnClicked(MouseAction action)
    {
        _dragging = false;
        _resizing = false;
    }
    
    protected virtual void DoDragging()
    {
        PositionInternal = Vector2.Clamp(InputChecker.MouseGamePos, InputChecker.CurrentLayerScreenMin + _mouseOffset, InputChecker.CurrentLayerScreenMax - (Size - _mouseOffset)) - _mouseOffset;
    }
    protected virtual void DoResizing()
    {
        Vector2 mouseDiff = Vector2.Clamp(
                                InputChecker.MouseGamePos, 
                                InputChecker.CurrentLayerScreenMin + _mouseOffset.ZeroX(),
                                InputChecker.CurrentLayerScreenMax - _originalSize.SubtractX(_mouseOffset.X).ReplaceY(_mouseOffset.Y)
                                ) - _mouseOffset;
        
        PositionInternal = new Vector2(_originalPosition.X, Math.Min(_originalPosition.Y + _originalSize.Y - MinSize.Y, mouseDiff.Y));
        SizeInternal = new Vector2(mouseDiff.X - Position.X + _originalSize.X, _originalPosition.Y + _originalSize.Y - mouseDiff.Y);
    }

    

    protected virtual void HandlePositioned(Vector2 old)
    {
        
    }
    protected virtual void HandleResized(Vector2 old)
    {
        InteractBarSize = InteractBarSize.ReplaceX(Size.X);
    }
    
    protected virtual void HandleColoured(UiCols type, Color old)
    {

    }


    protected virtual void HandleScrolled(float scroll) { }
}