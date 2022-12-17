using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    [ClientOnly]
    public class BigDuck : Duck
    {
        public int iFr;
        public int iFrames;
        public int hp;
        public int mHp;
        public StateBinding _hpBinding = new StateBinding("hp");
        public StateBinding _mhpBinding = new StateBinding("mHp");
        public StateBinding _iFramesBinding = new StateBinding("iFrames");
        public StateBinding _duckSizeBinding = new StateBinding("duckSize");
        public StateBinding _netProgressBinding = new StateBinding("netProgress");
        public BigDuck(float xpos, float ypos, Profile p, float scal = 2, int lif = 100, int iframe = -1) : base(xpos, ypos, p)
        {
            duckSize = scal;
            hp = lif;
            mHp = lif;
            iFr = iframe;
        }
        public override void Update()
        {
            if (iFrames > 0)
            {
                iFrames--;
            }
            jumpSpeed = -5f / (netProgress + 1f);
            runMax = 3.1f / (netProgress * 2 + 1f);
            if (holdObject != null)
            {
                if (holdObject.xscale == 1)
                {
                    holdObject.scale *= duckSize;
                    holdObject.collisionSize *= duckSize;
                    holdObject.collisionOffset *= duckSize;
                    Send.Message(new NMSetScale(holdObject, holdObject.scale));
                }
            }
            for (int i = 0; i < nettedPoses.Count; i += 2)
            {
                float f = nettedPoses[i].Distance(Offset(nettedPoses[i + 1]));
                if (f > 64 * xscale)
                {
                    netProgress -= 0.2f / duckSize;
                    if (netProgress < 0) netProgress = 0;
                    nettedPoses.RemoveAt(i);
                    nettedPoses.RemoveAt(i);
                }
            }
            base.Update();
        }
        public List<Vec2> nettedPoses = new List<Vec2>();
        public float netProgress;
        public override void ThrowItem(bool throwWithForce = true)
        {
            if (holdObject == null)
                return;
            if (_throwFondle)
                Fondle(holdObject);
            ObjectThrown(holdObject);
            holdObject.hSpeed = 0f;
            holdObject.vSpeed = 0f;
            holdObject.clip.Add(this);
            holdObstructed = false;
            if (holdObject is Mine && !(holdObject as Mine).pin && (!crouch || !grounded))
                (holdObject as Mine).Arm();
            if (!crouch)
            {
                float num1 = 1f;
                float num2 = 1f;
                if (inputProfile.Down(Triggers.Left) || inputProfile.Down(Triggers.Right))
                    num1 = 2.5f;
                if (num1 == 1.0 && inputProfile.Down(Triggers.Up))
                {
                    holdObject.vSpeed -= 5f * holdWeightMultiplier;
                }
                else
                {
                    float num3 = num1 * holdWeightMultiplier;
                    if (inputProfile.Down(Triggers.Up))
                        num2 = 2f;
                    float num4 = num2 * holdWeightMultiplier;
                    if (offDir > 0)
                        holdObject.hSpeed += 3f * num3;
                    else
                        holdObject.hSpeed -= 3f * num3;
                    if (reverseThrow)
                        holdObject.hSpeed = -holdObject.hSpeed;
                    holdObject.vSpeed -= 2f * num4;
                }
            }
            if (Recorder.currentRecording != null)
                Recorder.currentRecording.LogAction(2);
            holdObject.hSpeed += 0.3f * offDir;
            holdObject.hSpeed *= holdObject.throwSpeedMultiplier;
            if (!throwWithForce)
                holdObject.hSpeed = holdObject.vSpeed = 0f;
            else if (Network.isActive)
            {
                if (isServerForObject)
                    _netTinyMotion.Play();
            }
            else
                SFX.Play("tinyMotion");
            _lastHoldItem = holdObject;
            _timeSinceThrow = 0;
            _holdObject.velocity *= duckSize;
            _holdObject = null;
        }
        public override void OnGhostObjectAdded()
        {
            if (!isServerForObject)
                return;
            if (_trappedInstance == null)
            {
                _trappedInstance = new BigTrapped(x, y - 9999f, this)
                {
                    active = false,
                    visible = false,
                    authority = (NetIndex8)80
                };
                if (!GhostManager.inGhostLoop)
                    GhostManager.context.MakeGhost(_trappedInstance);
                Level.Add(_trappedInstance);
                Fondle(_trappedInstance);
            }
            if (_cookedInstance == null)
            {
                _cookedInstance = new CookedDuck(x, y - 9999f)
                {
                    active = false,
                    visible = false,
                    authority = (NetIndex8)80
                };
                if (!GhostManager.inGhostLoop)
                    GhostManager.context.MakeGhost(_cookedInstance);
                Level.Add(_cookedInstance);
                if (_profile.localPlayer)
                    Fondle(_cookedInstance);
            }
            if (_ragdollInstance != null)
                return;
            _ragdollInstance = new BigRagdoll(x, y - 9999f, this, false, 0f, 0, Vec2.Zero)
            {
                npi = netProfileIndex
            };
            _ragdollInstance.RunInit();
            _ragdollInstance.active = false;
            _ragdollInstance.visible = false;
            _ragdollInstance.authority = (NetIndex8)80;
            Level.Add(_ragdollInstance);
            Fondle(_ragdollInstance);
        }
        public override void GoRagdoll()
        {
            if (Network.isActive && (_ragdollInstance == null || _ragdollInstance != null && _ragdollInstance.visible || _cookedInstance != null && _cookedInstance.visible) || ragdoll != null || _cooked != null)
                return;
            _hovering = false;
            float ypos = y + 4f;
            float degrees;
            if (sliding)
            {
                ypos += 6f;
                degrees = offDir >= 0 ? 0f : 180f;
            }
            else
                degrees = -90f;
            Vec2 v = new Vec2(_hSpeed, _vSpeed);
            hSpeed = 0f;
            vSpeed = 0f;
            if (Network.isActive)
            {
                ragdoll = _ragdollInstance;
                _ragdollInstance.active = true;
                _ragdollInstance.visible = true;
                _ragdollInstance.solid = true;
                _ragdollInstance.enablePhysics = true;
                _ragdollInstance.x = x;
                _ragdollInstance.y = y;
                _ragdollInstance.owner = null;
                _ragdollInstance.npi = netProfileIndex;
                _ragdollInstance.SortOutParts(x, ypos, this, sliding, degrees, offDir, v);
                Fondle(_ragdollInstance);
            }
            else
            {
                ragdoll = new BigRagdoll(x, ypos, this, sliding, degrees, offDir, v);
                Level.Add(ragdoll);
                ragdoll.RunInit();
            }
            if (ragdoll == null)
                return;
            ragdoll.connection = connection;
            ragdoll.part1.connection = connection;
            ragdoll.part2.connection = connection;
            ragdoll.part3.connection = connection;
            if (!fancyShoes)
            {
                Hat hatt = (Hat)GetEquipment(typeof(Hat));
                if (hatt != null && !hatt.strappedOn && !(DGRSettings.StickyHats && hatt is TeamHat))
                {
                    Unequip(hatt);
                    hatt.hSpeed = hSpeed * 1.2f;
                    hatt.vSpeed = vSpeed - 2f;
                }
                ThrowItem(false);
            }
            OnTeleport();
            if (y > -4000.0)
                y -= 5000f;
            sliding = false;
            crouch = false;
        }
        public override void Netted(Net n)
        {
            if (duckSize > 1)
            {
                IEnumerable<Block> bbs = Level.CheckCircleAll<Block>(position, 24 * xscale);

                int rng = bbs.Count();
                bool added = false;
                foreach (Block b in bbs)
                {
                    rng -= Rando.Int(1, 2);
                    if (b.frame == 11 || b.frame == 19 || b is BlockGroup) continue;
                    if (rng == 0)
                    {
                        nettedPoses.Add(b.position);
                        nettedPoses.Add(Rando.Vec2(-4, 4, -4, 6));
                        added = true;
                        break;
                    }
                }
                if (added)
                {
                    Block b = Level.Nearest<Block>(position, 24 * xscale);
                    if (b == null)
                    {
                        return;
                    }
                    else
                    {
                        nettedPoses.Add(b.position);
                        nettedPoses.Add(Rando.Vec2(-4, 4, -4, 6));
                    }
                }
                netProgress += 0.2f / duckSize;
                SuperFondle(n, DuckNetwork.localConnection);
                Level.Remove(n);
                if (netProgress < 1) return;
                netProgress = 0;
                nettedPoses.Clear();
            }

            if (Network.isActive && (_trappedInstance == null || _trappedInstance.visible) || _trapped != null)
                return;
            if (Network.isActive)
            {
                _trapped = _trappedInstance;
                _trappedInstance.active = true;
                _trappedInstance.visible = true;
                _trappedInstance.solid = true;
                _trappedInstance.enablePhysics = true;
                _trappedInstance.x = x;
                _trappedInstance.y = y;
                _trappedInstance.owner = null;
                _trappedInstance.InitializeStuff();
                n.Fondle(_trappedInstance);
                n.Fondle(this);
                if (_trappedInstance._duckOwner != null)
                    RumbleManager.AddRumbleEvent(_trappedInstance._duckOwner.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.None));
            }
            else
            {
                RumbleManager.AddRumbleEvent(profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.None));
                _trapped = new BigTrapped(x, y, this);
                Level.Add(_trapped);
            }
            ReturnItemToWorld(_trapped);
            OnTeleport();
            if (holdObject != null)
                n.Fondle(holdObject);
            ThrowItem(false);
            Level.Remove(n);
            ++profile.stats.timesNetted;
            _trapped.clip.Add(this);
            _trapped.clip.Add(n);
            _trapped.hSpeed = hSpeed + n.hSpeed * 0.4f;
            _trapped.vSpeed = (float)(vSpeed + n.vSpeed - 1.0);
            if (_trapped.hSpeed > 6.0)
                _trapped.hSpeed = 6f;
            if (_trapped.hSpeed < -6.0)
                _trapped.hSpeed = -6f;
            if (n.onFire)
                Burn(n.position, n);
            if (n.responsibleProfile == null || n.responsibleProfile.duck == null)
                return;
            trappedBy = n.responsibleProfile;
            n.responsibleProfile.duck.AddCoolness(1);
            Event.Log(new NettedEvent(n.responsibleProfile, profile));
        }
        public override void Draw()
        {
            for (int i = 0; i < nettedPoses.Count; i += 2)
            {
                Graphics.DrawLine(nettedPoses[i], Offset(nettedPoses[i + 1]), Color.White, xscale, depth - 1);
            }
            if (iFrames > 0)
            {
                alpha = iFrames % 2 == 0 ? 1 : 0.5f;
            }
            else alpha = 1;

            float f = ((float)hp / (float)mHp) * collisionSize.x * 3f;
            Graphics.DrawRect(topLeft - new Vec2(collisionSize.x, 4), topLeft + new Vec2(f - collisionSize.x, -9), Color.Red, 0.9f);
            Graphics.DrawRect(topLeft - new Vec2(collisionSize.x, 4), topRight + new Vec2(collisionSize.x, -9), Color.Black, 1, false);

            base.Draw();
        }
        public override void TryGrab()
        {
            foreach (Holdable h in Level.CheckCircleAll<Holdable>(new Vec2(x, y + 4f), 20f * duckSize).OrderBy(h => h, new CompareHoldablePriorities(this)))
            {
                if (h.owner == null && h.canPickUp && (h != _lastHoldItem || _timeSinceThrow >= 30) && h.active && h.visible && Level.CheckLine<Block>(position, h.position) == null)
                {
                    GiveHoldable(h);
                    if (Network.isActive)
                    {
                        if (isServerForObject)
                            _netTinyMotion.Play();
                    }
                    else
                        SFX.Play("tinyMotion");
                    if (holdObject.disarmedFrom != this && (DateTime.Now - holdObject.disarmTime).TotalSeconds < 0.5)
                        AddCoolness(2);
                    tryGrabFrames = 0;
                    break;
                }
            }
        }
        public override bool Kill(DestroyType type = null)
        {
            if (hp == 0)
            {
                return base.Kill(type);
            }
            else
            {
                iFrames = iFr;
                hp--;
                return false;
            }
        }
    }
}
