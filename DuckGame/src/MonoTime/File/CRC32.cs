// Decompiled with JetBrains decompiler
// Type: DuckGame.CRC32
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.IO;
using System.Text;

namespace DuckGame
{
    public class CRC32
    {
        private static uint[] table = new uint[256]
        {
          0U,
          1996959894U,
          3993919788U,
          2567524794U,
          124634137U,
          1886057615U,
          3915621685U,
          2657392035U,
          249268274U,
          2044508324U,
          3772115230U,
          2547177864U,
          162941995U,
          2125561021U,
          3887607047U,
          2428444049U,
          498536548U,
          1789927666U,
          4089016648U,
          2227061214U,
          450548861U,
          1843258603U,
          4107580753U,
          2211677639U,
          325883990U,
          1684777152U,
          4251122042U,
          2321926636U,
          335633487U,
          1661365465U,
          4195302755U,
          2366115317U,
          997073096U,
          1281953886U,
          3579855332U,
          2724688242U,
          1006888145U,
          1258607687U,
          3524101629U,
          2768942443U,
          901097722U,
          1119000684U,
          3686517206U,
          2898065728U,
          853044451U,
          1172266101U,
          3705015759U,
          2882616665U,
          651767980U,
          1373503546U,
          3369554304U,
          3218104598U,
          565507253U,
          1454621731U,
          3485111705U,
          3099436303U,
          671266974U,
          1594198024U,
          3322730930U,
          2970347812U,
          795835527U,
          1483230225U,
          3244367275U,
          3060149565U,
          1994146192U,
          31158534U,
          2563907772U,
          4023717930U,
          1907459465U,
          112637215U,
          2680153253U,
          3904427059U,
          2013776290U,
          251722036U,
          2517215374U,
          3775830040U,
          2137656763U,
          141376813U,
          2439277719U,
          3865271297U,
          1802195444U,
          476864866U,
          2238001368U,
          4066508878U,
          1812370925U,
          453092731U,
          2181625025U,
          4111451223U,
          1706088902U,
          314042704U,
          2344532202U,
          4240017532U,
          1658658271U,
          366619977U,
          2362670323U,
          4224994405U,
          1303535960U,
          984961486U,
          2747007092U,
          3569037538U,
          1256170817U,
          1037604311U,
          2765210733U,
          3554079995U,
          1131014506U,
          879679996U,
          2909243462U,
          3663771856U,
          1141124467U,
          855842277U,
          2852801631U,
          3708648649U,
          1342533948U,
          654459306U,
          3188396048U,
          3373015174U,
          1466479909U,
          544179635U,
          3110523913U,
          3462522015U,
          1591671054U,
          702138776U,
          2966460450U,
          3352799412U,
          1504918807U,
          783551873U,
          3082640443U,
          3233442989U,
          3988292384U,
          2596254646U,
          62317068U,
          1957810842U,
          3939845945U,
          2647816111U,
          81470997U,
          1943803523U,
          3814918930U,
          2489596804U,
          225274430U,
          2053790376U,
          3826175755U,
          2466906013U,
          167816743U,
          2097651377U,
          4027552580U,
          2265490386U,
          503444072U,
          1762050814U,
          4150417245U,
          2154129355U,
          426522225U,
          1852507879U,
          4275313526U,
          2312317920U,
          282753626U,
          1742555852U,
          4189708143U,
          2394877945U,
          397917763U,
          1622183637U,
          3604390888U,
          2714866558U,
          953729732U,
          1340076626U,
          3518719985U,
          2797360999U,
          1068828381U,
          1219638859U,
          3624741850U,
          2936675148U,
          906185462U,
          1090812512U,
          3747672003U,
          2825379669U,
          829329135U,
          1181335161U,
          3412177804U,
          3160834842U,
          628085408U,
          1382605366U,
          3423369109U,
          3138078467U,
          570562233U,
          1426400815U,
          3317316542U,
          2998733608U,
          733239954U,
          1555261956U,
          3268935591U,
          3050360625U,
          752459403U,
          1541320221U,
          2607071920U,
          3965973030U,
          1969922972U,
          40735498U,
          2617837225U,
          3943577151U,
          1913087877U,
          83908371U,
          2512341634U,
          3803740692U,
          2075208622U,
          213261112U,
          2463272603U,
          3855990285U,
          2094854071U,
          198958881U,
          2262029012U,
          4057260610U,
          1759359992U,
          534414190U,
          2176718541U,
          4139329115U,
          1873836001U,
          414664567U,
          2282248934U,
          4279200368U,
          1711684554U,
          285281116U,
          2405801727U,
          4167216745U,
          1634467795U,
          376229701U,
          2685067896U,
          3608007406U,
          1308918612U,
          956543938U,
          2808555105U,
          3495958263U,
          1231636301U,
          1047427035U,
          2932959818U,
          3654703836U,
          1088359270U,
          936918000U,
          2847714899U,
          3736837829U,
          1202900863U,
          817233897U,
          3183342108U,
          3401237130U,
          1404277552U,
          615818150U,
          3134207493U,
          3453421203U,
          1423857449U,
          601450431U,
          3009837614U,
          3294710456U,
          1567103746U,
          711928724U,
          3020668471U,
          3272380065U,
          1510334235U,
          755167117U
        };
        private uint _crc = uint.MaxValue;

        public unsafe void ProcessBlock(byte* block, int blockLen)
        {
            for (int index = 0; index < blockLen; ++index)
                _crc = _crc >> 8 ^ table[((int)_crc ^ block[index]) & byte.MaxValue];
        }

        public void Initialize() => _crc = uint.MaxValue;

        public unsafe void ProcessBlock(byte[] block, int blockLen)
        {
            fixed (byte* block1 = block)
                ProcessBlock(block1, blockLen);
        }

        public unsafe void ProcessBlock(byte[] block, int blockStart, int blockLen)
        {
            fixed (byte* numPtr = block)
                ProcessBlock(numPtr + blockStart, blockLen);
        }

        public void ProcessBlock(byte[] block) => ProcessBlock(block, block.Length);

        public uint Finalize()
        {
            _crc = (uint)(_crc ^ ulong.MaxValue);
            if (_crc < 0U)
                _crc = _crc;
            int crc = (int)_crc;
            Initialize();
            return (uint)crc;
        }

        public static uint Generate(byte[] input)
        {
            CRC32 crC32 = new CRC32();
            crC32.ProcessBlock(input);
            return crC32.Finalize();
        }

        public static uint Generate(byte[] input, int start, int length)
        {
            if (input.Length < start || length < 1)
            {
                return 0;
            }
            length = Math.Min(input.Length - start, start + length);
            CRC32 crC32 = new CRC32();
            crC32.ProcessBlock(input, start, length);
            return crC32.Finalize();
        }

        public static unsafe uint Generate(byte* input, int len)
        {
            CRC32 crC32 = new CRC32();
            crC32.ProcessBlock(input, len);
            return crC32.Finalize();
        }

        public static uint Generate(Stream stream, int bufferLength)
        {
            CRC32 crC32 = new CRC32();
            byte[] numArray = new byte[bufferLength];
            while (stream.Position != stream.Length)
            {
                int blockLen = stream.Read(numArray, 0, bufferLength);
                crC32.ProcessBlock(numArray, blockLen);
            }
            return crC32.Finalize();
        }

        public static uint Generate(Stream stream) => Generate(stream, 2048);

        public static uint Generate(string str, Encoding encoding) => Generate(encoding.GetBytes(str));

        public static uint Generate(string str) => Generate(str, Encoding.Default);
    }
}
