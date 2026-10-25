using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DuckGame
{
    /// <summary>
    /// A small markdown renderer built on the chat's text layout. It covers what release notes use: # headers, - * + and 1. lists
    /// (nested by indentation), **bold**, *italic*, ~~strike~~, `code`, ``` code blocks, > quotes, --- rules, links and tables.
    /// Inline markdown is turned into the chat's |TAG| formatting. Anything it doesn't recognize shows as plain text.
    /// </summary>
    public sealed class MarkdownView
    {
        private enum BlockKind
        {
            Paragraph,
            Heading,
            ListItem,
            Code,
            Quote,
            Rule,
            TableRow,
        }

        private sealed class Block
        {
            public BlockKind kind;
            public string text = "";
            public int level = -1;
            /// <summary>"1." for numbered list items, null for bullets.</summary>
            public string marker;
            public bool tableHeader;
            /// <summary>Starts right under the previous block, with no paragraph gap (list items, table rows...).</summary>
            public bool tight;

            public ChatTextLayout layout;
            public ChatTextLayout clip;
            public float x;
            public float y;
            public float height;
        }

        private readonly List<Block> _blocks = new List<Block>();
        private FancyBitmapFont _font;
        private FancyBitmapFont _bigFont;
        private float _width = -1f;
        private string _source;

        public static readonly Color HeadingColor = Colors.DGPink;
        /// <summary>[Name] tags (who made a change) are drawn in this color so long lists are easier to read.</summary>
        private const string NameTagColor = "|130,130,145|";
        private static readonly Color QuoteColor = new Color(170, 170, 190);

        public float height { get; private set; }

        public float lineHeight => _font != null ? (_font.characterHeight + 2) * _font.scale.y : 4f;

        public void SetText(string markdown)
        {
            if (markdown == _source)
                return;
            _source = markdown;
            _width = -1f;
            _blocks.Clear();
            Parse(markdown ?? "");
        }

        #region Parsing

        private static readonly Regex s_heading = new Regex(@"^ {0,3}(#{1,6})[ \t]+(.*?)[ \t#]*$");
        private static readonly Regex s_rule = new Regex(@"^ {0,3}([-*_])([ \t]*\1){2,}[ \t]*$");
        private static readonly Regex s_bullet = new Regex(@"^( *)([-*+])[ \t]+(.*)$");
        private static readonly Regex s_numbered = new Regex(@"^( *)(\d{1,3})[.)][ \t]+(.*)$");
        private static readonly Regex s_fence = new Regex(@"^ *(```+|~~~+)");
        private static readonly Regex s_quote = new Regex(@"^ *>[ \t]?(.*)$");
        private static readonly Regex s_tableSeparator = new Regex(@"^ *\|?[ \t]*:?-+:?[ \t]*(\|[ \t]*:?-+:?[ \t]*)*\|?[ \t]*$");

        private void Parse(string markdown)
        {
            string[] lines = markdown.Replace("\r\n", "\n").Replace('\r', '\n').Replace("\t", "    ").Split('\n');
            List<int> listIndents = new List<int>();
            Block open = null;
            bool blankBefore = false;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line.Trim().Length == 0)
                {
                    open = null;
                    blankBefore = true;
                    continue;
                }
                int indent = line.Length - line.TrimStart(' ').Length;

                Match fence = s_fence.Match(line);
                if (fence.Success)
                {
                    string closing = fence.Groups[1].Value;
                    StringBuilder code = new StringBuilder();
                    for (i++; i < lines.Length; i++)
                    {
                        if (lines[i].TrimStart(' ').StartsWith(closing))
                            break;
                        if (code.Length > 0)
                            code.Append('\n');
                        // Code blocks are indented to line up with the list item they're in
                        code.Append(StripIndent(lines[i], indent));
                    }
                    _blocks.Add(new Block { kind = BlockKind.Code, text = code.ToString(), level = ListLevelFor(listIndents, indent) });
                    open = null;
                    blankBefore = false;
                    continue;
                }

                Match match = s_heading.Match(line);
                if (match.Success)
                {
                    listIndents.Clear();
                    _blocks.Add(new Block { kind = BlockKind.Heading, text = match.Groups[2].Value, level = match.Groups[1].Value.Length });
                    open = null;
                    blankBefore = false;
                    continue;
                }

                if (s_rule.IsMatch(line))
                {
                    listIndents.Clear();
                    _blocks.Add(new Block { kind = BlockKind.Rule });
                    open = null;
                    blankBefore = false;
                    continue;
                }

                Match bullet = s_bullet.Match(line);
                Match numbered = bullet.Success ? null : s_numbered.Match(line);
                if (bullet.Success || numbered.Success)
                {
                    Match item = bullet.Success ? bullet : numbered;
                    int level = ListLevel(listIndents, item.Groups[1].Value.Length);
                    open = new Block
                    {
                        kind = BlockKind.ListItem,
                        text = item.Groups[3].Value,
                        level = level,
                        marker = numbered != null ? numbered.Groups[2].Value + "." : null,
                        tight = !blankBefore && _blocks.Count > 0 && _blocks[_blocks.Count - 1].kind == BlockKind.ListItem,
                    };
                    _blocks.Add(open);
                    blankBefore = false;
                    continue;
                }

                Match quote = s_quote.Match(line);
                if (quote.Success)
                {
                    listIndents.Clear();
                    if (open != null && open.kind == BlockKind.Quote)
                        AppendLine(open, quote.Groups[1].Value);
                    else
                    {
                        open = new Block { kind = BlockKind.Quote, text = quote.Groups[1].Value };
                        _blocks.Add(open);
                    }
                    blankBefore = false;
                    continue;
                }

                string trimmed = line.Trim();
                if (trimmed.StartsWith("|") && trimmed.Length > 1)
                {
                    listIndents.Clear();
                    if (s_tableSeparator.IsMatch(trimmed))
                    {
                        // The row above it was the header
                        if (_blocks.Count > 0 && _blocks[_blocks.Count - 1].kind == BlockKind.TableRow)
                            _blocks[_blocks.Count - 1].tableHeader = true;
                    }
                    else
                    {
                        bool continues = open != null && open.kind == BlockKind.TableRow;
                        open = new Block { kind = BlockKind.TableRow, text = trimmed, tight = continues };
                        _blocks.Add(open);
                    }
                    blankBefore = false;
                    continue;
                }

                // Plain text: continues the paragraph or list item above it, otherwise starts a new paragraph
                if (open != null && (open.kind == BlockKind.Paragraph || open.kind == BlockKind.ListItem))
                    AppendLine(open, line);
                else
                {
                    int level = -1;
                    if (listIndents.Count > 0 && indent > 0)
                        level = ListLevelFor(listIndents, indent);
                    else
                        listIndents.Clear();
                    open = new Block { kind = BlockKind.Paragraph, text = line.TrimStart(' '), level = level };
                    _blocks.Add(open);
                }
                blankBefore = false;
            }
        }

        private static string StripIndent(string line, int indent)
        {
            int strip = 0;
            while (strip < indent && strip < line.Length && line[strip] == ' ')
                strip++;
            return line.Substring(strip);
        }

        private static void AppendLine(Block block, string line)
        {
            // Two trailing spaces or a backslash are a line break in markdown, otherwise lines join like in a paragraph
            bool hardBreak = block.text.EndsWith("  ") || block.text.EndsWith("\\");
            string previous = block.text.TrimEnd(' ');
            if (previous.EndsWith("\\"))
                previous = previous.Substring(0, previous.Length - 1);
            block.text = previous + (hardBreak ? "\n" : " ") + line.TrimStart(' ');
        }

        /// <summary>
        /// Nesting comes from indentation (not the bullet character, changelogs mix * and - on the same level).
        /// </summary>
        private static int ListLevel(List<int> indents, int indent)
        {
            while (indents.Count > 0 && indent < indents[indents.Count - 1])
                indents.RemoveAt(indents.Count - 1);
            if (indents.Count == 0 || indent > indents[indents.Count - 1])
                indents.Add(indent);
            return indents.Count - 1;
        }

        /// <summary>Which list level indented text (a code block or paragraph inside a list item) belongs to, -1 if none.</summary>
        private static int ListLevelFor(List<int> indents, int indent)
        {
            int level = -1;
            for (int i = 0; i < indents.Count; i++)
            {
                if (indent > indents[i])
                    level = i;
            }
            return level;
        }

        #endregion

        #region Inline markdown to chat tags

        private const string EscapablePunctuation = "\\`*_{}[]()#+-.!~>|";

        /// <summary>Turns inline markdown into the chat's formatting tags (|B|, |I|, |S|, |C|).</summary>
        public static string ConvertInline(string text)
        {
            StringBuilder builder = new StringBuilder(text.Length + 16);
            ConvertInline(text, builder);
            return builder.ToString();
        }

        private static void ConvertInline(string text, StringBuilder builder)
        {
            for (int i = 0; i < text.Length;)
            {
                char c = text[i];

                if (c == '\\' && i + 1 < text.Length && EscapablePunctuation.IndexOf(text[i + 1]) >= 0)
                {
                    builder.Append(text[i + 1]);
                    i += 2;
                    continue;
                }

                if (c == '`')
                {
                    int run = CountRun(text, i, '`');
                    int close = FindRun(text, i + run, '`', run);
                    if (close >= 0)
                    {
                        string code = text.Substring(i + run, close - i - run);
                        if (code.Length > 2 && code[0] == ' ' && code[code.Length - 1] == ' ')
                            code = code.Substring(1, code.Length - 2);
                        builder.Append("|C|").Append(code.Replace("|C|", "| C|")).Append("|C|");
                        i = close + run;
                        continue;
                    }
                    builder.Append('`', run);
                    i += run;
                    continue;
                }

                if ((c == '*' || c == '_' || c == '~') && TryEmphasis(text, i, builder, out int next))
                {
                    i = next;
                    continue;
                }

                if (c == '[' && TryLink(text, i, builder, out next))
                {
                    i = next;
                    continue;
                }

                if (c == '<')
                {
                    int close = text.IndexOf('>', i + 1);
                    if (close > i)
                    {
                        string inner = text.Substring(i + 1, close - i - 1);
                        // <https://...> autolinks
                        if (ChatTextLayoutBuilder.MatchUrl(inner, 0) == inner.Length)
                        {
                            builder.Append(inner);
                            i = close + 1;
                            continue;
                        }
                        if (Regex.IsMatch(inner, @"^/?(br|b|i|u|s|em|strong|sub|sup|kbd|details|summary|p)\b[^<]*$", RegexOptions.IgnoreCase))
                        {
                            if (inner.StartsWith("br", StringComparison.OrdinalIgnoreCase))
                                builder.Append('\n');
                            i = close + 1;
                            continue;
                        }
                    }
                }

                builder.Append(c);
                i++;
            }
        }

        private static int CountRun(string text, int index, char c)
        {
            int end = index;
            while (end < text.Length && text[end] == c)
                end++;
            return end - index;
        }

        private static int FindRun(string text, int from, char c, int length)
        {
            for (int i = from; i < text.Length; i++)
            {
                if (text[i] != c)
                    continue;
                int run = CountRun(text, i, c);
                if (run == length)
                    return i;
                i += run - 1;
            }
            return -1;
        }

        private static bool IsWordChar(char c) => char.IsLetterOrDigit(c);

        private static bool TryEmphasis(string text, int index, StringBuilder builder, out int next)
        {
            next = index;
            char c = text[index];
            int run = CountRun(text, index, c);
            int length;
            string tag;
            if (c == '~')
            {
                if (run != 2)
                    return false;
                length = 2;
                tag = "|S|";
            }
            else if (run >= 3)
            {
                length = 3;
                tag = "|B||I|";
            }
            else if (run == 2)
            {
                length = 2;
                tag = "|B|";
            }
            else
            {
                length = 1;
                tag = "|I|";
            }

            int start = index + length;
            if (start >= text.Length || char.IsWhiteSpace(text[start]))
                return false;
            // snake_case and the like aren't emphasis
            if (c == '_' && index > 0 && IsWordChar(text[index - 1]))
                return false;

            for (int i = start + 1; i <= text.Length - length; i++)
            {
                if (text[i] == '`')
                {
                    // Don't close inside inline code
                    int codeRun = CountRun(text, i, '`');
                    int codeClose = FindRun(text, i + codeRun, '`', codeRun);
                    if (codeClose >= 0)
                        i = codeClose + codeRun - 1;
                    continue;
                }
                if (text[i] != c)
                    continue;
                int closeRun = CountRun(text, i, c);
                if (char.IsWhiteSpace(text[i - 1]))
                {
                    i += closeRun - 1;
                    continue;
                }
                bool matches = length == 1 ? closeRun == 1 || closeRun == 3 : closeRun >= length;
                if (!matches)
                {
                    i += closeRun - 1;
                    continue;
                }
                int end = i + length;
                if (c == '_' && end < text.Length && IsWordChar(text[end]))
                    continue;
                builder.Append(tag);
                ConvertInline(text.Substring(start, i - start), builder);
                builder.Append(length == 3 ? "|I||B|" : tag);
                next = end;
                return true;
            }
            return false;
        }

        private static bool TryLink(string text, int index, StringBuilder builder, out int next)
        {
            next = index;
            int depth = 0;
            int close = -1;
            for (int i = index; i < text.Length; i++)
            {
                if (text[i] == '[')
                    depth++;
                else if (text[i] == ']' && --depth == 0)
                {
                    close = i;
                    break;
                }
            }
            if (close < 0)
                return false;
            if (close + 1 >= text.Length || text[close + 1] != '(')
            {
                // A [Name] tag, dimmed. |PREV| goes back to whatever color came before it
                if (close - index - 1 < 1 || close - index - 1 > 40)
                    return false;
                builder.Append(NameTagColor).Append('[');
                ConvertInline(text.Substring(index + 1, close - index - 1), builder);
                builder.Append("]|PREV|");
                next = close + 1;
                return true;
            }
            int urlEnd = text.IndexOf(')', close + 2);
            if (urlEnd < 0)
                return false;
            string label = text.Substring(index + 1, close - index - 1);
            string url = text.Substring(close + 2, urlEnd - close - 2).Trim();
            int space = url.IndexOf(' ');
            if (space > 0)
                url = url.Substring(0, space);
            ConvertInline(label, builder);
            // Only raw addresses are clickable in the chat layout, so the address follows the text
            if (ChatTextLayoutBuilder.MatchUrl(url, 0) == url.Length && label.Trim() != url)
                builder.Append(" (").Append(url).Append(')');
            next = urlEnd + 1;
            return true;
        }

        #endregion

        #region Layout

        private float indentStep => 6f * _font.scale.x * 2f;

        /// <summary>Lays the blocks out for this width (in the font's units). Does nothing if it's already laid out for it.</summary>
        public void Layout(float width)
        {
            if (_font == null)
            {
                _font = new FancyBitmapFont("smallFontChat");
                _bigFont = new FancyBitmapFont("smallFontChat");
            }
            _font.scale = new Vec2(0.5f);
            _bigFont.scale = new Vec2(1f);
            if (Math.Abs(width - _width) < 0.01f)
                return;
            _width = width;

            float lh = lineHeight;
            float gap = lh * 0.6f;
            float y = 0f;
            Block previous = null;
            foreach (Block block in _blocks)
            {
                float top;
                if (previous == null)
                    top = 0f;
                else if (block.kind == BlockKind.Heading)
                    top = block.level <= 1 ? lh * 1.5f : lh;
                else if (block.tight)
                    top = Math.Max(0.5f, _font.scale.y);
                else if (previous.kind == BlockKind.Heading)
                    top = lh * 0.35f;
                else if (block.kind == BlockKind.ListItem && previous.kind == BlockKind.ListItem || block.level >= 0 && previous.kind == BlockKind.ListItem)
                    top = lh * 0.3f;
                else
                    top = gap;
                y += top;

                float indent = block.level >= 0 ? (block.level + 1) * indentStep : 0f;
                block.x = indent;
                block.y = y;
                switch (block.kind)
                {
                    case BlockKind.Heading:
                    {
                        bool big = block.level <= 1;
                        ChatTextLayoutBuilder builder = new ChatTextLayoutBuilder(big ? _bigFont : _font, width);
                        if (block.level <= 2)
                            builder.SetColor(HeadingColor);
                        builder.AppendFormatted((big ? "" : "|B|") + ConvertInline(block.text.TrimEnd()));
                        block.x = 0f;
                        block.layout = builder.Build();
                        break;
                    }
                    case BlockKind.Rule:
                        block.layout = null;
                        block.height = lh * 0.5f;
                        break;
                    case BlockKind.Code:
                    {
                        ChatTextLayoutBuilder builder = new ChatTextLayoutBuilder(_font, width - indent);
                        builder.AppendFormatted("|CB|" + block.text.Replace("|CB|", "| CB|") + "|CB|");
                        block.layout = builder.Build();
                        break;
                    }
                    case BlockKind.Quote:
                    {
                        block.x = indentStep * 0.6f;
                        ChatTextLayoutBuilder builder = new ChatTextLayoutBuilder(_font, width - block.x);
                        builder.SetColor(QuoteColor);
                        builder.AppendFormatted(ConvertInline(block.text.TrimEnd()));
                        block.layout = builder.Build();
                        break;
                    }
                    case BlockKind.TableRow:
                    {
                        ChatTextLayoutBuilder builder = new ChatTextLayoutBuilder(_font, width);
                        builder.AppendFormatted((block.tableHeader ? "|B|" : "") + ConvertInline(TableRowText(block.text)));
                        block.layout = builder.Build();
                        break;
                    }
                    default:
                    {
                        ChatTextLayoutBuilder builder = new ChatTextLayoutBuilder(_font, Math.Max(width - indent, 40f));
                        builder.AppendFormatted(ConvertInline(block.text.TrimEnd()));
                        block.layout = builder.Build();
                        break;
                    }
                }
                if (block.layout != null)
                {
                    block.height = block.layout.height;
                    block.clip = new ChatTextLayout
                    {
                        font = block.layout.font,
                        lineHeight = block.layout.lineHeight,
                        charHeight = block.layout.charHeight,
                        codePadding = block.layout.codePadding,
                        width = block.layout.width,
                        height = block.layout.height,
                    };
                    block.clip.glyphs.AddRange(block.layout.glyphs);
                }
                y += block.height;
                previous = block;
            }
            height = y;
        }

        /// <summary>"| a | b |" becomes "a    b", cells spaced apart.</summary>
        private static string TableRowText(string row)
        {
            row = row.Trim();
            if (row.StartsWith("|"))
                row = row.Substring(1);
            if (row.EndsWith("|") && !row.EndsWith("\\|"))
                row = row.Substring(0, row.Length - 1);
            List<string> cells = new List<string>();
            StringBuilder cell = new StringBuilder();
            bool inCode = false;
            for (int i = 0; i < row.Length; i++)
            {
                char c = row[i];
                if (c == '\\' && i + 1 < row.Length && row[i + 1] == '|')
                {
                    cell.Append("\\|");
                    i++;
                    continue;
                }
                if (c == '`')
                    inCode = !inCode;
                if (c == '|' && !inCode)
                {
                    cells.Add(cell.ToString().Trim());
                    cell.Clear();
                    continue;
                }
                cell.Append(c);
            }
            cells.Add(cell.ToString().Trim());
            return string.Join("    ", cells);
        }

        #endregion

        #region Drawing

        /// <summary>
        /// Draws the part between <paramref name="scroll"/> and <paramref name="scroll"/> + <paramref name="viewHeight"/>
        /// at <paramref name="origin"/>. Lines that would stick out of the view are left out.
        /// </summary>
        public void Draw(Vec2 origin, float scroll, float viewHeight, float alpha, float depth, ChatLink hoveredLink)
        {
            float top = scroll;
            float bottom = scroll + viewHeight;
            foreach (Block block in _blocks)
            {
                if (block.y + block.height < top || block.y > bottom)
                    continue;
                Vec2 position = origin + new Vec2(block.x, block.y - scroll);

                if (block.kind == BlockKind.Rule)
                {
                    float ry = (float)Math.Round(position.y + block.height / 2f);
                    if (ry >= origin.y && ry <= origin.y + viewHeight)
                        Graphics.DrawRect(new Vec2(origin.x, ry), new Vec2(origin.x + _width, ry + _font.scale.y), Color.White * (0.35f * alpha), ChatBox.D(depth));
                    continue;
                }

                ChatTextLayout layout = block.layout;
                bool partial = block.y < top || block.y + block.height > bottom;
                if (partial)
                {
                    layout = block.clip;
                    layout.lines.Clear();
                    layout.blocks.Clear();
                    float visibleTop = top - block.y;
                    float visibleBottom = bottom - block.y;
                    foreach (ChatLine line in block.layout.lines)
                    {
                        if (line.y >= visibleTop && line.y + line.height <= visibleBottom)
                            layout.lines.Add(line);
                    }
                    foreach (ChatCodeBlock codeBlock in block.layout.blocks)
                    {
                        float from = Math.Max(codeBlock.rect.y, visibleTop);
                        float to = Math.Min(codeBlock.rect.Bottom, visibleBottom);
                        if (to > from)
                            layout.blocks.Add(new ChatCodeBlock { rect = new Rectangle(codeBlock.rect.x, from, codeBlock.rect.width, to - from), content = codeBlock.content });
                    }
                }
                layout.Draw(position, Color.White, alpha, depth, hoveredLink: hoveredLink);

                if (block.kind == BlockKind.ListItem && block.layout.lines.Count > 0)
                {
                    ChatLine first = block.layout.lines[0];
                    if (first.y >= top - block.y && first.y + first.height <= bottom - block.y)
                        DrawMarker(block, position, alpha, depth);
                }
                else if (block.kind == BlockKind.Quote)
                {
                    float from = Math.Max(position.y, origin.y);
                    float to = Math.Min(position.y + block.height, origin.y + viewHeight);
                    if (to > from)
                        Graphics.DrawRect(new Vec2(origin.x + 1f, from), new Vec2(origin.x + 1f + _font.scale.x * 2f, to), QuoteColor * (0.6f * alpha), ChatBox.D(depth));
                }
            }
        }

        private void DrawMarker(Block block, Vec2 position, float alpha, float depth)
        {
            float charHeight = _font.characterHeight * _font.scale.y;
            if (block.marker != null)
            {
                float markerWidth = _font.GetWidth(block.marker);
                _font.Draw(block.marker, position.x - markerWidth - _font.scale.x * 3f, position.y, Color.White * alpha, ChatBox.D(depth));
                return;
            }
            // Filled square, then hollow, then a small square, like browsers do
            float size = Math.Max(1f, (float)Math.Round(charHeight * 0.4f / _font.scale.y) * _font.scale.y);
            float cx = position.x - indentStep * 0.5f;
            float cy = position.y + charHeight / 2f;
            Vec2 tl = new Vec2((float)Math.Round(cx - size / 2f, 1), (float)Math.Round(cy - size / 2f, 1));
            Vec2 br = tl + new Vec2(size, size);
            Color color = HeadingColor * alpha;
            Depth d = ChatBox.D(depth);
            switch (block.level % 3)
            {
                case 0:
                    Graphics.DrawRect(tl, br, color, d);
                    break;
                case 1:
                    Graphics.DrawRect(tl, br, color, d, false, _font.scale.x);
                    break;
                default:
                    Vec2 inset = new Vec2(size * 0.25f, size * 0.25f);
                    Graphics.DrawRect(tl + inset, br - inset, Color.White * (0.7f * alpha), d);
                    break;
            }
        }

        /// <summary>The link under <paramref name="local"/> (relative to the view's top left, scroll included), or null.</summary>
        public ChatLink GetLinkAt(Vec2 local)
        {
            foreach (Block block in _blocks)
            {
                if (block.layout == null || local.y < block.y || local.y > block.y + block.height)
                    continue;
                return block.layout.GetLinkAt(local - new Vec2(block.x, block.y));
            }
            return null;
        }

        #endregion
    }
}
