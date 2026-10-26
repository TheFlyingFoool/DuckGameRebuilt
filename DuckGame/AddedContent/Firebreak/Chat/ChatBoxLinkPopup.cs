using System;
using System.Collections.Generic;

namespace DuckGame
{
    /// <summary>
    /// The Link Popup option: clicking a link in chat first shows where it goes in a small popup next to the link,
    /// styled like the chat's other popups (moji picker, suggestions), with Open / Copy link / Cancel buttons.
    /// Enter opens and Escape cancels too.
    /// </summary>
    public static partial class ChatBox
    {
        private static ChatLink s_linkPopup;
        private static Vec2 s_linkPopupAnchor;
        private static int s_linkPopupSelection;
        private static int s_linkPopupHovered = -1;
        private static readonly List<Rectangle> s_linkPopupItems = new List<Rectangle>();
        private static Rectangle s_linkPopupRect;

        private const int LinkPopupOpen = 0;
        private const int LinkPopupCopy = 1;
        private const int LinkPopupCancel = 2;
        private const int LinkPopupMaxLines = 3;

        public static bool linkPopupOpen => s_linkPopup != null;

        /// <summary>A link in chat was clicked: ask first (Link Popup option) or open it right away.</summary>
        private static void RequestOpenLink(ChatLink link)
        {
            if (link == null)
                return;
            if (!DGRSettings.ChatLinkPopup)
            {
                OpenLink(link.url);
                return;
            }
            s_linkPopup = link;
            s_linkPopupAnchor = s_mousePosition;
            s_linkPopupSelection = LinkPopupOpen;
            s_linkPopupHovered = -1;
            s_linkPopupRect = new Rectangle(-100f, -100f, 0f, 0f);
            s_linkPopupItems.Clear();
            ClearHistorySelection();
        }

        private static void CloseLinkPopup()
        {
            s_linkPopup = null;
            s_linkPopupHovered = -1;
            s_linkPopupItems.Clear();
        }

        /// <summary>Where the link would open, for the open button's label.</summary>
        private static string linkPopupOpenLabel => !DGRSettings.OpenURLsInBrowser && Steam.IsOverlayEnabled() ? "Open in Steam" : "Open in browser";

        private static void ActivateLinkPopup(int item)
        {
            ChatLink link = s_linkPopup;
            if (link == null)
                return;
            switch (item)
            {
                case LinkPopupOpen:
                    OpenLink(link.url);
                    break;
                case LinkPopupCopy:
                    SetClipboard(link.url);
                    s_copiedLink = link;
                    s_copiedLinkFrames = 90;
                    break;
            }
            CloseLinkPopup();
        }

        /// <summary>While the popup is up typing goes nowhere, Enter opens the link (see <see cref="HandleEnterWhileTyping"/>).</summary>
        private static void UpdateLinkPopupKeys()
        {
            s_inputQueue.Clear();
            Keyboard.KeyString = "";
        }

        /// <summary>Hovering highlights a button, clicking uses it, clicking anywhere outside the popup closes it.</summary>
        private static void UpdateLinkPopupMouse(Vec2 mouse, bool pressed)
        {
            s_cursorKind = CursorKind.Arrow;
            s_linkPopupHovered = -1;
            for (int i = 0; i < s_linkPopupItems.Count; i++)
            {
                if (!s_linkPopupItems[i].Contains(mouse))
                    continue;
                s_linkPopupHovered = i;
                s_cursorKind = CursorKind.Hand;
                if (pressed)
                    ActivateLinkPopup(i);
                return;
            }
            if (pressed && !s_linkPopupRect.Contains(mouse))
                CloseLinkPopup();
        }

        /// <summary>Splits the address into lines that fit, the last one ends in "..." if it doesn't all fit.</summary>
        private static List<string> WrapUrl(FancyBitmapFont font, string url, float maxWidth, out bool truncated)
        {
            List<string> lines = new List<string>();
            int start = 0;
            while (start < url.Length && lines.Count < LinkPopupMaxLines)
            {
                int end = start;
                float width = 0f;
                while (end < url.Length && width + font.GetGlyphAdvance(url[end]) <= maxWidth)
                    width += font.GetGlyphAdvance(url[end++]);
                if (end == start)
                    end++;
                lines.Add(url.Substring(start, end - start));
                start = end;
            }
            truncated = start < url.Length && lines.Count > 0;
            if (truncated)
            {
                string last = lines[lines.Count - 1];
                while (last.Length > 0 && MeasurePlain(font, last + "...") > maxWidth)
                    last = last.Substring(0, last.Length - 1);
                lines[lines.Count - 1] = last + "...";
            }
            return lines;
        }

        /// <summary>Where text drawn with <see cref="DrawPlain"/> actually has pixels, relative to its x, y.</summary>
        private static Rectangle MeasureInk(FancyBitmapFont font, string value)
        {
            float x = 0f, left = float.MaxValue, right = float.MinValue, top = float.MaxValue, bottom = float.MinValue;
            foreach (char c in value)
            {
                Rectangle ink = c == ' ' ? default : font.GetGlyphInk(c);
                if (ink.width > 0f)
                {
                    left = Math.Min(left, x + ink.x);
                    right = Math.Max(right, x + ink.x + ink.width);
                    top = Math.Min(top, ink.y);
                    bottom = Math.Max(bottom, ink.y + ink.height);
                }
                x += font.GetGlyphAdvance(c);
            }
            return left > right ? new Rectangle(0f, 0f, x, font.characterHeight * font.scale.y) : new Rectangle(left, top, right - left, bottom - top);
        }

