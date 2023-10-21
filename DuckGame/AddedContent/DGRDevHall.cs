using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class DGRDevHall : XMLLevel
    {
        public DGRDevHall(string name) : base(name)
        {
            _followCam = new FollowCam
            {
                lerpMult = 1f,
                startCentered = false
            };
            camera = _followCam;
        }
        private List<Duck> _pendingSpawns;
        public Vec2 SpawnPosition;
        public override void Initialize()
        {
            TeamSelect2.DefaultSettings();
            base.Initialize();
            _pendingSpawns = new Deathmatch(this).SpawnPlayers(false);
            foreach (Duck pendingSpawn in _pendingSpawns)
            {
                SpawnPosition = pendingSpawn.position;
                followCam.Add(pendingSpawn);
                First<ArcadeHatConsole>()?.MakeHatSelector(pendingSpawn);
            }
            followCam.Adjust();
        }
        public Duck _duck;
        public bool _entering = true;
        public override void Update()
        {
            if (_entering)
            {
                Graphics.fade = Lerp.Float(Graphics.fade, 1f, 0.05f);
                if (Graphics.fade > 0.99f)
                {
                    _entering = false;
                    Graphics.fade = 1f;
                }
            }
            if (_pendingSpawns != null && _pendingSpawns.Count > 0)
            {
                Duck pendingSpawn = _pendingSpawns[0];
                AddThing(pendingSpawn);
                _pendingSpawns.RemoveAt(0);
                _duck = pendingSpawn;
            }
            base.Update();
        }
        private FollowCam _followCam;
        public FollowCam followCam => _followCam;
    }
}
