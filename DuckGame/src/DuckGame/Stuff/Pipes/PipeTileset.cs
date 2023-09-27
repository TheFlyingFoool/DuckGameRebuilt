// Decompiled with JetBrains decompiler
// Type: DuckGame.PipeTileset
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using DuckGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DuckGame
{
    public class PipeTileset : Block, IDontMove, IDrawToDifferentLayers
    {
        public EditorProperty<bool> trapdoor = new EditorProperty<bool>(false);
        public EditorProperty<bool> background = new EditorProperty<bool>(false);
        private Dictionary<Direction, PipeTileset> connections = new Dictionary<Direction, PipeTileset>();
        public SpriteMap _sprite;
        public float pipeDepth;
        public bool searchUp;
        public bool searchDown;
        public bool searchLeft;
        public bool searchRight;
        private PipeTileset _pipeUp;
        private PipeTileset _pipeDown;
        private PipeTileset _pipeLeft;
        private PipeTileset _pipeRight;
        private bool _validPipe;
        private PipeTileset _oppositeEnd;
        public float travelLength;
        private HashSet<ITeleport> _objectsInPipes = new HashSet<ITeleport>();
        private List<PipeBundle> _transporting = new List<PipeBundle>();
        private HashSet<PhysicsObject> _pipingOut = new HashSet<PhysicsObject>();
        private List<ITeleport> _removeFromPipe = new List<ITeleport>();
        private List<MaterialThing> _colliding;
        private int framesSincePipeout;
        private int _transportingIndex;
        public bool _initializedConnections;
        private bool _testedValidity;
        private int _initializedBackground;
        private bool entered;
        private int _failBullets;
        private static PipeTileset _lastAdd;
        public bool hasKinks;
        public bool _foregroundDraw;
        private bool _drawBlockOverlay;
        private List<PipeParticle> _particles = new List<PipeParticle>();
        private float partRot;
        private int partWait = -100;
        public float _flapLerp;
        public float _flap;

        public bool IsBackground()
        {
            if (Corderator.instance != null && Corderator.instance.PlayingThatShitBack)
            {
                return background.value;
            }
            return connections.Count > 1 && background.value;
        }

        public PipeTileset(float x, float y, string pSprite)
          : base(x, y)
        {
            shouldbegraphicculled = false; // this his here because background pipes dont play well with graphic culling, mabye look into doing it better in this case
            _editorName = "Pipe";
            editorTooltip = "Travel through pipes!";
            layer = Layer.Game;
            depth = (Depth)0.9f;
            thickness = 3f;
            _sprite = new SpriteMap(pSprite, 18, 18);
            graphic = _sprite;
            physicsMaterial = PhysicsMaterial.Metal;
            center = new Vec2(9f, 9f);
            _sprite.CenterOrigin();
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            _sprite.frame = 0;
            placementLayerOverride = Layer.Foreground;
        }

        public Vec2 endOffset => position + endNormal * 9f;

        public Vec2 endNormal
        {
            get
            {
                if (Up() != null)
                    return new Vec2(0f, 1f);
                if (Down() != null)
                    return new Vec2(0f, -1f);
                if (Left() != null)
                    return new Vec2(1f, 0f);
                return Right() != null ? new Vec2(-1f, 0f) : position;
            }
        }

        public bool MovingIntoPipe(Vec2 pPosition, Vec2 pVelocity, float pThresh = 2f)
        {
            bool flag = false;
            if (endNormal.x != 0 && pVelocity.x != 0)
            {
                if (Math.Sign(pVelocity.x) != Math.Sign(endNormal.x))
                    flag = true;
            }
            else if (endNormal.y != 0 && pVelocity.y != 0 && Math.Sign(pVelocity.y) != Math.Sign(endNormal.y))
                flag = true;
            return flag && (Left() != null && pPosition.x < right + pThresh && pPosition.y <= bottom + pThresh && pPosition.y >= top - pThresh || Right() != null && pPosition.x < left - pThresh && pPosition.y <= bottom + pThresh && pPosition.y >= top - pThresh || Up() != null && pPosition.y < bottom + pThresh && pPosition.x <= right + pThresh && pPosition.x >= left - pThresh || Down() != null && pPosition.y > top - pThresh && pPosition.x <= right + pThresh && pPosition.x >= left - pThresh);
        }

        public PipeTileset oppositeEnd => _oppositeEnd;

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            if (connections.Count > 0)
            {
                binaryClassChunk.AddProperty("up", Up() != null);
                binaryClassChunk.AddProperty("down", Down() != null);
                binaryClassChunk.AddProperty("left", Left() != null);
                binaryClassChunk.AddProperty("right", Right() != null);
            }
            else
            {
                binaryClassChunk.AddProperty("up", searchUp);
                binaryClassChunk.AddProperty("down", searchDown);
                binaryClassChunk.AddProperty("left", searchLeft);
                binaryClassChunk.AddProperty("right", searchRight);
            }
            binaryClassChunk.AddProperty("pipeFrame", _sprite.frame);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            searchUp = node.GetProperty<bool>("up");
            searchDown = node.GetProperty<bool>("down");
            searchLeft = node.GetProperty<bool>("left");
            searchRight = node.GetProperty<bool>("right");
            _sprite.frame = node.GetProperty<int>("pipeFrame");
            return true;
        }

        public override Type TabRotate(bool control)
        {
            Thing pipetype = this;
            if (control)
            {
                background = !background;
            }
            else
            {
                switch (pipetype)
                {
                    case DuckGame.PipeBlue:
                        editorCycleType = typeof(PipeRed);
                        break;
                    case DuckGame.PipeRed:
                        editorCycleType = typeof(PipeGreen);
                        break;
                    case PipeGreen:
                        editorCycleType = typeof(PipeBlue);
                        break;
                }
            }
            return editorCycleType;
        }

        public bool isEntryPipe => _validPipe && connections.Count == 1 && !(bool) trapdoor;
        //9 11
        private void PipeOut(PhysicsObject d)
        {
            FlapPipe();
            if (d is Duck)
            {
                (d as Duck).immobilized = false;
                (d as Duck).pipeOut = 6;
                (d as Duck).CancelFlapping();
            }
            bool flag = false;
            d.hSpeed = 0f;
            d.clip.Clear();
            if (Down() != null)
            {
                d.position = position - new Vec2(0f, 10f);
                if (d is Duck)
                {
                    (d as Duck).jumping = true;
                    (d as Duck).slamWait = 4;
                }
                d.vSpeed = -6f;
                if (d is RagdollPart)
                    d.hSpeed += Rando.Float(-1f, 1f);
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 6; ++index)
                {
                    SmallSmoke smallSmoke = SmallSmoke.New(x + Rando.Float(-4f, 4f), y + Rando.Float(-4f, 4f));
                    smallSmoke.velocity = new Vec2(Rando.Float(-0.5f, 0.5f), Rando.Float(0f, -0.5f));
                    Level.Add(smallSmoke);
                }
                if (Network.isActive && framesSincePipeout > 2)
                {
                    Send.Message(new NMPipeOut(new Vec2(x, y), 0));
                    framesSincePipeout = 0;
                }
                if (d is Duck && Level.CheckLine<Block>(position - new Vec2(0f, 16f), position - new Vec2(0f, 32f)) != null)
                {
                    Duck duck = d as Duck;
                    duck.position = position - new Vec2(0f, 16f);
                    duck.GoRagdoll();
                    flag = true;
                }
            }
            else if (Left() != null || Right() != null)
            {
                d.position = position + new Vec2(Left() != null ? 12f : -12f, -2f);
                d.vSpeed = -0f;
                if (Left() != null)
                    d.hSpeed = 6f;
                else
                    d.hSpeed = -6f;
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 6; ++index)
                {
                    SmallSmoke smallSmoke = SmallSmoke.New(x + (Left() != null ? 12f : -12f) + Rando.Float(-4f, 4f), y + Rando.Float(-4f, 4f));
                    if (Left() != null)
                        smallSmoke.velocity = new Vec2(Rando.Float(0.2f, 0.7f), Rando.Float(-0.5f, 0.5f));
                    else
                        smallSmoke.velocity = new Vec2(Rando.Float(-0.7f, -0.2f), Rando.Float(-0.5f, 0.5f));
                    Level.Add(smallSmoke);
                }
                if (Network.isActive && framesSincePipeout > 2)
                {
                    Send.Message(new NMPipeOut(new Vec2(x + (Left() != null ? 12f : -12f), y), Left() != null ? (byte)1 : (byte)3));
                    framesSincePipeout = 0;
                }
                if (d is Duck)
                {
                    Duck t = d as Duck;
                    t.sliding = true;
                    t.crouch = true;
                    t.crouchLock = true;
                    t.SetCollisionMode("slide");
                    t.position.y -= 6f;
                    t.ReturnItemToWorld(t);
                }
                d.clip.Add(this);
                flag = true;
            }
            else
            {
                d.position = position + new Vec2(0f, 4f);
                d.vSpeed = 5f;
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 6; ++index)
                {
                    SmallSmoke smallSmoke = SmallSmoke.New(x + Rando.Float(-4f, 4f), y + 12f + Rando.Float(-4f, 4f));
                    smallSmoke.velocity = new Vec2(Rando.Float(-0.5f, 0.5f), Rando.Float(0.2f, 0.7f));
                    Level.Add(smallSmoke);
                }
                if (Network.isActive && framesSincePipeout > 2)
                {
                    Send.Message(new NMPipeOut(new Vec2(x, y + 12f), 2));
                    framesSincePipeout = 0;
                }
                if (d is Duck && (Level.CheckLine<Block>(position + new Vec2(0f, 16f), position + new Vec2(0f, 32f)) != null || Level.CheckLine<IPlatform>(position + new Vec2(0f, 16f), position + new Vec2(0f, 32f)) != null))
                {
                    Duck duck = d as Duck;
                    duck.position = position + new Vec2(0f, 16f);
                    duck.GoRagdoll();
                    flag = true;
                }
            }
            d.Ejected(this);
            if (!flag)
            {
                Clip(d);
                d.skipClip = true;
                _pipingOut.Add(d);
            }
            else
                d.skipClip = false;
        }

        private void BreakPipeLink(PhysicsObject d)
        {
            _removeFromPipe.Add(d);
            if (d is Duck)
                (d as Duck).immobilized = false;
            d.skipClip = false;
        }

        private void Clip(PhysicsObject d)
        {
            if (d == null)
                return;
            if (_colliding != null)
            {
                foreach (MaterialThing materialThing in _colliding)
                    d.clip.Add(materialThing);
            }
            d.clip.Add(this);
        }

        private void UpdatePipeEnd()
        {
            if (_colliding != null)
                return;
            _colliding = new List<MaterialThing>();
            foreach (IPlatform platform in _pipeUp != null || _pipeDown != null ? Level.CheckRectAll<IPlatform>(topLeft + new Vec2(0f, -16f), bottomRight + new Vec2(0f, 16f)).ToList() : (IEnumerable<IPlatform>)Level.CheckLineAll<IPlatform>(topLeft + new Vec2(-16f, 0f), bottomRight + new Vec2(16f, 0f)).ToList())
            {
                if (platform is MaterialThing && !(platform is PhysicsObject))
                    _colliding.Add(platform as MaterialThing);
            }
        }

        private void UpdatePipeEndLate()
        {
            foreach (PhysicsObject physicsObject in _pipingOut)
            {
                Fondle(physicsObject, DuckNetwork.localConnection);
                Clip(physicsObject);
                physicsObject.skipClip = true;
                physicsObject.grounded = false;
                physicsObject._sleeping = false;
                if (!Collision.Rect(rectangle, physicsObject.rectangle))
                    BreakPipeLink(physicsObject);
            }
            foreach (ITeleport teleport in _removeFromPipe)
            {
                _objectsInPipes.Remove(teleport);
                if (teleport is PhysicsObject)
                    _pipingOut.Remove(teleport as PhysicsObject);
            }
            _removeFromPipe.Clear();
        }

        private void StartTransporting(Thing pThing)
        {
            if (pThing is RagdollPart)
            {
                Ragdoll doll = (pThing as RagdollPart).doll;
                if (doll == null)
                    return;
                _transporting.Add(new PipeBundle()
                {
                    thing = doll.part1,
                    cameraPosition = doll.part1.position
                });
                _transporting.Add(new PipeBundle()
                {
                    thing = doll.part2,
                    cameraPosition = doll.part1.position
                });
                _transporting.Add(new PipeBundle()
                {
                    thing = doll.part3,
                    cameraPosition = doll.part1.position
                });
                _removeFromPipe.Add(doll.part1);
                _removeFromPipe.Add(doll.part2);
                _removeFromPipe.Add(doll.part3);
            }
            else
            {
                _transporting.Add(new PipeBundle()
                {
                    thing = pThing,
                    cameraPosition = pThing.position
                });
                _removeFromPipe.Add(pThing as ITeleport);
            }
        }

        private void UpdateEntryPipe()
        {
            IEnumerable<PhysicsObject> physicsObjects = null;
            if (Down() != null)
                physicsObjects = Level.CheckRectAll<PhysicsObject>(topLeft + new Vec2(1f, -32f), bottomRight + new Vec2(-1f, 4f));
            else if (Up() != null)
                physicsObjects = Level.CheckRectAll<PhysicsObject>(topLeft + new Vec2(1f, -4f), bottomRight + new Vec2(-1f, 32f));
            else if (Left() != null)
                physicsObjects = Level.CheckRectAll<PhysicsObject>(topLeft + new Vec2(-4f, 3f), bottomRight + new Vec2(32f, -3f));
            else if (Right() != null)
                physicsObjects = Level.CheckRectAll<PhysicsObject>(topLeft + new Vec2(-32f, 3f), bottomRight + new Vec2(4f, -3f));
            foreach (PhysicsObject physicsObject in physicsObjects)
            {
                if (physicsObject.owner == null && !physicsObject.inPipe)
                {
                    bool flag1 = false;
                    if (Down() != null)
                        flag1 = physicsObject.bottom + physicsObject.vSpeed > top - 6 && physicsObject.width <= 16 && (!(physicsObject is Duck) || !(physicsObject as Duck).sliding);
                    else if (Up() != null)
                        flag1 = physicsObject.top + physicsObject.vSpeed < bottom + 6 && physicsObject.width <= 16 && (!(physicsObject is Duck) || !(physicsObject as Duck).sliding);
                    else if (Left() != null)
                        flag1 = physicsObject.left + physicsObject.hSpeed < right + 2 && physicsObject.height <= 16;
                    else if (Right() != null)
                        flag1 = physicsObject.right + physicsObject.hSpeed > left - 2 && physicsObject.height <= 16;
                    if (flag1 && physicsObject != null && physicsObject.isServerForObject)
                    {
                        bool flag2 = false;
                        if (Down() != null)
                            flag2 = physicsObject.vSpeed > -0.1f && physicsObject.bottom < top + 4 && Math.Abs(physicsObject.hSpeed) < 10;
                        else if (Up() != null)
                            flag2 = physicsObject.vSpeed < -2 && physicsObject.top > bottom - 4 && Math.Abs(physicsObject.hSpeed) < 10;
                        else if (Left() != null)
                            flag2 = physicsObject.hSpeed < -0.2f && physicsObject.left > right - 4 && Math.Abs(physicsObject.vSpeed) < 4;
                        else if (Right() != null)
                            flag2 = physicsObject.hSpeed > 0.2f && physicsObject.right < left + 4 && Math.Abs(physicsObject.vSpeed) < 4;
                        if (flag2 && !_pipingOut.Contains(physicsObject))
                        {
                            if (physicsObject is RagdollPart)
                            {
                                Ragdoll doll = (physicsObject as RagdollPart).doll;
                                if (doll != null && doll.part1 != null && doll.part2 != null && doll.part3 != null && doll.part1.owner == null && doll.part2.owner == null && doll.part3.owner == null && !_pipingOut.Contains(doll.part1) && !_pipingOut.Contains(doll.part2) && !_pipingOut.Contains(doll.part3))
                                {
                                    _objectsInPipes.Add(doll.part1);
                                    _objectsInPipes.Add(doll.part2);
                                    _objectsInPipes.Add(doll.part3);
                                    doll.part1.inPipe = true;
                                    doll.part2.inPipe = true;
                                    doll.part3.inPipe = true;
                                }
                            }
                            else
                            {
                                physicsObject.inPipe = true;
                                physicsObject.OnTeleport();
                                _objectsInPipes.Add(physicsObject);
                            }
                        }
                    }
                }
            }
            foreach (ITeleport teleport in Level.CheckCircleAll<ITeleport>(endOffset, 16f))
            {
                if (teleport is QuadLaserBullet)
                {
                    QuadLaserBullet quadLaserBullet = teleport as QuadLaserBullet;
                    if (!quadLaserBullet.inPipe && oppositeEnd != null && !(bool)oppositeEnd.trapdoor && MovingIntoPipe(quadLaserBullet.position, quadLaserBullet.travel, 4f))
                    {
                        _objectsInPipes.Add(teleport);
                        quadLaserBullet.inPipe = true;
                    }
                }
                else if (teleport is PhysicsParticle)
                {
                    PhysicsParticle physicsParticle = teleport as PhysicsParticle;
                    if (!physicsParticle.inPipe && oppositeEnd != null && !(bool)oppositeEnd.trapdoor && MovingIntoPipe(physicsParticle.position, physicsParticle.velocity, 4f))
                    {
                        physicsParticle.inPipe = true;
                        _objectsInPipes.Add(teleport);
                    }
                }
            }
            Vec2 vec2_1;
            foreach (ITeleport objectsInPipe in _objectsInPipes)
            {
                if (!_removeFromPipe.Contains(objectsInPipe))
                {
                    switch (objectsInPipe)
                    {
                        case QuadLaserBullet _:
                            QuadLaserBullet quadLaserBullet1 = objectsInPipe as QuadLaserBullet;
                            quadLaserBullet1.position = Lerp.Vec2Smooth(quadLaserBullet1.position, position, 0.2f);
                            vec2_1 = position - quadLaserBullet1.position;
                            if (vec2_1.length < 6)
                            {
                                quadLaserBullet1.position = oppositeEnd.position + oppositeEnd.endNormal * 4f;
                                QuadLaserBullet quadLaserBullet2 = quadLaserBullet1;
                                Vec2 endNormal = oppositeEnd.endNormal;
                                vec2_1 = quadLaserBullet1.travel;
                                double length = vec2_1.length;
                                Vec2 vec2_2 = endNormal * (float)length;
                                quadLaserBullet2.travel = vec2_2;
                                _removeFromPipe.Add(objectsInPipe);
                                quadLaserBullet1.inPipe = false;
                                continue;
                            }
                            continue;
                        case PhysicsParticle _:
                            PhysicsParticle physicsParticle = objectsInPipe as PhysicsParticle;
                            physicsParticle._grounded = true;
                            physicsParticle.position = Lerp.Vec2Smooth(physicsParticle.position, position, 0.2f);
                            physicsParticle.hSpeed *= 0.9f;
                            physicsParticle.vSpeed *= 0.9f;
                            vec2_1 = position - physicsParticle.position;
                            if (vec2_1.length < 6)
                            {
                                physicsParticle.position = oppositeEnd.endOffset + new Vec2(Rando.Float(-5f, 5f) * Math.Abs(oppositeEnd.endNormal.y), Rando.Float(-5f, 5f) * Math.Abs(oppositeEnd.endNormal.x));
                                physicsParticle.velocity = oppositeEnd.endNormal * Rando.Float(1f, 2f);
                                physicsParticle.hSpeed += Rando.Float(-1f, 1f) * Math.Abs(oppositeEnd.endNormal.y);
                                physicsParticle.vSpeed += Rando.Float(-1f, 1f) * Math.Abs(oppositeEnd.endNormal.x);
                                physicsParticle._grounded = false;
                                _removeFromPipe.Add(objectsInPipe);
                                physicsParticle.inPipe = false;
                                continue;
                            }
                            continue;
                        case PhysicsObject _:
                            PhysicsObject physicsObject = objectsInPipe as PhysicsObject;
                            bool flag3 = Up() != null || Down() != null;
                            if (physicsObject is Duck)
                            {
                                (physicsObject as Duck).immobilized = true;
                                if (!flag3)
                                {
                                    (physicsObject as Duck).crouch = true;
                                    (physicsObject as Duck).sliding = true;
                                }
                            }
                            Fondle(physicsObject, DuckNetwork.localConnection);
                            Clip(physicsObject);
                            physicsObject.skipClip = true;
                            physicsObject.grounded = false;
                            physicsObject._sleeping = false;
                            if (flag3)
                            {
                                physicsObject.position.x = Lerp.FloatSmooth(physicsObject.position.x, x, 0.4f);
                                physicsObject.hSpeed *= 0.8f;
                                if (Down() != null)
                                    physicsObject.vSpeed += 0.4f;
                                else
                                    physicsObject.vSpeed -= 0.4f;
                            }
                            else
                            {
                                if (physicsObject is Duck)
                                    physicsObject.position.y = Lerp.FloatSmooth(physicsObject.position.y, y - 10f, 0.6f);
                                else
                                    physicsObject.position.y = Lerp.FloatSmooth(physicsObject.position.y, y - (physicsObject.collisionCenter.y - y), 0.5f);
                                physicsObject.vSpeed *= 0.8f;
                                if (Left() != null)
                                    physicsObject.hSpeed -= 0.4f;
                                else if (Right() != null)
                                    physicsObject.hSpeed += 0.4f;
                            }
                            if (flag3)
                            {
                                foreach (IPlatform platform in Level.CheckRectAll<IPlatform>(topLeft + new Vec2(2f, -24f), bottomRight + new Vec2(-2f, 24f)))
                                {
                                    if (platform is MaterialThing)
                                        physicsObject.clip.Add(platform as MaterialThing);
                                }
                            }
                            else
                            {
                                foreach (IPlatform platform in Level.CheckRectAll<IPlatform>(topLeft + new Vec2(-24f, 2f), bottomRight + new Vec2(24f, -2f)))
                                {
                                    if (platform is MaterialThing)
                                        physicsObject.clip.Add(platform as MaterialThing);
                                }
                            }
                            vec2_1 = physicsObject.position - position;
                            if (vec2_1.length > 32 || physicsObject.owner != null)
                            {
                                physicsObject.inPipe = false;
                                BreakPipeLink(physicsObject);
                                continue;
                            }
                            if (flag3 && Math.Abs(physicsObject.position.x - x) < 4 || !flag3 && (physicsObject is Duck && Math.Abs(physicsObject.position.y - (y - 10f)) < 4 || !(physicsObject is Duck) && Math.Abs(physicsObject.position.y - y) < 4))
                            {
                                bool flag4 = false;
                                if (Down() != null)
                                    flag4 = physicsObject.position.y > top + 6;
                                else if (Up() != null)
                                    flag4 = physicsObject.position.y < bottom - 6;
                                else if (Left() != null)
                                    flag4 = physicsObject.position.x < right - 6;
                                else if (Right() != null)
                                    flag4 = physicsObject.position.x > left + 6;
                                if (flag4)
                                {
                                    StartTransporting(physicsObject);
                                    continue;
                                }
                                continue;
                            }
                            continue;
                        default:
                            continue;
                    }
                }
            }
            for (_transportingIndex = 0; _transportingIndex < _transporting.Count; ++_transportingIndex)
            {
                PipeBundle bundle = _transporting[_transportingIndex];
                Thing thing = bundle.thing;
                if (thing is PhysicsObject)
                {
                    PhysicsObject o = thing as PhysicsObject;
                    o.updatePhysics = false;
                    o.overfollow = 0.5f;
                    o.position = new Vec2(-5000f, -1000f);
                    o.cameraPositionOverride = bundle.cameraPosition;
                    bundle.cameraPosition = Lerp.Vec2(bundle.cameraPosition, _oppositeEnd.position, travelLength / 10f);
                    if ((bundle.cameraPosition - _oppositeEnd.position).length < 4)
                    {
                        FinishTransporting(o, bundle);
                        SFX.Play("pipeOut", pitch: Rando.Float(-0.1f, 0.1f));
                    }
                }
            }
            base.Update();
        }

        private void FinishTransporting(
          PhysicsObject o,
          PipeBundle bundle,
          bool pDoingRagthing = false)
        {
            o.updatePhysics = true;
            o.overfollow = 0f;
            if (bundle != null)
                o.position = bundle.cameraPosition;
            o.cameraPositionOverride = Vec2.Zero;
            o.inPipe = false;
            if (bundle == null)
                return;
            int index = _transporting.IndexOf(bundle);
            if (index < 0)
                return;
            if (!pDoingRagthing && o is RagdollPart)
            {
                Ragdoll r = (o as RagdollPart).doll;
                if (r != null)
                {
                    FinishTransporting(r.part1, _transporting.FirstOrDefault(x => x.thing == r.part1), true);
                    FinishTransporting(r.part2, _transporting.FirstOrDefault(x => x.thing == r.part2), true);
                    FinishTransporting(r.part3, _transporting.FirstOrDefault(x => x.thing == r.part3), true);
                    r.part1.position = new Vec2(r.part2.x + Rando.Float(-4f, 4f), r.part2.y + Rando.Float(-4f, 4f));
                    r.part3.position = new Vec2(r.part2.x + Rando.Float(-4f, 4f), r.part2.y + Rando.Float(-4f, 4f));
                    return;
                }
            }
            else
            {
                if (o is RagdollPart)
                    (o as RagdollPart)._lastReasonablePosition = o.position;
                _oppositeEnd.PipeOut(o);
                _transporting.RemoveAt(index);
            }
            if (index > _transportingIndex)
                return;
            --_transportingIndex;
        }

        public override void EditorAdded()
        {
            Dictionary<Direction, PipeTileset> neighbors = GetNeighbors();
            if (!AttemptObviousConnection(neighbors) && _lastAdd != null)
            {
                if (Up(neighbors) == _lastAdd && Up(neighbors).ReadyForConnection())
                    MakeConnection(Up(neighbors));
                else if (Down(neighbors) == _lastAdd && Down(neighbors).ReadyForConnection())
                    MakeConnection(Down(neighbors));
                else if (Left(neighbors) == _lastAdd && Left(neighbors).ReadyForConnection())
                    MakeConnection(Left(neighbors));
                else if (Right(neighbors) == _lastAdd && Right(neighbors).ReadyForConnection())
                    MakeConnection(Right(neighbors));
            }
            searchUp = searchDown = searchLeft = searchRight = false;
            if (Up() != null)
            {
                searchUp = true;
                Up().searchDown = true;
            }
            if (Down() != null)
            {
                searchDown = true;
                Down().searchUp = true;
            }
            if (Left() != null)
            {
                searchLeft = true;
                Left().searchRight = true;
            }
            if (Right() != null)
            {
                searchRight = true;
                Right().searchLeft = true;
            }
            TestValidity();
            _lastAdd = this;
        }

        public override void EditorFlip(bool pVertical)
        {
            if (pVertical)
            {
                bool searchUp = this.searchUp;
                this.searchUp = searchDown;
                searchDown = searchUp;
            }
            else
            {
                bool searchLeft = this.searchLeft;
                this.searchLeft = searchRight;
                searchRight = searchLeft;
            }
        }

        public override void EditorRemoved()
        {
            if (Up() != null)
            {
                PipeTileset pipeTileset = Up();
                BreakConnection(Up());
                pipeTileset.TestValidity();
            }
            if (Down() != null)
            {
                PipeTileset pipeTileset = Down();
                BreakConnection(Down());
                pipeTileset.TestValidity();
            }
            if (Left() != null)
            {
                PipeTileset pipeTileset = Left();
                BreakConnection(Left());
                pipeTileset.TestValidity();
            }
            if (Right() != null)
            {
                PipeTileset pipeTileset = Right();
                BreakConnection(Right());
                pipeTileset.TestValidity();
            }
            TestValidity();
            _lastAdd = null;
        }

        public override void EditorObjectsChanged()
        {
            ReConnect();
            TestValidity();
            UpdateConnectionFrame();
            base.EditorObjectsChanged();
        }

        public override void EditorRender()
        {
            if (!(Level.current is Editor) || !((Level.current as Editor).placementType is PipeTileset))
                return;
            alpha = 0.6f;
            Draw();
            alpha = 1f;
            base.EditorRender();
        }

        public override void OnEditorLoaded()
        {
            ReConnect();
            TestValidity();
        }

        public override void Update()
        {
            if (!_initializedConnections)
            {
                ReConnect();
                _initializedConnections = true;
            }
            else if (!_testedValidity)
                TestValidity();
            if (_failBullets > 0)
                --_failBullets;
            if (connections.Count == 1)
                UpdatePipeEnd();
            ++framesSincePipeout;
            if (isEntryPipe)
                UpdateEntryPipe();
            if (connections.Count == 1)
                UpdatePipeEndLate();
            ++_initializedBackground;
            if (_initializedBackground == 2 && !(Level.current is Editor))
            {
                if (IsBackground())
                    _collisionOffset = new Vec2(Vec2.MinValue);
                if (_validPipe && connections.Count == 1 && (Left() != null || Right() != null) && Level.CheckPoint<Block>(position, this) != null)
                    solid = false;
            }
            base.Update();
        }

        public override void Draw()
        {
            Color color = graphic.color;
            if (IsBackground())
            {
                depth = pipeDepth - 1.8f;
                graphic.color = color * 0.5f;
                graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, byte.MaxValue);
            }
            else
            {
                depth = (Depth)pipeDepth;
                if (Left() != null && Left().IsBackground())
                {
                    int frame = _sprite.frame;
                    _sprite.frame = 22;
                    _sprite.flipH = true;
                    Graphics.Draw(_sprite, x - 16f, y, depth + 5);
                    _sprite.flipH = false;
                    _sprite.frame = frame;
                }
                if (Right() != null && Right().IsBackground())
                {
                    int frame = _sprite.frame;
                    _sprite.frame = 22;
                    Graphics.Draw(_sprite, x + 16f, y, depth + 5);
                    _sprite.frame = frame;
                }
                if (Up() != null && Up().IsBackground())
                {
                    int frame = _sprite.frame;
                    _sprite.frame = 22;
                    _sprite.angleDegrees = -90f;
                    Graphics.Draw(_sprite, x, y - 16f, depth + 5);
                    _sprite.angleDegrees = 0f;
                    _sprite.frame = frame;
                }
                if (Down() != null && Down().IsBackground())
                {
                    int frame = _sprite.frame;
                    _sprite.frame = 22;
                    _sprite.angleDegrees = 90f;
                    _sprite.flipV = true;
                    Graphics.Draw(_sprite, x, y + 16f, depth + 5);
                    _sprite.angleDegrees = 0f;
                    _sprite.flipV = false;
                    _sprite.frame = frame;
                }
            }
            if (isEntryPipe && !(Level.current is Editor))
                DrawParticles();
            base.Draw();
            graphic.color = color;
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with is PhysicalBullet)
            {
                Hit((with as PhysicalBullet).bullet, (with as PhysicalBullet).bullet.currentTravel);
                if (entered)
                    return;
            }
            base.OnSolidImpact(with, from);
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            entered = false;
            if (connections.Count == 1 && (!(bool)trapdoor || bullet.ammo.penetration >= thickness) && ((hitPos.x - 8 < left && Right() != null && bullet.travelDirNormalized.x > 0.3f || hitPos.x + 8 > right && Left() != null && bullet.travelDirNormalized.x < -0.3f) && hitPos.y > top && hitPos.y < bottom || (hitPos.y - 8 < top && Down() != null && bullet.travelDirNormalized.y > 0.3f || hitPos.y + 8 > bottom && Up() != null && bullet.travelDirNormalized.y < -0.3f) && hitPos.x > left && hitPos.x < right) && oppositeEnd != null)
            {
                float rng = bullet._totalLength - (bullet._actualStart - hitPos).length;
                float num = 0f;
                if (rng > 0)
                {
                    bool flag1 = false;
                    bool flag2 = bullet.ammo is ATMissile;
                    if (bullet.ammo != null && !bullet.ammo.flawlessPipeTravel && !flag2 && hasKinks)
                    {
                        flag1 = true;
                        ++_failBullets;
                        if (_failBullets > 5)
                            flag1 = false;
                    }
                    if (flag2)
                        rng = bullet.ammo.range;
                    if ((bool)oppositeEnd.trapdoor && bullet.ammo.penetration < oppositeEnd.thickness)
                        flag1 = true;
                    if (!flag1)
                    {
                        if (oppositeEnd.Left() != null)
                            bullet.DoRebound(oppositeEnd.endOffset, 0f + num, rng);
                        if (oppositeEnd.Right() != null)
                            bullet.DoRebound(oppositeEnd.endOffset, 180f + num, rng);
                        if (oppositeEnd.Up() != null)
                            bullet.DoRebound(oppositeEnd.endOffset, 270f + num, rng);
                        if (oppositeEnd.Down() != null)
                            bullet.DoRebound(oppositeEnd.endOffset, 90f + num, rng);
                    }
                    entered = true;
                    return true;
                }
            }
            return base.Hit(bullet, hitPos);
        }

        private void ReConnect()
        {
            if (!searchUp && !searchDown && !searchLeft && !searchRight)
                return;
            connections.Clear();
            _pipeLeft = null;
            _pipeRight = null;
            _pipeUp = null;
            _pipeDown = null;
            Dictionary<Direction, PipeTileset> neighbors = GetNeighbors();
            if (searchUp && Up(neighbors) != null)
                MakeConnection(Up(neighbors));
            if (searchDown && Down(neighbors) != null)
                MakeConnection(Down(neighbors));
            if ((searchLeft || flipHorizontal && searchRight) && Left(neighbors) != null)
                MakeConnection(Left(neighbors));
            if (!searchRight && (!flipHorizontal || !searchLeft) || Right(neighbors) == null)
                return;
            MakeConnection(Right(neighbors));
        }

        public PipeTileset Up(
          Dictionary<Direction, PipeTileset> pNeighbors = null)
        {
            if (pNeighbors == null)
                return _pipeUp;
            PipeTileset pipeTileset;
            pNeighbors.TryGetValue(Direction.Up, out pipeTileset);
            return pipeTileset;
        }

        public PipeTileset Down(
          Dictionary<Direction, PipeTileset> pNeighbors = null)
        {
            if (pNeighbors == null)
                return _pipeDown;
            PipeTileset pipeTileset;
            pNeighbors.TryGetValue(Direction.Down, out pipeTileset);
            return pipeTileset;
        }

        public PipeTileset Left(
          Dictionary<Direction, PipeTileset> pNeighbors = null)
        {
            if (pNeighbors == null)
                return _pipeLeft;
            PipeTileset pipeTileset;
            pNeighbors.TryGetValue(Direction.Left, out pipeTileset);
            return pipeTileset;
        }

        public PipeTileset Right(
          Dictionary<Direction, PipeTileset> pNeighbors = null)
        {
            if (pNeighbors == null)
                return _pipeRight;
            PipeTileset pipeTileset;
            pNeighbors.TryGetValue(Direction.Right, out pipeTileset);
            return pipeTileset;
        }

        protected virtual Dictionary<Direction, PipeTileset> GetNeighbors()
        {
            Dictionary<Direction, PipeTileset> neighbors = new Dictionary<Direction, PipeTileset>();
            PipeTileset pipeTileset1 = Level.CheckPointAll<PipeTileset>(x, y - 16f).Where(x => x.group == group).FirstOrDefault();
            if (pipeTileset1 != null)
                neighbors[Direction.Up] = pipeTileset1;
            PipeTileset pipeTileset2 = Level.CheckPointAll<PipeTileset>(x, y + 16f).Where(x => x.group == group).FirstOrDefault();
            if (pipeTileset2 != null)
                neighbors[Direction.Down] = pipeTileset2;
            PipeTileset pipeTileset3 = Level.CheckPointAll<PipeTileset>(x - 16f, y).Where(x => x.group == group).FirstOrDefault();
            if (pipeTileset3 != null)
                neighbors[Direction.Left] = pipeTileset3;
            PipeTileset pipeTileset4 = Level.CheckPointAll<PipeTileset>(x + 16f, y).Where(x => x.group == group).FirstOrDefault();
            if (pipeTileset4 != null)
                neighbors[Direction.Right] = pipeTileset4;
            return neighbors;
        }

        public bool ReadyForConnection() => connections.Count <= 1;

        private PipeTileset Travel(PipeTileset pTowards)
        {
            PipeTileset pipeTileset = this;
            int num = 0;
            while (pTowards != this)
            {
                ++num;
                if (num <= byte.MaxValue)
                {
                    if (pTowards._pipeLeft != null && (pTowards._pipeUp != null || pTowards._pipeDown != null) || pTowards._pipeRight != null && (pTowards._pipeUp != null || pTowards._pipeDown != null))
                        hasKinks = true;
                    if (pTowards.connections.Count > 2)
                        pTowards.ReConnect();
                    if (pTowards._pipeUp != null && pTowards._pipeUp != pipeTileset)
                    {
                        pipeTileset = pTowards;
                        pTowards = pTowards._pipeUp;
                    }
                    else if (pTowards._pipeDown != null && pTowards._pipeDown != pipeTileset)
                    {
                        pipeTileset = pTowards;
                        pTowards = pTowards._pipeDown;
                    }
                    else if (pTowards._pipeLeft != null && pTowards._pipeLeft != pipeTileset)
                    {
                        pipeTileset = pTowards;
                        pTowards = pTowards._pipeLeft;
                    }
                    else if (pTowards._pipeRight != null && pTowards._pipeRight != pipeTileset)
                    {
                        pipeTileset = pTowards;
                        pTowards = pTowards._pipeRight;
                    }
                    else
                        break;
                }
                else
                    break;
            }
            return pTowards;
        }

        private void MakeConnection(PipeTileset pWith)
        {
            if (pWith.y == y)
            {
                if (pWith.x > x)
                {
                    pWith.connections[Direction.Left] = this;
                    pWith._pipeLeft = this;
                    connections[Direction.Right] = pWith;
                    _pipeRight = pWith;
                }
                if (pWith.x < x)
                {
                    pWith.connections[Direction.Right] = this;
                    pWith._pipeRight = this;
                    connections[Direction.Left] = pWith;
                    _pipeLeft = pWith;
                }
            }
            else if (pWith.x == x)
            {
                if (pWith.y > y)
                {
                    pWith.connections[Direction.Up] = this;
                    pWith._pipeUp = this;
                    connections[Direction.Down] = pWith;
                    _pipeDown = pWith;
                }
                if (pWith.y < y)
                {
                    pWith.connections[Direction.Down] = this;
                    pWith._pipeDown = this;
                    connections[Direction.Up] = pWith;
                    _pipeUp = pWith;
                }
            }
            UpdateConnectionFrame();
            pWith.UpdateConnectionFrame();
        }

        private void BreakConnection(PipeTileset pWith)
        {
            if (pWith.y == y)
            {
                if (pWith.x > x)
                {
                    pWith.connections.Remove(Direction.Left);
                    pWith.searchLeft = false;
                    connections.Remove(Direction.Right);
                    _pipeRight = null;
                }
                if (pWith.x < x)
                {
                    pWith.connections.Remove(Direction.Right);
                    pWith._pipeRight = null;
                    pWith.searchRight = false;
                    connections.Remove(Direction.Left);
                    _pipeLeft = null;
                }
            }
            else if (pWith.x == x)
            {
                if (pWith.y > y)
                {
                    pWith.connections.Remove(Direction.Up);
                    pWith._pipeUp = null;
                    pWith.searchUp = false;
                    connections.Remove(Direction.Down);
                    _pipeDown = null;
                }
                if (pWith.y < y)
                {
                    pWith.connections.Remove(Direction.Down);
                    pWith._pipeDown = null;
                    pWith.searchDown = false;
                    connections.Remove(Direction.Up);
                    _pipeUp = null;
                }
            }
            UpdateConnectionFrame();
            pWith.UpdateConnectionFrame();
        }

        private void UpdateConnectionFrame()
        {
            OnUpdateConnectionFrame();
            if (connections.Count == 1)
            {
                _drawBlockOverlay = Level.CheckPoint<Block>(position, this) != null;
                _foregroundDraw = true;
            }
            else
                _foregroundDraw = false;
            switch (frame) // switched
            {
                case 0:
                    collisionSize = new Vec2(14f, 14f);
                    collisionOffset = new Vec2(-7f, -7f);
                    break;
                case 1:
                    collisionSize = new Vec2(15f, 15f);
                    collisionOffset = new Vec2(-7f, -7f);
                    break;
                case 2:
                    collisionSize = new Vec2(15f, 15f);
                    collisionOffset = new Vec2(-8f, -7f);
                    break;
                case 3:
                    collisionSize = new Vec2(16f, 14f);
                    collisionOffset = new Vec2(-8f, -7f);
                    break;
                case 5:
                    collisionSize = new Vec2(15f, 15f);
                    collisionOffset = new Vec2(-7f, -8f);
                    break;
                case 6:
                    collisionSize = new Vec2(15f, 15f);
                    collisionOffset = new Vec2(-8f, -8f);
                    break;
                case 7:
                    collisionSize = new Vec2(14f, 16f);
                    collisionOffset = new Vec2(-7f, -8f);
                    break;
                case 8:
                    collisionSize = new Vec2(14f, 15f);
                    collisionOffset = new Vec2(-7f, -7f);
                    break;
                case 9:
                    collisionSize = new Vec2(14f, 15f);
                    collisionOffset = new Vec2(-7f, -8f);
                    break;
                case 10:
                    collisionSize = new Vec2(15f, 14f);
                    collisionOffset = new Vec2(-7f, -7f);
                    break;
                case 11:
                    collisionSize = new Vec2(15f, 14f);
                    collisionOffset = new Vec2(-8f, -7f);
                    break;
            }
            if (IsBackground())
            {
                solid = false;
                thickness = 0f;
                physicsMaterial = PhysicsMaterial.Default;
            }
            else
            {
                solid = true;
                thickness = 3f;
                physicsMaterial = PhysicsMaterial.Metal;
            }
        }

        private void OnUpdateConnectionFrame()
        {
            if (connections.Count == 0)
                _sprite.frame = 0;
            else if (connections.Count == 1)
            {
                if (Up() != null)
                    _sprite.frame = 9;
                else if (Down() != null)
                    _sprite.frame = 8;
                else if (Left() != null)
                {
                    _sprite.frame = 11;
                }
                else
                {
                    if (Right() == null)
                        return;
                    _sprite.frame = 10;
                }
            }
            else if (Up() != null && Right() != null)
                _sprite.frame = 5;
            else if (Up() != null && Left() != null)
                _sprite.frame = 6;
            else if (Up() != null && Down() != null)
                _sprite.frame = 7;
            else if (Down() != null && Right() != null)
                _sprite.frame = 1;
            else if (Down() != null && Left() != null)
            {
                _sprite.frame = 2;
            }
            else
            {
                if (Left() == null || Right() == null)
                    return;
                _sprite.frame = 3;
            }
        }

        private bool AttemptObviousConnection(
          Dictionary<Direction, PipeTileset> pNeighbors)
        {
            bool flag = false;
            if (Up(pNeighbors) == null && Down(pNeighbors) == null)
            {
                if (Left(pNeighbors) != null && Left(pNeighbors).ReadyForConnection())
                {
                    MakeConnection(Left(pNeighbors));
                    flag = true;
                }
                if (Right(pNeighbors) != null && Right(pNeighbors).ReadyForConnection())
                {
                    MakeConnection(Right(pNeighbors));
                    flag = true;
                }
            }
            if (Left(pNeighbors) == null && Right(pNeighbors) == null)
            {
                if (Up(pNeighbors) != null && Up(pNeighbors).ReadyForConnection())
                {
                    MakeConnection(Up(pNeighbors));
                    flag = true;
                }
                if (Down(pNeighbors) != null && Down(pNeighbors).ReadyForConnection())
                {
                    MakeConnection(Down(pNeighbors));
                    flag = true;
                }
            }
            if (!flag)
            {
                List<PipeTileset> pipeTilesetList = new List<PipeTileset>();
                foreach (KeyValuePair<Direction, PipeTileset> pNeighbor in pNeighbors)
                {
                    if (pNeighbor.Value.ReadyForConnection())
                        pipeTilesetList.Add(pNeighbor.Value);
                }
                if (pipeTilesetList.Count <= 0 || pipeTilesetList.Count > 2)
                    return false;
                flag = true;
                if (_lastAdd != null && (pipeTilesetList[0] == _lastAdd || pipeTilesetList.Count > 1 && pipeTilesetList[1] == _lastAdd))
                {
                    MakeConnection(_lastAdd);
                }
                else
                {
                    MakeConnection(pipeTilesetList[0]);
                    if (pipeTilesetList.Count == 2)
                        MakeConnection(pipeTilesetList[1]);
                }
            }
            return flag;
        }

        private void TestValidity()
        {
            int num1 = 0;
            int num2 = 0;
            PipeTileset pTowards1 = null;
            PipeTileset pTowards2 = null;
            PipeTileset pipeTileset1 = null;
            PipeTileset pipeTileset2 = null;
            if (connections.Count == 1)
            {
                if (_pipeUp != null)
                {
                    ++num2;
                    pTowards1 = _pipeUp;
                    pipeTileset2 = this;
                }
                else if (_pipeDown != null)
                {
                    ++num1;
                    pTowards1 = _pipeDown;
                    pipeTileset2 = this;
                }
                else if (_pipeLeft != null)
                {
                    pTowards1 = _pipeLeft;
                    pipeTileset2 = this;
                }
                else if (_pipeRight != null)
                {
                    pTowards1 = _pipeRight;
                    pipeTileset2 = this;
                }
            }
            else
            {
                if (_pipeUp != null)
                {
                    if (pTowards1 == null)
                        pTowards1 = _pipeUp;
                    else
                        pTowards2 = _pipeUp;
                }
                if (_pipeDown != null)
                {
                    if (pTowards1 == null)
                        pTowards1 = _pipeDown;
                    else
                        pTowards2 = _pipeDown;
                }
                if (_pipeLeft != null)
                {
                    if (pTowards1 == null)
                        pTowards1 = _pipeLeft;
                    else
                        pTowards2 = _pipeLeft;
                }
                if (_pipeRight != null)
                {
                    if (pTowards1 == null)
                        pTowards1 = _pipeRight;
                    else
                        pTowards2 = _pipeRight;
                }
            }
            if (pTowards1 != null)
            {
                pipeTileset1 = Travel(pTowards1);
                if (pipeTileset1 != this && pipeTileset1.connections.Count == 1)
                {
                    if (pipeTileset1._pipeUp == null && pipeTileset1._pipeDown != null)
                        ++num1;
                    if (pipeTileset1._pipeDown == null && pipeTileset1._pipeUp != null)
                        ++num2;
                }
            }
            if (pTowards2 != null)
            {
                pipeTileset2 = Travel(pTowards2);
                if (pipeTileset2 != this && pipeTileset2.connections.Count == 1)
                {
                    if (pipeTileset2._pipeUp == null && pipeTileset2._pipeDown != null)
                        ++num1;
                    if (pipeTileset2._pipeDown == null && pipeTileset2._pipeUp != null)
                        ++num2;
                }
            }
            int num3;
            switch (num1)
            {
                case 1:
                    num3 = num2 == 1 ? 1 : 0;
                    break;
                case 2:
                    num3 = 1;
                    break;
                default:
                    num3 = 0;
                    break;
            }
            _validPipe = num3 != 0;
            _validPipe = connections.Count > 0;
            if (pipeTileset1 != null)
                pipeTileset1._validPipe = _validPipe;
            if (pipeTileset2 != null)
                pipeTileset2._validPipe = _validPipe;
            if (_validPipe)
            {
                pipeTileset1._oppositeEnd = pipeTileset2;
                pipeTileset2._oppositeEnd = pipeTileset1;
                PipeTileset pipeTileset3 = pipeTileset1;
                PipeTileset pipeTileset4 = pipeTileset2;
                Vec2 vec2 = pipeTileset1.position - pipeTileset2.position;
                double length;
                float num4 = (float)(length = vec2.length);
                pipeTileset4.travelLength = (float)length;
                double num5 = num4;
                pipeTileset3.travelLength = (float)num5;
            }
            _testedValidity = true;
        }

        private void DrawParticles()
        {
            if (partWait == -100)
                partRot = Rando.Float(10f);
            Vec2 vec2_1 = endNormal;
            Vec2 vec2_2 = vec2_1.Rotate(Maths.DegToRad(Up() != null || Right() != null ? 90f : -90f), Vec2.Zero);
            --partWait;
            if (partWait <= 0)
            {
                PipeParticle pipeParticle = null;
                foreach (PipeParticle particle in _particles)
                {
                    if (particle.alpha >= 1)
                    {
                        pipeParticle = particle;
                        break;
                    }
                }
                if (pipeParticle == null)
                {
                    pipeParticle = new PipeParticle();
                    _particles.Add(pipeParticle);
                }
                pipeParticle.position = position + endNormal * 20f + Maths.AngleToVec(partRot) * (10f + Rando.Float(24f)) * vec2_2;
                pipeParticle.alpha = 0f;
                pipeParticle.velocity = Vec2.Zero;
                partWait = 5;
                partRot += 1.72152f;
            }
            for (int index = 0; index < _particles.Count; ++index)
            {
                if (_particles[index].alpha < 1)
                {
                    Vec2 vec2_3 = position - _particles[index].position;
                    _particles[index].velocity -= endNormal * 0.03f;
                    _particles[index].position -= vec2_3 * vec2_2 * 0.07f;
                    Graphics.DrawLine(_particles[index].position, _particles[index].position + _particles[index].velocity * 3f, Color.White * _particles[index].alpha, 0.75f, depth - 10);
                    _particles[index].position += _particles[index].velocity;
                    _particles[index].alpha += 0.016f;
                    vec2_1 = _particles[index].position * endNormal - position * endNormal;
                    if (vec2_1.length < 2)
                        _particles[index].alpha = 1f;
                }
            }
        }

        public void OnDrawLayer(Layer pLayer)
        {
            if (!_foregroundDraw || pLayer != Layer.Blocks)
                return;
            int frame = _sprite.frame;
            _flapLerp = Lerp.FloatSmooth(_flapLerp, _flap, 0.25f);
            _flap = Lerp.Float(_flap, 0f, 0.15f);
            if (_drawBlockOverlay)
            {
                _sprite.frame += 8;
                Graphics.Draw(_sprite, position.x, position.y, (Depth)0.5f);
            }
            if (trapdoor.value)
            {
                Vec2 center = _sprite.center;
                _sprite.frame = 20;
                float num = _drawBlockOverlay ? 10f : 9f;
                if (Left() != null)
                {
                    _sprite.center = new Vec2(2f, 0f);
                    _sprite.angleDegrees = (float)(0 - _flapLerp * 90);
                    Graphics.Draw(_sprite, position.x + num, position.y - 9f, (Depth)0.5f);
                    _sprite.center = new Vec2(9f, 9f);
                    _sprite.angleDegrees = 0f;
                    _sprite.frame = 21;
                    Graphics.Draw(_sprite, position.x + (num - 1f), position.y - 9f, (Depth)0.4f);
                }
                else if (Right() != null)
                {
                    _sprite.center = new Vec2(2f, 18f);
                    _sprite.angleDegrees = (float)(180 + _flapLerp * 90);
                    _sprite.flipV = true;
                    Graphics.Draw(_sprite, position.x - num, position.y - 9f, (Depth)0.5f);
                    _sprite.center = new Vec2(9f, 9f);
                    _sprite.angleDegrees = 0f;
                    _sprite.frame = 21;
                    _sprite.flipH = true;
                    Graphics.Draw(_sprite, position.x - (num - 1f), position.y - 9f, (Depth)0.4f);
                }
                else if (Up() != null)
                {
                    _sprite.center = new Vec2(2f, 0f);
                    _sprite.angleDegrees = (float)(90 - _flapLerp * 90);
                    Graphics.Draw(_sprite, position.x + 9f, position.y + num, (Depth)0.5f);
                    _sprite.center = new Vec2(9f, 9f);
                    _sprite.angleDegrees = 90f;
                    _sprite.frame = 21;
                    Graphics.Draw(_sprite, position.x + 9f, position.y + (num - 1f), (Depth)0.4f);
                }
                else if (Down() != null)
                {
                    _sprite.center = new Vec2(2f, 18f);
                    _sprite.angleDegrees = (float)(270 + _flapLerp * 90);
                    _sprite.flipV = true;
                    Graphics.Draw(_sprite, position.x + 9f, position.y - num, (Depth)0.5f);
                    _sprite.center = new Vec2(9f, 9f);
                    _sprite.angleDegrees = 90f;
                    _sprite.frame = 21;
                    _sprite.flipH = true;
                    Graphics.Draw(_sprite, position.x + 9f, position.y - (num - 1f), (Depth)0.4f);
                }
                _sprite.flipV = false;
                _sprite.flipH = false;
                _sprite.center = center;
            }
            _sprite.frame = frame;
        }

        public void FlapPipe() => _flap = 1.9f;

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
        }

        private class PipeBundle
        {
            public Thing thing;
            public Vec2 cameraPosition;
        }

        private class PipeParticle
        {
            public Vec2 position;
            public Vec2 velocity;
            public float alpha;
        }
    }
}
