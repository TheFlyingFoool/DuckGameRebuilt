using System;
using System.Collections.Generic;
using System.Text;

namespace DuckGame
{
    public enum ChatGlyphKind : byte
    {
        Char,
        Sprite,
        Image,
        Break,
    }

    /// <summary>
    /// A clickable http(s) link in a chat message. Every glyph of the link points at the same instance.
    /// </summary>
    public sealed class ChatLink
    {
        public string url;
    }

    /// <summary>
    /// A sprite (moji / button icon) found in typed text, see <see cref="ChatTextLayoutBuilder.FindSprites"/>.
    /// </summary>
    public readonly struct ChatSpriteSpan
    {
        public readonly int start;
        public readonly int end;
        public readonly Sprite sprite;

        public ChatSpriteSpan(int start, int end, Sprite sprite)
        {
            this.start = start;
            this.end = end;
            this.sprite = sprite;
        }
    }

    /// <summary>
    /// One positioned piece of a laid out chat message: a character, an inline sprite (moji / button icon), an image or a line break.
    /// </summary>
    public sealed class ChatGlyph
    {
        public ChatGlyphKind kind;
        public char ch;
        public Sprite sprite;
        public ChatImageAttachment image;
        /// <summary>What gets put on the clipboard for this glyph, null means the character itself.</summary>
        public string copyText;
        public ChatStyle style;
        public bool hasColor;
        public Color color;
        /// <summary>Drawn faded, used for markup in the input box.</summary>
        public bool dim;
        /// <summary>Drawn white on a dark tag, used for the "+123" marker on cut off messages.</summary>
        public bool chip;
        /// <summary>Index in the source string, only set for literal text (the input box).</summary>
        public int sourceIndex = -1;
        /// <summary>For a sprite in the input box, the index after the last source character it stands for.</summary>
        public int sourceEnd = -1;
        /// <summary>The link this glyph is part of, if any.</summary>
        public ChatLink link;
        /// <summary>Code block this glyph belongs to, -1 if none.</summary>
        public int block = -1;
        public float x;
        public float y;
        public float w;
        public float h;
        public float drawOffset;
        /// <summary>Horizontal squish, so wide characters fit in a monospace cell.</summary>
        public float xScale = 1f;
        public int line;

        public bool isSpace => kind == ChatGlyphKind.Char && ch == ' ' && !chip;

        public string copyString
        {
            get
            {
                if (copyText != null)
                    return copyText;
                switch (kind)
                {
                    case ChatGlyphKind.Char:
                        return ch.ToString();
                    case ChatGlyphKind.Break:
                        return "\n";
                    case ChatGlyphKind.Image:
                        return "[image]";
                    default:
                        return "";
                }
            }
        }
    }

    public sealed class ChatLine
    {
        public int start;
        public int end;
        public float y;
        public float height;
        public float width;
        public int block = -1;
    }

    public sealed class ChatCodeBlock
    {
        public Rectangle rect;
        public string content = "";
    }

    /// <summary>
    /// Word wrapped, formatted text for the chat. Built with <see cref="ChatTextLayoutBuilder"/>, then drawn and hit tested.
    /// All positions are relative to the layout's top left corner.
    /// </summary>
    public sealed class ChatTextLayout
    {
        public const float ItalicShear = 0.22f;

        public readonly List<ChatGlyph> glyphs = new List<ChatGlyph>();
        public readonly List<ChatLine> lines = new List<ChatLine>();
        public readonly List<ChatCodeBlock> blocks = new List<ChatCodeBlock>();
        public FancyBitmapFont font;
        public float width;
        public float height;
        public float lineHeight;
        public float charHeight;
        public float codePadding;

        #region Hit testing

        private int LineEndForCaret(ChatLine line)
        {
            if (line.end > line.start && glyphs[line.end - 1].kind == ChatGlyphKind.Break)
                return line.end - 1;
            return line.end;
        }

        public ChatLine GetLineAt(float y)
        {
            if (lines.Count == 0)
                return null;
            foreach (ChatLine line in lines)
            {
                if (y < line.y + line.height)
                    return line;
            }
            return lines[lines.Count - 1];
        }

        /// <summary>
        /// The caret position (0 = before the first glyph, glyphs.Count = after the last) closest to a point.
        /// </summary>
        public int GetBoundaryAt(Vec2 local)
        {
            ChatLine line = GetLineAt(local.y);
            if (line == null)
                return 0;
            int end = LineEndForCaret(line);
            for (int i = line.start; i < end; i++)
            {
                ChatGlyph glyph = glyphs[i];
                if (local.x < glyph.x + glyph.w * 0.5f)
                    return i;
            }
            return end;
        }

