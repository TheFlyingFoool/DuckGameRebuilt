using Microsoft.Xna.Framework.Content;
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
