//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Security.Cryptography;

//namespace XnaToFna.XEX
//{
//  public class XEXImageData
//  {
//    public const uint XEX2_SECTION_LENGTH = 65536;
//    private static readonly byte[] IV = new byte[16];
//    private static readonly byte[] xe_xex2_retail_key = new byte[16]
//    {
//      (byte) 32,
//      (byte) 177,
//      (byte) 133,
//      (byte) 165,
//      (byte) 157,
//      (byte) 40,
//      (byte) 253,
//      (byte) 195,
//      (byte) 64,
//      (byte) 88,
//      (byte) 63,
//      (byte) 187,
//      (byte) 8,
//      (byte) 150,
//      (byte) 191,
//      (byte) 145
//    };
//    private static readonly byte[] xe_xex2_devkit_key = new byte[16];
//    public XEXImageData.XEXHeader header;
//    public XEXImageData.XEXSystemFlags system_flags;
//    public XEXImageData.XEXExecutionInfo execution_info;
//    public XEXImageData.XEXRatings game_ratings;
//    public XEXImageData.XEXTLSInfo tls_info;
//    public XEXImageData.XEXFileFormat file_format_info = new XEXImageData.XEXFileFormat();
//    public XEXImageData.XEXLoaderInfo loader_info;
//    public byte[] session_key = new byte[16];
//    public uint exe_address;
//    public uint exe_entry_point;
//    public uint exe_stack_size;
//    public uint exe_heap_size;
//    public List<XEXImageData.XEXResourceInfo> resources = new List<XEXImageData.XEXResourceInfo>();
//    public List<XEXImageData.XEXOptionalHeader> optional_headers = new List<XEXImageData.XEXOptionalHeader>();
//    public List<XEXImageData.XEXSection> sections = new List<XEXImageData.XEXSection>();
//    public List<uint> import_records = new List<uint>();
//    public byte[] m_memoryData;
//    public int m_memorySize;

//    public XEXImageData(BinaryReader reader)
//    {
//      this.LoadHeaders(reader);
//      this.LoadImageData(reader);
//    }