        /// <summary>
        /// The glyph directly under a point, or -1.
        /// </summary>
        public int GetGlyphAt(Vec2 local)
        {
            if (local.y < 0f || local.y > height)
                return -1;
            foreach (ChatLine line in lines)
            {
                if (local.y < line.y || local.y >= line.y + line.height)
                    continue;
                for (int i = line.start; i < line.end; i++)
                {
                    ChatGlyph glyph = glyphs[i];
                    if (glyph.kind == ChatGlyphKind.Break)
                        continue;
                    float glyphHeight = glyph.kind == ChatGlyphKind.Image ? glyph.h : line.height;
                    if (local.x >= glyph.x && local.x < glyph.x + glyph.w && local.y < glyph.y + glyphHeight)
                        return i;
                }
            }
            return -1;
        }

        /// <summary>The link under a point, or null.</summary>
        public ChatLink GetLinkAt(Vec2 local)
        {
            int index = GetGlyphAt(local);
            return index >= 0 ? glyphs[index].link : null;
        }

        public ChatCodeBlock GetBlockAt(Vec2 local)
        {
            foreach (ChatCodeBlock block in blocks)
            {
                if (block.rect.Contains(local))
                    return block;
            }
            return null;
        }

        /// <summary>
        /// Top left corner of where a caret at <paramref name="boundary"/> should be drawn.
        /// </summary>
        public Vec2 GetCaretPosition(int boundary)
        {
            if (lines.Count == 0)
                return Vec2.Zero;
            foreach (ChatLine line in lines)
            {
                if (boundary >= line.start && boundary < line.end)
                {
                    ChatGlyph glyph = glyphs[boundary];
                    return new Vec2(glyph.x, line.y);
                }
            }
            ChatLine last = lines[lines.Count - 1];
            if (last.start >= glyphs.Count || last.end == last.start)
                return new Vec2(last.block >= 0 ? codePadding : 0f, last.y);
            ChatGlyph lastGlyph = glyphs[Math.Min(boundary, glyphs.Count) - 1];
            return new Vec2(lastGlyph.x + lastGlyph.w, last.y);
        }

        public string GetCopyText(int from, int to)
        {
            if (from > to)
            {
                int swap = from;
                from = to;
                to = swap;
            }
            from = Math.Max(0, from);
            to = Math.Min(glyphs.Count, to);
            StringBuilder builder = new StringBuilder();
            for (int i = from; i < to; i++)
                builder.Append(glyphs[i].copyString);
            return builder.ToString();
        }

        /// <summary>
        /// Word boundaries around a glyph, for double click selection.
        /// </summary>
        public void GetWordAt(int index, out int from, out int to)
        {
            from = to = index;
            if (index < 0 || index >= glyphs.Count)
                return;
            bool IsWordGlyph(ChatGlyph g) => g.kind == ChatGlyphKind.Sprite || g.kind == ChatGlyphKind.Char && !char.IsWhiteSpace(g.ch) && !char.IsPunctuation(g.ch);
            if (!IsWordGlyph(glyphs[index]))
            {
                to = index + 1;
                return;
            }
            while (from > 0 && IsWordGlyph(glyphs[from - 1]) && glyphs[from - 1].line == glyphs[index].line)
                from--;
            to = index;
            while (to < glyphs.Count && IsWordGlyph(glyphs[to]) && glyphs[to].line == glyphs[index].line)
                to++;
        }

        #endregion

        #region Drawing

        /// <summary>
        /// Draws the layout. The font must have the same scale it had when the layout was built.
        /// </summary>
        /// <param name="selectionFrom">First selected glyph (inclusive), -1 for no selection.</param>
        /// <param name="selectionTo">Last selected glyph (exclusive).</param>
        public static readonly Color LinkColor = new Color(16, 72, 190);
        public static readonly Color LinkHoverColor = new Color(40, 120, 255);

