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

    public override bool CanRead => this.Input.CanRead;

    public override bool CanSeek => this.Input.CanSeek;

    public override bool CanWrite => this.Input.CanWrite;

    public override long Length => this.Input.Length;

    public override long Position
    {
      get => this.Input.Position;
      set => this.Seek(value, SeekOrigin.Begin);
    }

    public CopyingStream(Stream input, Stream output)
      : this(input, false, output, false)
    {
    }

    public CopyingStream(Stream input, bool leaveInputOpen, Stream output, bool leaveOutputOpen)
    {
      this.Input = input;
      this.LeaveInputOpen = leaveInputOpen;
      this.Output = output;
      this.LeaveOutputOpen = leaveOutputOpen;
    }

    public override void Flush()
    {
      this.Input.Flush();
      if (!this.Copy)
        return;
      this.Output.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      int count1 = this.Input.Read(buffer, offset, count);
      if (this.Copy)
        this.Output.Write(buffer, offset, count1);
      return count1;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      if (!this.Copy)
        return this.Input.Seek(offset, origin);
      long offset1 = 0;
      switch (origin)
      {
        case SeekOrigin.Begin:
          offset1 = offset;
          break;
        case SeekOrigin.Current:
          offset1 = this.Position + offset;
          break;
        case SeekOrigin.End:
          offset1 = this.Input.Length - offset;
          break;
      }
      if (offset1 == this.Position)
        return this.Position;
      if (offset1 < this.Position)
      {
        this.Output.Seek(offset1, SeekOrigin.Begin);
        return this.Input.Seek(offset, origin);
      }
      byte[] buffer = new byte[offset - this.Position];
      int offset2 = 0;
      while (offset2 < buffer.Length)
        offset2 += this.Input.Read(buffer, offset2, buffer.Length - offset2);
      this.Output.Write(buffer, 0, buffer.Length);
      return this.Position;
    }

    public override void SetLength(long value) => this.Input.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count)
    {
      this.Input.Write(buffer, offset, count);
      if (!this.Copy)
        return;
      this.Output.Write(buffer, offset, count);
    }

    protected override void Dispose(bool disposing)
    {
      if (!this.LeaveInputOpen)
        this.Input.Dispose();
      if (!this.LeaveOutputOpen)
        this.Output.Dispose();
      base.Dispose(disposing);
    }
  }
}