        private static void DrawLinkPopup()
        {
            ChatLink link = s_linkPopup;
            if (link == null)
                return;
            FancyBitmapFont font = SetupFont();
            // Spacing is measured in pixels of the built-in chat font. A system font is drawn at half scale and its letters
            // sit at different spots in their cells, so there everything goes by where the letters really are instead.
            Rectangle lineInk = MeasureInk(font, "Hdgjy");
            Vec2 unit = font.scale;
            if (font is RasterFont && core._builtInChatFont != null)
            {
                float unitY = Math.Max(1f, lineInk.height / core._builtInChatFont.characterHeight);
                unit = new Vec2(unitY * font.scale.x / font.scale.y, unitY);
            }
            float padding = 3f * unit.x;
            float lineHeight = lineInk.height + 2f * unit.y;
            float textY = unit.y - lineInk.y;

            // Width: fits the address up to about a third of the screen, never narrower than the buttons
            string[] labels = { linkPopupOpenLabel, "Copy link", "Cancel" };
            float buttonPadding = 4f * unit.x;
            float gap = 2f * unit.x;
            float buttonsWidth = -gap;
            foreach (string label in labels)
                buttonsWidth += MeasureInk(font, label).width + buttonPadding * 2f + gap;
            float maxTextWidth = Math.Max(buttonsWidth, Math.Min(MeasurePlain(font, link.url), Layer.Console.width * 0.33f));
            List<string> lines = WrapUrl(font, link.url, maxTextWidth, out bool truncated);
            float textWidth = buttonsWidth;
            foreach (string line in lines)
                textWidth = Math.Max(textWidth, MeasureInk(font, line).width);
            const string header = "Open this link?";
            Rectangle headerInk = MeasureInk(font, header);
            textWidth = Math.Max(textWidth, headerInk.width);

            float width = textWidth + padding * 2f;
            float buttonHeight = lineHeight + 2f * unit.y;
            float height = padding + lineHeight + 2f + lines.Count * lineHeight + padding + buttonHeight + padding;

            // Next to where the link was clicked, above it if there's room (the chat sits at the bottom of the screen)
            Vec2 tl = new Vec2(s_linkPopupAnchor.x - 6f * unit.x, s_linkPopupAnchor.y - height - 6f * unit.y);
            if (tl.y < 2f)
                tl.y = s_linkPopupAnchor.y + 10f * unit.y;
            tl.x = Maths.Clamp(tl.x, 2f, Math.Max(2f, Layer.Console.width - width - 2f));
            tl.y = Maths.Clamp(tl.y, 2f, Math.Max(2f, Layer.Console.height - height - 2f));
            tl = new Vec2((float)Math.Round(tl.x), (float)Math.Round(tl.y));
            Vec2 br = tl + new Vec2(width, height);
            s_linkPopupRect = new Rectangle(tl.x, tl.y, width, height);
            DrawPopupFrame(tl, br);

            float x = tl.x + padding;
            float y = tl.y + padding;
            DrawPlain(font, header, x - headerInk.x, y + textY, PopupDimText, OverlayDepth + 0.05f);
            y += lineHeight + 2f;

            // The address, the site's name brighter so it's easy to see where it really goes
            int hostStart = link.url.IndexOf("://", StringComparison.Ordinal);
            hostStart = hostStart < 0 ? 0 : hostStart + 3;
            int hostEnd = link.url.IndexOfAny(new[] { '/', '?', '#' }, hostStart);
            if (hostEnd < 0)
                hostEnd = link.url.Length;
            int index = 0;
            for (int l = 0; l < lines.Count; l++)
            {
                string line = lines[l];
                bool ellipsis = truncated && l == lines.Count - 1;
                int real = ellipsis ? line.Length - 3 : line.Length;
                float lineX = x - MeasureInk(font, line).x;
                for (int i = 0; i < line.Length; i++)
                {
                    bool host = i < real && index + i >= hostStart && index + i < hostEnd;
                    char c = line[i];
                    if (c != ' ')
                        font.DrawGlyph(c, lineX, y + textY, host ? Color.White : new Color(160, 160, 160), D(OverlayDepth + 0.05f));
                    lineX += font.GetGlyphAdvance(c);
                }
                index += real;
                y += lineHeight;
            }
            y += padding;

            // Buttons along the bottom, right aligned like most popups
            s_linkPopupItems.Clear();
            float bx = br.x - padding - buttonsWidth;
            for (int i = 0; i < labels.Length; i++)
            {
                Rectangle labelInk = MeasureInk(font, labels[i]);
                float bw = labelInk.width + buttonPadding * 2f;
                Rectangle rect = new Rectangle(bx, y, bw, buttonHeight);
                s_linkPopupItems.Add(rect);
                bool hovered = i == s_linkPopupHovered;
                bool primary = i == LinkPopupOpen;
                Color fill = primary ? ChatTextLayout.LinkColor * (hovered ? 1f : 0.8f) : Color.White * (hovered ? 0.2f : 0.08f);
                Graphics.DrawRect(new Vec2(rect.x, rect.y), new Vec2(rect.Right, rect.Bottom), fill, D(OverlayDepth + 0.02f));
                Color textColor = primary || hovered ? Color.White : new Color(200, 200, 200);
                DrawPlain(font, labels[i], rect.x + buttonPadding - labelInk.x, rect.y + (buttonHeight - lineInk.height) / 2f - lineInk.y, textColor, OverlayDepth + 0.05f);
                bx += bw + gap;
            }
        }
    }
}