//    private void LoadHeaders(BinaryReader reader)
//    {
//      this.header = new XEXImageData.XEXHeader(reader);
//      if (this.header.xex2 != 1480939570U)
//        throw new InvalidDataException(string.Format("File not a XEX2 file - magic numbers in file: 0x{0}", (object) this.header.xex2.ToString("X8")));
//      for (uint index = 0; index < this.header.header_count; ++index)
//      {
//        XEXImageData.XEXOptionalHeader xexOptionalHeader = new XEXImageData.XEXOptionalHeader(reader);
//        bool flag = true;
//        switch ((uint) (xexOptionalHeader.key & (XEXImageData.XEXHeaderKeys) 255))
//        {
//          case 0:
//          case 1:
//            xexOptionalHeader.value = xexOptionalHeader.offset;
//            xexOptionalHeader.offset = 0U;
//            break;
//          case (uint) byte.MaxValue:
//            long position = reader.BaseStream.Position;
//            reader.BaseStream.Seek((long) xexOptionalHeader.offset, SeekOrigin.Begin);
//            xexOptionalHeader.length = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//            reader.BaseStream.Seek(position, SeekOrigin.Begin);
//            xexOptionalHeader.offset += 4U;
//            if ((long) (xexOptionalHeader.length + xexOptionalHeader.offset) >= reader.BaseStream.Length)
//              throw new InvalidDataException(string.Format("Optional header {0} (0x{1}) crosses file boundary. Will not be read.", (object) index, (object) xexOptionalHeader.key.ToString("X8")));
//            break;
//          default:
//            xexOptionalHeader.length = (uint) (xexOptionalHeader.key & (XEXImageData.XEXHeaderKeys) 255) * 4U;
//            if ((long) (xexOptionalHeader.length + xexOptionalHeader.offset) >= reader.BaseStream.Length)
//              throw new InvalidDataException(string.Format("Optional header {0} (0x{1}) crosses file boundary. Will not be read.", (object) index, (object) xexOptionalHeader.key.ToString("X8")));
//            break;
//        }
//        if (flag)
//          this.optional_headers.Add(xexOptionalHeader);
//      }
//      for (int index1 = 0; index1 < this.optional_headers.Count; ++index1)
//      {
//        XEXImageData.XEXOptionalHeader optionalHeader = this.optional_headers[index1];
//        if (optionalHeader.length > 0U && optionalHeader.offset != 0U)
//          reader.BaseStream.Seek((long) optionalHeader.offset, SeekOrigin.Begin);
//        switch (optionalHeader.key)
//        {
//          case XEXImageData.XEXHeaderKeys.XEX_HEADER_RESOURCE_INFO:
//            uint num1 = (optionalHeader.length - 4U) / 16U;
//            this.resources.Clear();
//            for (uint index2 = 0; index2 < num1; ++index2)
//              this.resources.Add(new XEXImageData.XEXResourceInfo(reader));
//            break;
//          case XEXImageData.XEXHeaderKeys.XEX_HEADER_FILE_FORMAT_INFO:
//            XEXImageData.XEXEncryptionHeader encryptionHeader = new XEXImageData.XEXEncryptionHeader(reader);
//            this.file_format_info.encryption_type = encryptionHeader.encryption_type;
//            this.file_format_info.compression_type = encryptionHeader.compression_type;
//            switch (encryptionHeader.compression_type)
//            {
//              case XEXImageData.XEXCompressionType.XEX_COMPRESSION_NONE:
//                Console.WriteLine("XEX: image::Binary is using no compression");
//                break;
//              case XEXImageData.XEXCompressionType.XEX_COMPRESSION_BASIC:
//                uint num2 = (optionalHeader.length - 8U) / 8U;
//                this.file_format_info.basic_blocks.Clear();
//                for (uint index3 = 0; index3 < num2; ++index3)
//                  this.file_format_info.basic_blocks.Add(new XEXImageData.XEXFileBasicCompressionBlock(reader));
//                Console.WriteLine("XEX: image::Binary is using basic compression with {0} blocks", (object) num2);
//                break;
//              case XEXImageData.XEXCompressionType.XEX_COMPRESSION_NORMAL:
//                this.file_format_info.normal = new XEXImageData.XEXFileNormalCompressionInfo(reader);
//                Console.WriteLine("XEX: image::Binary is using normal compression with block size = {0}", (object) this.file_format_info.normal.block_size);
//                break;
//              case XEXImageData.XEXCompressionType.XEX_COMPRESSION_DELTA:
//                Console.WriteLine("XEX: image::Binary is using unsupported delta compression");
//                break;
//            }
//            if (encryptionHeader.encryption_type != XEXImageData.XEXEncryptionType.XEX_ENCRYPTION_NONE)
//            {
//              Console.WriteLine("XEX: image::Binary is encrypted");
//              break;
//            }
//            break;
//          case XEXImageData.XEXHeaderKeys.XEX_HEADER_ENTRY_POINT:
//            this.exe_entry_point = optionalHeader.value;
//            Console.WriteLine("XEX: Found entry point: 0x{0}", (object) this.exe_entry_point.ToString("X8"));
//            break;
//          case XEXImageData.XEXHeaderKeys.XEX_HEADER_IMAGE_BASE_ADDRESS:
//            this.exe_address = optionalHeader.value;
//            Console.WriteLine("XEX: Found base addrses: 0x{0}", (object) this.exe_address.ToString("X8"));
//            break;
//          case XEXImageData.XEXHeaderKeys.XEX_HEADER_IMPORT_LIBRARIES:
//            XEXImageData.XEXImportLibraryBlockHeader libraryBlockHeader = new XEXImageData.XEXImportLibraryBlockHeader(reader);
//            long position = reader.BaseStream.Position;
//            reader.BaseStream.Seek((long) libraryBlockHeader.string_table_size, SeekOrigin.Current);
//            for (uint index4 = 0; index4 < libraryBlockHeader.count; ++index4)
//            {
//              XEXImageData.XEXImportLibraryHeader importLibraryHeader = new XEXImageData.XEXImportLibraryHeader(reader);
//              for (uint index5 = 0; index5 < (uint) importLibraryHeader.record_count; ++index5)
//                this.import_records.Add(ContentHelper.SwapEndian(true, reader.ReadUInt32()));
//            }
//            break;
//          case XEXImageData.XEXHeaderKeys.XEX_HEADER_TLS_INFO:
//            this.tls_info = new XEXImageData.XEXTLSInfo(reader);
//            break;
//          case XEXImageData.XEXHeaderKeys.XEX_HEADER_DEFAULT_STACK_SIZE:
//            this.exe_stack_size = optionalHeader.value;
//            break;
//          case XEXImageData.XEXHeaderKeys.XEX_HEADER_DEFAULT_HEAP_SIZE:
//            this.exe_heap_size = optionalHeader.value;
//            break;
//          case XEXImageData.XEXHeaderKeys.XEX_HEADER_SYSTEM_FLAGS:
//            this.system_flags = (XEXImageData.XEXSystemFlags) optionalHeader.value;
//            break;
//          case XEXImageData.XEXHeaderKeys.XEX_HEADER_EXECUTION_INFO:
//            this.execution_info = new XEXImageData.XEXExecutionInfo(reader);
//            break;
//        }
//      }
//      reader.BaseStream.Seek((long) this.header.certificate_offset, SeekOrigin.Begin);
//      this.loader_info = new XEXImageData.XEXLoaderInfo(reader);
//      Console.WriteLine("XEX: Binary size: 0x{0}", (object) this.loader_info.image_size.ToString("X8"));
//      reader.BaseStream.Seek((long) (this.header.certificate_offset + 384U), SeekOrigin.Begin);
//      uint num = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//      for (uint index = 0; index < num; ++index)
//        this.sections.Add(new XEXImageData.XEXSection(reader));
//      byte[] numArray = XEXImageData.xe_xex2_devkit_key;
//      if (this.execution_info.title_id != 0U)
//      {
//        Console.WriteLine("XEX: Found TitleID 0x{0}", (object) this.execution_info.title_id.ToString("X8"));
//        numArray = XEXImageData.xe_xex2_retail_key;
//      }
//      using (Aes aes = (Aes) new AesManaged())
//      {
//        aes.Mode = CipherMode.CBC;
//        aes.BlockSize = 128;
//        aes.KeySize = 128;
//        aes.Key = numArray;
//        aes.Padding = PaddingMode.None;
//        aes.IV = XEXImageData.IV;
//        using (ICryptoTransform decryptor = aes.CreateDecryptor())
//        {
//          this.session_key = new byte[16];
//          decryptor.TransformBlock(this.loader_info.file_key, 0, 16, this.session_key, 0);
//        }
//      }
//    }

