// Decompiled with JetBrains decompiler
// Type: XnaToFna.ContentTransformers.GZipContentReader`1
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using Microsoft.Xna.Framework.Content;
using System.IO;
using System.IO.Compression;

namespace XnaToFna.ContentTransformers
{
    public class GZipContentReader<ContentType> : ContentTypeReader<ContentType>
    {
        private ForcedStreamContentManager WrappedContentManager;

        protected override ContentType Read(ContentReader input, ContentType existing)
        {
            if (WrappedContentManager == null)
                WrappedContentManager = new ForcedStreamContentManager(input.ContentManager.ServiceProvider);
            WrappedContentManager.RootDirectory = input.ContentManager.RootDirectory;
            WrappedContentManager.Stream = new GZipStream(input.BaseStream, CompressionMode.Decompress, true);
            bool enabled = FNAHooks.Enabled;
            FNAHooks.Enabled = false;
            ContentType contentType = WrappedContentManager.Load<ContentType>(input.AssetName);
            FNAHooks.Enabled = enabled;
            return contentType;
        }
    }
}
