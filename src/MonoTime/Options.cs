// Decompiled with JetBrains decompiler
// Type: DuckGame.Options
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public static class Options
    {
        private static UIMenu _optionsMenu;
        private static UIMenu _controllerWarning;
        private static OptionsData _data = new OptionsData();
        private static OptionsDataLocal _localData = new OptionsDataLocal();
        private static bool _removedOptionsMenu = false;
        private static bool _openedOptionsMenu = false;
        public static UIMenu openOnClose = null;
        private static UIMenu _graphicsMenu;
        private static UIMenu _audioMenu;
        private static UIMenu _accessibilityMenu;
        private static UIMenu _ttsMenu;
        private static UIMenu _blockMenu;
        private static UIMenu _controlsMenu;
        public static UIMenu _lastCreatedAudioMenu;
        public static UIMenu _lastCreatedGraphicsMenu;
        public static UIMenu _lastCreatedAccessibilityMenu;
        public static UIMenu _lastCreatedTTSMenu;
        public static UIMenu _lastCreatedBlockMenu;
        public static UIMenu _lastCreatedControlsMenu;
        public static int flagForSave = 0;
        private static bool _doingResolutionRestart = false;
        private static List<string> chatFonts = new List<string>()
    {
      "Duck Font",
      "Arial",
      "Calibri",
      "Courier New",
      "Comic Sans MS",
      "Custom"
    };
        public static UIMenu tempTTSMenu;
        public static UIMenu tempBlockMenu;
        private static bool loadCalled = false;
        public static int legacyPreferredColor = -1;
        //private static string _pendingTTS;
        private static bool _resolutionChanged = false;

        public static string GetMuteSettings(Profile pProfile)
        {
            string str;
            return Options.Data.muteSettings.TryGetValue(pProfile.steamID, out str) ? str : "";
        }

        public static void SetMuteSetting(Profile pProfile, string pSetting, bool pValue)
        {
            string str;
            if (!Options.Data.muteSettings.TryGetValue(pProfile.steamID, out str))
                str = Options.Data.muteSettings[pProfile.steamID] = "";
            if (pValue && !str.Contains(pSetting))
                str += pSetting;
            else if (!pValue)
                str = str.Replace(pSetting, "");
            Options.Data.muteSettings[pProfile.steamID] = str;
        }

        public static UIMenu optionsMenu => Options._optionsMenu;

        public static UIMenu controllerWarning => Options._controllerWarning;

        public static OptionsData Data
        {
            get => Options._data;
            set => Options._data = value;
        }

        public static OptionsDataLocal LocalData
        {
            get => Options._localData;
            set => Options._localData = value;
        }

        public static bool menuOpen => Options._optionsMenu.open;

        public static UIMenu graphicsMenu => Options._graphicsMenu;

        public static UIMenu audioMenu => Options._audioMenu;

        public static UIMenu accessibilityMenu => Options._accessibilityMenu;

        public static UIMenu ttsMenu => Options._ttsMenu;

        public static UIMenu blockMenu => Options._blockMenu;

        public static UIMenu controlsMenu => Options._controlsMenu;

        public static void AddMenus(UIComponent to)
        {
            to.Add(optionsMenu, false);
            to.Add(graphicsMenu, false);
            to.Add(audioMenu, false);
            if (Options.accessibilityMenu != null)
                to.Add(accessibilityMenu, false);
            if (Options.ttsMenu != null)
                to.Add(ttsMenu, false);
            if (Options.blockMenu != null)
                to.Add(blockMenu, false);
            if (Options.controlsMenu != null)
            {
                to.Add(controlsMenu, false);
                to.Add((controlsMenu as UIControlConfig)._confirmMenu, false);
                to.Add((controlsMenu as UIControlConfig)._warningMenu, false);
            }
            to.Add(controllerWarning, false);
        }

        public static void QuitShowingControllerWarning()
        {
            Options.Data.showControllerWarning = false;
            Options.Save();
        }

        public static UIMenu CreateOptionsMenu()
        {
            UIMenu optionsMenu = new UIMenu("@WRENCH@OPTIONS@SCREWDRIVER@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, conString: "@CANCEL@BACK @SELECT@SELECT");
            optionsMenu.Add(new UIMenuItemSlider("SFX Volume", field: new FieldBinding(Data, "sfxVolume"), step: 0.06666667f), true);
            optionsMenu.Add(new UIMenuItemSlider("Music Volume", field: new FieldBinding(Data, "musicVolume"), step: 0.06666667f), true);
            optionsMenu.Add(new UIMenuItemSlider("Rumble Intensity", field: new FieldBinding(Data, "rumbleIntensity"), step: 0.06666667f), true);
            optionsMenu.Add(new UIText(" ", Color.White), true);
            optionsMenu.Add(new UIMenuItemToggle("SHENANIGANS", field: new FieldBinding(Data, "shennanigans")), true);
            Options._lastCreatedGraphicsMenu = Options.CreateGraphicsMenu(optionsMenu);
            Options._lastCreatedAccessibilityMenu = Options.CreateAccessibilityMenu(optionsMenu);
            Options._lastCreatedTTSMenu = Options.tempTTSMenu;
            Options._lastCreatedBlockMenu = Options.tempBlockMenu;
            Options._lastCreatedAudioMenu = Options.CreateAudioMenu(optionsMenu);
            optionsMenu.Add(new UIText(" ", Color.White), true);
            optionsMenu.Add(new UIMenuItem("GRAPHICS", new UIMenuActionOpenMenu(optionsMenu, _lastCreatedGraphicsMenu), backButton: true), true);
            optionsMenu.Add(new UIMenuItem("AUDIO", new UIMenuActionOpenMenu(optionsMenu, _lastCreatedAudioMenu), backButton: true), true);
            optionsMenu.Add(new UIText(" ", Color.White), true);
            optionsMenu.Add(new UIMenuItem("USABILITY", new UIMenuActionOpenMenu(optionsMenu, _lastCreatedAccessibilityMenu), backButton: true), true);
            optionsMenu.SetBackFunction(new UIMenuActionCloseMenuCallFunction(optionsMenu, new UIMenuActionCloseMenuCallFunction.Function(Options.OptionsMenuClosed)));
            optionsMenu.Close();
            return optionsMenu;
        }

        public static void Initialize()
        {
            Options._optionsMenu = Options.CreateOptionsMenu();
            Options._controllerWarning = Options.CreateControllerWarning();
            Options._graphicsMenu = Options._lastCreatedGraphicsMenu;
            Options._accessibilityMenu = Options._lastCreatedAccessibilityMenu;
            Options._audioMenu = Options._lastCreatedAudioMenu;
            Options._ttsMenu = Options._lastCreatedTTSMenu;
            Options._blockMenu = Options._lastCreatedBlockMenu;
        }

        public static UIMenu CreateControllerWarning()
        {
            UIMenu menu = new UIMenu("Is that a PS4 Controller?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 220f, conString: "@CANCEL@BACK @SELECT@SELECT");
            UIMenu uiMenu1 = menu;
            UIText component1 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            uiMenu1.Add(component1, true);
            UIMenu uiMenu2 = menu;
            UIText component2 = new UIText("It seems you may have a |DGBLUE|PS4 Controller", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            uiMenu2.Add(component2, true);
            UIMenu uiMenu3 = menu;
            UIText component3 = new UIText("plugged in! If so, and if you are running", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            uiMenu3.Add(component3, true);
            UIMenu uiMenu4 = menu;
            UIText component4 = new UIText("|DGBLUE|DS4Windows|PREV| or a |DGBLUE|3rd party PS4 Controller Driver|PREV|,", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            uiMenu4.Add(component4, true);
            UIMenu uiMenu5 = menu;
            UIText component5 = new UIText("you may need to |DGRED|disable|PREV| it.", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            uiMenu5.Add(component5, true);
            UIMenu uiMenu6 = menu;
            UIText component6 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            uiMenu6.Add(component6, true);
            UIMenu uiMenu7 = menu;
            UIText component7 = new UIText("If everything works okay, you can ignore", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            uiMenu7.Add(component7, true);
            UIMenu uiMenu8 = menu;
            UIText component8 = new UIText("this message. If you're controlling 2 ducks", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            uiMenu8.Add(component8, true);
            UIMenu uiMenu9 = menu;
            UIText component9 = new UIText("at once, then this message is for you!", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            uiMenu9.Add(component9, true);
            UIMenu uiMenu10 = menu;
            UIText component10 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            uiMenu10.Add(component10, true);
            menu.Add(new UIMenuItem("|DGORANGE|OK THEN", new UIMenuActionCloseMenu(menu), c: Color.White), true);
            menu.Add(new UIMenuItem("|DGRED|DON'T SHOW THIS AGAIN", new UIMenuActionCloseMenuCallFunction(menu, new UIMenuActionCloseMenuCallFunction.Function(Options.QuitShowingControllerWarning)), c: Color.White), true);
            menu.SetBackFunction(new UIMenuActionCloseMenu(menu));
            menu.Close();
            return menu;
        }

        public static float GetWindowScaleMultiplier()
        {
            if (Options.Data.windowScale == 0)
                return 1f;
            return Options.Data.windowScale == 1 ? 1.5f : 2f;
        }

        public static void ScreenModeChanged(string pMode)
        {
            if (pMode == "Fullscreen")
                Resolution.Set(Options.Data.windowedFullscreen ? Options.LocalData.windowedFullscreenResolution : Options.LocalData.fullscreenResolution);
            else
                Resolution.Set(Options.LocalData.windowedResolution);
        }

        public static void WindowedFullscreenChanged()
        {
            if (Resolution.current.mode == ScreenMode.Windowed)
                return;
            Options.ScreenModeChanged("Fullscreen");
        }

        public static void FullscreenChanged() => Options.ScreenModeChanged(Options.Data.fullscreen ? "Fullscreen" : "Windowed");

        private static void ExclusiveAudioModeChanged() => SFX._audio.LoseDevice();

        private static void AudioEngineChanged()
        {
            MonoMain.audioModeOverride = (AudioMode)Options.Data.audioMode;
            Windows_Audio.ResetDevice();
        }

        private static void ApplyResolution()
        {
            if (!Options._doingResolutionRestart)
                Resolution.Set(Options.LocalData.currentResolution);
            if (Options.LocalData.currentResolution.mode == ScreenMode.Fullscreen)
                Options.LocalData.fullscreenResolution = Options.LocalData.currentResolution;
            else if (Options.LocalData.currentResolution.mode == ScreenMode.Borderless)
                Options.LocalData.windowedFullscreenResolution = Options.LocalData.currentResolution;
            else
                Options.LocalData.windowedResolution = Options.LocalData.currentResolution;
        }

        public static UIMenu CreateGraphicsMenu(UIMenu pOptionsMenu)
        {
            UIMenu menu = new UIMenu("@WRENCH@GRAPHICS@SCREWDRIVER@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 240f, conString: "@CANCEL@BACK @SELECT@SELECT");
            menu.Add(new UIMenuItemToggle("Fullscreen", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(Options.FullscreenChanged)), new FieldBinding(Data, "fullscreen")), true);
            menu.Add(new UIMenuItemResolution("Resolution", new FieldBinding(LocalData, "currentResolution", max: 0.0f))
            {
                selectAction = new Action(Options.ApplyResolution)
            }, true);
            menu.Add(new UIText(" ", Color.White), true);
            menu.Add(new UIMenuItemToggle("Windowed Fullscreen", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(Options.WindowedFullscreenChanged)), new FieldBinding(Data, "windowedFullscreen")), true);
            menu.Add(new UIText(" ", Color.White), true);
            menu.Add(new UIMenuItemToggle("Fire Glow", field: new FieldBinding(Data, "fireGlow")), true);
            menu.Add(new UIMenuItemToggle("Lighting", field: new FieldBinding(Data, "lighting")), true);
            menu.Add(new UIMenuItemToggle("Backfill Fix", field: new FieldBinding(Data, "fillBackground")), true);
            menu.Add(new UIMenuItemToggle("Explosion Flashes", field: new FieldBinding(Data, "flashing")), true);
            menu.Add(new UIText(" ", Color.White), true);
            menu.Add(new UIMenuItemNumber("Console Width", field: new FieldBinding(Data, "consoleWidth", 25f, 100f), step: 10), true);
            menu.Add(new UIMenuItemNumber("Console Height", field: new FieldBinding(Data, "consoleHeight", 10f, 100f), step: 10), true);
            menu.Add(new UIMenuItemNumber("Console Scale", field: new FieldBinding(Data, "consoleScale", max: 4f), valStrings: new List<string>()
      {
        "Tiny",
        "Regular",
        "Large",
        "Gigantic",
        "WUMBO"
      }), true);
            menu.Add(new UIText(" ", Color.White), true);
            menu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(menu, pOptionsMenu), backButton: true), true);
            return menu;
        }

        public static UIMenu CreateAudioMenu(UIMenu pOptionsMenu)
        {
            UIMenu menu = new UIMenu("@WRENCH@Audio@SCREWDRIVER@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 280f, conString: "@CANCEL@BACK @SELECT@SELECT");
            menu.Add(new UIText("Exclusive Mode can reduce", Colors.DGBlue), true);
            menu.Add(new UIText("audio latency, but will", Colors.DGBlue), true);
            menu.Add(new UIText("stop all other programs from", Colors.DGBlue), true);
            menu.Add(new UIText("making sound while Duck Game", Colors.DGBlue), true);
            menu.Add(new UIText("is running!", Colors.DGBlue), true);
            menu.Add(new UIText(" ", Colors.DGBlue), true);
            menu.Add(new UIMenuItemToggle("Exclusive Mode", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(Options.ExclusiveAudioModeChanged)), new FieldBinding(Data, "audioExclusiveMode")), true);
            menu.Add(new UIMenuItemNumber("Audio Engine", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(Options.AudioEngineChanged)), new FieldBinding(Data, "audioMode", 1f, 3f), valStrings: new List<string>()
      {
        "None",
        "WaveOut",
        "Wasapi",
        "DirectSound"
      }), true);
            menu.Add(new UIText(" ", Color.White), true);
            menu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(menu, pOptionsMenu), backButton: true), true);
            return menu;
        }

        public static int selectedFont
        {
            get
            {
                string chatFont = Options.Data.chatFont;
                int num = Options.chatFonts.IndexOf(chatFont);
                if (chatFont == "")
                    num = 0;
                return num == -1 ? Options.chatFonts.Count - 1 : num;
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value >= Options.chatFonts.Count - 1)
                    value = Options.chatFonts.Count - 2;
                Options.Data.chatFont = value != 0 ? Options.chatFonts[value] : "";
                DuckNetwork.UpdateFont();
            }
        }

        public static int fontSize
        {
            get => Options.Data.chatFontSize;
            set
            {
                if (value < 12)
                    value = 12;
                if (value >= 80)
                    value = 80;
                Options.Data.chatFontSize = value;
                DuckNetwork.UpdateFont();
            }
        }

        private static void CloseMoreMenu() => DuckNetwork.UpdateFont();

        public static int languageFilter
        {
            get => !Options.Data.languageFilter ? 0 : 1;
            set => Options.Data.languageFilter = value == 1;
        }

        public static int mojiFilter
        {
            get => Options.Data.mojiFilter;
            set => Options.Data.mojiFilter = value;
        }

        public static int hatFilter
        {
            get => Options.Data.hatFilter;
            set => Options.Data.hatFilter = value;
        }

        public static UIMenu CreateBlockMenu(UIMenu pAccessibilityMenu)
        {
            try
            {
                return new UIBlockManagement(pAccessibilityMenu);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static UIMenu CreateTTSMenu(UIMenu pAccessibilityMenu)
        {
            try
            {
                UIMenu menu = new UIMenu("TTS SETTINGS", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 280f, conString: "@SELECT@SELECT");
                menu.Add(new UIMenuItemToggle("Text To Speech", field: new FieldBinding(Data, "textToSpeech")), true);
                List<string> sayVoices = SFX.GetSayVoices();
                List<string> valStrings = new List<string>();
                foreach (string str1 in sayVoices)
                {
                    string str2 = str1.Replace("Microsoft ", "").Replace(" Desktop", "");
                    if (str2.Length > 10)
                        str2 = str2.Substring(0, 8) + "..";
                    valStrings.Add(str2);
                }
                menu.Add(new UIMenuItemNumber("TTS Voice", field: new FieldBinding(Data, "textToSpeechVoice", max: sayVoices.Count), valStrings: valStrings), true);
                menu.Add(new UIMenuItemSlider("TTS Volume", field: new FieldBinding(Data, "textToSpeechVolume"), step: 0.06666667f), true);
                menu.Add(new UIMenuItemSlider("TTS Speed", field: new FieldBinding(Data, "textToSpeechRate"), step: 0.04739336f), true);
                menu.Add(new UIMenuItemToggle("TTS Read Names", field: new FieldBinding(Data, "textToSpeechReadNames")), true);
                menu.Add(new UIText(" ", Color.White), true);
                menu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenuCallFunction(menu, pAccessibilityMenu, new UIMenuActionOpenMenuCallFunction.Function(Options.CloseMoreMenu))), true);
                menu.SetBackFunction(new UIMenuActionOpenMenuCallFunction(menu, pAccessibilityMenu, new UIMenuActionOpenMenuCallFunction.Function(Options.CloseMoreMenu)));
                return menu;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static UIMenu CreateAccessibilityMenu(UIMenu pOptionsMenu)
        {
            try
            {
                UIMenu accessibilityMenu = new UIMenu("USABILITY", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 280f, conString: "@SELECT@SELECT");
                accessibilityMenu.Add(new UIMenuItemToggle("IME Support", field: new FieldBinding(Data, "imeSupport")), true);
                accessibilityMenu.Add(new UIText(" ", Color.White), true);
                accessibilityMenu.Add(new UIText("Chat Settings", Color.White), true);
                accessibilityMenu.Add(new UIMenuItemNumber("Custom MOJIs", field: new FieldBinding(typeof(Options), "mojiFilter", 0.0f, 2f, 0.1f), valStrings: new List<string>()
        {
          "|DGGREENN|@languageFilterOn@DISABLED",
          "|DGYELLO|@languageFilterOn@FRIENDS ",
          "|DGREDDD| @languageFilterOff@ENABLED"
        }), true);
                accessibilityMenu.Add(new UIMenuItemNumber("Custom Hats", field: new FieldBinding(typeof(Options), "hatFilter", 0.0f, 2f, 0.1f), valStrings: new List<string>()
        {
          "|DGGREEN|   ENABLED",
          "|DGYELLO|   FRIENDS",
          "|DGRED|  DISABLED  "
        }), true);
                Options.tempBlockMenu = Options.CreateBlockMenu(accessibilityMenu);
                accessibilityMenu.Add(new UIMenuItem("Manage Block List", new UIMenuActionOpenMenu(accessibilityMenu, tempBlockMenu)), true);
                accessibilityMenu.Add(new UIText(" ", Color.White), true);
                accessibilityMenu.Add(new UIMenuItemNumber("Chat Font", field: new FieldBinding(typeof(Options), "selectedFont", 0.0f, 6f, 0.1f), valStrings: Options.chatFonts), true);
                accessibilityMenu.Add(new UIMenuItemNumber("Chat Font Size", field: new FieldBinding(typeof(Options), "fontSize", 12f, 30f, 0.1f)), true);
                accessibilityMenu.Add(new UIMenuItemNumber("Chat Head Size", field: new FieldBinding(Data, "chatHeadScale"), valStrings: new List<string>()
        {
          "Regular",
          "Large"
        }), true);
                accessibilityMenu.Add(new UIMenuItemNumber("Chat Opacity", field: new FieldBinding(Data, "chatOpacity", 20f, 100f), step: 10), true);
                if (SFX.hasTTS)
                {
                    Options.tempTTSMenu = Options.CreateTTSMenu(accessibilityMenu);
                    accessibilityMenu.Add(new UIText(" ", Color.White), true);
                    accessibilityMenu.Add(new UIMenuItem("Text To Speech", new UIMenuActionOpenMenu(accessibilityMenu, tempTTSMenu), backButton: true), true);
                }
                else
                {
                    accessibilityMenu.Add(new UIText(" ", Color.White), true);
                    accessibilityMenu.Add(new UIText("|DGRED|Text To Speech Not Installed...", Color.White), true);
                }
                accessibilityMenu.Add(new UIText(" ", Color.White), true);
                accessibilityMenu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenuCallFunction(accessibilityMenu, pOptionsMenu, new UIMenuActionOpenMenuCallFunction.Function(Options.CloseMoreMenu))), true);
                accessibilityMenu.SetBackFunction(new UIMenuActionOpenMenuCallFunction(accessibilityMenu, pOptionsMenu, new UIMenuActionOpenMenuCallFunction.Function(Options.CloseMoreMenu)));
                return accessibilityMenu;
            }
            catch (Exception ex)
            {
                DevConsole.LogComplexMessage("Error creating accessibility menu: " + ex.StackTrace.ToString(), Colors.DGRed);
                return null;
            }
        }

        public static void MergeDefaultPreferDefault() => Options.MergeDefault(true);

        public static void MergeDefaultPreferAccount() => Options.MergeDefault(false);

        public static void CancelResolutionChange() => Options.LocalData.currentResolution = Resolution.lastApplied;

        public static void RestartAndApplyResolution()
        {
            Options._doingResolutionRestart = true;
            Options.ApplyResolution();
            Options.Save();
            Options.SaveLocalData();
            ModLoader.RestartGame();
        }

        public static void MergeDefault(bool pPreferDefault, bool pShowDialog = true)
        {
            if (Profiles.experienceProfile != null)
            {
                if (MonoMain.logFileOperations)
                    DevConsole.Log(DCSection.General, "Options.MergeDefault()");
                Profile experienceProfile = Profiles.experienceProfile;
                Profile p = Profiles.all.ElementAt<Profile>(0);
                Profiles.Save(experienceProfile, "__backup_");
                Profiles.Save(p, "__backup_");
                experienceProfile.numSandwiches += p.numSandwiches;
                experienceProfile.milkFill += p.milkFill;
                experienceProfile.littleManLevel += p.littleManLevel - 1;
                experienceProfile.numLittleMen += p.numLittleMen;
                experienceProfile.littleManBucks += p.littleManBucks;
                experienceProfile.timesMetVincent += p.timesMetVincent;
                experienceProfile.timesMetVincentSale += p.timesMetVincentSale;
                experienceProfile.timesMetVincentSell += p.timesMetVincentSell;
                experienceProfile.timesMetVincentImport += p.timesMetVincentImport;
                experienceProfile.xp += p.xp;
                foreach (KeyValuePair<int, int> furniture in p._furnitures)
                {
                    if (!experienceProfile._furnitures.ContainsKey(furniture.Key))
                        experienceProfile._furnitures[furniture.Key] = furniture.Value;
                    else
                        experienceProfile._furnitures[furniture.Key] += furniture.Value;
                }
                experienceProfile.stats = (ProfileStats)(experienceProfile.stats + p.stats);
                foreach (string unlock in p.unlocks)
                {
                    if (!experienceProfile.unlocks.Contains(unlock))
                        experienceProfile.unlocks.Add(unlock);
                }
                foreach (KeyValuePair<string, ChallengeData> challenge in Challenges.challenges)
                {
                    ChallengeSaveData saveData1 = p.GetSaveData(challenge.Key);
                    ChallengeSaveData saveData2 = experienceProfile.GetSaveData(challenge.Key);
                    if (saveData2.trophy == TrophyType.Baseline && saveData1.trophy != TrophyType.Baseline || saveData1.trophy != 0 & pPreferDefault)
                    {
                        saveData2.Deserialize(saveData1.Serialize());
                        saveData2.profileID = experienceProfile.id;
                        experienceProfile.challengeData[saveData2.challenge] = saveData2;
                    }
                }
                experienceProfile.ticketCount = Challenges.GetTicketCount(experienceProfile);
                Profiles.Save(experienceProfile);
            }
            if (pShowDialog)
            {
                UIMenu menu = new UIMenu("@WRENCH@MERGE COMPLETE@SCREWDRIVER@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 260f, conString: "@CANCEL@BACK @SELECT@SELECT");
                menu.Add(new UIText("Successfully merged profiles!", Colors.DGBlue), true);
                menu.Add(new UIMenuItem("FINALLY!!", new UIMenuActionCloseMenu(menu)), true);
                menu.SetBackFunction(new UIMenuActionCloseMenuCallFunction(menu, new UIMenuActionCloseMenuCallFunction.Function(Options.OptionsMenuClosed)));
                menu.Close();
                Level.Add(menu);
                MonoMain.pauseMenu = menu;
                menu.Open();
            }
            Options.Data.defaultAccountMerged = true;
            Options.Save();
        }

        public static UIMenu CreateProfileMergeMenu()
        {
            UIMenu menu = new UIMenu("@WRENCH@MERGE PROFILES@SCREWDRIVER@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 270f, conString: "@CANCEL@BACK @SELECT@SELECT");
            menu.Add(new UIText("Looks like you have a 'DEFAULT'", Colors.DGBlue), true);
            menu.Add(new UIText("profile. These are now obsolete.", Colors.DGBlue), true);
            menu.Add(new UIText("", Colors.DGBlue), true);
            menu.Add(new UIText("Would you like to merge all", Colors.DGBlue), true);
            menu.Add(new UIText("data from the 'DEFAULT' profile", Colors.DGBlue), true);
            menu.Add(new UIText("into this one?", Colors.DGBlue), true);
            menu.Add(new UIText("", Colors.DGBlue), true);
            menu.Add(new UIMenuItem("NO!", new UIMenuActionCloseMenu(menu)), true);
            menu.Add(new UIMenuItem("YES! (PREFER DEFAULT)", new UIMenuActionCloseMenuCallFunction(menu, new UIMenuActionCloseMenuCallFunction.Function(Options.MergeDefaultPreferDefault))), true);
            menu.Add(new UIMenuItem("YES! (PREFER THIS ACCOUNT)", new UIMenuActionCloseMenuCallFunction(menu, new UIMenuActionCloseMenuCallFunction.Function(Options.MergeDefaultPreferAccount))), true);
            menu.SetBackFunction(new UIMenuActionCloseMenuCallFunction(menu, new UIMenuActionCloseMenuCallFunction.Function(Options.OptionsMenuClosed)));
            menu.Close();
            return menu;
        }

        public static UIMenu CreateResolutionApplyMenu()
        {
            UIMenu menu = new UIMenu("@WRENCH@NEW ASPECT RATIO@SCREWDRIVER@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 270f, conString: "@CANCEL@BACK @SELECT@SELECT");
            menu.Add(new UIText("To apply a resolution", Colors.DGBlue), true);
            menu.Add(new UIText("with a different aspect ratio,", Colors.DGBlue), true);
            menu.Add(new UIText("The game must be restarted.", Colors.DGBlue), true);
            menu.Add(new UIText("", Colors.DGBlue), true);
            menu.Add(new UIText("Would you like to restart", Colors.DGBlue), true);
            menu.Add(new UIText("and apply changes?", Colors.DGBlue), true);
            menu.Add(new UIText("", Colors.DGBlue), true);
            menu.Add(new UIMenuItem("NO!", new UIMenuActionCloseMenuCallFunction(menu, new UIMenuActionCloseMenuCallFunction.Function(Options.CancelResolutionChange))), true);
            menu.Add(new UIMenuItem("YES! (Restart)", new UIMenuActionCloseMenuCallFunction(menu, new UIMenuActionCloseMenuCallFunction.Function(Options.RestartAndApplyResolution))), true);
            menu.SetBackFunction(new UIMenuActionCloseMenuCallFunction(menu, new UIMenuActionCloseMenuCallFunction.Function(Options.OptionsMenuClosed)));
            menu.Close();
            return menu;
        }

        public static void OpenOptionsMenu()
        {
            Options._removedOptionsMenu = false;
            Options._openedOptionsMenu = true;
            Level.Add(_optionsMenu);
            Options._optionsMenu.Open();
        }

        public static void OptionsMenuClosed()
        {
            Options.Save();
            Options.SaveLocalData();
            if (Options.openOnClose == null)
                return;
            Options.openOnClose.Open();
        }

        public static string optionsFileName => DuckFile.optionsDirectory + "/options.dat";

        private static string optionsFileLocalName => DuckFile.optionsDirectory + "/localsettings.dat";

        public static void Save()
        {
            if (!Options.loadCalled)
            {
                if (!MonoMain.logFileOperations)
                    return;
                DevConsole.Log(DCSection.General, "Options.Save() skipped (loadCalled = false)");
            }
            else
            {
                if (MonoMain.logFileOperations)
                    DevConsole.Log(DCSection.General, "Options.Save()");
                DuckXML doc = new DuckXML();
                DXMLNode node = new DXMLNode("Data");
                node.Add(Options._data.Serialize());
                doc.Add(node);
                DuckFile.SaveDuckXML(doc, Options.optionsFileName);
            }
        }

        public static void SaveLocalData()
        {
            if (!Options.loadCalled)
            {
                if (!MonoMain.logFileOperations)
                    return;
                DevConsole.Log(DCSection.General, "Options.SaveLocalData() skipped (loadCalled = false)");
            }
            else
            {
                if (MonoMain.logFileOperations)
                    DevConsole.Log(DCSection.General, "Options.SaveLocalData()");
                DuckXML doc = new DuckXML();
                DXMLNode node = new DXMLNode("Data");
                node.Add(Options._localData.Serialize());
                doc.Add(node);
                DuckFile.SaveDuckXML(doc, Options.optionsFileLocalName);
            }
        }

        public static void Load()
        {
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "Options.Load()");
            Options.loadCalled = true;
            DuckXML duckXml1 = DuckFile.LoadDuckXML(Options.optionsFileName);
            if (duckXml1 != null)
            {
                Profile profile = new Profile("");
                IEnumerable<DXMLNode> source = duckXml1.Elements("Data") ?? duckXml1.Elements("OptionsData");
                if (source != null)
                {
                    foreach (DXMLNode element in source.Elements<DXMLNode>())
                    {
                        if (element.Name == nameof(Options))
                        {
                            Options._data.Deserialize(element);
                            break;
                        }
                    }
                }
            }
            DuckXML duckXml2 = DuckFile.LoadDuckXML(Options.optionsFileLocalName);
            if (duckXml2 == null)
                return;
            Profile profile1 = new Profile("");
            IEnumerable<DXMLNode> source1 = duckXml2.Elements("Data");
            if (source1 == null)
                return;
            foreach (DXMLNode element in source1.Elements<DXMLNode>())
            {
                if (element.Name == nameof(Options))
                {
                    Options._localData.Deserialize(element);
                    break;
                }
            }
        }

        public static void PostLoad()
        {
            if ((double)Options.Data.musicVolume > 1.0)
                Options.Data.musicVolume /= 100f;
            if ((double)Options.Data.sfxVolume > 1.0)
                Options.Data.sfxVolume /= 100f;
            if (Options.Data.windowScale < 0)
                Options.Data.windowScale = !MonoMain.fourK ? 0 : 1;
            Options.Data.consoleWidth = Math.Min(100, Math.Max(Options.Data.consoleWidth, 25));
            Options.Data.consoleHeight = Math.Min(100, Math.Max(Options.Data.consoleHeight, 10));
            Options.Data.consoleScale = Math.Min(5, Math.Max(Options.Data.consoleScale, 1));
            if (Options.Data.currentSaveVersion != -1)
            {
                if (Options.Data.currentSaveVersion < 2)
                    Options.Data.consoleScale = 1;
                if (Options.Data.currentSaveVersion < 3)
                    Options.Data.windowedFullscreen = true;
            }
            if (Options.Data.currentSaveVersion < 4 || Options.Data.currentSaveVersion == -1)
                DGSave.showOnePointFiveMessages = true;
            if (Options.Data.currentSaveVersion < 5)
            {
                if (Options.Data.keyboard1PlayerIndex > 0)
                {
                    Options.legacyPreferredColor = Options.Data.keyboard1PlayerIndex;
                    Options.Data.keyboard1PlayerIndex = 0;
                }
                Options.Data.windowedFullscreen = true;
            }
            if (Options.Data.audioMode == 0 || Options.Data.audioMode >= 4)
                Options.Data.audioMode = 2;
            Options.Data.UpdateCurrentVersion();
            if (Options.LocalData.previousAdapterResolution == null || Resolution.adapterResolution != Options.LocalData.previousAdapterResolution)
            {
                Resolution.RestoreDefaults();
                Options.LocalData.previousAdapterResolution = Resolution.adapterResolution;
            }
            if (Options.LocalData.windowedResolution.mode != ScreenMode.Windowed)
                Options.LocalData.windowedResolution = Resolution.FindNearest(ScreenMode.Windowed, Options.LocalData.windowedResolution.x, Options.LocalData.windowedResolution.y);
            if (Options.LocalData.fullscreenResolution.mode != ScreenMode.Fullscreen)
                Options.LocalData.fullscreenResolution = Resolution.FindNearest(ScreenMode.Fullscreen, Options.LocalData.fullscreenResolution.x, Options.LocalData.fullscreenResolution.y);
            if (Options.LocalData.windowedFullscreenResolution.mode != ScreenMode.Borderless)
                Options.LocalData.windowedFullscreenResolution = Resolution.FindNearest(ScreenMode.Borderless, Options.LocalData.windowedFullscreenResolution.x, Options.LocalData.windowedFullscreenResolution.y);
            Options.LocalData.currentResolution = !Options.Data.fullscreen ? Options.LocalData.windowedResolution : (Options.Data.windowedFullscreen ? Options.LocalData.windowedFullscreenResolution : Options.LocalData.fullscreenResolution);
            if (MonoMain.oldAngles)
                Options.Data.oldAngleCode = true;
            if (!MonoMain.defaultControls)
                return;
            Options.Data.keyboard1PlayerIndex = 0;
            Options.Data.keyboard2PlayerIndex = 1;
        }

        public static void Update()
        {
            Music.masterVolume = Math.Min(1f, Math.Max(0.0f, Options.Data.musicVolume));
            SFX.volume = Math.Min(1f, Math.Max(0.0f, Options.Data.sfxVolume));
            if (Options._openedOptionsMenu && !Options._removedOptionsMenu && Options._optionsMenu != null && !Options._optionsMenu.open && !Options._optionsMenu.animating)
            {
                Options._openedOptionsMenu = false;
                Options._removedOptionsMenu = true;
                Level.Remove(_optionsMenu);
            }
            if (Options._resolutionChanged)
            {
                Options._resolutionChanged = false;
                Options.ResolutionChanged();
            }
            if (Options.flagForSave <= 0)
                return;
            --Options.flagForSave;
            if (Options.flagForSave != 0)
                return;
            Options.Save();
        }

        public static void ResolutionChanged()
        {
            if (!MonoMain.started)
            {
                Options._resolutionChanged = true;
            }
            else
            {
                FontGDIContext._fontDatas.Clear();
                DuckNetwork.UpdateFont();
                DevConsole.RefreshConsoleFont();
            }
        }
    }
}
