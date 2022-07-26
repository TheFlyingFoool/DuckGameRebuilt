// Decompiled with JetBrains decompiler
// Type: DuckGame.UnlockableAchievement
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class UnlockableAchievement : Unlockable
    {
        public UnlockableAchievement(
          string identifier,
          Func<bool> condition,
          string nam,
          string desc,
          string achieve)
          : this(true, identifier, condition, nam, desc, achieve)
        {
        }

        public UnlockableAchievement(
          bool canHint,
          string identifier,
          Func<bool> condition,
          string nam,
          string desc,
          string achieve)
          : base(identifier, condition, nam, desc, achieve)
        {
            this.allowHints = canHint;
        }
    }
}
