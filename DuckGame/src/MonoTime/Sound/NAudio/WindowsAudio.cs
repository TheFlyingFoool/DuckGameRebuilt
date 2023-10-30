using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace DuckGame
{
    public class Windows_Audio // also Linux but not going to change the name :P 
    {
        private static AudioMode _mode = AudioMode.Wasapi;
        public static bool initialized = false;
        private static int _losingDevice = 0;
        private static AudioMode _forceMode = AudioMode.None;
        private bool _recreateNonExclusive;
        private bool _recreateAlternateAudio;


        public void Platform_Initialize()
        {
            if (true)
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
        }

        public void Update()
        {
            if (true) // mabye come back to this later
            {
                return;
            }
        }

        public void LoseDevice()
        {
            if (true) // mabye come back to this later
            {
                return;
            }
        }

        public static AudioMode forceMode
        {
            get => _forceMode;
            set
            {
                _forceMode = value;
                if (true) // mabye come back to this later
                {
                    return;
                }
            }
        }

        public static void ResetDevice()
        {
            _mode = MonoMain.audioModeOverride == AudioMode.None ? (AudioMode)Options.Data.audioMode : MonoMain.audioModeOverride;
            if (true) // mabye come back to this later
            {
                return;
            }
        }

        private void RecreateDevice()
        {
            if (true) // mabye come back to this later
            {
                return;
            }
        }

       

        public void Dispose()
        {
            if (true) // mabye come back to this later
            {
                return;
            }
        }

        /// <summary>Registers a call back for Device Events</summary>
        /// <param name="client">Object implementing IMMNotificationClient type casted as IMMNotificationClient interface</param>
        /// <returns></returns>
       

        /// <summary>UnRegisters a call back for Device Events</summary>
        /// <param name="client">Object implementing IMMNotificationClient type casted as IMMNotificationClient interface </param>
        /// <returns></returns>
        
       
    }
}
