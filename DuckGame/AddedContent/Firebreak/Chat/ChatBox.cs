using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;

namespace DuckGame
{
    /// <summary>
    /// The online chat UI: the input box (caret editing, selection, undo, clipboard), the message history
    /// (word wrapped formatted bubbles that can be selected and copied with the mouse), image attachments and the mouse cursor.
    /// <para>
    /// Hooked into <see cref="DuckNetwork.Update"/> and <see cref="DuckNetwork.Draw"/>, the text being typed still lives in
    /// <see cref="DuckNetworkCore.currentEnterText"/>.
    /// </para>
    /// </summary>
    public static partial class ChatBox
    {
        /// <summary>Not a real limit, just a sanity cap so nobody pastes a novel into the lobby.</summary>
        public const int MaxMessageLength = 2000;
        /// <summary>Most lines (newlines + 1) a message can be typed with.</summary>
        public const int MaxLines = 24;
        private const int MaxMessagesDrawn = 9;
        private const int DoubleClickFrames = 24;

        private static DuckNetworkCore core => DuckNetwork.core;

        /// <summary>
        /// A depth that doesn't take a value from <see cref="Depth"/>'s shared span counter. Pulling one from it for every
        /// glyph made the counter wrap at different points depending on how much text was on screen, which made chat
        /// heads and bubbles swap places (flicker). Chat elements are ordered by their depth values alone.
        /// </summary>
        internal static Depth D(float value) => new Depth(value, 0f);

        #region State

        // Input box
        private static int s_caret;
        private static int s_anchor;
        private static readonly List<char> s_inputQueue = new List<char>();
        private static readonly Func<char, bool> s_textInputHandler = c =>
        {
            if (!core.enteringText || core.stopEnteringText || DevConsole.open)
                return false;
            if (s_inputQueue.Count < 4096)
                s_inputQueue.Add(c);
            return true;
        };
        private static readonly List<(string text, int caret)> s_undo = new List<(string, int)>();
        private static readonly List<(string text, int caret)> s_redo = new List<(string, int)>();
        private static int s_lastUndoFrame = -999;
        private static bool s_wasOpen;

        // Mouse
        private enum DragMode
        {
            None,
            Input,
            History,
        }

        private enum CursorKind
        {
            Arrow = 0,
            Hand = 1,
            Text = 7,
        }

        private static DragMode s_drag;
        private static Vec2 s_mousePosition;
        private static Vec2 s_mouseOpenPosition;
        private static bool s_showCursor;
        private static CursorKind s_cursorKind;
        private static int s_frame;
        private static int s_lastClickFrame = -999;
        private static Vec2 s_lastClickPosition;
        private static int s_clickCount;
        private static SpriteMap s_cursorSprite;

        // History selection, positions are glyph boundaries in a message's layout
        private static ChatMessage s_selectionAnchorMessage;
        private static int s_selectionAnchor;
        private static ChatMessage s_selectionFocusMessage;
        private static int s_selectionFocus;
        private static int s_wordAnchorFrom = -1;
        private static int s_wordAnchorTo = -1;

        // What was drawn last frame, for hit testing
        private class DrawnBubble
        {
            public ChatMessage message;
            public ChatTextLayout layout;
            public Vec2 textOrigin;
            public Rectangle bounds;
        }

        private static readonly List<DrawnBubble> s_drawnBubbles = new List<DrawnBubble>();
        private static ChatTextLayout s_inputLayout;
        private static Vec2 s_inputTextOrigin;
        private static Rectangle s_inputBounds;
        private static int s_inputPrefixLength;
        private static Rectangle s_imageButton;
        private static Rectangle s_mojiButton;
        private static Rectangle s_guideButton;
        private static float s_inputRight;
        /// <summary>How visible the buttons in the chat box are, they fade in when the mouse comes close.</summary>
        private static float s_toolFade = 0.4f;
        private static float s_inputTop;
        private static string s_tooltip;
        private static bool s_guideHovered;

        private static Rectangle s_removeAttachmentButton;
        private static Rectangle s_attachmentThumbnail;
        private static bool s_inputDrawn;

        public static ChatImageAttachment hoveredImage;
        private static ChatLink s_hoveredLink;
        private static ChatLink s_pressedLink;
        private static ChatLink s_copiedLink;
        private static int s_copiedLinkFrames;
        private static Vec2 s_pressedLinkPosition;
        private static ChatCodeBlock s_hoveredBlock;
        private static Rectangle s_hoveredBlockCopyButton;
        private static string s_copiedBlockContent;
        private static ChatMessage s_hoveredBlockMessage;
        private static int s_hoveredBlockIndex = -1;
        private static int s_copiedFrames;
        public static bool previewOpen => ChatImagePreview.isOpen;

        /// <summary>Whether the mouse wheel is being used by the chat (image preview zoom, scrolling the moji picker) instead of scrolling the history.</summary>
        public static bool capturesMouseWheel => ChatImagePreview.isOpen || linkPopupOpen || s_pickerOpen && s_pickerRect.Contains(s_mousePosition);

        #endregion

        #region Text helpers

        private static string text
        {
            get => core.currentEnterText ?? "";
            set => core.currentEnterText = value ?? "";
        }

        private static bool hasInputSelection => s_anchor != s_caret;
        private static int selectionStart => Math.Min(s_anchor, s_caret);
        private static int selectionEnd => Math.Max(s_anchor, s_caret);

        private static void ClampCaret()
        {
            int length = text.Length;
            s_caret = SnapCaret(Maths.Clamp(s_caret, 0, length), 1);
            s_anchor = SnapCaret(Maths.Clamp(s_anchor, 0, length), 1);
        }

        // Mojis in the input box are drawn as their picture, the caret can't stop inside one
        private static string s_spriteText;
        private static List<ChatSpriteSpan> s_spriteSpans = new List<ChatSpriteSpan>();
        private static int s_spriteMojiVersion = -1;
        private static int s_spriteRefreshFrame;

        private static List<ChatSpriteSpan> GetInputSprites()
        {
            string current = text;
            // Rechecked now and then too, a moji file can show up while typing
            int spriteKey = DuckFile.mojiVersion * 4 + Options.Data.mojiFilter;
            if (!ReferenceEquals(current, s_spriteText) && current != s_spriteText || s_spriteMojiVersion != spriteKey || s_frame - s_spriteRefreshFrame > 60)
            {
                s_spriteText = current;
                s_spriteMojiVersion = spriteKey;
                s_spriteRefreshFrame = s_frame;
                s_spriteSpans = core._chatFont == null ? new List<ChatSpriteSpan>() : ChatTextLayoutBuilder.FindSprites(core._chatFont, current, null);
            }
            return s_spriteSpans;
        }

        /// <summary>
        /// Moves a caret position out of a moji, towards its end if <paramref name="direction"/> is positive, its start if negative.
        /// </summary>
        private static int SnapCaret(int position, int direction)
        {
            foreach (ChatSpriteSpan span in GetInputSprites())
            {
                if (position > span.start && position < span.end)
                {
                    if (direction == 0)
                        return position - span.start <= span.end - position ? span.start : span.end;
                    return direction < 0 ? span.start : span.end;
                }
            }
            return position;
        }

        /// <summary>The glyph boundary in the input layout for a caret position in the typed text.</summary>
        private static int CaretToBoundary(int caret)
        {
            ChatTextLayout layout = s_inputLayout;
            if (layout == null)
                return s_inputPrefixLength + caret;
            for (int i = s_inputPrefixLength; i < layout.glyphs.Count; i++)
            {
                if (layout.glyphs[i].sourceIndex >= caret)
                    return i;
            }
            return layout.glyphs.Count;
        }

        private static void PushUndo(bool force = false)
        {
            if (!force && s_frame - s_lastUndoFrame < 30 && s_undo.Count > 0)
            {
                s_lastUndoFrame = s_frame;
                return;
            }
            s_lastUndoFrame = s_frame;
            if (s_undo.Count > 0 && s_undo[s_undo.Count - 1].text == text)
                return;
            s_undo.Add((text, s_caret));
            if (s_undo.Count > 100)
                s_undo.RemoveAt(0);
            s_redo.Clear();
        }

        private static void Undo()
        {
            if (s_undo.Count == 0)
                return;
            (string previousText, int previousCaret) = s_undo[s_undo.Count - 1];
            s_undo.RemoveAt(s_undo.Count - 1);
            s_redo.Add((text, s_caret));
            text = previousText;
            s_caret = s_anchor = previousCaret;
            s_lastUndoFrame = -999;
            ClampCaret();
        }

        private static void Redo()
        {
            if (s_redo.Count == 0)
                return;
            (string nextText, int nextCaret) = s_redo[s_redo.Count - 1];
            s_redo.RemoveAt(s_redo.Count - 1);
            s_undo.Add((text, s_caret));
            text = nextText;
            s_caret = s_anchor = nextCaret;
            s_lastUndoFrame = -999;
            ClampCaret();
        }

        private static void DeleteSelection()
        {
            if (!hasInputSelection)
                return;
            int start = selectionStart;
            text = text.Remove(start, selectionEnd - start);
            s_caret = s_anchor = start;
        }

        /// <summary>
        /// Types text at the caret, replacing the selection.
        /// </summary>
        public static void InsertText(string insert)
        {
            if (string.IsNullOrEmpty(insert))
                return;
            insert = insert.Replace("\r\n", "\n").Replace('\r', '\n').Replace('\t', ' ');
            StringBuilder clean = new StringBuilder(insert.Length);
            foreach (char c in insert)
            {
                if (c == '\n' || c >= ' ' && c != (char)127)
                    clean.Append(c);
            }
            insert = clean.ToString();
            PushUndo(insert.Length > 1 || insert == " ");
            DeleteSelection();
            int room = MaxMessageLength - text.Length;
            if (room <= 0)
                return;
            if (insert.Length > room)
                insert = insert.Substring(0, room);
            // Keep the box from growing off the screen
            int newlineRoom = MaxLines - 1 - text.Count(x => x == '\n');
            if (insert.IndexOf('\n') >= 0)
            {
                StringBuilder limited = new StringBuilder(insert.Length);
                foreach (char c in insert)
                {
                    if (c == '\n')
                    {
                        if (newlineRoom <= 0)
                        {
                            limited.Append(' ');
                            continue;
                        }
                        newlineRoom--;
                    }
                    limited.Append(c);
                }
                insert = limited.ToString();
            }
            text = text.Insert(s_caret, insert);
            s_caret += insert.Length;
            s_anchor = s_caret;
            ResetBlink();
        }

