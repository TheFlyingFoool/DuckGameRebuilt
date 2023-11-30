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
            allowHints = canHint;
        }
    }
}
