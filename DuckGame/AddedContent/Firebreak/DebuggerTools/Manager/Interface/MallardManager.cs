using DuckGame.ConsoleEngine;
using DuckGame.MMConfig;
using DuckGame.ConsoleInterface.Panes;
using System.IO;
using System.Windows.Forms;

namespace DuckGame.ConsoleInterface
{
    public static class MallardManager
    {
        public static MallardManagerConfig Config;
        public static MallardManagerPane DisplayPane;
        public static MallardPointer Pointer => GameCursor.CurrentCursor as MallardPointer;
        
        private static bool s_open;
        public static bool Open
        {
            get => Config.Enabled && s_open;
            set
            {
                if (value && LockMovementQueue.TryAdd("mallardManager"))
                {
                    s_open = true;
                    OnOpen();
                }
                else if (!value && LockMovementQueue.TryRemove("mallardManager"))
                {
                    s_open = false;
                    OnClose();
                }
                else return;
            }
        }

        private static void OnClose()
        {
            GameCursor.CurrentCursor = null;
        }

        private static void OnOpen()
        {
            GameCursor.CurrentCursor = new MallardPointer();
            DisplayPane.OnFocus();

            if (Config.Zoom <= 0)
                Config.Zoom = Graphics.height / 540f;
        }

        [PostInitialize(Priority = 0)]
        public static void Initialize()
        {
            LoadConfig();
            EnsureScriptsFolder();

            DisplayPane = new MMConsolePane();
        }

        private static void EnsureScriptsFolder()
        {
            string dirPath = Commands.ScriptsDirPath;
            
            if (Directory.Exists(dirPath))
                return;

            Directory.CreateDirectory(dirPath);
        }

        [DrawingContext(CustomID = "mmUpdate")]
        public static void Update()
        {
            if (OnKeybindAttribute.IsActive(Config.Keymap.General.ToggleMallardManager))
            {
                DevConsoleCommands.MMToggle();
                return;
            }
            
            if (!Open)
                return;

            if (DisplayPane.SwitchToPane is not null)
                DisplayPane = DisplayPane.SwitchToPane;
            
            if (!DisplayPane.Active)
            {
                Initialize();
                Open = false;
                return;
            }
            
            DisplayPane.Update();
            
            if (OnKeybindAttribute.IsActive(Config.Keymap.General.ReloadConfig))
            {
                Initialize();
                OnOpen();
            }
            
            if (Keyboard.control)
            {
                if (OnKeybindAttribute.IsActive(Config.Keymap.General.ZoomIn))
                    Config.Zoom++;
                else if (OnKeybindAttribute.IsActive(Config.Keymap.General.ZoomOut))
                    Config.Zoom = Config.Zoom - 1 <= 0 ? Config.Zoom : Config.Zoom - 1;
                else if (OnKeybindAttribute.IsActive(Config.Keymap.General.ResetZoom))
                    Config.Zoom = 1f;

                // if (OnKeybindAttribute.IsActive(Keys.OemOpenBrackets))
                //     DisplayPane = new MMSplitPane(Orientation.Horizontal, DisplayPane, new MMConsolePane());
                // else if (OnKeybindAttribute.IsActive(Keys.OemCloseBrackets))
                //     DisplayPane = new MMSplitPane(Orientation.Vertical, DisplayPane, new MMConsolePane());
            }
        }

        [DrawingContext(DrawingLayer.Console, CustomID = "mmDraw")]
        public static void Draw()
        {
            if (!Open)
                return;

            float zoom = Config.Zoom;
            
            Rectangle bounds = new(0, 0, Layer.Console.width, Layer.Console.height);

            DisplayPane.Bounds = bounds;
            DisplayPane.Draw(1.5f, zoom);
        }

        private static void LoadConfig()
        {
            Config = AdvancedConfig.Get<MallardManagerConfig>();
        }

        public static class Colors
        {
            public static Color UserText => (Color) Config.Colors.UserText * Config.Opacity;
            public static Color PrimarySystemText => (Color) Config.Colors.PrimarySystemText * Config.Opacity;
            public static Color SecondarySystemText => (Color) Config.Colors.SecondarySystemText * Config.Opacity;
            public static Color UserOverlay => (Color) Config.Colors.UserOverlay * Config.Opacity;
            public static Color Primary => (Color) Config.Colors.Primary * Config.Opacity;
            public static Color PrimarySub => (Color) Config.Colors.PrimarySub * Config.Opacity;
            public static Color Secondary => (Color) Config.Colors.Secondary * Config.Opacity;
            public static Color SecondarySub => (Color) Config.Colors.SecondarySub * Config.Opacity;
            public static Color PrimaryBackground => (Color) Config.Colors.PrimaryBackground * Config.Opacity;
            public static Color SecondaryBackground => (Color) Config.Colors.SecondaryBackground * Config.Opacity;
        }
    }
}