        private static bool IsWordChar(char c) => char.IsLetterOrDigit(c) || c == '_' || c == '\'';

        private static int WordLeft(int position)
        {
            string t = text;
            int i = Maths.Clamp(position, 0, t.Length);
            while (i > 0 && char.IsWhiteSpace(t[i - 1]))
                i--;
            if (i > 0 && !IsWordChar(t[i - 1]))
            {
                while (i > 0 && !IsWordChar(t[i - 1]) && !char.IsWhiteSpace(t[i - 1]))
                    i--;
                return i;
            }
            while (i > 0 && IsWordChar(t[i - 1]))
                i--;
            return i;
        }

        private static int WordRight(int position)
        {
            string t = text;
            int i = Maths.Clamp(position, 0, t.Length);
            if (i < t.Length && IsWordChar(t[i]))
            {
                while (i < t.Length && IsWordChar(t[i]))
                    i++;
            }
            else if (i < t.Length && !char.IsWhiteSpace(t[i]))
            {
                while (i < t.Length && !IsWordChar(t[i]) && !char.IsWhiteSpace(t[i]))
                    i++;
            }
            while (i < t.Length && char.IsWhiteSpace(t[i]) && t[i] != '\n')
                i++;
            return i;
        }

        private static int LineStart(int position)
        {
            int newline = position > 0 ? text.LastIndexOf('\n', position - 1) : -1;
            return newline + 1;
        }

        private static int LineEnd(int position)
        {
            int newline = text.IndexOf('\n', position);
            return newline < 0 ? text.Length : newline;
        }

        private static void MoveCaret(int position, bool extend)
        {
            position = Maths.Clamp(position, 0, text.Length);
            s_caret = SnapCaret(position, Math.Sign(position - s_caret));
            if (!extend)
                s_anchor = s_caret;
            ResetBlink();
        }

        private static void ResetBlink() => core.cursorFlash = 15;

        #endregion

        #region Clipboard

        private static string s_lastCopied;
        private static int s_lastCopyFrame = -999;

