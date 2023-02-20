using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagedBass;
using ManagedBass.Mix;
using System.Drawing;
using ManagedBass.Fx;

namespace DuckGame
{
    internal static class AndroidAudio
    {
        static int mixer;

        static AndroidAudio()
        {
            if(!Bass.Init(-1, 44100, DeviceInitFlags.Default, IntPtr.Zero))
            {
                throw new Exception(Bass.LastError.ToString());
            }
            mixer = BassMix.CreateMixerStream(44100, 2, BassFlags.Default);
            if(mixer == 0)
            {
                throw new Exception(Bass.LastError.ToString());
            }
            if(!Bass.ChannelPlay(mixer, true))
            {
                throw new Exception(Bass.LastError.ToString());
            }
            //BassMix.ChannelSetSync(mixer, SyncFlags.End, 0, SyncEvent);
        }

        public static void Kill()
        {
            Bass.Free();
        }

        public static SyncProcedure SyncEvent = Callback;

        static void Callback(int handle, int Channel, int Data, IntPtr User)
        {
            //int h = handle == mixer ? Channel : handle;
            if (!BassMix.ChannelSetPosition(Channel, 0))
            {
                throw new Exception("Callback " + Bass.LastError.ToString());
            }
            //if (!BassMix.MixerRemoveChannel(Handle))
            //{
            //    throw new Exception(Bass.LastError.ToString());
            //}
        }

        public static void Loop(int handle, bool loop)
        {
            if (loop) {
                if (!Bass.ChannelAddFlag(handle, BassFlags.Loop))
                {
                    throw new Exception(Bass.LastError.ToString());
                }
                return;
            }
            if (!Bass.ChannelRemoveFlag(handle, BassFlags.Loop))
            {
                throw new Exception(Bass.LastError.ToString());
            }
        }

        public static long GetLength(int handle, bool inSeconds = false)
        {
            long l = Bass.ChannelGetLength(handle);
            if (l == -1)
            {
                throw new Exception(Bass.LastError.ToString());
            }
            if (inSeconds)
            {
                l = (long)Bass.ChannelBytes2Seconds(handle, l);
                if (l == -1)
                {
                    throw new Exception(Bass.LastError.ToString());
                }
            }
            return l;
        }
        
        public static long GetPostion(int handle, bool inSeconds = false)
        {
            long p = Bass.ChannelGetPosition(handle);
            if(p == -1)
            {
                throw new Exception(Bass.LastError.ToString());
            }
            if (inSeconds)
            {
                p = (long)Bass.ChannelBytes2Seconds(handle, p);
                if (p == -1)
                {
                    throw new Exception(Bass.LastError.ToString());
                }
            }
            return p;
        }
        
        public static void SetPan(int handle, float pan)
        {
            if (!Bass.ChannelSetAttribute(handle, ChannelAttribute.Pan, pan))
            {
                throw new Exception(Bass.LastError.ToString());
            }
        }
        
        public static void SetVolume(int handle, float volume)
        {
            if (!Bass.ChannelSetAttribute(handle, ChannelAttribute.Volume, volume))
            {
                throw new Exception(Bass.LastError.ToString());
            }
        }

        public static void Pitch(int handle, float pitch)
        {
            if (!Bass.ChannelSetAttribute(handle, ChannelAttribute.Pitch, pitch))
            {
                throw new Exception(Bass.LastError.ToString());
            }
        }

        public static void Rewind(int handle)
        {
            if (!Bass.ChannelSetPosition(handle, 0))
            {
                throw new Exception(Bass.LastError.ToString());
            }
        }

        public static void Free(int handle)
        {
            if (!Bass.StreamFree(handle))
            {
                throw new Exception(Bass.LastError.ToString());
            }
        }

        static List<int> Channels = new List<int>();

        public static int GetSample(int handle)
        {
            handle = Bass.SampleGetChannel(handle, BassFlags.Decode | BassFlags.SampleChannelStream);
            //handle = Bass.SampleGetChannel(handle, BassFlags.Decode);
            if (handle == 0)
            {
                throw new Exception(Bass.LastError.ToString());
            }
            //var info = Bass.SampleGetInfo(handle);
            //var stream = Bass.CreateStream(info.Frequency, 2, info.Flags | BassFlags.Decode, StreamProcedureType.Push);
            //byte[] data = new byte[info.Length];
            //Bass.SampleGetData(handle, data);
            //Bass.StreamPutData(stream, data, info.Length);
            handle = BassFx.TempoCreate(handle, BassFlags.Decode);
            if (handle == 0)
            {
                throw new Exception(Bass.LastError.ToString());
            }
            return handle;
        }

        public static void Play(int handle, bool isMusic = false)
        {
            if (isMusic)
            {
                if (!Bass.ChannelPlay(handle))
                {
                    throw new Exception(Bass.LastError.ToString());
                }
                return;
            }
            //if (Channels.Contains(handle))
            //{
            //    if (!BassMix.ChannelSetPosition(handle, 0))
            //    {
            //        throw new Exception("Mixer " + Bass.LastError.ToString());
            //    }
            //    //if (!BassMix.MixerRemoveChannel(handle))
            //    //{
            //    //    throw new Exception("Mixer " + Bass.LastError.ToString());
            //    //}
            //    return;
            //}
            //handle = Bass.SampleGetChannel(handle, BassFlags.Decode);
            //if (handle == 0)
            //{
            //    throw new Exception(Bass.LastError.ToString());
            //}
            //handle = BassFx.TempoCreate(handle, BassFlags.Decode);
            //if (handle == 0)
            //{
            //    throw new Exception(Bass.LastError.ToString());
            //}
            if (!BassMix.MixerAddChannel(mixer, handle, BassFlags.Default))
            {
                throw new Exception("Mixer " + Bass.LastError.ToString());
            }
            //Channels.Add(handle);
            var state = Bass.ChannelIsActive(mixer);
            if(state != PlaybackState.Playing)
            {
                if (!Bass.ChannelPlay(mixer))
                {
                    throw new Exception("Mixer " +  Bass.LastError.ToString());
                }
            }
        }

        public static void Stop(int handle)
        {
            if (!Bass.ChannelStop(handle))
            {
                throw new Exception(Bass.LastError.ToString());
            }
        }
        
        public static void Pause(int handle)
        {
            Bass.ChannelPause(handle);
        }

        public static int FromFile(string path, bool isMusic = false)
        {
            if (isMusic)
            {
                var handle = Bass.CreateStream(path, 0, 0, BassFlags.Default);
                if (handle == 0)
                {
                    throw new Exception(Bass.LastError.ToString());
                }
                return handle;
            }
            //var shandle = Bass.SampleLoad(path, 0, 0, 5, BassFlags.Decode);
            var shandle = Bass.SampleLoad(path, 0, 0, 5, BassFlags.Decode);
            if (shandle == 0)
            {
                throw new Exception(Bass.LastError.ToString());
            }
            return shandle;
            //handle = BassFx.TempoCreate(handle, BassFlags.Decode);
            //if (handle == 0)
            //{
            //    throw new Exception(Bass.LastError.ToString());
            //}
            //if (!isMusic)
            //{
            //    Bass.ChannelSetSync(handle, SyncFlags.End, 0, SyncEvent);
            //}
            //return handle;
        }
    }
}