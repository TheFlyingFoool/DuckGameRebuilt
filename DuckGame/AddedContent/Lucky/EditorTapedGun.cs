namespace DuckGame
{
    // TapedGun made by the WeaponTaper. Vanilla clients load the guns inside a TapedGun without
    // matching network ids, which spawns untaped copies of them online, so this is kept DGR only
    [ClientOnly]
    public class EditorTapedGun : TapedGun
    {
        public EditorTapedGun(float xval, float yval)
          : base(xval, yval)
        {
        }
    }
}
