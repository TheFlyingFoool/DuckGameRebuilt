// Decompiled with JetBrains decompiler
// Type: DuckGame.OptionsData
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class OptionsData : DataClass
    {
        private bool _textToSpeech;
        private float _textToSpeechVolume;
        private float _textToSpeechRate;
        private int _textToSpeechVoice;

        public int audioMode { get; set; }

        public float musicVolume { get; set; }

        public float sfxVolume { get; set; }

        public bool muteOnBackground { get; set; }

        public bool shennanigans { get; set; }

        public bool controllerRumble { get; set; }

        public bool fullscreen { get; set; }

        public bool powerUser { get; set; }

        public bool gotDevMedal { get; set; }

        public float rumbleIntensity { get; set; }

        public int currentSaveVersion { get; set; }

        public int specialTimes { get; set; }

        public ulong lastSteamID { get; set; }

        public bool vsync { get; set; }

        public bool altTimestep { get; set; }

        public bool windowedFullscreen { get; set; }

        public bool flashing { get; set; }

        public bool fireGlow { get; set; }

        public bool lighting { get; set; }

        public bool fillBackground { get; set; }

        public bool cloud { get; set; }

        public bool disableModOnCrash { get; set; }

        public bool disableModOnLoadFailure { get; set; }

        public int keyboard1PlayerIndex { get; set; }

        public int keyboard2PlayerIndex { get; set; }

        public int windowScale { get; set; }

        public bool imeSupport { get; set; }

        public bool audioExclusiveMode { get; set; }

        public int consoleWidth { get; set; }

        public int consoleHeight { get; set; }

        public int consoleScale { get; set; }

        public string consoleFont { get; set; }

        public int consoleFontSize { get; set; }

        public int chatOpacity { get; set; }

        public int chatMode { get; set; }

        public string chatFont { get; set; }

        public int chatFontSize { get; set; }

        public int chatHeadScale { get; set; }

        public Dictionary<ulong, string> muteSettings { get; set; }

        public List<ulong> blockedPlayers { get; set; }

        public List<ulong> unblockedPlayers { get; set; }

        public List<ulong> recentPlayers { get; set; }

        public bool oldAngleCode { get; set; }

        public bool languageFilter { get; set; }

        public int mojiFilter { get; set; }

        public int hatFilter { get; set; }

        public bool hatParticles { get; set; }

        public bool textToSpeech
        {
            get => _textToSpeech;
            set
            {
                if (!Program.IsLinuxD && !Program.isLinux && _textToSpeech != value && value)
                {
                    SFX.ApplyTTSSettings();
                    if (MonoMain.started)
                    {
                        SFX.StopSaying();
                        SFX.Say("Chat Text To Speech Enabled!");
                    }
                }
                _textToSpeech = value;
            }
        }

        public float textToSpeechVolume
        {
            get => _textToSpeechVolume;
            set
            {
                if (!Program.isLinux && _textToSpeechVolume != value && textToSpeech)
                {
                    SFX.ApplyTTSSettings();
                    if (MonoMain.started)
                    {
                        SFX.StopSaying();
                        SFX.Say("Volume " + ((int)(value * 100.0)).ToString());
                    }
                }
                _textToSpeechVolume = value;
            }
        }

        public bool textToSpeechReadNames { get; set; }

        public float textToSpeechRate
        {
            get => _textToSpeechRate;
            set
            {
                if (!Program.isLinux && _textToSpeechRate != value && textToSpeech)
                {
                    SFX.ApplyTTSSettings();
                    if (MonoMain.started)
                    {
                        SFX.StopSaying();
                        SFX.Say("Speed " + ((int)Math.Round((value - 0.5) * 20.0)).ToString());
                    }
                }
                _textToSpeechRate = value;
            }
        }

        public int textToSpeechVoice
        {
            get => _textToSpeechVoice;
            set
            {
                if (!Program.isLinux && _textToSpeechVoice != value && textToSpeech)
                {
                    SFX.ApplyTTSSettings();
                    List<string> sayVoices = SFX.GetSayVoices();
                    if (value < 0 || value >= sayVoices.Count)
                        value = 0;
                    SFX.SetSayVoice(sayVoices[value]);
                    if (MonoMain.started)
                    {
                        SFX.StopSaying();
                        SFX.Say(sayVoices[value]);
                    }
                }
                _textToSpeechVoice = value;
            }
        }

        public bool defaultAccountMerged { get; set; }

        public bool didAutoMerge { get; set; }

        public bool showNetworkModWarning { get; set; }

        public bool showControllerWarning { get; set; }

        public OptionsData()
        {
            _nodeName = "Options";
            sfxVolume = 0.8f;
            musicVolume = 0.7f;
            muteOnBackground = false;
            shennanigans = true;
            controllerRumble = true;
            vsync = true;
            windowedFullscreen = true;
            windowScale = -1;
            specialTimes = 0;
            flashing = true;
            fireGlow = true;
            lighting = true;
            disableModOnCrash = true;
            disableModOnLoadFailure = true;
            showNetworkModWarning = true;
            showControllerWarning = true;
            fillBackground = true;
            keyboard1PlayerIndex = 0;
            keyboard2PlayerIndex = 1;
            rumbleIntensity = 1f;
            powerUser = true; //set to true
            defaultAccountMerged = false;
            didAutoMerge = false;
            cloud = true;
            textToSpeechVolume = 1f;
            textToSpeech = false;
            textToSpeechReadNames = true;
            textToSpeechRate = 0.5f;
            languageFilter = true;
            chatHeadScale = 0;
            chatFontSize = 18;
            chatFont = "";
            chatOpacity = 100;
            muteSettings = new Dictionary<ulong, string>();
            blockedPlayers = new List<ulong>();
            unblockedPlayers = new List<ulong>();
            recentPlayers = new List<ulong>();
            consoleWidth = 80;
            consoleHeight = 40;
            consoleScale = 1;
            consoleFontSize = 12;
            currentSaveVersion = -1;
            audioMode = 2;
        }

        public void UpdateCurrentVersion() => currentSaveVersion = 5;
    }
}
