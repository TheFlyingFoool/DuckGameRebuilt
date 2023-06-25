using DuckGame.ConsoleInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace DuckGame.ConsoleInterface.Panes
{
    public partial class MMConfigPane
    {
        public static partial class UI
        {
            public class AdvancedConfigItem
            {
                public string Name;
                public Type Type;
                public Func<object> ValueGetter;
                public Action<object> ValueSetter;
                public List<Attribute> Attributes = new();
                public bool IsHeader = false;
                public List<Element> Elements = new();
                public FieldInfo ItemFieldInfo;

                public AdvancedConfigItem InitializeElements()
                {
                    if (Type == typeof(bool))
                    {
                        Elements.Add(new Checkbox(
                            () => (bool) ValueGetter(),
                            newValue => ValueSetter(newValue)
                        ));
                    }
                    else if (Type.IsNumericType())
                    {
                        int displayRounding = Attributes.FirstOf<ACRoundingAttribute>()?.Decimals ?? 2;

                        double minValue = Attributes.FirstOf<ACMinAttribute>()?.Value ?? Extensions.GetMinValue(Type);
                        double maxValue = Attributes.FirstOf<ACMaxAttribute>()?.Value ?? Extensions.GetMaxValue(Type);

                        if (Attributes.FirstOf<ACDiscreteAttribute>() is not null)
                        {
                            Elements.Add(new Checkbox(
                                () => Math.Abs(Convert.ToDouble(ValueGetter()) - maxValue) < float.Epsilon,
                                b => ValueSetter(b ? maxValue : minValue)
                            ));
                        }

                        if (Attributes.FirstOf<ACSliderAttribute>() is { } sliderAttribute)
                        {
                            Elements.Add(new Slider(minValue, maxValue, sliderAttribute.Step, displayRounding,
                                () => Convert.ToDouble(ValueGetter()),
                                newValue => ValueSetter(newValue)
                            ) { SecondaryStep = sliderAttribute.SecondaryStep });
                        }
                        
                        // text box
                    }
                    else if (Type == typeof(string))
                    {
                        if (Attributes.FirstOf<ACKeybindAttribute>() is { } keybindAttribute)
                        {
                            // button that listens for keybind and displays it
                            Elements.Add(new Button(null));
                            keybindAttribute.InitializeMembers(ItemFieldInfo, ItemFieldInfo.DeclaringType.GetTypeInfo());
                            Elements.Add(new Label(keybindAttribute.KeybindString));
                        }
                        else if (Attributes.FirstOf<ACFileAttribute>() is { } fileAttribute)
                        {
                            // button that opens file explorer
                        }
                        else if (Attributes.FirstOf<ACColorAttribute>() is { } colorAttribute)
                        {
                            // textbox and color display
                        }
                        
                        // text box
                    }
                    
                    return this;
                }

                public float Draw(Rectangle boundingBox, Depth depth, float deltaUnit)
                {
                    float yOffset = boundingBox.height;

                    if (boundingBox.Contains(Mouse.positionConsole))
                    {
                        Rectangle outlineRect = new(
                            boundingBox.x - deltaUnit,
                            boundingBox.y - deltaUnit,
                            boundingBox.width + 2 * deltaUnit,
                            boundingBox.height + 2 * deltaUnit);

                        // Graphics.DrawRect(outlineRect, MallardManager.Colors.SecondarySub, depth, false, deltaUnit);
                        Graphics.DrawLine(outlineRect.tl - (deltaUnit * 0.5f, 0),
                            outlineRect.bl - (deltaUnit * 0.5f, 0), MallardManager.Colors.SecondarySub, deltaUnit,
                            depth);
                        Graphics.DrawLine(outlineRect.tr + (deltaUnit * 0.5f, 0),
                            outlineRect.br + (deltaUnit * 0.5f, 0), MallardManager.Colors.SecondarySub, deltaUnit,
                            depth);
                    }

                    Graphics.DrawRect(boundingBox, MallardManager.Colors.SecondaryBackground, depth);
                    RebuiltMono.Draw(Extensions.SplitByUppercaseRegex.Replace(Name, "$0 ").TrimEnd(),
                        boundingBox.tl + new Vec2(2 * deltaUnit), MallardManager.Colors.SecondarySub, depth + 0.1f,
                        deltaUnit);

                    float xOffset = 0;
                    foreach (Element element in Elements)
                    {
                        element.FrameBounds = boundingBox;
                        
                        float horizontalSpacing = 2 * deltaUnit;
                        xOffset += element.Draw(depth, xOffset) + horizontalSpacing;
                    }
                    
                    // if (Type == typeof(bool))
                    // {
                    //     // checkbox
                    //     Checkbox cb = new(boundingBox, (bool)Value);
                    //     
                    //     cb.Draw(depth);
                    // }
                    // else if (Type.IsNumericType())
                    // {
                    //     double? minValue = ((ACMinAttribute)Attributes.FirstOrDefault(x => x is ACMinAttribute))?.Value;
                    //     double? maxValue = ((ACMaxAttribute)Attributes.FirstOrDefault(x => x is ACMaxAttribute))?.Value;
                    //     int decimalRounding =
                    //         ((ACRoundingAttribute)Attributes.FirstOrDefault(x => x is ACRoundingAttribute))?.Decimals ??
                    //         2;
                    //     // double step = ((ACIncrementAttribute)Attributes.FirstOrDefault(x => x is ACIncrementAttribute))?.Value ?? 1;
                    //
                    //     if (minValue is not null && maxValue is not null) // slider
                    //     {
                    //         Rectangle sliderBounds = new(
                    //             boundingBox.x + boundingBox.width - boundingBox.height * 7 + 1.5f * deltaUnit,
                    //             boundingBox.Top + 1.5f * deltaUnit,
                    //             boundingBox.height * 7 - deltaUnit - deltaUnit * 2,
                    //             boundingBox.height - deltaUnit - deltaUnit * 2);
                    //
                    //         Graphics.DrawOutlinedRect(sliderBounds, MallardManager.Colors.Secondary,
                    //             MallardManager.Colors.SecondarySub, depth + 0.2f, deltaUnit);
                    //
                    //         ProgressValue sliderProgress =
                    //             new(Convert.ToDouble(Value), 0, minValue.Value, maxValue.Value);
                    //
                    //         Rectangle sliderDotBounds = new(
                    //             sliderBounds.x + sliderBounds.width * (float)sliderProgress.NormalizedValue,
                    //             sliderBounds.y + sliderBounds.height * 0.5f,
                    //             sliderBounds.height,
                    //             sliderBounds.height);
                    //
                    //         Graphics.DrawLine(sliderDotBounds.tl - (0, deltaUnit + sliderDotBounds.height * 0.5f),
                    //             sliderDotBounds.bl + (0, deltaUnit - sliderDotBounds.height * 0.5f),
                    //             MallardManager.Colors.UserOverlay, deltaUnit, depth + 0.3f);
                    //
                    //         string roundedValue = $"{Math.Round(Convert.ToDouble(Value), decimalRounding)}";
                    //         SizeF textDimensions = RebuiltMono.GetDimensions(roundedValue, deltaUnit);
                    //         RebuiltMono.Draw(roundedValue,
                    //             sliderBounds.tl - (textDimensions.Width + deltaUnit, -deltaUnit * 0.5f),
                    //             MallardManager.Colors.SecondarySub, depth + 0.1f, deltaUnit);
                    //     }
                    //     else // textbox
                    //     {
                    //     }
                    // }
                    // else if (Type == typeof(string))
                    // {
                    //     // check if ackeybind
                    //     // check if accolor
                    //     // check if acfile
                    //
                    //     // textbox
                    // }
                    // else if (Type.InheritsFrom<Enum>())
                    // {
                    //     // dropdown with search
                    // }
                    // else if (Type == typeof(Vec2))
                    // {
                    //     // check if acscreenposition
                    //
                    //     // two textboxes
                    // }

                    return yOffset;
                }

                public float Update(Rectangle boundingBox, float deltaUnit)
                {
                    float xOffset = 0;
                    foreach (Element element in Elements)
                    {
                        element.FrameBounds = boundingBox;
                        
                        float horizontalSpacing = 2 * deltaUnit;
                        xOffset += element.Update(xOffset) + horizontalSpacing;
                    }

                    return boundingBox.height;
                }
            }
        }
    }
}