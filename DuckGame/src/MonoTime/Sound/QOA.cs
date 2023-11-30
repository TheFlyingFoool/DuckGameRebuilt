using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//https://github.com/pfusik/qoa-fu
namespace DuckGame
{
	// Generated automatically with "fut". Do not edit.

	/// <summary>Least Mean Squares Filter.</summary>
	class LMS
	{

		internal readonly int[] History = new int[4];

		internal readonly int[] Weights = new int[4];

		internal void Assign(LMS source)
		{
			Array.Copy(source.History, 0, History, 0, 4);
			Array.Copy(source.Weights, 0, Weights, 0, 4);
		}

		internal int Predict() => (History[0] * Weights[0] + History[1] * Weights[1] + History[2] * Weights[2] + History[3] * Weights[3]) >> 13;

		internal void Update(int sample, int residual)
		{
			int delta = residual >> 4;
			Weights[0] += History[0] < 0 ? -delta : delta;
			Weights[1] += History[1] < 0 ? -delta : delta;
			Weights[2] += History[2] < 0 ? -delta : delta;
			Weights[3] += History[3] < 0 ? -delta : delta;
			History[0] = History[1];
			History[1] = History[2];
			History[2] = History[3];
			History[3] = sample;
		}
	}

	/// <summary>Common part of the "Quite OK Audio" format encoder and decoder.</summary>
	public abstract class QOABase
	{

		protected static int Clamp(int value, int min, int max) => value < min ? min : value > max ? max : value;

		protected int FrameHeader;

		/// <summary>Maximum number of channels supported by the format.</summary>
		public const int MaxChannels = 8;

		/// <summary>Returns the number of audio channels.</summary>
		public int GetChannels() => FrameHeader >> 24;

		/// <summary>Returns the sample rate in Hz.</summary>
		public int GetSampleRate() => FrameHeader & 16777215;

		protected const int SliceSamples = 20;

		protected const int MaxFrameSlices = 256;

		/// <summary>Maximum number of samples per frame.</summary>
		public const int MaxFrameSamples = 5120;

		protected int GetFrameBytes(int sampleCount)
		{
			int slices = (sampleCount + 19) / 20;
			return 8 + GetChannels() * (16 + slices * 8);
		}

		protected static readonly short[] ScaleFactors = { 1, 7, 21, 45, 84, 138, 211, 304, 421, 562, 731, 928, 1157, 1419, 1715, 2048 };

		protected static int Dequantize(int quantized, int scaleFactor)
		{
			int dequantized;
			switch (quantized >> 1)
			{
				case 0:
					dequantized = (scaleFactor * 3 + 2) >> 2;
					break;
				case 1:
					dequantized = (scaleFactor * 5 + 1) >> 1;
					break;
				case 2:
					dequantized = (scaleFactor * 9 + 1) >> 1;
					break;
				default:
					dequantized = scaleFactor * 7;
					break;
			}
			return (quantized & 1) != 0 ? -dequantized : dequantized;
		}
	}

	/// <summary>Encoder of the "Quite OK Audio" format.</summary>
	public abstract class QOAEncoder : QOABase
	{
		internal QOAEncoder()
		{
			for (int _i0 = 0; _i0 < 8; _i0++)
			{
				LMSes[_i0] = new LMS();
			}
		}

		/// <summary>Writes the 64-bit integer in big endian order.</summary>
		/// <remarks>Returns <see langword="true" /> on success.</remarks>
		/// <param name="l">The integer to be written to the QOA stream.</param>
		protected abstract bool WriteLong(long l);

		readonly LMS[] LMSes = new LMS[8];

		/// <summary>Writes the file header.</summary>
		/// <remarks>Returns <see langword="true" /> on success.</remarks>
		/// <param name="totalSamples">File length in samples per channel.</param>
		/// <param name="channels">Number of audio channels.</param>
		/// <param name="sampleRate">Sample rate in Hz.</param>
		public bool WriteHeader(int totalSamples, int channels, int sampleRate)
		{
			if (totalSamples <= 0 || channels <= 0 || channels > 8 || sampleRate <= 0 || sampleRate >= 16777216)
				return false;
			FrameHeader = channels << 24 | sampleRate;
			for (int c = 0; c < channels; c++)
			{
				Array.Clear(LMSes[c].History, 0, 4);
				LMSes[c].Weights[0] = 0;
				LMSes[c].Weights[1] = 0;
				LMSes[c].Weights[2] = -8192;
				LMSes[c].Weights[3] = 16384;
			}
			long magic = 1903124838;
			return WriteLong(magic << 32 | totalSamples);
		}

