// Decompiled with JetBrains decompiler
// Type: DuckGame.Speech
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;
#if !__ANDROID__
using System.Speech.Synthesis;
#endif

namespace DuckGame
{
    public class Speech
    {
        public object _speech;

        public void Initialize()
        {
#if !__ANDROID__
            if (_speech != null)
                return;
            _speech = new SpeechSynthesizer();
            (_speech as SpeechSynthesizer).SetOutputToDefaultAudioDevice();
            ApplyTTSSettings();
#endif
        }

        public object speech => _speech;

        public void Say(string pString)
        {
#if !__ANDROID__
            if (_speech == null)
                return;
            (speech as SpeechSynthesizer).SpeakAsync(pString);
#endif
        }

        public void StopSaying()
        {
#if !__ANDROID__
            if (_speech == null)
                return;
            (speech as SpeechSynthesizer).SpeakAsyncCancelAll();
#endif
        }

        public void SetSayVoice(string pName)
        {
#if !__ANDROID__
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
#endif
        }

        public List<string> GetSayVoices()
        {
            if (_speech == null)
                return new List<string>();
            List<string> sayVoices = new List<string>();
#if !__ANDROID__
            try
            {
                foreach (InstalledVoice installedVoice in (speech as SpeechSynthesizer).GetInstalledVoices().ToList())
                    sayVoices.Add(installedVoice.VoiceInfo.Name);
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "|DGRED|SFX.GetSayVoices failed:" + ex.Message);
            }
#endif
            return sayVoices;
        }

        public void ApplyTTSSettings()
        {
#if !__ANDROID__
            if (_speech == null)
                return;
            try
            {
                if (Program.IsLinuxD || Program.isLinux || !Options.Data.textToSpeech)
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
#endif
        }

        public void SetOutputToDefaultAudioDevice()
        {
#if !__ANDROID__
            (_speech as SpeechSynthesizer).SetOutputToDefaultAudioDevice();
#endif
        }
    }
}
