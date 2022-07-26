// Decompiled with JetBrains decompiler
// Type: DuckGame.MusicInstance
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class MusicInstance : SoundEffectInstance
    {
        public MusicInstance(SoundEffect pData)
          : base(pData)
        {
            this._isMusic = true;
        }

        public override int Read(float[] buffer, int offset, int count)
        {
            if (this._data == null || !this._inMixer)
                return 0;
            if ((double)this._volume <= 0.0)
                return count;
            int length = 0;
            lock (this)
            {
                if (this._data.data == null)
                {
                    length = this._data.Decode(buffer, offset, count);
                }
                else
                {
                    do
                        ;
                    while (this._position + count > this._data.decodedSamples && this._data.Decoder_DecodeChunk());
                    length = Math.Min(count, this._data.decodedSamples - this._position);
                    Array.Copy((Array)SoundEffect._songBuffer, this._position, (Array)buffer, offset, length);
                }
                this._position += length;
                if (length != count)
                {
                    if (this.SoundEndEvent != null)
                        this.SoundEndEvent();
                    if (this._loop)
                    {
                        this._position = 0;
                        offset += length;
                        if (this._data.data == null)
                        {
                            this._data.Rewind();
                            length = this._data.Decode(buffer, offset, count);
                        }
                        else
                        {
                            length = Math.Min(SoundEffect._songBuffer.Length - this._position, count - length);
                            Array.Copy((Array)SoundEffect._songBuffer, this._position, (Array)buffer, offset, length);
                        }
                        this._position += length;
                        length = count;
                    }
                }
            }
            this._inMixer = this._inMixer && length == count;
            return length;
        }
    }
}