        /// <summary>
        /// Puts text on the clipboard without holding up the game. On Windows this happens on a background thread;
        /// elsewhere SDL wants the main thread, which is a single quick call.
        /// </summary>
        public static void SetClipboard(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;
            // Holding ctrl+C repeats the key press, no point copying the same thing over and over
            if (value == s_lastCopied && s_frame - s_lastCopyFrame < 30)
                return;
            s_lastCopied = value;
            s_lastCopyFrame = s_frame;
            if (Program.isLinux || Program.IsLinuxD)
            {
                try
                {
                    FNAPlatform.SetClipboardText(value);
                }
                catch (Exception e)
                {
                    DevConsole.Log(e);
                }
                return;
            }
            try
            {
                Thread thread = new Thread(() =>
                {
                    try
                    {
                        // copy: true keeps the text on the clipboard after this thread is gone
                        System.Windows.Forms.Clipboard.SetDataObject(value, true, 10, 50);
                    }
                    catch (Exception)
                    {
                    }
                })
                {
                    IsBackground = true,
                    Name = "Chat clipboard",
                };
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
            catch (Exception e)
            {
                DevConsole.Log(e);
            }
        }

        private static string GetClipboard()
        {
            string result = null;
            try
            {
                Thread thread = new Thread(() =>
                {
                    if (FNAPlatform.HasClipboardText())
                        result = FNAPlatform.GetClipboardText();
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            }
            catch (Exception e)
            {
                DevConsole.Log(e);
            }
            return result;
        }

        /// <summary>
        /// Opens an http(s) link. Follows the Open URLs in Browser option like the rest of DGR: off means the Steam overlay,
        /// but if the overlay can't be used (Steam isn't running or the overlay is turned off) the browser is used instead.
        /// </summary>
        internal static void OpenLink(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri) || uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                return;
            string target = uri.AbsoluteUri;
            if (!DGRSettings.OpenURLsInBrowser && Steam.IsOverlayEnabled())
            {
                Steam.OverlayOpenURL(target);
                return;
            }
            OpenInBrowser(target);
        }

        /// <summary>Opens a link in the default browser, off the main thread since that can take a moment.</summary>
        private static void OpenInBrowser(string target)
        {
            bool linux = Program.isLinux || Program.IsLinuxD;
            try
            {
                Thread thread = new Thread(() =>
                {
                    try
                    {
                        if (linux)
                            Process.Start("xdg-open", "\"" + target.Replace("\"", "%22") + "\"");
                        else
                            Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
                    }
                    catch (Exception)
                    {
                    }
                })
                {
                    IsBackground = true,
                    Name = "Chat link",
                };
                thread.Start();
            }
            catch (Exception e)
            {
                DevConsole.Log(e);
            }
        }

        private static void Paste()
        {
            string clipboard = GetClipboard();
            if (!string.IsNullOrEmpty(clipboard))
            {
                InsertText(clipboard);
                return;
            }
            ChatImages.TryAttachFromClipboard();
        }

        private static void Copy()
        {
            if (hasInputSelection)
            {
                SetClipboard(text.Substring(selectionStart, selectionEnd - selectionStart));
                return;
            }
            string history = GetHistorySelectionText();
            if (!string.IsNullOrEmpty(history))
                SetClipboard(history);
        }

        #endregion

        #region Opening, closing and sending

        /// <summary>Called when the chat box opens.</summary>
        public static void OnOpen()
        {
            text = "";
            s_caret = s_anchor = 0;
            s_undo.Clear();
            s_redo.Clear();
            s_inputQueue.Clear();
            Keyboard.KeyString = "";
            ChatImages.DiscardPendingAttachment();
            ChatImages.attachmentError = null;
            ClearHistorySelection();
            ChatImagePreview.Close();
            s_drag = DragMode.None;
            s_showCursor = false;
            s_mouseOpenPosition = Mouse.positionConsole;
            s_wasOpen = true;
            ResetMojiUI();
            CloseLinkPopup();
            MarkAllRead();
        }

        private static void OnClose()
        {
            s_wasOpen = false;
            s_inputQueue.Clear();
            ClearHistorySelection();
            ChatImagePreview.Close();
            s_drag = DragMode.None;
            hoveredImage = null;
            s_hoveredBlock = null;
            s_hoveredLink = null;
            s_pressedLink = null;
            ResetMojiUI();
            CloseLinkPopup();
            ChatImages.DiscardPendingAttachment();
            if (Keyboard.TextInputOverride == s_textInputHandler)
                Keyboard.TextInputOverride = null;
        }

        /// <summary>
        /// Removes every message from the chat (just locally), used by the clearchat command.
        /// Images that were only kept for those messages are freed, our own sent images can still be downloaded by others.
        /// </summary>
        /// <returns>How many messages were removed.</returns>
        public static int ClearChat()
        {
            int count = core.chatMessages.Count;
            core.chatMessages.Clear();
            s_testQueue.Clear();
            MarkAllRead();
            core.chatMessageOffset = 0;
            ClearHistorySelection();
            s_drawnBubbles.Clear();
            if (s_drag == DragMode.History)
                s_drag = DragMode.None;
            hoveredImage = null;
            s_hoveredBlock = null;
            s_hoveredBlockMessage = null;
            s_hoveredBlockIndex = -1;
            s_hoveredLink = null;
            s_pressedLink = null;
            if (ChatImagePreview.isOpen && !ChatImagePreview.IsShowing(ChatImages.pendingAttachment))
                ChatImagePreview.Close();
            ChatImages.ForgetShownImages();
            return count;
        }

        /// <summary>Called after the message was sent.</summary>
        public static void OnSent()
        {
            s_caret = s_anchor = 0;
            ChatImages.DiscardPendingAttachment();
        }

        public static bool hasSomethingToSend => text.Length > 0 || ChatImages.pendingAttachment != null;

        /// <summary>Whether the "typing" bubble should show over our duck: once something has actually been typed, or whenever the chat is open (Typing Bubble option).</summary>
        public static bool isTyping => core.enteringText && !core.stopEnteringText && (DGRSettings.ChatTypingBubble == 1 || text.Trim().Length > 0 || ChatImages.pendingAttachment != null);

        public static void OnAttachmentChanged()
        {
            ResetBlink();
        }

        /// <summary>
        /// Lets the chat handle escape before it closes the chat box.
        /// </summary>
        /// <returns>True if escape was used up (closing the image preview).</returns>
        public static bool HandleEscape()
        {
            if (linkPopupOpen)
            {
                CloseLinkPopup();
                return true;
            }
            if (s_pickerOpen)
            {
                s_pickerOpen = false;
                return true;
            }
            // Escape hides the suggestions first. The /steal "nothing to steal" note has nothing to pick, so escape just closes the chat.
            if (autocompleteVisible)
            {
                DismissAutocomplete();
                return true;
            }
            return ChatImagePreview.HandleEscape();
        }

        /// <summary>
        /// Called by <see cref="DuckNetwork"/> when the chat key is pressed while typing.
        /// </summary>
        /// <returns>True if the key press was used to add a new line instead of sending.</returns>
        public static bool HandleChatKeyWhileTyping()
        {
            // Enter opens the link in the link popup
            if (linkPopupOpen)
            {
                ActivateLinkPopup(s_linkPopupSelection);
                return true;
            }
            // Enter picks the highlighted moji suggestion, like tab
            if (autocompleteVisible && !Keyboard.shift)
            {
                AcceptAutocomplete();
                return true;
            }
            if (Keyboard.shift && LockMovementQueue.Empty)
            {
                InsertText("\n");
                return true;
            }
            return false;
        }

        #endregion

        #region Update

        /// <summary>
        /// Everything that needs to happen every frame, called from <see cref="DuckNetwork.Update"/>.
        /// </summary>
        public static void Update()
        {
            s_frame++;
            ChatImages.Update();
            UpdateTestMessages();
            UpdateFileDrops(core.enteringText && !core.stopEnteringText);
            if (s_copiedFrames > 0)
                s_copiedFrames--;
            if (s_copiedLinkFrames > 0)
                s_copiedLinkFrames--;
            if (s_favoriteFeedbackFrames > 0)
                s_favoriteFeedbackFrames--;

            bool open = core.enteringText && !core.stopEnteringText;
            if (!open)
            {
                if (s_wasOpen)
                    OnClose();
                return;
            }
            if (!s_wasOpen)
                s_wasOpen = true;

            if (DevConsole.open)
            {
                if (Keyboard.TextInputOverride == s_textInputHandler)
                    Keyboard.TextInputOverride = null;
                return;
            }
            Keyboard.TextInputOverride = s_textInputHandler;

            ChatImagePreview.UpdateKeys();
            UpdateTyping();
            UpdateAutocomplete();
            UpdateMouse();
        }

        private static void UpdateTyping()
        {
            if (linkPopupOpen)
            {
                UpdateLinkPopupKeys();
                return;
            }
            ClampCaret();
            bool control = Keyboard.control;
            bool shift = Keyboard.shift;

            // Characters (and control characters) from the text input event, in the order they were typed
            if (s_inputQueue.Count > 0)
            {
                StringBuilder typed = new StringBuilder();
                void FlushTyped()
                {
                    if (typed.Length == 0)
                        return;
                    InsertText(typed.ToString());
                    typed.Clear();
                }
                foreach (char c in s_inputQueue)
                {
                    switch (c)
                    {
                        case (char)8: // backspace
                            FlushTyped();
                            PushUndo();
                            if (hasInputSelection)
                                DeleteSelection();
                            else if (s_caret > 0)
                            {
                                int from = SnapCaret(control ? WordLeft(s_caret) : s_caret - 1, -1);
                                text = text.Remove(from, s_caret - from);
                                s_caret = s_anchor = from;
                            }
                            ResetBlink();
                            break;
                        case (char)127: // delete
                            FlushTyped();
                            PushUndo();
                            if (hasInputSelection)
                                DeleteSelection();
                            else if (s_caret < text.Length)
                            {
                                int to = SnapCaret(control ? WordRight(s_caret) : s_caret + 1, 1);
                                text = text.Remove(s_caret, to - s_caret);
                            }
                            ResetBlink();
                            break;
                        case (char)2: // home
                            FlushTyped();
                            MoveCaret(control ? 0 : LineStart(s_caret), shift);
                            break;
                        case (char)3: // end
                            FlushTyped();
                            MoveCaret(control ? text.Length : LineEnd(s_caret), shift);
                            break;
                        case (char)22: // ctrl+v
                            FlushTyped();
                            Paste();
                            break;
                        case (char)9:
                        case (char)13:
                            break;
                        default:
                            if (c >= ' ')
                                typed.Append(c);
                            break;
                    }
                }
                FlushTyped();
                s_inputQueue.Clear();
            }

            // IME and anything else that still writes into KeyString
            if (!string.IsNullOrEmpty(Keyboard.KeyString))
            {
                InsertText(Keyboard.KeyString);
                Keyboard.KeyString = "";
            }

            if (Keyboard.Pressed(Keys.Left))
            {
                if (hasInputSelection && !shift)
                    MoveCaret(control ? WordLeft(selectionStart) : selectionStart, false);
                else
                    MoveCaret(control ? WordLeft(s_caret) : s_caret - 1, shift);
            }
            if (Keyboard.Pressed(Keys.Right))
            {
                if (hasInputSelection && !shift)
                    MoveCaret(control ? WordRight(selectionEnd) : selectionEnd, false);
                else
                    MoveCaret(control ? WordRight(s_caret) : s_caret + 1, shift);
            }
            if (Keyboard.Pressed(Keys.Up) || Keyboard.Pressed(Keys.Down))
            {
                int direction = Keyboard.Pressed(Keys.Up) ? -1 : 1;
                if (autocompleteVisible)
                    MoveAutocompleteSelection(direction);
                else
                    MoveCaretVertically(direction, shift);
            }
            if (Keyboard.Pressed(Keys.Tab) && autocompleteVisible)
                AcceptAutocomplete();

            // AltGr shows up as ctrl+alt on Windows, don't eat characters typed with it
            if (control && !Keyboard.alt)
            {
                if (Keyboard.Pressed(Keys.A))
                {
                    s_anchor = 0;
                    s_caret = text.Length;
                    ClearHistorySelection();
                }
                if (Keyboard.Pressed(Keys.C) && !s_copyKeyHeld)
                    Copy();
                if (Keyboard.Pressed(Keys.X) && hasInputSelection && !s_copyKeyHeld)
                {
                    Copy();
                    PushUndo(true);
                    DeleteSelection();
                }
                if (Keyboard.Pressed(Keys.Z))
                {
                    if (shift)
                        Redo();
                    else
                        Undo();
                }
                if (Keyboard.Pressed(Keys.Y))
                    Redo();
            }
            s_copyKeyHeld = Keyboard.Down(Keys.C) || Keyboard.Down(Keys.X);
            ClampCaret();
        }

        private static bool s_copyKeyHeld;

        private static void MoveCaretVertically(int direction, bool extend)
        {
            ChatTextLayout layout = s_inputLayout;
            if (layout == null || layout.lines.Count <= 1)
            {
                MoveCaret(direction < 0 ? 0 : text.Length, extend);
                return;
            }
            Vec2 caret = layout.GetCaretPosition(CaretToBoundary(s_caret));
            Vec2 target = new Vec2(caret.x + 0.5f, caret.y + layout.lineHeight * 0.5f + direction * layout.lineHeight);
            if (target.y < 0f)
            {
                MoveCaret(0, extend);
                return;
            }
            if (target.y > layout.height)
            {
                MoveCaret(text.Length, extend);
                return;
            }
            MoveCaret(InputBoundaryToCaret(layout.GetBoundaryAt(target)), extend);
        }

        private static int InputBoundaryToCaret(int boundary)
        {
            if (s_inputLayout == null)
                return 0;
            if (boundary >= s_inputLayout.glyphs.Count)
                return text.Length;
            ChatGlyph glyph = s_inputLayout.glyphs[Math.Max(boundary, 0)];
            if (glyph.sourceIndex < 0)
                return 0;
            return Math.Min(glyph.sourceIndex, text.Length);
        }

        private static void UpdateMouse()
        {
            Vec2 mouse = Mouse.positionConsole;
            if ((mouse - s_mouseOpenPosition).lengthSq > 4f || Mouse.left == InputState.Pressed)
                s_showCursor = true;
            s_mousePosition = mouse;

            bool pressed = Mouse.left == InputState.Pressed;
            bool down = Mouse.left == InputState.Down || pressed;

            // The link popup takes over the mouse while it's up
            if (linkPopupOpen)
            {
                s_drag = DragMode.None;
                hoveredImage = null;
                s_hoveredLink = null;
                s_tooltip = null;
                s_guideHovered = false;
                UpdateLinkPopupMouse(mouse, pressed);
                return;
            }

            // The image preview takes over the mouse while it's open
            if (ChatImagePreview.isOpen)
            {
                s_cursorKind = (CursorKind)ChatImagePreview.UpdateMouse(mouse);
                hoveredImage = null;
                return;
            }

            if (pressed)
            {
                if (s_frame - s_lastClickFrame <= DoubleClickFrames && (mouse - s_lastClickPosition).lengthSq < 16f)
                    s_clickCount++;
                else
                    s_clickCount = 1;
                s_lastClickFrame = s_frame;
                s_lastClickPosition = mouse;
            }

            // Dragging a selection
            if (s_drag != DragMode.None)
            {
                s_cursorKind = s_pressedLink != null ? CursorKind.Hand : CursorKind.Text;
                if (!down)
                {
                    s_drag = DragMode.None;
                    // A click (not a drag) on a link opens it
                    if (s_pressedLink != null && (mouse - s_pressedLinkPosition).lengthSq < 9f)
                    {
                        ClearHistorySelection();
                        RequestOpenLink(s_pressedLink);
                    }
                    s_pressedLink = null;
                }
                else if (s_drag == DragMode.Input && s_inputLayout != null)
                {
                    s_caret = InputBoundaryToCaret(s_inputLayout.GetBoundaryAt(mouse - s_inputTextOrigin));
                    if (s_wordAnchorFrom >= 0)
                        ExtendWordSelection(ref s_anchor, ref s_caret, s_wordAnchorFrom, s_wordAnchorTo);
                    ResetBlink();
                }
                else if (s_drag == DragMode.History)
                {
                    if (s_pressedLink != null && (mouse - s_pressedLinkPosition).lengthSq >= 9f)
                        s_pressedLink = null;
                    DrawnBubble bubble = FindBubbleForDrag(mouse);
                    if (bubble != null)
                    {
                        s_selectionFocusMessage = bubble.message;
                        s_selectionFocus = bubble.layout.GetBoundaryAt(mouse - bubble.textOrigin);
                        if (s_wordAnchorFrom >= 0)
                        {
                            if (bubble.message == s_selectionAnchorMessage)
                                ExtendWordSelection(ref s_selectionAnchor, ref s_selectionFocus, s_wordAnchorFrom, s_wordAnchorTo);
                            else
                            {
                                // Dragged into another message: keep the whole word/message the drag started on selected
                                bool downwards = GetOrder(bubble.message) > GetOrder(s_selectionAnchorMessage);
                                s_selectionAnchor = downwards ? s_wordAnchorFrom : s_wordAnchorTo;
                            }
                        }
                    }
                }
                return;
            }

            hoveredImage = null;
            s_hoveredBlock = null;
            s_hoveredBlockMessage = null;
            s_hoveredBlockIndex = -1;
            s_cursorKind = CursorKind.Arrow;
            s_tooltip = null;
            s_guideHovered = false;
            s_hoveredLink = null;

            // Moji picker and suggestions sit on top of everything else
            if (UpdateMojiMouse(mouse, pressed))
                return;

            // Buttons on the input box
            if (s_inputDrawn && s_guideButton.Contains(mouse))
            {
                s_guideHovered = true;
                return;
            }
            if (s_inputDrawn && s_mojiButton.Contains(mouse))
            {
                s_cursorKind = CursorKind.Hand;
                if (!s_pickerOpen)
                    s_tooltip = "Mojis (or type : and a name)";
                if (pressed)
                    TogglePicker();
                return;
            }
            if (s_inputDrawn && ChatImages.sendingEnabled && s_imageButton.Contains(mouse))
            {
                s_cursorKind = CursorKind.Hand;
                s_tooltip = ChatImages.preparingAttachment ? "Loading image..." : "Attach an image (or paste one with Ctrl+V)";
                if (pressed)
                    ChatImages.OpenFilePicker();
                return;
            }
            if (ChatImages.pendingAttachment != null && s_removeAttachmentButton.Contains(mouse))
            {
                s_cursorKind = CursorKind.Hand;
                s_tooltip = "Remove image";
                if (pressed)
                    ChatImages.DiscardPendingAttachment();
                return;
            }
            if (ChatImages.pendingAttachment != null && s_attachmentThumbnail.Contains(mouse))
            {
                s_cursorKind = CursorKind.Hand;
                s_tooltip = "Click to view";
                if (pressed)
                    ChatImagePreview.Open(ChatImages.pendingAttachment);
                return;
            }

            // Input text
            if (s_inputDrawn && s_inputLayout != null && s_inputBounds.Contains(mouse))
            {
                s_cursorKind = CursorKind.Text;
                int hoveredGlyph = s_inputLayout.GetGlyphAt(mouse - s_inputTextOrigin);
                if (hoveredGlyph >= s_inputPrefixLength)
                {
                    // alt+click a moji to (un)favorite it
                    string mojiName = pressed && Keyboard.alt ? GetUsableMojiName(s_inputLayout.glyphs[hoveredGlyph]) : null;
                    if (mojiName != null)
                    {
                        ToggleFavorite(mojiName);
                        s_tooltip = GetSpriteTooltip(s_inputLayout.glyphs[hoveredGlyph]);
                        return;
                    }
                    s_tooltip = GetSpriteTooltip(s_inputLayout.glyphs[hoveredGlyph]);
                }
                if (pressed)
                {
                    ClearHistorySelection();
                    int caret = InputBoundaryToCaret(s_inputLayout.GetBoundaryAt(mouse - s_inputTextOrigin));
                    s_wordAnchorFrom = s_wordAnchorTo = -1;
                    if (s_clickCount >= 3)
                    {
                        // Triple click selects the line, which stays selected while dragging afterwards
                        s_anchor = LineStart(caret);
                        s_caret = LineEnd(caret);
                        s_wordAnchorFrom = s_anchor;
                        s_wordAnchorTo = s_caret;
                    }
                    else if (s_clickCount == 2)
                    {
                        int glyph = s_inputLayout.GetGlyphAt(mouse - s_inputTextOrigin);
                        if (glyph >= 0)
                        {
                            s_inputLayout.GetWordAt(glyph, out int from, out int to);
                            s_anchor = InputBoundaryToCaret(from);
                            s_caret = InputBoundaryToCaret(to);
                            s_wordAnchorFrom = s_anchor;
                            s_wordAnchorTo = s_caret;
                        }
                    }
                    else
                    {
                        s_caret = caret;
                        if (!Keyboard.shift)
                            s_anchor = caret;
                    }
                    ResetBlink();
                    s_drag = DragMode.Input;
                }
                return;
            }

            // Chat history
            foreach (DrawnBubble bubble in s_drawnBubbles)
            {
                if (!bubble.bounds.Contains(mouse))
                    continue;
                Vec2 local = mouse - bubble.textOrigin;
                ChatTextLayout layout = bubble.layout;

                ChatCodeBlock block = layout.GetBlockAt(local);
                if (block != null)
                {
                    s_hoveredBlock = block;
                    s_hoveredBlockMessage = bubble.message;
                    s_hoveredBlockIndex = layout.blocks.IndexOf(block);
                    s_hoveredBlockCopyButton = GetCopyButtonRect(layout, block, bubble.textOrigin);
                    if (s_hoveredBlockCopyButton.Contains(mouse))
                    {
                        s_cursorKind = CursorKind.Hand;
                        if (pressed)
                        {
                            SetClipboard(block.content);
                            s_copiedBlockContent = block.content;
                            s_copiedFrames = 90;
                        }
                        return;
                    }
                }

                int glyphIndex = layout.GetGlyphAt(local);
                if (glyphIndex >= 0 && layout.glyphs[glyphIndex].kind == ChatGlyphKind.Image)
                {
                    ChatImageAttachment image = layout.glyphs[glyphIndex].image;
                    hoveredImage = image;
                    ChatImages.Refresh(image);
                    s_tooltip = ChatImages.GetTooltip(image);
                    if (ChatImages.IsClickable(image))
                    {
                        s_cursorKind = CursorKind.Hand;
                        if (pressed)
                        {
                            ClearHistorySelection();
                            if (image.state == ChatImageState.Ready)
                                ChatImagePreview.Open(image);
                            else
                                ChatImages.Request(image);
                        }
                        return;
                    }
                }

                ChatLink link = glyphIndex >= 0 ? layout.glyphs[glyphIndex].link : null;
                if (link != null)
                {
                    s_hoveredLink = link;
                    // Right click copies the link
                    if (Mouse.right == InputState.Pressed)
                    {
                        SetClipboard(link.url);
                        s_copiedLink = link;
                        s_copiedLinkFrames = 90;
                    }
                    if (s_copiedLink == link && s_copiedLinkFrames > 0)
                        s_tooltip = "Link copied";
                    else
                        s_tooltip = (link.url.Length > 60 ? link.url.Substring(0, 57) + "..." : link.url) + "  (right-click to copy)";
                }
                else if (glyphIndex >= 0)
                {
                    // alt+click a moji to (un)favorite it, if it's one we can use
                    string mojiName = pressed && Keyboard.alt ? GetUsableMojiName(layout.glyphs[glyphIndex]) : null;
                    if (mojiName != null)
                    {
                        ToggleFavorite(mojiName);
                        s_tooltip = GetSpriteTooltip(layout.glyphs[glyphIndex]);
                        return;
                    }
                    s_tooltip = GetSpriteTooltip(layout.glyphs[glyphIndex]);
                }
                s_cursorKind = link != null ? CursorKind.Hand : CursorKind.Text;
                if (pressed)
                {
                    s_pressedLink = s_clickCount == 1 && !Keyboard.shift ? link : null;
                    s_pressedLinkPosition = mouse;
                    int boundary = layout.GetBoundaryAt(local);
                    s_wordAnchorFrom = s_wordAnchorTo = -1;
                    if (s_clickCount >= 3)
                    {
                        s_selectionAnchorMessage = s_selectionFocusMessage = bubble.message;
                        s_selectionAnchor = 0;
                        s_selectionFocus = layout.glyphs.Count;
                        // Keeps the whole message selected while the mouse is still held after the third click
                        s_wordAnchorFrom = 0;
                        s_wordAnchorTo = layout.glyphs.Count;
                    }
                    else if (s_clickCount == 2 && glyphIndex >= 0)
                    {
                        layout.GetWordAt(glyphIndex, out int from, out int to);
                        s_selectionAnchorMessage = s_selectionFocusMessage = bubble.message;
                        s_selectionAnchor = from;
                        s_selectionFocus = to;
                        s_wordAnchorFrom = from;
                        s_wordAnchorTo = to;
                    }
                    else if (Keyboard.shift && s_selectionAnchorMessage != null)
                    {
                        s_selectionFocusMessage = bubble.message;
                        s_selectionFocus = boundary;
                    }
                    else
                    {
                        s_selectionAnchorMessage = s_selectionFocusMessage = bubble.message;
                        s_selectionAnchor = s_selectionFocus = boundary;
                    }
                    s_anchor = s_caret;
                    s_drag = DragMode.History;
                }
                return;
            }

            // Clicked on nothing
            if (pressed)
                ClearHistorySelection();
        }

        /// <summary>A moji's (or other sprite's) name, shown when hovering it. Null for anything else.</summary>
        private static string GetSpriteTooltip(ChatGlyph glyph)
        {
            if (glyph == null || glyph.kind != ChatGlyphKind.Sprite || string.IsNullOrEmpty(glyph.copyText))
                return null;
            string name = glyph.copyText;
            if (name.Length > 40)
                name = name.Substring(0, 37) + "...";
            string mojiName = GetUsableMojiName(glyph);
            if (mojiName == null)
                return name;
            string feedback = GetFavoriteFeedback(mojiName);
            return feedback != null ? name + "  " + feedback : IsFavorite(mojiName) ? name + "  (favorite)" : name;
        }

        /// <summary>
        /// Dragging after a double/triple click keeps the word/line that was clicked selected and extends from it, like most text editors.
        /// </summary>
        /// <param name="wordFrom">Start of the clicked word/line (caret position in the input box, glyph boundary in history).</param>
        private static void ExtendWordSelection(ref int anchor, ref int focus, int wordFrom, int wordTo)
        {
            int from = wordFrom;
            int to = wordTo;
            if (focus < from)
                anchor = to;
            else
            {
                anchor = from;
                if (focus < to)
                    focus = to;
            }
        }

        private static DrawnBubble FindBubbleForDrag(Vec2 mouse)
        {
            if (s_drawnBubbles.Count == 0)
                return null;
            DrawnBubble best = null;
            float bestDistance = float.MaxValue;
            foreach (DrawnBubble bubble in s_drawnBubbles)
            {
                float distance = 0f;
                if (mouse.y < bubble.bounds.Top)
                    distance = bubble.bounds.Top - mouse.y;
                else if (mouse.y > bubble.bounds.Bottom)
                    distance = mouse.y - bubble.bounds.Bottom;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = bubble;
                }
            }
            return best;
        }

        #endregion

        #region History selection

        private static void ClearHistorySelection()
        {
            s_selectionAnchorMessage = s_selectionFocusMessage = null;
            s_selectionAnchor = s_selectionFocus = 0;
            s_wordAnchorFrom = s_wordAnchorTo = -1;
        }

        /// <summary>Top to bottom order of a message in the chat, -1 if it's gone.</summary>
        private static int GetOrder(ChatMessage message)
        {
            int index = core.chatMessages.IndexOf(message);
            return index < 0 ? -1 : core.chatMessages.Count - 1 - index;
        }

        private static bool GetHistorySelection(out int startOrder, out int startPosition, out int endOrder, out int endPosition)
        {
            startOrder = endOrder = startPosition = endPosition = 0;
            if (s_selectionAnchorMessage == null || s_selectionFocusMessage == null)
                return false;
            int anchorOrder = GetOrder(s_selectionAnchorMessage);
            int focusOrder = GetOrder(s_selectionFocusMessage);
            if (anchorOrder < 0 || focusOrder < 0)
            {
                ClearHistorySelection();
                return false;
            }
            if (anchorOrder < focusOrder || anchorOrder == focusOrder && s_selectionAnchor <= s_selectionFocus)
            {
                startOrder = anchorOrder;
                startPosition = s_selectionAnchor;
                endOrder = focusOrder;
                endPosition = s_selectionFocus;
            }
            else
            {
                startOrder = focusOrder;
                startPosition = s_selectionFocus;
                endOrder = anchorOrder;
                endPosition = s_selectionAnchor;
            }
            return !(startOrder == endOrder && startPosition == endPosition);
        }

        private static void GetSelectionRange(ChatMessage message, int glyphCount, out int from, out int to)
        {
            from = to = -1;
            if (!GetHistorySelection(out int startOrder, out int startPosition, out int endOrder, out int endPosition))
                return;
            int order = GetOrder(message);
            if (order < startOrder || order > endOrder)
                return;
            from = order == startOrder ? startPosition : 0;
            to = order == endOrder ? endPosition : glyphCount;
        }

        private static string GetHistorySelectionText()
        {
            if (!GetHistorySelection(out int startOrder, out _, out int endOrder, out _))
                return null;
            StringBuilder builder = new StringBuilder();
            List<ChatMessage> messages = core.chatMessages;
            for (int order = startOrder; order <= endOrder; order++)
            {
                ChatMessage message = messages[messages.Count - 1 - order];
                if (message.layout == null)
                    continue;
                GetSelectionRange(message, message.layout.glyphs.Count, out int from, out int to);
                if (from < 0)
                    continue;
                if (builder.Length > 0)
                    builder.Append('\n');
                builder.Append(message.layout.GetCopyText(from, to));
            }
            return builder.ToString();
        }

        #endregion

        #region Layout

        private static float headScale => Resolution.current.x > 1920 ? 2f : 1f;

        /// <summary>
        /// The built in pixel font follows the Chat Font Size option too (vanilla only applied it to system fonts),
        /// the default of 18 being the original size.
        /// </summary>
        private static float builtInFontScale => Maths.Clamp(Options.Data.chatFontSize, 8, 80) / 18f;

        private static float ratioFix => Level.current is RockScoreboard ? 1f / (Resolution.current.aspect / 1.777777778f) : 1f;

        private static float headOffset => 10f * (Options.Data.chatHeadScale + 1) * headScale;

        private static FancyBitmapFont SetupFont(float messageScale = 1f)
        {
            FancyBitmapFont font = core._chatFont;
            if (font is RasterFont)
                font.scale = new Vec2(0.5f);
            else
                font.scale = new Vec2(headScale * builtInFontScale) * messageScale;
            font.scale *= new Vec2(1f, ratioFix);
            return font;
        }

        /// <summary>How much of the screen the chat (messages and the typing box) can take up, a quarter up to all of it (Chat Width option).
        /// They start 14 in from the left, at full width they stop as far in from the right.</summary>
        private static float chatAreaWidth => Math.Min(Layer.Console.width * (Maths.Clamp(DGRSettings.ChatMaxWidth, 1, 4) / 4f), Layer.Console.width - 28f);

        /// <summary>How wide chat messages can get before they wrap.</summary>
        private static float messageWrapWidth => Math.Max(160f, chatAreaWidth - headOffset - 8f * DuckNetwork.chatScale);

        private static float imageMaxWidth => Math.Min(messageWrapWidth, Layer.Console.width * 0.35f);

        private static float imageMaxHeight => Layer.Console.height * 0.3f;

        /// <summary>Messages longer than this are cut short until the chat is opened.</summary>
        public const int CollapsedLength = 100;
        private const int CollapsedLines = 3;

        // The Chat Rendering option, which special parts of chat text get drawn
        private const int RenderFormattingFlag = 1;
        private const int RenderColorsFlag = 2;
        private static int chatRendering => DGRSettings.ChatRendering & (RenderFormattingFlag | RenderColorsFlag);
        private static bool renderFormatting => (chatRendering & RenderFormattingFlag) != 0;
        private static bool renderColors => (chatRendering & RenderColorsFlag) != 0;

        /// <summary>Splits the DGR badge (@DGRBIG...@) off the end of a display name, it's the game's and not part of what the player wrote.</summary>
        private static void SplitNameBadge(string full, out string name, out string badge)
        {
            full = full ?? "";
            int at = full.LastIndexOf("@DGRBIG", StringComparison.Ordinal);
            if (at >= 0 && full.EndsWith("@"))
            {
                name = full.Substring(0, at);
                badge = full.Substring(at);
            }
            else
            {
                name = full;
                badge = "";
            }
        }

        private static ChatTextLayout BuildMessageLayout(List<ChatEntry> entries, FancyBitmapFont font, float wrapWidth, bool collapsed = false)
        {
            ChatTextLayoutBuilder builder = new ChatTextLayoutBuilder(font, wrapWidth);
            for (int i = 0; i < entries.Count; i++)
            {
                ChatEntry entry = entries[i];
                if (entry.who == null)
                    continue;
                builder.mojiConnection = entry.who.connection == DuckNetwork.localConnection ? null : entry.who.connection;
                if (builder.count > 0)
                    builder.AppendBreak();
                builder.ResetStyle();

                // Name: the color and badges come from the game, the name itself follows the Chat Rendering option
                SplitNameBadge(entry.who.nameUIBodge, out string name, out string badge);
                builder.links = false;
                builder.AppendFormatted((i == 0 && entry.who.slotType == SlotType.Spectator ? "@SPECTATORBIG@" : "") + (i == 0 ? "|WHITE|" : "|GRAY|"), false, false);
                builder.colors = renderColors;
                builder.AppendFormatted(name, false, false);
                builder.colors = true;
                builder.AppendFormatted(badge + ": ", false, false);
                builder.ResetStyle();

                // Message: anything turned off is left out, like a vanilla player would see it
                builder.links = true;
                builder.colors = renderColors;
                if (collapsed)
                    builder.BeginLimit(CollapsedLength, CollapsedLines);
                if (renderFormatting)
                    builder.AppendFormatted(entry.text);
                else
                    builder.AppendFormatted(ChatFormatting.StripForVanilla(entry.text), true, false);
                if (collapsed)
                    builder.EndLimit();
                builder.colors = true;
                if (entry.image != null)
                {
                    Vec2 size = ChatImages.GetDisplaySize(entry.image, font, Math.Min(imageMaxWidth, wrapWidth), imageMaxHeight);
                    builder.AppendImage(entry.image, size.x, size.y);
                }
            }
            return builder.Build();
        }

        private static ChatTextLayout EnsureLayout(ChatMessage message, FancyBitmapFont font, float wrapWidth, bool collapsed)
        {
            StringBuilder key = new StringBuilder();
            key.Append(collapsed ? 'c' : 'f').Append(DuckFile.mojiVersion).Append('|').Append(Options.Data.mojiFilter).Append('|').Append(chatRendering).Append('|').Append(font.GetHashCode()).Append('|').Append(font.scale.x).Append('|').Append(font.scale.y).Append('|').Append(wrapWidth).Append('|').Append(message.entries.Count);
            foreach (ChatEntry entry in message.entries)
            {
                if (entry.image != null)
                    key.Append('|').Append((int)entry.image.state).Append(':').Append(entry.image.version);
            }
            string layoutKey = key.ToString();
            message.layoutAge++;
            if (message.layout == null || message.layoutKey != layoutKey || message.layoutAge > 120)
            {
                try
                {
                    message.layout = BuildMessageLayout(message.entries, font, wrapWidth, collapsed);
                }
                catch (Exception e)
                {
                    DevConsole.Log(e);
                    ChatTextLayoutBuilder fallback = new ChatTextLayoutBuilder(font, wrapWidth);
                    fallback.AppendLiteral("??????", 0, false);
                    message.layout = fallback.Build();
                }
                message.layoutKey = layoutKey;
                message.layoutAge = 0;
            }
            return message.layout;
        }

        /// <summary>How many lines these entries take up in a chat bubble right now.</summary>
        public static int CountLines(List<ChatEntry> entries)
        {
            if (entries == null || entries.Count == 0 || core._chatFont == null)
                return 0;
            FancyBitmapFont font = SetupFont();
            return BuildMessageLayout(entries, font, messageWrapWidth).lines.Count;
        }

        /// <summary>Whether a message can be tacked onto the previous chat bubble (vanilla does this for short messages).</summary>
        public static bool CanMergeInto(ChatMessage previous, ChatEntry entry)
        {
            if (entry.image != null || previous.entries.Count == 0)
                return false;
            foreach (ChatEntry previousEntry in previous.entries)
            {
                if (previousEntry.image != null || HasCodeBlock(previousEntry.text))
                    return false;
            }
            return !HasCodeBlock(entry.text);
        }

        private static bool HasCodeBlock(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            for (int i = value.IndexOf('|'); i >= 0; i = value.IndexOf('|', i + 1))
            {
                if (ChatFormatting.TryMatchTag(value, i, out ChatFormatting.Tag tag, out _) && tag.kind == ChatFormatting.TagKind.CodeBlock)
                    return true;
            }
            return false;
        }

        #endregion

        #region Drawing

        /// <summary>
        /// Draws the chat, called from <see cref="DuckNetwork.Draw"/> on the console layer.
        /// </summary>
        public static void Draw()
        {
            s_drawnBubbles.Clear();
            s_inputDrawn = false;
            // The chat draws above the console's depths, so it would show through the console. Hide it while that's open.
            if (core._chatFont == null || DevConsole.open)
                return;

            bool open = core.enteringText && !core.stopEnteringText;
            if (open && ChatImagePreview.isOpen)
            {
                ChatImagePreview.Draw(SetupFont());
                DrawCursor();
                return;
            }

            Vec2 screen = new Vec2(Layer.Console.width, Layer.Console.height);
            float chatScale = DuckNetwork.chatScale;
            float alpha = Options.Data.chatOpacity / 100f;
            float offsetY = 0f;

            if (open)
                offsetY = DrawInput(screen, alpha);

            // Minimal Chat: just unread counters until the chat is opened
            if (!core.enteringText && minimalChat)
            {
                DrawUnreadCounters(screen, alpha);
                return;
            }

            float bubbleDepth = 0.1f;
            int index = -1;
            int drawn = 0;
            float wrapWidth = messageWrapWidth;
            foreach (ChatMessage chatMessage in core.chatMessages)
            {
                index++;
                if (chatMessage.who == null)
                    continue;
                // While the chat is open everything shows at full opacity, but the fade keeps its progress
                // (vanilla reset it to 1 here, which made every old message fade out again after closing the chat)
                bool showAll = core.enteringText;
                if (showAll)
                {
                    if (core.chatMessageOffset > index)
                        continue;
                }
                else if (chatMessage.alpha < 0f)
                    continue;

                float head = headOffset;
                float messageScale = core._chatFont is RasterFont ? 1f : chatMessage.scale;
                FancyBitmapFont font = SetupFont(messageScale);
                // Long messages only show in full while the chat is open, so they don't take over the screen
                bool collapsed = !core.enteringText;
                ChatTextLayout layout = EnsureLayout(chatMessage, font, wrapWidth * messageScale, collapsed);

                float width = layout.width + head + 8f * chatScale;
                float height = layout.height;
                Vec2 p1 = new Vec2(14f, offsetY + (screen.y - (height + 10f)));
                Vec2 p2 = p1 + new Vec2(width, height);
                if (p2.y < 0f)
                    break;

                float messageAlpha = (showAll ? 1f : Math.Min(chatMessage.alpha, 1f)) * alpha;
                Graphics.DrawRect(p1 + new Vec2(-1f, -1f), p2 + new Vec2(1f, 1f), Color.Black * 0.8f * messageAlpha, D(bubbleDepth - 0.0017f), false, 1f * chatScale);

                float pop = 0.3f + chatMessage.text.Length * 0.007f;
                if (pop > 0.5f)
                    pop = 0.5f;
                if (MonoMain.UpdateLerpState)
                {
                    if (chatMessage.slide > 0.8f)
                        chatMessage.scale = Lerp.FloatSmooth(chatMessage.scale, 1f, 0.1f, 1.1f);
                    else if (chatMessage.slide > 0.5f)
                        chatMessage.scale = Lerp.FloatSmooth(chatMessage.scale, 1f + pop, 0.1f, 1.1f);
                    chatMessage.slide = Lerp.FloatSmooth(chatMessage.slide, 1f, 0.1f, 1.1f);
                }

                Color color = DrawChatHead(chatMessage.who, p1, messageAlpha, bubbleDepth, out bool hasPersona);
                if (hasPersona)
                {
                    color *= 0.85f;
                    color.a = byte.MaxValue;
                }
                Graphics.DrawRect(p1, p2, color * 0.85f * messageAlpha, D(bubbleDepth - 1f / 500f));

                Vec2 textOrigin = p1 + new Vec2(2f + head, 1f * font.scale.y);
                GetSelectionRange(chatMessage, layout.glyphs.Count, out int from, out int to);
                font.symbolYOffset = 4f;
                ChatCodeBlock hoveredBlock = open && s_hoveredBlockMessage == chatMessage && s_hoveredBlockIndex >= 0 && s_hoveredBlockIndex < layout.blocks.Count ? layout.blocks[s_hoveredBlockIndex] : null;
                layout.Draw(textOrigin, Color.Black, messageAlpha, 1f, from, to, hoveredBlock, open ? s_hoveredLink : null);

                if (open)
                {
                    s_drawnBubbles.Add(new DrawnBubble
                    {
                        message = chatMessage,
                        layout = layout,
                        textOrigin = textOrigin,
                        bounds = new Rectangle(p1.x - 1f, p1.y - 1f, p2.x - p1.x + 2f, p2.y - p1.y + 2f),
                    });
                }

                offsetY -= height + 4f;
                bubbleDepth -= 0.01f;
                if (++drawn >= MaxMessagesDrawn)
                    break;
            }
            SetupFont();

            if (open)
            {
                if (linkPopupOpen)
                    DrawLinkPopup();
                else if (s_guideHovered && s_inputDrawn)
                    DrawFormatGuide();
                else
                {
                    if (s_inputDrawn)
                        DrawMojiPopups();
                    if (s_tooltip != null && s_showCursor)
                        DrawTooltip(s_tooltip);
                }
                DrawCursor();
            }
        }

        // Drawn above everything else in the chat (text sits at depth 1)
        private const float OverlayDepth = 1.2f;

        private static void DrawTooltip(string tooltip)
        {
            FancyBitmapFont font = SetupFont();
            float padding = 3f * font.scale.x;
            // Drawn as plain text so a moji's name (":skull:") or a link isn't turned back into a picture or color
            float width = MeasurePlain(font, tooltip) + padding * 2f;
            float height = font.characterHeight * font.scale.y + padding * 2f;
            Vec2 position = Mouse.positionConsole + new Vec2(10f, 12f) * headScale;
            // Keep it on screen
            if (position.x + width > Layer.Console.width - 2f)
                position.x = Layer.Console.width - 2f - width;
            if (position.y + height > Layer.Console.height - 2f)
                position.y = Mouse.positionConsole.y - height - 4f;
            position.x = Math.Max(2f, position.x);
            position.y = Math.Max(2f, position.y);
            Graphics.DrawRect(position, position + new Vec2(width, height), new Color(24, 24, 24) * 0.95f, D(OverlayDepth));
            Graphics.DrawRect(position, position + new Vec2(width, height), Color.White * 0.35f, D(OverlayDepth + 0.01f), false, 1f);
            DrawPlain(font, tooltip, position.x + padding, position.y + padding, Color.White, OverlayDepth + 0.05f);
        }

        private static readonly string[][] s_formatGuide =
        {
            new[] { "|B|  |BOLD|", "|B|Bold|B|" },
            new[] { "|I|  |ITALIC|", "|I|Italic|I|" },
            new[] { "|U|  |UNDERLINE|", "|U|Underline|U|" },
            new[] { "|S|  |STRIKE|", "|S|Strikethrough|S|" },
            new[] { "|C|  |CODE|", "|C|Code|C|, shown as typed" },
            new[] { "|CB|  |CODEBLOCK|", "Code block, on its own lines" },
            new[] { "|R|  |RESET|", "Normal style and color" },
            new[] { "|RED|  |BLUE|  ...", "|RED|Colors|R|" },
        };

        /// <summary>The formatting cheat sheet shown while hovering the Aa button.</summary>
        private static void DrawFormatGuide()
        {
            FancyBitmapFont font = SetupFont();
            float padding = 4f * font.scale.x;
            float gap = 8f * font.scale.x;
            List<ChatTextLayout> tags = new List<ChatTextLayout>();
            List<ChatTextLayout> examples = new List<ChatTextLayout>();
            float tagWidth = 0f;
            float exampleWidth = 0f;
            foreach (string[] row in s_formatGuide)
            {
                ChatTextLayoutBuilder tagBuilder = new ChatTextLayoutBuilder(font, 10000f);
                tagBuilder.AppendLiteral(row[0], 0, false);
                ChatTextLayout tag = tagBuilder.Build();
                ChatTextLayoutBuilder exampleBuilder = new ChatTextLayoutBuilder(font, 10000f);
                exampleBuilder.AppendFormatted(row[1]);
                ChatTextLayout example = exampleBuilder.Build();
                tags.Add(tag);
                examples.Add(example);
                tagWidth = Math.Max(tagWidth, tag.width);
                exampleWidth = Math.Max(exampleWidth, example.width);
            }
            string title = "FORMATTING";
            string footer = "Tags turn a style on and off. Shift+Enter for a new line";
            float lineHeight = (font.characterHeight + 2) * font.scale.y;
            float width = Math.Max(tagWidth + gap + exampleWidth, Math.Max(font.GetWidth(title), font.GetWidth(footer))) + padding * 2f;
            float height = lineHeight * (s_formatGuide.Length + 2) + padding * 2f + 4f;
            Vec2 position = new Vec2(14f, s_inputTop - height - 6f);
            if (position.y < 2f)
                position.y = 2f;

            Graphics.DrawRect(position, position + new Vec2(width, height), new Color(24, 24, 24) * 0.95f, D(OverlayDepth));
            Graphics.DrawRect(position, position + new Vec2(width, height), Color.White * 0.35f, D(OverlayDepth + 0.01f), false, 1f);
            float y = position.y + padding;
            font.Draw(title, position.x + padding, y, Color.White, D(OverlayDepth + 0.05f));
            y += lineHeight + 2f;
            for (int i = 0; i < tags.Count; i++)
            {
                tags[i].Draw(new Vec2(position.x + padding, y), new Color(170, 170, 170), 1f, OverlayDepth + 0.15f);
                examples[i].Draw(new Vec2(position.x + padding + tagWidth + gap, y), Color.White, 1f, OverlayDepth + 0.15f);
                y += lineHeight;
            }
            y += 2f;
            font.Draw(footer, position.x + padding, y, new Color(170, 170, 170), D(OverlayDepth + 0.05f));
        }

        private static Color DrawChatHead(Profile who, Vec2 p1, float alpha, float depth, out bool hasPersona)
        {
            Color color = Color.White;
            hasPersona = false;
            if (who.persona == null)
                return color;
            hasPersona = true;
            float scale = headScale;
            float ratio = ratioFix;
            color = who.persona.colorUsable;
            if (who.persona == Persona.Duck2)
            {
                color.r += 30;
                color.g += 30;
                color.b += 30;
            }
            if (who.slotType == SlotType.Spectator)
                color = Colors.DGPurple;
            SpriteMap hat = null;
            SpriteMap chatBust = who.persona.chatBust;
            Vec2 hatOffset = Vec2.Zero;
            if (who.team != null && who.team.hasHat && (who.connection != DuckNetwork.localConnection || !who.team.locked))
            {
                hatOffset = who.team.hatOffset * scale * (Options.Data.chatHeadScale + 1);
                hat = who.team.GetHat(who.persona);
            }
            bool quacking = who.netData.Get<bool>("quack");
            if (who.duck != null && !who.duck.dead && !who.duck.removeFromLevel)
                quacking = who.duck.quack > 0;
            Vec2 headPosition = new Vec2(p1.x, p1.y + -2 * (Options.Data.chatHeadScale + 1));
            if (hat != null)
            {
                hat.CenterOrigin();
                hat.depth = D(depth - 1f / 1000f);
                hat.alpha = alpha;
                hat.frame = !quacking || hat.texture == null || hat.texture.width <= 32 ? 0 : 1;
                hat.scale = new Vec2(scale, scale) * (Options.Data.chatHeadScale + 1);
                hat.scale *= new Vec2(1f, ratio);
                Graphics.Draw(hat, headPosition.x - hatOffset.x, headPosition.y - hatOffset.y);
                hat.scale = new Vec2(1f, 1f);
                hat.alpha = 1f;
            }
            chatBust.frame = !quacking ? 0 : 1;
            chatBust.depth = D(depth - 0.0015f);
            chatBust.alpha = alpha;
            chatBust.scale = new Vec2(scale, scale) * (Options.Data.chatHeadScale + 1);
            chatBust.scale *= new Vec2(1f, ratio);
            Graphics.Draw(chatBust, headPosition.x + 2f * chatBust.scale.x, headPosition.y + 5f * chatBust.scale.y);
            return color;
        }

        /// <summary>
        /// Draws the box you type in, returns how much the message history has to move up to make room.
        /// </summary>
        private static float DrawInput(Vec2 screen, float alpha)
        {
            if (MonoMain.UpdateLerpState)
                core.cursorFlash++;
            if (core.cursorFlash > 30)
                core.cursorFlash = 0;

            Profile localProfile = DuckNetwork.localProfile;
            if (localProfile == null)
                return 0f;
            float chatScale = DuckNetwork.chatScale;
            FancyBitmapFont font = SetupFont();
            float lineHeight = (font.characterHeight + 2) * font.scale.y;
            // Buttons live inside the box at the right end of the last line: [counter] [image] [moji] [Aa]
            float toolSize = lineHeight;
            float toolGap = Math.Max(1f, (float)Math.Round(font.scale.x));
            bool imageTool = ChatImages.sendingEnabled;
            // The Chat Buttons option can hide them completely, then they don't take up any room either
            int toolCount = chatButtonsMode == ChatButtonsHide ? 0 : imageTool ? 3 : 2;
            string counter = text.Length >= MaxMessageLength - 100 ? text.Length + "/" + MaxMessageLength : null;
            float counterWidth = counter == null ? 0f : font.GetWidth(counter) + 4f * font.scale.x;
            float buttonsWidth = toolCount > 0 ? toolCount * toolSize + (toolCount - 1) * toolGap : 0f;
            // Text always wraps as if the buttons were there, so it doesn't jump around when they come and go
            float toolsWidth = buttonsWidth + counterWidth + 4f * font.scale.x;
            // HOVER: the box is its normal size while the buttons are hidden. It widens as soon as the mouse comes near,
            // and shrinks back once they've faded out (so they never draw over the text)
            int buttonsMode = chatButtonsMode;
            bool buttonsOut = buttonsMode == ChatButtonsShow || buttonsMode == ChatButtonsHover && (s_toolsWanted || s_toolFade > 0.01f);
            float reservedWidth = (buttonsOut ? buttonsWidth : 0f) + counterWidth + 4f * font.scale.x;
            float wrapWidth = Math.Max(120f, chatAreaWidth - 8f * chatScale - toolsWidth);

            string prefix = localProfile.name + ": ";
            ChatTextLayoutBuilder builder = new ChatTextLayoutBuilder(font, wrapWidth) { trailingEmptyLine = true };
            builder.AppendLiteral(prefix, 0, false);
            s_inputPrefixLength = prefix.Length;
            builder.AppendLiteral(text, 0, true, GetInputSprites());
            ChatTextLayout layout = builder.Build();
            // The prefix glyphs shouldn't map to positions in the typed text
            for (int i = 0; i < s_inputPrefixLength && i < layout.glyphs.Count; i++)
                layout.glyphs[i].sourceIndex = -1;
            s_inputLayout = layout;

            // Attachment row (thumbnail, loading or error message)
            ChatImageAttachment attachment = ChatImages.pendingAttachment;
            string attachmentLabel = null;
            Vec2 thumbnailSize = Vec2.Zero;
            if (attachment != null && attachment.sprite != null)
            {
                float maxThumb = lineHeight * 3f;
                float thumbScale = Math.Min(1f, Math.Min(maxThumb * 2f / attachment.width, maxThumb / attachment.height));
                thumbnailSize = new Vec2(attachment.width * thumbScale, attachment.height * thumbScale);
                string attachmentName = attachment.name.Length == 0 ? "PASTED IMAGE" : attachment.name;
                attachmentLabel = (attachmentName.Length > 24 ? attachmentName.Substring(0, 21) + "..." : attachmentName) + " (" + ChatImages.FormatSize(attachment.byteSize) + ")";
            }
            else if (ChatImages.preparingAttachment)
                attachmentLabel = "LOADING IMAGE...";
            else if (ChatImages.attachmentError != null)
                attachmentLabel = "|DGRED|" + ChatImages.attachmentError;
            float padding = 2f;
            float attachmentHeight = attachmentLabel == null ? 0f : Math.Max(thumbnailSize.y, lineHeight) + padding * 2f;

            float caretWidth = font.GetGlyphAdvance('_');
            float attachmentWidth = attachmentLabel == null ? 0f : thumbnailSize.x + font.GetWidth(attachmentLabel) + 12f + lineHeight;
            float width = Math.Max(layout.width + caretWidth + reservedWidth, attachmentWidth) + 4f * chatScale;
            // Where the box ends with the buttons out, the mouse coming near there brings them back
            float fullRight = 14f + Math.Max(layout.width + caretWidth + toolsWidth, attachmentWidth) + 4f * chatScale;
            float textHeight = layout.height;
            float height = textHeight + attachmentHeight;
            float bottom = screen.y - (font.characterHeight + 10) * font.scale.y + lineHeight;
            Vec2 p1 = new Vec2(14f, bottom - height);
            Vec2 p2 = p1 + new Vec2(width, height);

            Color color = Color.White;
            if (localProfile.persona != null)
                color = localProfile.persona.colorUsable;
            if (localProfile.slotType == SlotType.Spectator)
                color = Colors.DGPurple;

            Graphics.DrawRect(p1 + new Vec2(-1f, -1f), p2 + new Vec2(1f, 1f), Color.Black * alpha, D(0.7f), false, 1f * chatScale);
            Graphics.DrawRect(p1, p2, color * 0.85f * alpha, D(0.8f));

            // Attachment row
            s_removeAttachmentButton = new Rectangle(-100f, -100f, 0f, 0f);
            s_attachmentThumbnail = new Rectangle(-100f, -100f, 0f, 0f);
            if (attachmentLabel != null)
            {
                Vec2 rowPosition = p1 + new Vec2(2f, padding);
                float labelX = rowPosition.x;
                if (attachment != null && attachment.sprite != null)
                {
                    attachment.sprite.scale = new Vec2(thumbnailSize.x / attachment.width, thumbnailSize.y / attachment.height);
                    attachment.sprite.alpha = alpha;
                    Graphics.Draw(attachment.sprite, rowPosition.x, rowPosition.y, D(0.85f));
                    s_attachmentThumbnail = new Rectangle(rowPosition.x, rowPosition.y, thumbnailSize.x, thumbnailSize.y);
                    labelX += thumbnailSize.x + 4f;
                }
                font.Draw(attachmentLabel, labelX, rowPosition.y + Math.Max(0f, (thumbnailSize.y - lineHeight) / 2f) + 1f, Color.Black * alpha, D(1f));
                if (attachment != null)
                {
                    // little x to remove the image
                    float size = lineHeight * 0.8f;
                    Vec2 removePosition = new Vec2(p2.x - size - 2f, rowPosition.y);
                    s_removeAttachmentButton = new Rectangle(removePosition.x, removePosition.y, size, size);
                    bool hovered = s_removeAttachmentButton.Contains(s_mousePosition);
                    Color xColor = (hovered ? Color.White : Color.Black) * alpha;
                    float inset = size * 0.25f;
                    float lineWidth = Math.Max(1f, font.scale.x);
                    Graphics.DrawLine(removePosition + new Vec2(inset, inset), removePosition + new Vec2(size - inset, size - inset), xColor, lineWidth, D(1f));
                    Graphics.DrawLine(removePosition + new Vec2(size - inset, inset), removePosition + new Vec2(inset, size - inset), xColor, lineWidth, D(1f));
                }
                Graphics.DrawLine(new Vec2(p1.x, p1.y + attachmentHeight), new Vec2(p2.x, p1.y + attachmentHeight), Color.Black * (0.3f * alpha), 1f, D(0.85f));
            }

            Vec2 textOrigin = new Vec2(p1.x + 2f, p1.y + attachmentHeight + 2f);
            s_inputTextOrigin = textOrigin;
            s_inputBounds = new Rectangle(p1.x, p1.y + attachmentHeight, width, textHeight);
            font.symbolYOffset = 4f;
            int from = hasInputSelection ? CaretToBoundary(selectionStart) : -1;
            int to = hasInputSelection ? CaretToBoundary(selectionEnd) : -1;
            layout.Draw(textOrigin, Color.Black, alpha, 1f, from, to);

            // Caret, the classic blinking underscore
            if (core.cursorFlash >= 15 && !hasInputSelection)
            {
                Vec2 caret = layout.GetCaretPosition(CaretToBoundary(s_caret));
                font.DrawGlyph('_', textOrigin.x + caret.x, textOrigin.y + caret.y, Color.Black * alpha, D(1f));
            }

            DrawInputTools(font, p1, p2, toolSize, toolGap, imageTool, counter, alpha, buttonsOut, fullRight);
            s_inputTop = p1.y;
            s_inputRight = p2.x;
            s_inputDrawn = true;
            return -(height + 4f * font.scale.y);
        }

        /// <summary>
        /// The image, moji and formatting buttons (and the character counter) at the right end of the chat box's last line.
        /// They stay faint until the mouse comes near, so they don't clutter the box while typing.
        /// </summary>
        private const int ChatButtonsHide = 0;
        private const int ChatButtonsHover = 1;
        private const int ChatButtonsShow = 2;

        private static int chatButtonsMode => Maths.Clamp(DGRSettings.ChatButtons, ChatButtonsHide, ChatButtonsShow);

        /// <summary>Whether the mouse was near the chat box last frame, for the HOVER option.</summary>
        private static bool s_toolsWanted;

        /// <param name="buttonsOut">False while HOVER has the buttons put away (the box is its normal size then).</param>
        /// <param name="fullRight">Where the box's right edge is with the buttons out.</param>
        private static void DrawInputTools(FancyBitmapFont font, Vec2 p1, Vec2 p2, float size, float gap, bool imageTool, string counter, float alpha, bool buttonsOut, float fullRight)
        {
            int mode = chatButtonsMode;
            Rectangle near = new Rectangle(p1.x - 24f, p1.y - 24f, Math.Max(p2.x, fullRight) - p1.x + 48f, p2.y - p1.y + 48f);
            bool mouseNear = s_showCursor && near.Contains(s_mousePosition) || s_pickerOpen;
            s_toolsWanted = mouseNear;
            // SHOW: always there. HOVER: fades in when the mouse comes near the box and out again when it leaves
            float target = mode == ChatButtonsShow || mouseNear ? 0.9f : 0f;
            if (MonoMain.UpdateLerpState)
                s_toolFade = Lerp.Float(s_toolFade, target, 0.08f);

            float right = p2.x - 1f;
            float y = p2.y - size;
            if (mode == ChatButtonsHide || !buttonsOut)
            {
                s_guideButton = s_mojiButton = s_imageButton = new Rectangle(-100f, -100f, 0f, 0f);
                s_guideHovered = false;
                if (mode == ChatButtonsHide)
                    s_pickerOpen = false;
                DrawCharacterCounter(font, counter, right, y, alpha);
                return;
            }
            Rectangle Next()
            {
                Rectangle rect = new Rectangle(right - size, y, size, size);
                right -= size + gap;
                return rect;
            }

            bool idle = s_drag == DragMode.None;
            float pixel = Math.Max(1f, (float)Math.Round(font.scale.x));

            // Aa, hovering shows the formatting guide
            s_guideButton = Next();
            bool guideHovered = s_guideHovered && idle;
            float guideAlpha = alpha * (guideHovered ? 1f : s_toolFade);
            if (guideHovered)
                Graphics.DrawRect(new Vec2(s_guideButton.x, s_guideButton.y), new Vec2(s_guideButton.Right, s_guideButton.Bottom), Color.Black * (0.15f * alpha), D(0.85f));
            {
                // the two letters squished together so they fit the square
                float overlap = font.scale.x;
                float advanceA = font.GetGlyphAdvance('A');
                float advanceSmallA = font.GetGlyphAdvance('a');
                float labelWidth = advanceA + advanceSmallA - overlap;
                float squish = Math.Min(1f, (size - 2f * font.scale.x) / labelWidth);
                float labelX = s_guideButton.x + (float)Math.Round((size - labelWidth * squish) / 2f);
                Color labelColor = Color.Black * guideAlpha;
                font.DrawGlyph('A', labelX, y + 2f, labelColor, D(1f), 0f, false, squish);
                font.DrawGlyph('a', labelX + (advanceA - overlap) * squish, y + 2f, labelColor, D(1f), 0f, false, squish);
            }

            // Moji picker
            s_mojiButton = Next();
            bool mojiHovered = s_mojiButton.Contains(s_mousePosition) && idle && s_showCursor;
            bool mojiActive = mojiHovered || s_pickerOpen;
            if (mojiActive)
                Graphics.DrawRect(new Vec2(s_mojiButton.x, s_mojiButton.y), new Vec2(s_mojiButton.Right, s_mojiButton.Bottom), Color.Black * (0.15f * alpha), D(0.85f));
            DrawMojiIcon(s_mojiButton, mojiActive, alpha * (mojiActive ? 1f : s_toolFade));

            // Image
            s_imageButton = new Rectangle(-100f, -100f, 0f, 0f);
            if (imageTool)
            {
                s_imageButton = Next();
                bool imageHovered = s_imageButton.Contains(s_mousePosition) && idle && s_showCursor;
                if (imageHovered)
                    Graphics.DrawRect(new Vec2(s_imageButton.x, s_imageButton.y), new Vec2(s_imageButton.Right, s_imageButton.Bottom), Color.Black * (0.15f * alpha), D(0.85f));
                float imageAlpha = alpha * (imageHovered ? 1f : s_toolFade);
                DrawImageIcon(new Vec2(s_imageButton.x, s_imageButton.y), size, (ChatImages.preparingAttachment ? Color.Gray : Color.Black) * imageAlpha, pixel);
            }

            DrawCharacterCounter(font, counter, right, y, alpha);
        }

        /// <summary>The character counter, shows up when getting close to the limit (left of the buttons, or at the end of the box without them).</summary>
        private static void DrawCharacterCounter(FancyBitmapFont font, string counter, float right, float y, float alpha)
        {
            if (counter == null)
                return;
            bool full = text.Length >= MaxMessageLength;
            float counterWidth = font.GetWidth(counter);
            font.Draw(counter, right - counterWidth - font.scale.x, y + 2f, (full ? Colors.DGRed : Color.Black * 0.75f) * alpha, D(1f));
        }

        private static void DrawImageIcon(Vec2 position, float size, Color color, float pixel)
        {
            pixel = Math.Max(1f, (float)Math.Round(pixel));
            float inset = (float)Math.Round(size * 0.18f);
            Vec2 tl = position + new Vec2(inset, inset + pixel);
            Vec2 br = position + new Vec2(size - inset, size - inset - pixel);
            Graphics.DrawRect(tl, br, color, D(1f), false, pixel);
            float w = br.x - tl.x;
            float h = br.y - tl.y;
            // mountains
            Graphics.DrawLine(new Vec2(tl.x + pixel, br.y - pixel), new Vec2(tl.x + w * 0.4f, tl.y + h * 0.45f), color, pixel, D(1f));
            Graphics.DrawLine(new Vec2(tl.x + w * 0.4f, tl.y + h * 0.45f), new Vec2(tl.x + w * 0.62f, tl.y + h * 0.72f), color, pixel, D(1f));
            Graphics.DrawLine(new Vec2(tl.x + w * 0.55f, tl.y + h * 0.62f), new Vec2(tl.x + w * 0.72f, tl.y + h * 0.5f), color, pixel, D(1f));
            Graphics.DrawLine(new Vec2(tl.x + w * 0.72f, tl.y + h * 0.5f), new Vec2(br.x - pixel, br.y - pixel), color, pixel, D(1f));
            // sun
            float sun = Math.Max(pixel, (float)Math.Round(w * 0.16f));
            Vec2 sunPosition = new Vec2(br.x - w * 0.3f, tl.y + h * 0.2f);
            Graphics.DrawRect(sunPosition, sunPosition + new Vec2(sun, sun), color, D(1f));
        }

        private static Rectangle GetCopyButtonRect(ChatTextLayout layout, ChatCodeBlock block, Vec2 origin)
        {
            FancyBitmapFont font = layout.font;
            float width = font.GetWidth(s_copiedFrames > 0 && s_copiedBlockContent == block.content ? "COPIED" : "COPY") + 6f * font.scale.x;
            float height = layout.lineHeight;
            return new Rectangle(origin.x + block.rect.x + block.rect.width - width - 1f, origin.y + block.rect.y + 1f, width, height);
        }

        /// <summary>The small copy button shown in the corner of a hovered code block.</summary>
        public static void DrawCodeBlockCopyButton(ChatTextLayout layout, ChatCodeBlock block, Vec2 origin, float alpha, float depth, float textDepth)
        {
            Rectangle rect = GetCopyButtonRect(layout, block, origin);
            bool copied = s_copiedFrames > 0 && s_copiedBlockContent == block.content;
            bool hovered = rect.Contains(s_mousePosition);
            Graphics.DrawRect(new Vec2(rect.x, rect.y), new Vec2(rect.Right, rect.Bottom), Color.Black * ((hovered ? 0.85f : 0.6f) * alpha), D(depth));
            FancyBitmapFont font = layout.font;
            font.Draw(copied ? "COPIED" : "COPY", rect.x + 3f * font.scale.x, rect.y + font.scale.y, (copied ? Colors.DGGreen : Color.White) * alpha, D(textDepth + 0.001f));
        }

        private static void DrawCursor()
        {
            if (!s_showCursor || DevConsole.open)
                return;
            if (s_cursorSprite == null)
                s_cursorSprite = new SpriteMap("cursors", 16, 16);
            float scale = headScale;
            Vec2 position = Mouse.positionConsole;
            Vec2 hotspot;
            switch (s_cursorKind)
            {
                case CursorKind.Text:
                    hotspot = new Vec2(4f, 6f);
                    break;
                case CursorKind.Hand:
                    hotspot = new Vec2(4f, 1f);
                    break;
                default:
                    hotspot = new Vec2(1f, 1f);
                    break;
            }
            s_cursorSprite.frame = (int)s_cursorKind;
            s_cursorSprite.scale = new Vec2(scale, scale);
            s_cursorSprite.alpha = 1f;
            s_cursorSprite.center = Vec2.Zero;
            Graphics.Draw(s_cursorSprite, position.x - hotspot.x * scale, position.y - hotspot.y * scale, D(1.5f));
        }

        #endregion
    }
}