        public void Draw(Vec2 origin, Color baseColor, float alpha, float textDepth, int selectionFrom = -1, int selectionTo = -1, ChatCodeBlock hoveredBlock = null, ChatLink hoveredLink = null)
        {
            if (selectionFrom > selectionTo)
            {
                int swap = selectionFrom;
                selectionFrom = selectionTo;
                selectionTo = swap;
            }

            float imageDepth = textDepth - 0.08f;
            float overlayDepth = textDepth - 0.03f;
            Depth codeDepth = ChatBox.D(textDepth - 0.1f);
            Depth selectionDepth = ChatBox.D(textDepth - 0.05f);
            Depth overlay = ChatBox.D(overlayDepth);
            Depth text = ChatBox.D(textDepth);
            Color codeBackground = Color.Black * (0.22f * alpha);

            foreach (ChatCodeBlock block in blocks)
            {
                Vec2 tl = origin + new Vec2(block.rect.x, block.rect.y);
                Graphics.DrawRect(tl, tl + new Vec2(block.rect.width, block.rect.height), codeBackground, codeDepth);
            }

            float thickness = font.decorationThickness;
            foreach (ChatLine line in lines)
            {
                float inlineCodeStart = -1f;
                float inlineCodeEnd = 0f;
                float chipStart = -1f;
                float chipEnd = 0f;
                for (int i = line.start; i < line.end; i++)
                {
                    ChatGlyph glyph = glyphs[i];
                    bool selected = i >= selectionFrom && i < selectionTo && selectionFrom >= 0;
                    float gx = origin.x + glyph.x;
                    float gy = origin.y + glyph.y;

                    if ((glyph.style & ChatStyle.Code) != 0 && glyph.block < 0)
                    {
                        if (inlineCodeStart < 0f)
                            inlineCodeStart = gx;
                        inlineCodeEnd = gx + glyph.w;
                    }
                    else if (inlineCodeStart >= 0f)
                    {
                        DrawInlineCodeBackground(inlineCodeStart, inlineCodeEnd, origin.y + line.y, codeBackground, codeDepth);
                        inlineCodeStart = -1f;
                    }

                    if (glyph.chip)
                    {
                        if (chipStart < 0f)
                            chipStart = gx;
                        chipEnd = gx + glyph.w;
                    }
                    else if (chipStart >= 0f)
                    {
                        DrawInlineCodeBackground(chipStart, chipEnd, origin.y + line.y, Color.Black * (0.7f * alpha), codeDepth);
                        chipStart = -1f;
                    }

                    if (selected && glyph.kind != ChatGlyphKind.Image)
                    {
                        float selectionWidth = glyph.kind == ChatGlyphKind.Break ? Math.Max(2f, charHeight * 0.3f) : glyph.w;
                        Graphics.DrawRect(new Vec2(gx, gy), new Vec2(gx + selectionWidth, gy + lineHeight), Color.Black * (0.75f * alpha), selectionDepth);
                    }

                    Color color = selected || glyph.chip ? Color.White
                        : glyph.link != null ? (glyph.link == hoveredLink ? LinkHoverColor : LinkColor)
                        : glyph.hasColor ? glyph.color : baseColor;
                    color *= alpha * (glyph.dim && !selected ? 0.55f : 1f);

                    switch (glyph.kind)
                    {
                        case ChatGlyphKind.Char:
                            if (glyph.ch != ' ')
                            {
                                font.DrawGlyph(glyph.ch, gx + glyph.drawOffset, gy, color, text,
                                    (glyph.style & ChatStyle.Italic) != 0 ? ItalicShear : 0f,
                                    (glyph.style & ChatStyle.Bold) != 0, glyph.xScale);
                            }
                            break;
                        case ChatGlyphKind.Sprite:
                            if (glyph.sprite != null)
                            {
                                float spriteAlpha = glyph.sprite.alpha;
                                glyph.sprite.alpha = alpha * (glyph.dim && !selected ? 0.55f : 1f);
                                // The sprite lives as long as the layout, don't let frame interpolation slide it around
                                glyph.sprite.SkipIntraTick = 1;
                                float yOffset = charHeight / 2f - glyph.sprite.height * glyph.sprite.scale.y / 2f;
                                Graphics.Draw(glyph.sprite, gx, gy + yOffset, text + 10);
                                glyph.sprite.alpha = spriteAlpha;
                            }
                            break;
                        case ChatGlyphKind.Image:
                            ChatImages.DrawAttachment(glyph, this, new Vec2(gx, gy), alpha, imageDepth, textDepth, baseColor);
                            if (selected)
                                Graphics.DrawRect(new Vec2(gx, gy), new Vec2(gx + glyph.w, gy + glyph.h), new Color(70, 130, 255) * (0.4f * alpha), overlay);
                            break;
                    }

                    if (glyph.kind == ChatGlyphKind.Char || glyph.kind == ChatGlyphKind.Sprite)
                    {
                        if ((glyph.style & ChatStyle.Underline) != 0 || glyph.link != null && glyph.kind == ChatGlyphKind.Char)
                        {
                            float uy = gy + charHeight;
                            Graphics.DrawRect(new Vec2(gx, uy), new Vec2(gx + glyph.w, uy + thickness), color, text);
                        }
                        if ((glyph.style & ChatStyle.Strikethrough) != 0)
                        {
                            float sy = gy + (float)Math.Round(charHeight * 0.45f / thickness) * thickness;
                            Graphics.DrawRect(new Vec2(gx, sy), new Vec2(gx + glyph.w, sy + thickness), color, text);
                        }
                    }
                }
                if (inlineCodeStart >= 0f)
                    DrawInlineCodeBackground(inlineCodeStart, inlineCodeEnd, origin.y + line.y, codeBackground, codeDepth);
                if (chipStart >= 0f)
                    DrawInlineCodeBackground(chipStart, chipEnd, origin.y + line.y, Color.Black * (0.7f * alpha), codeDepth);
            }

            if (hoveredBlock != null && blocks.Contains(hoveredBlock))
                ChatBox.DrawCodeBlockCopyButton(this, hoveredBlock, origin, alpha, overlayDepth, textDepth);
        }

