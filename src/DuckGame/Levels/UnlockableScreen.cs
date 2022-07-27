// Decompiled with JetBrains decompiler
// Type: DuckGame.UnlockableScreen
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class UnlockableScreen : Level
    {
        private HashSet<Unlockable> _unlocks;
        private UIUnlockBox _unlockBox;

        public UnlockableScreen() => this._centeredView = true;

        public override void Initialize()
        {
            base.Initialize();
            Unlockables.HasPendingUnlocks();
            this._unlocks = Unlockables.GetPendingUnlocks();
        }

        public override void Update()
        {
            if (this._unlockBox == null)
            {
                this._unlockBox = new UIUnlockBox(this._unlocks.ToList<Unlockable>(), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f);
                MonoMain.pauseMenu = _unlockBox;
            }
            base.Update();
        }
    }
}
