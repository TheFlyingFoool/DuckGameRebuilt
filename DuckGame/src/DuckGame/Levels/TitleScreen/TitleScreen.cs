// Decompiled with JetBrains decompiler
// Type: DuckGame.TitleScreen
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class TitleScreen : Level
    {
        public List<StarParticle> particles = new List<StarParticle>();
        private int wait;
        private float dim = 0.8f;
        private float moveSpeed = 1f;
        private float moveWait = 1f;
        private float flash;
        private bool _returnFromArcade;
        private Profile _arcadeProfile;
        private InputProfile _arcadeInputProfile;
        private TitleMenuSelection _selection = TitleMenuSelection.Play;
        private TitleMenuSelection _desiredSelection = TitleMenuSelection.Play;
        private BigTitle _title;
        private BitmapFont _font;
        private Sprite _background;
        //private Sprite _optionsPlatform;
        private Sprite _rightPlatform;
        private Sprite _leftPlatform;
        private Sprite _beamPlatform;
        private Sprite _upperMonitor;
        private Sprite _optionsTV;
        private Sprite _libraryBookcase;
        private Sprite _editorBench;
        private Sprite _editorBenchPaint;
        private Sprite _bigUButton;
        private Sprite _airlock;
        private SpriteMap _controls;
        private Sprite _starField;
        public int roundsPerSet = 8;
        public int setsPerGame = 3;
        private SpaceBackgroundMenu _space;
        private bool _fadeIn;
        private bool _fadeInFull;
        private float _pressStartBlink;
        private string _selectionText = "";
        private string _selectionTextDesired = "MULTIPLAYER";
        private float _selectionFade = 1f;
        private int _controlsFrame;
        private int _controlsFrameDesired = 1;
        private float _controlsFade = 1f;
        private OptionsBeam _optionsBeam;
        private LibraryBeam _libraryBeam;
        private MultiBeam _multiBeam;
        private EditorBeam _editorBeam;
        private Duck _duck;
        private bool _fastMultiplayer;
        private bool _enterMultiplayer;
        private UIComponent _optionsGroup;
        private MenuBoolean _quit = new MenuBoolean();
        private MenuBoolean _dontQuit = new MenuBoolean();
        private UIMenu _quitMenu;
        private UIMenu _optionsMenu;
        private UIMenu _controlConfigMenu;
        private UIMenu _graphicsMenu;
        private UIMenu _dgrMenu;
        private UIMenu _audioMenu;
        private UIMenu _flagMenu;
        private UIMenu _parentalControlsMenu;
        private UIMenu _duckGameUpdateMenu;
        private UIMenu _modsDisabledMenu;
        private UIMenu _steamWarningMessage;
        private UIComponent _pauseGroup;
        private UIMenu _mainPauseMenu;
        private MenuBoolean _enterCreditsMenuBool = new MenuBoolean();
        private UIMenu _betaMenu;
        private UIMenu _cloudConfigMenu;
        private UIMenu _cloudDeleteConfirmMenu;
        private UIMenu _accessibilityMenu;
        private UIMenu _ttsMenu;
        private UIMenu _blockMenu;
        private UIMenu _modConfigMenu;
        private UICloudManagement _cloudManagerMenu;
        private bool _enterEditor;
        private bool _enterCredits;
        private bool _enterArcade;
        private static bool _hasMenusOpen = false;
        public static bool modsChanged = false;
        private static bool firstStart = true;
        public List<List<string>> creditsRoll = new List<List<string>>();
        //private bool showPendingDeletionDialogue;
        //private bool showSizeNotificationDialogue;
        private bool _fadeBackground;
        private bool _enterLibrary;
        //private bool _enterBuyScreen;
        private float extraFade = 1f;
        private bool _startedMusic;
        private float starWait;
        private float switchWait = 1f;
        private float creditsScroll;
        private bool shownPrompt;
        private bool shownPromptF;
        private bool startStars = true;
        private int cpick;
        private bool quittingCredits;
        private bool showedNewVersionStartup;
        private bool showedModsDisabled;
        // private int time;
        //  private static bool _showedSteamFailMessage = false;

        public TitleScreen()
          : this(false, null)
        {
        }
        public static void SpargLogic()
        {
            if (Input.Pressed(Triggers.Start) || Input.Pressed(Triggers.Select)) current = new TitleScreen();
        }
        public TitleScreen(bool returnFromArcade, Profile arcadeProfile)
        {
            _centeredView = true;
            _returnFromArcade = returnFromArcade;
            _arcadeProfile = arcadeProfile;
            if (arcadeProfile == null)
                return;
            _arcadeInputProfile = arcadeProfile.inputProfile;
        }

        public bool menuOpen => Options.menuOpen || _enterMultiplayer;
        public bool enterMultiplayer
        {
            get { return _enterMultiplayer; }
            set { _enterMultiplayer = value; }
        }

        private void CloudDelete()
        {
            Cloud.DeleteAllCloudData(false);
            DuckFile.DeleteAllSaveData();
        }

        public static bool hasMenusOpen => _hasMenusOpen;

        private void AddCreditLine(params string[] line)
        {
            creditsRoll.Add(new List<string>(line));
        }

        public override void Initialize()
        {
            Vote.ClearVotes();
            Program.gameLoadedSuccessfully = true;
            Global.Save();
            HUD.ClearPlayerChangeDisplays();
            AddCreditLine("DUCK GAME");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@COMMUNITY HERO@RWINGGRAY@");
            AddCreditLine("John \"BroDuck\" Pichardo");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@LEAD TESTERS@RWINGGRAY@");
            AddCreditLine("Jacob Paul");
            AddCreditLine("Tyler Molz");
            AddCreditLine("Andrew Morrish");
            AddCreditLine("Dayton McKay");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@MIGHTY HELPFUL FRIEND@RWINGGRAY@");
            AddCreditLine("|DGGREEN|YupDanielThatsMe");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@HEROS OF MOD@RWINGGRAY@");
            AddCreditLine("Dord");
            AddCreditLine("YupDanielThatsMe");
            AddCreditLine("YoloCrayolo3");
            AddCreditLine("Zloty_Diament");
            AddCreditLine("eim64");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@TESTERS@RWINGGRAY@");
            AddCreditLine("John Pichardo", "Lotad");
            AddCreditLine("Tufukins", "Sleepy Jirachi");
            AddCreditLine("Paul Hartling", "thebluecosmonaut");
            AddCreditLine("Dan Gaechter", "James Nieves");
            AddCreditLine("Dr. Docter", "ShyNieke");
            AddCreditLine("Spectaqual", "RealGnomeTasty");
            AddCreditLine("Karim Aifi", "Zaahck");
            AddCreditLine("dino rex (guy)", "Peter Smith");
            AddCreditLine("Colin Jacobson", "mage legend");
            AddCreditLine("YvngXero", "Trevor Etzold");
            AddCreditLine("Fluury", "Phantom329");
            AddCreditLine("Kevin Duffy", "Michael Niemann");
            AddCreditLine("Zloty_Diament", "Ben");
            AddCreditLine("Bolus", "Unluck");
            AddCreditLine("Temppuuh", "Rasenshriken");
            AddCreditLine("Andresian", "Spencer Portwood");
            AddCreditLine("James \"Sunder\" Beliakoff");
            AddCreditLine("David Sabosky (SidDaSloth)");
            AddCreditLine("Jordan \"Renim\" Gauge");
            AddCreditLine("Tommaso \"Giampiero\" Bresciani");
            AddCreditLine("Nicodemo \"Nikkodemus\" Bresciani");
            AddCreditLine("Valentin Zeyfang (RedMser)");
            AddCreditLine("Luke Bromley (mrred55)");
            AddCreditLine("Christopher Alan Bell");
            AddCreditLine("Koteeevvv");
            AddCreditLine("Soh", "NiK0");
            AddCreditLine("kalamari");
            AddCreditLine("Mike Timofeev");
            AddCreditLine("JYAD (Just Your Average Duck)");
            AddCreditLine(" Argo The Rat");
            AddCreditLine("Adam Urbina");
            AddCreditLine("Leonardo \"Baffo\" Magnani");
            AddCreditLine("The Burger Always Wins");
            AddCreditLine("RaV3_past");
            AddCreditLine("Collin", "|DGPURPLE|Drake");
            AddCreditLine("Tater");
            AddCreditLine("");
            AddCreditLine("Jaydex72");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@EDITOR SEARCH IDEA@RWINGGRAY@");
            AddCreditLine("Zloty_Diament");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@A DUCK GAME COSPLAYER@RWINGGRAY@");
            AddCreditLine("Colin Lamb");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@SLEEPY AND FRIENDS@RWINGGRAY@");
            AddCreditLine("Lotad");
            AddCreditLine("Sleepy Jirachi");
            AddCreditLine("Silverlace");
            AddCreditLine("Slimy");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@FEATHERS WILL FLY CREW@RWINGGRAY@");
            AddCreditLine("Dan \"lucidinertia\" Myszak");
            AddCreditLine("Yannick \"Becer\" Marcotte-Gourde");
            AddCreditLine("Aleksander \"Acrimonious Defect\" K.D.");
            AddCreditLine("Tater", "KlockworkCanary");
            AddCreditLine("Conre", "Xatmamune");
            AddCreditLine("White Ink", "CaptainCrack");
            AddCreditLine("laduck", "This Guy");
            AddCreditLine("Repiteo", "VirtualFishbowl");
            AddCreditLine("Slinky", "JaYlab212");
            AddCreditLine("", "");
            AddCreditLine("The Entire FWF Community!");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@RUSS MONEY@RWINGGRAY@");
            AddCreditLine("AS HIMSELF");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("DEVELOPMENT TEAM");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@ART, PROGRAMMING, MUSIC@RWINGGRAY@");
            AddCreditLine("Landon Podbielski");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@DGR TEAM@RWINGGRAY@");
            AddCreditLine("NiK0");
            AddCreditLine("Dan");
            AddCreditLine("Collin");
            AddCreditLine("|RED|Fire|WHITE|break|CREDITSGRAY|");
            AddCreditLine("|BLACK|Erik|GRAY|7302");
            AddCreditLine("|DGBLUE|othello|PURPLE|7");
            AddCreditLine("|GREEN|klof44|CREDITSGRAY|");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@GITHUB CONTRIBUTOR@RWINGGRAY@");
            AddCreditLine("Lutalli");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@ROOM FURNITURE@RWINGGRAY@");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@HOME UPDATE HAT ART@RWINGGRAY@");
            AddCreditLine("Dayton McKay");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@MOD SUPPORT PROGRAMMER@RWINGGRAY@");
            AddCreditLine("Paril");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@WAFFLES, CLAMS, HIGHFIVES HATS@RWINGGRAY@");
            AddCreditLine("Lindsey Layne King");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@EGGPAL, BRAD HATS@RWINGGRAY@");
            AddCreditLine("mushbuh");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@KERCHIEFS, POSTALS, WAHHS HATS@RWINGGRAY@");
            AddCreditLine("Case Marsteller");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@B52s, UUFOS HATS@RWINGGRAY@");
            AddCreditLine("William Baldwin");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@WOLVES HAT, MILKSHAKE@RWINGGRAY@");
            AddCreditLine("Dord");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("NINTENDO SWITCH PORT");
            AddCreditLine("&");
            AddCreditLine("DEFINITIVE EDITION");
            AddCreditLine("Armature Studio");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@ENGINEERING@RWINGGRAY@");
            AddCreditLine("John Allensworth");
            AddCreditLine("Tom Ivey");
            AddCreditLine("Bryan Wagstaff");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@DIRECTOR OF PRODUCTION@RWINGGRAY@");
            AddCreditLine("Mark Nau");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@PRODUCTION@RWINGGRAY@");
            AddCreditLine("Tom Ivey");
            AddCreditLine("Mike Pirrone");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@QUALITY ASSURANCE@RWINGGRAY@");
            AddCreditLine("Gwen Dalmacio");
            AddCreditLine("Mike Pirrone");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@ARMATURE OPERATIONS@RWINGGRAY@");
            AddCreditLine("Nadine Rossignol");
            AddCreditLine("Nicole Casarona");
            AddCreditLine("Michael Thai");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@BUSINESS DEVELOPMENT@RWINGGRAY@");
            AddCreditLine("Jonathan Zamkoff");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@STUDIO EXECUTIVES@RWINGGRAY@");
            AddCreditLine("Greg John");
            AddCreditLine("Todd Keller");
            AddCreditLine("Mark Pacini");
            AddCreditLine("Jonathan Zamkoff");
            AddCreditLine("");
            AddCreditLine("|CREDITSGRAY|@LWINGGRAY@SPECIAL THANKS@RWINGGRAY@");
            AddCreditLine("Vitor Menezes");
            AddCreditLine("Wayne Sikes");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("ADULT SWIM GAMES");
            AddCreditLine("");
            AddCreditLine("Liz Pate");
            AddCreditLine("Jacob Paul");
            AddCreditLine("Brian Marquez");
            AddCreditLine("Daniel Fuller");
            AddCreditLine("Peter Gollan");
            AddCreditLine("Mariam Naziripour");
            AddCreditLine("Dan Nichols");
            AddCreditLine("Adam Baptiste");
            AddCreditLine("Case Marsteller");
            AddCreditLine("");
            AddCreditLine("Chris Johnston");
            AddCreditLine("Steve Gee");
            AddCreditLine("Charles Park");
            AddCreditLine("Kyle Young");
            AddCreditLine("Duke Nguyen");
            AddCreditLine("Andre Curtis");
            AddCreditLine("Briana Chichester");
            AddCreditLine("William Baldwin");
            AddCreditLine("Taylor Anderson-Barkley");
            AddCreditLine("Josh Terry");
            AddCreditLine("Maddie Beasley");
            AddCreditLine("Justin Morris");
            AddCreditLine("Joseph DuBois");
            AddCreditLine("Lindsey Wade");
            AddCreditLine("Adam Hatch");
            AddCreditLine("Kristy Sottilaro");
            AddCreditLine("");
            AddCreditLine("Jeff Olsen");
            AddCreditLine("Tucker Dean");
            AddCreditLine("Elizabeth Murphy");
            AddCreditLine("David Verble");
            AddCreditLine("Sean Baptiste");
            AddCreditLine("Jacqui Collins");
            AddCreditLine("Zo Douglas");
            AddCreditLine("Megan Fausti");
            AddCreditLine("Abigail Tyson");
            AddCreditLine("Ryan Murray");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("Thank you OUYA for publishing");
            AddCreditLine("the original version of Duck Game.");
            AddCreditLine("Especially Bob Mills, who");
            AddCreditLine("made it all happen.");
            AddCreditLine("");
            AddCreditLine("We need to go camping again.");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("Thank you ADULT SWIM GAMES");
            AddCreditLine("for publishing Duck Game, and");
            AddCreditLine("for doing so much promotion and");
            AddCreditLine("testing.");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("Thank you Paril for");
            AddCreditLine("writing the mod support for Duck Game.");
            AddCreditLine("Mods wouldn't have been possible");
            AddCreditLine("without you.");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("BroDuck you've been a huge help");
            AddCreditLine("keeping the community running,");
            AddCreditLine("I don't know what would have happened");
            AddCreditLine("without your help.");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("DORD your weapons mod is absolutely");
            AddCreditLine("amazing and beautiful.");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("Thank you everyone for playing");
            AddCreditLine("Duck Game, for all your support,");
            AddCreditLine("and for being so kind.");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("");
            AddCreditLine("The End");
            for (int index = 0; index < 300; ++index)
                AddCreditLine("");
            AddCreditLine("Cya later!");
            //if (!DG.InitializeDRM())
            //{
            //    Level.current = new BetaScreen();
            //}
            //else everything below till the end of the method
            _starField = new Sprite("background/starField");
            TeamSelect2.DefaultSettings();
            if (Network.isActive)
                Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Returned to title screen."));
            if (Music.currentSong != "Title" && Music.currentSong != "TitleDemo" || Music.finished)
                Music.Play("Title");
            if (GameMode.playedGame)
                GameMode.playedGame = false;
            _optionsGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
            _optionsMenu = new UIMenu("@WRENCH@OPTIONS@SCREWDRIVER@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, conString: "@CANCEL@BACK @SELECT@SELECT");
            _controlConfigMenu = new UIControlConfig(_optionsMenu, "@WRENCH@DEVICE DEFAULTS@SCREWDRIVER@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 194f, conString: "@WASD@@SELECT@ADJUST @CANCEL@BACK");
            _flagMenu = new UIFlagSelection(_optionsMenu, "FLAG", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f);
            _optionsMenu.Add(new UIMenuItemSlider("SFX Volume", field: new FieldBinding(Options.Data, "sfxVolume"), step: 0.06666667f), true);
            _optionsMenu.Add(new UIMenuItemSlider("Music Volume", field: new FieldBinding(Options.Data, "musicVolume"), step: 0.06666667f), true);

            _graphicsMenu = Options.CreateGraphicsMenu(_optionsMenu);
            _dgrMenu = Options.CreateDGRMenu(_optionsMenu);
            _audioMenu = Options.CreateAudioMenu(_optionsMenu);
            _accessibilityMenu = Options.CreateAccessibilityMenu(_optionsMenu);
            _ttsMenu = Options.tempTTSMenu;
            _blockMenu = Options.tempBlockMenu;
            _optionsMenu.Add(new UIMenuItemSlider("Rumble Intensity", field: new FieldBinding(Options.Data, "rumbleIntensity"), step: 0.06666667f), true);
            _optionsMenu.Add(new UIText(" ", Color.White), true);
            _optionsMenu.Add(new UIMenuItemToggle("SHENANIGANS", field: new FieldBinding(Options.Data, "shennanigans")), true);
            _optionsMenu.Add(new UIText(" ", Color.White), true);
            _optionsMenu.Add(new UIMenuItem("REBUILT", new UIMenuActionOpenMenu(_optionsMenu, _dgrMenu), backButton: true), true);
            _optionsMenu.Add(new UIMenuItem("EDIT CONTROLS", new UIMenuActionOpenMenuCallFunction(_optionsMenu, _controlConfigMenu, new UIMenuActionOpenMenuCallFunction.Function(UIControlConfig.ResetWarning)), backButton: true), true);
            _optionsMenu.Add(new UIMenuItem("GRAPHICS", new UIMenuActionOpenMenu(_optionsMenu, _graphicsMenu), backButton: true), true);
            _optionsMenu.Add(new UIMenuItem("AUDIO", new UIMenuActionOpenMenu(_optionsMenu, _audioMenu), backButton: true), true);
            _cloudConfigMenu = new UIMenu("@WRENCH@SAVE DATA@SCREWDRIVER@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 280f, conString: "@CANCEL@BACK @SELECT@SELECT");
            _cloudDeleteConfirmMenu = new UIMenu("CLEAR SAVE DATA?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 280f, conString: "@SELECT@SELECT");
            _cloudManagerMenu = new UICloudManagement(_cloudConfigMenu);
            _cloudConfigMenu.Add(new UIMenuItemToggle("Enable Steam Cloud", field: new FieldBinding(Options.Data, "cloud")), true);
            _cloudConfigMenu.Add(new UIMenuItem("Manage Save Data", new UIMenuActionOpenMenu(_cloudConfigMenu, _cloudManagerMenu)), true);
            _cloudConfigMenu.Add(new UIMenuItem("|DGRED|CLEAR ALL SAVE DATA", new UIMenuActionOpenMenu(_cloudConfigMenu, _cloudDeleteConfirmMenu), backButton: true), true);
            _cloudDeleteConfirmMenu.Add(new UIText("This will DELETE all data", Colors.DGRed), true);
            _cloudDeleteConfirmMenu.Add(new UIText("(Profiles, Options, Levels)", Colors.DGRed), true);
            _cloudDeleteConfirmMenu.Add(new UIText("from your Duck Game save!", Colors.DGRed), true);
            _cloudDeleteConfirmMenu.Add(new UIText("", Colors.DGRed), true);
            _cloudDeleteConfirmMenu.Add(new UIText("Do not do this, unless you're", Colors.DGRed), true);
            _cloudDeleteConfirmMenu.Add(new UIText("absolutely sure!", Colors.DGRed), true);
            _cloudDeleteConfirmMenu.Add(new UIText(" ", Colors.DGRed), true);
            _cloudDeleteConfirmMenu.Add(new UIMenuItem("|DGRED|DELETE AND RESTART.", new UIMenuActionOpenMenuCallFunction(_cloudDeleteConfirmMenu, _cloudConfigMenu, new UIMenuActionOpenMenuCallFunction.Function(CloudDelete))), true);
            _cloudDeleteConfirmMenu.Add(new UIMenuItem("|DGGREEN|CANCEL!", new UIMenuActionOpenMenu(_cloudDeleteConfirmMenu, _cloudConfigMenu)), true);
            _cloudDeleteConfirmMenu._defaultSelection = 1;
            _cloudDeleteConfirmMenu.SetBackFunction(new UIMenuActionOpenMenu(_cloudDeleteConfirmMenu, _cloudConfigMenu));
            _cloudDeleteConfirmMenu.Close();
            _cloudConfigMenu.Add(new UIText(" ", Colors.DGBlue), true);
            _cloudConfigMenu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(_cloudConfigMenu, _optionsMenu), backButton: true), true);
            _optionsMenu.Add(new UIText(" ", Color.White), true);
            if (MonoMain.moddingEnabled)
            {
                _modConfigMenu = new UIModManagement(_optionsMenu, "@WRENCH@MANAGE MODS@SCREWDRIVER@", Layer.HUD.camera.width, Layer.HUD.camera.height, 550f, conString: "@WASD@@SELECT@ADJUST @MENU1@TOGGLE @CANCEL@BACK");
                _optionsMenu.Add(new UIMenuItem("MANAGE MODS", new UIMenuActionOpenMenu(_optionsMenu, _modConfigMenu), backButton: true), true);
            }
            _optionsMenu.Add(new UIMenuItem("SELECT FLAG", new UIMenuActionOpenMenu(_optionsMenu, _flagMenu), backButton: true), true);
            _optionsMenu.Add(new UIText(" ", Color.White), true);
            if (_accessibilityMenu != null)
                _optionsMenu.Add(new UIMenuItem("USABILITY", new UIMenuActionOpenMenu(_optionsMenu, _accessibilityMenu), backButton: true), true);
            _optionsMenu.SetBackFunction(new UIMenuActionCloseMenuCallFunction(_optionsMenu, new UIMenuActionCloseMenuCallFunction.Function(OptionsSaveAndClose)));
            _optionsMenu.Close();
            _optionsGroup.Add(_optionsMenu, false);
            _controlConfigMenu.Close();
            _flagMenu.Close();
            if (MonoMain.moddingEnabled)
                _modConfigMenu.Close();
            _cloudConfigMenu.Close();
            _cloudManagerMenu.Close();
            _optionsGroup.Add(_controlConfigMenu, false);
            _optionsGroup.Add((_controlConfigMenu as UIControlConfig)._confirmMenu, false);
            _optionsGroup.Add((_controlConfigMenu as UIControlConfig)._warningMenu, false);
            _optionsGroup.Add(_flagMenu, false);
            _optionsGroup.Add(_graphicsMenu, false);
            _optionsGroup.Add(_dgrMenu, false);
            _optionsGroup.Add(Options._DGRGraphicsMenu, false);
            _optionsGroup.Add(Options._DGRHudMenu, false);
            _optionsGroup.Add(Options._DGRMiscMenu, false);
            _optionsGroup.Add(Options._DGROptimMenu, false);
            _optionsGroup.Add(_audioMenu, false);
            if (_accessibilityMenu != null)
                _optionsGroup.Add(_accessibilityMenu, false);
            if (_ttsMenu != null)
                _optionsGroup.Add(_ttsMenu, false);
            if (_blockMenu != null)
                _optionsGroup.Add(_blockMenu, false);
            if (MonoMain.moddingEnabled)
            {
                _optionsGroup.Add(_modConfigMenu, false);
                _optionsGroup.Add((_modConfigMenu as UIModManagement)._modSettingsMenu, false);
                _optionsGroup.Add((_modConfigMenu as UIModManagement)._editModMenu, false);
                _optionsGroup.Add((_modConfigMenu as UIModManagement)._yesNoMenu, false);
            }
            _optionsGroup.Add(_cloudManagerMenu, false);
            _optionsGroup.Add(_cloudManagerMenu._deleteMenu, false);
            _optionsGroup.Add(_cloudConfigMenu, false);
            _optionsGroup.Add(_cloudDeleteConfirmMenu, false);
            _optionsGroup.Close();
            Add(_optionsGroup);
            //_betaMenu = new UIMenu("@WRENCH@WELCOME TO BETA!@SCREWDRIVER@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 240f, conString: "@CANCEL@OK!");
            //_betaMenu.Add(new UIImage(new Sprite("message"), UIAlign.Center, 0.25f, 51f), true);
            //_betaMenu.Close();
            //_betaMenu._backButton = new UIMenuItem("BACK", new UIMenuActionCloseMenu(_betaMenu), backButton: true);
            //_betaMenu._isMenu = true;
            //Level.Add(_betaMenu);
            _pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f)
            {
                isPauseMenu = true
            };
            _mainPauseMenu = new UIMenu("@LWING@DUCK GAME@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@CLOSE @SELECT@SELECT");
            _quitMenu = new UIMenu("REALLY QUIT?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK @SELECT@SELECT");
            _quitMenu.Add(new UIMenuItem("NO!", new UIMenuActionOpenMenu(_quitMenu, _mainPauseMenu)), true);
            _quitMenu.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuSetBoolean(_pauseGroup, _quit)), true);
            _quitMenu.Close();
            _pauseGroup.Add(_quitMenu, false);
            _parentalControlsMenu = new UIMenu("PARENTAL CONTROLS", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, conString: "@CANCEL@CLOSE @SELECT@SELECT");
            _parentalControlsMenu.Add(new UIText("Certain online features have been", Color.White), true);
            _parentalControlsMenu.Add(new UIText("disabled by Parental Controls.", Color.White), true);
            _parentalControlsMenu.Add(new UIText("", Color.White), true);
            _parentalControlsMenu.Add(new UIMenuItem("OK", new UIMenuActionCloseMenu(_pauseGroup)), true);
            _parentalControlsMenu.Close();
            _pauseGroup.Add(_parentalControlsMenu, false);
            int pMinLength = 50;
            float num1 = 3f;
            _duckGameUpdateMenu = new UIMenu("DUCK GAME 1.5!", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 220f, conString: "@SELECT@OK!");
            UIMenu duckGameUpdateMenu1 = _duckGameUpdateMenu;
            UIText component1 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            duckGameUpdateMenu1.Add(component1, true);
            UIMenu duckGameUpdateMenu2 = _duckGameUpdateMenu;
            UIText component2 = new UIText("Duck Game has received a major update!", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            duckGameUpdateMenu2.Add(component2, true);
            UIMenu duckGameUpdateMenu3 = _duckGameUpdateMenu;
            UIText component3 = new UIText("Some of the biggest changes include:", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            duckGameUpdateMenu3.Add(component3, true);
            UIMenu duckGameUpdateMenu4 = _duckGameUpdateMenu;
            UIText component4 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            duckGameUpdateMenu4.Add(component4, true);
            UIMenu duckGameUpdateMenu5 = _duckGameUpdateMenu;
            UIText component5 = new UIText("-Support for up to 8 players and 4 spectators".Padded(pMinLength), Color.White, heightAdd: (-num1))
            {
                scale = new Vec2(0.5f)
            };
            duckGameUpdateMenu5.Add(component5, true);
            UIMenu duckGameUpdateMenu6 = _duckGameUpdateMenu;
            UIText component6 = new UIText("-New hats, weapons, equipment and furniture".Padded(pMinLength), Color.White, heightAdd: (-num1))
            {
                scale = new Vec2(0.5f)
            };
            duckGameUpdateMenu6.Add(component6, true);
            UIMenu duckGameUpdateMenu7 = _duckGameUpdateMenu;
            UIText component7 = new UIText("-New city themed levels".Padded(pMinLength), Color.White, heightAdd: (-num1))
            {
                scale = new Vec2(0.5f)
            };
            duckGameUpdateMenu7.Add(component7, true);
            UIMenu duckGameUpdateMenu8 = _duckGameUpdateMenu;
            UIText component8 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            duckGameUpdateMenu8.Add(component8, true);
            UIMenu duckGameUpdateMenu9 = _duckGameUpdateMenu;
            UIText component9 = new UIText("-Custom font support for chat".Padded(pMinLength), Color.White, heightAdd: (-num1))
            {
                scale = new Vec2(0.5f)
            };
            duckGameUpdateMenu9.Add(component9, true);
            UIMenu duckGameUpdateMenu10 = _duckGameUpdateMenu;
            UIText component10 = new UIText("-4K and custom resolution support".Padded(pMinLength), Color.White, heightAdd: (-num1))
            {
                scale = new Vec2(0.5f)
            };
            duckGameUpdateMenu10.Add(component10, true);
            UIMenu duckGameUpdateMenu11 = _duckGameUpdateMenu;
            UIText component11 = new UIText("-Host Migration, Invite Links, LAN play".Padded(pMinLength), Color.White, heightAdd: (-num1))
            {
                scale = new Vec2(0.5f)
            };
            duckGameUpdateMenu11.Add(component11, true);
            UIMenu duckGameUpdateMenu12 = _duckGameUpdateMenu;
            UIText component12 = new UIText("-Major online synchronization improvements".Padded(pMinLength), Color.White, heightAdd: (-num1))
            {
                scale = new Vec2(0.5f)
            };
            duckGameUpdateMenu12.Add(component12, true);
            UIMenu duckGameUpdateMenu13 = _duckGameUpdateMenu;
            UIText component13 = new UIText("-Major performance improvements".Padded(pMinLength), Color.White, heightAdd: (-num1))
            {
                scale = new Vec2(0.5f)
            };
            duckGameUpdateMenu13.Add(component13, true);
            UIMenu duckGameUpdateMenu14 = _duckGameUpdateMenu;
            UIText component14 = new UIText("-Hundreds and hundreds of bug fixes".Padded(pMinLength), Color.White, heightAdd: (-num1))
            {
                scale = new Vec2(0.5f)
            };
            duckGameUpdateMenu14.Add(component14, true);
            UIMenu duckGameUpdateMenu15 = _duckGameUpdateMenu;
            UIText component15 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            duckGameUpdateMenu15.Add(component15, true);
            UIMenu duckGameUpdateMenu16 = _duckGameUpdateMenu;
            UIText component16 = new UIText("Thank you for all your support!", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            duckGameUpdateMenu16.Add(component16, true);
            UIMenu duckGameUpdateMenu17 = _duckGameUpdateMenu;
            UIText component17 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            duckGameUpdateMenu17.Add(component17, true);
            _duckGameUpdateMenu.SetAcceptFunction(new UIMenuActionCloseMenu(_pauseGroup));
            _duckGameUpdateMenu.SetBackFunction(new UIMenuActionCloseMenu(_pauseGroup));
            _duckGameUpdateMenu.Close();
            _pauseGroup.Add(_duckGameUpdateMenu, false);
            _steamWarningMessage = new UIMenu("Steam Not Connected!", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 220f, conString: "@SELECT@ I see...");
            UIMenu steamWarningMessage1 = _steamWarningMessage;
            UIText component18 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            steamWarningMessage1.Add(component18, true);
            UIMenu steamWarningMessage2 = _steamWarningMessage;
            UIText component19 = new UIText("It seems that either you're not logged in", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            steamWarningMessage2.Add(component19, true);
            UIMenu steamWarningMessage3 = _steamWarningMessage;
            UIText component20 = new UIText("to Steam, or Steam failed to authenticate.", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            steamWarningMessage3.Add(component20, true);
            UIMenu steamWarningMessage4 = _steamWarningMessage;
            UIText component21 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            steamWarningMessage4.Add(component21, true);
            UIMenu steamWarningMessage5 = _steamWarningMessage;
            UIText component22 = new UIText("You can still play- but realtime", Color.White, heightAdd: (-num1))
            {
                scale = new Vec2(0.5f)
            };
            steamWarningMessage5.Add(component22, true);
            UIMenu steamWarningMessage6 = _steamWarningMessage;
            UIText component23 = new UIText("features like Online Play and the Workshop", Color.White, heightAdd: (-num1))
            {
                scale = new Vec2(0.5f)
            };
            steamWarningMessage6.Add(component23, true);
            UIMenu steamWarningMessage7 = _steamWarningMessage;
            UIText component24 = new UIText("will be |DGRED|unavailable|PREV|.", Color.White, heightAdd: (-num1))
            {
                scale = new Vec2(0.5f)
            };
            steamWarningMessage7.Add(component24, true);
            UIMenu steamWarningMessage8 = _steamWarningMessage;
            UIText component25 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            steamWarningMessage8.Add(component25, true);
            _steamWarningMessage.SetAcceptFunction(new UIMenuActionCloseMenu(_pauseGroup));
            _steamWarningMessage.SetBackFunction(new UIMenuActionCloseMenu(_pauseGroup));
            _steamWarningMessage.Close();
            _pauseGroup.Add(_steamWarningMessage, false);
            _modsDisabledMenu = new UIMenu("MODS CHANGED!", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 240f, conString: "@SELECT@I see...");
            UIMenu modsDisabledMenu1 = _modsDisabledMenu;
            UIText component26 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            modsDisabledMenu1.Add(component26, true);
            UIMenu modsDisabledMenu2 = _modsDisabledMenu;
            UIText component27 = new UIText("To ensure a smooth update, all enabled", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            modsDisabledMenu2.Add(component27, true);
            UIMenu modsDisabledMenu3 = _modsDisabledMenu;
            UIText component28 = new UIText("mods have been temporarily set to |DGRED|disabled|PREV|.", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            modsDisabledMenu3.Add(component28, true);
            UIMenu modsDisabledMenu4 = _modsDisabledMenu;
            UIText component29 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            modsDisabledMenu4.Add(component29, true);
            UIMenu modsDisabledMenu5 = _modsDisabledMenu;
            UIText component30 = new UIText("Mod compatibility has been a high priority, and", Color.White, heightAdd: (-num1))
            {
                scale = new Vec2(0.5f)
            };
            modsDisabledMenu5.Add(component30, true);
            UIMenu modsDisabledMenu6 = _modsDisabledMenu;
            UIText component31 = new UIText("most mods should work no problem with the new version.", Color.White, heightAdd: (-num1))
            {
                scale = new Vec2(0.5f)
            };
            modsDisabledMenu6.Add(component31, true);
            UIMenu modsDisabledMenu7 = _modsDisabledMenu;
            UIText component32 = new UIText("They can be re-enabled through the |DGORANGE|MANAGE MODS|PREV| menu", Color.White, heightAdd: (-num1))
            {
                scale = new Vec2(0.5f)
            };
            modsDisabledMenu7.Add(component32, true);
            UIMenu modsDisabledMenu8 = _modsDisabledMenu;
            UIText component33 = new UIText("accessible via the top left options console.", Color.White, heightAdd: (-num1))
            {
                scale = new Vec2(0.5f)
            };
            modsDisabledMenu8.Add(component33, true);
            UIMenu modsDisabledMenu9 = _modsDisabledMenu;
            UIText component34 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            modsDisabledMenu9.Add(component34, true);
            UIMenu modsDisabledMenu10 = _modsDisabledMenu;
            UIText component35 = new UIText("Some older mods may |DGRED|not|PREV| work...", Color.White, heightAdd: (-num1))
            {
                scale = new Vec2(0.5f)
            };
            modsDisabledMenu10.Add(component35, true);
            UIMenu modsDisabledMenu11 = _modsDisabledMenu;
            UIText component36 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            modsDisabledMenu11.Add(component36, true);
            UIMenu modsDisabledMenu12 = _modsDisabledMenu;
            UIText component37 = new UIText("Please be mindful of any crashes caused by", Color.White, heightAdd: (-num1))
            {
                scale = new Vec2(0.5f)
            };
            modsDisabledMenu12.Add(component37, true);
            UIMenu modsDisabledMenu13 = _modsDisabledMenu;
            UIText component38 = new UIText("re-enabling specific mods, and use the '|DGBLUE|-nomods|PREV|'", Color.White, heightAdd: (-num1))
            {
                scale = new Vec2(0.5f)
            };
            modsDisabledMenu13.Add(component38, true);
            UIMenu modsDisabledMenu14 = _modsDisabledMenu;
            UIText component39 = new UIText("launch option if you run into trouble!", Color.White, heightAdd: (-num1))
            {
                scale = new Vec2(0.5f)
            };
            modsDisabledMenu14.Add(component39, true);
            UIMenu modsDisabledMenu15 = _modsDisabledMenu;
            UIText component40 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            modsDisabledMenu15.Add(component40, true);
            _modsDisabledMenu.SetAcceptFunction(new UIMenuActionCloseMenu(_pauseGroup));
            _modsDisabledMenu.SetBackFunction(new UIMenuActionCloseMenu(_pauseGroup));
            _modsDisabledMenu.Close();
            _pauseGroup.Add(_modsDisabledMenu, false);
            UIDivider component41 = new UIDivider(true, 0.8f);
            component41.rightSection.Add(new UIImage("pauseIcons", UIAlign.Right), true);
            _mainPauseMenu.Add(component41, true);
            component41.leftSection.Add(new UIMenuItem("RESUME", new UIMenuActionCloseMenu(_pauseGroup)), true);
            component41.leftSection.Add(new UIMenuItem("OPTIONS", new UIMenuActionOpenMenu(_mainPauseMenu, Options.optionsMenu), UIAlign.Left), true);
            component41.leftSection.Add(new UIMenuItem("CREDITS", new UIMenuActionCloseMenuSetBoolean(_pauseGroup, _enterCreditsMenuBool), UIAlign.Left), true);
            component41.leftSection.Add(new UIText("", Color.White), true);
            component41.leftSection.Add(new UIMenuItem("|DGRED|QUIT", new UIMenuActionOpenMenu(_mainPauseMenu, _quitMenu)), true);
            Options.openOnClose = _mainPauseMenu;
            Options.AddMenus(_pauseGroup);
            _mainPauseMenu.Close();
            _pauseGroup.Add(_mainPauseMenu, false);
            _pauseGroup.Close();
            Add(_pauseGroup);
            _font = new BitmapFont("biosFont", 8);
            _background = new Sprite("title/background");
            //this._optionsPlatform = new Sprite("title/optionsPlatform")
            //{
            //    depth = (Depth)0.9f
            //};
            _rightPlatform = new Sprite("title/rightPlatform")
            {
                depth = (Depth)0.9f
            };
            _beamPlatform = new Sprite("title/beamPlatform")
            {
                depth = (Depth)0.9f
            };
            _upperMonitor = new Sprite("title/upperMonitor")
            {
                depth = (Depth)0.85f
            };
            _airlock = new Sprite("title/airlock")
            {
                depth = -0.85f
            };
            _leftPlatform = new Sprite("title/leftPlatform")
            {
                depth = (Depth)0.9f
            };
            _optionsTV = new Sprite("title/optionsTV")
            {
                depth = -0.9f
            };
            _libraryBookcase = new Sprite("title/libraryBookcase")
            {
                depth = -0.9f
            };
            _editorBench = new Sprite("title/editorBench")
            {
                depth = -0.9f
            };
            _editorBenchPaint = new Sprite("title/editorBenchPaint")
            {
                depth = (Depth)0.9f
            };
            _bigUButton = new Sprite("title/bigUButtonPC");
            _bigUButton.CenterOrigin();
            _bigUButton.depth = (Depth)0.95f;
            _controls = new SpriteMap("title/controlsPC", 100, 11);
            _controls.CenterOrigin();
            _controls.depth = (Depth)0.95f;
            _multiBeam = new MultiBeam(160f, -30f);
            Add(_multiBeam);
            _optionsBeam = new OptionsBeam(28f, -110f);
            Add(_optionsBeam);
            _libraryBeam = new LibraryBeam(292f, -110f);
            Add(_libraryBeam);
            _editorBeam = new EditorBeam(28f, 100f);
            Add(_editorBeam);
            VersionSign vs = new VersionSign(176f, 18f);
            Add(vs);
            for (int index = 0; index < 21; ++index)
            {
                SpaceTileset t = new SpaceTileset(index * 16 - 6, 176f)
                {
                    frame = 3,
                    layer = Layer.Game,
                    setLayer = false
                };
                AddThing(t);
            }
            SpriteMap spriteMap = new SpriteMap("duck", 32, 32);
            _space = new SpaceBackgroundMenu(-999f, -999f, true, 0.6f)
            {
                update = false
            };
            Add(_space);
            _things.RefreshState();
            Layer.Game.fade = 0f;
            Layer.Foreground.fade = 0f;
            Add(new Block(120f, 155f, 80f, 30f, PhysicsMaterial.Metal));
            Add(new Block(134f, 148f, 52f, 30f, PhysicsMaterial.Metal));
            Add(new Block(0f, 61f, 63f, 70f, PhysicsMaterial.Metal));
            Add(new Block(257f, 61f, 63f, 60f, PhysicsMaterial.Metal));
            Add(new Spring(90f, 160f, 0.32f));
            Add(new Spring(229f, 160f, 0.32f));
            Add(new VerticalDoor(270f, 160f)
            {
                filterDefault = true
            });
            foreach (Team team in Teams.all)
            {
                int num2;
                int num3 = num2 = 0;
                team.score = num2;
                team.prevScoreboardScore = num3;
            }
            foreach (Profile prof in Profiles.all)
            {
                if (prof.team != null)
                    prof.team.Leave(prof);
                prof.inputProfile = null;
            }
            InputProfile.ReassignDefaultInputProfiles(true);
            if (_arcadeProfile == null)
            {
                TeamSelect2.ControllerLayoutsChanged();
                Teams.Player1.Join(Profiles.DefaultPlayer1);
                Teams.Player2.Join(Profiles.DefaultPlayer2);
                Teams.Player3.Join(Profiles.DefaultPlayer3);
                Teams.Player4.Join(Profiles.DefaultPlayer4);
            }
            else
            {
                Teams.Player1.Join(_arcadeProfile);
                _arcadeProfile.inputProfile = _arcadeInputProfile;
            }
            Input.lastActiveProfile = InputProfile.DefaultPlayer1;
            if (!DuckNetwork.ShowUserXPGain() && Unlockables.HasPendingUnlocks())
                MonoMain.pauseMenu = new UIUnlockBox(Unlockables.GetPendingUnlocks().ToList(), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f);
            base.Initialize();
        }

        private void Join()
        {
        }

        private void OptionsSaveAndClose()
        {
            Options.Save();
            Options.SaveLocalData();
            _optionsGroup.Close();
        }

        public override void OnSessionEnded(DuckNetErrorInfo error)
        {
            Teams.Player1.Join(Profiles.DefaultPlayer1);
            Teams.Player2.Join(Profiles.DefaultPlayer2);
            Teams.Player3.Join(Profiles.DefaultPlayer3);
            Teams.Player4.Join(Profiles.DefaultPlayer4);
            InputProfile.ReassignDefaultInputProfiles();
        }

        public override void Update()
        {
            if (_duck != null && DGSave.showOnePointFiveMessages)
            {
                if (!showedNewVersionStartup && !DuckFile.freshInstall && DGSave.upgradingFromVanilla)
                {
                    MonoMain.pauseMenu = _duckGameUpdateMenu;
                    _duckGameUpdateMenu.Open();
                    showedNewVersionStartup = true;
                    return;
                }
                if (!showedModsDisabled && DGSave.showModsDisabledMessage)
                {
                    MonoMain.pauseMenu = _modsDisabledMenu;
                    _modsDisabledMenu.Open();
                    showedModsDisabled = true;
                    return;
                }
                DGSave.showOnePointFiveMessages = false;
            }
            int num = 1;
            if (startStars)
            {
                num = 250;
                startStars = false;
            }
            if (_duck != null && _duck.profile != null && !_duck.profile.inputProfile.HasAnyConnectedDevice())
            {
                foreach (InputProfile defaultProfile in InputProfile.defaultProfiles)
                {
                    if (defaultProfile.HasAnyConnectedDevice())
                    {
                        InputProfile.SwapDefaultInputStrings(defaultProfile.name, _duck.profile.inputProfile.name);
                        InputProfile.ReassignDefaultInputProfiles();
                        _duck.profile.inputProfile = defaultProfile;
                        break;
                    }
                }
            }
            if (!_enterCredits && !_enterMultiplayer && _duck != null && _duck.inputProfile.Pressed(Triggers.Start))
            {
                _pauseGroup.Open();
                _mainPauseMenu.Open();
                SFX.Play("pause", 0.6f);
                MonoMain.pauseMenu = _pauseGroup;
            }

            if ((_multiBeam.entered || !_fadeInFull))
            {
                if (Input.Pressed(Triggers.Grab))
                {
                    if (_enterEditor)
                    {
                        current = Main.editor;
                    }
                    _enterEditor = true;
                }
                if (Input.Pressed(Triggers.Shoot))
                {
                    if (_enterMultiplayer)
                    {
                        for (int i = 1; i < Teams.all.Count; i++)
                        {
                            //if (Teams.all[i].activeProfiles.Find(p => p.inputProfile == profileWithDevice) != null) continue;
                            Teams.all[i].ClearProfiles();
                        }
                        current = new TeamSelect2();
                    }
                    _fastMultiplayer = true;
                    _enterMultiplayer = true;
                }
            }
            for (int index = 0; index < num; ++index)
            {
                starWait -= Maths.IncFrameTimer();
                if (starWait < 0.0)
                {
                    starWait = 0.1f + Rando.Float(0.2f);
                    Color color = Colors.DGRed;
                    if (cpick == 1)
                        color = Colors.DGBlue;
                    else if (cpick == 2)
                        color = Colors.DGGreen;
                    if (Rando.Float(1f) > 0.995f)
                        color = Colors.DGPink;
                    particles.Add(new StarParticle()
                    {
                        pos = new Vec2(0f, (int)(Rando.Float(0f, 150f) / 1.0)),
                        speed = new Vec2(Rando.Float(0.5f, 1f), 0f),
                        color = color,
                        flicker = Rando.Float(100f, 230f)
                    });
                    ++cpick;
                    if (cpick > 2)
                        cpick = 0;
                }
                List<StarParticle> starParticleList = new List<StarParticle>();
                foreach (StarParticle particle in particles)
                {
                    particle.pos += particle.speed * (float)(0.5 + (1.0 - extraFade) * 0.5);
                    if (particle.pos.x > 300.0 && !_enterCredits || particle.pos.x > 680.0)
                        starParticleList.Add(particle);
                }
                foreach (StarParticle starParticle in starParticleList)
                    particles.Remove(starParticle);
            }
            if (menuOpen)
            {
                Layer.Game.fade = Lerp.Float(Layer.Game.fade, 0.2f, 0.02f);
                Layer.Foreground.fade = Lerp.Float(Layer.Foreground.fade, 0.2f, 0.02f);
                Layer.Background.fade = Lerp.Float(Layer.Foreground.fade, 0.2f, 0.02f);
            }
            else
            {
                Layer.Game.fade = Lerp.Float(Layer.Game.fade, _fadeInFull ? 1f : (_fadeIn ? 0.5f : 0f), _fadeInFull ? 0.01f : 3f / 500f);
                Layer.Foreground.fade = Lerp.Float(Layer.Foreground.fade, _fadeIn ? 1f : 0f, 0.01f);
                Layer.Background.fade = Lerp.Float(Layer.Background.fade, _fadeBackground ? 0f : 1f, 0.02f);
            }
            if (_enterArcade)
            {
                ++_duck.x;
                _duck.immobilized = true;
                _duck.enablePhysics = false;
                Graphics.fade = Lerp.Float(Graphics.fade, 0f, 0.05f);
                if (Graphics.fade < 0.01f)
                {
                    current.Clear();
                    current = new ArcadeLevel(Content.GetLevelID("arcade"));
                }
            }
            else
            {
                if (_enterCredits)
                {
                    _duck.immobilized = true;
                    _duck.updatePhysics = false;
                    if (camera.x < 140.0)
                    {
                        flashDissipationSpeed = 0.08f;
                        Graphics.flashAdd = 2f;
                        camera.x += 330f;
                        foreach (StarParticle particle in particles)
                            particle.pos.x += 320f;
                    }
                    else
                    {
                        switchWait -= 0.04f;
                        if (switchWait <= 0.0)
                        {
                            if (!_startedMusic)
                                Music.volumeMult = Lerp.Float(Music.volumeMult, 0f, 3f / 500f);
                            if (Layer.Parallax.camera.y > -12.0)
                            {
                                camera.y += 0.064f;
                                Layer.Parallax.camera.y -= 0.08f;
                            }
                            else
                            {
                                if (!_startedMusic)
                                {
                                    Music.volumeMult = 1.2f;
                                    Music.Play("tabledoodles", false);
                                    _startedMusic = true;
                                }
                                if (creditsScroll > 939.0)
                                {
                                    if (Layer.Parallax.camera.y > -22.0)
                                    {
                                        camera.y += 0.064f;
                                        Layer.Parallax.camera.y -= 0.08f;
                                    }
                                    extraFade -= 0.01f;
                                    if (extraFade < 0.0)
                                        extraFade = 0f;
                                }
                                if (creditsScroll > 2650.0 && !shownPrompt)
                                {
                                    shownPrompt = true;
                                    HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@Exit");
                                }
                                if (creditsScroll < 100.0 && !shownPromptF)
                                {
                                    HUD.AddCornerControl(HUDCorner.BottomRight, "@SHOOT@Fast");
                                    shownPromptF = true;
                                }
                                else if (creditsScroll > 100.0 && shownPromptF)
                                {
                                    HUD.CloseCorner(HUDCorner.BottomRight);
                                    shownPromptF = false;
                                }

                                if (Input.Down(Triggers.Shoot))
                                    creditsScroll += 2f;
                                else
                                    creditsScroll += 0.25f;
                            }
                        }
                    }
                    if (!_duck.inputProfile.Pressed(Triggers.Cancel))
                        return;
                    _enterCredits = false;
                    quittingCredits = true;
                    return;
                }
                if (_duck != null)
                {
                    _duck.updatePhysics = true;
                    if (_duck.x > 324.0)
                        _enterArcade = true;
                }
                if (quittingCredits)
                {
                    flashDissipationSpeed = 0.08f;
                    Graphics.flashAdd = 2f;
                    camera.x = 0f;
                    foreach (StarParticle particle in particles)
                        particle.pos.x -= 320f;
                    camera.y = 0f;
                    Layer.Parallax.camera.y = 0f;
                    creditsScroll = 0f;
                    extraFade = 1f;
                    _startedMusic = false;
                    starWait = 0f;
                    switchWait = 1f;
                    creditsScroll = 0f;
                    startStars = true;
                    quittingCredits = false;
                    shownPrompt = false;
                    shownPromptF = false;
                    HUD.CloseAllCorners();
                    _duck.immobilized = false;
                    Music.Play("Title");
                }
            }
            Music.volumeMult = 1f;
            _hasMenusOpen = menuOpen;
            if (!_enterMultiplayer && !_enterEditor && !_enterLibrary) // && !this._enterBuyScreen
            {
                if (Graphics.fade < 1.0)
                    Graphics.fade += 1f / 1000f;
                else
                    Graphics.fade = 1f;
            }
            else
            {
                Graphics.fade -= 0.05f;
                if (Graphics.fade <= 0.0)
                {
                    Graphics.fade = 0f;
                    Music.Stop();
                    if (_enterMultiplayer)
                    {
                        if (_fastMultiplayer)
                        {
                            for (int i = 1; i < Teams.all.Count; i++)
                            {
                                //if (Teams.all[i].activeProfiles.Find(p => p.inputProfile == profileWithDevice) != null) continue;
                                Teams.all[i].ClearProfiles();
                            }
                        }
                        else
                        {
                            foreach (Team team in Teams.all)
                                team.ClearProfiles();
                        }
                        current = new TeamSelect2();
                    }
                    else if (_enterEditor)
                        current = Main.editor;
                    else if (_enterLibrary)
                        current = new DoorRoom();
                    // else if (this._enterBuyScreen)
                    //  Level.current = new BuyScreen(Main.currencyType, Main.price);
                }
            }
            _pressStartBlink += 0.01f;
            if (_pressStartBlink > 1.0)
                --_pressStartBlink;
            if (_duck != null)
            {
                if (_dontQuit.value)
                {
                    _dontQuit.value = false;
                    _duck.hSpeed = 10f;
                }
                if (_quit.value)
                {
                    MonoMain.exit = true;
                    return;
                }
                //if (InputProfile.active.Pressed(Triggers.Start) && Main.foundPurchaseInfo && Main.isDemo) free my people no buying here
                //{
                //    this._enterBuyScreen = true;
                //    this._duck.immobilized = true;
                //}
            }
            if (_enterCreditsMenuBool.value)
            {
                _enterCreditsMenuBool.value = false;
                _enterCredits = true;
                _duck.immobilized = true;
            }
            if (_multiBeam.entered)
            {
                _selectionTextDesired = "MULTIPLAYER";
                _desiredSelection = TitleMenuSelection.Play;
                if (!_enterMultiplayer && _duck.inputProfile.Pressed(Triggers.Select) && MonoMain.pauseMenu == null)
                {
                    SFX.Play("plasmaFire");
                    _enterMultiplayer = true;
                    _duck.immobilized = true;
                }
            }
            else if (_optionsBeam.entered)
            {
                _selectionTextDesired = "OPTIONS";
                _desiredSelection = TitleMenuSelection.Options;
                if (!Options.menuOpen && _duck.inputProfile.Pressed(Triggers.Select))
                {
                    SFX.Play("plasmaFire");
                    _optionsGroup.Open();
                    _optionsMenu.Open();
                    MonoMain.pauseMenu = _optionsGroup;
                    _duck.immobilized = true;
                }
            }
            else if (_libraryBeam.entered)
            {
                _selectionTextDesired = "LIBRARY";
                _desiredSelection = TitleMenuSelection.Stats;
                if (_duck.inputProfile.Pressed(Triggers.Select) && Profiles.allCustomProfiles.Count > 0 && MonoMain.pauseMenu == null)
                {
                    SFX.Play("plasmaFire");
                    _enterLibrary = true;
                    _duck.immobilized = true;
                }
            }
            else if (_editorBeam.entered)
            {
                _selectionTextDesired = "LEVEL EDITOR";
                _desiredSelection = TitleMenuSelection.Editor;
                if (_duck.inputProfile.Pressed(Triggers.Select) && MonoMain.pauseMenu == null)
                {
                    SFX.Play("plasmaFire");
                    _enterEditor = true;
                    _duck.immobilized = true;
                }
            }
            else
            {
                _selectionTextDesired = " ";
                _desiredSelection = TitleMenuSelection.None;
            }
            _controlsFrameDesired = !(_selectionTextDesired != " ") ? 2 : 1;
            if (_selectionText != _selectionTextDesired)
            {
                _selectionFade -= 0.1f;
                if (_selectionFade <= 0.0)
                {
                    _selectionFade = 0f;
                    _selectionText = _selectionTextDesired;
                    _selection = _desiredSelection;
                }
            }
            else
                _selectionFade = Lerp.Float(_selectionFade, 1f, 0.1f);
            if (_controlsFrame != _controlsFrameDesired)
            {
                _controlsFade -= 0.1f;
                if (_controlsFade <= 0.0)
                {
                    _controlsFade = 0f;
                    _controlsFrame = _controlsFrameDesired;
                }
            }
            else
                _controlsFade = Lerp.Float(_controlsFade, 1f, 0.1f);
            if (_returnFromArcade)
            {
                if (!_fadeIn)
                {
                    _fadeIn = true;
                    _title = new BigTitle();
                    _title.x = (float)(Layer.HUD.camera.width / 2.0 - _title.graphic.w / 2 + 3.0);
                    _title.y = Layer.HUD.camera.height / 2f;
                    AddThing(_title);
                    _title.fade = true;
                    _title.alpha = 0f;
                    Layer.Game.fade = 1f;
                    Layer.Foreground.fade = 1f;
                    Layer.Background.fade = 1f;
                    _arcadeProfile.inputProfile = _arcadeInputProfile;
                    _duck = new Duck(310f, 160f, _arcadeProfile)
                    {
                        offDir = -1
                    };
                    InputProfile.active = _duck.profile.inputProfile;
                }
                Graphics.fade = Lerp.Float(Graphics.fade, 1f, 0.05f);
                if (Graphics.fade > 0.99f)
                {
                    Graphics.fade = 1f;
                    _returnFromArcade = false;
                }
            }
            if (_fadeIn && !_fadeInFull)
            {
                if (!_returnFromArcade)
                    _duck = null;
                //if (TitleScreen.firstStart && ParentalControls.AreParentalControlsActive())
                //{
                //    MonoMain.pauseMenu = _parentalControlsMenu;
                //    this._parentalControlsMenu.Open();
                //    TitleScreen.firstStart = false;
                //}
                foreach (Profile defaultProfile in Profiles.defaultProfiles)
                {
                    if (defaultProfile.inputProfile.JoinGamePressed())
                    {
                        Join();
                        foreach (Profile profile in Profiles.all)
                            profile.team = null;
                        defaultProfile.ApplyDefaults();
                        _duck = new Duck(160f, 60f, defaultProfile);
                        if (SFX.NoSoundcard)
                            HUD.AddInputChangeDisplay("@UNPLUG@|RED|No Soundcard Detected!!");
                    }
                }
                if (_duck != null)
                {
                    //if (Main.foundPurchaseInfo && Main.isDemo)
                    //    HUD.AddCornerControl(HUDCorner.TopRight, "@START@BUY GAME", this._duck.inputProfile);
                    InputProfile.active = _duck.profile.inputProfile;
                    _fadeInFull = true;
                    _title.fade = true;
                    Add(_duck);
                }
            }
            else if (_duck != null)
            {
                if (_duck.dead)
                {
                    Remove(_duck);
                    _duck = new Duck(160f, 60f, _duck.profile);
                    Add(_duck);
                    HUD.AddInputChangeDisplay(" Cmon Now That Was Dumb, Dont You Agree? ");
                }
                foreach (Profile defaultProfile in Profiles.defaultProfiles)
                {
                    foreach (string trigger in Triggers.SimpleTriggerList)
                    {
                        if (defaultProfile.inputProfile.Pressed(trigger, false))
                        {
                            _duck.profile = defaultProfile;
                            InputProfile.active = _duck.profile.inputProfile;
                            break;
                        }
                    }
                }
            }
            _space.parallax.y = -80f;
            moveWait -= 0.02f;
            if (moveWait < 0.0)
            {
                if (_title == null)
                {
                    _title = new BigTitle();
                    _title.x = (float)(Layer.HUD.camera.width / 2.0 - _title.graphic.w / 2 + 3.0);
                    _title.y = Layer.HUD.camera.height / 2f;
                    AddThing(_title);
                }
                moveSpeed = Maths.LerpTowards(moveSpeed, 0f, 0.0015f);
            }
            if (_title == null)
                return;
            ++wait;
            if (wait == 60)
                flash = 1f;
            if (wait == 60)
            {
                _title.graphic.color = Color.White;
                _title.alpha = 1f;
                _fadeIn = true;
            }
            if (flash > 0.0)
            {
                flash -= 0.016f;
                dim -= 0.08f;
                if (dim >= 0.0)
                    return;
                dim = 0f;
            }
            else
                flash = 0f;
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer == Layer.Foreground)
            {
                Graphics.Draw(_upperMonitor, 84f, 0f);
                if (_fadeInFull)
                {
                    _font.alpha = _selectionFade;
                    _font.inputProfile = _duck.inputProfile;
                    if (_selection == TitleMenuSelection.None)
                    {
                        string text = "@WASD@MOVE @JUMP@JUMP";
                        _font.Draw(text, current.camera.PercentW(50f) - _font.GetWidth(text) / 2f, 16f, Color.White, (Depth)0.95f);
                    }
                    else if (_selection == TitleMenuSelection.Play)
                    {
                        string text = "@SELECT@PLAY GAME";
                        _font.Draw(text, current.camera.PercentW(50f) - _font.GetWidth(text) / 2f, 16f, Color.White, (Depth)0.95f);
                    }
                    else if (_selection == TitleMenuSelection.Stats)
                    {
                        if (Profiles.allCustomProfiles.Count > 0)
                        {
                            string text = "@SELECT@LIBRARY";
                            _font.Draw(text, current.camera.PercentW(50f) - _font.GetWidth(text) / 2f, 16f, Color.White, (Depth)0.95f);
                        }
                    }
                    else if (_selection == TitleMenuSelection.Options)
                    {
                        string text = "@SELECT@OPTIONS";
                        _font.Draw(text, current.camera.PercentW(50f) - _font.GetWidth(text) / 2f, 16f, Color.White, (Depth)0.95f);
                    }
                    else if (_selection == TitleMenuSelection.Editor)
                    {
                        string text = "@SELECT@EDITOR";
                        _font.Draw(text, current.camera.PercentW(50f) - _font.GetWidth(text) / 2f, 16f, Color.White, (Depth)0.95f);
                    }
                    //Graphics.Draw(_editorBenchPaint, 45f, 168f);
                }
                else
                {
                    string text = "PRESS START";
                    string text2 = "@GRAB@";
                    string text3 = "@SHOOT@";

                    InputProfile profileWithDevice = InputProfile.FirstProfileWithDevice;

                    if (_pressStartBlink >= 0.5)
                    {
                        _font.Draw(text, current.camera.PercentW(50f) - _font.GetWidth(text) / 2f, 15f, Color.White, (Depth)0.95f);
                        text2 = "GO TO EDITOR";
                        text3 = "GO TO LOBBY";
                        _font.Draw(text2, current.camera.PercentW(50f) - _font.GetWidth(text2) / 2f, 4f, Color.White, (Depth)0.95f, profileWithDevice);
                        _font.Draw(text3, current.camera.PercentW(50f) - _font.GetWidth(text3) / 2f, 26f, Color.White, (Depth)0.95f, profileWithDevice);
                    }
                    else
                    {
                        Graphics.DrawString("@START@", new Vec2(current.camera.PercentW(50f) - (_font.GetWidth("@START@", false, profileWithDevice) - 1f) / 2f, 15f), Color.White, (Depth)0.9f, profileWithDevice);
                        _font.Draw(text2, current.camera.PercentW(50f) - (_font.GetWidth(text2, false, profileWithDevice) - 1f) / 2f, 3f, Color.White, (Depth)0.95f, profileWithDevice);
                        _font.Draw(text3, current.camera.PercentW(50f) - (_font.GetWidth(text3, false, profileWithDevice) - 1f) / 2f, 27f, Color.White, (Depth)0.95f, profileWithDevice);
                    }
                }
            }
            else if (layer == Layer.Game)
            {
                Graphics.Draw(_editorBenchPaint, 45f, 168f);
                Graphics.Draw(_leftPlatform, 0f, 61f);
                Graphics.Draw(_airlock, 266f, 135f);
                Graphics.Draw(_rightPlatform, byte.MaxValue, 61f);
                Graphics.Draw(_beamPlatform, 118f, 146f);
                Graphics.Draw(_optionsTV, 0f, 19f);
                Graphics.Draw(_libraryBookcase, 263f, 12f);
                Graphics.Draw(_editorBench, 1f, 130f);
                if (creditsScroll > 0.1)
                {
                    float num1 = 0f;
                    foreach (List<string> stringList in creditsRoll)
                    {
                        float num2 = num1 + (200f - creditsScroll);
                        if (num2 >= -11.0 && num2 < 200.0)
                        {
                            if (stringList.Count == 1)
                            {
                                float stringWidth = Graphics.GetStringWidth(stringList[0]);
                                Graphics.DrawStringColoredSymbols(stringList[0], new Vec2((float)(490.0 - stringWidth / 2.0), num1 + (200f - creditsScroll)), Color.White, (Depth)1f);
                            }
                            else
                            {
                                double stringWidth1 = Graphics.GetStringWidth(stringList[0]);
                                Graphics.DrawStringColoredSymbols(stringList[0], new Vec2(347f, num1 + (200f - creditsScroll)), Color.White, (Depth)1f);
                                double stringWidth2 = Graphics.GetStringWidth(stringList[1]);
                                Graphics.DrawStringColoredSymbols(stringList[1], new Vec2(507f, num1 + (200f - creditsScroll)), Color.White, (Depth)1f);
                            }
                        }
                        num1 += 11f;
                    }
                }
            }
            else if (layer == Layer.Parallax)
            {
                float num = 0f;
                if (camera.y > 4.0)
                {
                    _starField.alpha = num + (float)((camera.y - 4.0) / 13.0) - extraFade * 0.7f;
                    Graphics.Draw(_starField, 0f, layer.camera.y - 58f, -0.99f);
                }
            }
            else if (layer == Layer.Background)
            {
                foreach (StarParticle particle in particles)
                {
                    float num3 = Math.Max(1f - Math.Min(Math.Abs(particle.pos.x - particle.flicker) / 10f, 1f), 0f);
                    float num4 = 0.2f;
                    if (camera.y > 0.0)
                        num4 += camera.y / 52f;
                    Graphics.DrawRect(particle.pos, particle.pos + new Vec2(1f, 1f), Color.White * (float)((num4 + num3 * 0.6f) * (0.3f + (1.0 - extraFade) * 0.7f)), -0.3f);
                    float num5 = 0.1f;
                    if (camera.y > 0.0)
                        num5 += camera.y / 52f;
                    Vec2 pos = particle.pos;
                    int num6 = 4;
                    for (int index = 0; index < num6; ++index)
                    {
                        float num7 = particle.speed.x * 8f;
                        Graphics.DrawLine(pos + new Vec2(-num7, 0.5f), pos + new Vec2(0f, 0.5f), particle.color * ((float)(1.0 - index / num6) * num5) * (float)(0.3f + (1.0 - extraFade) * 0.7f), depth: (-0.4f));
                        pos.x -= num7;
                    }
                }
                _background.depth = (Depth)0f;
                Rectangle sourceRectangle = new Rectangle(0f, 0f, 90f, _background.height);
                Graphics.Draw(_background, 0f, 0f, sourceRectangle);
                sourceRectangle = new Rectangle(63f, 107f, 194f, 61f);
                Graphics.Draw(_background, sourceRectangle.x, sourceRectangle.y, sourceRectangle);
                sourceRectangle = new Rectangle(230f, 61f, 28f, 61f);
                Graphics.Draw(_background, sourceRectangle.x, sourceRectangle.y, sourceRectangle);
                sourceRectangle = new Rectangle(230f, 0f, 90f, 61f);
                Graphics.Draw(_background, sourceRectangle.x, sourceRectangle.y, sourceRectangle);
                sourceRectangle = new Rectangle(230f, 124f, 90f, 56f);
                Graphics.Draw(_background, sourceRectangle.x, sourceRectangle.y, sourceRectangle);
                sourceRectangle = new Rectangle(90f, 0f, 140f, 50f);
                Graphics.Draw(_background, sourceRectangle.x, sourceRectangle.y, sourceRectangle);
            }
            base.PostDrawLayer(layer);
        }
    }
}