        private void DrawInlineCodeBackground(float from, float to, float y, Color color, Depth depth)
        {
            Graphics.DrawRect(new Vec2(from - 1f, y), new Vec2(to + 1f, y + lineHeight), color, depth);
        }

        #endregion
    }

    /// <summary>
    /// Turns chat text into a <see cref="ChatTextLayout"/>. Text can be appended in formatted mode (tags are applied)
    /// or literal mode (every character shows up as-is, used for the input box).
    /// </summary>
    public sealed class ChatTextLayoutBuilder
    {
        private readonly FancyBitmapFont _font;
        private readonly float _maxWidth;
        private readonly List<ChatGlyph> _glyphs = new List<ChatGlyph>();
        private readonly float _lineHeight;
        private readonly float _charHeight;
        private readonly float _monoWidth;
        private readonly float _codePadding;
        private readonly float _codePaddingY;
        private int _blockCount;

        private ChatStyle _style;
        private bool _hasColor;
        private Color _color;
        private bool _previousHasColor;
        private Color _previousColor;

        public NetworkConnection mojiConnection;
        public bool trailingEmptyLine;

        // Collapsing long messages
        private int _visibleLimit = -1;
        private int _lineLimit;
        private int _visibleCount;
        private int _lineCount;
        private int _hiddenCount;
        private bool _truncated;

        /// <summary>True if the text since <see cref="BeginLimit"/> got cut off.</summary>
        public bool truncated => _truncated;

        /// <summary>
        /// Text appended with <see cref="AppendFormatted"/> after this is cut off after <paramref name="characters"/> visible
        /// characters or <paramref name="lines"/> lines, ending in "...". Call <see cref="EndLimit"/> afterwards.
        /// </summary>
        public void BeginLimit(int characters, int lines)
        {
            _visibleLimit = characters;
            _lineLimit = lines;
            _visibleCount = 0;
            _lineCount = 1;
            _hiddenCount = 0;
            _truncated = false;
        }

        public void EndLimit()
        {
            if (_truncated)
            {
                // "..." in the normal text color, then a dark tag saying how much is hidden
                _style = ChatStyle.None;
                _hasColor = false;
                ChatGlyph lastGlyph = last;
                if (lastGlyph != null && lastGlyph.kind == ChatGlyphKind.Char && lastGlyph.ch == ' ')
                    _glyphs.RemoveAt(_glyphs.Count - 1);
                foreach (char c in "...")
                    AddChar(c);
                foreach (char c in " +" + _hiddenCount + " ")
                {
                    ChatGlyph glyph = AddChar(c);
                    glyph.chip = true;
                    glyph.copyText = "";
                }
            }
            _visibleLimit = -1;
            _truncated = false;
        }

        private bool Allow(bool isBreak)
        {
            if (_visibleLimit < 0)
                return true;
            if (_truncated)
            {
                _hiddenCount++;
                return false;
            }
            if (isBreak ? ++_lineCount > _lineLimit : ++_visibleCount > _visibleLimit)
            {
                _truncated = true;
                _hiddenCount++;
                return false;
            }
            return true;
        }

        public float maxWidth => _maxWidth;
        public float lineHeight => _lineHeight;

        public ChatTextLayoutBuilder(FancyBitmapFont font, float maxWidth)
        {
            _font = font;
            _maxWidth = Math.Max(maxWidth, 40f);
            _charHeight = font.characterHeight * font.scale.y;
            _lineHeight = (font.characterHeight + 2) * font.scale.y;
            // A typical character's width: narrower characters get centered in it, wider ones get squished to fit
            _monoWidth = font.GetGlyphAdvance('0');
            _codePadding = Math.Max(2f, 3f * font.scale.x);
            _codePaddingY = Math.Max(1f, 2f * font.scale.y);
        }

        public int count => _glyphs.Count;

        public ChatGlyph last => _glyphs.Count > 0 ? _glyphs[_glyphs.Count - 1] : null;

        public void ResetStyle()
        {
            _style = ChatStyle.None;
            _hasColor = false;
            _previousHasColor = false;
        }

        public void SetColor(Color color)
        {
            _previousHasColor = _hasColor;
            _previousColor = _color;
            _hasColor = true;
            _color = color;
        }

