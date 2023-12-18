// Decompiled with JetBrains decompiler
// Type: XnaToFna.CopyingStream
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using System.IO;

namespace XnaToFna
{
    public class CopyingStream : Stream
    {
        public Stream Input;
        public bool LeaveInputOpen;
        public Stream Output;
        public bool LeaveOutputOpen;
        public bool Copy = true;

        public override bool CanRead => Input.CanRead;

        public override bool CanSeek => Input.CanSeek;

        public override bool CanWrite => Input.CanWrite;

        public override long Length => Input.Length;

        public override long Position
        {
            get => Input.Position;
            set => Seek(value, SeekOrigin.Begin);
        }

        public CopyingStream(Stream input, Stream output)
          : this(input, false, output, false)
        {
        }

        public CopyingStream(Stream input, bool leaveInputOpen, Stream output, bool leaveOutputOpen)
        {
            Input = input;
            LeaveInputOpen = leaveInputOpen;
            Output = output;
            LeaveOutputOpen = leaveOutputOpen;
        }

        public override void Flush()
        {
            Input.Flush();
            if (!Copy)
                return;
            Output.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int count1 = Input.Read(buffer, offset, count);
            if (Copy)
                Output.Write(buffer, offset, count1);
            return count1;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (!Copy)
                return Input.Seek(offset, origin);
            long offset1 = 0;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    offset1 = offset;
                    break;
                case SeekOrigin.Current:
                    offset1 = Position + offset;
                    break;
                case SeekOrigin.End:
                    offset1 = Input.Length - offset;
                    break;
            }
            if (offset1 == Position)
                return Position;
            if (offset1 < Position)
            {
                Output.Seek(offset1, SeekOrigin.Begin);
                return Input.Seek(offset, origin);
            }
            byte[] buffer = new byte[offset - Position];
            int offset2 = 0;
            while (offset2 < buffer.Length)
                offset2 += Input.Read(buffer, offset2, buffer.Length - offset2);
            Output.Write(buffer, 0, buffer.Length);
            return Position;
        }

        public override void SetLength(long value) => Input.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count)
        {
            Input.Write(buffer, offset, count);
            if (!Copy)
                return;
            Output.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            if (!LeaveInputOpen)
                Input.Dispose();
            if (!LeaveOutputOpen)
                Output.Dispose();
            base.Dispose(disposing);
        }
    }
}
