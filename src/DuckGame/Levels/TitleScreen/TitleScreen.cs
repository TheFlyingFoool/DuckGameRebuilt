// Decompiled with JetBrains decompiler
// Type: DuckGame.TitleScreen
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
        private bool _enterMultiplayer;
        private UIComponent _optionsGroup;
        private MenuBoolean _quit = new MenuBoolean();
        private MenuBoolean _dontQuit = new MenuBoolean();
        private UIMenu _quitMenu;
        private UIMenu _optionsMenu;
        private UIMenu _controlConfigMenu;
        private UIMenu _graphicsMenu;
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

        public TitleScreen(bool returnFromArcade, Profile arcadeProfile)
        {
            this._centeredView = true;
            this._returnFromArcade = returnFromArcade;
            this._arcadeProfile = arcadeProfile;
            if (arcadeProfile == null)
                return;
            this._arcadeInputProfile = arcadeProfile.inputProfile;
        }

        public bool menuOpen => Options.menuOpen || this._enterMultiplayer;

        private void CloudDelete()
        {
            Cloud.DeleteAllCloudData(false);
            DuckFile.DeleteAllSaveData();
        }

        public static bool hasMenusOpen => TitleScreen._hasMenusOpen;

        private void AddCreditLine(params string[] s) => this.creditsRoll.Add(new List<string>(s));

        public override void Initialize()
        {
            Vote.ClearVotes();
            Program.gameLoadedSuccessfully = true;
            Global.Save();
            HUD.ClearPlayerChangeDisplays();
            this.AddCreditLine("DUCK GAME");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@COMMUNITY HERO@RWINGGRAY@");
            this.AddCreditLine("John \"BroDuck\" Pichardo");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@LEAD TESTERS@RWINGGRAY@");
            this.AddCreditLine("Jacob Paul");
            this.AddCreditLine("Tyler Molz");
            this.AddCreditLine("Andrew Morrish");
            this.AddCreditLine("Dayton McKay");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@MIGHTY HELPFUL FRIEND@RWINGGRAY@");
            this.AddCreditLine("|DGGREEN|YupDanielThatsMe");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@HEROS OF MOD@RWINGGRAY@");
            this.AddCreditLine("Dord");
            this.AddCreditLine("YupDanielThatsMe");
            this.AddCreditLine("YoloCrayolo3");
            this.AddCreditLine("Zloty_Diament");
            this.AddCreditLine("eim64");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@TESTERS@RWINGGRAY@");
            this.AddCreditLine("John Pichardo", "Lotad");
            this.AddCreditLine("Tufukins", "Sleepy Jirachi");
            this.AddCreditLine("Paul Hartling", "thebluecosmonaut");
            this.AddCreditLine("Dan Gaechter", "James Nieves");
            this.AddCreditLine("Dr. Docter", "ShyNieke");
            this.AddCreditLine("Spectaqual", "RealGnomeTasty");
            this.AddCreditLine("Karim Aifi", "Zaahck");
            this.AddCreditLine("dino rex (guy)", "Peter Smith");
            this.AddCreditLine("Colin Jacobson", "mage legend");
            this.AddCreditLine("YvngXero", "Trevor Etzold");
            this.AddCreditLine("Fluury", "Phantom329");
            this.AddCreditLine("Kevin Duffy", "Michael Niemann");
            this.AddCreditLine("Zloty_Diament", "Ben");
            this.AddCreditLine("Bolus", "Unluck");
            this.AddCreditLine("Temppuuh", "Rasenshriken");
            this.AddCreditLine("Andresian", "Spencer Portwood");
            this.AddCreditLine("James \"Sunder\" Beliakoff");
            this.AddCreditLine("David Sabosky (SidDaSloth)");
            this.AddCreditLine("Jordan \"Renim\" Gauge");
            this.AddCreditLine("Tommaso \"Giampiero\" Bresciani");
            this.AddCreditLine("Nicodemo \"Nikkodemus\" Bresciani");
            this.AddCreditLine("Valentin Zeyfang (RedMser)");
            this.AddCreditLine("Luke Bromley (mrred55)");
            this.AddCreditLine("Christopher Alan Bell");
            this.AddCreditLine("Koteeevvv");
            this.AddCreditLine("Soh", "NiK0");
            this.AddCreditLine("kalamari");
            this.AddCreditLine("Mike Timofeev");
            this.AddCreditLine("JYAD (Just Your Average Duck)");
            this.AddCreditLine(" Argo The Rat");
            this.AddCreditLine("Adam Urbina");
            this.AddCreditLine("Leonardo \"Baffo\" Magnani");
            this.AddCreditLine("The Burger Always Wins");
            this.AddCreditLine("RaV3_past");
#if DuckGameTurbo
            this.AddCreditLine("Collin But Faster", "|DGPURPLE|Drake");
#elif DuckGame
            this.AddCreditLine("Collin Based", "|DGPURPLE|Drake");
#else
            this.AddCreditLine("Collin", "|DGPURPLE|Drake");
#endif
            this.AddCreditLine("Tater");
            this.AddCreditLine("");
            this.AddCreditLine("Jaydex72");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@EDITOR SEARCH IDEA@RWINGGRAY@");
            this.AddCreditLine("Zloty_Diament");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@A DUCK GAME COSPLAYER@RWINGGRAY@");
            this.AddCreditLine("Colin Lamb");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@SLEEPY AND FRIENDS@RWINGGRAY@");
            this.AddCreditLine("Lotad");
            this.AddCreditLine("Sleepy Jirachi");
            this.AddCreditLine("Silverlace");
            this.AddCreditLine("Slimy");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@FEATHERS WILL FLY CREW@RWINGGRAY@");
            this.AddCreditLine("Dan \"lucidinertia\" Myszak");
            this.AddCreditLine("Yannick \"Becer\" Marcotte-Gourde");
            this.AddCreditLine("Aleksander \"Acrimonious Defect\" K.D.");
            this.AddCreditLine("Tater", "KlockworkCanary");
            this.AddCreditLine("Conre", "Xatmamune");
            this.AddCreditLine("White Ink", "CaptainCrack");
            this.AddCreditLine("laduck", "This Guy");
            this.AddCreditLine("Repiteo", "VirtualFishbowl");
            this.AddCreditLine("Slinky", "JaYlab212");
            this.AddCreditLine("", "");
            this.AddCreditLine("The Entire FWF Community!");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@RUSS MONEY@RWINGGRAY@");
            this.AddCreditLine("AS HIMSELF");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("DEVELOPMENT TEAM");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@ART, PROGRAMMING, MUSIC@RWINGGRAY@");
            this.AddCreditLine("NiK0");
            this.AddCreditLine("Dan");
            this.AddCreditLine("Collin");
            this.AddCreditLine("Landon Podbielski");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@ROOM FURNITURE@RWINGGRAY@");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@HOME UPDATE HAT ART@RWINGGRAY@");
            this.AddCreditLine("Dayton McKay");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@MOD SUPPORT PROGRAMMER@RWINGGRAY@");
            this.AddCreditLine("Paril");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@WAFFLES, CLAMS, HIGHFIVES HATS@RWINGGRAY@");
            this.AddCreditLine("Lindsey Layne King");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@EGGPAL, BRAD HATS@RWINGGRAY@");
            this.AddCreditLine("mushbuh");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@KERCHIEFS, POSTALS, WAHHS HATS@RWINGGRAY@");
            this.AddCreditLine("Case Marsteller");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@B52s, UUFOS HATS@RWINGGRAY@");
            this.AddCreditLine("William Baldwin");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@WOLVES HAT, MILKSHAKE@RWINGGRAY@");
            this.AddCreditLine("Dord");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("NINTENDO SWITCH PORT");
            this.AddCreditLine("&");
            this.AddCreditLine("DEFINITIVE EDITION");
            this.AddCreditLine("Armature Studio");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@ENGINEERING@RWINGGRAY@");
            this.AddCreditLine("John Allensworth");
            this.AddCreditLine("Tom Ivey");
            this.AddCreditLine("Bryan Wagstaff");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@DIRECTOR OF PRODUCTION@RWINGGRAY@");
            this.AddCreditLine("Mark Nau");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@PRODUCTION@RWINGGRAY@");
            this.AddCreditLine("Tom Ivey");
            this.AddCreditLine("Mike Pirrone");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@QUALITY ASSURANCE@RWINGGRAY@");
            this.AddCreditLine("Gwen Dalmacio");
            this.AddCreditLine("Mike Pirrone");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@ARMATURE OPERATIONS@RWINGGRAY@");
            this.AddCreditLine("Nadine Rossignol");
            this.AddCreditLine("Nicole Casarona");
            this.AddCreditLine("Michael Thai");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@BUSINESS DEVELOPMENT@RWINGGRAY@");
            this.AddCreditLine("Jonathan Zamkoff");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@STUDIO EXECUTIVES@RWINGGRAY@");
            this.AddCreditLine("Greg John");
            this.AddCreditLine("Todd Keller");
            this.AddCreditLine("Mark Pacini");
            this.AddCreditLine("Jonathan Zamkoff");
            this.AddCreditLine("");
            this.AddCreditLine("|CREDITSGRAY|@LWINGGRAY@SPECIAL THANKS@RWINGGRAY@");
            this.AddCreditLine("Vitor Menezes");
            this.AddCreditLine("Wayne Sikes");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("ADULT SWIM GAMES");
            this.AddCreditLine("");
            this.AddCreditLine("Liz Pate");
            this.AddCreditLine("Jacob Paul");
            this.AddCreditLine("Brian Marquez");
            this.AddCreditLine("Daniel Fuller");
            this.AddCreditLine("Peter Gollan");
            this.AddCreditLine("Mariam Naziripour");
            this.AddCreditLine("Dan Nichols");
            this.AddCreditLine("Adam Baptiste");
            this.AddCreditLine("Case Marsteller");
            this.AddCreditLine("");
            this.AddCreditLine("Chris Johnston");
            this.AddCreditLine("Steve Gee");
            this.AddCreditLine("Charles Park");
            this.AddCreditLine("Kyle Young");
            this.AddCreditLine("Duke Nguyen");
            this.AddCreditLine("Andre Curtis");
            this.AddCreditLine("Briana Chichester");
            this.AddCreditLine("William Baldwin");
            this.AddCreditLine("Taylor Anderson-Barkley");
            this.AddCreditLine("Josh Terry");
            this.AddCreditLine("Maddie Beasley");
            this.AddCreditLine("Justin Morris");
            this.AddCreditLine("Joseph DuBois");
            this.AddCreditLine("Lindsey Wade");
            this.AddCreditLine("Adam Hatch");
            this.AddCreditLine("Kristy Sottilaro");
            this.AddCreditLine("");
            this.AddCreditLine("Jeff Olsen");
            this.AddCreditLine("Tucker Dean");
            this.AddCreditLine("Elizabeth Murphy");
            this.AddCreditLine("David Verble");
            this.AddCreditLine("Sean Baptiste");
            this.AddCreditLine("Jacqui Collins");
            this.AddCreditLine("Zo Douglas");
            this.AddCreditLine("Megan Fausti");
            this.AddCreditLine("Abigail Tyson");
            this.AddCreditLine("Ryan Murray");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("Thank you OUYA for publishing");
            this.AddCreditLine("the original version of Duck Game.");
            this.AddCreditLine("Especially Bob Mills, who");
            this.AddCreditLine("made it all happen.");
            this.AddCreditLine("");
            this.AddCreditLine("We need to go camping again.");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("Thank you ADULT SWIM GAMES");
            this.AddCreditLine("for publishing Duck Game, and");
            this.AddCreditLine("for doing so much promotion and");
            this.AddCreditLine("testing.");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("Thank you Paril for");
            this.AddCreditLine("writing the mod support for Duck Game.");
            this.AddCreditLine("Mods wouldn't have been possible");
            this.AddCreditLine("without you.");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("BroDuck you've been a huge help");
            this.AddCreditLine("keeping the community running,");
            this.AddCreditLine("I don't know what would have happened");
            this.AddCreditLine("without your help.");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("DORD your weapons mod is absolutely");
            this.AddCreditLine("amazing and beautiful.");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("Thank you everyone for playing");
            this.AddCreditLine("Duck Game, for all your support,");
            this.AddCreditLine("and for being so kind.");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("");
            this.AddCreditLine("The End");
            for (int index = 0; index < 300; ++index)
                this.AddCreditLine("");
            this.AddCreditLine("Cya later!");
            if (!DG.InitializeDRM())
            {
                Level.current = new BetaScreen();
            }
            else
            {
                this._starField = new Sprite("background/starField");
                TeamSelect2.DefaultSettings();
                if (Network.isActive)
                    Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Returned to title screen."));
                if (Music.currentSong != "Title" && Music.currentSong != "TitleDemo" || Music.finished)
                    Music.Play("Title");
                if (GameMode.playedGame)
                    GameMode.playedGame = false;
                this._optionsGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0.0f, 0.0f);
                this._optionsMenu = new UIMenu("@WRENCH@OPTIONS@SCREWDRIVER@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, conString: "@CANCEL@BACK @SELECT@SELECT");
                this._controlConfigMenu = new UIControlConfig(this._optionsMenu, "@WRENCH@DEVICE DEFAULTS@SCREWDRIVER@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 194f, conString: "@WASD@@SELECT@ADJUST @CANCEL@BACK");
                this._flagMenu = new UIFlagSelection(this._optionsMenu, "FLAG", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f);
                this._optionsMenu.Add(new UIMenuItemSlider("SFX Volume", field: new FieldBinding(Options.Data, "sfxVolume"), step: 0.06666667f), true);
                this._optionsMenu.Add(new UIMenuItemSlider("Music Volume", field: new FieldBinding(Options.Data, "musicVolume"), step: 0.06666667f), true);
                this._graphicsMenu = Options.CreateGraphicsMenu(this._optionsMenu);
                this._audioMenu = Options.CreateAudioMenu(this._optionsMenu);
                this._accessibilityMenu = Options.CreateAccessibilityMenu(this._optionsMenu);
                this._ttsMenu = Options.tempTTSMenu;
                this._blockMenu = Options.tempBlockMenu;
                this._optionsMenu.Add(new UIMenuItemSlider("Rumble Intensity", field: new FieldBinding(Options.Data, "rumbleIntensity"), step: 0.06666667f), true);
                this._optionsMenu.Add(new UIText(" ", Color.White), true);
                this._optionsMenu.Add(new UIMenuItemToggle("SHENANIGANS", field: new FieldBinding(Options.Data, "shennanigans")), true);
                this._optionsMenu.Add(new UIText(" ", Color.White), true);
                this._optionsMenu.Add(new UIMenuItem("EDIT CONTROLS", new UIMenuActionOpenMenuCallFunction(_optionsMenu, _controlConfigMenu, new UIMenuActionOpenMenuCallFunction.Function(UIControlConfig.ResetWarning)), backButton: true), true);
                this._optionsMenu.Add(new UIMenuItem("GRAPHICS", new UIMenuActionOpenMenu(_optionsMenu, _graphicsMenu), backButton: true), true);
                this._optionsMenu.Add(new UIMenuItem("AUDIO", new UIMenuActionOpenMenu(_optionsMenu, _audioMenu), backButton: true), true);
                this._cloudConfigMenu = new UIMenu("@WRENCH@SAVE DATA@SCREWDRIVER@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 280f, conString: "@CANCEL@BACK @SELECT@SELECT");
                this._cloudDeleteConfirmMenu = new UIMenu("CLEAR SAVE DATA?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 280f, conString: "@SELECT@SELECT");
                this._cloudManagerMenu = new UICloudManagement(this._cloudConfigMenu);
                this._cloudConfigMenu.Add(new UIMenuItemToggle("Enable Steam Cloud", field: new FieldBinding(Options.Data, "cloud")), true);
                this._cloudConfigMenu.Add(new UIMenuItem("Manage Save Data", new UIMenuActionOpenMenu(_cloudConfigMenu, _cloudManagerMenu)), true);
                this._cloudConfigMenu.Add(new UIMenuItem("|DGRED|CLEAR ALL SAVE DATA", new UIMenuActionOpenMenu(_cloudConfigMenu, _cloudDeleteConfirmMenu), backButton: true), true);
                this._cloudDeleteConfirmMenu.Add(new UIText("This will DELETE all data", Colors.DGRed), true);
                this._cloudDeleteConfirmMenu.Add(new UIText("(Profiles, Options, Levels)", Colors.DGRed), true);
                this._cloudDeleteConfirmMenu.Add(new UIText("from your Duck Game save!", Colors.DGRed), true);
                this._cloudDeleteConfirmMenu.Add(new UIText("", Colors.DGRed), true);
                this._cloudDeleteConfirmMenu.Add(new UIText("Do not do this, unless you're", Colors.DGRed), true);
                this._cloudDeleteConfirmMenu.Add(new UIText("absolutely sure!", Colors.DGRed), true);
                this._cloudDeleteConfirmMenu.Add(new UIText(" ", Colors.DGRed), true);
                this._cloudDeleteConfirmMenu.Add(new UIMenuItem("|DGRED|DELETE AND RESTART.", new UIMenuActionOpenMenuCallFunction(_cloudDeleteConfirmMenu, _cloudConfigMenu, new UIMenuActionOpenMenuCallFunction.Function(this.CloudDelete))), true);
                this._cloudDeleteConfirmMenu.Add(new UIMenuItem("|DGGREEN|CANCEL!", new UIMenuActionOpenMenu(_cloudDeleteConfirmMenu, _cloudConfigMenu)), true);
                this._cloudDeleteConfirmMenu._defaultSelection = 1;
                this._cloudDeleteConfirmMenu.SetBackFunction(new UIMenuActionOpenMenu(_cloudDeleteConfirmMenu, _cloudConfigMenu));
                this._cloudDeleteConfirmMenu.Close();
                this._cloudConfigMenu.Add(new UIText(" ", Colors.DGBlue), true);
                this._cloudConfigMenu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(_cloudConfigMenu, _optionsMenu), backButton: true), true);
                this._optionsMenu.Add(new UIText(" ", Color.White), true);
                if (MonoMain.moddingEnabled)
                {
                    this._modConfigMenu = new UIModManagement(this._optionsMenu, "@WRENCH@MANAGE MODS@SCREWDRIVER@", Layer.HUD.camera.width, Layer.HUD.camera.height, 550f, conString: "@WASD@@SELECT@ADJUST @MENU1@TOGGLE @CANCEL@BACK");
                    this._optionsMenu.Add(new UIMenuItem("MANAGE MODS", new UIMenuActionOpenMenu(_optionsMenu, _modConfigMenu), backButton: true), true);
                }
                this._optionsMenu.Add(new UIMenuItem("SELECT FLAG", new UIMenuActionOpenMenu(_optionsMenu, _flagMenu), backButton: true), true);
                this._optionsMenu.Add(new UIText(" ", Color.White), true);
                if (this._accessibilityMenu != null)
                    this._optionsMenu.Add(new UIMenuItem("USABILITY", new UIMenuActionOpenMenu(_optionsMenu, _accessibilityMenu), backButton: true), true);
                this._optionsMenu.SetBackFunction(new UIMenuActionCloseMenuCallFunction(_optionsMenu, new UIMenuActionCloseMenuCallFunction.Function(this.OptionsSaveAndClose)));
                this._optionsMenu.Close();
                this._optionsGroup.Add(_optionsMenu, false);
                this._controlConfigMenu.Close();
                this._flagMenu.Close();
                if (MonoMain.moddingEnabled)
                    this._modConfigMenu.Close();
                this._cloudConfigMenu.Close();
                this._cloudManagerMenu.Close();
                this._optionsGroup.Add(_controlConfigMenu, false);
                this._optionsGroup.Add((_controlConfigMenu as UIControlConfig)._confirmMenu, false);
                this._optionsGroup.Add((_controlConfigMenu as UIControlConfig)._warningMenu, false);
                this._optionsGroup.Add(_flagMenu, false);
                this._optionsGroup.Add(_graphicsMenu, false);
                this._optionsGroup.Add(_audioMenu, false);
                if (this._accessibilityMenu != null)
                    this._optionsGroup.Add(_accessibilityMenu, false);
                if (this._ttsMenu != null)
                    this._optionsGroup.Add(_ttsMenu, false);
                if (this._blockMenu != null)
                    this._optionsGroup.Add(_blockMenu, false);
                if (MonoMain.moddingEnabled)
                {
                    this._optionsGroup.Add(_modConfigMenu, false);
                    this._optionsGroup.Add((_modConfigMenu as UIModManagement)._modSettingsMenu, false);
                    this._optionsGroup.Add((_modConfigMenu as UIModManagement)._editModMenu, false);
                    this._optionsGroup.Add((_modConfigMenu as UIModManagement)._yesNoMenu, false);
                }
                this._optionsGroup.Add(_cloudManagerMenu, false);
                this._optionsGroup.Add(_cloudManagerMenu._deleteMenu, false);
                this._optionsGroup.Add(_cloudConfigMenu, false);
                this._optionsGroup.Add(_cloudDeleteConfirmMenu, false);
                this._optionsGroup.Close();
                Level.Add(_optionsGroup);
                this._betaMenu = new UIMenu("@WRENCH@WELCOME TO BETA!@SCREWDRIVER@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 240f, conString: "@CANCEL@OK!");
                this._betaMenu.Add(new UIImage(new Sprite("message"), UIAlign.Center, 0.25f, 51f), true);
                this._betaMenu.Close();
                this._betaMenu._backButton = new UIMenuItem("BACK", new UIMenuActionCloseMenu(_betaMenu), backButton: true);
                this._betaMenu._isMenu = true;
                Level.Add(_betaMenu);
                this._pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0.0f, 0.0f)
                {
                    isPauseMenu = true
                };
                this._mainPauseMenu = new UIMenu("@LWING@DUCK GAME@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@CLOSE @SELECT@SELECT");
                this._quitMenu = new UIMenu("REALLY QUIT?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK @SELECT@SELECT");
                this._quitMenu.Add(new UIMenuItem("NO!", new UIMenuActionOpenMenu(_quitMenu, _mainPauseMenu)), true);
                this._quitMenu.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuSetBoolean(this._pauseGroup, this._quit)), true);
                this._quitMenu.Close();
                this._pauseGroup.Add(_quitMenu, false);
                this._parentalControlsMenu = new UIMenu("PARENTAL CONTROLS", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, conString: "@CANCEL@CLOSE @SELECT@SELECT");
                this._parentalControlsMenu.Add(new UIText("Certain online features have been", Color.White), true);
                this._parentalControlsMenu.Add(new UIText("disabled by Parental Controls.", Color.White), true);
                this._parentalControlsMenu.Add(new UIText("", Color.White), true);
                this._parentalControlsMenu.Add(new UIMenuItem("OK", new UIMenuActionCloseMenu(this._pauseGroup)), true);
                this._parentalControlsMenu.Close();
                this._pauseGroup.Add(_parentalControlsMenu, false);
                int pMinLength = 50;
                float num1 = 3f;
                this._duckGameUpdateMenu = new UIMenu("DUCK GAME 1.5!", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 220f, conString: "@SELECT@OK!");
                UIMenu duckGameUpdateMenu1 = this._duckGameUpdateMenu;
                UIText component1 = new UIText("", Color.White, heightAdd: -3f)
                {
                    scale = new Vec2(0.5f)
                };
                duckGameUpdateMenu1.Add(component1, true);
                UIMenu duckGameUpdateMenu2 = this._duckGameUpdateMenu;
                UIText component2 = new UIText("Duck Game has received a major update!", Color.White, heightAdd: -4f)
                {
                    scale = new Vec2(0.5f)
                };
                duckGameUpdateMenu2.Add(component2, true);
                UIMenu duckGameUpdateMenu3 = this._duckGameUpdateMenu;
                UIText component3 = new UIText("Some of the biggest changes include:", Color.White, heightAdd: -4f)
                {
                    scale = new Vec2(0.5f)
                };
                duckGameUpdateMenu3.Add(component3, true);
                UIMenu duckGameUpdateMenu4 = this._duckGameUpdateMenu;
                UIText component4 = new UIText("", Color.White, heightAdd: -3f)
                {
                    scale = new Vec2(0.5f)
                };
                duckGameUpdateMenu4.Add(component4, true);
                UIMenu duckGameUpdateMenu5 = this._duckGameUpdateMenu;
                UIText component5 = new UIText("-Support for up to 8 players and 4 spectators".Padded(pMinLength), Color.White, heightAdd: (-num1))
                {
                    scale = new Vec2(0.5f)
                };
                duckGameUpdateMenu5.Add(component5, true);
                UIMenu duckGameUpdateMenu6 = this._duckGameUpdateMenu;
                UIText component6 = new UIText("-New hats, weapons, equipment and furniture".Padded(pMinLength), Color.White, heightAdd: (-num1))
                {
                    scale = new Vec2(0.5f)
                };
                duckGameUpdateMenu6.Add(component6, true);
                UIMenu duckGameUpdateMenu7 = this._duckGameUpdateMenu;
                UIText component7 = new UIText("-New city themed levels".Padded(pMinLength), Color.White, heightAdd: (-num1))
                {
                    scale = new Vec2(0.5f)
                };
                duckGameUpdateMenu7.Add(component7, true);
                UIMenu duckGameUpdateMenu8 = this._duckGameUpdateMenu;
                UIText component8 = new UIText("", Color.White, heightAdd: -3f)
                {
                    scale = new Vec2(0.5f)
                };
                duckGameUpdateMenu8.Add(component8, true);
                UIMenu duckGameUpdateMenu9 = this._duckGameUpdateMenu;
                UIText component9 = new UIText("-Custom font support for chat".Padded(pMinLength), Color.White, heightAdd: (-num1))
                {
                    scale = new Vec2(0.5f)
                };
                duckGameUpdateMenu9.Add(component9, true);
                UIMenu duckGameUpdateMenu10 = this._duckGameUpdateMenu;
                UIText component10 = new UIText("-4K and custom resolution support".Padded(pMinLength), Color.White, heightAdd: (-num1))
                {
                    scale = new Vec2(0.5f)
                };
                duckGameUpdateMenu10.Add(component10, true);
                UIMenu duckGameUpdateMenu11 = this._duckGameUpdateMenu;
                UIText component11 = new UIText("-Host Migration, Invite Links, LAN play".Padded(pMinLength), Color.White, heightAdd: (-num1))
                {
                    scale = new Vec2(0.5f)
                };
                duckGameUpdateMenu11.Add(component11, true);
                UIMenu duckGameUpdateMenu12 = this._duckGameUpdateMenu;
                UIText component12 = new UIText("-Major online synchronization improvements".Padded(pMinLength), Color.White, heightAdd: (-num1))
                {
                    scale = new Vec2(0.5f)
                };
                duckGameUpdateMenu12.Add(component12, true);
                UIMenu duckGameUpdateMenu13 = this._duckGameUpdateMenu;
                UIText component13 = new UIText("-Major performance improvements".Padded(pMinLength), Color.White, heightAdd: (-num1))
                {
                    scale = new Vec2(0.5f)
                };
                duckGameUpdateMenu13.Add(component13, true);
                UIMenu duckGameUpdateMenu14 = this._duckGameUpdateMenu;
                UIText component14 = new UIText("-Hundreds and hundreds of bug fixes".Padded(pMinLength), Color.White, heightAdd: (-num1))
                {
                    scale = new Vec2(0.5f)
                };
                duckGameUpdateMenu14.Add(component14, true);
                UIMenu duckGameUpdateMenu15 = this._duckGameUpdateMenu;
                UIText component15 = new UIText("", Color.White, heightAdd: -3f)
                {
                    scale = new Vec2(0.5f)
                };
                duckGameUpdateMenu15.Add(component15, true);
                UIMenu duckGameUpdateMenu16 = this._duckGameUpdateMenu;
                UIText component16 = new UIText("Thank you for all your support!", Color.White, heightAdd: -4f)
                {
                    scale = new Vec2(0.5f)
                };
                duckGameUpdateMenu16.Add(component16, true);
                UIMenu duckGameUpdateMenu17 = this._duckGameUpdateMenu;
                UIText component17 = new UIText("", Color.White, heightAdd: -3f)
                {
                    scale = new Vec2(0.5f)
                };
                duckGameUpdateMenu17.Add(component17, true);
                this._duckGameUpdateMenu.SetAcceptFunction(new UIMenuActionCloseMenu(this._pauseGroup));
                this._duckGameUpdateMenu.SetBackFunction(new UIMenuActionCloseMenu(this._pauseGroup));
                this._duckGameUpdateMenu.Close();
                this._pauseGroup.Add(_duckGameUpdateMenu, false);
                this._steamWarningMessage = new UIMenu("Steam Not Connected!", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 220f, conString: "@SELECT@ I see...");
                UIMenu steamWarningMessage1 = this._steamWarningMessage;
                UIText component18 = new UIText("", Color.White, heightAdd: -3f)
                {
                    scale = new Vec2(0.5f)
                };
                steamWarningMessage1.Add(component18, true);
                UIMenu steamWarningMessage2 = this._steamWarningMessage;
                UIText component19 = new UIText("It seems that either you're not logged in", Color.White, heightAdd: -4f)
                {
                    scale = new Vec2(0.5f)
                };
                steamWarningMessage2.Add(component19, true);
                UIMenu steamWarningMessage3 = this._steamWarningMessage;
                UIText component20 = new UIText("to Steam, or Steam failed to authenticate.", Color.White, heightAdd: -4f)
                {
                    scale = new Vec2(0.5f)
                };
                steamWarningMessage3.Add(component20, true);
                UIMenu steamWarningMessage4 = this._steamWarningMessage;
                UIText component21 = new UIText("", Color.White, heightAdd: -3f)
                {
                    scale = new Vec2(0.5f)
                };
                steamWarningMessage4.Add(component21, true);
                UIMenu steamWarningMessage5 = this._steamWarningMessage;
                UIText component22 = new UIText("You can still play- but realtime", Color.White, heightAdd: (-num1))
                {
                    scale = new Vec2(0.5f)
                };
                steamWarningMessage5.Add(component22, true);
                UIMenu steamWarningMessage6 = this._steamWarningMessage;
                UIText component23 = new UIText("features like Online Play and the Workshop", Color.White, heightAdd: (-num1))
                {
                    scale = new Vec2(0.5f)
                };
                steamWarningMessage6.Add(component23, true);
                UIMenu steamWarningMessage7 = this._steamWarningMessage;
                UIText component24 = new UIText("will be |DGRED|unavailable|PREV|.", Color.White, heightAdd: (-num1))
                {
                    scale = new Vec2(0.5f)
                };
                steamWarningMessage7.Add(component24, true);
                UIMenu steamWarningMessage8 = this._steamWarningMessage;
                UIText component25 = new UIText("", Color.White, heightAdd: -3f)
                {
                    scale = new Vec2(0.5f)
                };
                steamWarningMessage8.Add(component25, true);
                this._steamWarningMessage.SetAcceptFunction(new UIMenuActionCloseMenu(this._pauseGroup));
                this._steamWarningMessage.SetBackFunction(new UIMenuActionCloseMenu(this._pauseGroup));
                this._steamWarningMessage.Close();
                this._pauseGroup.Add(_steamWarningMessage, false);
                this._modsDisabledMenu = new UIMenu("MODS CHANGED!", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 240f, conString: "@SELECT@I see...");
                UIMenu modsDisabledMenu1 = this._modsDisabledMenu;
                UIText component26 = new UIText("", Color.White, heightAdd: -3f)
                {
                    scale = new Vec2(0.5f)
                };
                modsDisabledMenu1.Add(component26, true);
                UIMenu modsDisabledMenu2 = this._modsDisabledMenu;
                UIText component27 = new UIText("To ensure a smooth update, all enabled", Color.White, heightAdd: -4f)
                {
                    scale = new Vec2(0.5f)
                };
                modsDisabledMenu2.Add(component27, true);
                UIMenu modsDisabledMenu3 = this._modsDisabledMenu;
                UIText component28 = new UIText("mods have been temporarily set to |DGRED|disabled|PREV|.", Color.White, heightAdd: -4f)
                {
                    scale = new Vec2(0.5f)
                };
                modsDisabledMenu3.Add(component28, true);
                UIMenu modsDisabledMenu4 = this._modsDisabledMenu;
                UIText component29 = new UIText("", Color.White, heightAdd: -3f)
                {
                    scale = new Vec2(0.5f)
                };
                modsDisabledMenu4.Add(component29, true);
                UIMenu modsDisabledMenu5 = this._modsDisabledMenu;
                UIText component30 = new UIText("Mod compatibility has been a high priority, and", Color.White, heightAdd: (-num1))
                {
                    scale = new Vec2(0.5f)
                };
                modsDisabledMenu5.Add(component30, true);
                UIMenu modsDisabledMenu6 = this._modsDisabledMenu;
                UIText component31 = new UIText("most mods should work no problem with the new version.", Color.White, heightAdd: (-num1))
                {
                    scale = new Vec2(0.5f)
                };
                modsDisabledMenu6.Add(component31, true);
                UIMenu modsDisabledMenu7 = this._modsDisabledMenu;
                UIText component32 = new UIText("They can be re-enabled through the |DGORANGE|MANAGE MODS|PREV| menu", Color.White, heightAdd: (-num1))
                {
                    scale = new Vec2(0.5f)
                };
                modsDisabledMenu7.Add(component32, true);
                UIMenu modsDisabledMenu8 = this._modsDisabledMenu;
                UIText component33 = new UIText("accessible via the top left options console.", Color.White, heightAdd: (-num1))
                {
                    scale = new Vec2(0.5f)
                };
                modsDisabledMenu8.Add(component33, true);
                UIMenu modsDisabledMenu9 = this._modsDisabledMenu;
                UIText component34 = new UIText("", Color.White, heightAdd: -3f)
                {
                    scale = new Vec2(0.5f)
                };
                modsDisabledMenu9.Add(component34, true);
                UIMenu modsDisabledMenu10 = this._modsDisabledMenu;
                UIText component35 = new UIText("Some older mods may |DGRED|not|PREV| work...", Color.White, heightAdd: (-num1))
                {
                    scale = new Vec2(0.5f)
                };
                modsDisabledMenu10.Add(component35, true);
                UIMenu modsDisabledMenu11 = this._modsDisabledMenu;
                UIText component36 = new UIText("", Color.White, heightAdd: -3f)
                {
                    scale = new Vec2(0.5f)
                };
                modsDisabledMenu11.Add(component36, true);
                UIMenu modsDisabledMenu12 = this._modsDisabledMenu;
                UIText component37 = new UIText("Please be mindful of any crashes caused by", Color.White, heightAdd: (-num1))
                {
                    scale = new Vec2(0.5f)
                };
                modsDisabledMenu12.Add(component37, true);
                UIMenu modsDisabledMenu13 = this._modsDisabledMenu;
                UIText component38 = new UIText("re-enabling specific mods, and use the '|DGBLUE|-nomods|PREV|'", Color.White, heightAdd: (-num1))
                {
                    scale = new Vec2(0.5f)
                };
                modsDisabledMenu13.Add(component38, true);
                UIMenu modsDisabledMenu14 = this._modsDisabledMenu;
                UIText component39 = new UIText("launch option if you run into trouble!", Color.White, heightAdd: (-num1))
                {
                    scale = new Vec2(0.5f)
                };
                modsDisabledMenu14.Add(component39, true);
                UIMenu modsDisabledMenu15 = this._modsDisabledMenu;
                UIText component40 = new UIText("", Color.White, heightAdd: -3f)
                {
                    scale = new Vec2(0.5f)
                };
                modsDisabledMenu15.Add(component40, true);
                this._modsDisabledMenu.SetAcceptFunction(new UIMenuActionCloseMenu(this._pauseGroup));
                this._modsDisabledMenu.SetBackFunction(new UIMenuActionCloseMenu(this._pauseGroup));
                this._modsDisabledMenu.Close();
                this._pauseGroup.Add(_modsDisabledMenu, false);
                UIDivider component41 = new UIDivider(true, 0.8f);
                component41.rightSection.Add(new UIImage("pauseIcons", UIAlign.Right), true);
                this._mainPauseMenu.Add(component41, true);
                component41.leftSection.Add(new UIMenuItem("RESUME", new UIMenuActionCloseMenu(this._pauseGroup)), true);
                component41.leftSection.Add(new UIMenuItem("OPTIONS", new UIMenuActionOpenMenu(_mainPauseMenu, Options.optionsMenu), UIAlign.Left), true);
                component41.leftSection.Add(new UIMenuItem("CREDITS", new UIMenuActionCloseMenuSetBoolean(this._pauseGroup, this._enterCreditsMenuBool), UIAlign.Left), true);
                component41.leftSection.Add(new UIText("", Color.White), true);
                component41.leftSection.Add(new UIMenuItem("|DGRED|QUIT", new UIMenuActionOpenMenu(_mainPauseMenu, _quitMenu)), true);
                Options.openOnClose = this._mainPauseMenu;
                Options.AddMenus(this._pauseGroup);
                this._mainPauseMenu.Close();
                this._pauseGroup.Add(_mainPauseMenu, false);
                this._pauseGroup.Close();
                Level.Add(_pauseGroup);
                this._font = new BitmapFont("biosFont", 8);
                this._background = new Sprite("title/background");
                //this._optionsPlatform = new Sprite("title/optionsPlatform")
                //{
                //    depth = (Depth)0.9f
                //};
                this._rightPlatform = new Sprite("title/rightPlatform")
                {
                    depth = (Depth)0.9f
                };
                this._beamPlatform = new Sprite("title/beamPlatform")
                {
                    depth = (Depth)0.9f
                };
                this._upperMonitor = new Sprite("title/upperMonitor")
                {
                    depth = (Depth)0.85f
                };
                this._airlock = new Sprite("title/airlock")
                {
                    depth = -0.85f
                };
                this._leftPlatform = new Sprite("title/leftPlatform")
                {
                    depth = (Depth)0.9f
                };
                this._optionsTV = new Sprite("title/optionsTV")
                {
                    depth = -0.9f
                };
                this._libraryBookcase = new Sprite("title/libraryBookcase")
                {
                    depth = -0.9f
                };
                this._editorBench = new Sprite("title/editorBench")
                {
                    depth = -0.9f
                };
                this._editorBenchPaint = new Sprite("title/editorBenchPaint")
                {
                    depth = (Depth)0.9f
                };
                this._bigUButton = new Sprite("title/bigUButtonPC");
                this._bigUButton.CenterOrigin();
                this._bigUButton.depth = (Depth)0.95f;
                this._controls = new SpriteMap("title/controlsPC", 100, 11);
                this._controls.CenterOrigin();
                this._controls.depth = (Depth)0.95f;
                this._multiBeam = new MultiBeam(160f, -30f);
                Level.Add(_multiBeam);
                this._optionsBeam = new OptionsBeam(28f, -110f);
                Level.Add(_optionsBeam);
                this._libraryBeam = new LibraryBeam(292f, -110f);
                Level.Add(_libraryBeam);
                this._editorBeam = new EditorBeam(28f, 100f);
                Level.Add(_editorBeam);
                for (int index = 0; index < 21; ++index)
                {
                    SpaceTileset t = new SpaceTileset(index * 16 - 6, 176f)
                    {
                        frame = 3,
                        layer = Layer.Game,
                        setLayer = false
                    };
                    this.AddThing(t);
                }
                SpriteMap spriteMap = new SpriteMap("duck", 32, 32);
                this._space = new SpaceBackgroundMenu(-999f, -999f, true, 0.6f)
                {
                    update = false
                };
                Level.Add(_space);
                this._things.RefreshState();
                Layer.Game.fade = 0.0f;
                Layer.Foreground.fade = 0.0f;
                Level.Add(new Block(120f, 155f, 80f, 30f, PhysicsMaterial.Metal));
                Level.Add(new Block(134f, 148f, 52f, 30f, PhysicsMaterial.Metal));
                Level.Add(new Block(0.0f, 61f, 63f, 70f, PhysicsMaterial.Metal));
                Level.Add(new Block(257f, 61f, 63f, 60f, PhysicsMaterial.Metal));
                Level.Add(new Spring(90f, 160f, 0.32f));
                Level.Add(new Spring(229f, 160f, 0.32f));
                Level.Add(new VerticalDoor(270f, 160f)
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
                if (this._arcadeProfile == null)
                {
                    TeamSelect2.ControllerLayoutsChanged();
                    Teams.Player1.Join(Profiles.DefaultPlayer1);
                    Teams.Player2.Join(Profiles.DefaultPlayer2);
                    Teams.Player3.Join(Profiles.DefaultPlayer3);
                    Teams.Player4.Join(Profiles.DefaultPlayer4);
                }
                else
                {
                    Teams.Player1.Join(this._arcadeProfile);
                    this._arcadeProfile.inputProfile = this._arcadeInputProfile;
                }
                Input.lastActiveProfile = InputProfile.DefaultPlayer1;
                if (!DuckNetwork.ShowUserXPGain() && Unlockables.HasPendingUnlocks())
                    MonoMain.pauseMenu = new UIUnlockBox(Unlockables.GetPendingUnlocks().ToList<Unlockable>(), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f);
                base.Initialize();
            }
        }

        private void Join()
        {
        }

        private void OptionsSaveAndClose()
        {
            Options.Save();
            Options.SaveLocalData();
            this._optionsGroup.Close();
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
            if (this._duck != null && DGSave.showOnePointFiveMessages)
            {
                if (!this.showedNewVersionStartup && !DuckFile.freshInstall && DGSave.upgradingFromVanilla)
                {
                    MonoMain.pauseMenu = _duckGameUpdateMenu;
                    this._duckGameUpdateMenu.Open();
                    this.showedNewVersionStartup = true;
                    return;
                }
                if (!this.showedModsDisabled && DGSave.showModsDisabledMessage)
                {
                    MonoMain.pauseMenu = _modsDisabledMenu;
                    this._modsDisabledMenu.Open();
                    this.showedModsDisabled = true;
                    return;
                }
                DGSave.showOnePointFiveMessages = false;
            }
            int num = 1;
            if (this.startStars)
            {
                num = 250;
                this.startStars = false;
            }
            if (this._duck != null && this._duck.profile != null && !this._duck.profile.inputProfile.HasAnyConnectedDevice())
            {
                foreach (InputProfile defaultProfile in InputProfile.defaultProfiles)
                {
                    if (defaultProfile.HasAnyConnectedDevice())
                    {
                        InputProfile.SwapDefaultInputStrings(defaultProfile.name, this._duck.profile.inputProfile.name);
                        InputProfile.ReassignDefaultInputProfiles();
                        this._duck.profile.inputProfile = defaultProfile;
                        break;
                    }
                }
            }
            if (!this._enterCredits && !this._enterMultiplayer && this._duck != null && this._duck.inputProfile.Pressed("START"))
            {
                this._pauseGroup.Open();
                this._mainPauseMenu.Open();
                SFX.Play("pause", 0.6f);
                MonoMain.pauseMenu = this._pauseGroup;
            }
            for (int index = 0; index < num; ++index)
            {
                this.starWait -= Maths.IncFrameTimer();
                if (starWait < 0.0)
                {
                    this.starWait = 0.1f + Rando.Float(0.2f);
                    Color color = Colors.DGRed;
                    if (this.cpick == 1)
                        color = Colors.DGBlue;
                    else if (this.cpick == 2)
                        color = Colors.DGGreen;
                    if ((double)Rando.Float(1f) > 0.995000004768372)
                        color = Colors.DGPink;
                    this.particles.Add(new StarParticle()
                    {
                        pos = new Vec2(0.0f, (int)((double)Rando.Float(0.0f, 150f) / 1.0)),
                        speed = new Vec2(Rando.Float(0.5f, 1f), 0.0f),
                        color = color,
                        flicker = Rando.Float(100f, 230f)
                    });
                    ++this.cpick;
                    if (this.cpick > 2)
                        this.cpick = 0;
                }
                List<StarParticle> starParticleList = new List<StarParticle>();
                foreach (StarParticle particle in this.particles)
                {
                    particle.pos += particle.speed * (float)(0.5 + (1.0 - extraFade) * 0.5);
                    if (particle.pos.x > 300.0 && !this._enterCredits || particle.pos.x > 680.0)
                        starParticleList.Add(particle);
                }
                foreach (StarParticle starParticle in starParticleList)
                    this.particles.Remove(starParticle);
            }
            if (this.menuOpen)
            {
                Layer.Game.fade = Lerp.Float(Layer.Game.fade, 0.2f, 0.02f);
                Layer.Foreground.fade = Lerp.Float(Layer.Foreground.fade, 0.2f, 0.02f);
                Layer.Background.fade = Lerp.Float(Layer.Foreground.fade, 0.2f, 0.02f);
            }
            else
            {
                Layer.Game.fade = Lerp.Float(Layer.Game.fade, this._fadeInFull ? 1f : (this._fadeIn ? 0.5f : 0.0f), this._fadeInFull ? 0.01f : 3f / 500f);
                Layer.Foreground.fade = Lerp.Float(Layer.Foreground.fade, this._fadeIn ? 1f : 0.0f, 0.01f);
                Layer.Background.fade = Lerp.Float(Layer.Background.fade, this._fadeBackground ? 0.0f : 1f, 0.02f);
            }
            if (this._enterArcade)
            {
                ++this._duck.x;
                this._duck.immobilized = true;
                this._duck.enablePhysics = false;
                Graphics.fade = Lerp.Float(Graphics.fade, 0.0f, 0.05f);
                if ((double)Graphics.fade < 0.00999999977648258)
                {
                    Level.current.Clear();
                    Level.current = new ArcadeLevel(Content.GetLevelID("arcade"));
                }
            }
            else
            {
                if (this._enterCredits)
                {
                    this._duck.immobilized = true;
                    this._duck.updatePhysics = false;
                    if ((double)this.camera.x < 140.0)
                    {
                        this.flashDissipationSpeed = 0.08f;
                        Graphics.flashAdd = 2f;
                        this.camera.x += 330f;
                        foreach (StarParticle particle in this.particles)
                            particle.pos.x += 320f;
                    }
                    else
                    {
                        this.switchWait -= 0.04f;
                        if (switchWait <= 0.0)
                        {
                            if (!this._startedMusic)
                                Music.volumeMult = Lerp.Float(Music.volumeMult, 0.0f, 3f / 500f);
                            if ((double)Layer.Parallax.camera.y > -12.0)
                            {
                                this.camera.y += 0.064f;
                                Layer.Parallax.camera.y -= 0.08f;
                            }
                            else
                            {
                                if (!this._startedMusic)
                                {
                                    Music.volumeMult = 1.2f;
                                    Music.Play("tabledoodles", false);
                                    this._startedMusic = true;
                                }
                                if (creditsScroll > 939.0)
                                {
                                    if ((double)Layer.Parallax.camera.y > -22.0)
                                    {
                                        this.camera.y += 0.064f;
                                        Layer.Parallax.camera.y -= 0.08f;
                                    }
                                    this.extraFade -= 0.01f;
                                    if (extraFade < 0.0)
                                        this.extraFade = 0.0f;
                                }
                                if (creditsScroll > 2650.0 && !this.shownPrompt)
                                {
                                    this.shownPrompt = true;
                                    HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@Exit");
                                }
                                this.creditsScroll += 0.25f;
                            }
                        }
                    }
                    if (!this._duck.inputProfile.Pressed("CANCEL"))
                        return;
                    this._enterCredits = false;
                    this.quittingCredits = true;
                    return;
                }
                if (this._duck != null)
                {
                    this._duck.updatePhysics = true;
                    if ((double)this._duck.x > 324.0)
                        this._enterArcade = true;
                }
                if (this.quittingCredits)
                {
                    this.flashDissipationSpeed = 0.08f;
                    Graphics.flashAdd = 2f;
                    this.camera.x = 0.0f;
                    foreach (StarParticle particle in this.particles)
                        particle.pos.x -= 320f;
                    this.camera.y = 0.0f;
                    Layer.Parallax.camera.y = 0.0f;
                    this.creditsScroll = 0.0f;
                    this.extraFade = 1f;
                    this._startedMusic = false;
                    this.starWait = 0.0f;
                    this.switchWait = 1f;
                    this.creditsScroll = 0.0f;
                    this.startStars = true;
                    this.quittingCredits = false;
                    this.shownPrompt = false;
                    HUD.CloseAllCorners();
                    this._duck.immobilized = false;
                    Music.Play("Title");
                }
            }
            Music.volumeMult = 1f;
            TitleScreen._hasMenusOpen = this.menuOpen;
            if (!this._enterMultiplayer && !this._enterEditor && !this._enterLibrary) // && !this._enterBuyScreen
            {
                if ((double)Graphics.fade < 1.0)
                    Graphics.fade += 1f / 1000f;
                else
                    Graphics.fade = 1f;
            }
            else
            {
                Graphics.fade -= 0.05f;
                if ((double)Graphics.fade <= 0.0)
                {
                    Graphics.fade = 0.0f;
                    Music.Stop();
                    if (this._enterMultiplayer)
                    {
                        foreach (Team team in Teams.all)
                            team.ClearProfiles();
                        Level.current = new TeamSelect2();
                    }
                    else if (this._enterEditor)
                        Level.current = Main.editor;
                    else if (this._enterLibrary)
                        Level.current = new DoorRoom();
                    // else if (this._enterBuyScreen)
                    //  Level.current = new BuyScreen(Main.currencyType, Main.price);
                }
            }
            this._pressStartBlink += 0.01f;
            if (_pressStartBlink > 1.0)
                --this._pressStartBlink;
            if (this._duck != null)
            {
                if (this._dontQuit.value)
                {
                    this._dontQuit.value = false;
                    this._duck.hSpeed = 10f;
                }
                if (this._quit.value)
                {
                    MonoMain.exit = true;
                    return;
                }
                //if (InputProfile.active.Pressed("START") && Main.foundPurchaseInfo && Main.isDemo) free my people no buying here
                //{
                //    this._enterBuyScreen = true;
                //    this._duck.immobilized = true;
                //}
            }
            if (this._enterCreditsMenuBool.value)
            {
                this._enterCreditsMenuBool.value = false;
                this._enterCredits = true;
                this._duck.immobilized = true;
            }
            if (this._multiBeam.entered)
            {
                this._selectionTextDesired = "MULTIPLAYER";
                this._desiredSelection = TitleMenuSelection.Play;
                if (!this._enterMultiplayer && this._duck.inputProfile.Pressed("SELECT") && MonoMain.pauseMenu == null)
                {
                    SFX.Play("plasmaFire");
                    this._enterMultiplayer = true;
                    this._duck.immobilized = true;
                }
            }
            else if (this._optionsBeam.entered)
            {
                this._selectionTextDesired = "OPTIONS";
                this._desiredSelection = TitleMenuSelection.Options;
                if (!Options.menuOpen && this._duck.inputProfile.Pressed("SELECT"))
                {
                    SFX.Play("plasmaFire");
                    this._optionsGroup.Open();
                    this._optionsMenu.Open();
                    MonoMain.pauseMenu = this._optionsGroup;
                    this._duck.immobilized = true;
                }
            }
            else if (this._libraryBeam.entered)
            {
                this._selectionTextDesired = "LIBRARY";
                this._desiredSelection = TitleMenuSelection.Stats;
                if (this._duck.inputProfile.Pressed("SELECT") && Profiles.allCustomProfiles.Count > 0 && MonoMain.pauseMenu == null)
                {
                    SFX.Play("plasmaFire");
                    this._enterLibrary = true;
                    this._duck.immobilized = true;
                }
            }
            else if (this._editorBeam.entered)
            {
                this._selectionTextDesired = "LEVEL EDITOR";
                this._desiredSelection = TitleMenuSelection.Editor;
                if (this._duck.inputProfile.Pressed("SELECT") && MonoMain.pauseMenu == null)
                {
                    SFX.Play("plasmaFire");
                    this._enterEditor = true;
                    this._duck.immobilized = true;
                }
            }
            else
            {
                this._selectionTextDesired = " ";
                this._desiredSelection = TitleMenuSelection.None;
            }
            this._controlsFrameDesired = !(this._selectionTextDesired != " ") ? 2 : 1;
            if (this._selectionText != this._selectionTextDesired)
            {
                this._selectionFade -= 0.1f;
                if (_selectionFade <= 0.0)
                {
                    this._selectionFade = 0.0f;
                    this._selectionText = this._selectionTextDesired;
                    this._selection = this._desiredSelection;
                }
            }
            else
                this._selectionFade = Lerp.Float(this._selectionFade, 1f, 0.1f);
            if (this._controlsFrame != this._controlsFrameDesired)
            {
                this._controlsFade -= 0.1f;
                if (_controlsFade <= 0.0)
                {
                    this._controlsFade = 0.0f;
                    this._controlsFrame = this._controlsFrameDesired;
                }
            }
            else
                this._controlsFade = Lerp.Float(this._controlsFade, 1f, 0.1f);
            if (this._returnFromArcade)
            {
                if (!this._fadeIn)
                {
                    this._fadeIn = true;
                    this._title = new BigTitle();
                    this._title.x = (float)((double)Layer.HUD.camera.width / 2.0 - this._title.graphic.w / 2 + 3.0);
                    this._title.y = Layer.HUD.camera.height / 2f;
                    this.AddThing(_title);
                    this._title.fade = true;
                    this._title.alpha = 0.0f;
                    Layer.Game.fade = 1f;
                    Layer.Foreground.fade = 1f;
                    Layer.Background.fade = 1f;
                    this._arcadeProfile.inputProfile = this._arcadeInputProfile;
                    this._duck = new Duck(310f, 160f, this._arcadeProfile)
                    {
                        offDir = -1
                    };
                    InputProfile.active = this._duck.profile.inputProfile;
                }
                Graphics.fade = Lerp.Float(Graphics.fade, 1f, 0.05f);
                if ((double)Graphics.fade > 0.990000009536743)
                {
                    Graphics.fade = 1f;
                    this._returnFromArcade = false;
                }
            }
            if (this._fadeIn && !this._fadeInFull)
            {
                if (!this._returnFromArcade)
                    this._duck = null;
                if (TitleScreen.firstStart && ParentalControls.AreParentalControlsActive())
                {
                    MonoMain.pauseMenu = _parentalControlsMenu;
                    this._parentalControlsMenu.Open();
                    TitleScreen.firstStart = false;
                }
                foreach (Profile defaultProfile in Profiles.defaultProfiles)
                {
                    if (defaultProfile.inputProfile.JoinGamePressed())
                    {
                        this.Join();
                        foreach (Profile profile in Profiles.all)
                            profile.team = null;
                        defaultProfile.ApplyDefaults();
                        this._duck = new Duck(160f, 60f, defaultProfile);
                        if (SFX.NoSoundcard)
                            HUD.AddInputChangeDisplay("@UNPLUG@|RED|No Soundcard Detected!!");
                    }
                }
                if (this._duck != null)
                {
                    if (Main.foundPurchaseInfo && Main.isDemo)
                        HUD.AddCornerControl(HUDCorner.TopRight, "@START@BUY GAME", this._duck.inputProfile);
                    InputProfile.active = this._duck.profile.inputProfile;
                    this._fadeInFull = true;
                    this._title.fade = true;
                    Level.Add(_duck);
                }
            }
            this._space.parallax.y = -80f;
            this.moveWait -= 0.02f;
            if (moveWait < 0.0)
            {
                if (this._title == null)
                {
                    this._title = new BigTitle();
                    this._title.x = (float)((double)Layer.HUD.camera.width / 2.0 - this._title.graphic.w / 2 + 3.0);
                    this._title.y = Layer.HUD.camera.height / 2f;
                    this.AddThing(_title);
                }
                this.moveSpeed = Maths.LerpTowards(this.moveSpeed, 0.0f, 0.0015f);
            }
            if (this._title == null)
                return;
            ++this.wait;
            if (this.wait == 60)
                this.flash = 1f;
            if (this.wait == 60)
            {
                this._title.graphic.color = Color.White;
                this._title.alpha = 1f;
                this._fadeIn = true;
            }
            if (flash > 0.0)
            {
                this.flash -= 0.016f;
                this.dim -= 0.08f;
                if (dim >= 0.0)
                    return;
                this.dim = 0.0f;
            }
            else
                this.flash = 0.0f;
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer == Layer.Foreground)
            {
                Graphics.Draw(this._upperMonitor, 84f, 0.0f);
                if (this._fadeInFull)
                {
                    this._font.alpha = this._selectionFade;
                    this._font.inputProfile = this._duck.inputProfile;
                    if (this._selection == TitleMenuSelection.None)
                    {
                        string text = "@WASD@MOVE @JUMP@JUMP";
                        this._font.Draw(text, Level.current.camera.PercentW(50f) - this._font.GetWidth(text) / 2f, 16f, Color.White, (Depth)0.95f);
                    }
                    else if (this._selection == TitleMenuSelection.Play)
                    {
                        string text = "@SELECT@PLAY GAME";
                        this._font.Draw(text, Level.current.camera.PercentW(50f) - this._font.GetWidth(text) / 2f, 16f, Color.White, (Depth)0.95f);
                    }
                    else if (this._selection == TitleMenuSelection.Stats)
                    {
                        if (Profiles.allCustomProfiles.Count > 0)
                        {
                            string text = "@SELECT@LIBRARY";
                            this._font.Draw(text, Level.current.camera.PercentW(50f) - this._font.GetWidth(text) / 2f, 16f, Color.White, (Depth)0.95f);
                        }
                    }
                    else if (this._selection == TitleMenuSelection.Options)
                    {
                        string text = "@SELECT@OPTIONS";
                        this._font.Draw(text, Level.current.camera.PercentW(50f) - this._font.GetWidth(text) / 2f, 16f, Color.White, (Depth)0.95f);
                    }
                    else if (this._selection == TitleMenuSelection.Editor)
                    {
                        string text = "@SELECT@EDITOR";
                        this._font.Draw(text, Level.current.camera.PercentW(50f) - this._font.GetWidth(text) / 2f, 16f, Color.White, (Depth)0.95f);
                    }
                    Graphics.Draw(this._editorBenchPaint, 45f, 168f);
                }
                else
                {
                    string text = "PRESS START";
                    if (_pressStartBlink >= 0.5)
                    {
                        this._font.Draw(text, Level.current.camera.PercentW(50f) - this._font.GetWidth(text) / 2f, 15f, Color.White, (Depth)0.95f);
                    }
                    else
                    {
                        InputProfile profileWithDevice = InputProfile.FirstProfileWithDevice;
                        if (profileWithDevice != null && profileWithDevice.lastActiveDevice != null && profileWithDevice.lastActiveDevice is GenericController)
                            Graphics.Draw(this._bigUButton, Level.current.camera.PercentW(50f) - 1f, 18f);
                        else
                            Graphics.DrawString("@START@", new Vec2(Level.current.camera.PercentW(50f) - 7f, 16f), Color.White, (Depth)0.9f, profileWithDevice);
                    }
                }
            }
            else if (layer == Layer.Game)
            {
                Graphics.Draw(this._leftPlatform, 0.0f, 61f);
                Graphics.Draw(this._airlock, 266f, 135f);
                Graphics.Draw(this._rightPlatform, byte.MaxValue, 61f);
                Graphics.Draw(this._beamPlatform, 118f, 146f);
                Graphics.Draw(this._optionsTV, 0.0f, 19f);
                Graphics.Draw(this._libraryBookcase, 263f, 12f);
                Graphics.Draw(this._editorBench, 1f, 130f);
                if (creditsScroll > 0.100000001490116)
                {
                    Graphics.caseSensitiveStringDrawing = true;
                    float num1 = 0.0f;
                    foreach (List<string> stringList in this.creditsRoll)
                    {
                        float num2 = num1 + (200f - this.creditsScroll);
                        if ((double)num2 >= -11.0 && (double)num2 < 200.0)
                        {
                            if (stringList.Count == 1)
                            {
                                float stringWidth = Graphics.GetStringWidth(stringList[0]);
                                Graphics.DrawStringColoredSymbols(stringList[0], new Vec2((float)(490.0 - (double)stringWidth / 2.0), num1 + (200f - this.creditsScroll)), Color.White, (Depth)1f);
                            }
                            else
                            {
                                double stringWidth1 = (double)Graphics.GetStringWidth(stringList[0]);
                                Graphics.DrawStringColoredSymbols(stringList[0], new Vec2(347f, num1 + (200f - this.creditsScroll)), Color.White, (Depth)1f);
                                double stringWidth2 = (double)Graphics.GetStringWidth(stringList[1]);
                                Graphics.DrawStringColoredSymbols(stringList[1], new Vec2(507f, num1 + (200f - this.creditsScroll)), Color.White, (Depth)1f);
                            }
                        }
                        num1 += 11f;
                    }
                    Graphics.caseSensitiveStringDrawing = false;
                }
            }
            else if (layer == Layer.Parallax)
            {
                float num = 0.0f;
                if ((double)this.camera.y > 4.0)
                {
                    this._starField.alpha = num + (float)(((double)this.camera.y - 4.0) / 13.0) - this.extraFade * 0.7f;
                    Graphics.Draw(this._starField, 0.0f, layer.camera.y - 58f, -0.99f);
                }
            }
            else if (layer == Layer.Background)
            {
                foreach (StarParticle particle in this.particles)
                {
                    float num3 = Math.Max(1f - Math.Min(Math.Abs(particle.pos.x - particle.flicker) / 10f, 1f), 0.0f);
                    float num4 = 0.2f;
                    if ((double)this.camera.y > 0.0)
                        num4 += this.camera.y / 52f;
                    Graphics.DrawRect(particle.pos, particle.pos + new Vec2(1f, 1f), Color.White * (float)(((double)num4 + (double)num3 * 0.600000023841858) * (0.300000011920929 + (1.0 - extraFade) * 0.699999988079071)), -0.3f);
                    float num5 = 0.1f;
                    if ((double)this.camera.y > 0.0)
                        num5 += this.camera.y / 52f;
                    Vec2 pos = particle.pos;
                    int num6 = 4;
                    for (int index = 0; index < num6; ++index)
                    {
                        float num7 = particle.speed.x * 8f;
                        Graphics.DrawLine(pos + new Vec2(-num7, 0.5f), pos + new Vec2(0.0f, 0.5f), particle.color * ((float)(1.0 - index / (double)num6) * num5) * (float)(0.300000011920929 + (1.0 - extraFade) * 0.699999988079071), depth: (-0.4f));
                        pos.x -= num7;
                    }
                }
                this._background.depth = (Depth)0.0f;
                Rectangle sourceRectangle = new Rectangle(0.0f, 0.0f, 90f, _background.height);
                Graphics.Draw(this._background, 0.0f, 0.0f, sourceRectangle);
                sourceRectangle = new Rectangle(63f, 107f, 194f, 61f);
                Graphics.Draw(this._background, sourceRectangle.x, sourceRectangle.y, sourceRectangle);
                sourceRectangle = new Rectangle(230f, 61f, 28f, 61f);
                Graphics.Draw(this._background, sourceRectangle.x, sourceRectangle.y, sourceRectangle);
                sourceRectangle = new Rectangle(230f, 0.0f, 90f, 61f);
                Graphics.Draw(this._background, sourceRectangle.x, sourceRectangle.y, sourceRectangle);
                sourceRectangle = new Rectangle(230f, 124f, 90f, 56f);
                Graphics.Draw(this._background, sourceRectangle.x, sourceRectangle.y, sourceRectangle);
                sourceRectangle = new Rectangle(90f, 0.0f, 140f, 50f);
                Graphics.Draw(this._background, sourceRectangle.x, sourceRectangle.y, sourceRectangle);
            }
            base.PostDrawLayer(layer);
        }
    }
}
