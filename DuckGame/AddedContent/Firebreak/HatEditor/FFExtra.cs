namespace DuckGame
{
    public partial class FeatherFashion
    {
        public static class FFColors
        {
            public static Color Primary = new("78909c");
            public static Color PrimaryHighlight = new("b0bec5");
            public static Color PrimaryDim = new("546e7a");
            
            public static Color Background = new("37474f");
            public static Color Focus = new("ffffff");
            public static Color Outline = new("000000");
            
            public static Color CanvasEmpty1 = new("c4c4c4");
            public static Color CanvasEmpty2 = new("808080");
        }
        
        public static class FFIcons
        {
            public static SpriteMap AddMetapixelButton;
            public static SpriteMap Brush;
            public static SpriteMap Cape;
            public static SpriteMap ColorPicker;
            public static SpriteMap Configuration;
            public static SpriteMap Copy;
            public static SpriteMap Eraser;
            public static SpriteMap FirstFrameOfTwo;
            public static SpriteMap SecondFrameOfTwo;
            public static SpriteMap FirstFrameOfFour;
            public static SpriteMap SecondFrameOfFour;
            public static SpriteMap ThirdFrameOfFour;
            public static SpriteMap FourthFrameOfFour;
            public static SpriteMap Hat;
            public static SpriteMap IncrementValueButton;
            public static SpriteMap DecrementValueButton;
            public static SpriteMap Loop;
            public static SpriteMap Metadata;
            public static SpriteMap PageNextButton;
            public static SpriteMap PagePreviousButton;
            public static SpriteMap Particles;
            public static SpriteMap Pause;
            public static SpriteMap Play;
            public static SpriteMap PlayPause;
            public static SpriteMap PlayTest;
            public static SpriteMap Kill;
            public static SpriteMap SpawnProp;
            public static SpriteMap SpawnRandom;
            public static SpriteMap Fullscreen;
            public static SpriteMap Rock;
            public static SpriteMap NextAnimation;
            public static SpriteMap PreviousAnimation;
            public static SpriteMap GlobalActionSwitchEditor;
            public static SpriteMap GlobalActionSwitchPreview;
            public static SpriteMap GlobalActionImport;
            public static SpriteMap GlobalActionSave;
            public static SpriteMap GlobalActionLeave;
            
            public static Sprite CanvasBig;
            public static Sprite CanvasSmall;
            public static Sprite EditorSwitchersFrame;
            public static Sprite CanvasToolsFrame;
            public static Sprite AnimationControlsBig;
            public static Sprite AnimationControlsSmall;
            public static Sprite PreviewPaneFrames;
            public static Sprite FFLogo_Beta;
            public static Sprite FFLogo;
            public static Sprite Watermark;
            public static Sprite Watermark_Beta;
            
            public static Sprite UploadBackground;
            public static Sprite UploadHatFrame;

            public static void Initialize()
            {
                AddMetapixelButton = new SpriteMap("ff_icons/add_metapixel", 68, 12);
                Brush = new SpriteMap("ff_icons/brush", 8, 8);
                Cape = new SpriteMap("ff_icons/cape", 8, 8);
                ColorPicker = new SpriteMap("ff_icons/colorPicker", 8, 8);
                Configuration = new SpriteMap("ff_icons/config", 8, 8);
                Copy = new SpriteMap("ff_icons/copy", 8, 8);
                Eraser = new SpriteMap("ff_icons/eraser", 8, 8);
                FirstFrameOfTwo = new SpriteMap("ff_icons/frame_big_1", 44, 8);
                SecondFrameOfTwo = new SpriteMap("ff_icons/frame_big_2", 44, 8);
                FirstFrameOfFour = new SpriteMap("ff_icons/frame_small_1", 20, 8);
                SecondFrameOfFour = new SpriteMap("ff_icons/frame_small_2", 20, 8);
                ThirdFrameOfFour = new SpriteMap("ff_icons/frame_small_3", 20, 8);
                FourthFrameOfFour = new SpriteMap("ff_icons/frame_small_4", 20, 8);
                Hat = new SpriteMap("ff_icons/hat", 8, 8);
                IncrementValueButton = new SpriteMap("ff_icons/inc", 5, 4);
                DecrementValueButton = new SpriteMap("ff_icons/inc_flipV", 5, 4);
                Loop = new SpriteMap("ff_icons/loop", 8, 8);
                Metadata = new SpriteMap("ff_icons/metadata", 8, 8);
                PageNextButton = new SpriteMap("ff_icons/pagemove", 6, 24);
                PagePreviousButton = new SpriteMap("ff_icons/pagemove_flipH", 6, 24);
                Particles = new SpriteMap("ff_icons/particles", 8, 8);
                Pause = new SpriteMap("ff_icons/pause", 8, 8);
                Play = new SpriteMap("ff_icons/play", 8, 8);
                PlayPause = new SpriteMap("ff_icons/playpause", 8, 8);
                PlayTest = new SpriteMap("ff_icons/playetest", 10, 8);
                Kill = new SpriteMap("ff_icons/kill", 8, 8);
                SpawnProp = new SpriteMap("ff_icons/spawnprop", 8, 8);
                SpawnRandom = new SpriteMap("ff_icons/spawnrandom", 8, 8);
                Fullscreen = new SpriteMap("ff_icons/fullscreenMinimize", 8, 8);
                Rock = new SpriteMap("ff_icons/rock", 8, 8);
                NextAnimation = new SpriteMap("ff_icons/skip", 8, 8);
                PreviousAnimation = new SpriteMap("ff_icons/skip", 8, 8) { flipH = true, center = (8, 0) };
                GlobalActionSwitchEditor = new SpriteMap("ff_icons/mode_editor", 48, 11);
                GlobalActionSwitchPreview = new SpriteMap("ff_icons/mode_preview", 48, 11);
                GlobalActionImport = new SpriteMap("ff_icons/menu_import", 48, 11);
                GlobalActionSave = new SpriteMap("ff_icons/menu_save", 48, 11);
                GlobalActionLeave = new SpriteMap("ff_icons/menu_leave", 48, 11);

                CanvasBig = new Sprite("ff_icons/canvas32");
                CanvasSmall = new Sprite("ff_icons/canvas12_24");
                EditorSwitchersFrame = new Sprite("ff_icons/editorSwitcher");
                CanvasToolsFrame = new Sprite("ff_icons/canvasTools");
                AnimationControlsBig = new Sprite("ff_icons/animationControls32");
                AnimationControlsSmall = new Sprite("ff_icons/animationControls12_24");
                PreviewPaneFrames = new Sprite("ff_icons/previewPane_frames");
                FFLogo_Beta = new Sprite("ff_icons/FFLogo_beta");
                FFLogo = new Sprite("ff_icons/FFLogo");
                Watermark = new Sprite("ff_icons/watermark");
                Watermark_Beta = new Sprite("ff_icons/watermark_beta");
                
                UploadBackground = new Sprite("ff_icons/upload/bg");
                UploadHatFrame = new Sprite("ff_icons/upload/hatframe");
            }
        }

        public enum EditorMode
        {
            Hat,
            Cape,
            Rock,
            Particle,
            Metapixel
        }

        public enum MetapixelEditorMode
        {
            MyMetapixelList,
            EditMetapixel,
            NewMetapixel,
        }

        public enum CanvasTool
        {
            Brush,
            Eraser,
            ColorPicker
        }

        public enum WorkMode
        {
            Editor,
            Preview
        }

        // yes.
        public static Color[] DefaultRockBuffer =
        {
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 255), new(0, 0, 0, 255), new(0, 0, 0, 255),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 255), new(0, 0, 0, 255), new(0, 0, 0, 255),
            new(0, 0, 0, 255), new(96, 119, 124, 255), new(184, 184, 184, 255),
            new(184, 184, 184, 255), new(0, 0, 0, 255), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 255), new(96, 119, 124, 255),
            new(157, 157, 157, 255), new(184, 184, 184, 255), new(157, 157, 157, 255),
            new(184, 184, 184, 255), new(157, 157, 157, 255), new(157, 157, 157, 255),
            new(184, 184, 184, 255), new(0, 0, 0, 255), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 255),
            new(96, 119, 124, 255), new(96, 119, 124, 255), new(96, 119, 124, 255),
            new(157, 157, 157, 255), new(157, 157, 157, 255), new(157, 157, 157, 255),
            new(157, 157, 157, 255), new(157, 157, 157, 255), new(157, 157, 157, 255),
            new(184, 184, 184, 255), new(0, 0, 0, 255), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 255), new(96, 119, 124, 255),
            new(96, 119, 124, 255), new(96, 119, 124, 255), new(96, 119, 124, 255),
            new(157, 157, 157, 255), new(157, 157, 157, 255), new(157, 157, 157, 255),
            new(157, 157, 157, 255), new(157, 157, 157, 255), new(157, 157, 157, 255),
            new(184, 184, 184, 255), new(0, 0, 0, 255), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 255), new(96, 119, 124, 255), new(96, 119, 124, 255),
            new(96, 119, 124, 255), new(96, 119, 124, 255), new(157, 157, 157, 255),
            new(96, 119, 124, 255), new(157, 157, 157, 255), new(157, 157, 157, 255),
            new(157, 157, 157, 255), new(157, 157, 157, 255), new(157, 157, 157, 255),
            new(157, 157, 157, 255), new(0, 0, 0, 255), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 255),
            new(96, 119, 124, 255), new(96, 119, 124, 255), new(96, 119, 124, 255),
            new(96, 119, 124, 255), new(157, 157, 157, 255), new(96, 119, 124, 255),
            new(96, 119, 124, 255), new(157, 157, 157, 255), new(157, 157, 157, 255),
            new(157, 157, 157, 255), new(157, 157, 157, 255), new(96, 119, 124, 255),
            new(157, 157, 157, 255), new(184, 184, 184, 255), new(0, 0, 0, 255),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 255), new(96, 119, 124, 255), new(96, 119, 124, 255),
            new(96, 119, 124, 255), new(157, 157, 157, 255), new(157, 157, 157, 255),
            new(157, 157, 157, 255), new(96, 119, 124, 255), new(96, 119, 124, 255),
            new(157, 157, 157, 255), new(157, 157, 157, 255), new(157, 157, 157, 255),
            new(157, 157, 157, 255), new(157, 157, 157, 255), new(157, 157, 157, 255),
            new(0, 0, 0, 255), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 255), new(96, 119, 124, 255),
            new(96, 119, 124, 255), new(157, 157, 157, 255), new(157, 157, 157, 255),
            new(157, 157, 157, 255), new(157, 157, 157, 255), new(157, 157, 157, 255),
            new(96, 119, 124, 255), new(96, 119, 124, 255), new(96, 119, 124, 255),
            new(157, 157, 157, 255), new(157, 157, 157, 255), new(157, 157, 157, 255),
            new(157, 157, 157, 255), new(0, 0, 0, 255), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 255), new(96, 119, 124, 255),
            new(96, 119, 124, 255), new(157, 157, 157, 255), new(96, 119, 124, 255),
            new(157, 157, 157, 255), new(96, 119, 124, 255), new(157, 157, 157, 255),
            new(157, 157, 157, 255), new(157, 157, 157, 255), new(157, 157, 157, 255),
            new(157, 157, 157, 255), new(157, 157, 157, 255), new(157, 157, 157, 255),
            new(184, 184, 184, 255), new(0, 0, 0, 255), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 255),
            new(96, 119, 124, 255), new(96, 119, 124, 255), new(96, 119, 124, 255),
            new(96, 119, 124, 255), new(157, 157, 157, 255), new(96, 119, 124, 255),
            new(157, 157, 157, 255), new(96, 119, 124, 255), new(157, 157, 157, 255),
            new(157, 157, 157, 255), new(157, 157, 157, 255), new(184, 184, 184, 255),
            new(0, 0, 0, 255), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 255),
            new(96, 119, 124, 255), new(96, 119, 124, 255), new(96, 119, 124, 255),
            new(96, 119, 124, 255), new(96, 119, 124, 255), new(96, 119, 124, 255),
            new(96, 119, 124, 255), new(96, 119, 124, 255), new(96, 119, 124, 255),
            new(96, 119, 124, 255), new(96, 119, 124, 255), new(0, 0, 0, 255),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 255), new(0, 0, 0, 255), new(0, 0, 0, 255), new(0, 0, 0, 255),
            new(0, 0, 0, 255), new(0, 0, 0, 255), new(0, 0, 0, 255), new(0, 0, 0, 255),
            new(0, 0, 0, 255), new(0, 0, 0, 255), new(0, 0, 0, 255), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0), new(0, 0, 0, 0),
            new(0, 0, 0, 0), new(0, 0, 0, 0),
        };
    }
}