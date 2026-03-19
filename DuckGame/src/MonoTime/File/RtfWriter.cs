using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DuckGame
{
    /// <summary>
    /// Replacement for "System.Windows.Forms.RichTextBox"'s SelectionColor() + SaveFile() functionality.
    /// With support for DGR color syntax, example: "|RED|red text |PREV| normal text".
    /// Created to remove the WinForms references with a functional equivalent.
    /// </summary>
    public class RtfWriter
    {
        public static class Tag
        {
            public const string ColorFont = @"\cf";
            public const string Unicode = @"\u";
            public const string NewLine = @"\par ";

            public const string Header = @"{\rtf1\ansi";
            public const string HeaderColorTable = @"{\colortbl ;";
            public const string HeaderFontTable = @"{\fonttbl ;";

            // These optional headers are used by WinForm's RichTextBox to define additional options like language and font:
            public const string HeaderLang = @"\ansicpg1252\deff0\nouicompat\deflang1033";
            public const string HeaderFonts = @"{\fonttbl{\f0\fnil\fcharset0 Microsoft Sans Serif; } }";
            public const string HeaderMsFontSize = @"{\*\generator Riched20 10.0.26100}\viewkind4\uc1\pard\cf1\f0\fs17";

            // HEADERS INFO:
            // \rtf1            RTF version 1.x
            // \ansi            Select ANSI charsed, used along with \ansicpgXXXX. UTF-8 encoding is not supported in RTF.
            // \ansicpg1252     Windows Code Page 1252  (Western European (Latin-1 Windows)). Tis is the recommended international value. https://learn.microsoft.com/en-us/windows/win32/intl/code-page-identifiers
            // \deff0           Sets default font to f0
            // \nouicompat      Sets "no UI compatibility mode" to remove legacy behavior.
            // \deflang1033     Sets the default language using an LCID (Language Code Identifier), used for spell check and hyphenation. 1033 = English (US). https://help.tradestation.com/10_00/eng/tsdevhelp/elobject/class_el/lcid_values.htm
            // \fnil            "font nil" selects the default system font as a fallback font.
            // \fcharset0       Sets the char-set for the font to 0 = ANSI.
            // {\*\generator Riched20 10.0.26100}       Name of the software that generated the RTF, this is the Windows RichEdit tag.
            // \viewkind4       Sets the viewer to use for display. 0 = normal. 1 = outline. 4 = print layout.
            // \uc1             Sets the unicode fallback length to 1 char.
            // \pard            Resets paragraph formatting: indent, alignment and spacing.
            // \fs17            Sets font-size to 17 half-points.
        }

        public static void AppendColorFontTag(StringBuilder rtf, int colorIndex)
        {
            rtf.Append(Tag.ColorFont);
            rtf.Append(colorIndex);
            rtf.Append(' ');
        }

        public static void AppendColorFontTag(StringBuilder rtf, List<Color> colorList, Color color)
        {
            if (!colorList.Contains(color)) {
                colorList.Add(color);
            }
            int colorIndex = colorList.IndexOf(color) + 1;
            AppendColorFontTag(rtf, colorIndex);
        }

        public static bool IsUnicodeChar(char c) => c > 127;

        private static void AppendAsUnicodeEscapedChar(StringBuilder rtf, char c)
            => rtf.Append(Tag.Unicode).Append((short)c).Append('?');

        public static void AppendEscapingUnicodeChars(StringBuilder rtf, char c)
        {
            if (IsUnicodeChar(c)) {
                AppendAsUnicodeEscapedChar(rtf, c);
            }
            else {
                rtf.Append(c);
            }
        }

        public const string SpecialCharacters = @"{}\";
        public static bool IsSpecialChar(char c)
            => SpecialCharacters.Contains(c);

        private static void AppendAsEscapedSpecialChar(StringBuilder rtf, char c)
            => rtf.Append('\\').Append(c);

        public static string EscapeSpecialChar(char c)
            => IsSpecialChar(c) ? $"\\{c}" : c.ToString();

        public static void AppendEscapingUnicodeAndSpecialChars(StringBuilder rtf, char c)
        {
            if (IsSpecialChar(c)) {
                AppendAsEscapedSpecialChar(rtf, c);
            }
            else {
                AppendEscapingUnicodeChars(rtf, c);
            }
        }

        public static void AppendEscapingUnicodeAndSpecialChars(StringBuilder rtf, string text)
        {
            foreach (char c in text ?? "") {
                AppendEscapingUnicodeAndSpecialChars(rtf, c);
            }
        }

        public static string EscapeText(string text)
        {
            var rtf = new StringBuilder();
            AppendEscapingUnicodeAndSpecialChars(rtf, text);
            return rtf.ToString();
        }

        public static string CreateRtfDoc(List<Color> colors, StringBuilder body)
        {
            var rtf = new StringBuilder(body.Length + 228);
            AppendSection(rtf, colors, body);
            rtf.Append('\0'); // This is optional. Added to mimic the same behavior as the previous RichTextBox method.
            return rtf.ToString();
        }

        public static void AppendSection(StringBuilder rtf, List<Color> colors, StringBuilder body)
        {
            rtf.AppendLine(Tag.Header);
            rtf.AppendLine(Tag.HeaderLang);
            rtf.AppendLine(Tag.HeaderFonts);
            rtf.AppendLine(Tag.HeaderMsFontSize);
            rtf.AppendLine(Tag.HeaderColorTable);
            foreach (var c in colors)
            {
                AppendHeaderColor(rtf, c);
            }
            rtf.AppendLine("}");
            rtf.Append(body);
            rtf.AppendLine("\n}");
        }

        public static void AppendHeaderColor(StringBuilder rtf, Color c)
            => AppendHeaderColor(rtf, c.r, c.g, c.b);

        public static void AppendHeaderColor(StringBuilder rtf, byte red, byte green, byte blue)
        {
            rtf.Append(@"\red");
            rtf.Append(red);
            rtf.Append(@"\green");
            rtf.Append(green);
            rtf.Append(@"\blue");
            rtf.Append(blue);
            rtf.AppendLine(";");
        }

        public static string CreateRtfFromDgLogs(string text)
        {
            var parsedLog = ParseDgLogs(text);
            return CreateRtfDoc(parsedLog.Colors, parsedLog.Body);
        }

        public static (List<Color> Colors, StringBuilder Body) ParseDgLogs(string text)
        {
            var colors = new List<Color> { Color.Black };
            var body = new StringBuilder((text?.Length ?? 0) * 2);
            if (string.IsNullOrEmpty(text)) {
                return (colors, body);
            }
            var currentColor = Color.Black;
            var previousColor = Color.Black;

            for (int letterIndex = 0; letterIndex < text.Length; letterIndex++)
            {
                char c = text[letterIndex];

                if (c == '|')
                {
                    var tag = ParseDgPipeTag(text, ref letterIndex);
                    Color parsedColor = ParseDgColorName(tag.Text, previousColor);
                    bool isValidColor = parsedColor != Color.Transparent;

                    if (isValidColor)
                    {
                        previousColor = currentColor;
                        currentColor = parsedColor;
                        if (currentColor == Color.White) {
                            currentColor = Color.Black;
                        }
                        if (currentColor != previousColor) {
                            AppendColorFontTag(body, colors, currentColor);
                        }
                    }
                    else {
                        body.Append(c);
                        AppendEscapingUnicodeAndSpecialChars(body, tag.Text);
                        if (tag.ClosingChar.HasValue) {
                            AppendEscapingUnicodeAndSpecialChars(body, tag.ClosingChar.Value);
                        }
                    }
                }
                else if (c == '\n' || c == '\r') {
                    previousColor = Color.Black;
                    if (currentColor != Color.Black) {
                        AppendColorFontTag(body, colors, Color.Black);
                        currentColor = Color.Black;
                    }
                    body.AppendLine(Tag.NewLine);
                }
                else {
                    AppendEscapingUnicodeAndSpecialChars(body, c);
                }
            }
            return (colors, body);
        }

        public static Color ParseDgColorName(string colorText, Color previousColor)
        {
            if (colorText == "PREV") {
                return previousColor;
            }
            else {
                return Colors.ParseColor(colorText);
            }
        }

        public static (string Text, char? ClosingChar) ParseDgPipeTag(string text, ref int parseIndex)
        {
            var tag = new StringBuilder(12);
            char? closingChar = null;
            while (true)
            {
                ++parseIndex;
                bool isEndOfText = parseIndex >= (text?.Length ?? 0);
                if (isEndOfText) {
                    break;
                }
                char c = text[parseIndex];
                bool isEndOfTag = c == ' ' || c == '|';
                if (isEndOfTag) {
                    closingChar = c;
                    break;
                }
                tag.Append(c);
            };
            return (tag.ToString(), closingChar);
        }

        public static void SaveLog(string logFilePath, string netLog)
        {
            DuckFile.CreatePath(logFilePath);
            var rtfText = CreateRtfFromDgLogs(netLog);
            File.WriteAllText(logFilePath, rtfText, Encoding.ASCII);
        }
    }
}
