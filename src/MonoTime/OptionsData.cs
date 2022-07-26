// Decompiled with JetBrains decompiler
// Type: DuckGame.OptionsData
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            get => this._textToSpeech;
            set
            {
                if (!Program.isLinux && this._textToSpeech != value && value)
                {
                    SFX.ApplyTTSSettings();
                    if (MonoMain.started)
                    {
                        SFX.StopSaying();
                        SFX.Say("Chat Text To Speech Enabled!");
                    }
                }
                this._textToSpeech = value;
            }
        }

        public float textToSpeechVolume
        {
            get => this._textToSpeechVolume;
            set
            {
                if (!Program.isLinux && (double)this._textToSpeechVolume != (double)value && this.textToSpeech)
                {
                    SFX.ApplyTTSSettings();
                    if (MonoMain.started)
                    {
                        SFX.StopSaying();
                        SFX.Say("Volume " + ((int)((double)value * 100.0)).ToString());
                    }
                }
                this._textToSpeechVolume = value;
            }
        }

        public bool textToSpeechReadNames { get; set; }

        public float textToSpeechRate
        {
            get => this._textToSpeechRate;
            set
            {
                if (!Program.isLinux && (double)this._textToSpeechRate != (double)value && this.textToSpeech)
                {
                    SFX.ApplyTTSSettings();
                    if (MonoMain.started)
                    {
                        SFX.StopSaying();
                        SFX.Say("Speed " + ((int)Math.Round(((double)value - 0.5) * 20.0)).ToString());
                    }
                }
                this._textToSpeechRate = value;
            }
        }

        public int textToSpeechVoice
        {
            get => this._textToSpeechVoice;
            set
            {
                if (!Program.isLinux && this._textToSpeechVoice != value && this.textToSpeech)
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
                this._textToSpeechVoice = value;
            }
        }

        public bool defaultAccountMerged { get; set; }

        public bool didAutoMerge { get; set; }

        public bool showNetworkModWarning { get; set; }

        public bool showControllerWarning { get; set; }

        public OptionsData()
        {
            this._nodeName = "Options";
            this.sfxVolume = 0.8f;
            this.musicVolume = 0.7f;
            this.shennanigans = true;
            this.controllerRumble = true;
            this.vsync = true;
            this.windowedFullscreen = true;
            this.windowScale = -1;
            this.specialTimes = 0;
            this.flashing = true;
            this.fireGlow = true;
            this.lighting = true;
            this.disableModOnCrash = true;
            this.disableModOnLoadFailure = true;
            this.showNetworkModWarning = true;
            this.showControllerWarning = true;
            this.fillBackground = true;
            this.keyboard1PlayerIndex = 0;
            this.keyboard2PlayerIndex = 1;
            this.rumbleIntensity = 1f;
            this.powerUser = false;
            this.defaultAccountMerged = false;
            this.didAutoMerge = false;
            this.cloud = true;
            this.textToSpeechVolume = 1f;
            this.textToSpeech = false;
            this.textToSpeechReadNames = true;
            this.textToSpeechRate = 0.5f;
            this.languageFilter = true;
            this.chatHeadScale = 0;
            this.chatFontSize = 18;
            this.chatFont = "";
            this.chatOpacity = 100;
            this.muteSettings = new Dictionary<ulong, string>();
            this.blockedPlayers = new List<ulong>();
            this.unblockedPlayers = new List<ulong>();
            this.recentPlayers = new List<ulong>();
            this.consoleWidth = 80;
            this.consoleHeight = 40;
            this.consoleScale = 1;
            this.consoleFontSize = 12;
            this.currentSaveVersion = -1;
            this.audioMode = 2;
        }

        public void UpdateCurrentVersion() => this.currentSaveVersion = 5;
    }
}
