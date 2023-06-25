using System;
using System.Drawing;
using System.Windows.Controls;

namespace DuckGame.ConsoleInterface.Panes
{
    public partial class MMConfigPane
    {
        public static partial class UI
        {
            public class Label : Element
            {
                public string DisplayString => _displayStringGetter();
                private Func<string> _displayStringGetter;
                
                public Label(string displayString)
                {
                    _displayStringGetter = () => displayString;
                }
                
                public Label(Func<string> displayStringGetter)
                {
                    _displayStringGetter = displayStringGetter;
                }
                
                public override float Draw(Depth depth, float xOffset)
                {
                    string displayString = DisplayString;
                    SizeF textDimensions = RebuiltMono.GetDimensions(displayString, DeltaUnit);

                    RebuiltMono.Draw(displayString, (FrameBounds.Right - DeltaUnit - xOffset, FrameBounds.Top + DeltaUnit * 2), MallardManager.Colors.SecondarySub, depth + 0.1f, DeltaUnit, ContentAlignment.TopRight);
                    
                    return textDimensions.Width;
                }

                public override float Update(float xOffset)
                {
                    return RebuiltMono.GetDimensions(DisplayString, DeltaUnit).Width;
                }
            }
        }
    }
}