// Decompiled with JetBrains decompiler
// Type: XnaToFna.ForcedStreamContentManager
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using Microsoft.Xna.Framework.Content;
using System;
using System.IO;

namespace XnaToFna
{
    public class ForcedStreamContentManager : ContentManager
    {
        public Stream Stream;

        public ForcedStreamContentManager(IServiceProvider serviceProvider)
          : base(serviceProvider)
        {
        }

        protected override Stream OpenStream(string assetName)
        {
            Stream stream;
            if (this.Stream != null)
            {
                stream = this.Stream;
                this.Stream = null;
            }
            else
                stream = base.OpenStream(assetName);
            return stream;
        }
    }
}