//    private void LoadImageDataUncompressed(BinaryReader data)
//    {
//      int length = (int) (data.BaseStream.Length - (long) this.header.exe_offset);
//      if (length >= 134217728)
//        throw new InvalidDataException(string.Format("Computed image size is to big (0x{0}), the exe offset = 0x{1}", (object) length.ToString("X8"), (object) this.header.exe_offset.ToString("X8")));
//      data.BaseStream.Seek((long) this.header.exe_offset, SeekOrigin.Begin);
//      byte[] inputData = data.ReadBytes(length);
//      if (this.file_format_info.encryption_type == XEXImageData.XEXEncryptionType.XEX_ENCRYPTION_NONE)
//      {
//        this.m_memoryData = inputData;
//        this.m_memorySize = length;
//      }
//      else
//      {
//        byte[] outputData = new byte[length];
//        this.DecryptBuffer(this.session_key, inputData, length, outputData, length);
//        this.m_memoryData = outputData;
//        this.m_memorySize = length;
//      }
//    }

//    private void LoadImageDataNormal(BinaryReader data)
//    {
//      int length1 = (int) (data.BaseStream.Length - (long) this.header.exe_offset);
//      data.BaseStream.Seek((long) this.header.exe_offset, SeekOrigin.Begin);
//      byte[] inputData = data.ReadBytes(length1);
//      int outputSize = length1;
//      byte[] numArray1 = inputData;
//      if (this.file_format_info.encryption_type == XEXImageData.XEXEncryptionType.XEX_ENCRYPTION_NORMAL)
//      {
//        numArray1 = new byte[length1];
//        this.DecryptBuffer(this.session_key, inputData, length1, numArray1, outputSize);
//      }
//      byte[] numArray2 = new byte[outputSize];
//      int outLen = 0;
//      int startIndex = 0;
//      int destinationIndex = 0;
//      int num1;
//      for (int index1 = (int) this.file_format_info.normal.block_size; index1 != 0; index1 = num1)
//      {
//        int num2 = startIndex + index1;
//        num1 = (int) ContentHelper.SwapEndian(true, BitConverter.ToUInt32(numArray1, startIndex));
//        int index2 = startIndex + 4 + 20;
//        while (true)
//        {
//          int length2 = (int) numArray1[index2] << 8 | (int) numArray1[index2 + 1];
//          int sourceIndex = index2 + 2;
//          if (length2 != 0)
//          {
//            Array.Copy((Array) numArray1, sourceIndex, (Array) numArray2, destinationIndex, length2);
//            index2 = sourceIndex + length2;
//            destinationIndex += length2;
//            outLen += 32768;
//          }
//          else
//            break;
//        }
//        startIndex = num2;
//      }
//      int inLen = destinationIndex;
//      Console.WriteLine("Uncompressed image size: {0}", (object) outLen);
//      Console.WriteLine("Compressed image size: {0}", (object) inLen);
//      byte[] buffer = new byte[outLen];
//      LzxDecoder lzxDecoder = new LzxDecoder((int) this.file_format_info.normal.window_bits);
//      using (MemoryStream inData = new MemoryStream(numArray2))
//      {
//        using (MemoryStream outData = new MemoryStream(buffer))
//        {
//          int num3;
//          if ((num3 = lzxDecoder.Decompress((Stream) inData, inLen, (Stream) outData, outLen)) != 0)
//            throw new InvalidDataException(string.Format("Unable to decompress image data: {0}", (object) num3));
//        }
//      }
//      Console.WriteLine("Image data decompressed");
//      this.m_memoryData = buffer;
//      this.m_memorySize = outLen;
//    }

