// Decompiled with JetBrains decompiler
// Type: DuckGame.GameModeCore
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class GameModeCore
    {
        public int roundsBetweenIntermission = 10;
        public int winsPerSet = 10;
        public bool _started;
        public bool getReady;
        public int _numMatchesPlayed;
        public bool showdown;
        public string previousLevel;
        public string _currentMusic = "";
        public bool firstDead;
        public bool playedGame;
    }
}
