using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class UnlockableScreen : Level
    {
        private HashSet<Unlockable> _unlocks;
        private UIUnlockBox _unlockBox;

        public UnlockableScreen() => _centeredView = true;

        public override void Initialize()
        {
            base.Initialize();
            Unlockables.HasPendingUnlocks();
            _unlocks = Unlockables.GetPendingUnlocks();
        }

        public override void Update()
        {
            if (_unlockBox == null)
            {
                _unlockBox = new UIUnlockBox(_unlocks.ToList(), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f);
                MonoMain.pauseMenu = _unlockBox;
            }
            base.Update();
        }
    }
}
