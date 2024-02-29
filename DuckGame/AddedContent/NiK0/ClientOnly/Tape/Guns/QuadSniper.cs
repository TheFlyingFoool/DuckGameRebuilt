namespace DuckGame
{
    [ClientOnly]
    public class QuadSniper : Sniper
    {
        public QuadSniper(float xpos, float ypos) : base(xpos, ypos)
        {
            graphic = new Sprite("quadsniper");
            _manualLoad = false;
            _kickForce = 7;
            _fireSound = "quadsniper";
        }
        public override void ApplyKick()
        {
            base.ApplyKick();
            hSpeed *= 1.5f;
            if (duck != null) duck.vSpeed *= 0.5f;
        }
        public override void Update()
        {
            base.Update();
            laserSight = false;
        }
        public override void Fire()
        {
            if (ammo > 0)
            {
                ammo--;
                ThinQuadLaserBullet q = new ThinQuadLaserBullet(barrelPosition.x, barrelPosition.y, barrelVector * 5);
                Level.Add(q);
                ApplyKick();
                PlayFireSound();
            }
            else DoAmmoClick();
        }
    }
    [ClientOnly]
    public class ThinQuadLaserBullet : Thing
    {
        public StateBinding _positionBinding = new CompressedVec2Binding("position", doLerp: true);
        public StateBinding _travelBinding = new CompressedVec2Binding(nameof(travel), 20);
        protected Vec2 _travel;
        protected SinWaveManualUpdate _wave = (SinWaveManualUpdate)0.5f;
        protected SinWaveManualUpdate _wave2 = (SinWaveManualUpdate)1f;
        public Vec2 travel
        {
            get => _travel;
            set => _travel = value;
        }
        public ThinQuadLaserBullet(float xpos, float ypos, Vec2 ttravel) : base(xpos, ypos)
        {
            travel = ttravel;
        }
        public override void Update()
        {
            position += travel;
            _wave.Update();
            _wave2.Update();
            if (isServerForObject && (x > Level.current.bottomRight.x + 200 || x < Level.current.topLeft.x - 200))
                Level.Remove(this);
            foreach (MaterialThing materialThing in Level.CheckLineAll<MaterialThing>(position, position + travel.normalized * 30))
            {
                if (materialThing.isServerForObject)
                {
                    bool destroyed = materialThing.destroyed;
                    materialThing.Destroy(new DTIncinerate(this));
                    if (materialThing.destroyed && !destroyed)
                    {
                        if (Recorder.currentRecording != null)
                            Recorder.currentRecording.LogAction(2);
                        if (materialThing is Duck && !(materialThing as Duck).dead)
                            Recorder.currentRecording.LogBonus();
                    }
                }
            }
            base.Update();
        }
        private Interp QuadLerp = new Interp(true);
        public override void Draw()
        {
            QuadLerp.UpdateLerpState(position, SkipIntratick > 0 ? 1 : MonoMain.IntraTick, MonoMain.UpdateLerpState);

            Graphics.DrawLine(QuadLerp.Position, QuadLerp.Position + travel.normalized * 30, new Color(byte.MaxValue - (int)(_wave.normalized * 90f), 137 + (int)(_wave.normalized * 50f), 31 + (int)(_wave.normalized * 30f)), 3, depth);
            Graphics.DrawLine(QuadLerp.Position + travel.normalized * 2, QuadLerp.Position + travel.normalized * 28, new Color(byte.MaxValue, 224 - (int)(_wave2.normalized * 150f), 90 + (int)(_wave2.normalized * 50f)), 1, depth + 1);
        }
    }
}
