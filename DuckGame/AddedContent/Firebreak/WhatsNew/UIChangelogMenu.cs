using System;
using System.Collections.Generic;

namespace DuckGame
{
    /// <summary>
    /// Options > Rebuilt > What's New, the GitHub release notes in a scrolling view, one release per page.
    /// Opening it marks the changelog as read, which turns the WHAT'S NEW button back to its normal color.
    /// </summary>
    public class UIChangelogMenu : UIMenu
    {
        private readonly UIChangelogView _view;

        public UIChangelogMenu(UIMenu pPrev, float xpos, float ypos)
          : base("|PINK|♥|WHITE|WHAT'S NEW|PINK|♥", xpos, ypos, 300f, conString: "@WASD@BROWSE @MENU1@GITHUB @CANCEL@BACK")
        {
            _view = new UIChangelogView(284f, 118f);
            Add(_view, true);
            SetBackFunction(new UIMenuActionOpenMenu(this, pPrev));
        }

        /// <summary>The text for the button that opens it, pink while this version's changelog hasn't been opened.</summary>
        public static string buttonText => Changelog.unread ? "|PINK|WHAT'S NEW" : "WHAT'S NEW";

        public override void Open()
        {
            base.Open();
            Changelog.Load();
            _view.Reset();
            Changelog.MarkSeen();
        }
    }

    public class UIChangelogView : UIComponent
    {
        private const float ScrollBarWidth = 2f;
        private const float ScrollBarGap = 4f;
        private const string NewerText = "< NEWER";
        private const string OlderText = "OLDER >";

        private readonly Dictionary<int, MarkdownView> _pages = new Dictionary<int, MarkdownView>();
        private readonly MarkdownView _message = new MarkdownView();
        private FancyBitmapFont _font;
        private int _page;
        private float _scroll;
        private ChatLink _hoveredLink;
        /// <summary>0 newer, 1 older, -1 none.</summary>
        private int _hoveredNav = -1;
        // MENU1 opens the release when it's let go, not when it's pressed. The Steam overlay takes the input while it's open,
        // so the key never looks released and pressing it would keep opening the page again.
        private bool _gitHubHeld;
        private int _gitHubCooldown;

        public UIChangelogView(float wide, float high)
          : base(0f, 0f, wide, high)
        {
        }

        private float navHeight => _font.characterHeight * _font.scale.y + 5f;

        private float contentHeight => height - navHeight;

        private float textWidth => width - ScrollBarWidth - ScrollBarGap;

        public void Reset()
        {
            _scroll = 0f;
            _page = 0;
            _gitHubHeld = false;
        }

        private bool loaded => Changelog.state == Changelog.LoadState.Loaded && Changelog.releases.Count > 0;

        private Changelog.Release currentRelease => loaded ? Changelog.releases[Maths.Clamp(_page, 0, Changelog.releases.Count - 1)] : null;

        /// <summary>The markdown for what's showing right now, laid out.</summary>
        private MarkdownView Prepare()
        {
            if (_font == null)
                _font = new FancyBitmapFont("smallFontChat");
            _font.scale = new Vec2(0.5f);

            MarkdownView view;
            switch (Changelog.state)
            {
                case Changelog.LoadState.Loaded:
                    if (Changelog.releases.Count == 0)
                    {
                        view = _message;
                        view.SetText("## No releases found\n\nThere's nothing on the GitHub releases page yet.");
                        break;
                    }
                    _page = Maths.Clamp(_page, 0, Changelog.releases.Count - 1);
                    if (!_pages.TryGetValue(_page, out view))
                    {
                        view = new MarkdownView();
                        view.SetText(ReleaseMarkdown(Changelog.releases[_page]));
                        _pages[_page] = view;
                    }
                    break;
                case Changelog.LoadState.Failed:
                    view = _message;
                    view.SetText("## GitHub releases page not loading :(\n\nCheck your connection and open this menu again, or open the releases page on GitHub.");
                    break;
                default:
                    view = _message;
                    view.SetText("Loading releases from GitHub...");
                    break;
            }
            view.Layout(textWidth);
            return view;
        }

        private static string ReleaseMarkdown(Changelog.Release release)
        {
            string subtitle = release.published != DateTime.MinValue ? release.published.ToLocalTime().ToString("MMMM d, yyyy") : "";
            if (release.name != release.tag)
                subtitle = subtitle.Length > 0 ? release.tag + " - " + subtitle : release.tag;
            if (release.prerelease)
                subtitle += subtitle.Length > 0 ? " (pre-release)" : "Pre-release";
            string body = release.body.Length > 0 ? release.body : "*No release notes.*";
            return "# " + release.name + "\n" + (subtitle.Length > 0 ? "*" + subtitle + "*\n" : "") + "\n" + body;
        }

        #region Navigation

        private bool hasOlder => loaded && _page < Changelog.releases.Count - 1;

        private bool hasNewer => loaded && _page > 0;

        private void ChangePage(int delta)
        {
            if (delta > 0 ? !hasOlder : !hasNewer)
                return;
            _page += delta;
            _scroll = 0f;
            SFX.DontSave = 1;
            SFX.Play("textLetter", 0.7f);
        }