        private ChatGlyph AddChar(char ch, int sourceIndex = -1, bool dim = false, int block = -1)
        {
            if (ch == '\t')
                ch = ' ';
            bool mono = (_style & (ChatStyle.Code | ChatStyle.CodeBlock)) != 0;
            float advance = _font.GetGlyphAdvance(ch);
            ChatGlyph glyph = new ChatGlyph
            {
                kind = ChatGlyphKind.Char,
                ch = ch,
                style = mono ? _style & (ChatStyle.Code | ChatStyle.CodeBlock) : _style,
                hasColor = _hasColor && !mono,
                color = _color,
                dim = dim,
                sourceIndex = sourceIndex,
                block = block,
            };
            if (mono)
            {
                glyph.w = _monoWidth;
                if (advance > _monoWidth && advance > 0f)
                    glyph.xScale = _monoWidth / advance;
                else
                    glyph.drawOffset = (float)Math.Round((_monoWidth - advance) / 2f);
            }
            else
            {
                glyph.w = advance + ((_style & ChatStyle.Bold) != 0 && ch != ' ' ? _font.boldOffset : 0f);
            }
            _glyphs.Add(glyph);
            return glyph;
        }

        public ChatGlyph AppendBreak(string copyText = null, int sourceIndex = -1, int block = -1)
        {
            ChatGlyph glyph = new ChatGlyph
            {
                kind = ChatGlyphKind.Break,
                copyText = copyText,
                sourceIndex = sourceIndex,
                block = block,
            };
            _glyphs.Add(glyph);
            return glyph;
        }

        public ChatGlyph AppendSprite(Sprite sprite, string copyText)
        {
            sprite.scale *= _font.chatSpriteScale;
            ChatGlyph glyph = new ChatGlyph
            {
                kind = ChatGlyphKind.Sprite,
                sprite = sprite,
                copyText = copyText,
                style = _style & (ChatStyle.Underline | ChatStyle.Strikethrough),
                hasColor = _hasColor,
                color = _color,
                w = sprite.width * sprite.scale.x + 1f,
            };
            _glyphs.Add(glyph);
            return glyph;
        }

        public ChatGlyph AppendImage(ChatImageAttachment image, float width, float height)
        {
            ChatGlyph glyph = new ChatGlyph
            {
                kind = ChatGlyphKind.Image,
                image = image,
                w = width,
                h = height,
            };
            _glyphs.Add(glyph);
            return glyph;
        }

