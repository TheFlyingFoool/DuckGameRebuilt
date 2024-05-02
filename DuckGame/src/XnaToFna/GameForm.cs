using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDL2;
using System;

namespace XnaToFna.ProxyForms
{
    public sealed class GameForm : Form
    {
        public static GameForm Instance;
        private bool _Dirty;
        private bool FakeFullscreenWindow;
        private Rectangle _WindowedBounds;
        private Rectangle _Bounds;
        private FormBorderStyle _FormBorderStyle = FormBorderStyle.FixedDialog;
        private FormWindowState _WindowState;
        private FormStartPosition _StartPosition = FormStartPosition.WindowsDefaultLocation;

        private bool Dirty
        {
            get => _Dirty;
            set
            {
                if (value)
                {
                    _FormBorderStyle = FormBorderStyle;
                    _WindowState = WindowState;
                }
                _Dirty = value;
            }
        }

        public override ProxyDrawing.Rectangle Bounds
        {
            get => new ProxyDrawing.Rectangle(_Bounds.X, _Bounds.Y, _Bounds.Width, _Bounds.Height);
            set => SDLBounds = _Bounds = _WindowedBounds = new Rectangle(value.X, value.Y, value.Width, value.Height);
        }

        public Rectangle SDLBounds
        {
            get => XnaToFnaHelper.Game.Window.ClientBounds;
            set
            {
                IntPtr handle = XnaToFnaHelper.Game.Window.Handle;
                SDL.SDL_SetWindowSize(handle, value.Width, value.Height);
                SDL.SDL_SetWindowPosition(handle, value.X, value.Y);
            }
        }

        protected override ProxyDrawing.Rectangle _ClientRectangle
        {
            get
            {
                Rectangle clientBounds = XnaToFnaHelper.Game.Window.ClientBounds;
                return new ProxyDrawing.Rectangle(0, 0, clientBounds.Width, clientBounds.Height);
            }
        }

        public override ProxyDrawing.Point Location
        {
            get
            {
                int x;
                int y;
                SDL.SDL_GetWindowPosition(XnaToFnaHelper.Game.Window.Handle, out x, out y);
                return new ProxyDrawing.Point(x, y);
            }
            set => SDL.SDL_SetWindowPosition(XnaToFnaHelper.Game.Window.Handle, value.X, value.Y);
        }

        public override FormBorderStyle FormBorderStyle
        {
            get
            {
                if (Dirty)
                    return _FormBorderStyle;
                if (XnaToFnaHelper.Game.Window.IsBorderlessEXT || FakeFullscreenWindow)
                    return FormBorderStyle.None;
                return XnaToFnaHelper.Game.Window.AllowUserResizing ? FormBorderStyle.Sizable : FormBorderStyle.FixedDialog;
            }
            set
            {
                Dirty = true;
                _FormBorderStyle = value;
            }
        }

        public override FormWindowState WindowState
        {
            get
            {
                if (Dirty)
                    return _WindowState;
                uint windowFlags = SDL.SDL_GetWindowFlags(XnaToFnaHelper.Game.Window.Handle);
                if (((int)windowFlags & 128) != 0 || FakeFullscreenWindow)
                    return FormWindowState.Maximized;
                return ((int)windowFlags & 64) != 0 ? FormWindowState.Minimized : FormWindowState.Normal;
            }
            set
            {
                Dirty = true;
                _WindowState = value;
            }
        }

        public override FormStartPosition StartPosition
        {
            get => _StartPosition;
            set
            {
                //if (((int) SDL.SDL_GetWindowFlags(XnaToFnaHelper.Game.Window.Handle) & 8) != 8)// come back to later mabye idk man
                //  return;
                //switch (value)
                //{
                //  case FormStartPosition.CenterScreen:
                //  case FormStartPosition.CenterParent:
                //    SDL.SDL_SetWindowPosition(XnaToFnaHelper.Game.Window.Handle, 805240832, 805240832);
                //    break;
                //  case FormStartPosition.WindowsDefaultLocation:
                //  case FormStartPosition.WindowsDefaultBounds:
                //    SDL.SDL_SetWindowPosition(XnaToFnaHelper.Game.Window.Handle, 805240832, 805240832);
                //    break;
                //}
                _StartPosition = value;
            }
        }

