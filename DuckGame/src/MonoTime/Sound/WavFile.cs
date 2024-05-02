using System.IO;

namespace DuckGame
{
    public class WavFile
    {
        private WavHeader _header;
        private short[][] _stereoData = new short[2][];
        private string _fileName = "";

        public short[][] stereoData => _stereoData;

        public int size => (int)(_header.dataSize / _header.blockSize);

        public int sampleRate => (int)_header.sampleRate;

        public WavFile(string file)
        {
            _fileName = file;
            _header = new WavHeader();
            using (FileStream input = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader binaryReader = new BinaryReader(input))
                {
                    try
                    {
                        _header.riffID = binaryReader.ReadBytes(4);
                        _header.size = binaryReader.ReadUInt32();
                        _header.wavID = binaryReader.ReadBytes(4);
                        _header.fmtID = binaryReader.ReadBytes(4);
                        _header.fmtSize = binaryReader.ReadUInt32();
                        _header.format = binaryReader.ReadUInt16();
                        _header.channels = binaryReader.ReadUInt16();
                        _header.sampleRate = binaryReader.ReadUInt32();
                        _header.bytePerSec = binaryReader.ReadUInt32();
                        _header.blockSize = binaryReader.ReadUInt16();
                        _header.bit = binaryReader.ReadUInt16();
                        while (true)
                        {
                            _header.dataID = binaryReader.ReadBytes(4);
                            _header.dataSize = binaryReader.ReadUInt32();
                            if (_header.dataID[0] != 100)
                                binaryReader.ReadBytes((int)_header.dataSize);
                            else
                                break;
                        }
                        if (_header.channels == 1)
                        {
                            uint length = _header.dataSize / _header.blockSize;
                            _stereoData[0] = new short[(int)length];
                            for (int index = 0; index < length; ++index)
                                _stereoData[0][index] = (short)binaryReader.ReadUInt16();
                        }
                        else
                        {
                            if (_header.channels != 2)
                                return;
                            uint length = _header.dataSize / _header.blockSize;
                            _stereoData[0] = new short[(int)length];
                            _stereoData[1] = new short[(int)length];
                            for (int index = 0; index < length; ++index)
                            {
                                _stereoData[0][index] = (short)binaryReader.ReadUInt16();
                                _stereoData[1][index] = (short)binaryReader.ReadUInt16();
                            }
                        }
                    }
                    finally
                    {
                        binaryReader?.Close();
                        input?.Close();
                    }
                }
            }
        }
    }
}
