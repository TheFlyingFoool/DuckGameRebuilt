namespace DuckGame.Cobalt
{
    public static partial class Patches
    {
        [Patch(typeof(Level), nameof(Level.PostDrawLayer), Harmony.Fix.Prefix)]
        public static void PostDrawLayer_prefix(Layer layer)
        {
            if (layer == Layer.HUD)
                Graphics.DrawRect(new Rectangle(2, 2, 4, 4), Rebuilder.OnDGR ? Color.Aqua : Color.Red, 2f);
        }
    }
}