//    private void LoadImageDataBasic(BinaryReader data)
//    {
//      int length = 0;
//      int count1 = this.file_format_info.basic_blocks.Count;
//      for (int index = 0; index < count1; ++index)
//      {
//        XEXImageData.XEXFileBasicCompressionBlock basicBlock = this.file_format_info.basic_blocks[index];
//        length += (int) basicBlock.data_size + (int) basicBlock.zero_size;
//      }
//      int count2 = (int) (data.BaseStream.Length - (long) this.header.exe_offset);
//      data.BaseStream.Seek((long) this.header.exe_offset, SeekOrigin.Begin);
//      byte[] numArray1 = data.ReadBytes(count2);
//      int num1 = 0;
//      byte[] numArray2 = length < 134217728 ? new byte[length] : throw new InvalidDataException(string.Format("Computed image size is to big (0x{0}), the exe offset = 0x{1}", (object) length.ToString("X8"), (object) this.header.exe_offset.ToString("X8")));
//      byte[] numArray3 = numArray2;
//      int num2 = 0;
//      using (Aes aes = (Aes) new AesManaged())
//      {
//        aes.Mode = CipherMode.CBC;
//        aes.BlockSize = 128;
//        aes.KeySize = 128;
//        aes.Key = this.session_key;
//        aes.Padding = PaddingMode.None;
//        aes.IV = XEXImageData.IV;
//        using (ICryptoTransform decryptor = aes.CreateDecryptor())
//        {
//          for (int index = 0; index < count1; ++index)
//          {
//            XEXImageData.XEXFileBasicCompressionBlock basicBlock = this.file_format_info.basic_blocks[index];
//            uint dataSize = basicBlock.data_size;
//            uint zeroSize = basicBlock.zero_size;
//            switch (this.file_format_info.encryption_type)
//            {
//              case XEXImageData.XEXEncryptionType.XEX_ENCRYPTION_NONE:
//                Array.Copy((Array) numArray1, 0L, (Array) numArray3, 0L, (long) dataSize);
//                break;
//              case XEXImageData.XEXEncryptionType.XEX_ENCRYPTION_NORMAL:
//                int inputOffset = num1;
//                int outputOffset = num2;
//                int num3 = 0;
//                while ((long) num3 < (long) dataSize)
//                {
//                  decryptor.TransformBlock(numArray1, inputOffset, 16, numArray3, outputOffset);
//                  num3 += 16;
//                  inputOffset += 16;
//                  outputOffset += 16;
//                }
//                break;
//            }
//            num1 += (int) dataSize;
//            num2 += (int) dataSize + (int) zeroSize;
//          }
//        }
//      }
//      int num4 = num1;
//      if ((long) num4 > data.BaseStream.Length)
//        throw new InvalidDataException(string.Format("XEX: Too much source data was consumed by block decompression ({0} > {1})", (object) num4, (object) data.BaseStream.Length));
//      if ((long) num4 < data.BaseStream.Length)
//        Console.WriteLine("XEX: {0} bytes of data was not consumed in block decompression (out of {1})", (object) (data.BaseStream.Length - (long) num4), (object) data.BaseStream.Length);
//      int num5 = num2;
//      if (num5 > length)
//        throw new InvalidDataException(string.Format("XEX: Too much data was outputed in block decompression ({0} > {1})", (object) num5, (object) length));
//      if (num5 < length)
//        Console.WriteLine("XEX: {0} bytes of data was not outputed in block decompression (out of {1})", (object) (length - num5), (object) length);
//      this.m_memoryData = numArray2;
//      this.m_memorySize = length;
//    }

//    private void LoadImageData(BinaryReader data)
//    {
//      XEXImageData.XEXCompressionType compressionType = this.file_format_info.compression_type;
//      switch (compressionType)
//      {
//        case XEXImageData.XEXCompressionType.XEX_COMPRESSION_NONE:
//          Console.WriteLine("XEX: image::Binary is not compressed");
//          this.LoadImageDataUncompressed(data);
//          break;
//        case XEXImageData.XEXCompressionType.XEX_COMPRESSION_BASIC:
//          Console.WriteLine("XEX: image::Binary is using basic compression (zero blocks)");
//          this.LoadImageDataBasic(data);
//          break;
//        case XEXImageData.XEXCompressionType.XEX_COMPRESSION_NORMAL:
//          Console.WriteLine("XEX: image::Binary is using normal compression");
//          this.LoadImageDataNormal(data);
//          break;
//        default:
//          throw new NotSupportedException(string.Format("Image is using unsupported compression mode {0} and cannot be loaded", (object) compressionType));
//      }
//    }

//    private void DecryptBuffer(
//      byte[] key,
//      byte[] inputData,
//      int inputSize,
//      byte[] outputData,
//      int outputSize)
//    {
//      if (this.file_format_info.encryption_type == XEXImageData.XEXEncryptionType.XEX_ENCRYPTION_NONE)
//      {
//        if (inputSize != outputSize)
//          throw new InvalidDataException("inputSize != outputSize");
//        Array.Copy((Array) inputData, 0, (Array) outputData, 0, inputSize);
//      }
//      else
//      {
//        using (Aes aes = (Aes) new AesManaged())
//        {
//          aes.Mode = CipherMode.CBC;
//          aes.BlockSize = 128;
//          aes.KeySize = 128;
//          aes.Key = key;
//          aes.Padding = PaddingMode.None;
//          aes.IV = XEXImageData.IV;
//          using (ICryptoTransform decryptor = aes.CreateDecryptor())
//          {
//            int inputOffset = 0;
//            int outputOffset = 0;
//            int num = 0;
//            while (num < inputSize)
//            {
//              decryptor.TransformBlock(inputData, inputOffset, 16, outputData, outputOffset);
//              num += 16;
//              inputOffset += 16;
//              outputOffset += 16;
//            }
//          }
//        }
//      }
//    }

//    public class XEXRatings
//    {
//      public byte rating_esrb;
//      public byte rating_pegi;
//      public byte rating_pegifi;
//      public byte rating_pegipt;
//      public byte rating_bbfc;
//      public byte rating_cero;
//      public byte rating_usk;
//      public byte rating_oflcau;
//      public byte rating_oflcnz;
//      public byte rating_kmrb;
//      public byte rating_brazil;
//      public byte rating_fpb;

