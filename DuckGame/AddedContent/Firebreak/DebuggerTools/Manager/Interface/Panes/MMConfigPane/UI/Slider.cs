using System;
using System.Drawing;
using System.Linq;

namespace DuckGame.ConsoleInterface.Panes
{
    public partial class MMConfigPane
    {
        public static partial class UI
        {
            public class Slider : Element
            {
                public double Min;
                public double Max;
                public double Step;
                public double? SecondaryStep;
                private readonly int _displayRoundingDigits;

                public double Value
                {
                    get => _valueGetter();
                    set => _valueSetter(value);
                }

                private Func<double> _valueGetter;
                private Action<double> _valueSetter;
                private bool _mouseSettingValue;

                private Rectangle SliderBounds => new(
                    FrameBounds.x + FrameBounds.width - FrameBounds.height * 7 + 1.5f * DeltaUnit,
                    FrameBounds.Top + 1.5f * DeltaUnit,
                    FrameBounds.height * 7 - DeltaUnit - DeltaUnit * 2,
                    FrameBounds.height - DeltaUnit - DeltaUnit * 2);
                
                public Slider(double min, double max, double step, int displayRoundingDigits,
                    Func<double> valueGetter, Action<double> valueSetter)
                {
                    Min = min;
                    Max = max;
                    Step = step;
                    _displayRoundingDigits = displayRoundingDigits;
                    _valueGetter = valueGetter;
                    _valueSetter = valueSetter;
                }
                
                public override float Draw(Depth depth, float xOffset)
                {
                    Rectangle sliderBoundsOffset = SliderBounds;
                    sliderBoundsOffset.x -= xOffset;
                    
                    Graphics.DrawOutlinedRect(sliderBoundsOffset, MallardManager.Colors.Secondary,
                        MallardManager.Colors.SecondarySub, depth + 0.2f, DeltaUnit);
                    
                    ProgressValue sliderProgress = new(Value, 0, Min, Max);
                    
                    Rectangle sliderDotBounds = new(
                        sliderBoundsOffset.x + sliderBoundsOffset.width * (float)sliderProgress.NormalizedValue,
                        sliderBoundsOffset.y + sliderBoundsOffset.height * 0.5f,
                        sliderBoundsOffset.height,
                        sliderBoundsOffset.height);
                    
                    Graphics.DrawLine(sliderDotBounds.tl - (0, DeltaUnit + sliderDotBounds.height * 0.5f),
                        sliderDotBounds.bl + (0, DeltaUnit - sliderDotBounds.height * 0.5f),
                        MallardManager.Colors.UserOverlay, DeltaUnit, depth + 0.3f);

                    int decimalDigits = new[] {(SecondaryStep ?? Step).GetDecimalDigits(), Step.GetDecimalDigits()}.Max();
                    string formatString = _displayRoundingDigits > 0 ? $"0.{new string('0', decimalDigits)}" : "0";
                    string roundedValue = Math.Round(Convert.ToDouble(Value), _displayRoundingDigits).ToString(formatString);
                    SizeF textDimensions = RebuiltMono.GetDimensions(roundedValue, DeltaUnit);
                    
                    RebuiltMono.Draw(roundedValue,
                        sliderBoundsOffset.tl - (textDimensions.Width + DeltaUnit, -DeltaUnit * 0.5f),
                        MallardManager.Colors.SecondarySub, depth + 0.1f, DeltaUnit);

                    return sliderBoundsOffset.width + textDimensions.Width + DeltaUnit;
                }

                public override float Update(float xOffset)
                {
                    Rectangle sliderBoundsOffset = SliderBounds;
                    sliderBoundsOffset.x -= xOffset;

                    if (Mouse.left == InputState.Pressed
                        && sliderBoundsOffset.Contains(MousePosition))
                        _mouseSettingValue = true;
                    
                    if (Mouse.left == InputState.Released)
                        _mouseSettingValue = false;
                    
                    if (_mouseSettingValue)
                    {
                        ProgressValue sliderProgress = new(value: MousePosition.x, incrementSize: 0, min: sliderBoundsOffset.x, max: sliderBoundsOffset.x + sliderBoundsOffset.width);
                        
                        // // may god have mercy in the afterlife
                        // float[] possibleValues = new float[(int) (Max / Step) + 1];
                        // for (int i = 0; i < possibleValues.Length; i++)
                        // {
                        //     possibleValues[i] = (float)(Min + i * Step);
                        // }
                        //
                        // Value = Maths.Clamp(Min + Extensions.FindClosestNumber(possibleValues, (float) (sliderProgress.NormalizedValue * Max)), Min, Max);
                        
                        double usedStep = Keyboard.shift && SecondaryStep is not null ? SecondaryStep.Value : Step;
                        
                        double clampedValue = Maths.Clamp(sliderProgress.NormalizedValue * Max, Min, Max);
                        double difference = clampedValue - Min;
                        double steps = Math.Round(difference / usedStep);
                        Value = Min + steps * usedStep;
                    }
                    
                    string roundedValue = $"{Math.Round(Convert.ToDouble(Value), _displayRoundingDigits)}";
                    SizeF textDimensions = RebuiltMono.GetDimensions(roundedValue, DeltaUnit);
                    
                    return sliderBoundsOffset.width + textDimensions.Width + DeltaUnit;
                }
            }
        }
    }
}