        private void OpenOnGitHub()
        {
            ChatBox.OpenLink(currentRelease?.url ?? Changelog.ReleasesPage);
        }

        /// <summary>0 newer (on the left), 1 older (on the right), relative to the view's top left.</summary>
        private Rectangle NavRect(int index)
        {
            float textWidth = _font.GetWidth(NavText(index));
            float left = index == 0 ? 0f : width - textWidth;
            return new Rectangle(left - 2f, 0f, textWidth + 4f, navHeight - 1f);
        }

        private static string NavText(int index) => index == 0 ? NewerText : OlderText;

        private bool NavEnabled(int index) => index == 0 ? hasNewer : hasOlder;

        #endregion

        public override void Update()
        {
            MarkdownView view = Prepare();
            UIMenu menu = rootMenu;
            if (open && !animating && (menu == null || menu.open) && !UIMenu.globalUILock)
            {
                float line = view.lineHeight;
                if (Input.Down(Triggers.MenuUp))
                    _scroll -= line * 0.5f;
                if (Input.Down(Triggers.MenuDown))
                    _scroll += line * 0.5f;
                if (Mouse.scrollingUp)
                    _scroll -= line * 3f;
                else if (Mouse.scrollingDown)
                    _scroll += line * 3f;
                if (Input.Pressed(Triggers.MenuLeft))
                    ChangePage(-1);
                else if (Input.Pressed(Triggers.MenuRight))
                    ChangePage(1);
                if (_gitHubCooldown > 0)
                    _gitHubCooldown--;
                else if (Input.Pressed(Triggers.Menu1))
                {
                    _gitHubHeld = true;
                    SFX.DontSave = 1;
                    SFX.Play("rockHitGround", 0.7f);
                }
                if (_gitHubHeld && !Input.Down(Triggers.Menu1))
                {
                    _gitHubHeld = false;
                    _gitHubCooldown = 60;
                    OpenOnGitHub();
                }

                Vec2 local = Mouse.position - new Vec2(x - width / 2f, y - height / 2f);
                _hoveredNav = -1;
                for (int i = 0; i < 2; i++)
                {
                    if (NavEnabled(i) && NavRect(i).Contains(local))
                        _hoveredNav = i;
                }
                _hoveredLink = null;
                Vec2 contentLocal = local - new Vec2(0f, navHeight);
                if (contentLocal.x >= 0f && contentLocal.y >= 0f && contentLocal.x < textWidth && contentLocal.y < contentHeight)
                    _hoveredLink = view.GetLinkAt(contentLocal + new Vec2(0f, _scroll));
                if (Mouse.left == InputState.Pressed)
                {
                    if (_hoveredNav == 0)
                        ChangePage(-1);
                    else if (_hoveredNav == 1)
                        ChangePage(1);
                    else if (_hoveredLink != null)
                        ChatBox.OpenLink(_hoveredLink.url);
                }
            }
            view = Prepare();
            _scroll = Maths.Clamp(_scroll, 0f, Math.Max(0f, view.height - contentHeight));
            base.Update();
        }

        public override void Draw()
        {
            UILerp.UpdateLerpState(position, MonoMain.IntraTick, MonoMain.UpdateLerpState);
            MarkdownView view = Prepare();
            Vec2 origin = new Vec2(UILerp.x - width / 2f, UILerp.y - height / 2f);
            float textDepth = depth.value + 0.2f;

            // Newer / older
            for (int i = 0; i < 2; i++)
            {
                Rectangle rect = NavRect(i);
                string text = NavText(i);
                Color color = !NavEnabled(i) ? Color.White * 0.25f : i == _hoveredNav ? Colors.DGPink : Color.White * 0.8f;
                _font.Draw(text, origin.x + rect.x + 2f, origin.y + 1f, color * alpha, ChatBox.D(textDepth));
            }
            Graphics.DrawRect(new Vec2(origin.x, origin.y + navHeight - 2f), new Vec2(origin.x + width, origin.y + navHeight - 2f + _font.scale.y), Color.White * (0.2f * alpha), ChatBox.D(textDepth));

            Vec2 contentOrigin = origin + new Vec2(0f, navHeight);
            float maxScroll = Math.Max(0f, view.height - contentHeight);
            _scroll = Maths.Clamp(_scroll, 0f, maxScroll);
            view.Draw(contentOrigin, _scroll, contentHeight, alpha, textDepth, _hoveredLink);

            if (maxScroll > 0f)
            {
                float barX = contentOrigin.x + width - ScrollBarWidth;
                Graphics.DrawRect(new Vec2(barX, contentOrigin.y), new Vec2(barX + ScrollBarWidth, contentOrigin.y + contentHeight), Color.White * (0.15f * alpha), ChatBox.D(textDepth));
                float thumb = Math.Max(8f, contentHeight * contentHeight / view.height);
                float thumbY = contentOrigin.y + (contentHeight - thumb) * (_scroll / maxScroll);
                Graphics.DrawRect(new Vec2(barX, thumbY), new Vec2(barX + ScrollBarWidth, thumbY + thumb), Colors.DGPink * alpha, ChatBox.D(textDepth + 0.01f));
            }
            base.Draw();
        }
    }
}
