using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class Resolution
    {
        public bool recommended;
        private static Resolution _lastApplied;
        private static IntPtr _window;
        private static float _screenDPI;
        private static int _takeFocus;
        public static GraphicsDeviceManager _device;
        private static Resolution _pendingResolution;
        //private static Matrix _matrix;
        public bool isDefault;
        public ScreenMode mode;
        public Vec2 dimensions;
        public Vec2 pos = Vec2.Zero;
        public static Dictionary<ScreenMode, List<Resolution>> supportedDisplaySizes;

        public static Resolution adapterResolution => GetDefault(ScreenMode.Fullscreen);

        public static Resolution current => Options.LocalData.currentResolution;

        public static float fontSizeMultiplier => current.x / 1280f;

        public static Resolution lastApplied => _lastApplied;

        public static Vec2 size => Graphics._screenViewport.HasValue ? new Vec2(Graphics._screenViewport.Value.Width, Graphics._screenViewport.Value.Height) : current.dimensions;

        private static float GetScreenDPI()
        {
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
            float dpiX = graphics.DpiX;
            graphics.Dispose();
            return dpiX;
        }
        public static GraphicsDeviceManager GetGraphics()
        {
            return _device;
        }
        public static void SetWindow(IntPtr windowhandler)
        {
             _window = windowhandler;       
        }
        public static void Set(Resolution pResolution)
        {
            _pendingResolution = pResolution;
        }
        public static void Apply()
        {
            Graphics.snap = 4f;
            if (_pendingResolution == null || Program.isLinux && !Keyboard.NothingPressed())
                return;
            _lastApplied = _pendingResolution;
            Options.LocalData.currentResolution = _pendingResolution;
            DevConsole.Log(DCSection.General, "Applying resolution (" + _pendingResolution.ToString() + ")");
            bool flag = false;
            foreach (DisplayMode supportedDisplayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                //if (Resolution._pendingResolution.x <= supportedDisplayMode.Width && Resolution._pendingResolution.y <= supportedDisplayMode.Height)
                //{
                //    flag = true;
                //    if (Resolution._pendingResolution.mode == ScreenMode.Borderless)
                //    {
                //        Resolution._device.PreferredBackBufferWidth = Resolution.adapterResolution.x;
                //        Resolution._device.PreferredBackBufferHeight = Resolution.adapterResolution.y;
                //    }
                //    else
                //    {
                //        Resolution._device.PreferredBackBufferWidth = Resolution._pendingResolution.x;
                //        Resolution._device.PreferredBackBufferHeight = Resolution._pendingResolution.y;
                //    }
                //    Resolution._device.IsFullScreen = Resolution._pendingResolution.mode == ScreenMode.Fullscreen;
                //    Resolution._device.ApplyChanges();
                //    break;
                //}
                flag = true;
                if (_pendingResolution.mode == ScreenMode.Borderless)
                {
                    _device.PreferredBackBufferWidth = adapterResolution.x;
                    _device.PreferredBackBufferHeight = adapterResolution.y;
                }
                else
                {
                    _device.PreferredBackBufferWidth = _pendingResolution.x;
                    _device.PreferredBackBufferHeight = _pendingResolution.y;
                }
                _device.IsFullScreen = _pendingResolution.mode == ScreenMode.Fullscreen;
                _device.ApplyChanges();
                break;
            }
            if (!flag)
            {
                RestoreDefaults();
                Apply();
            }
            else
            {
                switch (Options.LocalData.currentResolution.mode)
                {
                    case ScreenMode.Windowed:
                        Graphics.mouseVisible = false;
                        Graphics._screenBufferTarget = null;
                        FNAPlatform.SetWindowBordered(MonoMain.instance.Window.Handle, true);// Resolution._window.FormBorderStyle = FormBorderStyle.FixedSingle;
                        //FNAPlatform.SetWindowPosition(MonoMain.instance.Window.Handle, Resolution.adapterResolution.x / 2 - Options.LocalData.currentResolution.x / 2, Resolution.adapterResolution.y / 2 - Options.LocalData.currentResolution.y / 2 - 16);
                        // Resolution._window.Location = new System.Drawing.Point(Resolution.adapterResolution.x / 2 - Options.LocalData.currentResolution.x / 2, Resolution.adapterResolution.y / 2 - Options.LocalData.currentResolution.y / 2 - 16);
                        // removed window positioning as it seems to auto center on size change atm ¯\_(ツ)_/¯, Dan
                        break;
                    case ScreenMode.Fullscreen:
                        Graphics.mouseVisible = false;
                        Graphics._screenBufferTarget = null;
                        break;
                    case ScreenMode.Borderless:
                        Graphics.mouseVisible = false;
                        Graphics._screenBufferTarget = new RenderTarget2D(Options.LocalData.currentResolution.x, Options.LocalData.currentResolution.y, true, RenderTargetUsage.PreserveContents);
                        FNAPlatform.SetWindowBordered(MonoMain.instance.Window.Handle, false); //  Resolution._window.FormBorderStyle = FormBorderStyle.None;
                        //FNAPlatform.SetWindowPosition(Resolution._window, (int)Options.LocalData.currentResolution.pos.x, (int)Options.LocalData.currentResolution.pos.y); //Resolution._window.Location = new System.Drawing.Point(0, 0);
                        // removed window positioning as it seems to auto center on size change atm ¯\_(ツ)_/¯, Dan
                        if (Graphics._screenBufferTarget.width < 400)
                        {
                            Graphics.snap = 1f;
                            break;
                        }
                        if (Graphics._screenBufferTarget.width < 800)
                        {
                            Graphics.snap = 2f;
                            break;
                        }
                        break;
                }
                MonoMain._screenCapture = new RenderTarget2D(current.x, current.y, true);
                MonoMain.RetakePauseCapture();
                LayerCore.ReinitializeLightingTargets();
                Options.ResolutionChanged();
                if (NetworkDebugger.enabled)
                    NetworkDebugger.instance.RefreshRectSizes();
                if (Layer.Game != null && Layer.Game.camera != null)
                    Layer.Game.camera.DoUpdate();
                if (Program.isLinux)
                    _takeFocus = 10;
                _pendingResolution = null;
                Graphics._screenViewport = new Viewport?();
            }
        }

        public static bool Update()
        {
            if (_takeFocus > 0)
            {
                --_takeFocus;
                if (_takeFocus == 0)
                    FNAPlatform.RaiseWindow(MonoMain.instance.Window.Handle);
                FNAPlatform.SetWindowInputFocus(MonoMain.instance.Window.Handle);
                //Resolution._window.Focus();
            }
            if (_pendingResolution == null)
                return false;
            Apply();
            return true;
        }

        public static Matrix getTransformationMatrix()
        {
            return Matrix.CreateScale(Graphics.viewport.Width / (float)MonoMain.screenWidth, Graphics.viewport.Height / (float)MonoMain.screenHeight, 1f);
        }

        public float aspect => dimensions.x / dimensions.y;

        public int x
        {
            get => (int)dimensions.x;
            set => dimensions.x = value;
        }

        public int y
        {
            get => (int)dimensions.y;
            set => dimensions.y = value;
        }

        public static void Initialize(object pWindow, GraphicsDeviceManager pDeviceManager)
        {
            _window = (IntPtr)pWindow;
            _device = pDeviceManager;
            supportedDisplaySizes = new Dictionary<ScreenMode, List<Resolution>>();
            DevConsole.Log(DCSection.General, "Enumerating display modes (" + GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.Count().ToString() + " found...)");
            if (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode == null)
                throw new Exception("No graphics display modes found, your graphics card may not be supported!");
            DevConsole.Log(DCSection.General, "Default adapter size is (" + GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width.ToString() + "x" + GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height.ToString() + ")");
            DevConsole.Log(DCSection.General, "Registered adapter size is (" + MonoMain.instance._adapterW.ToString() + "x" + MonoMain.instance._adapterH.ToString() + ")");
            RegisterDisplaySize(ScreenMode.Fullscreen, new Resolution()
            {
                dimensions = new Vec2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)
            }, false);
            RegisterDisplaySize(ScreenMode.Borderless, new Resolution()
            {
                dimensions = new Vec2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)
            }, false);
            RegisterDisplaySize(ScreenMode.Windowed, new Resolution()
            {
                dimensions = new Vec2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)
            }, false);
            foreach (DisplayMode supportedDisplayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                RegisterDisplaySize(ScreenMode.Fullscreen, new Resolution()
                {
                    dimensions = new Vec2(supportedDisplayMode.Width, supportedDisplayMode.Height)
                }, false);
                if (supportedDisplayMode.Width <= MonoMain.instance._adapterW && supportedDisplayMode.Height <= MonoMain.instance._adapterH)
                {
                    RegisterDisplaySize(ScreenMode.Windowed, new Resolution()
                    {
                        dimensions = new Vec2(supportedDisplayMode.Width, supportedDisplayMode.Height)
                    }, false);
                    RegisterDisplaySize(ScreenMode.Borderless, new Resolution()
                    {
                        dimensions = new Vec2(supportedDisplayMode.Width, supportedDisplayMode.Height)
                    }, false);
                }
            }
            RegisterDisplaySize(ScreenMode.Borderless, new Resolution()
            {
                dimensions = new Vec2(640f, 360f)
            }, false);
            RegisterDisplaySize(ScreenMode.Borderless, new Resolution()
            {
                dimensions = new Vec2(320f, 180f)
            }, false);
            RegisterDisplaySize(ScreenMode.Windowed, new Resolution()
            {
                dimensions = new Vec2(1280f, 720f)
            }, false, true);
            RegisterDisplaySize(ScreenMode.Windowed, new Resolution()
            {
                dimensions = new Vec2(1920f, 1080f)
            }, false, true);
            RegisterDisplaySize(ScreenMode.Windowed, new Resolution()
            {
                dimensions = new Vec2(2560f, 1440f)
            }, false, true);
            RegisterDisplaySize(ScreenMode.Windowed, new Resolution()
            {
                dimensions = new Vec2(2880f, 1620f)
            }, false, true);
            DevConsole.Log(DCSection.General, "Finished enumerating display modes (F(" + supportedDisplaySizes[ScreenMode.Fullscreen].Count.ToString() + ") W(" + supportedDisplaySizes[ScreenMode.Windowed].Count.ToString() + ") B(" + supportedDisplaySizes[ScreenMode.Borderless].Count.ToString() + "))");
            SortDisplaySizes();
            string[] strArray = new string[7]
            {
                "Finished sorting display modes (F(",
                supportedDisplaySizes[ScreenMode.Fullscreen].Count.ToString(),
                ") W(",
                null,
                null,
                null,
                null
            };
            int count = supportedDisplaySizes[ScreenMode.Windowed].Count;
            strArray[3] = count.ToString();
            strArray[4] = ") B(";
            count = supportedDisplaySizes[ScreenMode.Borderless].Count;
            strArray[5] = count.ToString();
            strArray[6] = "))";
            DevConsole.Log(DCSection.General, string.Concat(strArray));
            if (supportedDisplaySizes[ScreenMode.Windowed].Count == 0 && supportedDisplaySizes[ScreenMode.Fullscreen].Count > 0)
            {
                Resolution resolution = supportedDisplaySizes[ScreenMode.Fullscreen].LastOrDefault();
                RegisterDisplaySize(ScreenMode.Windowed, new Resolution()
                {
                    dimensions = resolution.dimensions
                }, false, pForce: true);
                RegisterDisplaySize(ScreenMode.Borderless, new Resolution()
                {
                    dimensions = resolution.dimensions
                }, false, pForce: true);
            }
            FindNearest(ScreenMode.Fullscreen, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height).isDefault = true;
            FindNearest(ScreenMode.Windowed, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height, 1.7777f, true).isDefault = true;
            FindNearest(ScreenMode.Borderless, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height).isDefault = true;
            RestoreDefaults();
            try
            {
                _screenDPI = GetScreenDPI();
            }
            catch (Exception)
            {
                _screenDPI = 120f;
            }
        }

        public static void RestoreDefaults()
        {
            Options.LocalData.fullscreenResolution = GetDefault(ScreenMode.Fullscreen);
            Options.LocalData.windowedResolution = GetDefault(ScreenMode.Windowed);
            Options.LocalData.windowedFullscreenResolution = GetDefault(ScreenMode.Borderless);
        }

        public override string ToString() => x.ToString() + "x" + y.ToString() + "x" + ((int)mode).ToString();

        public string ToShortString()
        {
            int num = x;
            string str1 = num.ToString();
            num = y;
            string str2 = num.ToString();
            return str1 + "x" + str2;
        }

        public static Resolution Load(string pSize, string pMemberName = null)
        {
            if (supportedDisplaySizes == null)
                return null;
            try
            {
                string[] strArray = pSize.ToLower().Trim().Split('x');
                if (strArray.Length >= 2)
                {
                    int int32_1 = Convert.ToInt32(strArray[0]);
                    int int32_2 = Convert.ToInt32(strArray[1]);
                    ScreenMode pMode = ScreenMode.Windowed;
                    if (strArray.Length == 3)
                        pMode = (ScreenMode)Convert.ToInt32(strArray[2]);
                    else if (pMemberName == "windowedResolution")
                        pMode = ScreenMode.Windowed;
                    else if (pMemberName == "windowedFullscreenResolution")
                        pMode = ScreenMode.Borderless;
                    else if (pMemberName == "fullscreenResolution")
                        pMode = ScreenMode.Fullscreen;
                    Resolution nearest = FindNearest(pMode, int32_1, int32_2);
                    if (nearest != null)
                        return nearest;
                }
            }
            catch (Exception)
            {
                DevConsole.Log(DCSection.General, "Failed to load resolution (" + pSize + ")");
            }
            return null;
        }

        public static Resolution GetDefault(ScreenMode pMode) => supportedDisplaySizes[pMode].FirstOrDefault(x => x.isDefault) ?? supportedDisplaySizes[pMode].Last();

        public static Resolution FindNearest(
          ScreenMode pMode,
          int pX,
          int pY,
          float pAspect = -1f,
          bool pRecommended = false)
        {
            Resolution nearestInternal = FindNearest_Internal(pMode, pX, pY, pAspect, pRecommended);
            if (nearestInternal == null)
            {
                ScreenMode pMode1 = (ScreenMode)((int)(pMode + 1) % 3);
                while (pMode1 != pMode && nearestInternal == null)
                    nearestInternal = FindNearest_Internal(pMode1, pX, pY, pAspect, pRecommended);
            }
            if (nearestInternal == null)
                DevConsole.Log(DCSection.General, "Failed to find display mode (" + pMode.ToString() + ", " + pX.ToString() + "x" + pY.ToString() + ")");
            return nearestInternal;
        }

        private static Resolution FindNearest_Internal(
          ScreenMode pMode,
          int pX,
          int pY,
          float pAspect = -1f,
          bool pRecommended = false)
        {
            Resolution nearestInternal = supportedDisplaySizes[pMode].LastOrDefault();
            foreach (Resolution resolution in supportedDisplaySizes[pMode])
            {
                float num1 = Math.Abs(resolution.aspect - pAspect);
                float num2 = Math.Abs(nearestInternal.aspect - pAspect);
                bool flag1 = num1 < num2 && Math.Abs(num1 - num2) > 0.05f;
                bool flag2 = Math.Abs(num1 - num2) < 0.05f;
                if ((nearestInternal == null || Math.Abs(resolution.x - pX) + Math.Abs(resolution.y - pY) < Math.Abs(nearestInternal.x - pX) + Math.Abs(nearestInternal.y - pY) || flag1 && pAspect > 0 || pRecommended && resolution.recommended && !nearestInternal.recommended) && (nearestInternal == null || pAspect < 0f || flag1 | flag2 && (resolution.recommended || !pRecommended)))
                    nearestInternal = resolution;
            }
            return nearestInternal;
        }

        internal static Resolution RegisterDisplaySize(
          ScreenMode pMode,
          Resolution pResolution,
          bool pSort = true,
          bool pRecommended = false,
          bool pForce = false)
        {
            List<Resolution> resolutionList = null;
            if (!supportedDisplaySizes.TryGetValue(pMode, out resolutionList))
                supportedDisplaySizes[pMode] = resolutionList = new List<Resolution>();
            if (pResolution.x > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width || pResolution.y > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)
            {
                DevConsole.Log("Invalid resolution (" + pResolution.ToString() + "): Larger than adapter (" + GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width.ToString() + "x" + GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height.ToString() + ")");
                return null;
            }
            if (pMode == ScreenMode.Windowed && pResolution.x == MonoMain.instance._adapterW && pResolution.y == MonoMain.instance._adapterH && !pForce)
            {
                DevConsole.Log("Invalid resolution (" + pResolution.ToString() + "): Windowed resolution must not equal adapter size (" + GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width.ToString() + "x" + GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height.ToString() + ")");
                return null;
            }
            Resolution resolution = resolutionList.FirstOrDefault(x => x.x == pResolution.x && x.y == pResolution.y);
            if (resolution == null)
                resolutionList.Add(pResolution);
            else
                pResolution = resolution;
            if (pSort)
                supportedDisplaySizes[pMode] = SortDisplaySizes(resolutionList);
            pResolution.mode = pMode;
            pResolution.recommended = pRecommended;
            return pResolution;
        }

        internal static void SortDisplaySizes()
        {
            supportedDisplaySizes[ScreenMode.Windowed] = SortDisplaySizes(supportedDisplaySizes[ScreenMode.Windowed]);
            supportedDisplaySizes[ScreenMode.Borderless] = SortDisplaySizes(supportedDisplaySizes[ScreenMode.Borderless]);
            supportedDisplaySizes[ScreenMode.Fullscreen] = SortDisplaySizes(supportedDisplaySizes[ScreenMode.Fullscreen]);
        }

        internal static List<Resolution> SortDisplaySizes(List<Resolution> pList) => pList == null ? new List<Resolution>() : pList.OrderBy(x => x.x * 100 + x.y).ToList();
    }
}
