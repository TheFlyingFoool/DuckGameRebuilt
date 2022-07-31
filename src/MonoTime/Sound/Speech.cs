// Decompiled with JetBrains decompiler
// Type: DuckGame.Speech
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;

namespace DuckGame
{
    public class Speech
    {
        public object _speech;

        public void Initialize()
        {
            if (this._speech != null)
                return;
            this._speech = new SpeechSynthesizer();
            (this._speech as SpeechSynthesizer).SetOutputToDefaultAudioDevice();
            this.ApplyTTSSettings();
        }

        public object speech => this._speech;

        public void Say(string pString)
        {
            if (this._speech == null)
                return;
            (this.speech as SpeechSynthesizer).SpeakAsync(pString);
        }

        public void StopSaying()
        {
            if (this._speech == null)
                return;
            (this.speech as SpeechSynthesizer).SpeakAsyncCancelAll();
        }

        public void SetSayVoice(string pName)
        {
            if (this._speech == null)
                return;
            try
            {
                (this.speech as SpeechSynthesizer).SelectVoice(pName);
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "|DGRED|SFX.SetSayVoice failed:" + ex.Message);
            }
        }

        public List<string> GetSayVoices()
        {
            if (this._speech == null)
                return new List<string>();
            List<string> sayVoices = new List<string>();
            try
            {
                foreach (InstalledVoice installedVoice in (this.speech as SpeechSynthesizer).GetInstalledVoices().ToList<InstalledVoice>())
                    sayVoices.Add(installedVoice.VoiceInfo.Name);
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "|DGRED|SFX.GetSayVoices failed:" + ex.Message);
            }
            return sayVoices;
        }

        public void ApplyTTSSettings()
        {
            if (this._speech == null)
                return;
            try
            {
                if (Program.isLinux || !Options.Data.textToSpeech)
                    return;
                (this.speech as SpeechSynthesizer).SpeakAsyncCancelAll();
                List<InstalledVoice> list = (this.speech as SpeechSynthesizer).GetInstalledVoices().ToList<InstalledVoice>();
                if (Options.Data.textToSpeechVoice >= 0 && Options.Data.textToSpeechVoice < list.Count)
                    (this.speech as SpeechSynthesizer).SelectVoice(list[Options.Data.textToSpeechVoice].VoiceInfo.Name);
                (this.speech as SpeechSynthesizer).Volume = Maths.Clamp((int)(Options.Data.textToSpeechVolume * 100.0), 0, 100);
                (this.speech as SpeechSynthesizer).Rate = Maths.Clamp((int)Math.Round((Options.Data.textToSpeechRate - 0.5) * 20.0), -10, 10);
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "|DGRED|SFX.ApplyTTSSettings failed:" + ex.Message);
            }
        }

        public void SetOutputToDefaultAudioDevice() => (this._speech as SpeechSynthesizer).SetOutputToDefaultAudioDevice();
    }
}
