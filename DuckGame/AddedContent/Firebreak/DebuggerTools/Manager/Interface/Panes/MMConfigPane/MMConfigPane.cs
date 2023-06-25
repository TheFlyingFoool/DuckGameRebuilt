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
    public partial class MMConfigPane : MallardManagerPane
    {
        public override bool Borderless { get; } = false;
        private Dictionary<string, List<UI.AdvancedConfigItem>> _configItems = new();
        private int _configScrollIndex = 0;

        public MMConfigPane()
        {
            foreach ((TypeInfo type, AdvancedConfig attribute) in AdvancedConfig.All)
            {
                List<UI.AdvancedConfigItem> items = new();

                foreach (FieldInfo configItem in type.DeclaredFields)
                {
                    object instance = AdvancedConfig.Get(type.AsType());
                    
                    if (configItem.GetCustomAttribute<ACHiddenAttribute>() != null)
                        continue;

                    items.Add(new UI.AdvancedConfigItem()
                    {
                        Name = configItem.Name,
                        Type = configItem.FieldType,
                        ValueGetter = () => configItem.GetValue(instance),
                        ValueSetter = newValue => configItem.SetValue(instance, Convert.ChangeType(newValue, configItem.FieldType)),
                        Attributes = configItem.GetCustomAttributes().Where(x => x is not ACHeaderAttribute).ToList(),
                        IsHeader = configItem.GetCustomAttribute<ACHeaderAttribute>() != null,
                        ItemFieldInfo = configItem
                    }.InitializeElements());
                }
                
                _configItems.Add(type.Name, items);
            }
        }

        public override void Update()
        {
            if (Mouse.scrollingUp)
                _configScrollIndex++;
            else if (Mouse.scrollingDown)
                _configScrollIndex--;

            float deltaUnit = MallardManager.Config.Zoom;
            
            float yOffset = _configScrollIndex * (RebuiltMono.HEIGHT + 1) * deltaUnit;
            foreach (KeyValuePair<string, List<UI.AdvancedConfigItem>> config in _configItems)
            {
                yOffset += (RebuiltMono.HEIGHT + 1) * deltaUnit;
                List<UI.AdvancedConfigItem> items = config.Value;

                foreach (UI.AdvancedConfigItem item in items)
                {
                    int xfactor = (int) Bounds.width / 4;
                    yOffset += item.Update(new Rectangle(Bounds.tl + (xfactor, yOffset), Bounds.width - (xfactor * 2), 12 * deltaUnit), deltaUnit) + deltaUnit * 2;
                }

                yOffset += 6 * deltaUnit;
            }
        }

        public override void DrawRaw(Depth depth, float deltaUnit)
        {
            Graphics.DrawRect(Bounds, MallardManager.Colors.PrimaryBackground, depth);

            float yOffset = _configScrollIndex * (RebuiltMono.HEIGHT + 1) * deltaUnit;
            foreach (KeyValuePair<string, List<UI.AdvancedConfigItem>> config in _configItems)
            {
                string configName = config.Key.EndsWith("Config") ? config.Key.Substring(0, config.Key.Length - 6) : config.Key;
                configName = Extensions.SplitByUppercaseRegex.Replace(configName, "$0 ").TrimEnd();
                RebuiltMono.Draw(configName, Bounds.tl + (Bounds.width / 2, yOffset + 2 * deltaUnit), MallardManager.Colors.PrimarySub, depth + 0.1f, deltaUnit, ContentAlignment.TopCenter);
                yOffset += (RebuiltMono.HEIGHT + 1) * deltaUnit;
                List<UI.AdvancedConfigItem> items = config.Value;

                foreach (UI.AdvancedConfigItem item in items)
                {
                    int xfactor = (int) Bounds.width / 4;
                    yOffset += item.Draw(new Rectangle(Bounds.tl + (xfactor, yOffset), Bounds.width - (xfactor * 2), 12 * deltaUnit), depth, deltaUnit) + deltaUnit * 2;
                }
                
                if (AdvancedConfig.Get<ExperimentalConfig>().Boolean)
                    RebuiltMono.Draw($"{Mouse.xConsole},{Mouse.yConsole}", Mouse.positionConsole, Color.Red, 2f, deltaUnit);

                yOffset += 6 * deltaUnit;
            }
        }

        public override void OnFocus()
        {
        }
    }
}