        /// <summary>
        /// Appends text with Duck Game color tags, sprites/mojis and the DGR formatting tags applied.
        /// </summary>
        /// <param name="copySprites">If false, sprites are left out when copying (used for the name badge).</param>
        /// <param name="formattingTags">If false, only vanilla color tags and sprites apply, the DGR formatting tags show as
        /// plain text (used for player names, which shouldn't be able to restyle the chat).</param>
        public void AppendFormatted(string text, bool copySprites = true, bool formattingTags = true)
        {
            if (string.IsNullOrEmpty(text))
                return;
            int openBlock = -1;
            bool inCode = false;
            for (int i = 0; i < text.Length;)
            {
                char c = text[i];
                if (c == '\r')
                {
                    i++;
                    continue;
                }

                // Inside code everything is literal, only the matching closing tag does something
                if (inCode || openBlock >= 0)
                {
                    if (c == '|' && ChatFormatting.TryMatchTag(text, i, out ChatFormatting.Tag closeTag, out int closeEnd)
                        && closeTag.kind == (inCode ? ChatFormatting.TagKind.Code : ChatFormatting.TagKind.CodeBlock))
                    {
                        if (inCode)
                        {
                            inCode = false;
                            _style &= ~ChatStyle.Code;
                        }
                        else
                        {
                            ChatGlyph lastGlyph = last;
                            if (!_truncated && (lastGlyph == null || lastGlyph.kind != ChatGlyphKind.Break || lastGlyph.block != openBlock))
                                AppendBreak(block: openBlock);
                            openBlock = -1;
                            _style &= ~ChatStyle.CodeBlock;
                        }
                        i = closeEnd;
                        continue;
                    }
                    if (Allow(c == '\n'))
                    {
                        if (c == '\n')
                            AppendBreak(block: openBlock);
                        else
                            AddChar(c, block: openBlock);
                    }
                    i++;
                    continue;
                }

                if (c == '|')
                {
                    if (formattingTags && ChatFormatting.TryMatchTag(text, i, out ChatFormatting.Tag tag, out int end))
                    {
                        switch (tag.kind)
                        {
                            case ChatFormatting.TagKind.Style:
                                _style ^= tag.style;
                                break;
                            case ChatFormatting.TagKind.Reset:
                                ResetStyle();
                                break;
                            case ChatFormatting.TagKind.Code:
                                inCode = true;
                                _style |= ChatStyle.Code;
                                break;
                            case ChatFormatting.TagKind.CodeBlock:
                                ChatGlyph lastGlyph = last;
                                if (!_truncated && lastGlyph != null && lastGlyph.kind != ChatGlyphKind.Break)
                                    AppendBreak();
                                openBlock = _blockCount++;
                                _style |= ChatStyle.CodeBlock;
                                // like ``` in markdown, a newline right after the opening tag is ignored
                                if (end < text.Length && text[end] == '\n')
                                    end++;
                                break;
                        }
                        i = end;
                        continue;
                    }
                    if (ChatFormatting.TryMatchColorTag(text, i, out Color color, out bool isPrevious, out int colorEnd))
                    {
                        if (isPrevious)
                        {
                            bool hadColor = _hasColor;
                            Color oldColor = _color;
                            _hasColor = _previousHasColor;
                            _color = _previousColor;
                            _previousHasColor = hadColor;
                            _previousColor = oldColor;
                        }
                        else
                            SetColor(color);
                        i = colorEnd;
                        continue;
                    }
                }

                // http(s) links, everything up to the next space is shown as-is (no mojis or tags inside)
                if (formattingTags && (c == 'h' || c == 'H'))
                {
                    int urlEnd = MatchUrl(text, i);
                    if (urlEnd > i)
                    {
                        ChatLink link = new ChatLink { url = text.Substring(i, urlEnd - i) };
                        for (; i < urlEnd; i++)
                        {
                            if (Allow(false))
                                AddChar(text[i]).link = link;
                        }
                        continue;
                    }
                }

                if (c == '@' || c == ':' && _font.chatFont)
                {
                    NetworkConnection previousConnection = _font._currentConnection;
                    _font._currentConnection = mojiConnection;
                    Sprite sprite = null;
                    int next = i + 1;
                    try
                    {
                        sprite = _font.TryParseSpriteAt(text, i, out next);
                    }
                    catch (Exception)
                    {
                        sprite = null;
                    }
                    _font._currentConnection = previousConnection;
                    if (sprite != null)
                    {
                        if (Allow(false))
                            AppendSprite(sprite, copySprites ? text.Substring(i, next - i) : "");
                        i = next;
                        continue;
                    }
                }

                if (Allow(c == '\n'))
                {
                    if (c == '\n')
                        AppendBreak();
                    else
                        AddChar(c);
                }
                i++;
            }
        }

        /// <summary>
        /// Appends text exactly as typed, every character becomes one glyph (except sprites, see below). Recognized tags are drawn faded.
        /// </summary>
        /// <param name="sourceOffset">Added to every glyph's <see cref="ChatGlyph.sourceIndex"/>.</param>
        /// <param name="sprites">Complete :moji:/@SPRITE@ tags to show as their picture, each one becomes a single glyph
        /// that stands for all of its characters (<see cref="ChatGlyph.sourceIndex"/> to <see cref="ChatGlyph.sourceEnd"/>).</param>
        public void AppendLiteral(string text, int sourceOffset = 0, bool dimMarkup = true, List<ChatSpriteSpan> sprites = null)
        {
            if (text == null)
                return;
            ChatStyle previousStyle = _style;
            bool previousHasColor = _hasColor;
            _style = ChatStyle.None;
            _hasColor = false;
            int dimUntil = -1;
            int nextSprite = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (sprites != null && nextSprite < sprites.Count && sprites[nextSprite].start == i)
                {
                    ChatSpriteSpan span = sprites[nextSprite++];
                    // The spans get reused between frames, AppendSprite scales the sprite it's given
                    Sprite clone = span.sprite.Clone();
                    clone.xscale = span.sprite.xscale;
                    clone.yscale = span.sprite.yscale;
                    ChatGlyph glyph = AppendSprite(clone, text.Substring(span.start, span.end - span.start));
                    glyph.sourceIndex = sourceOffset + span.start;
                    glyph.sourceEnd = sourceOffset + span.end;
                    i = span.end - 1;
                    continue;
                }
                if (dimMarkup && c == '|' && i >= dimUntil && ChatFormatting.IsMarkupAt(text, i, out int end))
                    dimUntil = end;
                if (c == '\n')
                    AppendBreak(sourceIndex: sourceOffset + i);
                else
                    AddChar(c, sourceOffset + i, i < dimUntil);
            }
            _style = previousStyle;
            _hasColor = previousHasColor;
        }

