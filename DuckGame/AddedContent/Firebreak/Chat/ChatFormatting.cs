using System;
using System.Collections.Generic;
using System.Text;

namespace DuckGame
{
    [Flags]
    public enum ChatStyle : byte
    {
        None = 0,
        Bold = 1 << 0,
        Italic = 1 << 1,
        Underline = 1 << 2,
        Strikethrough = 1 << 3,
        Code = 1 << 4,
        CodeBlock = 1 << 5,
    }

    /// <summary>
    /// Chat formatting tags, written in the same |TAG| style as Duck Game's color tags.
    /// <para>
    /// |B| / |BOLD|, |I| / |ITALIC|, |U| / |UNDER| / |UNDERLINE| and |S| / |STRIKE| / |STRIKETHROUGH| toggle their style,
    /// |C| / |CODE| / |MONO| / |MONOSPACE| toggles inline code, |CB| / |CODEBLOCK| toggles a code block,
    /// and |R| / |RESET| goes back to the default style and color. Everything inside code is shown as-is.
    /// </para>
    /// </summary>
    public static class ChatFormatting
    {
        public enum TagKind
        {
            Style,
            Code,
            CodeBlock,
            Reset,
        }

        public readonly struct Tag
        {
            public readonly TagKind kind;
            public readonly ChatStyle style;

            public Tag(TagKind kind, ChatStyle style)
            {
                this.kind = kind;
                this.style = style;
            }
        }

        private const int MaxTagLength = 16;

        private static readonly Dictionary<string, Tag> s_tags = new Dictionary<string, Tag>(StringComparer.Ordinal)
        {
            { "ITALIC", new Tag(TagKind.Style, ChatStyle.Italic) },
            { "I", new Tag(TagKind.Style, ChatStyle.Italic) },
            { "BOLD", new Tag(TagKind.Style, ChatStyle.Bold) },
            { "B", new Tag(TagKind.Style, ChatStyle.Bold) },
            { "UNDERLINE", new Tag(TagKind.Style, ChatStyle.Underline) },
            { "UNDER", new Tag(TagKind.Style, ChatStyle.Underline) },
            { "U", new Tag(TagKind.Style, ChatStyle.Underline) },
            { "STRIKETHROUGH", new Tag(TagKind.Style, ChatStyle.Strikethrough) },
            { "STRIKE", new Tag(TagKind.Style, ChatStyle.Strikethrough) },
            { "S", new Tag(TagKind.Style, ChatStyle.Strikethrough) },
            { "CODE", new Tag(TagKind.Code, ChatStyle.Code) },
            { "MONO", new Tag(TagKind.Code, ChatStyle.Code) },
            { "MONOSPACE", new Tag(TagKind.Code, ChatStyle.Code) },
            { "C", new Tag(TagKind.Code, ChatStyle.Code) },
            { "CODEBLOCK", new Tag(TagKind.CodeBlock, ChatStyle.CodeBlock) },
            { "CB", new Tag(TagKind.CodeBlock, ChatStyle.CodeBlock) },
            { "RESET", new Tag(TagKind.Reset, ChatStyle.None) },
            { "R", new Tag(TagKind.Reset, ChatStyle.None) },
        };

        private static bool TryGetTagName(string text, int index, out string name, out int end)
        {
            name = null;
            end = index;
            if (index >= text.Length || text[index] != '|')
                return false;
            int limit = Math.Min(text.Length, index + MaxTagLength + 2);
            for (int i = index + 1; i < limit; i++)
            {
                char c = text[i];
                if (c == '|')
                {
                    if (i == index + 1)
                        return false;
                    name = text.Substring(index + 1, i - index - 1);
                    end = i + 1;
                    return true;
                }
                if (c == ' ' || c == '\n')
                    return false;
            }
            return false;
        }

        /// <summary>
        /// Checks for one of the formatting tags at <paramref name="index"/> (which should point at a '|').
        /// </summary>
        /// <param name="end">Index of the first character after the tag.</param>
        public static bool TryMatchTag(string text, int index, out Tag tag, out int end)
        {
            tag = default;
            if (!TryGetTagName(text, index, out string name, out end))
                return false;
            return s_tags.TryGetValue(name, out tag);
        }

        /// <summary>
        /// Checks for a vanilla color tag (|RED|, |255,0,0|, |PREV|...) at <paramref name="index"/>.
        /// </summary>
        public static bool TryMatchColorTag(string text, int index, out Color color, out bool isPrevious, out int end)
        {
            color = Color.White;
            isPrevious = false;
            if (!TryGetTagName(text, index, out string name, out end))
                return false;
            if (name == "PREV")
            {
                isPrevious = true;
                return true;
            }
            color = Colors.ParseColor(name);
            return color != Colors.Transparent;
        }

        /// <summary>
        /// True if <paramref name="index"/> is the start of any tag that changes how text looks (formatting or color).
        /// </summary>
        public static bool IsMarkupAt(string text, int index, out int end)
        {
            if (TryMatchTag(text, index, out _, out end))
                return true;
            return TryMatchColorTag(text, index, out _, out _, out end);
        }

        public static bool ContainsFormatting(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;
            for (int i = text.IndexOf('|'); i >= 0 && i < text.Length; i = text.IndexOf('|', i + 1))
            {
                if (TryMatchTag(text, i, out _, out _))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the DGR-only formatting tags so the message reads naturally on clients that can't render them.
        /// Vanilla color tags are kept since every client understands those.
        /// </summary>
        public static string StripForVanilla(string text)
        {
            if (!ContainsFormatting(text))
                return text;

            StringBuilder builder = new StringBuilder(text.Length);
            TagKind? openCode = null;
            bool usedColor = false;
            for (int i = 0; i < text.Length;)
            {
                char c = text[i];
                if (c == '|')
                {
                    if (TryMatchTag(text, i, out Tag tag, out int end))
                    {
                        if (openCode.HasValue)
                        {
                            if (tag.kind == openCode.Value)
                            {
                                openCode = null;
                                i = end;
                                continue;
                            }
                        }
                        else
                        {
                            switch (tag.kind)
                            {
                                case TagKind.Code:
                                case TagKind.CodeBlock:
                                    openCode = tag.kind;
                                    break;
                                case TagKind.Reset:
                                    if (usedColor)
                                    {
                                        builder.Append("|BLACK|");
                                        usedColor = false;
                                    }
                                    break;
                            }
                            i = end;
                            continue;
                        }
                    }
                    else if (!openCode.HasValue && TryMatchColorTag(text, i, out _, out _, out int colorEnd))
                    {
                        usedColor = true;
                        builder.Append(text, i, colorEnd - i);
                        i = colorEnd;
                        continue;
                    }
                }
                builder.Append(c);
                i++;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Removes every tag (formatting and color), used for text-to-speech and other plain text uses.
        /// </summary>
        public static string StripAllTags(string text)
        {
            if (string.IsNullOrEmpty(text) || text.IndexOf('|') < 0)
                return text;
            StringBuilder builder = new StringBuilder(text.Length);
            for (int i = 0; i < text.Length;)
            {
                if (text[i] == '|' && IsMarkupAt(text, i, out int end))
                {
                    i = end;
                    continue;
                }
                builder.Append(text[i]);
                i++;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Whether this profile's client renders formatting tags itself (in which case it gets the original text).
        /// Uses the regular DGR flag; the version check keeps older DGR releases, which predate formatting, on the
        /// stripped text.
        /// </summary>
        public static bool SupportsFormatting(Profile profile)
        {
            return profile != null && profile.isUsingRebuilt && profile.inSameRebuiltVersion;
        }
    }
}
