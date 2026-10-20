namespace DuckGame
{
    /// <summary>
    /// Asks the sender of an image to start streaming it with <see cref="NMChatImageChunk"/>s.
    /// </summary>
    [ClientOnly]
    public class NMChatImageRequest : NMDuckNetworkEvent
    {
        public uint imageId;

        public NMChatImageRequest()
        {
        }

        public NMChatImageRequest(uint pImageId) => imageId = pImageId;

        public override void Activate() => ChatImages.OnRequest(connection, imageId);
    }
}
