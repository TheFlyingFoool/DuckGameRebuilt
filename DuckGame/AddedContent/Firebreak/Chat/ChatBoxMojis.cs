using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    /// <summary>
    /// Moji suggestions while typing (":" and a couple of letters, tab or enter to complete) and the moji picker opened from the chat box.
    /// </summary>
    public static partial class ChatBox
    {
        #region Moji list

        /// <summary>The mojis that come with the game (other trigger sprites are button icons and such).</summary>
        private static readonly string[] s_vanillaMojis =
        {
            "happyface", "yayface", "sadface", "cryface", "angryface", "puffyface", "wowface", "wtfface",
            "straightface", "oiface", "sweatface", "blankface", "shrug", "cooked", "rip",
        };

        private sealed class MojiEntry
        {
            public string name;
            public Sprite sprite;
            public bool custom;
            /// <summary>Name of the player it can be stolen from, for /steal suggestions.</summary>
            public string owner;
        }

        private static List<MojiEntry> s_mojiList = new List<MojiEntry>();
        private static string s_mojiListKey;

        private static bool customMojisEnabled => DuckFile.mojimode && Options.Data.mojiFilter != 0;

        /// <summary>Every moji the local player can use: their own custom ones first, then the built in ones.</summary>
        private static List<MojiEntry> GetMojiList()
        {
            string key = customMojisEnabled + "|" + DuckFile.mojiVersion + "|" + DuckFile.mojis.Count;
            if (key == s_mojiListKey)
                return s_mojiList;
            s_mojiListKey = key;
            List<MojiEntry> list = new List<MojiEntry>();
            if (customMojisEnabled)
            {
                foreach (KeyValuePair<string, Sprite> moji in DuckFile.mojis.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(moji.Key) || moji.Value == null || !IsUsableMojiName(moji.Key))
                        continue;
                    list.Add(new MojiEntry { name = moji.Key, sprite = CloneMoji(moji.Value), custom = true });
                }
            }
            foreach (string name in s_vanillaMojis)
            {
                Sprite sprite = Input.GetTriggerSprite(name);
                if (sprite != null)
                    list.Add(new MojiEntry { name = name, sprite = CloneMoji(sprite) });
            }
            s_mojiList = list;
            s_acLastKey = null;
            return list;
        }

        private static List<MojiEntry> s_stealList = new List<MojiEntry>();
        private static string s_stealListKey;

        /// <summary>
        /// Custom mojis other players in the lobby have that we don't, the ones /steal can take.
        /// Follows the custom MOJI setting the same way showing them does (off, friends only, on).
        /// </summary>
        private static List<MojiEntry> GetStealableMojis()
        {
            if (!customMojisEnabled || !Network.isActive)
            {
                if (s_stealList.Count > 0)
                {
                    s_stealList = new List<MojiEntry>();
                    s_stealListKey = null;
                    s_acLastKey = null;
                }
                return s_stealList;
            }
            string key = Options.Data.mojiFilter + "|" + DuckFile.mojiVersion + "|" + DuckFile.mojis.Count + "|" + Network.connections.Count;
            if (key == s_stealListKey)
                return s_stealList;
            s_stealListKey = key;
            Dictionary<string, MojiEntry> found = new Dictionary<string, MojiEntry>();
            foreach (NetworkConnection connection in Network.connections)
            {
                if (connection == null || connection == DuckNetwork.localConnection || connection.identifier == null)
                    continue;
                if (Options.Data.mojiFilter == 1 && connection.data is User user && user.relationship != FriendRelationship.Friend)
                    continue;
                if (!DuckFile.profileMojis.TryGetValue(connection.identifier, out Dictionary<string, Sprite> theirs))
                    continue;
                Profile owner = DuckNetwork.profiles.FirstOrDefault(x => x.connection == connection);
                foreach (KeyValuePair<string, Sprite> moji in theirs)
                {
                    if (moji.Value == null || string.IsNullOrEmpty(moji.Key) || !IsUsableMojiName(moji.Key))
                        continue;
                    if (DuckFile.mojis.ContainsKey(moji.Key) || found.ContainsKey(moji.Key))
                        continue;
                    found[moji.Key] = new MojiEntry { name = moji.Key, sprite = CloneMoji(moji.Value), custom = true, owner = owner?.name };
                }
            }
            s_stealList = found.Values.OrderBy(x => x.name, StringComparer.OrdinalIgnoreCase).ToList();
            s_acLastKey = null;
            return s_stealList;
        }

        private static Sprite CloneMoji(Sprite sprite)
        {
            Sprite clone = sprite.Clone();
            clone.xscale = sprite.xscale;
            clone.yscale = sprite.yscale;
            clone.SkipIntraTick = 1;
            return clone;
        }

        /// <summary>Characters that can be part of a moji name as it's typed (the name ends at a space, ':' or '@').</summary>
        private static bool IsMojiNameChar(char c) => !char.IsWhiteSpace(c) && c != ':' && c != '@' && c != '|' && c >= ' ';

        private static bool IsUsableMojiName(string name)
        {
            foreach (char c in name)
            {
                if (!IsMojiNameChar(c))
                    return false;
            }
            return true;
        }

        /// <summary>Whether a position in the typed text is inside |C| or |CB| code, where mojis show as typed.</summary>
        private static bool IsInsideCode(string value, int index)
        {
            ChatFormatting.TagKind? openCode = null;
            for (int i = value.IndexOf('|'); i >= 0 && i < index; i = value.IndexOf('|', i + 1))
            {
                if (!ChatFormatting.TryMatchTag(value, i, out ChatFormatting.Tag tag, out int end))
                    continue;
                if (tag.kind == ChatFormatting.TagKind.Code || tag.kind == ChatFormatting.TagKind.CodeBlock)
                {
                    if (openCode == null)
                        openCode = tag.kind;
                    else if (openCode == tag.kind)
                        openCode = null;
                }
                i = end - 1;
            }
            return openCode != null;
        }

        #region Favorites

        private static List<string> favoriteMojis => DGRSettings.ChatFavoriteMojis ??= new List<string>();

        private static bool IsFavorite(string name) => name != null && favoriteMojis.Contains(name);

        // "Added to favorites" feedback, shown where the moji's name is shown for a moment
        private static string s_favoriteFeedbackName;
        private static string s_favoriteFeedback;
        private static int s_favoriteFeedbackFrames;

        /// <summary>Adds a moji to the favorites or takes it out (alt+click), saved with the rest of the DGR settings.</summary>
        private static void ToggleFavorite(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;
            bool added = !favoriteMojis.Remove(name);
            if (added)
                favoriteMojis.Add(name);
            s_favoriteFeedbackName = name;
            s_favoriteFeedback = added ? "Added to favorites" : "Removed from favorites";
            s_favoriteFeedbackFrames = 90;
            s_acLastKey = null; // suggestion order changes
            try
            {
                AutoConfigHandler.SaveAll(false);
            }
            catch (Exception e)
            {
                DevConsole.Log(e);
            }
        }

        /// <summary>The feedback after alt+clicking this moji, if it was just (un)favorited.</summary>
        private static string GetFavoriteFeedback(string name) => s_favoriteFeedbackFrames > 0 && name == s_favoriteFeedbackName ? s_favoriteFeedback : null;

        /// <summary>
        /// The name of a moji the local player can use, from what a moji glyph stands for (":name:" or "@name@"), null if it isn't one.
        /// </summary>
        private static string GetUsableMojiName(ChatGlyph glyph)
        {
            if (glyph == null || glyph.kind != ChatGlyphKind.Sprite || glyph.copyText == null || glyph.copyText.Length < 3)
                return null;
            string name = glyph.copyText.Substring(1, glyph.copyText.Length - 2);
            return GetMojiList().Any(x => x.name == name) ? name : null;
        }

        /// <summary>Small yellow sparkle in the corner of a favorite moji.</summary>
        private static void DrawFavoriteMark(float right, float top, float size, float depth)
        {
            size = Math.Max(3f, (float)Math.Round(size));
            float thin = Math.Max(1f, (float)Math.Round(size / 3f));
            float x = right - size;
            float middle = (float)Math.Round((size - thin) / 2f);
            Graphics.DrawRect(new Vec2(x, top + middle), new Vec2(x + size, top + middle + thin), Colors.DGYellow, D(depth));
            Graphics.DrawRect(new Vec2(x + middle, top), new Vec2(x + middle + thin, top + size), Colors.DGYellow, D(depth));
        }

        #endregion

        /// <summary>Types a moji at the caret.</summary>
        private static void InsertMoji(string name)
        {
            PushUndo(true);
            InsertText(":" + name + ":");
            ClearHistorySelection();
        }

        #endregion

        #region Suggestions

        private const int AutocompleteMax = 6;
        private const int StealSuggestionsMax = 8;
        private const string StealCommand = "/steal";
        /// <summary>Suggesting mojis for /steal (other players' mojis we don't have) instead of our own.</summary>
        private static bool s_acSteal;
        /// <summary>For /steal, where the ":name:" being replaced ends.</summary>
        private static int s_acReplaceEnd = -1;
        private static int s_acStart = -1;
        private static string s_acQuery = "";
        private static readonly List<MojiEntry> s_acItems = new List<MojiEntry>();
        private static int s_acSelected;
        private static string s_acLastKey;
        private static string s_acDismissedKey;
        /// <summary>What the popup is showing right now (mode, position and typed name), see <see cref="DismissAutocomplete"/>.</summary>
        private static string s_acKey;
        private static readonly List<Rectangle> s_acRowRects = new List<Rectangle>();
        private static Rectangle s_acRect;
        private static Vec2 s_acLastMouse;

        private static bool autocompleteVisible => s_acStart >= 0 && s_acItems.Count > 0;

        /// <summary>The /steal popup with nothing to suggest, it says why instead.</summary>
        private static bool stealHintVisible => s_acSteal && s_acStart >= 0 && s_acItems.Count == 0;

        /// <summary>Looks for a ":name" being typed right before the caret and finds the mojis it could be.</summary>
        private static void UpdateAutocomplete()
        {
            int start = FindStealStart(out string query, out int replaceEnd);
            bool steal = start >= 0;
            if (!steal)
            {
                start = DGRSettings.ChatMojiSuggestions ? FindAutocompleteStart(out query) : -1;
                replaceEnd = -1;
            }
            if (start < 0)
            {
                s_acStart = -1;
                s_acSteal = false;
                s_acItems.Clear();
                return;
            }
            string key = (steal ? "steal" : "") + start + ":" + query;
            if (key == s_acDismissedKey)
            {
                s_acStart = -1;
                return;
            }
            s_acDismissedKey = null;
            s_acKey = key;
            List<MojiEntry> list = steal ? GetStealableMojis() : GetMojiList();
            if (key != s_acLastKey)
            {
                s_acLastKey = key;
                s_acSelected = 0;
                s_acItems.Clear();
                if (query.Length == 0)
                    s_acItems.AddRange(list.Take(StealSuggestionsMax)); // just "/steal", already sorted by name
                else
                {
                    // Names starting with what was typed first, then names containing it, favorites and shorter names first
                    s_acItems.AddRange(list
                        .Select(x => (entry: x, at: x.name.IndexOf(query, StringComparison.OrdinalIgnoreCase)))
                        .Where(x => x.at >= 0)
                        .OrderBy(x => x.at == 0 ? 0 : 1)
                        .ThenBy(x => IsFavorite(x.entry.name) ? 0 : 1) // favorites first among equally good matches
                        .ThenBy(x => x.entry.name.Length)
                        .ThenBy(x => x.entry.name, StringComparer.OrdinalIgnoreCase)
                        .Take(steal ? StealSuggestionsMax : AutocompleteMax)
                        .Select(x => x.entry));
                }
            }
            s_acSteal = steal;
            s_acReplaceEnd = replaceEnd;
            s_acStart = start;
            s_acQuery = query;
            s_acSelected = Maths.Clamp(s_acSelected, 0, Math.Max(0, s_acItems.Count - 1));
        }

        private static int FindAutocompleteStart(out string query)
        {
            query = "";
            if (s_pickerOpen || hasInputSelection || DevConsole.open)
                return -1;
            string value = text;
            int caret = Maths.Clamp(s_caret, 0, value.Length);
            int nameStart = caret;
            while (nameStart > 0 && IsMojiNameChar(value[nameStart - 1]))
                nameStart--;
            if (nameStart == 0 || value[nameStart - 1] != ':' || caret - nameStart < 2)
                return -1;
            int colon = nameStart - 1;
            // "10:30" or "word:abc" isn't the start of a moji
            if (colon > 0 && char.IsLetterOrDigit(value[colon - 1]))
                return -1;
            foreach (ChatSpriteSpan span in GetInputSprites())
            {
                if (colon >= span.start && colon < span.end)
                    return -1;
            }
            if (IsInsideCode(value, colon))
                return -1;
            query = value.Substring(nameStart, caret - nameStart);
            return colon;
        }

        /// <summary>
        /// While typing "/steal :name:", where the ":name:" part starts (or where it goes). Everything after "/steal" can be suggested,
        /// even with nothing typed yet, so the popup works as a list of what can be stolen.
        /// </summary>
        private static int FindStealStart(out string query, out int replaceEnd)
        {
            query = "";
            replaceEnd = -1;
            if (s_pickerOpen || hasInputSelection || DevConsole.open)
                return -1;
            string value = text;
            if (!value.StartsWith(StealCommand))
                return -1;
            int i = StealCommand.Length;
            if (i < value.Length && value[i] != ' ' && value[i] != ':')
                return -1;
            while (i < value.Length && value[i] == ' ')
                i++;
            int start = i;
            int nameStart = i < value.Length && value[i] == ':' ? i + 1 : i;
            int nameEnd = nameStart;
            while (nameEnd < value.Length && IsMojiNameChar(value[nameEnd]))
                nameEnd++;
            int caret = Maths.Clamp(s_caret, 0, value.Length);
            // Once the caret is past the closing ':' the command is ready to send
            if (caret < start || caret > nameEnd)
                return -1;
            query = caret > nameStart ? value.Substring(nameStart, caret - nameStart) : "";
            replaceEnd = nameEnd < value.Length && value[nameEnd] == ':' ? nameEnd + 1 : nameEnd;
            return start;
        }

        private static void MoveAutocompleteSelection(int direction)
        {
            if (s_acItems.Count == 0)
                return;
            s_acSelected = (s_acSelected + direction + s_acItems.Count) % s_acItems.Count;
        }

        private static void DismissAutocomplete()
        {
            if (s_acStart >= 0)
                s_acDismissedKey = s_acKey; // same key UpdateAutocomplete checks, including the /steal mode
            s_acStart = -1;
        }

        /// <summary>Replaces the ":name" being typed with the selected moji.</summary>
        private static void AcceptAutocomplete()
        {
            if (!autocompleteVisible || s_acSelected >= s_acItems.Count)
                return;
            MojiEntry entry = s_acItems[s_acSelected];
            string value = text;
            int start = Maths.Clamp(s_acStart, 0, value.Length);
            int end;
            string insert = ":" + entry.name + ":";
            if (s_acSteal)
            {
                // "/steal :name:", nothing after it
                end = Maths.Clamp(s_acReplaceEnd, start, value.Length);
                if (start == StealCommand.Length)
                    insert = " " + insert;
            }
            else
            {
                end = Maths.Clamp(s_caret, start, value.Length);
                // Replace the rest of the name too if the caret was in the middle of it
                while (end < value.Length && IsMojiNameChar(value[end]))
                    end++;
                if (end < value.Length && value[end] == ':')
                    end++;
                if (end >= value.Length || !char.IsWhiteSpace(value[end]))
                    insert += " ";
            }
            string result = value.Remove(start, end - start).Insert(start, insert);
            if (result.Length > MaxMessageLength)
                return;
            PushUndo(true);
            text = result;
            s_caret = s_anchor = start + insert.Length;
            s_acStart = -1;
            s_acItems.Clear();
            s_acLastKey = null;
            ResetBlink();
        }

        #endregion

        #region Picker

        private static bool s_pickerOpen;
        private static int s_pickerScroll;
        private static Rectangle s_pickerRect;
        private static MojiEntry s_pickerHovered;
        private static readonly List<(Rectangle rect, MojiEntry entry)> s_pickerCells = new List<(Rectangle, MojiEntry)>();

        private const int PickerMinColumns = 8;
        private const string PickerOffHint = "Custom MOJIs are off (Options > Usability)";
        private const string PickerEmptyHint = "Put pictures in Custom/Moji to add your own";
        private const string PickerFooterHint = "Shift+click: add several   Alt+click: favorite";
        private const int PickerVisibleRows = 5;

        private sealed class PickerRow
        {
            public string header;
            public string hint;
            public List<MojiEntry> items;
        }

        private static void TogglePicker()
        {
            s_pickerOpen = !s_pickerOpen;
            s_pickerHovered = null;
            if (s_pickerOpen)
            {
                s_acStart = -1;
                ClearHistorySelection();
            }
        }

        private static void ResetMojiUI()
        {
            s_pickerOpen = false;
            s_pickerHovered = null;
            s_pickerCells.Clear();
            s_pickerRect = new Rectangle(-100f, -100f, 0f, 0f);
            s_acStart = -1;
            s_acSteal = false;
            s_acItems.Clear();
            s_acLastKey = null;
            s_acDismissedKey = null;
            s_acRowRects.Clear();
            s_acRect = new Rectangle(-100f, -100f, 0f, 0f);
        }

        private static List<PickerRow> BuildPickerRows(int columns)
        {
            List<PickerRow> rows = new List<PickerRow>();
            List<MojiEntry> list = GetMojiList();
            void AddGrid(IEnumerable<MojiEntry> entries)
            {
                PickerRow row = null;
                foreach (MojiEntry entry in entries)
                {
                    if (row == null || row.items.Count >= columns)
                    {
                        row = new PickerRow { items = new List<MojiEntry>() };
                        rows.Add(row);
                    }
                    row.items.Add(entry);
                }
            }
            // Favorites first, in the order they were added (only ones that can be used right now)
            List<MojiEntry> favorites = favoriteMojis
                .Select(name => list.FirstOrDefault(x => x.name == name))
                .Where(x => x != null)
                .ToList();
            if (favorites.Count > 0)
            {
                rows.Add(new PickerRow { header = "FAVORITES" });
                AddGrid(favorites);
            }
            rows.Add(new PickerRow { header = "YOUR MOJIS" });
            if (!customMojisEnabled)
                rows.Add(new PickerRow { hint = PickerOffHint });
            else if (!list.Any(x => x.custom))
                rows.Add(new PickerRow { hint = PickerEmptyHint });
            else
                AddGrid(list.Where(x => x.custom));
            rows.Add(new PickerRow { header = "DUCK GAME" });
            AddGrid(list.Where(x => !x.custom));
            return rows;
        }

        #endregion

        #region Mouse

        /// <summary>Mouse handling for the picker and the suggestions, returns true if they used the mouse this frame.</summary>
        private static bool UpdateMojiMouse(Vec2 mouse, bool pressed)
        {
            if (s_pickerOpen)
            {
                s_pickerHovered = null;
                if (s_pickerRect.Contains(mouse))
                {
                    if (Mouse.isScrolling)
                        s_pickerScroll += Mouse.scroll > 0f ? 1 : -1;
                    foreach ((Rectangle rect, MojiEntry entry) in s_pickerCells)
                    {
                        if (!rect.Contains(mouse))
                            continue;
                        s_pickerHovered = entry;
                        s_cursorKind = CursorKind.Hand;
                        if (pressed && Keyboard.alt)
                            ToggleFavorite(entry.name);
                        else if (pressed)
                        {
                            InsertMoji(entry.name);
                            // shift+click to add a few in a row
                            if (!Keyboard.shift)
                                s_pickerOpen = false;
                        }
                        break;
                    }
                    return true;
                }
                // Clicking anywhere else closes it
                if (pressed && !s_mojiButton.Contains(mouse))
                {
                    s_pickerOpen = false;
                    return true;
                }
            }

            if (autocompleteVisible && s_acRect.Contains(mouse))
            {
                bool moved = (mouse - s_acLastMouse).lengthSq > 0.25f;
                s_acLastMouse = mouse;
                for (int i = 0; i < s_acRowRects.Count && i < s_acItems.Count; i++)
                {
                    if (!s_acRowRects[i].Contains(mouse))
                        continue;
                    s_cursorKind = CursorKind.Hand;
                    if (moved || pressed)
                        s_acSelected = i;
                    if (pressed && Keyboard.alt && !s_acSteal)
                        ToggleFavorite(s_acItems[i].name);
                    else if (pressed)
                        AcceptAutocomplete();
                    break;
                }
                return true;
            }
            s_acLastMouse = mouse;
            return false;
        }

        #endregion

        #region Drawing

        private static readonly Color PopupBackground = new Color(24, 24, 24);
        private static readonly Color PopupDimText = new Color(150, 150, 150);

        private static void DrawMojiPopups()
        {
            s_pickerCells.Clear();
            s_acRowRects.Clear();
            if (s_pickerOpen)
            {
                s_acRect = new Rectangle(-100f, -100f, 0f, 0f);
                DrawPicker();
                return;
            }
            s_pickerRect = new Rectangle(-100f, -100f, 0f, 0f);
            if (autocompleteVisible || stealHintVisible)
                DrawAutocomplete();
            else
                s_acRect = new Rectangle(-100f, -100f, 0f, 0f);
        }

        private static void DrawPopupFrame(Vec2 tl, Vec2 br)
        {
            Graphics.DrawRect(tl, br, PopupBackground * 0.95f, D(OverlayDepth));
            Graphics.DrawRect(tl, br, Color.White * 0.35f, D(OverlayDepth + 0.01f), false, 1f);
        }

        /// <summary>Draws text exactly as given (no tags or mojis), returns its width.</summary>
        private static float DrawPlain(FancyBitmapFont font, string value, float x, float y, Color color, float depth)
        {
            float start = x;
            foreach (char c in value)
            {
                if (c != ' ')
                    font.DrawGlyph(c, x, y, color, D(depth));
                x += font.GetGlyphAdvance(c);
            }
            return x - start;
        }

        private static float MeasurePlain(FancyBitmapFont font, string value)
        {
            float width = 0f;
            foreach (char c in value)
                width += font.GetGlyphAdvance(c);
            return width;
        }

        /// <summary>Draws a sprite scaled to fit a box, centered in it.</summary>
        private static void DrawSpriteFitted(Sprite sprite, float x, float y, float boxWidth, float boxHeight, Color tint, float alpha, float depth)
        {
            if (sprite == null || sprite.width <= 0 || sprite.height <= 0 || boxWidth <= 0f || boxHeight <= 0f)
                return;
            float scale = Math.Min(boxWidth / sprite.width, boxHeight / sprite.height);
            // Keep the pixel art crisp when there's room for it
            if (scale >= 1f)
                scale = (float)Math.Floor(scale * 2f) / 2f;
            Vec2 oldScale = sprite.scale;
            Vec2 oldCenter = sprite.center;
            float oldAlpha = sprite.alpha;
            Color oldColor = sprite.color;
            sprite.scale = new Vec2(scale, scale);
            sprite.center = Vec2.Zero;
            sprite.alpha = alpha;
            sprite.color = tint;
            sprite.SkipIntraTick = 1;
            Graphics.Draw(sprite, (float)Math.Round(x + (boxWidth - sprite.width * scale) / 2f), (float)Math.Round(y + (boxHeight - sprite.height * scale) / 2f), D(depth));
            sprite.scale = oldScale;
            sprite.center = oldCenter;
            sprite.alpha = oldAlpha;
            sprite.color = oldColor;
        }

        private static Sprite s_mojiIcon;

        /// <summary>The smiley on the moji button, dark until hovered.</summary>
        private static void DrawMojiIcon(Rectangle rect, bool active, float alpha)
        {
            if (s_mojiIcon == null)
            {
                Sprite happy = Input.GetTriggerSprite("happyface");
                if (happy == null)
                    return;
                s_mojiIcon = CloneMoji(happy);
            }
            float inset = (float)Math.Round(rect.height * 0.15f);
            DrawSpriteFitted(s_mojiIcon, rect.x + inset, rect.y + inset, rect.width - inset * 2f, rect.height - inset * 2f, active ? Color.White : new Color(60, 60, 60), alpha, 1f);
        }

        private static void DrawAutocomplete()
        {
            FancyBitmapFont font = SetupFont();
            float padding = 3f * font.scale.x;
            float lineHeight = (font.characterHeight + 2) * font.scale.y;
            float rowHeight = (float)Math.Round(lineHeight * 1.4f);
            float iconSize = rowHeight - 2f * font.scale.y;
            string header;
            string emptyHint = null;
            if (!s_acSteal)
                header = "MOJIS MATCHING :" + s_acQuery;
            else if (s_acItems.Count > 0)
            {
                int total = GetStealableMojis().Count;
                // With nothing typed only the first few fit, say how many there are so it's clear typing narrows them down
                header = s_acQuery.Length == 0
                    ? (total > s_acItems.Count ? "MOJIS YOU CAN STEAL (" + total + ", TYPE TO SEARCH)" : "MOJIS YOU CAN STEAL")
                    : "MOJIS YOU CAN STEAL MATCHING :" + s_acQuery;
            }
            else if (!customMojisEnabled)
            {
                header = "NOTHING TO STEAL";
                emptyHint = "Custom MOJIs are off (Options > Usability)";
            }
            else if (GetStealableMojis().Count == 0)
            {
                header = "NOTHING TO STEAL";
                emptyHint = "Nobody here has a MOJI you don't";
            }
            else
            {
                header = "NOTHING TO STEAL MATCHING :" + s_acQuery;
                emptyHint = "Delete some letters to see the rest";
            }
            // With TAB bound to CHAT it sends instead, ENTER still picks
            string hint = IsChatKey(Keys.Tab) ? "ENTER" : "TAB";
            float hintWidth = MeasurePlain(font, hint);
            float width = MeasurePlain(font, header);
            if (emptyHint != null)
                width = Math.Max(width, MeasurePlain(font, emptyHint));
            foreach (MojiEntry entry in s_acItems)
            {
                // /steal rows also say whose moji it is
                string side = s_acSteal && entry.owner != null ? "from " + entry.owner : hint;
                width = Math.Max(width, iconSize + padding + MeasurePlain(font, ":" + entry.name + ":") + padding * 3f + Math.Max(hintWidth, MeasurePlain(font, side)));
            }
            width += padding * 2f;
            float height = padding * 2f + lineHeight + 2f + rowHeight * s_acItems.Count + (emptyHint != null ? lineHeight : 0f);
            Vec2 tl = new Vec2(14f, s_inputTop - height - 4f);
            if (tl.y < 2f)
                tl.y = 2f;
            Vec2 br = tl + new Vec2(width, height);
            s_acRect = new Rectangle(tl.x, tl.y, width, height);
            DrawPopupFrame(tl, br);

            float y = tl.y + padding;
            DrawPlain(font, header, tl.x + padding, y, PopupDimText, OverlayDepth + 0.05f);
            y += lineHeight + 2f;
            if (emptyHint != null)
                DrawPlain(font, emptyHint, tl.x + padding, y, new Color(120, 120, 120), OverlayDepth + 0.05f);
            for (int i = 0; i < s_acItems.Count; i++)
            {
                MojiEntry entry = s_acItems[i];
                Rectangle row = new Rectangle(tl.x + 1f, y, width - 2f, rowHeight);
                s_acRowRects.Add(row);
                bool selected = i == s_acSelected;
                if (selected)
                    Graphics.DrawRect(new Vec2(row.x, row.y), new Vec2(row.Right, row.Bottom), Color.White * 0.14f, D(OverlayDepth + 0.02f));
                DrawSpriteFitted(entry.sprite, tl.x + padding, y + (rowHeight - iconSize) / 2f, iconSize, iconSize, Color.White, 1f, OverlayDepth + 0.04f);
                if (!s_acSteal && IsFavorite(entry.name))
                    DrawFavoriteMark(tl.x + padding + iconSize + 1f, y + 1f, iconSize * 0.3f, OverlayDepth + 0.045f);
                float textY = y + (rowHeight - font.characterHeight * font.scale.y) / 2f;
                DrawPlain(font, ":" + entry.name + ":", tl.x + padding + iconSize + padding, textY, selected ? Color.White : new Color(210, 210, 210), OverlayDepth + 0.05f);
                string feedback = s_acSteal ? null : GetFavoriteFeedback(entry.name);
                if (feedback != null)
                    DrawPlain(font, feedback, br.x - padding - MeasurePlain(font, feedback), textY, Colors.DGYellow, OverlayDepth + 0.05f);
                else if (s_acSteal && entry.owner != null)
                {
                    string owner = "from " + entry.owner;
                    DrawPlain(font, owner, br.x - padding - MeasurePlain(font, owner), textY, PopupDimText, OverlayDepth + 0.05f);
                }
                else if (selected)
                    DrawPlain(font, hint, br.x - padding - hintWidth, textY, PopupDimText, OverlayDepth + 0.05f);
                y += rowHeight;
            }
        }

        private static void DrawPicker()
        {
            FancyBitmapFont font = SetupFont();
            float padding = 3f * font.scale.x;
            float lineHeight = (font.characterHeight + 2) * font.scale.y;
            float cell = (float)Math.Round(lineHeight * 1.6f);
            float headerHeight = lineHeight + 2f;
            float viewHeight = cell * PickerVisibleRows;
            // At least 8 mojis across, wider if a hint needs the room (more columns then)
            float innerWidth = cell * PickerMinColumns;
            innerWidth = Math.Max(innerWidth, MeasurePlain(font, PickerFooterHint));
            if (!customMojisEnabled)
                innerWidth = Math.Max(innerWidth, MeasurePlain(font, PickerOffHint));
            else if (!GetMojiList().Any(x => x.custom))
                innerWidth = Math.Max(innerWidth, MeasurePlain(font, PickerEmptyHint));
            int columns = Math.Max(PickerMinColumns, (int)(innerWidth / cell));
            innerWidth = Math.Max(innerWidth, columns * cell);
            float width = padding * 2f + innerWidth;
            float height = padding * 2f + lineHeight + 2f + viewHeight + 2f + lineHeight;

            List<PickerRow> rows = BuildPickerRows(columns);
            float RowHeight(PickerRow row) => row.items != null ? cell : headerHeight;

            // Scrolling is by whole rows, so nothing has to be clipped
            int maxScroll = rows.Count;
            float fromEnd = 0f;
            while (maxScroll > 0 && fromEnd + RowHeight(rows[maxScroll - 1]) <= viewHeight)
                fromEnd += RowHeight(rows[--maxScroll]);
            s_pickerScroll = Maths.Clamp(s_pickerScroll, 0, maxScroll);

            float x = Math.Max(14f, s_inputRight - width);
            if (x + width > Layer.Console.width - 2f)
                x = Math.Max(2f, Layer.Console.width - 2f - width);
            Vec2 tl = new Vec2(x, s_inputTop - height - 4f);
            if (tl.y < 2f)
                tl.y = 2f;
            Vec2 br = tl + new Vec2(width, height);
            s_pickerRect = new Rectangle(tl.x, tl.y, width, height);
            DrawPopupFrame(tl, br);

            float y = tl.y + padding;
            DrawPlain(font, "MOJIS", tl.x + padding, y, Color.White, OverlayDepth + 0.05f);
            if (s_pickerScroll > 0 || maxScroll > 0)
            {
                string scrollHint = s_pickerScroll < maxScroll ? "SCROLL FOR MORE" : "";
                DrawPlain(font, scrollHint, br.x - padding - MeasurePlain(font, scrollHint), y, PopupDimText, OverlayDepth + 0.05f);
            }
            y += lineHeight + 2f;
            float viewBottom = y + viewHeight;
            for (int r = s_pickerScroll; r < rows.Count; r++)
            {
                PickerRow row = rows[r];
                float rowHeight = RowHeight(row);
                if (y + rowHeight > viewBottom + 0.5f)
                    break;
                if (row.header != null)
                {
                    DrawPlain(font, row.header, tl.x + padding, y + 1f, PopupDimText, OverlayDepth + 0.05f);
                    Graphics.DrawLine(new Vec2(tl.x + padding, y + headerHeight - 1f), new Vec2(br.x - padding, y + headerHeight - 1f), Color.White * 0.15f, 1f, D(OverlayDepth + 0.02f));
                }
                else if (row.hint != null)
                    DrawPlain(font, row.hint, tl.x + padding, y + 1f, new Color(120, 120, 120), OverlayDepth + 0.05f);
                else
                {
                    for (int i = 0; i < row.items.Count; i++)
                    {
                        MojiEntry entry = row.items[i];
                        Rectangle rect = new Rectangle(tl.x + padding + i * cell, y, cell, cell);
                        s_pickerCells.Add((rect, entry));
                        bool hovered = entry == s_pickerHovered;
                        if (hovered)
                            Graphics.DrawRect(new Vec2(rect.x, rect.y), new Vec2(rect.Right, rect.Bottom), Color.White * 0.16f, D(OverlayDepth + 0.02f));
                        float inset = (float)Math.Round(cell * 0.12f);
                        DrawSpriteFitted(entry.sprite, rect.x + inset, rect.y + inset, cell - inset * 2f, cell - inset * 2f, Color.White, 1f, OverlayDepth + 0.04f);
                        if (IsFavorite(entry.name))
                            DrawFavoriteMark(rect.Right - 1f, rect.y + 1f, cell * 0.22f, OverlayDepth + 0.045f);
                    }
                }
                y += rowHeight;
            }

            // Footer: the hovered moji's name, or how to use the picker
            float footerY = tl.y + padding + lineHeight + 2f + viewHeight + 2f;
            Graphics.DrawLine(new Vec2(tl.x + 1f, footerY - 1f), new Vec2(br.x - 1f, footerY - 1f), Color.White * 0.15f, 1f, D(OverlayDepth + 0.02f));
            string footer = PickerFooterHint;
            if (s_pickerHovered != null)
            {
                string feedback = GetFavoriteFeedback(s_pickerHovered.name);
                footer = ":" + s_pickerHovered.name + ":" + (feedback != null ? "  " + feedback : IsFavorite(s_pickerHovered.name) ? "  (favorite)" : "");
            }
            float maxFooter = width - padding * 2f;
            while (footer.Length > 4 && MeasurePlain(font, footer) > maxFooter)
                footer = footer.Substring(0, footer.Length - 4) + "...";
            DrawPlain(font, footer, tl.x + padding, footerY + 1f, s_pickerHovered != null ? Color.White : PopupDimText, OverlayDepth + 0.05f);
        }

        #endregion
    }
}