//      public XEXRatings(BinaryReader reader)
//      {
//        this.rating_esrb = reader.ReadByte();
//        this.rating_pegi = reader.ReadByte();
//        this.rating_pegifi = reader.ReadByte();
//        this.rating_pegipt = reader.ReadByte();
//        this.rating_bbfc = reader.ReadByte();
//        this.rating_cero = reader.ReadByte();
//        this.rating_usk = reader.ReadByte();
//        this.rating_oflcau = reader.ReadByte();
//        this.rating_oflcnz = reader.ReadByte();
//        this.rating_kmrb = reader.ReadByte();
//        this.rating_brazil = reader.ReadByte();
//        this.rating_fpb = reader.ReadByte();
//      }
//    }

//    public class XEXVersion
//    {
//      public uint value;

//      public XEXVersion(BinaryReader reader) => this.value = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//    }

//    public class XEXOptionalHeader
//    {
//      public uint offset;
//      public uint length;
//      public uint value;
//      public XEXImageData.XEXHeaderKeys key;

//      public XEXOptionalHeader(BinaryReader reader)
//      {
//        this.key = (XEXImageData.XEXHeaderKeys) ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        this.offset = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        this.length = 0U;
//        this.value = 0U;
//      }
//    }

//    public class XEXResourceInfo
//    {
//      public string name;
//      public uint address;
//      public uint size;

//      public XEXResourceInfo(BinaryReader reader)
//      {
//        this.name = new string(reader.ReadChars(8)).TrimEnd(new char[1]);
//        this.address = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        this.size = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//      }
//    }

//    public class XEXExecutionInfo
//    {
//      public uint media_id;
//      public XEXImageData.XEXVersion version;
//      public XEXImageData.XEXVersion base_version;
//      public uint title_id;
//      public byte platform;
//      public byte executable_table;
//      public byte disc_number;
//      public byte disc_count;
//      public uint savegame_id;

//      public XEXExecutionInfo(BinaryReader reader)
//      {
//        this.media_id = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        this.version = new XEXImageData.XEXVersion(reader);
//        this.base_version = new XEXImageData.XEXVersion(reader);
//        this.title_id = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        this.platform = reader.ReadByte();
//        this.executable_table = reader.ReadByte();
//        this.disc_number = reader.ReadByte();
//        this.disc_count = reader.ReadByte();
//        this.savegame_id = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//      }
//    }

//    public class XEXTLSInfo
//    {
//      public uint slot_count;
//      public uint raw_data_address;
//      public uint data_size;
//      public uint raw_data_size;

//      public XEXTLSInfo(BinaryReader reader)
//      {
//        this.slot_count = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        this.raw_data_address = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        this.data_size = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        this.raw_data_size = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//      }
//    }

//    public class XEXImportLibraryBlockHeader
//    {
//      public uint string_table_size;
//      public uint count;

//      public XEXImportLibraryBlockHeader(BinaryReader reader)
//      {
//        this.string_table_size = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        this.count = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//      }
//    }

//    public class XEXImportLibraryHeader
//    {
//      public uint unknown;
//      public byte[] digest = new byte[20];
//      public uint import_id;
//      public XEXImageData.XEXVersion version;
//      public XEXImageData.XEXVersion min_version;
//      public ushort name_index;
//      public ushort record_count;

//      public XEXImportLibraryHeader(BinaryReader reader)
//      {
//        this.unknown = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        reader.BaseStream.Read(this.digest, 0, this.digest.Length);
//        this.version = new XEXImageData.XEXVersion(reader);
//        this.min_version = new XEXImageData.XEXVersion(reader);
//        this.name_index = ContentHelper.SwapEndian(true, reader.ReadUInt16());
//        this.record_count = ContentHelper.SwapEndian(true, reader.ReadUInt16());
//      }
//    }

//    public class XEXStaticLibrary
//    {
//      public string name;
//      public ushort major;
//      public ushort minor;
//      public ushort build;
//      public ushort qfe;
//      public XEXImageData.XEXAprovalType approval;

//      public XEXStaticLibrary(BinaryReader reader)
//      {
//        this.name = new string(reader.ReadChars(8)).TrimEnd(new char[1]);
//        this.major = ContentHelper.SwapEndian(true, reader.ReadUInt16());
//        this.minor = ContentHelper.SwapEndian(true, reader.ReadUInt16());
//        this.build = ContentHelper.SwapEndian(true, reader.ReadUInt16());
//        this.qfe = ContentHelper.SwapEndian(true, reader.ReadUInt16());
//        this.approval = (XEXImageData.XEXAprovalType) ContentHelper.SwapEndian(true, reader.ReadUInt32());
//      }
//    }

//    public class XEXFileBasicCompressionBlock
//    {
//      public uint data_size;
//      public uint zero_size;

//      public XEXFileBasicCompressionBlock(BinaryReader reader)
//      {
//        this.data_size = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        this.zero_size = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//      }
//    }

//    public class XEXFileNormalCompressionInfo
//    {
//      public uint window_size;
//      public uint window_bits;
//      public uint block_size;
//      public byte[] block_hash = new byte[20];

