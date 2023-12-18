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