		bool WriteLMS(int[] a)
		{
			long a0 = a[0];
			long a1 = a[1];
			long a2 = a[2];
			return WriteLong(a0 << 48 | (a1 & 65535) << 32 | (a2 & 65535) << 16 | (a[3] & 65535));
		}

		/// <summary>Encodes and writes a frame.</summary>
		/// <param name="samples">PCM samples: <c>samplesCount * channels</c> elements.</param>
		/// <param name="samplesCount">Number of samples per channel.</param>
		public bool WriteFrame(short[] samples, int samplesCount)
		{
			if (samplesCount <= 0 || samplesCount > 5120)
				return false;
			long header = FrameHeader;
			if (!WriteLong(header << 32 | samplesCount << 16 | GetFrameBytes(samplesCount)))
				return false;
			int channels = GetChannels();
			for (int c = 0; c < channels; c++)
			{
				if (!WriteLMS(LMSes[c].History) || !WriteLMS(LMSes[c].Weights))
					return false;
			}
			LMS lms = new LMS();
			LMS bestLMS = new LMS();
			for (int sampleIndex = 0; sampleIndex < samplesCount; sampleIndex += 20)
			{
				int sliceSamples = samplesCount - sampleIndex;
				if (sliceSamples > 20)
					sliceSamples = 20;
				for (int c = 0; c < channels; c++)
				{
					long bestError = 9223372036854775807;
					long bestSlice = 0;
					for (int scaleFactor = 0; scaleFactor < 16; scaleFactor++)
					{
						lms.Assign(LMSes[c]);
						int reciprocal = WriteFramereciprocals[scaleFactor];
						long slice = scaleFactor;
						long currentError = 0;
						for (int s = 0; s < sliceSamples; s++)
						{
							int sample = samples[(sampleIndex + s) * channels + c];
							int predicted = lms.Predict();
							int residual = sample - predicted;
							int scaled = (residual * reciprocal + 32768) >> 16;
							if (scaled != 0)
								scaled += scaled < 0 ? 1 : -1;
							if (residual != 0)
								scaled += residual > 0 ? 1 : -1;
							int quantized = WriteFramequantTab[8 + Clamp(scaled, -8, 8)];
							int dequantized = Dequantize(quantized, ScaleFactors[scaleFactor]);
							int reconstructed = Clamp(predicted + dequantized, -32768, 32767);
							long error = sample - reconstructed;
							currentError += error * error;
							if (currentError >= bestError)
								break;
							lms.Update(reconstructed, dequantized);
							slice = slice << 3 | quantized;
						}
						if (currentError < bestError)
						{
							bestError = currentError;
							bestSlice = slice;
							bestLMS.Assign(lms);
						}
					}
					LMSes[c].Assign(bestLMS);
					bestSlice <<= (20 - sliceSamples) * 3;
					if (!WriteLong(bestSlice))
						return false;
				}
			}
			return true;
		}

		static readonly int[] WriteFramereciprocals = { 65536, 9363, 3121, 1457, 781, 475, 311, 216, 156, 117, 90, 71, 57, 47, 39, 32 };

		static readonly byte[] WriteFramequantTab = { 7, 7, 7, 5, 5, 3, 3, 1, 0, 0, 2, 2, 4, 4, 6, 6,
		6 };
	}

	/// <summary>Decoder of the "Quite OK Audio" format.</summary>
	public abstract class QOADecoder : QOABase
	{

		/// <summary>Reads a byte from the stream.</summary>
		/// <remarks>Returns the unsigned byte value or -1 on EOF.</remarks>
		protected abstract int ReadByte();

		/// <summary>Seeks the stream to the given position.</summary>
		/// <param name="position">File offset in bytes.</param>
		protected abstract void SeekToByte(int position);

		int Buffer;

		int BufferBits;

		int ReadBits(int bits)
		{
			while (BufferBits < bits)
			{
				int b = ReadByte();
				if (b < 0)
					return -1;
				Buffer = Buffer << 8 | b;
				BufferBits += 8;
			}
			BufferBits -= bits;
			int result = Buffer >> BufferBits;
			Buffer &= (1 << BufferBits) - 1;
			return result;
		}

