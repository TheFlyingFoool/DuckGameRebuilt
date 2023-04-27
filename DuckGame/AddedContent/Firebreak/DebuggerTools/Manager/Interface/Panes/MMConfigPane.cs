using Humanizer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;

namespace DuckGame.ConsoleInterface.Panes
{
    public class MMConfigPane : MallardManagerPane
    {
        public override bool Borderless { get; } = false;
        private Dictionary<string, List<AdvancedConfigItem>> _configItems = new();
        private int _configScrollIndex = 0;

        public MMConfigPane()
        {
            foreach ((TypeInfo type, AdvancedConfig attribute) in AdvancedConfig.All)
            {
                List<AdvancedConfigItem> items = new();

                foreach (FieldInfo configItem in type.DeclaredFields)
                {
                    var instance = AdvancedConfig.Get(type.AsType());

                    items.Add(new AdvancedConfigItem()
                    {
                        Name = configItem.Name,
                        Type = configItem.FieldType,
                        Value = configItem.GetValue(instance),
                        Attributes = configItem.GetCustomAttributes().ToList(),
                        IsHeader = configItem.GetCustomAttribute<ACHeaderAttribute>() != null
                    });
                }
                
                _configItems.Add(type.Name, items);
            }
        }

        public override void Update()
        {
            if (Mouse.scrollingUp)
                _configScrollIndex--;
            else if (Mouse.scrollingDown)
                _configScrollIndex++;
        }

        public override void DrawRaw(Depth depth, float deltaUnit)
        {
            Graphics.DrawRect(Bounds, MallardManager.Colors.PrimaryBackground, depth);
            
            float yOffset = 0;
            int i = 0;
            foreach (var pair in _configItems)
            {
                string category = pair.Key;
                List<AdvancedConfigItem> items = pair.Value;
                
                RebuiltMono.Draw(category, Bounds.tl + (deltaUnit * 2, deltaUnit + yOffset), MallardManager.Colors.Secondary, depth + 0.1f, deltaUnit * 1.5f);
                yOffset += RebuiltMono.HEIGHT * deltaUnit * 1.5f;

                foreach (AdvancedConfigItem item in items)
                {
                    Rectangle itemBoundBox = new(Bounds.tl + (0, yOffset - deltaUnit * 2), Bounds.width, RebuiltMono.HEIGHT * deltaUnit);
                    
                    if (itemBoundBox.Bottom > Bounds.Bottom)
                        break;

                    bool selected = itemBoundBox.Contains(Mouse.positionConsole);
                    
                    if (selected)
                        Graphics.DrawRect(itemBoundBox, MallardManager.Colors.SecondaryBackground, depth + 0.05f);

                    RebuiltMono.Draw(item.Name, Bounds.tl + (deltaUnit * 2, yOffset), MallardManager.Colors.Primary, depth + 0.1f, deltaUnit);
                    
                    float valueModXOffset;
                    bool doRenderValue;

                    if (!item.IsHeader)
                        RenderConfigItem(yOffset, item, deltaUnit, depth, selected, out doRenderValue, out valueModXOffset);
                    else
                        RenderHeader(yOffset, item, deltaUnit, depth, selected, out doRenderValue, out valueModXOffset);

                    if (doRenderValue)
                        RebuiltMono.Draw(item.Type.InheritsFrom(typeof(Enum)) ? $"{((Enum) item.Value).Humanize()}" : JsonConvert.SerializeObject(item.Value), Bounds.tr + (-1 * (deltaUnit * 2 + valueModXOffset), yOffset), MallardManager.Colors.Primary, depth + 0.1f, deltaUnit, ContentAlignment.TopRight);

                    i++;

                    yOffset += RebuiltMono.HEIGHT * deltaUnit + deltaUnit;
                }

                yOffset += deltaUnit * 6;
            }
        }

        private void RenderHeader(float yOffset, AdvancedConfigItem item, float deltaUnit, Depth depth, bool selected, out bool doRenderValue, out float valueModXOffset)
        {
            valueModXOffset = 0;
            doRenderValue = false;
        }

        private void RenderConfigItem(float yOffset, AdvancedConfigItem item, float deltaUnit, Depth depth, bool selected, out bool doRenderValue, out float valueModXOffset)
        {
            valueModXOffset = 0;
            doRenderValue = true;
            
            if (item.Type == typeof(float) 
                && item.Attributes.Count(x => x is ACMinAttribute or ACMaxAttribute) == 2)
            {
                valueModXOffset += deltaUnit * 66;          
                double min = ((ACMinAttribute)item.Attributes.First(x => x is ACMinAttribute)).Value;
                double max = ((ACMaxAttribute)item.Attributes.First(x => x is ACMaxAttribute)).Value;
                double incrementValue = ((ACIncrementValueAttribute)item.Attributes.FirstOrDefault(x => x is ACIncrementValueAttribute))?.Value ?? 0.05;            
                double completionRatio = double.Parse(item.Value.ToString()) / (max - min);         
                Rectangle sliderBox = new(Bounds.Right - deltaUnit * 66, yOffset + (RebuiltMono.HEIGHT / 2f * deltaUnit) - 2 * deltaUnit, deltaUnit * 64, deltaUnit * 2);
                Rectangle currentValueBox = new((float) (sliderBox.tl.x + sliderBox.width * completionRatio) - deltaUnit * 2, sliderBox.y - deltaUnit * 2, deltaUnit * 2, deltaUnit * 6);
                Rectangle sliderHitBox = new(sliderBox.x, currentValueBox.y, sliderBox.width, currentValueBox.height);
                
                Graphics.DrawRect(sliderBox, MallardManager.Colors.Primary, depth + 0.1f);
                Graphics.DrawRect(currentValueBox, MallardManager.Colors.UserOverlay, depth + 0.15f);
                if (Mouse.left == InputState.Down && selected)
                {
                    float mouseX = Mouse.xConsole + deltaUnit;
                    
                    if (mouseX <= sliderHitBox.Left)
                        item.Value = min;
                    else if (mouseX >= sliderHitBox.Right)
                        item.Value = max;
                    else
                    {
                        mouseX -= sliderHitBox.Left;
                        double newCompletionRatio = mouseX / ((max - min) * sliderBox.width);           
                        item.Value = newCompletionRatio - ((newCompletionRatio - incrementValue / 2) % incrementValue) + incrementValue / 2;
                        item.Value = Math.Round((double) item.Value, 7);
                    }
                }
            }
            else if (item.Type == typeof(bool))
            {
                valueModXOffset += deltaUnit * 10;
                doRenderValue = false;

                bool isTrue = (bool) item.Value;
                
                Rectangle checkbox = new(Bounds.Right - valueModXOffset, yOffset + 2, deltaUnit * 8, deltaUnit * 8);
                Color fillColor = isTrue
                    ? MallardManager.Colors.UserOverlay
                    : MallardManager.Colors.Primary;
                Color outlineColor = MallardManager.Colors.PrimarySub;

                Graphics.DrawOutlinedRect(checkbox, fillColor, outlineColor, depth + 0.1f, deltaUnit);

                if (Mouse.left == InputState.Pressed && selected)
                {
                    isTrue ^= true;
                    item.Value = isTrue;
                }
            }
        }

        public override void OnFocus()
        {
        }
    }
}