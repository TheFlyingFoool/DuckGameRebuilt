using System;
using System.Windows.Forms;

namespace DuckGame.ConsoleInterface.Panes
{
    public sealed class MMSplitPane : MMParentPane
    {
        public Orientation Orientation;
        public bool EditMode;

        public MMSplitPane(Orientation orientation, params MallardManagerPane[] panes)
        {
            Orientation = orientation;
            Children.AddRange(panes);
        }
        
        public override void Update()
        {
            base.Update();
            
            if (Keyboard.control && Keyboard.Pressed(Keys.E))
            {
                EditMode ^= true;
                return;
            }
            
            if (!EditMode)
                FocusedPane?.Update();
        }

        public override void DrawRaw(Depth depth, float deltaUnit)
        {
            if (EditMode)
                DrawEditMode(depth, deltaUnit);
            else DrawSplits(depth, deltaUnit);
        }

        private MallardManagerPane? _paneToMove = null;
        private int _paneToMoveIndex = -1;

        public void DrawEditMode(Depth depth, float zoom)
        {
            DrawSplits(depth - 2f, zoom, (i, splitPaneBounds) =>
            {
                MallardManagerPane pane = Children[i];

                if (pane is MMSplitPane splitPane)
                {
                    splitPane.DrawEditMode(depth, zoom);
                    return;
                }
                
                bool mouseHovering = splitPaneBounds.Contains(Mouse.positionConsole);
                Color overlayBoxColor = mouseHovering && Mouse.right == InputState.Down
                    ? MallardManager.Colors.UserOverlay * 0.35f
                    : Color.Black * MallardManager.Config.Opacity * 0.6f;
                
                Graphics.DrawRect(splitPaneBounds, overlayBoxColor, depth);

                Vec2 point1 = Keyboard.shift
                    ? splitPaneBounds.tl with {y = splitPaneBounds.Center.y}
                    : splitPaneBounds.tl with {x = splitPaneBounds.Center.x};

                Vec2 point2 = Keyboard.shift
                    ? splitPaneBounds.br with {y = splitPaneBounds.Center.y}
                    : splitPaneBounds.br with {x = splitPaneBounds.Center.x};

                if (!mouseHovering)
                    return;

                Rectangle closePaneBox = new(splitPaneBounds.tr - (zoom * 8, 0), zoom * 8, zoom * 8);
                Graphics.DrawOutlinedRect(closePaneBox, MallardManager.Colors.SecondarySystemText, MallardManager.Colors.UserOverlay, depth + 0.1f, zoom / 2);

                if (Mouse.right != InputState.Down)
                    Graphics.DrawLine(point1, point2, MallardManager.Colors.UserOverlay, zoom, depth);

                if (Mouse.right == InputState.Pressed)
                {
                    _paneToMove = pane;
                    _paneToMoveIndex = i;
                }
                else if (Mouse.right == InputState.Released && _paneToMoveIndex >= 0)
                {
                    Children[_paneToMoveIndex] = Children[i];
                    Children[i] = _paneToMove;
                    
                    _paneToMove = null;
                    _paneToMoveIndex = -1;
                }

                if (Mouse.left == InputState.Pressed)
                {
                    if (closePaneBox.Contains(Mouse.positionConsole))
                    {
                        Children[i].Active = false;
                        RemovePane(Children[i]);
                    }
                    else
                    {
                        Children[i] = new MMSplitPane(Keyboard.shift
                                ? Orientation.Vertical
                                : Orientation.Horizontal,
                            pane, new MMConsolePane());
                    }
                }
                else if (_paneToMove is not null)
                {
                    var oldBounds = _paneToMove.Bounds;
                    
                    _paneToMove.Bounds = new Rectangle(Mouse.positionConsole, splitPaneBounds.width / 2, splitPaneBounds.height / 2);
                    _paneToMove.Draw(depth + 1, zoom / 2);
                    _paneToMove.Bounds = oldBounds;
                }
            });
        }

        public void DrawSplits(Depth depth, float zoom, Action<int, Rectangle> postDrawAction = null)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                MallardManagerPane pane = Children[i];
                float width = Bounds.width;
                float height = Bounds.height;

                float xPos = Bounds.x;
                float yPos = Bounds.y;

                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        width /= Children.Count;
                        xPos += width * i;
                        break;
                    case Orientation.Vertical:
                        height /= Children.Count;
                        yPos += height * i;
                        break;
                    
                    default: throw new ArgumentOutOfRangeException();
                }

                Rectangle splitPaneBounds = new(xPos, yPos, width, height);
                
                // dont wanna put this in Update because extra
                // effort and calls, so it just sits with Draw
                if (splitPaneBounds.Contains(MallardPointer.LastClickPosition)
                    && FocusedPane != pane)
                {
                    FocusedPane = pane;
                }

                pane.Bounds = splitPaneBounds;
                pane.Draw(depth - 0.02f, zoom);

                postDrawAction?.Invoke(i, splitPaneBounds);
            }
        }
    }
}