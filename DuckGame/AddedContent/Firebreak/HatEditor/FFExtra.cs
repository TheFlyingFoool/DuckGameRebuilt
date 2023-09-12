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
            public static SpriteMap PlayTest;
            public static SpriteMap Rock;
            public static SpriteMap NextAnimation;
            public static SpriteMap PreviousAnimation;
            
            public static Sprite CanvasBig;
            public static Sprite CanvasSmall;
            public static Sprite EditorSwitchersFrame;
            public static Sprite CanvasToolsFrame;
            public static Sprite AnimationControlsBig;
            public static Sprite AnimationControlsSmall;
            public static Sprite PreviewPaneFrames;

            public static void Initialize()
            {
                AddMetapixelButton = new SpriteMap("ff_icons/add_metapixel", 68, 12);
                Brush = new SpriteMap("ff_icons/brush", 8, 8);
                Cape = new SpriteMap("ff_icons/cape", 8, 8);
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
                PlayTest = new SpriteMap("ff_icons/playetest", 24, 8);
                Rock = new SpriteMap("ff_icons/rock", 8, 8);
                NextAnimation = new SpriteMap("ff_icons/skip", 8, 8);
                PreviousAnimation = new SpriteMap("ff_icons/skip_flipH", 8, 8);

                CanvasBig = new Sprite("ff_icons/canvas32");
                CanvasSmall = new Sprite("ff_icons/canvas12_24");
                EditorSwitchersFrame = new Sprite("ff_icons/editorSwitcher");
                CanvasToolsFrame = new Sprite("ff_icons/canvasTools");
                AnimationControlsBig = new Sprite("ff_icons/animationControls32");
                AnimationControlsSmall = new Sprite("ff_icons/animationControls12_24");
                PreviewPaneFrames = new Sprite("ff_icons/previewPane_frames");
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

        public enum CanvasTool
        {
            Brush,
            Eraser
        }

        public enum WorkMode
        {
            Editor,
            Preview
        }
    }
}