//      public XEXFileNormalCompressionInfo(BinaryReader reader)
//      {
//        this.window_size = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        this.block_size = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        reader.BaseStream.Read(this.block_hash, 0, this.block_hash.Length);
//        uint windowSize = this.window_size;
//        uint num = 0;
//        while (num < 32U)
//        {
//          windowSize <<= 1;
//          if (windowSize == 2147483648U)
//            break;
//          ++num;
//          ++this.window_bits;
//        }
//      }
//    }

//    public class XEXEncryptionHeader
//    {
//      public XEXImageData.XEXEncryptionType encryption_type;
//      public XEXImageData.XEXCompressionType compression_type;

//      public XEXEncryptionHeader(BinaryReader reader)
//      {
//        this.encryption_type = (XEXImageData.XEXEncryptionType) ContentHelper.SwapEndian(true, reader.ReadUInt16());
//        this.compression_type = (XEXImageData.XEXCompressionType) ContentHelper.SwapEndian(true, reader.ReadUInt16());
//      }
//    }

//    public class XEXFileFormat
//    {
//      public XEXImageData.XEXEncryptionType encryption_type;
//      public XEXImageData.XEXCompressionType compression_type;
//      public List<XEXImageData.XEXFileBasicCompressionBlock> basic_blocks = new List<XEXImageData.XEXFileBasicCompressionBlock>();
//      public XEXImageData.XEXFileNormalCompressionInfo normal;
//    }

//    public class XEXLoaderInfo
//    {
//      public uint header_size;
//      public uint image_size;
//      public byte[] rsa_signature = new byte[256];
//      public uint unklength;
//      public uint image_flags;
//      public uint load_address;
//      public byte[] section_digest = new byte[20];
//      public uint import_table_count;
//      public byte[] import_table_digest = new byte[20];
//      public byte[] media_id = new byte[16];
//      public byte[] file_key = new byte[16];
//      public uint export_table;
//      public byte[] header_digest = new byte[20];
//      public uint game_regions;
//      public uint media_flags;

//      public XEXLoaderInfo(BinaryReader reader)
//      {
//        this.header_size = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        this.image_size = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        reader.BaseStream.Read(this.rsa_signature, 0, this.rsa_signature.Length);
//        this.unklength = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        this.image_flags = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        this.load_address = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        reader.BaseStream.Read(this.section_digest, 0, this.section_digest.Length);
//        this.import_table_count = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        reader.BaseStream.Read(this.import_table_digest, 0, this.import_table_digest.Length);
//        reader.BaseStream.Read(this.media_id, 0, this.media_id.Length);
//        reader.BaseStream.Read(this.file_key, 0, this.file_key.Length);
//        this.export_table = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        reader.BaseStream.Read(this.header_digest, 0, this.header_digest.Length);
//        this.game_regions = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        this.media_flags = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//      }
//    }

//    public class XEXSection
//    {
//      public uint info_value;
//      public byte[] digest = new byte[20];

//      public XEXSection(BinaryReader reader)
//      {
//        this.info_value = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        reader.BaseStream.Read(this.digest, 0, this.digest.Length);
//      }
//    }

//    public class XEXHeader
//    {
//      public uint xex2;
//      public uint module_flags;
//      public uint exe_offset;
//      public uint unknown0;
//      public uint certificate_offset;
//      public uint header_count;

//      public XEXHeader(BinaryReader reader)
//      {
//        this.xex2 = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        this.module_flags = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        this.exe_offset = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        this.unknown0 = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        this.certificate_offset = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//        this.header_count = ContentHelper.SwapEndian(true, reader.ReadUInt32());
//      }
//    }

//    public enum XEXHeaderKeys : uint
//    {
//      XEX_HEADER_RESOURCE_INFO = 767, // 0x000002FF
//      XEX_HEADER_FILE_FORMAT_INFO = 1023, // 0x000003FF
//      XEX_HEADER_BASE_REFERENCE = 1029, // 0x00000405
//      XEX_HEADER_DELTA_PATCH_DESCRIPTOR = 1535, // 0x000005FF
//      XEX_HEADER_BOUNDING_PATH = 33023, // 0x000080FF
//      XEX_HEADER_DEVICE_ID = 33029, // 0x00008105
//      XEX_HEADER_ORIGINAL_BASE_ADDRESS = 65537, // 0x00010001
//      XEX_HEADER_ENTRY_POINT = 65792, // 0x00010100
//      XEX_HEADER_IMAGE_BASE_ADDRESS = 66049, // 0x00010201
//      XEX_HEADER_IMPORT_LIBRARIES = 66559, // 0x000103FF
//      XEX_HEADER_CHECKSUM_TIMESTAMP = 98306, // 0x00018002
//      XEX_HEADER_ENABLED_FOR_CALLCAP = 98562, // 0x00018102
//      XEX_HEADER_ENABLED_FOR_FASTCAP = 98816, // 0x00018200
//      XEX_HEADER_ORIGINAL_PE_NAME = 99327, // 0x000183FF
//      XEX_HEADER_STATIC_LIBRARIES = 131327, // 0x000200FF
//      XEX_HEADER_TLS_INFO = 131332, // 0x00020104
//      XEX_HEADER_DEFAULT_STACK_SIZE = 131584, // 0x00020200
//      XEX_HEADER_DEFAULT_FILESYSTEM_CACHE_SIZE = 131841, // 0x00020301
//      XEX_HEADER_DEFAULT_HEAP_SIZE = 132097, // 0x00020401
//      XEX_HEADER_PAGE_HEAP_SIZE_AND_FLAGS = 163842, // 0x00028002
//      XEX_HEADER_SYSTEM_FLAGS = 196608, // 0x00030000
//      XEX_HEADER_EXECUTION_INFO = 262150, // 0x00040006
//      XEX_HEADER_TITLE_WORKSPACE_SIZE = 262657, // 0x00040201
//      XEX_HEADER_GAME_RATINGS = 262928, // 0x00040310
//      XEX_HEADER_LAN_KEY = 263172, // 0x00040404
//      XEX_HEADER_XBOX360_LOGO = 263679, // 0x000405FF
//      XEX_HEADER_MULTIDISC_MEDIA_IDS = 263935, // 0x000406FF
//      XEX_HEADER_ALTERNATE_TITLE_IDS = 264191, // 0x000407FF
//      XEX_HEADER_ADDITIONAL_TITLE_MEMORY = 264193, // 0x00040801
//      XEX_HEADER_EXPORTS_BY_NAME = 14746626, // 0x00E10402
//    }

