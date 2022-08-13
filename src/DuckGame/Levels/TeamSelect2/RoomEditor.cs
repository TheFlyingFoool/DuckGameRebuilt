// Decompiled with JetBrains decompiler
// Type: DuckGame.RoomEditor
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class RoomEditor : Thing
    {
        private float _fade;
        private bool _open;
        private bool _closing;
        private float _takenFlash;
        public REMode _mode;
        private REMode _desiredMode;
        //private InputProfile _inputProfile;
        private Profile _profile;
        private BitmapFont _font;
        //private BitmapFont _smallFont;
        private ProfileBox2 _box;
        private Sprite _scren;
        private Sprite _bigScren;
        private Sprite _whiteCircle;
        //private Sprite _moveArrow;
        private Sprite _cantPlace;
        private Sprite _cantPlaceLarge;
        private Sprite _furnitureCursor;
        //private Profile _starterProfile;
        private HatSelector _selector;
        private Sprite _furni;
        private FancyBitmapFont _fancyFont = new FancyBitmapFont("smallFont");
        //private UIMenu _confirmMenu;
        private MenuBoolean _deleteProfile = new MenuBoolean();
        private static List<Furniture> allFurnis;
        private static Dictionary<int, Furniture> _furniMap = new Dictionary<int, Furniture>();
        public static Dictionary<FurnitureGroup, List<Furniture>> _furniGroupMap = new Dictionary<FurnitureGroup, List<Furniture>>();
        public short _furniSelection;
        public short _desiredFurniSelection;
        public float _slide;
        public float _slideTo;
        public float _upSlide;
        public float _upSlideTo;
        public Vec2 _furniPos = new Vec2(-30f, -30f);
        public Vec2 _furniCursor = new Vec2(-30f, -30f);
        //private bool upOption;
        //private bool _wasDown;
        //private bool _autoSelect;
        private List<DeviceInputMapping> _pendingMaps = new List<DeviceInputMapping>();
        public FurniturePosition _hover;
        public int _placementVariation;
        public static int maxFurnitures = 25;
        public bool _placementFlip;
        public static int roomSize = 141;
        private bool invalidPlacement;
        //private Vec2 _realFurniPos;
        //private float _moodVal = 0.5f;
        private Material grayscale = new Material("Shaders/greyscale");

        public float fade => _fade;

        public bool open => _open;

        public RoomEditor(float xpos, float ypos, ProfileBox2 box, HatSelector sel)
          : base(xpos, ypos)
        {
            _font = new BitmapFont("biosFontUI", 8, 7)
            {
                scale = new Vec2(0.5f, 0.5f)
            };
            _collisionSize = new Vec2(141f, 89f);
            _cantPlace = new Sprite("cantPlace");
            _cantPlace.CenterOrigin();
            _cantPlaceLarge = new Sprite("cantPlaceLarge");
            _cantPlaceLarge.CenterOrigin();
            _box = box;
            _selector = sel;
            //this._moveArrow = new Sprite("moveArrow")
            //{
            //    center = new Vec2(0f, 4f)
            //};
            _scren = new Sprite("furni/scren");
            _scren.CenterOrigin();
            _bigScren = new Sprite("furni/bigScren");
            _bigScren.CenterOrigin();
            _whiteCircle = new Sprite("furni/whiteCircle");
            _whiteCircle.CenterOrigin();
            //this._smallFont = new BitmapFont("smallBiosFont", 7, 6)
            //{
            //    scale = new Vec2(0.5f, 0.5f)
            //};
            _furni = new Sprite("furni/stone");
            _furni.center = new Vec2(_furni.width / 2, _furni.height);
            _furnitureCursor = new Sprite("arcade/furnCursor");
            _furnitureCursor.CenterOrigin();
        }

        public override void Initialize() => base.Initialize();

        public void Reset()
        {
            _open = false;
            _selector.fade = 1f;
            _fade = 0f;
            _desiredFurniSelection = 0;
            _furniSelection = 0;
            _mode = REMode.Main;
            _desiredMode = _mode;
            _selector.screen.DoFlashTransition();
        }

        public static Furniture GetFurniture(int id)
        {
            RoomEditor.AllFurnis();
            Furniture furniture;
            RoomEditor._furniMap.TryGetValue(id, out furniture);
            return furniture;
        }

        public static Furniture GetFurniture(string name)
        {
            RoomEditor.AllFurnis();
            return Utils.FirstOrDefault<Furniture>(RoomEditor.allFurnis, x => x.name == name);
        }

        public static List<Furniture> AllFurnis()
        {
            if (RoomEditor.allFurnis == null)
            {
                RoomEditor.allFurnis = new List<Furniture>()
        {
          new Furniture(true, false, "Sensitive to cold.", Rarity.Common, "achimenes", 15, 22, "achimenes", Furniture.Flowers,  null, false, true),
          new Furniture(true, false, "Lily of the Nile.", Rarity.Rare, "agapanthus", 16, 22, "agapanthus", Furniture.Flowers,  null, true),
          new Furniture(true, false, "Give me armfuls of lillies.", Rarity.Rare, "arum lily", 16, 22, "arum lily", Furniture.Flowers,  null, true),
          new Furniture(true, false, "Shade hardy flowers.", Rarity.Rare, "astilbe", 16, 22, "astilbe", Furniture.Flowers,  null, true),
          new Furniture(true, false, "Braided ficus' are a big deal.", Rarity.Rare, "braided ficus", 20, 29, "braided ficus", Furniture.Flowers,  null, true),
          new Furniture(true, false, "Flowers from SPACE!", Rarity.Rare, "gloxinia", 16, 22, "gloxinia", Furniture.Flowers,  null, true),
          new Furniture(true, false, "Blooms in the winter but dies in the cold.", Rarity.Rare, "lachenalia", 17, 22, "lachenalia", Furniture.Flowers,  null, false, true),
          new Furniture(true, false, "Not necessarily a narcissist.", Rarity.VeryRare, "narcissus", 16, 22, "NARCISSUS", Furniture.Flowers,  null, true),
          new Furniture(true, false, "Say it with me, OR. KID.", Rarity.UltraRare, "orchid", 16, 22, "orchid", Furniture.Flowers,  null, true),
          new Furniture(true, false, "pel-ar-GO-nee-um", Rarity.Common, "pelargonium", 16, 22, "pelargonium", Furniture.Flowers,  null, true),
          new Furniture(true, false, "Not saliva, salvia.", Rarity.Rare, "salvia", 16, 22, "salvia", Furniture.Flowers,  null, true),
          new Furniture(true, false, "Actually has nothing to do with spiders.", Rarity.Rare, "spiderplant", 19, 22, "spiderplant", Furniture.Flowers,  null, false, true),
          new Furniture(true, false, "Breaking the statice quo.", Rarity.Common, "statice", 16, 22, "statice", Furniture.Flowers,  null, true),
          new Furniture(true, false, "Will probably die indoors.", Rarity.SuperRare, "sunflower", 18, 30, "sunflower", Furniture.Flowers,  null, true),
          new Furniture(true, false, "Butterflies like these flowers.", Rarity.Common, "trachelium", 16, 22, "trachelium", Furniture.Flowers,  null, true),
          new Furniture(true, false, "Get ready for tulip fest!", Rarity.VeryRare, "tulip", 16, 22, "tulip", Furniture.Flowers,  null, true),
          new Furniture(true, false, "AKA ‘George Lily", Rarity.VeryRare, "vallota", 16, 22, "VALLOTA", Furniture.Flowers,  null, true),
          new Furniture(true, false, "Keep away from moths.", Rarity.Rare, "verbascum", 16, 22, "verbascum", Furniture.Flowers,  null, true),
          new Furniture(true, false, "Bringer of life to Flowers everywhere.", Rarity.Rare, "wateringcan", 16, 8, "WATERING CAN", Furniture.Flowers,  null, true),
          new Furniture(true, false, "Yucca Yucca", Rarity.Rare, "Yucca Branch", 16, 32, "YUCCA BRANCH", Furniture.Flowers,  null, true),
          new Furniture(false, false, "How floral.", Rarity.VeryVeryRare,  null, 0, 0, "ITALO FONT", Furniture.Flowers, new SpriteMap("furni/fontIcon", 24, 24), FurnitureType.Font, new BitmapFont("furni/italFont", 8)
          {
            fallbackIndex = 249
          }),
          new Furniture(true, false, "Bathe yourself in luxury.", Rarity.VeryVeryRare, "tub", 40, 24, "FANCY TUB", Furniture.Fancy,  null, true),
          new Furniture(false, false, "Stunning.", Rarity.VeryVeryRare, "mirror", 17, 16, "FANCY MIRROR", Furniture.Fancy),
          new Furniture(false, false, "Rustic Style, Evolved*", Rarity.SuperRare, "chandelier", 34, 26, "CHANDELURE", Furniture.Fancy,  null, false, true),
          new Furniture(false, false, "Fancy Fancy.", Rarity.VeryVeryRare, "sink", 17, 14, "FANCY SINK", Furniture.Fancy,  null, false, surface: true, topoff: 4f),
          new Furniture(false, false, "How fabulously legible.", Rarity.Legendary,  null, 0, 0, "FANCY FONT", Furniture.Fancy, new SpriteMap("furni/fontIcon", 24, 24), FurnitureType.Font, new BitmapFont("furni/ornateFont", 8)
          {
            fallbackIndex = 249
          }),
          new Furniture(false, false, "Feel poor no more.", Rarity.VerySuperRare, "leftRoomFG_opulent", 141, 87, "FANCY ROOM", Furniture.Fancy, new SpriteMap("furni/fancy/fancy", 24, 24), FurnitureType.Theme, bak: "leftRoomBG_opulent"),
          new Furniture(true, false, "Exquisite.", Rarity.VeryVeryRare, "bust", 13, 24, "BUST", Furniture.Fancy,  null, true),
          new Furniture(false, false, "Divine.", Rarity.VeryVeryRare, "fancylamp", 15, 22, "FANCY LAMP", Furniture.Fancy,  null, true),
          new Furniture(false, false, "A remarkable table.", Rarity.VeryVeryRare, "fancytable", 32, 14, "FANCY TABLE", Furniture.Fancy,  null, true, surface: true),
          new Furniture(true, false, "The Duke himself.", Rarity.SuperRare, "portrait", 27, 28, "PORTRAIT", Furniture.Fancy,  null, false),
          new Furniture(true, false, "Very fun to spin around.", Rarity.VeryVeryRare, "globe", 15, 18, "GLOBE", Furniture.Fancy,  null, true),
          new Furniture(false, false, "Doesn’t tell the time anymore. Gramps is retired.", Rarity.UltraRare, "grandfatherclock", 22, 44, "GRANDPA CLOCK", Furniture.Fancy,  null, true),
          new Furniture(false, false, "Move on up to the next tax bracket.", Rarity.SuperRare, "moneybag", 16, 16, "MONEY", Furniture.Fancy,  null, true, surface: true, topoff: 6f),
          new Furniture(false, false, "For when you don’t want to see your food.", Rarity.VeryVeryRare, "platter", 21, 11, "PLATTER", Furniture.Fancy,  null, true),
          new Furniture(false, false, "You probably think this game is about you.", Rarity.VeryVeryRare, "vanity", 23, 31, "VANITY", Furniture.Fancy,  null, true, surface: true, topoff: 14f),
          new Furniture(false, false, "Keeps the riff raff out.", Rarity.UltraRare, "velvetrope", 30, 16, "VELVET ROPE", Furniture.Fancy,  null, true),
          new Furniture(false, false, "This’ll impress your butt.", Rarity.VeryVeryRare, "fancychair", 18, 20, "FANCY CHAIR", Furniture.Fancy,  null, true, surface: true, topoff: 11f),
          new Furniture(false, false, "It’ll turn your frown upside down, also your words.", Rarity.UltraRare,  null, 0, 0, "HANDSTAND FONT", Furniture.Cheap, new SpriteMap("furni/fontIcon", 24, 24), FurnitureType.Font, new BitmapFont("furni/handstandFont", 8)
          {
            fallbackIndex = 249
          }),
          new Furniture(false, false, "Please, use your inside voice.", Rarity.VeryVeryRare,  null, 0, 0, "QUIET FONT", Furniture.Outdoor, new SpriteMap("furni/fontIcon", 24, 24), FurnitureType.Font, new BitmapFont("furni/quietFont", 8)
          {
            fallbackIndex = 249
          }),
          new Furniture(false, false, "SPEAK UP! I CAN’T HEAR YOU!", Rarity.SuperRare,  null, 0, 0, "TINY FONT", Furniture.Everyday, new SpriteMap("furni/fontIcon", 24, 24), FurnitureType.Font, new BitmapFont("furni/tinyFont", 8)
          {
            fallbackIndex = 249
          }),
          new Furniture(false, false, "It’s bad luck to put one on a boat.", Rarity.Rare, "bigsink", 24, 13, "SINK", Furniture.Bathroom,  null, false, surface: true, topoff: 7f),
          new Furniture(true, false, "Tough on grime*", Rarity.Rare, "cleaner", 8, 14, "CLEANER", Furniture.Bathroom,  null, true),
          new Furniture(false, false, "Almost always empty.", Rarity.Rare, "papertowel", 17, 17, "PAPERTOWEL", Furniture.Bathroom,  null, false, surface: true),
          new Furniture(false, false, "Please don’t splash the seat.", Rarity.Rare, "toilet", 17, 20, "TOILET", Furniture.Bathroom,  null, true, surface: true),
          new Furniture(false, false, "Smells great!", Rarity.VeryRare, "urinal", 13, 28, "URINAL", Furniture.Bathroom,  null, false, surface: true),
          new Furniture(true, false, "Shower yourself in modesty.", Rarity.Rare, "showercombo", 36, 40, "SHOWER", Furniture.Bathroom,  null, true, surface: true, topoff: 26f),
          new Furniture(false, false, "Reminds me of home.", Rarity.VeryVeryRare, "leftRoomFG_tiles", 141, 87, "BATH ROOM", Furniture.Bathroom, new SpriteMap("furni/bathroom/tiles", 24, 24), FurnitureType.Theme, bak: "leftRoomBG_tiled"),
          new Furniture(false, false, "Printed on expensive paper.", Rarity.VeryRare, "diploma", 23, 17, "DIPLOMA", Furniture.Cheap,  null, false),
          new Furniture(true, false, "Rather stylish.", Rarity.Rare, "garbo", 27, 16, "GARBAGE", Furniture.Cheap,  null, true, surface: true, topoff: 9f),
          new Furniture(false, false, "Can’t find the lamp shade. Sorry.", Rarity.Rare, "lampNoShade", 9, 22, "OLD LAMP", Furniture.Cheap,  null, true),
          new Furniture(true, false, "It’s not my fault.", Rarity.VeryRare, "broken window", 24, 22, "BAD WINDOW", Furniture.Cheap,  null, false),
          new Furniture(false, false, "Barrels of fun.", Rarity.Common, "barrel", 16, 17, "BARREL", Furniture.Ship,  null, true, surface: true),
          new Furniture(true, false, "Very famous!", Rarity.UltraRare, "famous_elf", 25, 30, "FAMOUS ELF", Furniture.Cheap,  null, true, surface: true, topoff: 14f, canGacha: false),
          new Furniture(false, false, "Not designed for indoor use.", Rarity.Rare, "hobofire", 32, 18, "HOBO FIRE", Furniture.Cheap,  null, true),
          new Furniture(false, false, "A Hole Lot of Fun*", Rarity.Rare, "life_preserver", 17, 17, "DONUT LIFE", Furniture.Ship),
          new Furniture(false, false, "Set sail for adventure...", Rarity.VeryRare, "anchor", 25, 28, "ANCHOR", Furniture.Ship,  null, false),
          new Furniture(false, false, "Literally anything could be inside. But it is empty.", Rarity.Common, "barrel_old", 15, 16, "OLD BARREL", Furniture.Cheap,  null, true, surface: true),
          new Furniture(true, false, "It’s gotta go somewhere.", Rarity.Rare, "fusebox", 23, 20, "FUSE BOX", Furniture.Cheap,  null, false, surface: true),
          new Furniture(false, false, "Lightly used. Almost brand new.", Rarity.Rare, "matress", 40, 9, "MATRESS", Furniture.Cheap,  null, true, surface: true),
          new Furniture(false, false, "A window but on a boat.", Rarity.VeryRare, "porthole", 18, 18, "PORTHOLE", Furniture.Ship),
          new Furniture(false, false, "Comes in sets of four. I only got one.", Rarity.SuperRare, "tire", 16, 16, "TIRE", Furniture.Cheap,  null, true, surface: true),
          new Furniture(false, false, "BOOM! Instant vacation.", Rarity.SuperRare, "inflatablepalmtree", 24, 25, "PALM TREE", Furniture.Cheap,  null, true),
          new Furniture(false, false, "Hey mom, our crap shack’s going to hell.", Rarity.VeryRare, "leftRoomFG_oldwood", 141, 87, "CRAPPY ROOM", Furniture.Cheap, new SpriteMap("furni/cheap/shabby", 24, 24), FurnitureType.Theme, bak: "leftRoomBG_oldwood"),
          new Furniture(true, false, "I dream of rain.", Rarity.VeryRare, "umbrella", 9, 24, "UMBRELLA", Furniture.Everyday),
          new Furniture(false, false, "I don’t even own a coat.", Rarity.VeryRare, "coathanger", 21, 7, "COAT HANGER", Furniture.Everyday),
          new Furniture(false, false, "Taste the WASTE*", Rarity.Common, "wastebasket", 14, 11, "WASTEBIN", Furniture.Everyday,  null, true),
          new Furniture(false, false, "Keep your stuff off the floor.", Rarity.VeryRare, "shelf", 16, 6, "SHELF", Furniture.Everyday,  null, false, surface: true),
          new Furniture(false, false, "Widen your horizon.", Rarity.VeryVeryRare,  null, 0, 0, "WIDE FONT", Furniture.Everyday, new SpriteMap("furni/fontIcon", 24, 24), FurnitureType.Font, new BitmapFont("furni/widefont", 8)
          {
            fallbackIndex = 249
          }),
          new Furniture(false, false, "Take a load off, Fanny.", Rarity.Rare, "armchair", 28, 19, "ARM CHAIR", Furniture.Everyday,  null, true, surface: true, topoff: 8f),
          new Furniture(false, false, "You all ready for this?", Rarity.VeryRare, "backboard", 31, 27, "BACK BOARD", Furniture.Outdoor,  null, false),
          new Furniture(false, false, "Welcome to the big time, kiddo.", Rarity.Common, "big boy table", 36, 16, "BIG BOY TABLE", Furniture.Everyday,  null, true, surface: true),
          new Furniture(false, false, "Lots of good stuff in there.", Rarity.Common, "cabinets", 31, 20, "CABINETS", Furniture.Everyday,  null, false, surface: true),
          new Furniture(false, false, "The stretch limo of armchairs.", Rarity.Rare, "couches", 35, 19, "COUCH", Furniture.Everyday,  null, true, surface: true, topoff: 8f),
          new Furniture(false, false, "Like a regular lamp but bigger.", Rarity.Rare, "fatLamp", 13, 18, "FAT LAMP", Furniture.Everyday,  null, true),
          new Furniture(false, false, "Imagine a wall... but you can see through it.”", Rarity.Common, "framewindow", 24, 24, "FRAME WINDOW", Furniture.Everyday,  null, false),
          new Furniture(false, false, "It’s as cold as ice.", Rarity.VeryRare, "fridge", 24, 36, "FRIDGE", Furniture.Everyday,  null, true, surface: true),
          new Furniture(false, false, "Guarantees a bright future!", Rarity.Common, "lamp", 13, 16, "LAMP", Furniture.Everyday,  null, true),
          new Furniture(false, false, "You’re really cookin’ now.", Rarity.VeryRare, "oven", 24, 30, "OVEN", Furniture.Everyday,  null, true, surface: true, topoff: 1f),
          new Furniture(false, false, "It is just so small.", Rarity.Common, "tiny table", 23, 16, "TINY TABLE", Furniture.Everyday,  null, true, surface: true),
          new Furniture(false, false, "A window into your soul.", Rarity.Rare, "window", 22, 22, "WINDOW", Furniture.Everyday,  null, false),
          new Furniture(false, false, "Intended for important things.", Rarity.SuperRare, "filecabinet", 18, 33, "FILE CABINET", Furniture.Office,  null, true, surface: true),
          new Furniture(false, false, "Take a break. Kick up your feet.", Rarity.Rare, "footstool", 15, 11, "FOOT STOOL", Furniture.Everyday,  null, true, surface: true),
          new Furniture(false, true, "What better way to spell success?", Rarity.SuperRare, "letters_red", 20, 16, "RED LETTER", Furniture.Characters,  null, false),
          new Furniture(false, true, "What better way to spell success?", Rarity.SuperRare, "letters_blue", 20, 16, "BLUE LETTER", Furniture.Characters,  null, false),
          new Furniture(false, true, "What better way to spell success?", Rarity.SuperRare, "letters_white", 20, 16, "WHITE LETTER", Furniture.Characters,  null, false),
          new Furniture(false, true, "What better way to spell success?", Rarity.UltraRare, "letters_purp", 20, 16, "PURPLE LETTER", Furniture.Characters,  null, false),
          new Furniture(false, true, "Count me in!", Rarity.UltraRare, "numbers", 16, 16, "NUMBER", Furniture.Characters,  null, false),
          new Furniture(false, false, "For either cleaning or curling.", Rarity.Common, "strawbroom", 11, 25, "STRAW BROOM", Furniture.Everyday,  null, true),
          new Furniture(true, false, "witch certified", Rarity.Rare, "broom", 13, 25, "BROOM", Furniture.Everyday,  null, true),
          new Furniture(true, false, "Maybe it’s about time you cleaned up your act.", Rarity.Rare, "vacuum", 24, 30, "VACUUM", Furniture.Everyday,  null, true),
          new Furniture(true, false, "Settle your kettle!", Rarity.Common, "kettle", 14, 13, "KETTLE", Furniture.Everyday,  null, true),
          new Furniture(false, false, "When its not locked shouldn’t it be an unlocker???", Rarity.Rare, "locker", 16, 34, "LOCKER", Furniture.Office,  null, true, surface: true),
          new Furniture(true, false, "Don’t worry, you’re still growing.", Rarity.SuperRare, "heightchart", 27, 34, "HEIGHT CHART", Furniture.Everyday,  null, true),
          new Furniture(false, false, "Eye level lighting.", Rarity.Rare, "Floor Lamp", 15, 35, "FLOOR LAMP", Furniture.Everyday,  null, true),
          new Furniture(false, false, "Doesn’t get many channels.", Rarity.VeryRare, "TV", 24, 20, "TV", Furniture.Everyday,  null, true, surface: true),
          new Furniture(true, false, "Take a seat. Professionally.", Rarity.Rare, "officechair", 16, 18, "OFFICE CHAIR", Furniture.Office,  null, true, surface: true, topoff: 9f),
          new Furniture(true, false, "Take a seat.", Rarity.Rare, "chair", 18, 20, "CHAIR", Furniture.Everyday,  null, true, surface: true, topoff: 10f),
          new Furniture(false, false, "Uses nine D cell batteries an hour.", Rarity.Rare, "boombox", 21, 16, "BOOMBOX", Furniture.Everyday,  null, true, surface: true, topoff: 4f),
          new Furniture(false, false, "A cabinet, and a table. What function!", Rarity.Rare, "countercabinet", 31, 23, "COUNTER", Furniture.Everyday,  null, true, surface: true),
          new Furniture(false, false, "Hold all my calls.", Rarity.Common, "deskphone", 17, 12, "DESK PHONE", Furniture.Office,  null, true),
          new Furniture(false, false, "A wristwatch for your room.", Rarity.Common, "wallclock", 16, 16, "WALL CLOCK", Furniture.Everyday),
          new Furniture(false, false, "Very loud.", Rarity.VeryRare, "clockradio", 18, 6, "CLOCK RADIO", Furniture.Everyday,  null, true),
          new Furniture(false, false, "I can’t remember my phone number.", Rarity.VeryRare, "kitchenphone", 12, 21, "WALL PHONE", Furniture.Everyday,  null, false, surface: true),
          new Furniture(false, false, "Right on.", Rarity.Rare, "vent", 18, 13, "VENT", Furniture.Everyday),
          new Furniture(false, false, "Nothing special to see here.", Rarity.VeryRare, "leftRoomFG_basic", 141, 87, "NORMAL ROOM", Furniture.Everyday, new SpriteMap("furni/everyday/basic", 24, 24), FurnitureType.Theme, bak: "leftRoomBG_basic"),
          new Furniture(false, false, "No sharp corners to hurt yourself on.", Rarity.VeryRare, "round table", 31, 16, "ROUND TABLE", Furniture.Bar,  null, true, surface: true),
          new Furniture(false, true, "Got something to say? Say it in neon.", Rarity.VeryRare, "neon", 28, 12, "NEON SIGN", Furniture.Bar,  null, false),
          new Furniture(true, false, "I’ve got my eyes on you.", Rarity.VeryRare, "CCTV", 13, 11, "CCTV", Furniture.Bar,  null, false, true),
          new Furniture(false, false, "No more missing cues.", Rarity.VeryRare, "cuerack", 19, 36, "CUE RACK", Furniture.Bar,  null, false),
          new Furniture(false, false, "*darts not included.", Rarity.SuperRare, "dartboard", 27, 21, "DART BOARD", Furniture.Bar,  null, false, surface: true),
          new Furniture(false, false, "or billiards or snooker table or whatever.", Rarity.SuperRare, "pooltable", 40, 16, "POOL TABLE", Furniture.Bar,  null, true, surface: true),
          new Furniture(false, false, "Well it can be hard to drink in the dark.", Rarity.VeryRare, "pooltablelighting", 33, 20, "BAR LIGHT", Furniture.Bar,  null, false, true),
          new Furniture(false, false, "Not THAT kind of stool.", Rarity.VeryRare, "stool", 15, 16, "BAR STOOL", Furniture.Bar,  null, true, surface: true),
          new Furniture(false, false, "Filled with dew.", Rarity.Rare, "cooler", 26, 14, "COOLER", Furniture.Outdoor,  null, true, surface: true),
          new Furniture(false, false, "Please don’t pour it on people.", Rarity.Rare, "drinkcooler", 21, 19, "DRINK THING", Furniture.Outdoor,  null, true, surface: true),
          new Furniture(false, false, "Now we’re cookin’ with gas!", Rarity.Common, "gasstove", 11, 13, "GAS STOVE", Furniture.Outdoor,  null, true, surface: true),
          new Furniture(false, false, "Attracts bugs.", Rarity.Rare, "lantern", 11, 15, "LANTERN", Furniture.Outdoor,  null, true),
          new Furniture(true, false, "Complete pan-demonium", Rarity.Rare, "pan", 23, 5, "PAN", Furniture.Outdoor,  null, true),
          new Furniture(false, false, "Pop-Tent, Crazy Good.", Rarity.VeryRare, "poptent", 44, 28, "POP TENT", Furniture.Outdoor,  null, true),
          new Furniture(true, false, "Pot-entially useful.", Rarity.Common, "pot", 19, 10, "POT", Furniture.Outdoor,  null, true),
          new Furniture(true, false, "Like a streak of lightning.", Rarity.VeryVeryRare, "bike", 37, 25, "BIKE", Furniture.Outdoor,  null, true),
          new Furniture(true, false, "Doesn’t look like any duck I’ve seen.", Rarity.Common, "flamingo", 22, 25, "FLAMINGO", Furniture.Outdoor,  null, true),
          new Furniture(true, false, "It’s OK to use it indoors.", Rarity.VeryRare, "lawnchair", 17, 17, "LAWN CHAIR", Furniture.Outdoor,  null, true, surface: true, topoff: 9f),
          new Furniture(false, false, "This is meant to be shared. Don’t drink alone.", Rarity.VeryVeryRare, "soda_pop", 8, 17, "COOL DRINK", Furniture.Outdoor,  null, true),
          new Furniture(false, false, "For that vaguely reminiscent of cheese taste.", Rarity.VeryRare, "condiment", 8, 15, "CHEESE GEL", Furniture.Outdoor,  null, true),
          new Furniture(false, false, "5 percent tomato. 95 percent sugar.", Rarity.VeryRare, "redcondiment", 8, 15, "TOMATO GEL", Furniture.Outdoor,  null, true),
          new Furniture(false, false, "Just terrible.", Rarity.VeryRare, "yellowcondiment", 8, 15, "AWFUL SAUCE", Furniture.Outdoor,  null, true),
          new Furniture(false, true, "Stop means STOP, buddy.", Rarity.SuperRare, "stopsign", 19, 19, "STOP SIGN", Furniture.Outdoor,  null, false),
          new Furniture(false, false, "Stands for: Burn Burgers Quickly", Rarity.Rare, "bbq", 16, 22, "BBQ", Furniture.Outdoor,  null, true),
          new Furniture(false, false, "The great outdoors!", Rarity.VeryVeryRare, "leftRoomFG_tree", 141, 87, "FOREST ROOM", Furniture.Outdoor, new SpriteMap("furni/outdoor/forest", 24, 24), FurnitureType.Theme, bak: "leftRoomBG_tree"),
          new Furniture(true, false, "Why can’t we have nice things?", Rarity.VeryRare, "brokepillar", 15, 16, "BAD PILLAR", Furniture.Stone,  null, true),
          new Furniture(false, false, "Time, to die.", Rarity.SuperRare, "graves", 15, 15, "GRAVE", Furniture.Stone,  null, true, surface: true),
          new Furniture(false, false, "A nice thing.", Rarity.VeryRare, "pillar table", 15, 16, "PILLAR", Furniture.Stone,  null, true, surface: true),
          new Furniture(false, false, "Will break if dropped. Guaranteed.", Rarity.VeryRare, "vase", 15, 14, "VASE", Furniture.Stone,  null, true, surface: true),
          new Furniture(false, false, "When one candle just isn’t enough.", Rarity.VeryRare, "candelabra", 17, 18, "CANDELABRA", Furniture.Stone,  null, true),
          new Furniture(true, false, "Duck be nimble, duck be quick.", Rarity.VeryVeryRare, "candlestick", 15, 15, "CANDLE STICK", Furniture.Stone,  null, true),
          new Furniture(false, false, "Just don’t look inside, okay.", Rarity.SuperRare, "casket", 19, 30, "CASKET", Furniture.Stone,  null, true, surface: true),
          new Furniture(false, false, "It’s got your name written all over it.", Rarity.VerySuperRare, "mausoleum", 33, 30, "MAUSOLEUM", Furniture.Stone,  null, true),
          new Furniture(false, false, "More comfortable than a regular rock.", Rarity.VeryVeryRare, "stone bench", 34, 12, "STONE BENCH", Furniture.Stone,  null, true, surface: true),
          new Furniture(false, false, "This is a safe place.", Rarity.SuperRare, "leftRoomFG_oldstone", 141, 87, "STONE ROOM", Furniture.Stone, new SpriteMap("furni/stone/stone", 24, 24), FurnitureType.Theme, bak: "leftRoomBG_oldstone"),
          new Furniture(false, false, "Set me up on my pony on my boat.", Rarity.VeryRare, "acousticG", 14, 30, "ACOUSTIC", Furniture.Instruments,  null, true),
          new Furniture(false, false, "Cello. Is it me you’re looking for?", Rarity.VeryRare, "cello", 15, 36, "CELLO", Furniture.Instruments,  null, true),
          new Furniture(false, false, "Mama let him play.", Rarity.VeryRare, "electricG", 14, 30, "STRAT", Furniture.Instruments,  null, true),
          new Furniture(true, false, "I would walk 1000 miles.", Rarity.SuperRare, "grand", 33, 34, "GRANDPIANO", Furniture.Instruments,  null, true, surface: true, topoff: 14f),
          new Furniture(true, false, "I never learned how to tune a harp.", Rarity.VeryRare, "harp", 30, 32, "HARP", Furniture.Instruments,  null, true),
          new Furniture(false, false, "Kid’s got rhythm!", Rarity.Rare, "snaredrum", 17, 20, "SNARE", Furniture.Instruments,  null, true, surface: true),
          new Furniture(false, false, "Twinkle twinkle little star.", Rarity.VeryVeryRare, "upright", 35, 23, "SCHOOLPIANO", Furniture.Instruments,  null, true, surface: true),
          new Furniture(false, false, "Like a rainbow in the dark.", Rarity.VeryRare, "electricG2", 16, 31, "METAL GUITAR", Furniture.Instruments,  null, true),
          new Furniture(false, false, "Funky.", Rarity.VeryRare, "electricG3", 14, 30, "TELE", Furniture.Instruments,  null, true),
          new Furniture(true, false, "Testing. Testing.", Rarity.VeryVeryRare, "mic stand", 19, 22, "MICROPHONE", Furniture.Instruments,  null, true),
          new Furniture(false, false, "Makes loud things even LOUDER!", Rarity.SuperRare, "amp", 18, 16, "AMP", Furniture.Instruments,  null, true, surface: true),
          new Furniture(false, false, "Get organized!", Rarity.UltraRare, "organ", 33, 36, "ORGAN", Furniture.Instruments,  null, true),
          new Furniture(true, false, "What a thoughtful gift!", Rarity.Special, "easel", 21, 30, "EASEL", Furniture.Momento,  null, true),
          new Furniture(false, false, "What a thoughtful gift.", Rarity.Special, "eggpedestal", 16, 28, "EGG", Furniture.Momento,  null, true),
          new Furniture(true, false, "What a thoughtful gift.", Rarity.Special, "giftbasket", 22, 19, "GIFT BASKET", Furniture.Momento,  null, true),
          new Furniture(true, false, "What a thoughtful gift.", Rarity.Special, "giftwine", 11, 20, "WINE", Furniture.Momento,  null, true),
          new Furniture(false, false, "What a thoughtful gift.", Rarity.Special, "jukebox", 32, 38, "JUKEBOX", Furniture.Momento,  null, true),
          new Furniture(true, true, "What a thoughtful gift...", Rarity.Special, "junk", 28, 23, "JUNK", Furniture.Momento,  null, true),
          new Furniture(true, false, "What a thoughtful gift.", Rarity.Special, "littlemanpic", 16, 16, "PHOTO", Furniture.Momento,  null, false),
          new Furniture(false, false, "What a thoughtful gift.", Rarity.Special, "plate", 16, 16, "PLATE", Furniture.Momento,  null, true),
          new Furniture(false, true, "What a thoughtful gift.", Rarity.Special, "YOYO", 39, 32, "YOYO", Furniture.Momento,  null, true),
          new Furniture(false, false, "Plain.", Rarity.Special, "biosFG", 141, 87, "BIOS ROOM", Furniture.Default, new SpriteMap("furni/default/default", 24, 24), FurnitureType.Theme, bak: "biosBG")
          {
            alwaysHave = true
          },
          new Furniture(false, false, "The old standard.", Rarity.Special,  null, 0, 0, "BIOS FONT", Furniture.Default, new SpriteMap("furni/fontIcon", 24, 24), FurnitureType.Font, new BitmapFont("furni/biosFont", 8))
          {
            alwaysHave = true
          },
          new Furniture(false, false, "Clear room!", Rarity.Special, "cantPlaceLarge", 32, 32, "CLEAR ROOM", Furniture.Default)
          {
            alwaysHave = true
          },
          new Furniture(true, false, "OH YEAH THAT'S THE STUFF!", Rarity.Common, "coffeemaker", 13, 15, "COFFEE MAKER", Furniture.Office,  null, true),
          new Furniture(false, false, "Tastes a little funny...", Rarity.SuperRare, "coffeevendingmachine", 24, 34, "COFFEE MACHINE", Furniture.Office,  null, true, surface: true),
          new Furniture(false, false, "8 kinds of water!", Rarity.VeryRare, "drinkvendingmachine", 26, 34, "DRINK MACHINE", Furniture.Office,  null, true, surface: true),
          new Furniture(true, false, "Works perfectly.", Rarity.Rare, "printer", 26, 29, "PRINTER", Furniture.Office,  null, true, surface: true),
          new Furniture(false, false, "Welcome Home!", Rarity.SuperRare, "leftRoomFG_office", 141, 87, "OFFICE ROOM", Furniture.Office, new SpriteMap("furni/office/office", 24, 24), FurnitureType.Theme, bak: "leftRoomBG_office"),
          new Furniture(true, false, "It's Wheel-y fun...", Rarity.Rare, "captainswheel", 25, 26, "CAPTAIN'S WHEEL", Furniture.Ship,  null, true),
          new Furniture(true, false, "Found on the good ship Friendship.", Rarity.SuperRare, "jollyroger", 33, 22, "JOLLY ROGER", Furniture.Ship,  null, false),
          new Furniture(true, false, "Also known as a Sneaky Peekie.", Rarity.VeryRare, "periscope", 23, 36, "PERISCOPE", Furniture.Ship,  null, false, true),
          new Furniture(true, false, "Inexplicably delicious.", Rarity.Common, "seaweed", 21, 18, "SEAWEED", Furniture.Ship,  null, false, true),
          new Furniture(false, false, "It's better down where it's wetter.", Rarity.VeryRare, "leftRoomFG_ship", 141, 87, "SHIP ROOM", Furniture.Ship, new SpriteMap("furni/ship/ship", 24, 24), FurnitureType.Theme, bak: "leftRoomBG_ship"),
          new Furniture(true, false, "Two settings, Freezing and Frozen.", Rarity.VeryRare, "airconditioner", 26, 18, "AIR CONDITIONER", Furniture.Freezer,  null, false, true),
          new Furniture(true, false, "Forgot to bring a jacket...", Rarity.Legendary, "ancestor", 24, 32, "ANCESTOR", Furniture.Freezer,  null, true),
          new Furniture(false, false, "Totally freezer burnt.", Rarity.SuperRare, "FrozenVeggie", 40, 28, "FROZEN VEGGIES", Furniture.Freezer,  null, true),
          new Furniture(false, false, "Now featuring cone.", Rarity.Common, "icecream", 12, 18, "ICECREAM", Furniture.Freezer,  null, true),
          new Furniture(false, false, "Keep refrigerated.", Rarity.VerySuperRare, "IceCreamPail", 40, 30, "ICECREAM PAIL", Furniture.Freezer,  null, true, surface: true),
          new Furniture(false, false, "Stains way too easily.", Rarity.Rare, "IceTable", 30, 16, "ICE TABLE", Furniture.Freezer,  null, true, surface: true),
          new Furniture(true, false, "Lies in wait.", Rarity.Common, "icicle", 24, 16, "ICICLE", Furniture.Freezer,  null, false, true),
          new Furniture(true, false, "Needs a hug.", Rarity.VeryVeryRare, "snowduck", 24, 21, "SNOW DUCK", Furniture.Freezer,  null, true),
          new Furniture(false, false, "Perfect for wetting butts.", Rarity.VeryRare, "snowfa", 35, 21, "SNOWFA", Furniture.Freezer,  null, true, surface: true, topoff: 8f),
          new Furniture(false, false, "A fridge for Icecream.", Rarity.SuperRare, "leftRoomFG_freezer", 141, 87, "FREEZER ROOM", Furniture.Freezer, new SpriteMap("furni/freezer/freezer", 24, 24), FurnitureType.Theme, bak: "leftRoomBG_freezer"),
          new Furniture(false, false, "Where hits are made.", Rarity.Legendary, "leftRoomFG_music", 141, 87, "MUSIC ROOM", Furniture.Instruments, new SpriteMap("furni/music/music", 24, 24), FurnitureType.Theme, bak: "leftRoomBG_music"),
          new Furniture(false, false, "Full of impossible to pronounce plants.", Rarity.SuperRare, "leftRoomFG_greenhouse", 141, 87, "GREENHOUSE ROOM", Furniture.Flowers, new SpriteMap("furni/flowers/greenhouse", 24, 24), FurnitureType.Theme, bak: "leftRoomBG_greenhouse"),
          new Furniture(false, false, "Watch out for darts...", Rarity.Rare, "leftRoomFG_bar", 141, 87, "BAR ROOM", Furniture.Bar, new SpriteMap("furni/bar/bar", 24, 24), FurnitureType.Theme, bak: "leftRoomBG_bar"),
          new Furniture(true, false, "Fly it with pride!", Rarity.Special, "flagpole", 8, 33, "FLAG", Furniture.Default,  null, true, canGacha: false)
          {
            isFlag = true,
            alwaysHave = true
          },
          new Furniture(true, false, "Fly it with pride!", Rarity.VeryRare, "naturalFlagpole", 8, 33, "NATURAL FLAG", Furniture.Outdoor,  null, true, canGacha: false)
          {
            isFlag = true
          },
          new Furniture(true, false, "Fly it with pride!", Rarity.VerySuperRare, "fancyFlagpole", 8, 33, "FANCY FLAG", Furniture.Fancy,  null, true, canGacha: false)
          {
            isFlag = true
          },
          new Furniture(true, false, "Fly it with pride!", Rarity.SuperRare, "stoneFlagpole", 8, 33, "STONE FLAG", Furniture.Stone,  null, true, canGacha: false)
          {
            isFlag = true
          },
          new Furniture(true, false, "Place in room to allow XP skipping.", Rarity.Special, "voodooVincent", 19, 21, "VOODOO VINCENT", Furniture.Default,  null, true, canGacha: false)
          {
            yOffset = -4f
          },
          new Furniture(false, false, "Stay outta my room!", Rarity.Special, "proximityDefenseTurret", 25, 16, "PERIMETER DEFENCE", Furniture.Default,  null, false, true, canGacha: false)
          {
            yOffset = 0f,
            visible = false
          }
        };
                int key = 0;
                foreach (Furniture allFurni in RoomEditor.allFurnis)
                {
                    List<Furniture> furnitureList = null;
                    if (!RoomEditor._furniGroupMap.TryGetValue(allFurni.group, out furnitureList))
                    {
                        furnitureList = new List<Furniture>();
                        RoomEditor._furniGroupMap[allFurni.group] = furnitureList;
                    }
                    furnitureList.Add(allFurni);
                    RoomEditor._furniMap[key] = allFurni;
                    allFurni.index = (short)key;
                    ++key;
                }
                RoomEditor.allFurnis.Sort((x, y) => (int)(x.group.name.CompareTo(y.group.name) * 100 + x.type));
            }
            return RoomEditor.allFurnis;
        }

        private int FurniIndexAdd(int index, int plus)
        {
            if (Profiles.experienceProfile == null)
                return 0;
            int num = index + plus;
            if (num >= Profiles.experienceProfile.GetAvailableFurnis().Count)
                return num % Profiles.experienceProfile.GetAvailableFurnis().Count;
            return num < 0 ? (Profiles.experienceProfile.GetAvailableFurnis().Count + num % Profiles.experienceProfile.GetAvailableFurnis().Count) % Profiles.experienceProfile.GetAvailableFurnis().Count : num;
        }

        public Furniture CurFurni()
        {
            if (Profiles.experienceProfile == null)
                return null;
            List<Furniture> availableFurnis = Profiles.experienceProfile.GetAvailableFurnis();
            if (_desiredFurniSelection > availableFurnis.Count)
                _desiredFurniSelection = 0;
            return availableFurnis[_desiredFurniSelection];
        }

        public override void Update()
        {
            if (_selector.screen.transitioning)
                return;
            _takenFlash = Lerp.Float(_takenFlash, 0f, 0.02f);
            if (!_open)
            {
                if (_fade >=  0.01f || !_closing)
                    return;
                _closing = false;
            }
            else
            {
                if (_mode != _desiredMode)
                    _mode = _desiredMode;
                if (_fade > 0.9f)
                {
                    int furniSelection = _furniSelection;
                    if (_slideTo != 0.0 && _slide != _slideTo)
                        _slide = Lerp.Float(_slide, _slideTo, 0.1f);
                    else if (_slideTo != 0.0 && _slide == _slideTo)
                    {
                        _slide = 0f;
                        _slideTo = 0f;
                        _furniSelection = _desiredFurniSelection;
                    }
                    if (_upSlideTo != 0.0 && _upSlide != _upSlideTo)
                        _upSlide = Lerp.Float(_upSlide, _upSlideTo, 0.1f);
                    else if (_upSlideTo != 0.0 && _upSlide == _upSlideTo)
                    {
                        _upSlide = 0f;
                        _upSlideTo = 0f;
                        _furniSelection = _desiredFurniSelection;
                    }
                    if (_desiredFurniSelection == _furniSelection && _slideTo == 0.0 && _upSlideTo == 0.0 && _mode == REMode.Main)
                    {
                        if (_selector.inputProfile.Down("MENULEFT"))
                        {
                            _desiredFurniSelection = (short)FurniIndexAdd(_desiredFurniSelection, -1);
                            _slideTo = -1f;
                            SFX.Play("consoleTick");
                        }
                        if (_selector.inputProfile.Down("MENURIGHT"))
                        {
                            _desiredFurniSelection = (short)FurniIndexAdd(_desiredFurniSelection, 1);
                            _slideTo = 1f;
                            if (_desiredFurniSelection >= 0)
                            {
                                int desiredFurniSelection = _desiredFurniSelection;
                                int count = Profiles.experienceProfile.GetAvailableFurnis().Count;
                            }
                            SFX.Play("consoleTick");
                        }
                        if (_selector.inputProfile.Down("MENUUP"))
                        {
                            _desiredFurniSelection = (short)FurniIndexAdd(_desiredFurniSelection, -5);
                            _upSlideTo = -1f;
                            if (_desiredFurniSelection >= 0)
                            {
                                int desiredFurniSelection = _desiredFurniSelection;
                                int count = Profiles.experienceProfile.GetAvailableFurnis().Count;
                            }
                            SFX.Play("consoleTick");
                        }
                        if (_selector.inputProfile.Down("MENUDOWN"))
                        {
                            _desiredFurniSelection = (short)FurniIndexAdd(_desiredFurniSelection, 5);
                            _upSlideTo = 1f;
                            if (_desiredFurniSelection >= 0)
                            {
                                int desiredFurniSelection = _desiredFurniSelection;
                                int count = Profiles.experienceProfile.GetAvailableFurnis().Count;
                            }
                            SFX.Play("consoleTick");
                        }
                        if (_selector.inputProfile.Pressed("SELECT"))
                        {
                            _placementVariation = 0;
                            SFX.Play("consoleSelect", 0.4f);
                            _desiredMode = REMode.Place;
                        }
                        if (_selector.inputProfile.Pressed("CANCEL"))
                        {
                            _desiredFurniSelection = 0;
                            _furniSelection = _desiredFurniSelection;
                            _open = false;
                            _selector.fade = 1f;
                            _fade = 0f;
                            _selector.screen.DoFlashTransition();
                            Profiles.Save(_selector.profile);
                            if (Network.isActive && _box.duck != null)
                                Send.Message(new NMRoomData(_box.duck.profile, _box.duck.profile.furniturePositionData));
                            _selector._editingRoom = false;
                            SFX.Play("consoleCancel", 0.4f);
                        }
                    }
                    else if (_mode == REMode.Place)
                    {
                        position = _box.position;
                        if (_furniCursor.x < 0.0)
                            _furniCursor = position + new Vec2(30f, 30f);
                        if (_selector.inputProfile.Down("RAGDOLL"))
                        {
                            if (_selector.inputProfile.Pressed("MENULEFT"))
                                --_furniCursor.x;
                            if (_selector.inputProfile.Pressed("MENURIGHT"))
                                ++_furniCursor.x;
                            if (_selector.inputProfile.Pressed("MENUUP"))
                                --_furniCursor.y;
                            if (_selector.inputProfile.Pressed("MENUDOWN"))
                                ++_furniCursor.y;
                        }
                        else
                        {
                            if (_selector.inputProfile.Down("MENULEFT"))
                                --_furniCursor.x;
                            if (_selector.inputProfile.Down("MENURIGHT"))
                                ++_furniCursor.x;
                            if (_selector.inputProfile.Down("MENUUP"))
                                --_furniCursor.y;
                            if (_selector.inputProfile.Down("MENUDOWN"))
                                ++_furniCursor.y;
                        }
                        Vec2 vec2_1 = new Vec2(x + 6f, y);
                        Vec2 vec2_2 = new Vec2(x + roomSize, y + 70f);
                        Furniture availableFurni = Profiles.experienceProfile.GetAvailableFurnis()[_desiredFurniSelection];
                        if (availableFurni.type == FurnitureType.Prop)
                        {
                            if (_furniCursor.x - (availableFurni.sprite.width / 2) < vec2_1.x)
                                _furniCursor.x = vec2_1.x + availableFurni.sprite.width / 2;
                            if (_furniCursor.x + (availableFurni.sprite.width / 2) > vec2_2.x)
                                _furniCursor.x = vec2_2.x - availableFurni.sprite.width / 2;
                            if (_furniCursor.y > vec2_2.y)
                                _furniCursor.y = vec2_2.y;
                            if (_furniCursor.y < vec2_1.y)
                                _furniCursor.y = vec2_1.y;
                        }
                        if (availableFurni.type == FurnitureType.Prop)
                        {
                            List<Rectangle> rectangleList = new List<Rectangle>();
                            foreach (FurniturePosition furniturePosition in _selector.profile.furniturePositions)
                            {
                                Furniture furniture = RoomEditor.GetFurniture(furniturePosition.id);
                                if (furniture != null && furniture.isSurface)
                                {
                                    Vec2 vec2_3 = new Vec2(furniturePosition.x, furniturePosition.y);
                                    if (_selector.box.rightRoom)
                                        vec2_3.x = roomSize - vec2_3.x;
                                    vec2_3 += _selector.box.position;
                                    rectangleList.Add(new Rectangle(vec2_3.x - furniture.sprite.width / 2, (float)(vec2_3.y - Math.Ceiling(furniture.sprite.height / 2.0) + 1.0) + furniture.topOffset, furniture.sprite.width, furniture.sprite.height - furniture.topOffset));
                                }
                            }
                            _furniPos.x = _furniCursor.x;
                            _furniPos.y = _furniCursor.y;
                            float num1 = (float)Math.Floor(availableFurni.sprite.height / 2.0);
                            if (availableFurni.stickToFloor)
                            {
                                float y = (float)(_furniCursor.y + num1 - 2.0);
                                Vec2 hitPos = Vec2.Zero;
                                float num2 = 999f;
                                foreach (Rectangle rect in rectangleList)
                                {
                                    if (rect.Top >= y - 2.0 && rect.Top - num1 < num2 && Collision.Line(new Vec2(_furniCursor.x, y), new Vec2(_furniCursor.x, y + 100f), rect))
                                        num2 = rect.Top - num1;
                                }
                                if (Level.CheckRay<IPlatform>(new Vec2(_furniCursor.x, y), new Vec2(_furniCursor.x, y + 100f), out hitPos) is Thing thing)
                                    _furniPos.y = thing.top - num1;
                                if (_furniPos.y > num2)
                                    _furniPos.y = num2;
                            }
                            else if (availableFurni.stickToRoof)
                            {
                                float y = (float)(_furniPos.y - num1 + 2.0);
                                Vec2 furniCursor = _furniCursor;
                                int num3 = availableFurni.sprite.height / 2;
                                Vec2 hitPos = Vec2.Zero;
                                if (Level.CheckRay<IPlatform>(new Vec2(_furniCursor.x, y), new Vec2(_furniCursor.x, y - 100f), out hitPos) is Thing thing)
                                {
                                    _furniPos.y = (float)(thing.bottom + availableFurni.sprite.height / 2 - 2.0);
                                    if (_box.rightRoom)
                                    {
                                        if (thing is Block && _furniPos.x < 226.0)
                                            _furniPos.y += 11f;
                                    }
                                    else if (thing is Block && _furniPos.x > 93.0 && _furniPos.x < 160.0)
                                        _furniPos.y += 11f;
                                }
                            }
                        }
                        _hover = null;
                        if (availableFurni.type == FurnitureType.Prop)
                        {
                            foreach (FurniturePosition furniturePosition in _selector.profile.furniturePositions)
                            {
                                Furniture furniture = RoomEditor.GetFurniture(furniturePosition.id);
                                if (furniture != null && furniture.type == FurnitureType.Prop)
                                {
                                    Vec2 vec2_4 = new Vec2(furniturePosition.x, furniturePosition.y);
                                    if (_selector.box.rightRoom)
                                        vec2_4.x = roomSize - vec2_4.x;
                                    vec2_4 += position;
                                    if ((_furniCursor - vec2_4).length < 4.0)
                                        _hover = furniturePosition;
                                }
                            }
                        }
                        if (_selector.inputProfile.Pressed("MENU2"))
                        {
                            if (_hover == null)
                            {
                                if (availableFurni.name == "PHOTO" || availableFurni.name == "EGG" || availableFurni.name == "EASEL")
                                {
                                    ++_placementVariation;
                                    if (Profiles.experienceProfile != null && _placementVariation > Profiles.experienceProfile.numLittleMen)
                                        _placementVariation = 0;
                                }
                                else if (availableFurni.sprite != null)
                                {
                                    ++_placementVariation;
                                    if (_placementVariation > availableFurni.sprite.texture.width / availableFurni.sprite.width - 1 + (availableFurni.sprite.texture.height / availableFurni.sprite.height - 1))
                                    {
                                        if (availableFurni.canFlip)
                                            _placementFlip = !_placementFlip;
                                        _placementVariation = 0;
                                    }
                                }
                            }
                            else
                            {
                                _selector.profile.furniturePositions.Remove(_hover);
                                _desiredFurniSelection = _furniSelection = (short)Profiles.experienceProfile.GetAvailableFurnis().IndexOf(RoomEditor.GetFurniture(_hover.id));
                                _placementFlip = _hover.flip;
                                _placementVariation = _hover.variation;
                                _hover = null;
                            }
                        }
                        invalidPlacement = false;
                        bool flag1 = false;
                        if (_furniPos.y < y || _furniPos.y > y + 70.0 || _furniPos.x < x || _furniPos.x > x + roomSize)
                        {
                            invalidPlacement = true;
                            flag1 = true;
                        }
                        if (_selector.inputProfile.Pressed("MENU2") && availableFurni.name == "CLEAR ROOM")
                        {
                            foreach (FurniturePosition furniturePosition in new List<FurniturePosition>(_selector.profile.furniturePositions))
                            {
                                _selector.profile.furniturePositions.Remove(furniturePosition);
                                SmallSmoke.shortlife = true;
                                for (int index = 0; index < 5; ++index)
                                {
                                    float scaleMul = Rando.Float(0.9f, 1.2f);
                                    SmallSmoke smallSmoke = SmallSmoke.New(position.x + furniturePosition.x + Rando.Float(-2f, 2f), position.y + furniturePosition.y + Rando.Float(-4f, 4f), scaleMul: scaleMul);
                                    smallSmoke.hSpeed += Rando.Float(-0.3f, 0.3f);
                                    smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                                    Level.Add(smallSmoke);
                                }
                                SmallSmoke.shortlife = false;
                            }
                        }
                        if (_selector.inputProfile.Pressed("SELECT") && availableFurni.name != "CLEAR ROOM")
                        {
                            if (availableFurni.type == FurnitureType.Prop && _hover != null)
                            {
                                _selector.profile.furniturePositions.Remove(_hover);
                            }
                            else
                            {
                                int num4 = Profiles.experienceProfile.GetNumFurnitures(availableFurni.index);
                                int furnituresPlaced = _selector.profile.GetNumFurnituresPlaced(availableFurni.index);
                                bool flag2 = false;
                                if (_selector.profile.GetTotalFurnituresPlaced() >= RoomEditor.maxFurnitures && availableFurni.type == FurnitureType.Prop)
                                {
                                    num4 = 0;
                                    flag2 = true;
                                }
                                int num5 = num4;
                                if (furnituresPlaced < num5 && (!invalidPlacement || (availableFurni.type == FurnitureType.Theme || availableFurni.type == FurnitureType.Font) && flag1))
                                {
                                    if (availableFurni.type == FurnitureType.Theme)
                                    {
                                        _selector.profile.furniturePositions.RemoveAll(sx => RoomEditor.GetFurniture(sx.id) != null && RoomEditor.GetFurniture(sx.id).type == FurnitureType.Theme);
                                        SFX.Play("consoleSelect", 0.4f);
                                        _desiredMode = REMode.Main;
                                    }
                                    else if (availableFurni.type == FurnitureType.Font)
                                    {
                                        _selector.profile.furniturePositions.RemoveAll(sx => RoomEditor.GetFurniture(sx.id) != null && RoomEditor.GetFurniture(sx.id).type == FurnitureType.Font);
                                        SFX.Play("consoleSelect", 0.4f);
                                        _desiredMode = REMode.Main;
                                    }
                                    if (availableFurni.group != Furniture.Default || availableFurni.type == FurnitureType.Prop)
                                    {
                                        FurniturePosition furniturePosition = new FurniturePosition
                                        {
                                            x = (byte)(_furniPos.x - position.x),
                                            y = (byte)(_furniPos.y - position.y),
                                            flip = _placementFlip
                                        };
                                        if (availableFurni.group == Furniture.Characters)
                                            furniturePosition.flip = _selector.box.rightRoom;
                                        if (_selector.box.rightRoom)
                                        {
                                            furniturePosition.x = (byte)((uint)RoomEditor.roomSize - furniturePosition.x);
                                            --furniturePosition.x;
                                        }
                                        furniturePosition.id = (ushort)availableFurni.index;
                                        furniturePosition.variation = (byte)_placementVariation;
                                        _selector.profile.furniturePositions.Add(furniturePosition);
                                        if (availableFurni.type == FurnitureType.Prop)
                                        {
                                            SmallSmoke.shortlife = true;
                                            for (int index = 0; index < 5; ++index)
                                            {
                                                float scaleMul = Rando.Float(0.9f, 1.2f);
                                                SmallSmoke smallSmoke = SmallSmoke.New(_furniPos.x - (availableFurni.sprite.width - 4) / 2 + index * ((availableFurni.sprite.width - 4) / 4) + Rando.Float(-2f, 2f), _furniPos.y + availableFurni.sprite.height / 4 + Rando.Float(-4f, 4f), scaleMul: scaleMul);
                                                smallSmoke.hSpeed += Rando.Float(-0.3f, 0.3f);
                                                smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                                                Level.Add(smallSmoke);
                                            }
                                            for (int index = 0; index < 5; ++index)
                                            {
                                                float scaleMul = Rando.Float(0.9f, 1.2f);
                                                SmallSmoke smallSmoke = SmallSmoke.New(_furniPos.x - (availableFurni.sprite.width - 4) / 2 + index * ((availableFurni.sprite.width - 4) / 4) + Rando.Float(-2f, 2f), _furniPos.y - availableFurni.sprite.height / 4 + Rando.Float(-4f, 4f), scaleMul: scaleMul);
                                                smallSmoke.hSpeed += Rando.Float(-0.3f, 0.3f);
                                                smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                                                Level.Add(smallSmoke);
                                            }
                                            SmallSmoke.shortlife = false;
                                        }
                                    }
                                }
                                else if (!invalidPlacement)
                                {
                                    if (flag2)
                                        _box._tooManyPulse = 1f;
                                    else
                                        _box._noMorePulse = 1f;
                                }
                            }
                        }
                        if (_selector.inputProfile.Pressed("CANCEL"))
                        {
                            _placementFlip = false;
                            SFX.Play("consoleSelect", 0.4f);
                            _desiredMode = REMode.Main;
                        }
                    }
                }
                _font.alpha = _fade;
                _font.depth = (Depth)0.96f;
                _font.scale = new Vec2(1f, 1f);
                if (_mode == REMode.Main)
                {
                    _pendingMaps.Clear();
                    Vec2 position = this.position;
                    this.position = Vec2.Zero;
                    _selector.screen.BeginDraw();
                    if (_desiredFurniSelection >= Profiles.experienceProfile.GetAvailableFurnis().Count)
                        return;
                    Furniture sel = Profiles.experienceProfile.GetAvailableFurnis()[_desiredFurniSelection];
                    string text1 = "@LWING@" + sel.name + "@RWING@";
                    BitmapFont font = sel.font;
                    if (font == null && sel.group != null)
                        font = sel.group.font;
                    if (font != null)
                    {
                        font.scale = new Vec2(1f);
                        font.characterYOffset = 1;
                        font.Draw(text1, Maths.RoundToPixel(new Vec2((float)(x + width / 2.0 - _font.GetWidth(text1) / 2.0), (float)(y + 7.0 - 2.0))), Color.White, (Depth)0.95f);
                        font.characterYOffset = 0;
                    }
                    else
                        _font.Draw(text1, Maths.RoundToPixel(new Vec2((float)(x + width / 2.0 - _font.GetWidth(text1) / 2.0), (float)(y + 8.0 - 2.0))), Color.White, (Depth)0.95f);
                    Graphics.DrawRect(new Vec2(x, y), new Vec2(x + 400f, y + 14f), Color.Black, (Depth)0.94f);
                    Graphics.DrawRect(new Vec2(x, y + 74f), new Vec2(x + 400f, y + 90f), Color.Black, (Depth)0.98f);
                    float num6 = -18f;
                    int count = Profiles.experienceProfile.GetAvailableFurnis().Count;
                    int num7 = RoomEditor._furniGroupMap[sel.group].IndexOf(sel);
                    int num8 = Profiles.experienceProfile.GetAvailableFurnis().Where<Furniture>(v => v.group == sel.group).Count<Furniture>();
                    for (int index1 = 0; index1 < 5; ++index1)
                    {
                        for (int index2 = 0; index2 < 11; ++index2)
                        {
                            int plus = index2 - 5 + (index1 - 2) * 5;
                            float x1 = (float)(x + 2.0 + index2 * 22 + -_slide * 20.0);
                            float num9 = (float)(y + 37.0 + -_upSlide * 20.0);
                            int index3 = FurniIndexAdd(_furniSelection, plus);
                            Furniture availableFurni = Profiles.experienceProfile.GetAvailableFurnis()[index3];
                            float x2 = (float)(x + (x + 2.0 + 242.0 - (x + 2f)) / 2.0 - 9.0);
                            double num10 = Maths.Clamp((float)((50.0 - Math.Abs(x1 - x2)) / 50.0), 0f, 1f);
                            DuckRig.GetHatPoint(_profile.persona.sprite.imageIndex);
                            SpriteMap g = availableFurni.sprite;
                            if (availableFurni.icon != null)
                                g = availableFurni.icon;
                            if (availableFurni.type == FurnitureType.Theme && index2 == 5 && index1 == 2)
                                g = availableFurni.sprite;
                            Vec2 zero = Vec2.Zero;
                            g.alpha = _profile.persona.sprite.alpha;
                            Vec2 pos1 = Vec2.Zero;
                            pos1 = new Vec2(x1, (float)(num9 + num6 + index1 * 20 - 14.0));
                            float num11 = 1f - Math.Min((float)(((pos1 - new Vec2(x2, (float)(y + 35.0 + 10.0))).length + 10.0) / 40.0), 1f);
                            g.scale = new Vec2(Math.Min((float)(0.5 + Math.Max(num11 - 0.5f, 0f) * 2.0), 1f));
                            pos1.x -= 44f;
                            pos1.y -= 6f;
                            g.depth = (Depth)(float)(0.85f + g.xscale * 0.1f);
                            if (availableFurni.type == FurnitureType.Theme && index2 == 5 && index1 == 2)
                            {
                                SpriteMap spriteMap = g;
                                spriteMap.scale *= 0.25f;
                            }
                            if (_fade >  0.01f)
                            {
                                if (availableFurni.group == sel.group)
                                {
                                    SpriteMap spriteMap = g;
                                    spriteMap.depth += 2000;
                                }
                                pos1 = Maths.RoundToPixel(pos1);
                                int numFurnitures = Profiles.experienceProfile.GetNumFurnitures(availableFurni.index);
                                if (_selector.profile.GetNumFurnituresPlaced(availableFurni.index) >= numFurnitures)
                                {
                                    _cantPlaceLarge.depth = g.depth + 5;
                                    _cantPlaceLarge.scale = new Vec2(0.25f);
                                    _cantPlaceLarge.alpha = 0.7f;
                                    Graphics.Draw(_cantPlaceLarge, pos1.x, pos1.y);
                                }
                                Vec2 scale = g.scale;
                                if (availableFurni.font != null && availableFurni.sprite == null)
                                {
                                    availableFurni.font.scale = new Vec2(g.xscale * 2f);
                                    availableFurni.font.Draw("F", pos1 + new Vec2(-3.5f, -3f) + (float)((g.xscale - 0.5) * 2.0) * new Vec2(-3f, -3f), Color.Black, g.depth + 10);
                                    Graphics.Draw(g, pos1.x, pos1.y);
                                }
                                else if (availableFurni.type == FurnitureType.Theme && index2 == 5 && index1 == 2)
                                {
                                    _bigScren.depth = g.depth - 10;
                                    _bigScren.scale = g.scale * 4f;
                                    _bigScren.color = g.color;
                                    Graphics.Draw(_bigScren, pos1.x - 0.5f, pos1.y);
                                    availableFurni.background.depth = g.depth - 5;
                                    availableFurni.background.scale = g.scale;
                                    availableFurni.background.color = g.color;
                                    Graphics.Draw(availableFurni.background, pos1.x, pos1.y);
                                    Graphics.Draw(g, pos1.x, pos1.y);
                                }
                                else
                                    availableFurni.Draw(pos1 + new Vec2(0f, 0f), g.depth);
                                g.scale = scale;
                                _whiteCircle.color = availableFurni.group.color;
                                if (index2 == 5 && index1 == 2)
                                {
                                    _whiteCircle.depth = (Depth)0.8f;
                                    if (availableFurni.group == sel.group)
                                        _whiteCircle.depth = new Depth(g.depth.value - 0.025f, g.depth.span);
                                    _whiteCircle.scale = new Vec2(g.xscale * 0.5f);
                                    Graphics.Draw(_whiteCircle, pos1.x, pos1.y);
                                    _whiteCircle.scale = new Vec2(g.xscale * 0.52f);
                                    _whiteCircle.color = new Color((byte)(availableFurni.group.color.r * 0.75), (byte)(availableFurni.group.color.g * 0.75), (byte)(availableFurni.group.color.b * 0.75));
                                    Sprite whiteCircle = _whiteCircle;
                                    whiteCircle.depth -= 30;
                                    Graphics.Draw(_whiteCircle, pos1.x, pos1.y);
                                    _whiteCircle.color = availableFurni.group.color;
                                    string str = availableFurni.group.name.Substring(0, 1).ToUpper() + availableFurni.group.name.Substring(1) + " Collection ";
                                    string text2;
                                    if (num8 == RoomEditor._furniGroupMap[availableFurni.group].Count)
                                    {
                                        text2 = str + "(Complete)";
                                    }
                                    else
                                    {
                                        string[] strArray = new string[6]
                                        {
                      str,
                      "(",
                      null,
                      null,
                      null,
                      null
                                        };
                                        int num12 = num7 + 1;
                                        strArray[2] = num12.ToString();
                                        strArray[3] = "/";
                                        num12 = RoomEditor._furniGroupMap[availableFurni.group].Count;
                                        strArray[4] = num12.ToString();
                                        strArray[5] = ")";
                                        text2 = string.Concat(strArray);
                                    }
                                    _fancyFont.depth = (Depth)0.99f;
                                    _fancyFont.scale = new Vec2(0.25f);
                                    if (_desiredFurniSelection == _furniSelection)
                                    {
                                        float num13 = (float)Math.Floor(_fancyFont.GetWidth(text2));
                                        float num14 = (float)Math.Floor(_fancyFont.GetWidth(text2) / 2.0);
                                        Vec2 pos2 = new Vec2(pos1.x - num14, pos1.y + 18f);
                                        _fancyFont.Draw(text2, pos2, new Color((byte)(availableFurni.group.color.r * 0.5), (byte)(availableFurni.group.color.g * 0.5), (byte)(availableFurni.group.color.b * 0.5)), (Depth)0.99f);
                                        _whiteCircle.scale = new Vec2(0.06f);
                                        _whiteCircle.depth = (Depth)0.98f;
                                        Graphics.Draw(_whiteCircle, pos2.x - 1f, pos2.y + 1f);
                                        Graphics.Draw(_whiteCircle, (float)(pos2.x + num13 + 1.0), pos2.y + 1f);
                                        _whiteCircle.scale = new Vec2(0.075f);
                                        _whiteCircle.depth = (Depth)0.94f;
                                        _whiteCircle.color = new Color((byte)(availableFurni.group.color.r * 0.75), (byte)(availableFurni.group.color.g * 0.75), (byte)(availableFurni.group.color.b * 0.75));
                                        Graphics.Draw(_whiteCircle, pos2.x - 1f, pos2.y + 1f);
                                        Graphics.Draw(_whiteCircle, (float)(pos2.x + num13 + 1.0), pos2.y + 1f);
                                        Graphics.DrawRect(pos2 + new Vec2(-1f, -1f), pos2 + new Vec2(num13 + 1f, 3f), availableFurni.group.color, (Depth)0.98f);
                                        Graphics.DrawRect(pos2 + new Vec2(-1.5f, -1.5f), pos2 + new Vec2(num13 + 1.5f, 3.5f), new Color((byte)(availableFurni.group.color.r * 0.75), (byte)(availableFurni.group.color.g * 0.75), (byte)(availableFurni.group.color.b * 0.75)), (Depth)0.94f);
                                        _whiteCircle.color = availableFurni.group.color;
                                        _whiteCircle.depth = (Depth)0.8f;
                                        _whiteCircle.scale = new Vec2(g.xscale * 0.5f);
                                    }
                                }
                                else
                                {
                                    _whiteCircle.depth = (Depth)0.7f;
                                    _whiteCircle.scale = new Vec2(g.xscale * 0.5f);
                                    Graphics.Draw(_whiteCircle, pos1.x, pos1.y);
                                }
                                if (Profiles.experienceProfile.GetAvailableFurnis()[FurniIndexAdd(index3, 5)].group == availableFurni.group)
                                {
                                    if (Profiles.experienceProfile.GetAvailableFurnis()[FurniIndexAdd(index3, 6)].group == availableFurni.group && Profiles.experienceProfile.GetAvailableFurnis()[FurniIndexAdd(index3, 1)].group == availableFurni.group)
                                        Graphics.DrawRect(pos1 + new Vec2(-8f, 0f), pos1 + new Vec2(14f, 22f), _whiteCircle.color, (Depth)0.7f);
                                    else
                                        Graphics.DrawRect(pos1 + new Vec2(-8f, 0f), pos1 + new Vec2(8f, 22f), _whiteCircle.color, (Depth)0.7f);
                                }
                                if (Profiles.experienceProfile.GetAvailableFurnis()[FurniIndexAdd(index3, -5)].group == availableFurni.group)
                                {
                                    if (Profiles.experienceProfile.GetAvailableFurnis()[FurniIndexAdd(index3, -6)].group == availableFurni.group && Profiles.experienceProfile.GetAvailableFurnis()[FurniIndexAdd(index3, -1)].group == availableFurni.group)
                                        Graphics.DrawRect(pos1 + new Vec2(-14f, -22f), pos1 + new Vec2(8f, 0f), _whiteCircle.color, (Depth)0.7f);
                                    else
                                        Graphics.DrawRect(pos1 + new Vec2(-8f, -22f), pos1 + new Vec2(8f, 0f), _whiteCircle.color, (Depth)0.7f);
                                }
                                if (Profiles.experienceProfile.GetAvailableFurnis()[FurniIndexAdd(index3, 1)].group == availableFurni.group)
                                    Graphics.DrawRect(pos1 + new Vec2(0f, -8f), pos1 + new Vec2(22f, 8f), _whiteCircle.color, (Depth)0.7f);
                                if (Profiles.experienceProfile.GetAvailableFurnis()[FurniIndexAdd(index3, -1)].group == availableFurni.group)
                                    Graphics.DrawRect(pos1 + new Vec2(-22f, 8f), pos1 + new Vec2(0f, -8f), _whiteCircle.color, (Depth)0.7f);
                            }
                            _profile.persona.sprite.color = Color.White;
                            g.color = Color.White;
                            _profile.persona.sprite.scale = new Vec2(1f, 1f);
                            g.scale = new Vec2(1f, 1f);
                        }
                    }
                    _font.Draw("@SELECT@", 4f, 79f, new Color(180, 180, 180), (Depth)0.99f, _profile.inputProfile);
                    _font.Draw("@CANCEL@", 122f, 79f, new Color(180, 180, 180), (Depth)0.99f, _profile.inputProfile);
                    this.position = position;
                    _selector.screen.EndDraw();
                }
                else
                {
                    _selector.screen.BeginDraw();
                    _selector.screen.EndDraw();
                }
            }
        }

        public void Open(Profile p)
        {
            if (_box == null && Network.InLobby())
                _box = (Level.current as TeamSelect2).GetBox(p.networkIndex);
            if (_box == null)
                return;
            //this._inputProfile = p.inputProfile;
            _profile = p; //this._starterProfile = p -> p;
            _open = true;
            _fade = 1f;
        }

        public override void Draw()
        {
            if (_fade <  0.01f)
                return;
            if (_mode == REMode.Main)
            {
                _selector.firstWord = "ADD";
                _selector.secondWord = "BACK";
            }
            else
            {
                if (_mode != REMode.Place)
                    return;
                Furniture availableFurni = Profiles.experienceProfile.GetAvailableFurnis()[_desiredFurniSelection];
                if (!(availableFurni.name != "CLEAR ROOM"))
                    return;
                Graphics.DrawRect(position, position + new Vec2(140f, 80f), Color.Black * 0.5f, (Depth)0.08f);
                if (_hover != null)
                {
                    Furniture furniture = RoomEditor.GetFurniture(_hover.id);
                    if (furniture == null)
                        return;
                    Vec2 vec2 = new Vec2(_hover.x, _hover.y);
                    if (_selector.box.rightRoom)
                        vec2.x = roomSize - vec2.x;
                    Vec2 pos = vec2 + position;
                    Vec2 p1_1 = new Vec2(pos.x - furniture.sprite.width / 2, pos.y - furniture.sprite.height / 2) + new Vec2(-2f, -2f);
                    Vec2 p1_2 = new Vec2(pos.x + furniture.sprite.width / 2, pos.y + furniture.sprite.height / 2) + new Vec2(2f, 2f);
                    Graphics.DrawLine(p1_1, p1_1 + new Vec2(2f, 0f), Color.White, depth: ((Depth)1f));
                    Graphics.DrawLine(p1_1, p1_1 + new Vec2(0f, 2f), Color.White, depth: ((Depth)1f));
                    Graphics.DrawLine(p1_2, p1_2 - new Vec2(2f, 0f), Color.White, depth: ((Depth)1f));
                    Graphics.DrawLine(p1_2, p1_2 - new Vec2(0f, 2f), Color.White, depth: ((Depth)1f));
                    Graphics.DrawLine(new Vec2(p1_1.x, p1_2.y), new Vec2(p1_1.x, p1_2.y) + new Vec2(2f, 0f), Color.White, depth: ((Depth)1f));
                    Graphics.DrawLine(new Vec2(p1_1.x, p1_2.y), new Vec2(p1_1.x, p1_2.y) - new Vec2(0f, 2f), Color.White, depth: ((Depth)1f));
                    Graphics.DrawLine(new Vec2(p1_2.x, p1_1.y), new Vec2(p1_2.x, p1_1.y) - new Vec2(2f, 0f), Color.White, depth: ((Depth)1f));
                    Graphics.DrawLine(new Vec2(p1_2.x, p1_1.y), new Vec2(p1_2.x, p1_1.y) + new Vec2(0f, 2f), Color.White, depth: ((Depth)1f));
                    if (furniture.sprite == null)
                        return;
                    furniture.sprite.flipH = _hover.flip;
                    if (_selector.box.rightRoom)
                        furniture.sprite.flipH = !furniture.sprite.flipH;
                    furniture.Draw(pos, (Depth)0.09f, _hover.variation);
                }
                else
                {
                    if (availableFurni.type != FurnitureType.Prop)
                        return;
                    int num = Profiles.experienceProfile.GetNumFurnitures(availableFurni.index);
                    if (_selector.profile.GetTotalFurnituresPlaced() >= RoomEditor.maxFurnitures)
                        num = 0;
                    int furnituresPlaced = _selector.profile.GetNumFurnituresPlaced(availableFurni.index);
                    availableFurni.sprite.depth = (Depth)0.09f;
                    Vec2 furniPos = _furniPos;
                    availableFurni.sprite.frame = _placementVariation;
                    availableFurni.sprite.flipH = _placementFlip;
                    if (_selector.box.rightRoom)
                        availableFurni.sprite.flipH = !availableFurni.sprite.flipH;
                    availableFurni.sprite.alpha = 1f;
                    if (furnituresPlaced >= num || invalidPlacement)
                        Graphics.material = grayscale;
                    availableFurni.Draw(furniPos, availableFurni.sprite.depth, _placementVariation);
                    Graphics.material = null;
                    _furnitureCursor.depth = (Depth)0.1f;
                    _cantPlace.depth = (Depth)0.1f;
                    if (furnituresPlaced >= num || invalidPlacement)
                        Graphics.Draw(_cantPlace, _furniCursor.x, _furniCursor.y);
                    else
                        Graphics.Draw(_furnitureCursor, _furniCursor.x, _furniCursor.y);
                    availableFurni.sprite.frame = 0;
                    availableFurni.sprite.flipH = false;
                    availableFurni.sprite.alpha = 1f;
                }
            }
        }
    }
}
