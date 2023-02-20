using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ManagedBass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    public class SoundEffect
    {
        public static float DistanceScale { get; set; }

        public static float DopplerScale { get; set; }

        public static float MasterVolume { get; set; }

        public static float SpeedOfSound { get; set; }

        public TimeSpan Duration { get; }

        public bool IsDisposed { get; }

        public string Name { get; set; }

        public int Handle;
        public bool isMusic;

        public static SoundEffect FromStream(Stream stream)
        {
            return SoundEffect.FromStream(stream, "wav");
        }

        public static SoundEffect FromStream(Stream stream, string extension)
        {
            //SoundEffect e = new SoundEffect();
            //if (e.Platform_Construct(stream, extension))
            //{
            //    return e;
            //}
            DevConsole.Log(DCSection.General, "|DGRED|SoundEffect.FromStream Failed!", -1);
            return null;
        }

        public static SoundEffect CreateStreaming(string pPath)
        {
            if (File.Exists(pPath))
            {
                return new SoundEffect
                {
                    streaming = true,
                    file = pPath
                };
            }
            DevConsole.Log(DCSection.General, "|DGRED|SoundEffect.CreateStreaming Failed (file not found)!", -1);
            return null;
        }

        public SoundEffect(string pPath, bool isMusic)
        {
            isMusic = isMusic;
            this.file = pPath;
            Handle = AndroidAudio.FromFile(pPath, isMusic);
        }

        public SoundEffect(string pPath)
        {
            this.file = pPath;
            Handle = AndroidAudio.FromFile(pPath, isMusic);
        }

        public SoundEffect()
        {
        }

        public SoundEffectInstance CreateInstance()
        {
            return new SoundEffectInstance(this);
        }

        public float[] data
        {
            get {
                return this._waveBuffer;
            }
        }

        public WaveFormat format { get; private set; }

        public int decodedSamples
        {
            get {
                return this._decodedSamples;
            }
        }

        public int totalSamples
        {
            get {
                return this._totalSamples;
            }
        }

        public int Decode(float[] pBuffer, int pOffset, int pCount)
        {
            throw new NotImplementedException();
        }

        public void Rewind()
        {
            AndroidAudio.Rewind(Handle);
        }        

        public void Dispose()
        {
            AndroidAudio.Free(Handle);
        }

        public string file;

        public bool streaming;

        public static float[] _songBuffer;

        private float[] _waveBuffer;

        public int dataSize;

        private int _decodedSamples;

        private int _totalSamples;

        private int kDecoderChunkSize = 22050;

        private static int kDecoderIndex = 0;

        private static object kDecoderHandle = new object();

        private int _decoderIndex;

        public float replaygainModifier = 1f;

        private Stream _stream;
    }
}