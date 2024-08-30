namespace DuckGame
{
    public class FFHatPreview : FFBoundary
    {
        public bool Fullscreen = false;
        public bool Playtest = false;
        public bool LoopAnimation = false;
        public bool AnimationPlaying = true;

        public FFHatPreview(Rectangle bounds)
            : base(bounds, 1f, BorderStyle.Thick)
        {
            
        }
    }
}