// Decompiled with JetBrains decompiler
// Type: DuckGame.MonoMainCore
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
