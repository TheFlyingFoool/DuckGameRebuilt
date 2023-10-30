using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DuckGame
{
    public class SoundEffect
    {
        public bool IsOgg;//dan
        public OggPlayer oggPlayer;//dan
        public Microsoft.Xna.Framework.Audio.SoundEffect soundEffect;//dan

        public string file;
        public bool streaming;
        public static float[] _songBuffer;
        private float[] _waveBuffer;
        public int dataSize;

        //private Thread _decoderThread;
        private int _decodedSamples;
        private int _totalSamples;
        private int kDecoderChunkSize = 22050;
        private static int kDecoderIndex = 0;
        private static object kDecoderHandle = new object();
        private int _decoderIndex;
        public float replaygainModifier = 1f;
        private Stream _stream;

        public static float DistanceScale { get; set; }

        public static float DopplerScale { get; set; }

        public static float MasterVolume { get; set; }

        public static float SpeedOfSound { get; set; }

        public TimeSpan Duration { get; }

        public bool IsDisposed { get; }

        public string Name { get; set; }

        public static SoundEffect FromStream(Stream stream) => FromStream(stream, "wav");
        public SoundEffect(Stream stream)
        {
            soundEffect = Microsoft.Xna.Framework.Audio.SoundEffect.FromStream(stream);
        }
        public static SoundEffect FromStream(Stream stream, string extension)
        {
                try
                {
                    return new SoundEffect(stream);
                }
                catch (Exception)
                { }
                DevConsole.Log(DCSection.General, "|DGRED|SoundEffect.FromStream Failed!", -1);
                return null;
            
        }

        public static SoundEffect CreateStreaming(string pPath)
        {
            if (File.Exists(pPath))
                return new SoundEffect()
                {
                    streaming = true,
                    file = pPath
                };
            DevConsole.Log(DCSection.General, "|DGRED|SoundEffect.CreateStreaming Failed (file not found)!");
            return null;
        }

        public SoundEffect(string pPath)
        {
            file = pPath;
            Platform_Construct(pPath);
        }

        public SoundEffect()
        {
        }

        public SoundEffectInstance CreateInstance() => new SoundEffectInstance(this);

        public float[] data => _waveBuffer;


        public int decodedSamples => _decodedSamples;

        public int totalSamples => _totalSamples;

        public int Decode(float[] pBuffer, int pOffset, int pCount) { return 0; }

        public void Rewind() { }

        public bool Decoder_DecodeChunk()
        {
            return false;
        }

        private void Thread_Decoder()
        {
            
        }

        public void Dispose()
        {
           
        }

        public bool Platform_Construct(Stream pStream, string pExtension)
        {
            return false;
        }

        private void PrepareReader(object reader, Stream pStream)
        {
            
        }

        public void Platform_Construct(string pPath)
        {
                int index = pPath.LastIndexOf(".");
                byte[] data = File.ReadAllBytes(pPath);
                if (index != -1 && pPath.Substring(index + 1).ToLower() == "ogg")
                {
                    IsOgg = true;
                    oggPlayer = new OggPlayer();
                    oggPlayer.SetOgg(new MemoryStream(data));
                }
                else
                {

                    soundEffect = Microsoft.Xna.Framework.Audio.SoundEffect.FromStream(new MemoryStream(data));
                }
        }

        public float[] Platform_GetData() => data;
    }
}
