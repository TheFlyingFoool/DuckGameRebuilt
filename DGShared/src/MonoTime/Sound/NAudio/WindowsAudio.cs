// Decompiled with JetBrains decompiler
// Type: DuckGame.Windows_Audio
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml
#if !__ANDROID__
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace DuckGame
{
    public class Windows_Audio // also Linux but not going to change the name :P 
    {
        private static IWavePlayer _output;
        private static MixingSampleProvider _mixer;
        private static AudioMode _mode = AudioMode.Wasapi;
        public static bool initialized = false;
        private static int _losingDevice = 0;
        private static AudioMode _forceMode = AudioMode.None;
        private bool _recreateNonExclusive;
        private bool _recreateAlternateAudio;
        private static ISampleProvider _currentMusic;
        private MMDeviceEnumerator deviceEnum;
        private NotificationClientImplementation notificationClient;
        private IMMNotificationClient notifyClient;

        public void Platform_Initialize()
        {
            if (Program.IsLinuxD)
            {
                try
                {
                    float n = Microsoft.Xna.Framework.Audio.SoundEffect.SpeedOfSound;
                }
                catch (Exception)
                {
                    initialized = false;
                    return;
                }
                initialized = true;
                return;
            }
            try
            {
                if (WaveOut.DeviceCount == 0)
                {
                    initialized = false;
                    return;
                }
            }
            catch (Exception)
            {
                initialized = false;
                return;
            }
            ResetDevice();
        }

        public void Update()
        {
            if (Program.IsLinuxD) // mabye come back to this later
            {
                return;
            }
            if (!initialized || SFX._audio == null || _losingDevice <= 0 || _output != null && _output.PlaybackState != PlaybackState.Stopped)
                return;
            if (_output != null)
            {
                _output.Dispose();
                _output = null;
            }
            --_losingDevice;
            if (_losingDevice != 0)
                return;
            RecreateDevice();
        }

        public void LoseDevice()
        {
            if (Program.IsLinuxD) // mabye come back to this later
            {
                return;
            }
            if (!initialized || _output == null)
                return;
            _output.Stop();
            _losingDevice = 60;
        }

        public static AudioMode forceMode
        {
            get => _forceMode;
            set
            {
                _forceMode = value;
                if (Program.IsLinuxD) // mabye come back to this later
                {
                    return;
                }
                if (SFX._audio == null)
                    return;
                ResetDevice();
            }
        }

        public static void ResetDevice()
        {
            _mode = MonoMain.audioModeOverride == AudioMode.None ? (AudioMode)Options.Data.audioMode : MonoMain.audioModeOverride;
            if (Program.IsLinuxD) // mabye come back to this later
            {
                return;
            }
            if (_output != null)
                SFX._audio.LoseDevice();
            else
                SFX._audio.RecreateDevice();
        }

        private void RecreateDevice()
        {
            if (Program.IsLinuxD) // mabye come back to this later
            {
                return;
            }
            try
            {
                if (_output != null)
                {
                    _output.Stop();
                    _output.Dispose();
                    _output = null;
                }
                if (_forceMode != AudioMode.None && !_recreateAlternateAudio && !_recreateNonExclusive)
                    _mode = _forceMode;
                switch (_mode)
                {
                    case AudioMode.Wave:
                        int deviceCount = WaveOut.DeviceCount;
                        _output = new WaveOutEvent()
                        {
                            DesiredLatency = 50,
                            NumberOfBuffers = 10
                        };
                        break;
                    case AudioMode.DirectSound:
                        _output = new DirectSoundOut();
                        break;
                    default:
                        if (!_recreateAlternateAudio)
                        {
                            if (notificationClient == null)
                            {
                                notificationClient = new NotificationClientImplementation(this);
                                notifyClient = notificationClient;
                                if (deviceEnum == null)
                                    deviceEnum = new MMDeviceEnumerator();
                                deviceEnum.RegisterEndpointNotificationCallback(notifyClient);
                            }
                            new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                            _output = new WasapiOut(_recreateNonExclusive || !Options.Data.audioExclusiveMode ? AudioClientShareMode.Shared : AudioClientShareMode.Exclusive, 20);
                            break;
                        }
                        goto case AudioMode.Wave;
                }
                if (_mixer == null)
                {
                    _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2))
                    {
                        ReadFully = true
                    };
                }
                _output.Init(_mixer);
            }
            catch (Exception ex)
            {
                if (_recreateAlternateAudio)
                {
                    initialized = false;
                    _output = null;
                    _mixer = null;
                    return;
                }
                if (_recreateNonExclusive)
                {
                    _recreateAlternateAudio = true;
                    DevConsole.Log(DCSection.General, "|DGRED|Failed to create audio device, reattempting creation in alternate mode:");
                    DevConsole.Log(DCSection.General, ex.Message);
                    RecreateDevice();
                    _recreateNonExclusive = false;
                    _recreateAlternateAudio = false;
                }
                else
                {
                    _recreateNonExclusive = true;
                    DevConsole.Log(DCSection.General, "|DGRED|Failed to create audio device, reattempting creation in non-exclusive mode:");
                    DevConsole.Log(DCSection.General, ex.Message);
                    RecreateDevice();
                    _recreateNonExclusive = false;
                    return;
                }
            }
            if (_output == null)
                return;
            initialized = true;
            _output.Play();
        }

        public static void AddSound(ISampleProvider pSound, bool pIsMusic)
        {
            if (Program.IsLinuxD) // mabye come back to this later
            {
                return;
            }
            if (!initialized || pSound == null || _mixer.MixerInputs == null || _mixer == null || _output == null)
                return;
            if (_mixer.MixerInputs.Count() > 32)
            {
                lock (_mixer.MixerInputs)
                {
                    ISampleProvider mixerInput = _mixer.MixerInputs.Where(x => x != _currentMusic).First();
                    _mixer.RemoveMixerInput(mixerInput);
                }
            }
            _mixer.AddMixerInput(pSound);
            if (_losingDevice <= 0)
                _output.Play();
            if (!pIsMusic)
                return;
            _currentMusic = pSound;
        }

        public static void RemoveSound(ISampleProvider pSound)
        {
            if (Program.IsLinuxD) // mabye come back to this later
            {
                return;
            }
            if (!initialized || _mixer == null)
                return;
            _mixer.RemoveMixerInput(pSound);
        }

        public void Dispose()
        {
            if (Program.IsLinuxD) // mabye come back to this later
            {
                return;
            }
            if (!initialized || notificationClient == null)
                return;
            UnRegisterEndpointNotificationCallback(notificationClient);
            _output.Dispose();
        }

        /// <summary>Registers a call back for Device Events</summary>
        /// <param name="client">Object implementing IMMNotificationClient type casted as IMMNotificationClient interface</param>
        /// <returns></returns>
        public int RegisterEndpointNotificationCallback([MarshalAs(UnmanagedType.Interface), In] IMMNotificationClient client)
        {
            if (Program.IsLinuxD) // mabye come back to this later
            {
                return 0;
            }
            if (deviceEnum == null)
                deviceEnum = new MMDeviceEnumerator();
            return deviceEnum.RegisterEndpointNotificationCallback(client);
        }

        /// <summary>UnRegisters a call back for Device Events</summary>
        /// <param name="client">Object implementing IMMNotificationClient type casted as IMMNotificationClient interface </param>
        /// <returns></returns>
        public int UnRegisterEndpointNotificationCallback([MarshalAs(UnmanagedType.Interface), In] IMMNotificationClient client)
        {
            if (Program.IsLinuxD) // mabye come back to this later
            {
                return 0;
            }
            if (deviceEnum == null)
                deviceEnum = new MMDeviceEnumerator();
            return deviceEnum.UnregisterEndpointNotificationCallback(client);
        }

        private class NotificationClientImplementation : IMMNotificationClient
        {
            private Windows_Audio _owner;

            public void OnDefaultDeviceChanged(
              DataFlow dataFlow,
              Role deviceRole,
              string defaultDeviceId)
            {
                _owner.LoseDevice();
            }

            public void OnDeviceAdded(string deviceId)
            {
            }

            public void OnDeviceRemoved(string deviceId)
            {
            }

            public void OnDeviceStateChanged(string deviceId, DeviceState newState)
            {
            }

            public NotificationClientImplementation(Windows_Audio pOwner)
            {
                _owner = pOwner;
                if (Environment.OSVersion.Version.Major < 6)
                    throw new NotSupportedException("This functionality is only supported on Windows Vista or newer.");
            }

            public void OnPropertyValueChanged(string deviceId, PropertyKey propertyKey)
            {
            }
        }
    }
}
#endif