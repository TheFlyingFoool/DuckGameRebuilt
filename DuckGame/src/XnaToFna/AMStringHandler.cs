using DuckGame;
using SixLabors.ImageSharp.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static DuckGame.GoalType;

namespace XnaToFna
{
    public class AMStringHandler  // Added for AncientMysteries
    {
        private readonly StringBuilder _builder;

        public AMStringHandler()
        {
            _builder = new StringBuilder(64);
        }

        public AMStringHandler(int capacity)
        {
            _builder = new StringBuilder(capacity);
        }

        public void AppendLiteral(string value)
        {
            if (!string.IsNullOrEmpty(value))
                _builder.Append(value);
        }

        public void AppendLiteralNoGrow(string value)
        {
            if (!string.IsNullOrEmpty(value))
                _builder.Append(value);
        }

        public void AppendChar(char value)
        {
            _builder.Append(value);
        }

        public void AppendDGColorString(Color color)
        {
            _builder.Append('|');
            _builder.Append(color.r);
            _builder.Append(',');
            _builder.Append(color.g);
            _builder.Append(',');
            _builder.Append(color.b);
            _builder.Append('|');
        }

        public void Clear()
        {
            _builder.Clear();
        }

        public override string ToString()
        {
            return _builder.ToString();
        }

        public string ToStringAndClear()
        {
            string result = _builder.ToString();
            _builder.Clear(); // keeps internal capacity for reuse
            return result;
        }
    }
    public static class AncientMysteriesReplacements
    {
        private static Type AncientMysteriesMod;
        private static FieldInfo displayNameHueField;
        private static FieldInfo setDisplayNameField;
        private static FieldInfo displayNameHueReversedField;
        private static Mod targetMod;
        private static AMStringHandler amstringHandler = new AMStringHandler(32);
        private static AMStringHandler amstringHandler2 = new AMStringHandler(32);
        public static void UpdateModDisplayName()
        {
            if (targetMod == null)
            {
                string targetName = "AncientMysteries.AncientMysteriesMod";
                foreach (var mod in ModLoader.allMods)
                {
                    // GetType().FullName includes the namespace
                    if (mod.GetType().FullName == targetName)
                    {
                        targetMod = mod;
                        break;
                    }
                }
            }
            if (targetMod == null)
            {
                return;
            }

            if (AncientMysteriesMod == null)
            {
                AncientMysteriesMod = targetMod.GetType();
            }
            if (AncientMysteriesMod == null)
            {
                return;
            }
            if (displayNameHueField == null)
            {
                displayNameHueField = AncientMysteriesMod.GetField(
                "displayNameHue",
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Static);
            }
     

            if (displayNameHueField == null)
            {
                return;
            }


            if (displayNameHueReversedField == null)
            {
                displayNameHueReversedField = AncientMysteriesMod.GetField(
                "displayNameHueReversed",
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Static);
            }


            if (displayNameHueReversedField == null)
            {
                return;
            }

            float displayNameHue = 0f;
            displayNameHue = (float)displayNameHueField.GetValue(null);
            // was in Hooks_OnUpdate moved it here for easy of replacement
            if ((bool)displayNameHueReversedField.GetValue(null))
            {
                displayNameHueField.SetValue(null, displayNameHue - 0.009f);
                displayNameHue = (float)displayNameHueField.GetValue(null);
                if (displayNameHue <= 0f)
                {
                    displayNameHueField.SetValue(null, 0f);
                    displayNameHueReversedField.SetValue(null, false);
                }
            }
            else
            {
                displayNameHueField.SetValue(null, displayNameHue + 0.009f);
                displayNameHue = (float)displayNameHueField.GetValue(null);
                if (displayNameHue >= 1f)
                {
                    displayNameHueField.SetValue(null, 1f);
                    displayNameHueReversedField.SetValue(null, true);
                }
            }


            displayNameHue = (float)displayNameHueField.GetValue(null);

            DuckGame.Color color = HSL.Hue(displayNameHue);

            amstringHandler = new AMStringHandler(32);
            amstringHandler.AppendDGColorString(color);
            amstringHandler.AppendLiteralNoGrow("Ancient Mysteries");

            string result = amstringHandler.ToStringAndClear();

            if (setDisplayNameField == null)
            {
                setDisplayNameField = AncientMysteriesMod.GetField(
                "setDisplayName",
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance);
            }

            if (setDisplayNameField == null)
            {
                return;
            }


   
            var setter = setDisplayNameField.GetValue(targetMod) as System.Action<string>;
            if (setter != null)
                setter(result);
        }
        public static string GetName(float highlight)
        {
            float displayNameHue = 0f;
            if (displayNameHueField == null)
            {
                displayNameHueField = AncientMysteriesMod.GetField(
                "displayNameHue",
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Static);
            }
            if (displayNameHueField != null)
                displayNameHue = (float)displayNameHueField.GetValue(null);

            DuckGame.Color color = HSL.Hue(displayNameHue, highlight);

            amstringHandler2 = new AMStringHandler(32);
            amstringHandler2.AppendDGColorString(color);
            amstringHandler2.AppendLiteralNoGrow("Ancient Levels");

            return amstringHandler2.ToStringAndClear();
        }
    }
}
public static class HSL
{
    // Token: 0x0600007E RID: 126 RVA: 0x0000404E File Offset: 0x0000224E
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color RandomRGB()
    {
        return HSL.FromHslFloat(Rando.Float(0f, 1f), Rando.Float(0.7f, 1f), Rando.Float(0.45f, 0.65f), 1f);
    }

    // Token: 0x0600007F RID: 127 RVA: 0x00004087 File Offset: 0x00002287
    public static Color Hue(float hue)
    {
        return HSL.FromHslFloat(hue, 1f, 0.5f, 1f);
    }

    // Token: 0x06000080 RID: 128 RVA: 0x0000409E File Offset: 0x0000229E
    public static Color Hue(float hue, float lightness)
    {
        return HSL.FromHslFloat(hue, 1f, lightness, 1f);
    }

    // Token: 0x06000081 RID: 129 RVA: 0x000040B4 File Offset: 0x000022B4
    public static Color FromHslFloat(float h, float s, float l, float alpha = 1f)
    {
        if (l == 0f)
        {
            return Color.Black;
        }
        if (s <= 0.001f)
        {
            int num = (int)((byte)(255f * l));
            return new Color((float)num, (float)num, (float)num, alpha);
        }
        float num2 = (l < 0.5f) ? (l * (1f + s)) : (l + s - s * l);
        float v = 2f * l - num2;
        float num3 = HSL.HslToRgb(v, num2, h + 0.33333334f);
        float num4 = HSL.HslToRgb(v, num2, h);
        float num5 = HSL.HslToRgb(v, num2, h - 0.33333334f);
        return new Color((byte)(255f * num3), (byte)(255f * num4), (byte)(255f * num5), (byte)(255f * alpha));
    }

    internal static float HslToRgb(float v1, float v2, float vH)
    {
        if (vH < 0f)
        {
            vH += 1f;
        }
        else if (vH > 1f)
        {
            vH -= 1f;
        }
        if (6f * vH < 1f)
        {
            return v1 + (v2 - v1) * 6f * vH;
        }
        if (2f * vH < 1f)
        {
            return v2;
        }
        if (3f * vH >= 2f)
        {
            return v1;
        }
        return v1 + (v2 - v1) * (0.6666667f - vH) * 6f;
    }
}