//    [Flags]
//    public enum XEXModuleFlags : ushort
//    {
//      XEX_MODULE_TITLE = 1,
//      XEX_MODULE_EXPORTS_TO_TITLE = 2,
//      XEX_MODULE_SYSTEM_DEBUGGER = 4,
//      XEX_MODULE_DLL_MODULE = 8,
//      XEX_MODULE_MODULE_PATCH = 16, // 0x0010
//      XEX_MODULE_PATCH_FULL = 32, // 0x0020
//      XEX_MODULE_PATCH_DELTA = 64, // 0x0040
//      XEX_MODULE_USER_MODE = 128, // 0x0080
//    }

//    [Flags]
//    public enum XEXSystemFlags : uint
//    {
//      XEX_SYSTEM_NO_FORCED_REBOOT = 1,
//      XEX_SYSTEM_FOREGROUND_TASKS = 2,
//      XEX_SYSTEM_NO_ODD_MAPPING = 4,
//      XEX_SYSTEM_HANDLE_MCE_INPUT = 8,
//      XEX_SYSTEM_RESTRICTED_HUD_FEATURES = 16, // 0x00000010
//      XEX_SYSTEM_HANDLE_GAMEPAD_DISCONNECT = 32, // 0x00000020
//      XEX_SYSTEM_INSECURE_SOCKETS = 64, // 0x00000040
//      XEX_SYSTEM_XBOX1_INTEROPERABILITY = 128, // 0x00000080
//      XEX_SYSTEM_DASH_CONTEXT = 256, // 0x00000100
//      XEX_SYSTEM_USES_GAME_VOICE_CHANNEL = 512, // 0x00000200
//      XEX_SYSTEM_PAL50_INCOMPATIBLE = 1024, // 0x00000400
//      XEX_SYSTEM_INSECURE_UTILITY_DRIVE = 2048, // 0x00000800
//      XEX_SYSTEM_XAM_HOOKS = 4096, // 0x00001000
//      XEX_SYSTEM_ACCESS_PII = 8192, // 0x00002000
//      XEX_SYSTEM_CROSS_PLATFORM_SYSTEM_LINK = 16384, // 0x00004000
//      XEX_SYSTEM_MULTIDISC_SWAP = 32768, // 0x00008000
//      XEX_SYSTEM_MULTIDISC_INSECURE_MEDIA = 65536, // 0x00010000
//      XEX_SYSTEM_AP25_MEDIA = 131072, // 0x00020000
//      XEX_SYSTEM_NO_CONFIRM_EXIT = 262144, // 0x00040000
//      XEX_SYSTEM_ALLOW_BACKGROUND_DOWNLOAD = 524288, // 0x00080000
//      XEX_SYSTEM_CREATE_PERSISTABLE_RAMDRIVE = 1048576, // 0x00100000
//      XEX_SYSTEM_INHERIT_PERSISTENT_RAMDRIVE = 2097152, // 0x00200000
//      XEX_SYSTEM_ALLOW_HUD_VIBRATION = 4194304, // 0x00400000
//      XEX_SYSTEM_ACCESS_UTILITY_PARTITIONS = 8388608, // 0x00800000
//      XEX_SYSTEM_IPTV_INPUT_SUPPORTED = 16777216, // 0x01000000
//      XEX_SYSTEM_PREFER_BIG_BUTTON_INPUT = 33554432, // 0x02000000
//      XEX_SYSTEM_ALLOW_EXTENDED_SYSTEM_RESERVATION = 67108864, // 0x04000000
//      XEX_SYSTEM_MULTIDISC_CROSS_TITLE = 134217728, // 0x08000000
//      XEX_SYSTEM_INSTALL_INCOMPATIBLE = 268435456, // 0x10000000
//      XEX_SYSTEM_ALLOW_AVATAR_GET_METADATA_BY_XUID = 536870912, // 0x20000000
//      XEX_SYSTEM_ALLOW_CONTROLLER_SWAPPING = 1073741824, // 0x40000000
//      XEX_SYSTEM_DASH_EXTENSIBILITY_MODULE = 2147483648, // 0x80000000
//    }