        public override bool Focused => XnaToFnaHelper.Game.IsActive;

        protected override void SetVisibleCore(bool visible)
        {
        }

        public void SDLWindowSizeChanged(object sender, EventArgs e)
        {
            Rectangle sdlBounds = SDLBounds;
            _Bounds = new Rectangle(sdlBounds.X, sdlBounds.Y, sdlBounds.Width, sdlBounds.Height);
            if (((int)SDL.SDL_GetWindowFlags(XnaToFnaHelper.Game.Window.Handle) & 1) != 0 || FakeFullscreenWindow)
                return;
            _WindowedBounds = _Bounds;
        }

        public void SDLWindowChanged(
          IntPtr window,
          int clientWidth,
          int clientHeight,
          bool wantsFullscreen,
          string screenDeviceName,
          ref string resultDeviceName)
        {
            SDLWindowSizeChanged(null, null);
        }

        protected override void _Close() => XnaToFnaHelper.Game.Exit();

        public void ApplyChanges()
        {
            if (!Dirty || Environment.GetEnvironmentVariable("FNADROID") == "1")
                return;
            XnaToFnaGame game = XnaToFnaHelper.Game;
            IntPtr handle = game.Window.Handle;
            GraphicsDeviceManager service = XnaToFnaHelper.GetService<IGraphicsDeviceManager, GraphicsDeviceManager>();
            bool isFullScreen = service.IsFullScreen;
            bool flag1 = FormBorderStyle == FormBorderStyle.None;
            bool flag2 = WindowState == FormWindowState.Maximized;
            bool fullscreenWindow = FakeFullscreenWindow;
            FakeFullscreenWindow = flag2 & flag1;
            XnaToFnaHelper.Log("[ProxyForms] Applying changes from ProxyForms.Form to SDL window");
            XnaToFnaHelper.Log(string.Format("[ProxyForms] Currently fullscreen: {0}; Fake fullscreen window: {1}; Border: {2}; State: {3}", isFullScreen, FakeFullscreenWindow, FormBorderStyle, WindowState));
            if (FakeFullscreenWindow)
            {
                XnaToFnaHelper.Log("[ProxyForms] Game expects borderless fullscreen... give it proper fullscreen instead.");
                if (!isFullScreen)
                    _WindowedBounds = SDLBounds;
                XnaToFnaHelper.Log(string.Format("[ProxyForms] Last window size: {0} x {1}", _WindowedBounds.Width, _WindowedBounds.Height));
                DisplayMode displayMode = service.GraphicsDevice.DisplayMode;
                service.PreferredBackBufferWidth = displayMode.Width;
                service.PreferredBackBufferHeight = displayMode.Height;
                service.IsFullScreen = true;
                service.ApplyChanges();
                _Bounds = SDLBounds;
            }
            else
            {
                if (fullscreenWindow)
                {
                    XnaToFnaHelper.Log("[ProxyForms] Leaving fake borderless fullscreen.");
                    service.IsFullScreen = false;
                }
                game.Window.IsBorderlessEXT = flag1;
                if (flag2)
                {
                    SDL.SDL_MaximizeWindow(handle);
                    _Bounds = SDLBounds;
                }
                else
                {
                    SDL.SDL_RestoreWindow(handle);
                    SDLBounds = _Bounds = _WindowedBounds;
                }
                XnaToFnaHelper.Log(string.Format("[ProxyForms] New window size: {0} x {1}", _Bounds.Width, _Bounds.Height));
                service.PreferredBackBufferWidth = _Bounds.Width;
                service.PreferredBackBufferHeight = _Bounds.Height;
                service.ApplyChanges();
            }
            Dirty = false;
        }
    }
}
