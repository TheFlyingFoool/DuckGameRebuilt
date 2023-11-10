namespace DuckGame
{
    public class UICustomLevelMenu : UIMenuItemNumber
    {
        public UICustomLevelMenu(UIMenuAction action = null, UIAlign al = UIAlign.Center, Color c = default(Color))
          : base("CUSTOM LEVELS", action, step: 0, c: c)
        {
            _useBaseActivationLogic = true;
            controlString = null;
        }

        public override void Update()
        {
            int num = 0;
            foreach (string activatedLevel in Editor.activatedLevels)
                ++num;
            if (_textItem != null)
            {
                if (num == 0)
                    _textItem.text = "NONE";
                else
                    _textItem.text = num.ToString();
            }
            base.Update();
        }
    }
}
