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
            if (Stream != null)
            {
                stream = Stream;
                Stream = null;
            }
            else
                stream = base.OpenStream(assetName);
            return stream;
        }
    }
}
