namespace DuckGame
{
    [ClientOnly]
    //[EditorGroup("Rebuilt|Wump|Machine Guns")]
    public class WumpAK47 : Gun
    {
        public StateBinding _firesBinding = new StateBinding("fires");
        public StateBinding _accuracyBinding = new StateBinding("_accuracyLost");
        public Sprite nub;
        public WumpAK47(float xpos, float ypos) : base(xpos, ypos)
        {
            ammo = 30;
            _ammoType = new ATHighCalMachinegun
            {
                bulletColor = new Color(0, 205, 255)
            };
            _type = "gun";


            nub = new Sprite("wumpak47snub")
            {
                center = new Vec2(2, 2.5f)
            };

            graphic = new Sprite("wumpak47");
            center = new Vec2(16f, 4f);
            collisionOffset = new Vec2(-8f, -3f);
            collisionSize = new Vec2(18f, 10f);
            _barrelOffsetTL = new Vec2(32f, 4f);
            _holdOffset = new Vec2(2, 0);
            _fireSound = "deepMachineGun2";
            _fullAuto = true;
            _fireWait = 1.2f;
            _kickForce = 3.5f;
            _fireRumble = RumbleIntensity.Kick;
            loseAccuracy = 0.2f;
            maxAccuracyLost = 0.8f;
            _editorName = "AK47";
            editorTooltip = "This aint no normal gun, charge up a barrage of bullets and watch out for the recoil.";
        }
        public override void Update()
        {
            if (fires > 0 && !charge && isServerForObject)
            {
                del++;
                if (infiniteAmmoVal) del++;
                if (del > inp)
                {
                    inp++;
                    if (inp > 5) inp = 5;
                    del = 0;
                    fires--;
                    _wait = 0;
                    Fire();
                }
            }
            base.Update();
        }
        public override void Draw()
        {
            Vec2 v = Offset(new Vec2(4 - fires, -4));
            nub.angle = angle;
            nub.scale = scale;
            nub.alpha = alpha;
            Graphics.Draw(nub, v.x, v.y, depth - 1);
            base.Draw();
        }
        public int fires;
        public int del;

        public bool charge;
        public int charging;
        public override void OnPressAction()
        {
            if (_wait == 0)
            {
                charge = true;
                del = 20;
                fires = 0;
                inp = 22;
            }
        }
        public int inp;
        public override void OnHoldAction()
        {
            if (charge && fires < (infiniteAmmoVal ? 12 : 7))
            {
                del++;
                if (infiniteAmmoVal) del++;
                if (del > inp)
                {
                    inp -= 4;
                    SFX.PlaySynchronized("click");
                    del = 0;
                    fires++;
                }
            }
        }
        public override void OnReleaseAction()
        {
            inp = 1;
            charge = false;
            base.OnReleaseAction();
        }
    }
}