		int TotalSamples;

		int PositionSamples;

		/// <summary>Reads the file header.</summary>
		/// <remarks>Returns <see langword="true" /> if the header is valid.</remarks>
		public bool ReadHeader()
		{
			if (ReadByte() != 'q' || ReadByte() != 'o' || ReadByte() != 'a' || ReadByte() != 'f')
				return false;
			BufferBits = Buffer = 0;
			TotalSamples = ReadBits(32);
			if (TotalSamples <= 0)
				return false;
			FrameHeader = ReadBits(32);
			if (FrameHeader <= 0)
				return false;
			PositionSamples = 0;
			int channels = GetChannels();
			return channels > 0 && channels <= 8 && GetSampleRate() > 0;
		}

		/// <summary>Returns the file length in samples per channel.</summary>
		public int GetTotalSamples() => TotalSamples;

		int GetMaxFrameBytes() => 8 + GetChannels() * 2064;

		bool ReadLMS(int[] result)
		{
			for (int i = 0; i < 4; i++)
			{
				int hi = ReadByte();
				if (hi < 0)
					return false;
				int lo = ReadByte();
				if (lo < 0)
					return false;
				result[i] = ((hi ^ 128) - 128) << 8 | lo;
			}
			return true;
		}

		/// <summary>Reads and decodes a frame.</summary>
		/// <remarks>Returns the number of samples per channel.</remarks>
		/// <param name="samples">PCM samples.</param>
		public int ReadFrame(short[] samples)
		{
			if (PositionSamples > 0 && ReadBits(32) != FrameHeader)
				return -1;
			int samplesCount = ReadBits(16);
			if (samplesCount <= 0 || samplesCount > 5120 || samplesCount > TotalSamples - PositionSamples)
				return -1;
			int channels = GetChannels();
			int slices = (samplesCount + 19) / 20;
			if (ReadBits(16) != 8 + channels * (16 + slices * 8))
				return -1;
			LMS[] lmses = new LMS[8];
			for (int _i0 = 0; _i0 < 8; _i0++)
			{
				lmses[_i0] = new LMS();
			}
			for (int c = 0; c < channels; c++)
			{
				if (!ReadLMS(lmses[c].History) || !ReadLMS(lmses[c].Weights))
					return -1;
			}
			for (int sampleIndex = 0; sampleIndex < samplesCount; sampleIndex += 20)
			{
				for (int c = 0; c < channels; c++)
				{
					int scaleFactor = ReadBits(4);
					if (scaleFactor < 0)
						return -1;
					scaleFactor = ScaleFactors[scaleFactor];
					int sampleOffset = sampleIndex * channels + c;
					for (int s = 0; s < 20; s++)
					{
						int quantized = ReadBits(3);
						if (quantized < 0)
							return -1;
						if (sampleIndex + s >= samplesCount)
							continue;
						int dequantized = Dequantize(quantized, scaleFactor);
						int reconstructed = Clamp(lmses[c].Predict() + dequantized, -32768, 32767);
						lmses[c].Update(reconstructed, dequantized);
						samples[sampleOffset] = (short)reconstructed;
						sampleOffset += channels;
					}
				}
			}
			PositionSamples += samplesCount;
			return samplesCount;
		}

		/// <summary>Seeks to the given time offset.</summary>
		/// <remarks>Requires the input stream to be seekable with <c>SeekToByte</c>.</remarks>
		/// <param name="position">Position from the beginning of the file.</param>
		public void SeekToSample(int position)
		{
			int frame = position / 5120;
			SeekToByte(frame == 0 ? 12 : 8 + frame * GetMaxFrameBytes());
			PositionSamples = frame * 5120;
		}

		/// <summary>Returns <see langword="true" /> if all frames have been read.</summary>
		public bool IsEnd() => PositionSamples >= TotalSamples;
	}

	public class QOASoundBase : QOADecoder
	{
		//TODO this eventually, i am clueless about it right now -NiK0
		public byte[] data;
		public int position;
		protected override void SeekToByte(int position)
		{
			this.position = position;
		}
		public bool Load(byte[] b)
		{
			data = b;
			if (ReadHeader())
			{
				return true;
			}
			return false;
		}
        protected override int ReadByte()
        {
			return data[position++];
        }
    } 
}
