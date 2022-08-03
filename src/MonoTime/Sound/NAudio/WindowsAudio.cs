// Decompiled with JetBrains decompiler
// Type: DuckGame.Windows_Audio
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace DuckGame
{
    public class Windows_Audio
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
        private Windows_Audio.NotificationClientImplementation notificationClient;
        private IMMNotificationClient notifyClient;

        public void Platform_Initialize()
        {
            try
            {
                if (WaveOut.DeviceCount == 0)
                {
                    Windows_Audio.initialized = false;
                    return;
                }
            }
            catch (Exception)
            {
                Windows_Audio.initialized = false;
                return;
            }
            Windows_Audio.ResetDevice();
        }

        public void Update()
        {
            if (!Windows_Audio.initialized || SFX._audio == null || Windows_Audio._losingDevice <= 0 || Windows_Audio._output != null && Windows_Audio._output.PlaybackState != PlaybackState.Stopped)
                return;
            if (Windows_Audio._output != null)
            {
                Windows_Audio._output.Dispose();
                Windows_Audio._output = null;
            }
            --Windows_Audio._losingDevice;
            if (Windows_Audio._losingDevice != 0)
                return;
            RecreateDevice();
        }

        public void LoseDevice()
        {
            if (!Windows_Audio.initialized || Windows_Audio._output == null)
                return;
            Windows_Audio._output.Stop();
            Windows_Audio._losingDevice = 60;
        }

        public static AudioMode forceMode
        {
            get => Windows_Audio._forceMode;
            set
            {
                Windows_Audio._forceMode = value;
                if (SFX._audio == null)
                    return;
                Windows_Audio.ResetDevice();
            }
        }

        public static void ResetDevice()
        {
            Windows_Audio._mode = MonoMain.audioModeOverride == AudioMode.None ? (AudioMode)Options.Data.audioMode : MonoMain.audioModeOverride;
            if (Windows_Audio._output != null)
                SFX._audio.LoseDevice();
            else
                SFX._audio.RecreateDevice();
        }

        private void RecreateDevice()
        {
            try
            {
                if (Windows_Audio._output != null)
                {
                    Windows_Audio._output.Stop();
                    Windows_Audio._output.Dispose();
                    Windows_Audio._output = null;
                }
                if (Windows_Audio._forceMode != AudioMode.None && !_recreateAlternateAudio && !_recreateNonExclusive)
                    Windows_Audio._mode = Windows_Audio._forceMode;
                switch (Windows_Audio._mode)
                {
                    case AudioMode.Wave:
                        int deviceCount = WaveOut.DeviceCount;
                        Windows_Audio._output = new WaveOutEvent()
                        {
                            DesiredLatency = 50,
                            NumberOfBuffers = 10
                        };
                        break;
                    case AudioMode.DirectSound:
                        Windows_Audio._output = new DirectSoundOut();
                        break;
                    default:
                        if (!_recreateAlternateAudio)
                        {
                            if (notificationClient == null)
                            {
                                notificationClient = new Windows_Audio.NotificationClientImplementation(this);
                                notifyClient = notificationClient;
                                if (deviceEnum == null)
                                    deviceEnum = new MMDeviceEnumerator();
                                deviceEnum.RegisterEndpointNotificationCallback(notifyClient);
                            }
                            new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                            Windows_Audio._output = new WasapiOut(_recreateNonExclusive || !Options.Data.audioExclusiveMode ? AudioClientShareMode.Shared : AudioClientShareMode.Exclusive, 20);
                            break;
                        }
                        goto case AudioMode.Wave;
                }
                if (Windows_Audio._mixer == null)
                {
                    Windows_Audio._mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2))
                    {
                        ReadFully = true
                    };
                }
                Windows_Audio._output.Init(_mixer);
            }
            catch (Exception ex)
            {
                if (_recreateAlternateAudio)
                {
                    Windows_Audio.initialized = false;
                    Windows_Audio._output = null;
                    Windows_Audio._mixer = null;
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
            if (Windows_Audio._output == null)
                return;
            Windows_Audio.initialized = true;
            Windows_Audio._output.Play();
        }

        public static void AddSound(ISampleProvider pSound, bool pIsMusic)
        {
            if (!Windows_Audio.initialized || pSound == null || Windows_Audio._mixer.MixerInputs == null || Windows_Audio._mixer == null || Windows_Audio._output == null)
                return;
            if (Windows_Audio._mixer.MixerInputs.Count<ISampleProvider>() > 32)
            {
                lock (Windows_Audio._mixer.MixerInputs)
                {
                    ISampleProvider mixerInput = Windows_Audio._mixer.MixerInputs.Where<ISampleProvider>(x => x != Windows_Audio._currentMusic).First<ISampleProvider>();
                    Windows_Audio._mixer.RemoveMixerInput(mixerInput);
                }
            }
            Windows_Audio._mixer.AddMixerInput(pSound);
            if (Windows_Audio._losingDevice <= 0)
                Windows_Audio._output.Play();
            if (!pIsMusic)
                return;
            Windows_Audio._currentMusic = pSound;
        }

        public static void RemoveSound(ISampleProvider pSound)
        {
            if (!Windows_Audio.initialized || Windows_Audio._mixer == null)
                return;
            Windows_Audio._mixer.RemoveMixerInput(pSound);
        }

        public void Dispose()
        {
            if (!Windows_Audio.initialized || notificationClient == null)
                return;
            UnRegisterEndpointNotificationCallback(notificationClient);
            Windows_Audio._output.Dispose();
        }

        /// <summary>Registers a call back for Device Events</summary>
        /// <param name="client">Object implementing IMMNotificationClient type casted as IMMNotificationClient interface</param>
        /// <returns></returns>
        public int RegisterEndpointNotificationCallback([MarshalAs(UnmanagedType.Interface), In] IMMNotificationClient client)
        {
            if (deviceEnum == null)
                deviceEnum = new MMDeviceEnumerator();
            return deviceEnum.RegisterEndpointNotificationCallback(client);
        }

        /// <summary>UnRegisters a call back for Device Events</summary>
        /// <param name="client">Object implementing IMMNotificationClient type casted as IMMNotificationClient interface </param>
        /// <returns></returns>
        public int UnRegisterEndpointNotificationCallback([MarshalAs(UnmanagedType.Interface), In] IMMNotificationClient client)
        {
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
