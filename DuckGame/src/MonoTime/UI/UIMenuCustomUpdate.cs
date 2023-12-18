using System;

namespace DuckGame.src.MonoTime.UI
{
    public class UIMenuCustomUpdate : UIMenu
    {
        private Action _customUpdate;

        public UIMenuCustomUpdate(
          Action customUpdate,
          string title,
          float xpos,
          float ypos,
          float wide = -1f,
          float high = -1f,
          string conString = "",
          InputProfile conProfile = null,
          bool tiny = false)
          : base(title, xpos, ypos, wide, high, conString, conProfile, tiny)
        {
            _customUpdate = customUpdate;
        }

        public override void Update()
        {
            if (_customUpdate != null)
                _customUpdate();
            base.Update();
        }
    }
}
