using System;
using System.Drawing;

namespace DuckGame.ConsoleInterface.Panes
{
    public class MMTestPane : MallardManagerPane
    {
        public override bool Borderless { get; } = false;

        public Color Color;
        public Color Color2;
        
        public MMTestPane(Color color)
        {
            Color = color;
            Color2 = color;
        }
        
        public MMTestPane()
        {
            Color = Color.Random();
            Color2 = Color.Random();
        }
        
        public override void Update()
        {
            
        }

        public override void DrawRaw(Depth depth, float deltaUnit)
        {
            Graphics.DrawRect(Bounds, MallardManager.Colors.PrimaryBackground, depth);
            
            // AcrossTime.Do(this, TimeSpan.FromSeconds(1), progress => 
            // {
            //     Vec2 textPosition = Bounds.bl - (deltaUnit * -2, RebuiltMono.HEIGHT * deltaUnit + deltaUnit * 2); 
            //     Color color = Lerp.Color(Color, Color2, (float) progress.NormalizedValue); 
            //      
            //     RebuiltMono.Draw($"{Math.Round(progress.NormalizedValue, 2)}", textPosition, color, depth + 0.1f, deltaUnit); 
            // });

            Color[] colors =
            {
                MallardManager.Colors.UserText,
                MallardManager.Colors.PrimarySystemText,
                MallardManager.Colors.SecondarySystemText,
                MallardManager.Colors.UserOverlay,
                MallardManager.Colors.Primary,
                MallardManager.Colors.PrimarySub,
                MallardManager.Colors.Secondary,
                MallardManager.Colors.SecondarySub,
                MallardManager.Colors.PrimaryBackground,
                MallardManager.Colors.SecondaryBackground,
            };
            string[] colorNames =
            {
                "UserText",
                "PrimarySystemText",
                "SecondarySystemText",
                "UserOverlay",
                "Primary",
                "PrimarySub",
                "Secondary",
                "SecondarySub",
                "PrimaryBackground",
                "SecondaryBackground",
            };
            
            for (int i = 0; i < colors.Length; i++)
            {
                float size = deltaUnit * 8;
                Rectangle colorBox = new(Bounds.tl + new Vec2(deltaUnit * 2) + (0, i * (size + deltaUnit * 2)), size, size);
                
                if (!Bounds.Contains(colorBox.br))
                    break;
                
                Graphics.DrawRect(colorBox, colors[i], depth + 0.2f);
                RebuiltMono.Draw(colorNames[i], colorBox.tr + (deltaUnit, 0), MallardManager.Colors.PrimarySub, depth + 0.15f, deltaUnit);
            }
        }

        public override void OnFocus() { }
    }
}