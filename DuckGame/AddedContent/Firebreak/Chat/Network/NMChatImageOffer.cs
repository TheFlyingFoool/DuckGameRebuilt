namespace DuckGame
{
    /// <summary>
    /// Sent to DGR clients right before the <see cref="NMChatMessage"/> it belongs to, announcing an image that can be requested
    /// with <see cref="NMChatImageRequest"/>.
    /// </summary>
    [ClientOnly]
    public class NMChatImageOffer : NMDuckNetworkEvent
    {
        public Profile profile;
        public uint imageId;
        public ushort width;
        public ushort height;
        public int byteSize;
        /// <summary>The original file name without extension, so receivers can save it under the same name.</summary>
        public string fileName;

        public NMChatImageOffer()
        {
        }

        public NMChatImageOffer(Profile pProfile, ChatImageAttachment pImage)
        {
            profile = pProfile;
            imageId = pImage.id;
            width = (ushort)pImage.width;
            height = (ushort)pImage.height;
            byteSize = pImage.byteSize;
            fileName = ChatImages.SanitizeFileName(pImage.name);
        }

        public override void Activate() => ChatImages.OnOffer(this);
    }
}
