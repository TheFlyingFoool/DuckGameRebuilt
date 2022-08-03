// Decompiled with JetBrains decompiler
// Type: DuckGame.NMAssignKill
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class NMAssignKill : NMAssignWin
    {
        public NMAssignKill(List<Profile> pProfiles, Profile pTheRealWinnerHere)
          : base(pProfiles, pTheRealWinnerHere)
        {
            _sound = "scoreDingShort";
        }

        public NMAssignKill() => _sound = "scoreDingShort";
    }
}