        /// <summary>
        /// Finds the complete :moji: and @SPRITE@ tags in typed text, the same ones <see cref="AppendFormatted"/> would show as a picture.
        /// Tags inside code (|C|...|C|, |CB|...|CB|) are skipped since those show as typed.
        /// </summary>
        public static List<ChatSpriteSpan> FindSprites(FancyBitmapFont font, string text, NetworkConnection connection)
        {
            List<ChatSpriteSpan> spans = new List<ChatSpriteSpan>();
            if (string.IsNullOrEmpty(text) || font == null || text.IndexOf('@') < 0 && (!font.chatFont || text.IndexOf(':') < 0))
                return spans;
            ChatFormatting.TagKind? openCode = null;
            NetworkConnection previousConnection = font._currentConnection;
            font._currentConnection = connection;
            try
            {
                for (int i = 0; i < text.Length;)
                {
                    char c = text[i];
                    if (c == '|' && ChatFormatting.TryMatchTag(text, i, out ChatFormatting.Tag tag, out int tagEnd))
                    {
                        if (tag.kind == ChatFormatting.TagKind.Code || tag.kind == ChatFormatting.TagKind.CodeBlock)
                        {
                            if (openCode == null)
                                openCode = tag.kind;
                            else if (openCode == tag.kind)
                                openCode = null;
                        }
                        i = tagEnd;
                        continue;
                    }
                    if (openCode == null && (c == 'h' || c == 'H'))
                    {
                        int urlEnd = MatchUrl(text, i);
                        if (urlEnd > i)
                        {
                            i = urlEnd;
                            continue;
                        }
                    }
                    if (openCode == null && (c == '@' || c == ':' && font.chatFont))
                    {
                        Sprite sprite = null;
                        int next = i + 1;
                        try
                        {
                            sprite = font.TryParseSpriteAt(text, i, out next);
                        }
                        catch (Exception)
                        {
                            sprite = null;
                        }
                        if (sprite != null && next > i + 1)
                        {
                            spans.Add(new ChatSpriteSpan(i, next, sprite));
                            i = next;
                            continue;
                        }
                    }
                    i++;
                }
            }
            finally
            {
                font._currentConnection = previousConnection;
            }
            return spans;
        }

        /// <summary>
        /// If an http:// or https:// link starts at <paramref name="index"/>, returns the index right after it, otherwise -1.
        /// Trailing punctuation (and unbalanced closing brackets) aren't counted as part of the link.
        /// </summary>
        public static int MatchUrl(string text, int index)
        {
            if (index > 0 && char.IsLetterOrDigit(text[index - 1]))
                return -1;
            int schemeLength;
            if (string.Compare(text, index, "https://", 0, 8, StringComparison.OrdinalIgnoreCase) == 0)
                schemeLength = 8;
            else if (string.Compare(text, index, "http://", 0, 7, StringComparison.OrdinalIgnoreCase) == 0)
                schemeLength = 7;
            else
                return -1;
            int end = index + schemeLength;
            int parens = 0;
            int brackets = 0;
            while (end < text.Length)
            {
                char c = text[end];
                if (char.IsWhiteSpace(c) || c < ' ' || c == '"' || c == '<' || c == '>' || c == '`')
                    break;
                if (c == '|' && ChatFormatting.IsMarkupAt(text, end, out _))
                    break;
                if (c == '(')
                    parens++;
                else if (c == ')')
                {
                    if (parens == 0)
                        break;
                    parens--;
                }
                else if (c == '[')
                    brackets++;
                else if (c == ']')
                {
                    if (brackets == 0)
                        break;
                    brackets--;
                }
                end++;
            }
            while (end > index + schemeLength && ".,!?;:'*".IndexOf(text[end - 1]) >= 0)
                end--;
            // Needs at least a host
            if (end - index - schemeLength < 1 || text[index + schemeLength] == '/' || text[index + schemeLength] == '.')
                return -1;
            return end;
        }

