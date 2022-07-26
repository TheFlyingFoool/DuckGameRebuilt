// Decompiled with JetBrains decompiler
// Type: DuckGame.NMLevelDataBegin
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMLevelDataBegin : NMConditionalEvent
    {
        public new byte levelIndex;

        public NMLevelDataBegin()
        {
        }

        public NMLevelDataBegin(byte pLevelIndex) => this.levelIndex = pLevelIndex;

        public override bool Update() => (int)Level.current.networkIndex == (int)this.levelIndex && Level.current.initializeFunctionHasBeenRun;

        public override void Activate()
        {
        }
    }
}
