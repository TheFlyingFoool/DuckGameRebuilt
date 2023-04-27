using System.Drawing;

namespace DuckGame.ConsoleInterface.Panes
{
    public class MMPagerPane : MMParentPane
    {
        private ProgressValue tabSwitchProgress = 1;
        private int s_oldFocusPaneIndex = -1;
        public MMPagerPane(params MallardManagerPane[] panes)
        {
            Children.AddRange(panes);
        }

        public override void Update()
        {
            base.Update();
            
            if (Keyboard.control
                && FocusedPane is not MMPagerPane)
            {
                if (Keyboard.Pressed(Keys.Tab)
                    && Children.Count > 1)
                {
                    s_oldFocusPaneIndex = s_focusPaneIndex;
                    s_focusPaneIndex++;
                    s_focusPaneIndex %= Children.Count;
                    OnFocusedPaneChange();
                }
                else if (Keyboard.Pressed(Keys.T))
                {
                    AddDefaultNewPane();
                }
                else if (Keyboard.Pressed(Keys.W))
                {
                    RemovePane(FocusedPane);
                }
            }
            else
            {
                FocusedPane?.Update();
            }

            if (!tabSwitchProgress.Completed)
            {
                tabSwitchProgress++;
                tabSwitchProgress = ~tabSwitchProgress;
            }
        }

        protected override void OnFocusedPaneChange()
        {
            base.OnFocusedPaneChange();
            tabSwitchProgress = 0;
        }

        public override void DrawRaw(Depth depth, float deltaUnit)
        {
            float renderSpaceTop = 20 * deltaUnit;
            Rectangle tabSpace = Bounds with {height = renderSpaceTop};

            Rectangle renderSpace = new(tabSpace.bl, tabSpace.br + new Vec2(0, Bounds.height - renderSpaceTop));

            DrawTabs(tabSpace, 30f, depth + 0.02f, deltaUnit);

            if (FocusedPane is null)
                return;
            
            FocusedPane.Bounds = renderSpace;
            FocusedPane.Draw(depth, deltaUnit);
        }

        private void DrawTabs(Rectangle bounds, float width, Depth depth, float zoom)
        {
            width *= zoom;

            Color boundBoxBgColor = MallardManager.Colors.PrimaryBackground;
            Color unselectedColor = MallardManager.Colors.Secondary;
            Color unselectedBorderColor = MallardManager.Colors.SecondarySub;
            Color selectedColor = MallardManager.Colors.Primary;
            Color selectedBorderColor = MallardManager.Colors.PrimarySub;
            
            Graphics.DrawRect(bounds, boundBoxBgColor, depth);

            Vec2 farthestTabBR = Vec2.Zero;
            
            for (int i = 0; i < Children.Count; i++)
            {
                bool isSelected = s_focusPaneIndex == i;
                bool wasSelected = s_oldFocusPaneIndex == i;

                ProgressValue multiplier = Ease.Out.Exponential(tabSwitchProgress);
                float selectionOffset = (isSelected ? 4 * zoom * -multiplier : wasSelected ? 4 * zoom * multiplier : 0) + (wasSelected ? 0 : 4 * zoom);
                float doubleZoom = zoom * 2;
                
                Rectangle tabBox = new(bounds.x + (i * width) + zoom, bounds.y + selectionOffset + zoom, width - doubleZoom, bounds.height - selectionOffset - doubleZoom);
                farthestTabBR = tabBox.br;
                
                if (tabBox.tr.x > bounds.tr.x)
                    break;

                Color tabBoxColor = isSelected ? selectedColor : unselectedColor;
                Color tabBoxBorderColor = isSelected ? selectedBorderColor : unselectedBorderColor;
                
                Graphics.DrawRect(tabBox, tabBoxColor, depth, true);
                Graphics.DrawRect(tabBox, tabBoxBorderColor, depth + 0.02f, false, zoom);
                
                Vec2 textPos = tabBox.tl + new Vec2(doubleZoom);
                RebuiltMono.Draw($"{i+1}", textPos, tabBoxBorderColor, depth + 0.04f, zoom);

                if (Mouse.left == InputState.Pressed 
                    && tabBox.Contains(Mouse.positionConsole) 
                    && i != s_focusPaneIndex) 
                {
                    s_oldFocusPaneIndex = s_focusPaneIndex;
                    s_focusPaneIndex = i;
                    OnFocusedPaneChange();
                }
            }

            float addTabBoxWidth = (width - zoom * 2) / 2;
            farthestTabBR += (zoom * 2, -addTabBoxWidth);
            Rectangle addTabBox = new(farthestTabBR, farthestTabBR + new Vec2(addTabBoxWidth));

            bool mouseHoversAddBox = addTabBox.Contains(Mouse.positionConsole);
            
            Color addBoxColor =       mouseHoversAddBox ? selectedColor       : unselectedColor;
            Color addBoxBorderColor = mouseHoversAddBox ? selectedBorderColor : unselectedBorderColor;

            Graphics.DrawRect(addTabBox, addBoxColor, depth, true);
            Graphics.DrawRect(addTabBox, addBoxBorderColor, depth + 0.02f, false, zoom);
            RebuiltMono.Draw("+", addTabBox.Center + (zoom / 2, zoom * 2), addBoxBorderColor, depth + 0.02f, zoom, ContentAlignment.MiddleCenter);

            if (mouseHoversAddBox && Mouse.left == InputState.Pressed)
            {
                AddDefaultNewPane();
            }
        }

        protected void AddDefaultNewPane()
        {
            AddPane(new MMConsolePane());
        }
    }
}