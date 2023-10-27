using System;

namespace DuckGame
{
    public class MusicInstance : SoundEffectInstance
    {
        public MusicInstance(SoundEffect pData)
          : base(pData)
        {
            _isMusic = true;
        }

        public override int Read(float[] buffer, int offset, int count)
        {
            if (_data == null || !_inMixer)
                return 0;
            if (_volume <= 0)
                return count;
            int length = 0;
            lock (this)
            {
                if (_data.data == null)
                {
                    length = _data.Decode(buffer, offset, count);
                }
                else
                {
                    do
                        ;
                    while (_position + count > _data.decodedSamples && _data.Decoder_DecodeChunk());
                    length = Math.Min(count, _data.decodedSamples - _position);
                    Array.Copy(SoundEffect._songBuffer, _position, buffer, offset, length);
                }
                _position += length;
                if (length != count)
                {
                    if (SoundEndEvent != null)
                        SoundEndEvent();
                    if (_loop)
                    {
                        _position = 0;
                        offset += length;
                        if (_data.data == null)
                        {
                            _data.Rewind();
                            length = _data.Decode(buffer, offset, count);
                        }
                        else
                        {
                            length = Math.Min(SoundEffect._songBuffer.Length - _position, count - length);
                            Array.Copy(SoundEffect._songBuffer, _position, buffer, offset, length);
                        }
                        _position += length;
                        length = count;
                    }
                }
            }
            _inMixer = _inMixer && length == count;
            return length;
        }
    }
}
