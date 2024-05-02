using System.Collections.Generic;

namespace DuckGame
{
    public class MonoMainCore
    {
        public UIComponent _pauseMenu;
        public List<UIComponent> closeMenuUpdate = new List<UIComponent>();
        public bool saidSpecial;
        public int gachas;
        public int rareGachas;
        public UIMenu _confirmMenu;
        public float _fade = 1f;
        public float _fadeAdd;
        public float _flashAdd;
        public bool closeMenus;
        public bool menuOpenedThisFrame;
        public bool dontResetSelection;
        public Layer ginormoBoardLayer;
        public HashSet<IEngineUpdatable> engineUpdatables = new HashSet<IEngineUpdatable>();
    }
}
