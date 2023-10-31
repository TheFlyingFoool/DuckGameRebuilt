// Decompiled with JetBrains decompiler
// Type: DuckGame.Speech
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            if (_speech != null)
                return;
            _speech = new SpeechSynthesizer();
            (_speech as SpeechSynthesizer).SetOutputToDefaultAudioDevice();
            ApplyTTSSettings();
        }

        public object speech => _speech;

        public void Say(string pString)
        {
            if (_speech == null)
                return;
            (speech as SpeechSynthesizer).SpeakAsync(pString);
        }

        public void StopSaying()
        {
            if (_speech == null)
                return;
            (speech as SpeechSynthesizer).SpeakAsyncCancelAll();
        }

        public void SetSayVoice(string pName)
        {
            if (_speech == null)
                return;
            try
            {
                (speech as SpeechSynthesizer).SelectVoice(pName);
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "|DGRED|SFX.SetSayVoice failed:" + ex.Message);
            }
        }

        public List<string> GetSayVoices()
        {
            if (_speech == null)
                return new List<string>();
            List<string> sayVoices = new List<string>();
            try
            {
                foreach (InstalledVoice installedVoice in (speech as SpeechSynthesizer).GetInstalledVoices().ToList())
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
            if (_speech == null)
                return;
            try
            {
                if (Program.DotNetBuild || Program.IsLinuxD || Program.isLinux || !Options.Data.textToSpeech)
                    return;
                (speech as SpeechSynthesizer).SpeakAsyncCancelAll();
                List<InstalledVoice> list = (speech as SpeechSynthesizer).GetInstalledVoices().ToList();
                if (Options.Data.textToSpeechVoice >= 0 && Options.Data.textToSpeechVoice < list.Count)
                    (speech as SpeechSynthesizer).SelectVoice(list[Options.Data.textToSpeechVoice].VoiceInfo.Name);
                (speech as SpeechSynthesizer).Volume = Maths.Clamp((int)(Options.Data.textToSpeechVolume * 100.0), 0, 100);
                (speech as SpeechSynthesizer).Rate = Maths.Clamp((int)Math.Round((Options.Data.textToSpeechRate - 0.5) * 20.0), -10, 10);
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "|DGRED|SFX.ApplyTTSSettings failed:" + ex.Message);
            }
        }

        public void SetOutputToDefaultAudioDevice() => (_speech as SpeechSynthesizer).SetOutputToDefaultAudioDevice();
    }
}
