// Decompiled with JetBrains decompiler
// Type: DuckGame.EditorTestLevel
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class EditorTestLevel : Thing
    {
        private Editor _editor;
        protected bool _quitTesting;

        public Editor editor => this._editor;

        public EditorTestLevel(Editor editor)
          : base()
        {
            this._editor = editor;
        }

        public override void Update()
        {
            if (!this._quitTesting || Level.current is ChallengeLevel)
                return;
            Level.current = (Level)this._editor;
        }
    }
}