//    public enum XEXAprovalType : ushort
//    {
//      XEX_APPROVAL_UNAPPROVED,
//      XEX_APPROVAL_POSSIBLE,
//      XEX_APPROVAL_APPROVED,
//      XEX_APPROVAL_EXPIRED,
//    }

//    public enum XEXEncryptionType : ushort
//    {
//      XEX_ENCRYPTION_NONE,
//      XEX_ENCRYPTION_NORMAL,
//    }

//    public enum XEXCompressionType : ushort
//    {
//      XEX_COMPRESSION_NONE,
//      XEX_COMPRESSION_BASIC,
//      XEX_COMPRESSION_NORMAL,
//      XEX_COMPRESSION_DELTA,
//    }

//    [Flags]
//    public enum XEXImageFlags : uint
//    {
//      XEX_IMAGE_MANUFACTURING_UTILITY = 2,
//      XEX_IMAGE_MANUFACTURING_SUPPORT_TOOLS = 4,
//      XEX_IMAGE_XGD2_MEDIA_ONLY = 8,
//      XEX_IMAGE_CARDEA_KEY = 256, // 0x00000100
//      XEX_IMAGE_XEIKA_KEY = 512, // 0x00000200
//      XEX_IMAGE_USERMODE_TITLE = 1024, // 0x00000400
//      XEX_IMAGE_USERMODE_SYSTEM = 2048, // 0x00000800
//      XEX_IMAGE_ORANGE0 = 4096, // 0x00001000
//      XEX_IMAGE_ORANGE1 = 8192, // 0x00002000
//      XEX_IMAGE_ORANGE2 = 16384, // 0x00004000
//      XEX_IMAGE_IPTV_SIGNUP_APPLICATION = 65536, // 0x00010000
//      XEX_IMAGE_IPTV_TITLE_APPLICATION = 131072, // 0x00020000
//      XEX_IMAGE_KEYVAULT_PRIVILEGES_REQUIRED = 67108864, // 0x04000000
//      XEX_IMAGE_ONLINE_ACTIVATION_REQUIRED = 134217728, // 0x08000000
//      XEX_IMAGE_PAGE_SIZE_4KB = 268435456, // 0x10000000
//      XEX_IMAGE_REGION_FREE = 536870912, // 0x20000000
//      XEX_IMAGE_REVOCATION_CHECK_OPTIONAL = 1073741824, // 0x40000000
//      XEX_IMAGE_REVOCATION_CHECK_REQUIRED = 2147483648, // 0x80000000
//    }

//    [Flags]
//    public enum XEXMediaFlags : uint
//    {
//      XEX_MEDIA_HARDDISK = 1,
//      XEX_MEDIA_DVD_X2 = 2,
//      XEX_MEDIA_DVD_CD = 4,
//      XEX_MEDIA_DVD_5 = 8,
//      XEX_MEDIA_DVD_9 = 16, // 0x00000010
//      XEX_MEDIA_SYSTEM_FLASH = 32, // 0x00000020
//      XEX_MEDIA_MEMORY_UNIT = 128, // 0x00000080
//      XEX_MEDIA_USB_MASS_STORAGE_DEVICE = 256, // 0x00000100
//      XEX_MEDIA_NETWORK = 512, // 0x00000200
//      XEX_MEDIA_DIRECT_FROM_MEMORY = 1024, // 0x00000400
//      XEX_MEDIA_RAM_DRIVE = 2048, // 0x00000800
//      XEX_MEDIA_SVOD = 4096, // 0x00001000
//      XEX_MEDIA_INSECURE_PACKAGE = 16777216, // 0x01000000
//      XEX_MEDIA_SAVEGAME_PACKAGE = 33554432, // 0x02000000
//      XEX_MEDIA_LOCALLY_SIGNED_PACKAGE = 67108864, // 0x04000000
//      XEX_MEDIA_LIVE_SIGNED_PACKAGE = 134217728, // 0x08000000
//      XEX_MEDIA_XBOX_PACKAGE = 268435456, // 0x10000000
//    }

//    public enum XEXRegion : uint
//    {
//      XEX_REGION_NTSCU = 255, // 0x000000FF
//      XEX_REGION_NTSCJ_JAPAN = 256, // 0x00000100
//      XEX_REGION_NTSCJ_CHINA = 512, // 0x00000200
//      XEX_REGION_NTSCJ = 65280, // 0x0000FF00
//      XEX_REGION_PAL_AU_NZ = 65536, // 0x00010000
//      XEX_REGION_PAL = 16711680, // 0x00FF0000
//      XEX_REGION_OTHER = 4278190080, // 0xFF000000
//      XEX_REGION_ALL = 4294967295, // 0xFFFFFFFF
//    }

//    public enum XEXSectionType : ushort
//    {
//      XEX_SECTION_CODE = 1,
//      XEX_SECTION_DATA = 2,
//      XEX_SECTION_READONLY_DATA = 3,
//    }
//  }
//}