        public ChatTextLayout Build()
        {
            ChatTextLayout layout = new ChatTextLayout
            {
                font = _font,
                lineHeight = _lineHeight,
                charHeight = _charHeight,
                codePadding = _codePadding,
            };
            layout.glyphs.AddRange(_glyphs);

            float y = 0f;
            float x = 0f;
            ChatLine line = null;
            int lastSpace = -1;
            int previousBlock = -1;

            void StartLine(int index, int block)
            {
                if (previousBlock >= 0 && block != previousBlock)
                    y += _codePaddingY;
                if (block >= 0 && block != previousBlock)
                    y += _codePaddingY;
                line = new ChatLine { start = index, end = index, y = y, height = _lineHeight, block = block };
                x = block >= 0 ? _codePadding : 0f;
                lastSpace = -1;
                previousBlock = block;
            }

            void EndLine(int endExclusive)
            {
                line.end = endExclusive;
                float lineWidth = 0f;
                for (int i = line.start; i < line.end; i++)
                {
                    ChatGlyph glyph = layout.glyphs[i];
                    glyph.line = layout.lines.Count;
                    glyph.y = line.y;
                    if (glyph.kind == ChatGlyphKind.Image)
                        line.height = Math.Max(line.height, glyph.h + 2f * _font.scale.y);
                    if (glyph.kind != ChatGlyphKind.Break && !glyph.isSpace)
                        lineWidth = Math.Max(lineWidth, glyph.x + glyph.w);
                }
                line.width = line.block >= 0 ? Math.Max(lineWidth, _codePadding) + _codePadding : lineWidth;
                layout.lines.Add(line);
                y += line.height;
                line = null;
            }

            List<ChatGlyph> glyphs = layout.glyphs;
            for (int i = 0; i < glyphs.Count; i++)
            {
                ChatGlyph glyph = glyphs[i];
                if (line == null)
                    StartLine(i, glyph.block);

                if (glyph.kind == ChatGlyphKind.Image)
                {
                    if (i > line.start)
                    {
                        EndLine(i);
                        StartLine(i, -1);
                    }
                    glyph.x = x;
                    EndLine(i + 1);
                    continue;
                }

                if (glyph.kind == ChatGlyphKind.Break)
                {
                    glyph.x = x;
                    glyph.w = 0f;
                    EndLine(i + 1);
                    continue;
                }

                float limit = _maxWidth - (line.block >= 0 ? _codePadding : 0f);
                if (x + glyph.w > limit && i > line.start && !glyph.isSpace)
                {
                    if (lastSpace >= line.start && lastSpace < i)
                    {
                        int wrapAt = lastSpace + 1;
                        EndLine(wrapAt);
                        StartLine(wrapAt, glyphs[wrapAt].block);
                        for (int j = wrapAt; j < i; j++)
                        {
                            glyphs[j].x = x;
                            x += glyphs[j].w;
                        }
                    }
                    else
                    {
                        // Don't split the "+123" tag across lines
                        int wrapAt = i;
                        while (glyph.chip && wrapAt > line.start && glyphs[wrapAt - 1].chip)
                            wrapAt--;
                        if (wrapAt == line.start)
                            wrapAt = i;
                        EndLine(wrapAt);
                        StartLine(wrapAt, glyphs[wrapAt].block);
                        for (int j = wrapAt; j < i; j++)
                        {
                            glyphs[j].x = x;
                            x += glyphs[j].w;
                        }
                    }
                }

                glyph.x = x;
                x += glyph.w;
                if (glyph.isSpace)
                    lastSpace = i;
            }
            if (line != null)
                EndLine(glyphs.Count);

            if (trailingEmptyLine && (glyphs.Count == 0 || glyphs[glyphs.Count - 1].kind == ChatGlyphKind.Break))
            {
                int block = glyphs.Count > 0 ? glyphs[glyphs.Count - 1].block : -1;
                StartLine(glyphs.Count, block);
                line.width = block >= 0 ? _codePadding * 2f : 0f;
                layout.lines.Add(line);
                y += line.height;
                line = null;
            }
            if (previousBlock >= 0)
                y += _codePaddingY;

            // Code block backgrounds and their contents
            for (int b = 0; b < _blockCount; b++)
            {
                float top = float.MaxValue;
                float bottom = 0f;
                float blockWidth = 0f;
                foreach (ChatLine blockLine in layout.lines)
                {
                    if (blockLine.block != b)
                        continue;
                    top = Math.Min(top, blockLine.y);
                    bottom = Math.Max(bottom, blockLine.y + blockLine.height);
                    blockWidth = Math.Max(blockWidth, blockLine.width);
                }
                if (top == float.MaxValue)
                    continue;
                StringBuilder content = new StringBuilder();
                foreach (ChatGlyph glyph in glyphs)
                {
                    if (glyph.block == b)
                        content.Append(glyph.copyString);
                }
                string contentString = content.ToString();
                if (contentString.EndsWith("\n"))
                    contentString = contentString.Substring(0, contentString.Length - 1);
                layout.blocks.Add(new ChatCodeBlock
                {
                    rect = new Rectangle(0f, top - _codePaddingY, Math.Max(blockWidth, _monoWidth * 6f), bottom - top + _codePaddingY * 2f),
                    content = contentString,
                });
            }

            float width = 0f;
            foreach (ChatLine l in layout.lines)
                width = Math.Max(width, l.width);
            foreach (ChatCodeBlock block in layout.blocks)
                width = Math.Max(width, block.rect.width);
            layout.width = width;
            layout.height = Math.Max(y, _lineHeight);
            return layout;
        }
    }
}
