// Decompiled with JetBrains decompiler
// Type: XnaToFna.ProxyForms.GameForm
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

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
        private Microsoft.Xna.Framework.Rectangle _WindowedBounds;
        private Microsoft.Xna.Framework.Rectangle _Bounds;
        private FormBorderStyle _FormBorderStyle = FormBorderStyle.FixedDialog;
        private FormWindowState _WindowState;
        private FormStartPosition _StartPosition = FormStartPosition.WindowsDefaultLocation;

        private bool Dirty
        {
            get => this._Dirty;
            set
            {
                if (value)
                {
                    this._FormBorderStyle = this.FormBorderStyle;
                    this._WindowState = this.WindowState;
                }
                this._Dirty = value;
            }
        }

        public override XnaToFna.ProxyDrawing.Rectangle Bounds
        {
            get => new XnaToFna.ProxyDrawing.Rectangle(this._Bounds.X, this._Bounds.Y, this._Bounds.Width, this._Bounds.Height);
            set => this.SDLBounds = this._Bounds = this._WindowedBounds = new Microsoft.Xna.Framework.Rectangle(value.X, value.Y, value.Width, value.Height);
        }

        public Microsoft.Xna.Framework.Rectangle SDLBounds
        {
            get => XnaToFnaHelper.Game.Window.ClientBounds;
            set
            {
                IntPtr handle = XnaToFnaHelper.Game.Window.Handle;
                SDL.SDL_SetWindowSize(handle, value.Width, value.Height);
                SDL.SDL_SetWindowPosition(handle, value.X, value.Y);
            }
        }

        protected override XnaToFna.ProxyDrawing.Rectangle _ClientRectangle
        {
            get
            {
                Microsoft.Xna.Framework.Rectangle clientBounds = XnaToFnaHelper.Game.Window.ClientBounds;
                return new XnaToFna.ProxyDrawing.Rectangle(0, 0, clientBounds.Width, clientBounds.Height);
            }
        }

        public override XnaToFna.ProxyDrawing.Point Location
        {
            get
            {
                int x;
                int y;
                SDL.SDL_GetWindowPosition(XnaToFnaHelper.Game.Window.Handle, out x, out y);
                return new XnaToFna.ProxyDrawing.Point(x, y);
            }
            set => SDL.SDL_SetWindowPosition(XnaToFnaHelper.Game.Window.Handle, value.X, value.Y);
        }

        public override FormBorderStyle FormBorderStyle
        {
            get
            {
                if (this.Dirty)
                    return this._FormBorderStyle;
                if (XnaToFnaHelper.Game.Window.IsBorderlessEXT || this.FakeFullscreenWindow)
                    return FormBorderStyle.None;
                return XnaToFnaHelper.Game.Window.AllowUserResizing ? FormBorderStyle.Sizable : FormBorderStyle.FixedDialog;
            }
            set
            {
                this.Dirty = true;
                this._FormBorderStyle = value;
            }
        }

        public override FormWindowState WindowState
        {
            get
            {
                if (this.Dirty)
                    return this._WindowState;
                uint windowFlags = SDL.SDL_GetWindowFlags(XnaToFnaHelper.Game.Window.Handle);
                if (((int)windowFlags & 128) != 0 || this.FakeFullscreenWindow)
                    return FormWindowState.Maximized;
                return ((int)windowFlags & 64) != 0 ? FormWindowState.Minimized : FormWindowState.Normal;
            }
            set
            {
                this.Dirty = true;
                this._WindowState = value;
            }
        }

        public override FormStartPosition StartPosition
        {
            get => this._StartPosition;
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
                this._StartPosition = value;
            }
        }

        public override bool Focused => XnaToFnaHelper.Game.IsActive;

        protected override void SetVisibleCore(bool visible)
        {
        }

        public void SDLWindowSizeChanged(object sender, EventArgs e)
        {
            Microsoft.Xna.Framework.Rectangle sdlBounds = this.SDLBounds;
            this._Bounds = new Microsoft.Xna.Framework.Rectangle(sdlBounds.X, sdlBounds.Y, sdlBounds.Width, sdlBounds.Height);
            if (((int)SDL.SDL_GetWindowFlags(XnaToFnaHelper.Game.Window.Handle) & 1) != 0 || this.FakeFullscreenWindow)
                return;
            this._WindowedBounds = this._Bounds;
        }

        public void SDLWindowChanged(
          IntPtr window,
          int clientWidth,
          int clientHeight,
          bool wantsFullscreen,
          string screenDeviceName,
          ref string resultDeviceName)
        {
            this.SDLWindowSizeChanged(null, null);
        }

        protected override void _Close() => XnaToFnaHelper.Game.Exit();

        public void ApplyChanges()
        {
            if (!this.Dirty || Environment.GetEnvironmentVariable("FNADROID") == "1")
                return;
            XnaToFnaGame game = XnaToFnaHelper.Game;
            IntPtr handle = game.Window.Handle;
            GraphicsDeviceManager service = XnaToFnaHelper.GetService<IGraphicsDeviceManager, GraphicsDeviceManager>();
            bool isFullScreen = service.IsFullScreen;
            bool flag1 = this.FormBorderStyle == FormBorderStyle.None;
            bool flag2 = this.WindowState == FormWindowState.Maximized;
            bool fullscreenWindow = this.FakeFullscreenWindow;
            this.FakeFullscreenWindow = flag2 & flag1;
            XnaToFnaHelper.Log("[ProxyForms] Applying changes from ProxyForms.Form to SDL window");
            XnaToFnaHelper.Log(string.Format("[ProxyForms] Currently fullscreen: {0}; Fake fullscreen window: {1}; Border: {2}; State: {3}", isFullScreen, FakeFullscreenWindow, FormBorderStyle, WindowState));
            if (this.FakeFullscreenWindow)
            {
                XnaToFnaHelper.Log("[ProxyForms] Game expects borderless fullscreen... give it proper fullscreen instead.");
                if (!isFullScreen)
                    this._WindowedBounds = this.SDLBounds;
                XnaToFnaHelper.Log(string.Format("[ProxyForms] Last window size: {0} x {1}", _WindowedBounds.Width, _WindowedBounds.Height));
                DisplayMode displayMode = service.GraphicsDevice.DisplayMode;
                service.PreferredBackBufferWidth = displayMode.Width;
                service.PreferredBackBufferHeight = displayMode.Height;
                service.IsFullScreen = true;
                service.ApplyChanges();
                this._Bounds = this.SDLBounds;
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
                    this._Bounds = this.SDLBounds;
                }
                else
                {
                    SDL.SDL_RestoreWindow(handle);
                    this.SDLBounds = this._Bounds = this._WindowedBounds;
                }
                XnaToFnaHelper.Log(string.Format("[ProxyForms] New window size: {0} x {1}", _Bounds.Width, _Bounds.Height));
                service.PreferredBackBufferWidth = this._Bounds.Width;
                service.PreferredBackBufferHeight = this._Bounds.Height;
                service.ApplyChanges();
            }
            this.Dirty = false;
        }
    }
}
