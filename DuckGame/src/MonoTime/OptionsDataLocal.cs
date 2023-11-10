namespace DuckGame
{
    public class OptionsDataLocal : DataClass
    {
        public Resolution currentResolution { get; set; }

        public Resolution windowedResolution { get; set; }

        public Resolution windowedFullscreenResolution { get; set; }

        public Resolution fullscreenResolution { get; set; }

        public Resolution previousAdapterResolution { get; set; }

        public OptionsDataLocal() => _nodeName = "Options";
    }
}
