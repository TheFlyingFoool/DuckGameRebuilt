// Decompiled with JetBrains decompiler
// Type: DuckGame.PipeTileset
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class PipeTileset : Block, IDontMove, IDrawToDifferentLayers
    {
        public EditorProperty<bool> trapdoor = new EditorProperty<bool>(false);
        public EditorProperty<bool> background = new EditorProperty<bool>(false);
        private Dictionary<PipeTileset.Direction, PipeTileset> connections = new Dictionary<PipeTileset.Direction, PipeTileset>();
        public SpriteMap _sprite;
        public float pipeDepth;
        private bool searchUp;
        private bool searchDown;
        private bool searchLeft;
        private bool searchRight;
        private PipeTileset _pipeUp;
        private PipeTileset _pipeDown;
        private PipeTileset _pipeLeft;
        private PipeTileset _pipeRight;
        private bool _validPipe;
        private PipeTileset _oppositeEnd;
        public float travelLength;
        private HashSet<ITeleport> _objectsInPipes = new HashSet<ITeleport>();
        private List<PipeTileset.PipeBundle> _transporting = new List<PipeTileset.PipeBundle>();
        private HashSet<PhysicsObject> _pipingOut = new HashSet<PhysicsObject>();
        private List<ITeleport> _removeFromPipe = new List<ITeleport>();
        private List<MaterialThing> _colliding;
        private int framesSincePipeout;
        private int _transportingIndex;
        private bool _initializedConnections;
        private bool _testedValidity;
        private int _initializedBackground;
        private bool entered;
        private int _failBullets;
        private static PipeTileset _lastAdd;
        public bool hasKinks;
        public bool _foregroundDraw;
        private bool _drawBlockOverlay;
        private List<PipeTileset.PipeParticle> _particles = new List<PipeTileset.PipeParticle>();
        private float partRot;
        private int partWait = -100;
        public float _flapLerp;
        public float _flap;

        public bool IsBackground() => this.connections.Count > 1 && this.background.value;

        public PipeTileset(float x, float y, string pSprite)
          : base(x, y)
        {
            this._editorName = "Pipe";
            this.editorTooltip = "Travel through pipes!";
            this.layer = Layer.Game;
            this.depth = (Depth)0.9f;
            this.thickness = 3f;
            this._sprite = new SpriteMap(pSprite, 18, 18);
            this.graphic = _sprite;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.center = new Vec2(9f, 9f);
            this._sprite.CenterOrigin();
            this._collisionSize = new Vec2(16f, 16f);
            this._collisionOffset = new Vec2(-8f, -8f);
            this._sprite.frame = 0;
            this.placementLayerOverride = Layer.Foreground;
        }

        public Vec2 endOffset => this.position + this.endNormal * 9f;

        public Vec2 endNormal
        {
            get
            {
                if (this.Up() != null)
                    return new Vec2(0.0f, 1f);
                if (this.Down() != null)
                    return new Vec2(0.0f, -1f);
                if (this.Left() != null)
                    return new Vec2(1f, 0.0f);
                return this.Right() != null ? new Vec2(-1f, 0.0f) : this.position;
            }
        }

        public bool MovingIntoPipe(Vec2 pPosition, Vec2 pVelocity, float pThresh = 2f)
        {
            bool flag = false;
            if (endNormal.x != 0.0 && pVelocity.x != 0.0)
            {
                if (Math.Sign(pVelocity.x) != Math.Sign(this.endNormal.x))
                    flag = true;
            }
            else if (endNormal.y != 0.0 && pVelocity.y != 0.0 && Math.Sign(pVelocity.y) != Math.Sign(this.endNormal.y))
                flag = true;
            return flag && (this.Left() != null && pPosition.x < (double)this.right + (double)pThresh && pPosition.y <= (double)this.bottom + (double)pThresh && pPosition.y >= (double)this.top - (double)pThresh || this.Right() != null && pPosition.x < (double)this.left - (double)pThresh && pPosition.y <= (double)this.bottom + (double)pThresh && pPosition.y >= (double)this.top - (double)pThresh || this.Up() != null && pPosition.y < (double)this.bottom + (double)pThresh && pPosition.x <= (double)this.right + (double)pThresh && pPosition.x >= (double)this.left - (double)pThresh || this.Down() != null && pPosition.y > (double)this.top - (double)pThresh && pPosition.x <= (double)this.right + (double)pThresh && pPosition.x >= (double)this.left - (double)pThresh);
        }

        public PipeTileset oppositeEnd => this._oppositeEnd;

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            if (this.connections.Count > 0)
            {
                binaryClassChunk.AddProperty("up", this.Up() != null);
                binaryClassChunk.AddProperty("down", this.Down() != null);
                binaryClassChunk.AddProperty("left", this.Left() != null);
                binaryClassChunk.AddProperty("right", this.Right() != null);
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
            this.searchUp = node.GetProperty<bool>("up");
            this.searchDown = node.GetProperty<bool>("down");
            this.searchLeft = node.GetProperty<bool>("left");
            this.searchRight = node.GetProperty<bool>("right");
            this._sprite.frame = node.GetProperty<int>("pipeFrame");
            return true;
        }

        public bool isEntryPipe => this._validPipe && this.connections.Count == 1 && !(bool)this.trapdoor;

        private void PipeOut(PhysicsObject d)
        {
            this.FlapPipe();
            if (d is Duck)
            {
                (d as Duck).immobilized = false;
                (d as Duck).pipeOut = 6;
                (d as Duck).CancelFlapping();
            }
            bool flag = false;
            d.hSpeed = 0.0f;
            d.clip.Clear();
            if (this.Down() != null)
            {
                d.position = this.position - new Vec2(0.0f, 10f);
                if (d is Duck)
                {
                    (d as Duck).jumping = true;
                    (d as Duck).slamWait = 4;
                }
                d.vSpeed = -6f;
                if (d is RagdollPart)
                    d.hSpeed += Rando.Float(-1f, 1f);
                for (int index = 0; index < 6; ++index)
                {
                    SmallSmoke smallSmoke = SmallSmoke.New(this.x + Rando.Float(-4f, 4f), this.y + Rando.Float(-4f, 4f));
                    smallSmoke.velocity = new Vec2(Rando.Float(-0.5f, 0.5f), Rando.Float(0.0f, -0.5f));
                    Level.Add(smallSmoke);
                }
                if (Network.isActive && this.framesSincePipeout > 2)
                {
                    Send.Message(new NMPipeOut(new Vec2(this.x, this.y), 0));
                    this.framesSincePipeout = 0;
                }
                if (d is Duck && Level.CheckLine<Block>(this.position - new Vec2(0.0f, 16f), this.position - new Vec2(0.0f, 32f)) != null)
                {
                    Duck duck = d as Duck;
                    duck.position = this.position - new Vec2(0.0f, 16f);
                    duck.GoRagdoll();
                    flag = true;
                }
            }
            else if (this.Left() != null || this.Right() != null)
            {
                d.position = this.position + new Vec2(this.Left() != null ? 12f : -12f, -2f);
                d.vSpeed = -0.0f;
                if (this.Left() != null)
                    d.hSpeed = 6f;
                else
                    d.hSpeed = -6f;
                for (int index = 0; index < 6; ++index)
                {
                    SmallSmoke smallSmoke = SmallSmoke.New(this.x + (this.Left() != null ? 12f : -12f) + Rando.Float(-4f, 4f), this.y + Rando.Float(-4f, 4f));
                    if (this.Left() != null)
                        smallSmoke.velocity = new Vec2(Rando.Float(0.2f, 0.7f), Rando.Float(-0.5f, 0.5f));
                    else
                        smallSmoke.velocity = new Vec2(Rando.Float(-0.7f, -0.2f), Rando.Float(-0.5f, 0.5f));
                    Level.Add(smallSmoke);
                }
                if (Network.isActive && this.framesSincePipeout > 2)
                {
                    Send.Message(new NMPipeOut(new Vec2(this.x + (this.Left() != null ? 12f : -12f), this.y), this.Left() != null ? (byte)1 : (byte)3));
                    this.framesSincePipeout = 0;
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
                d.position = this.position + new Vec2(0.0f, 4f);
                d.vSpeed = 5f;
                for (int index = 0; index < 6; ++index)
                {
                    SmallSmoke smallSmoke = SmallSmoke.New(this.x + Rando.Float(-4f, 4f), this.y + 12f + Rando.Float(-4f, 4f));
                    smallSmoke.velocity = new Vec2(Rando.Float(-0.5f, 0.5f), Rando.Float(0.2f, 0.7f));
                    Level.Add(smallSmoke);
                }
                if (Network.isActive && this.framesSincePipeout > 2)
                {
                    Send.Message(new NMPipeOut(new Vec2(this.x, this.y + 12f), 2));
                    this.framesSincePipeout = 0;
                }
                if (d is Duck && (Level.CheckLine<Block>(this.position + new Vec2(0.0f, 16f), this.position + new Vec2(0.0f, 32f)) != null || Level.CheckLine<IPlatform>(this.position + new Vec2(0.0f, 16f), this.position + new Vec2(0.0f, 32f)) != null))
                {
                    Duck duck = d as Duck;
                    duck.position = this.position + new Vec2(0.0f, 16f);
                    duck.GoRagdoll();
                    flag = true;
                }
            }
            d.Ejected(this);
            if (!flag)
            {
                this.Clip(d);
                d.skipClip = true;
                this._pipingOut.Add(d);
            }
            else
                d.skipClip = false;
        }

        private void BreakPipeLink(PhysicsObject d)
        {
            this._removeFromPipe.Add(d);
            if (d is Duck)
                (d as Duck).immobilized = false;
            d.skipClip = false;
        }

        private void Clip(PhysicsObject d)
        {
            if (d == null)
                return;
            if (this._colliding != null)
            {
                foreach (MaterialThing materialThing in this._colliding)
                    d.clip.Add(materialThing);
            }
            d.clip.Add(this);
        }

        private void UpdatePipeEnd()
        {
            if (this._colliding != null)
                return;
            this._colliding = new List<MaterialThing>();
            foreach (IPlatform platform in this._pipeUp != null || this._pipeDown != null ? Level.CheckRectAll<IPlatform>(this.topLeft + new Vec2(0.0f, -16f), this.bottomRight + new Vec2(0.0f, 16f)).ToList<IPlatform>() : (IEnumerable<IPlatform>)Level.CheckLineAll<IPlatform>(this.topLeft + new Vec2(-16f, 0.0f), this.bottomRight + new Vec2(16f, 0.0f)).ToList<IPlatform>())
            {
                if (platform is MaterialThing && !(platform is PhysicsObject))
                    this._colliding.Add(platform as MaterialThing);
            }
        }

        private void UpdatePipeEndLate()
        {
            foreach (PhysicsObject physicsObject in this._pipingOut)
            {
                Thing.Fondle(physicsObject, DuckNetwork.localConnection);
                this.Clip(physicsObject);
                physicsObject.skipClip = true;
                physicsObject.grounded = false;
                physicsObject._sleeping = false;
                if (!Collision.Rect(this.rectangle, physicsObject.rectangle))
                    this.BreakPipeLink(physicsObject);
            }
            foreach (ITeleport teleport in this._removeFromPipe)
            {
                this._objectsInPipes.Remove(teleport);
                if (teleport is PhysicsObject)
                    this._pipingOut.Remove(teleport as PhysicsObject);
            }
            this._removeFromPipe.Clear();
        }

        private void StartTransporting(Thing pThing)
        {
            if (pThing is RagdollPart)
            {
                Ragdoll doll = (pThing as RagdollPart).doll;
                if (doll == null)
                    return;
                this._transporting.Add(new PipeTileset.PipeBundle()
                {
                    thing = doll.part1,
                    cameraPosition = doll.part1.position
                });
                this._transporting.Add(new PipeTileset.PipeBundle()
                {
                    thing = doll.part2,
                    cameraPosition = doll.part1.position
                });
                this._transporting.Add(new PipeTileset.PipeBundle()
                {
                    thing = doll.part3,
                    cameraPosition = doll.part1.position
                });
                this._removeFromPipe.Add(doll.part1);
                this._removeFromPipe.Add(doll.part2);
                this._removeFromPipe.Add(doll.part3);
            }
            else
            {
                this._transporting.Add(new PipeTileset.PipeBundle()
                {
                    thing = pThing,
                    cameraPosition = pThing.position
                });
                this._removeFromPipe.Add(pThing as ITeleport);
            }
        }

        private void UpdateEntryPipe()
        {
            IEnumerable<PhysicsObject> physicsObjects = null;
            if (this.Down() != null)
                physicsObjects = Level.CheckRectAll<PhysicsObject>(this.topLeft + new Vec2(1f, -32f), this.bottomRight + new Vec2(-1f, 4f));
            else if (this.Up() != null)
                physicsObjects = Level.CheckRectAll<PhysicsObject>(this.topLeft + new Vec2(1f, -4f), this.bottomRight + new Vec2(-1f, 32f));
            else if (this.Left() != null)
                physicsObjects = Level.CheckRectAll<PhysicsObject>(this.topLeft + new Vec2(-4f, 3f), this.bottomRight + new Vec2(32f, -3f));
            else if (this.Right() != null)
                physicsObjects = Level.CheckRectAll<PhysicsObject>(this.topLeft + new Vec2(-32f, 3f), this.bottomRight + new Vec2(4f, -3f));
            foreach (PhysicsObject physicsObject in physicsObjects)
            {
                if (physicsObject.owner == null && !physicsObject.inPipe)
                {
                    bool flag1 = false;
                    if (this.Down() != null)
                        flag1 = (double)physicsObject.bottom + (double)physicsObject.vSpeed > (double)this.top - 6.0 && (double)physicsObject.width <= 16.0 && (!(physicsObject is Duck) || !(physicsObject as Duck).sliding);
                    else if (this.Up() != null)
                        flag1 = (double)physicsObject.top + (double)physicsObject.vSpeed < (double)this.bottom + 6.0 && (double)physicsObject.width <= 16.0 && (!(physicsObject is Duck) || !(physicsObject as Duck).sliding);
                    else if (this.Left() != null)
                        flag1 = (double)physicsObject.left + (double)physicsObject.hSpeed < (double)this.right + 2.0 && (double)physicsObject.height <= 16.0;
                    else if (this.Right() != null)
                        flag1 = (double)physicsObject.right + (double)physicsObject.hSpeed > (double)this.left - 2.0 && (double)physicsObject.height <= 16.0;
                    if (flag1 && physicsObject != null && physicsObject.isServerForObject)
                    {
                        bool flag2 = false;
                        if (this.Down() != null)
                            flag2 = (double)physicsObject.vSpeed > -0.100000001490116 && (double)physicsObject.bottom < (double)this.top + 4.0 && (double)Math.Abs(physicsObject.hSpeed) < 10.0;
                        else if (this.Up() != null)
                            flag2 = (double)physicsObject.vSpeed < -2.0 && (double)physicsObject.top > (double)this.bottom - 4.0 && (double)Math.Abs(physicsObject.hSpeed) < 10.0;
                        else if (this.Left() != null)
                            flag2 = (double)physicsObject.hSpeed < -0.200000002980232 && (double)physicsObject.left > (double)this.right - 4.0 && (double)Math.Abs(physicsObject.vSpeed) < 4.0;
                        else if (this.Right() != null)
                            flag2 = (double)physicsObject.hSpeed > 0.200000002980232 && (double)physicsObject.right < (double)this.left + 4.0 && (double)Math.Abs(physicsObject.vSpeed) < 4.0;
                        if (flag2 && !this._pipingOut.Contains(physicsObject))
                        {
                            if (physicsObject is RagdollPart)
                            {
                                Ragdoll doll = (physicsObject as RagdollPart).doll;
                                if (doll != null && doll.part1 != null && doll.part2 != null && doll.part3 != null && doll.part1.owner == null && doll.part2.owner == null && doll.part3.owner == null && !this._pipingOut.Contains(doll.part1) && !this._pipingOut.Contains(doll.part2) && !this._pipingOut.Contains(doll.part3))
                                {
                                    this._objectsInPipes.Add(doll.part1);
                                    this._objectsInPipes.Add(doll.part2);
                                    this._objectsInPipes.Add(doll.part3);
                                    doll.part1.inPipe = true;
                                    doll.part2.inPipe = true;
                                    doll.part3.inPipe = true;
                                }
                            }
                            else
                            {
                                physicsObject.inPipe = true;
                                physicsObject.OnTeleport();
                                this._objectsInPipes.Add(physicsObject);
                            }
                        }
                    }
                }
            }
            foreach (ITeleport teleport in Level.CheckCircleAll<ITeleport>(this.endOffset, 16f))
            {
                if (teleport is QuadLaserBullet)
                {
                    QuadLaserBullet quadLaserBullet = teleport as QuadLaserBullet;
                    if (!quadLaserBullet.inPipe && this.oppositeEnd != null && !(bool)this.oppositeEnd.trapdoor && this.MovingIntoPipe(quadLaserBullet.position, quadLaserBullet.travel, 4f))
                    {
                        this._objectsInPipes.Add(teleport);
                        quadLaserBullet.inPipe = true;
                    }
                }
                else if (teleport is PhysicsParticle)
                {
                    PhysicsParticle physicsParticle = teleport as PhysicsParticle;
                    if (!physicsParticle.inPipe && this.oppositeEnd != null && !(bool)this.oppositeEnd.trapdoor && this.MovingIntoPipe(physicsParticle.position, physicsParticle.velocity, 4f))
                    {
                        physicsParticle.inPipe = true;
                        this._objectsInPipes.Add(teleport);
                    }
                }
            }
            Vec2 vec2_1;
            foreach (ITeleport objectsInPipe in this._objectsInPipes)
            {
                if (!this._removeFromPipe.Contains(objectsInPipe))
                {
                    switch (objectsInPipe)
                    {
                        case QuadLaserBullet _:
                            QuadLaserBullet quadLaserBullet1 = objectsInPipe as QuadLaserBullet;
                            quadLaserBullet1.position = Lerp.Vec2Smooth(quadLaserBullet1.position, this.position, 0.2f);
                            vec2_1 = this.position - quadLaserBullet1.position;
                            if ((double)vec2_1.length < 6.0)
                            {
                                quadLaserBullet1.position = this.oppositeEnd.position + this.oppositeEnd.endNormal * 4f;
                                QuadLaserBullet quadLaserBullet2 = quadLaserBullet1;
                                Vec2 endNormal = this.oppositeEnd.endNormal;
                                vec2_1 = quadLaserBullet1.travel;
                                double length = (double)vec2_1.length;
                                Vec2 vec2_2 = endNormal * (float)length;
                                quadLaserBullet2.travel = vec2_2;
                                this._removeFromPipe.Add(objectsInPipe);
                                quadLaserBullet1.inPipe = false;
                                continue;
                            }
                            continue;
                        case PhysicsParticle _:
                            PhysicsParticle physicsParticle = objectsInPipe as PhysicsParticle;
                            physicsParticle._grounded = true;
                            physicsParticle.position = Lerp.Vec2Smooth(physicsParticle.position, this.position, 0.2f);
                            physicsParticle.hSpeed *= 0.9f;
                            physicsParticle.vSpeed *= 0.9f;
                            vec2_1 = this.position - physicsParticle.position;
                            if ((double)vec2_1.length < 6.0)
                            {
                                physicsParticle.position = this.oppositeEnd.endOffset + new Vec2(Rando.Float(-5f, 5f) * Math.Abs(this.oppositeEnd.endNormal.y), Rando.Float(-5f, 5f) * Math.Abs(this.oppositeEnd.endNormal.x));
                                physicsParticle.velocity = this.oppositeEnd.endNormal * Rando.Float(1f, 2f);
                                physicsParticle.hSpeed += Rando.Float(-1f, 1f) * Math.Abs(this.oppositeEnd.endNormal.y);
                                physicsParticle.vSpeed += Rando.Float(-1f, 1f) * Math.Abs(this.oppositeEnd.endNormal.x);
                                physicsParticle._grounded = false;
                                this._removeFromPipe.Add(objectsInPipe);
                                physicsParticle.inPipe = false;
                                continue;
                            }
                            continue;
                        case PhysicsObject _:
                            PhysicsObject physicsObject = objectsInPipe as PhysicsObject;
                            bool flag3 = this.Up() != null || this.Down() != null;
                            if (physicsObject is Duck)
                            {
                                (physicsObject as Duck).immobilized = true;
                                if (!flag3)
                                {
                                    (physicsObject as Duck).crouch = true;
                                    (physicsObject as Duck).sliding = true;
                                }
                            }
                            Thing.Fondle(physicsObject, DuckNetwork.localConnection);
                            this.Clip(physicsObject);
                            physicsObject.skipClip = true;
                            physicsObject.grounded = false;
                            physicsObject._sleeping = false;
                            if (flag3)
                            {
                                physicsObject.position.x = Lerp.FloatSmooth(physicsObject.position.x, this.x, 0.4f);
                                physicsObject.hSpeed *= 0.8f;
                                if (this.Down() != null)
                                    physicsObject.vSpeed += 0.4f;
                                else
                                    physicsObject.vSpeed -= 0.4f;
                            }
                            else
                            {
                                if (physicsObject is Duck)
                                    physicsObject.position.y = Lerp.FloatSmooth(physicsObject.position.y, this.y - 10f, 0.6f);
                                else
                                    physicsObject.position.y = Lerp.FloatSmooth(physicsObject.position.y, this.y - (physicsObject.collisionCenter.y - this.y), 0.5f);
                                physicsObject.vSpeed *= 0.8f;
                                if (this.Left() != null)
                                    physicsObject.hSpeed -= 0.4f;
                                else if (this.Right() != null)
                                    physicsObject.hSpeed += 0.4f;
                            }
                            if (flag3)
                            {
                                foreach (IPlatform platform in Level.CheckRectAll<IPlatform>(this.topLeft + new Vec2(2f, -24f), this.bottomRight + new Vec2(-2f, 24f)))
                                {
                                    if (platform is MaterialThing)
                                        physicsObject.clip.Add(platform as MaterialThing);
                                }
                            }
                            else
                            {
                                foreach (IPlatform platform in Level.CheckRectAll<IPlatform>(this.topLeft + new Vec2(-24f, 2f), this.bottomRight + new Vec2(24f, -2f)))
                                {
                                    if (platform is MaterialThing)
                                        physicsObject.clip.Add(platform as MaterialThing);
                                }
                            }
                            vec2_1 = physicsObject.position - this.position;
                            if ((double)vec2_1.length > 32.0 || physicsObject.owner != null)
                            {
                                physicsObject.inPipe = false;
                                this.BreakPipeLink(physicsObject);
                                continue;
                            }
                            if (flag3 && (double)Math.Abs(physicsObject.position.x - this.x) < 4.0 || !flag3 && (physicsObject is Duck && (double)Math.Abs(physicsObject.position.y - (this.y - 10f)) < 4.0 || !(physicsObject is Duck) && (double)Math.Abs(physicsObject.position.y - this.y) < 4.0))
                            {
                                bool flag4 = false;
                                if (this.Down() != null)
                                    flag4 = physicsObject.position.y > (double)this.top + 6.0;
                                else if (this.Up() != null)
                                    flag4 = physicsObject.position.y < (double)this.bottom - 6.0;
                                else if (this.Left() != null)
                                    flag4 = physicsObject.position.x < (double)this.right - 6.0;
                                else if (this.Right() != null)
                                    flag4 = physicsObject.position.x > (double)this.left + 6.0;
                                if (flag4)
                                {
                                    this.StartTransporting(physicsObject);
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
            for (this._transportingIndex = 0; this._transportingIndex < this._transporting.Count; ++this._transportingIndex)
            {
                PipeTileset.PipeBundle bundle = this._transporting[this._transportingIndex];
                Thing thing = bundle.thing;
                if (thing is PhysicsObject)
                {
                    PhysicsObject o = thing as PhysicsObject;
                    o.updatePhysics = false;
                    o.overfollow = 0.5f;
                    o.position = new Vec2(-5000f, -1000f);
                    o.cameraPositionOverride = bundle.cameraPosition;
                    bundle.cameraPosition = Lerp.Vec2(bundle.cameraPosition, this._oppositeEnd.position, this.travelLength / 10f);
                    if ((double)(bundle.cameraPosition - this._oppositeEnd.position).length < 4.0)
                    {
                        this.FinishTransporting(o, bundle);
                        SFX.Play("pipeOut", pitch: Rando.Float(-0.1f, 0.1f));
                    }
                }
            }
            base.Update();
        }

        private void FinishTransporting(
          PhysicsObject o,
          PipeTileset.PipeBundle bundle,
          bool pDoingRagthing = false)
        {
            o.updatePhysics = true;
            o.overfollow = 0.0f;
            if (bundle != null)
                o.position = bundle.cameraPosition;
            o.cameraPositionOverride = Vec2.Zero;
            o.inPipe = false;
            if (bundle == null)
                return;
            int index = this._transporting.IndexOf(bundle);
            if (index < 0)
                return;
            if (!pDoingRagthing && o is RagdollPart)
            {
                Ragdoll r = (o as RagdollPart).doll;
                if (r != null)
                {
                    this.FinishTransporting(r.part1, this._transporting.FirstOrDefault<PipeTileset.PipeBundle>(x => x.thing == r.part1), true);
                    this.FinishTransporting(r.part2, this._transporting.FirstOrDefault<PipeTileset.PipeBundle>(x => x.thing == r.part2), true);
                    this.FinishTransporting(r.part3, this._transporting.FirstOrDefault<PipeTileset.PipeBundle>(x => x.thing == r.part3), true);
                    r.part1.position = new Vec2(r.part2.x + Rando.Float(-4f, 4f), r.part2.y + Rando.Float(-4f, 4f));
                    r.part3.position = new Vec2(r.part2.x + Rando.Float(-4f, 4f), r.part2.y + Rando.Float(-4f, 4f));
                    return;
                }
            }
            else
            {
                if (o is RagdollPart)
                    (o as RagdollPart)._lastReasonablePosition = o.position;
                this._oppositeEnd.PipeOut(o);
                this._transporting.RemoveAt(index);
            }
            if (index > this._transportingIndex)
                return;
            --this._transportingIndex;
        }

        public override void EditorAdded()
        {
            Dictionary<PipeTileset.Direction, PipeTileset> neighbors = this.GetNeighbors();
            if (!this.AttemptObviousConnection(neighbors) && PipeTileset._lastAdd != null)
            {
                if (this.Up(neighbors) == PipeTileset._lastAdd && this.Up(neighbors).ReadyForConnection())
                    this.MakeConnection(this.Up(neighbors));
                else if (this.Down(neighbors) == PipeTileset._lastAdd && this.Down(neighbors).ReadyForConnection())
                    this.MakeConnection(this.Down(neighbors));
                else if (this.Left(neighbors) == PipeTileset._lastAdd && this.Left(neighbors).ReadyForConnection())
                    this.MakeConnection(this.Left(neighbors));
                else if (this.Right(neighbors) == PipeTileset._lastAdd && this.Right(neighbors).ReadyForConnection())
                    this.MakeConnection(this.Right(neighbors));
            }
            this.searchUp = this.searchDown = this.searchLeft = this.searchRight = false;
            if (this.Up() != null)
            {
                this.searchUp = true;
                this.Up().searchDown = true;
            }
            if (this.Down() != null)
            {
                this.searchDown = true;
                this.Down().searchUp = true;
            }
            if (this.Left() != null)
            {
                this.searchLeft = true;
                this.Left().searchRight = true;
            }
            if (this.Right() != null)
            {
                this.searchRight = true;
                this.Right().searchLeft = true;
            }
            this.TestValidity();
            PipeTileset._lastAdd = this;
        }

        public override void EditorFlip(bool pVertical)
        {
            if (pVertical)
            {
                bool searchUp = this.searchUp;
                this.searchUp = this.searchDown;
                this.searchDown = searchUp;
            }
            else
            {
                bool searchLeft = this.searchLeft;
                this.searchLeft = this.searchRight;
                this.searchRight = searchLeft;
            }
        }

        public override void EditorRemoved()
        {
            if (this.Up() != null)
            {
                PipeTileset pipeTileset = this.Up();
                this.BreakConnection(this.Up());
                pipeTileset.TestValidity();
            }
            if (this.Down() != null)
            {
                PipeTileset pipeTileset = this.Down();
                this.BreakConnection(this.Down());
                pipeTileset.TestValidity();
            }
            if (this.Left() != null)
            {
                PipeTileset pipeTileset = this.Left();
                this.BreakConnection(this.Left());
                pipeTileset.TestValidity();
            }
            if (this.Right() != null)
            {
                PipeTileset pipeTileset = this.Right();
                this.BreakConnection(this.Right());
                pipeTileset.TestValidity();
            }
            this.TestValidity();
            PipeTileset._lastAdd = null;
        }

        public override void EditorObjectsChanged()
        {
            this.ReConnect();
            this.TestValidity();
            this.UpdateConnectionFrame();
            base.EditorObjectsChanged();
        }

        public override void EditorRender()
        {
            if (!(Level.current is Editor) || !((Level.current as Editor).placementType is PipeTileset))
                return;
            this.alpha = 0.6f;
            this.Draw();
            this.alpha = 1f;
            base.EditorRender();
        }

        public override void OnEditorLoaded()
        {
            this.ReConnect();
            this.TestValidity();
        }

        public override void Update()
        {
            if (!this._initializedConnections)
            {
                this.ReConnect();
                this._initializedConnections = true;
            }
            else if (!this._testedValidity)
                this.TestValidity();
            if (this._failBullets > 0)
                --this._failBullets;
            if (this.connections.Count == 1)
                this.UpdatePipeEnd();
            ++this.framesSincePipeout;
            if (this.isEntryPipe)
                this.UpdateEntryPipe();
            if (this.connections.Count == 1)
                this.UpdatePipeEndLate();
            ++this._initializedBackground;
            if (this._initializedBackground == 2 && !(Level.current is Editor))
            {
                if (this.IsBackground())
                    this._collisionOffset = new Vec2(Vec2.MinValue);
                if (this._validPipe && this.connections.Count == 1 && (this.Left() != null || this.Right() != null) && Level.CheckPoint<Block>(this.position, this) != null)
                    this.solid = false;
            }
            base.Update();
        }

        public override void Draw()
        {
            Color color = this.graphic.color;
            if (this.IsBackground())
            {
                this.depth = (Depth)(this.pipeDepth - 1.8f);
                this.graphic.color = color * 0.5f;
                this.graphic.color = new Color(this.graphic.color.r, this.graphic.color.g, this.graphic.color.b, byte.MaxValue);
            }
            else
            {
                this.depth = (Depth)this.pipeDepth;
                if (this.Left() != null && this.Left().IsBackground())
                {
                    int frame = this._sprite.frame;
                    this._sprite.frame = 22;
                    this._sprite.flipH = true;
                    Graphics.Draw(_sprite, this.x - 16f, this.y, this.depth + 5);
                    this._sprite.flipH = false;
                    this._sprite.frame = frame;
                }
                if (this.Right() != null && this.Right().IsBackground())
                {
                    int frame = this._sprite.frame;
                    this._sprite.frame = 22;
                    Graphics.Draw(_sprite, this.x + 16f, this.y, this.depth + 5);
                    this._sprite.frame = frame;
                }
                if (this.Up() != null && this.Up().IsBackground())
                {
                    int frame = this._sprite.frame;
                    this._sprite.frame = 22;
                    this._sprite.angleDegrees = -90f;
                    Graphics.Draw(_sprite, this.x, this.y - 16f, this.depth + 5);
                    this._sprite.angleDegrees = 0.0f;
                    this._sprite.frame = frame;
                }
                if (this.Down() != null && this.Down().IsBackground())
                {
                    int frame = this._sprite.frame;
                    this._sprite.frame = 22;
                    this._sprite.angleDegrees = 90f;
                    this._sprite.flipV = true;
                    Graphics.Draw(_sprite, this.x, this.y + 16f, this.depth + 5);
                    this._sprite.angleDegrees = 0.0f;
                    this._sprite.flipV = false;
                    this._sprite.frame = frame;
                }
            }
            if (this.isEntryPipe && !(Level.current is Editor))
                this.DrawParticles();
            base.Draw();
            this.graphic.color = color;
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with is PhysicalBullet)
            {
                this.Hit((with as PhysicalBullet).bullet, (with as PhysicalBullet).bullet.currentTravel);
                if (this.entered)
                    return;
            }
            base.OnSolidImpact(with, from);
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            this.entered = false;
            if (this.connections.Count == 1 && (!(bool)this.trapdoor || bullet.ammo.penetration >= (double)this.thickness) && ((hitPos.x - 8.0 < (double)this.left && this.Right() != null && bullet.travelDirNormalized.x > 0.300000011920929 || hitPos.x + 8.0 > (double)this.right && this.Left() != null && bullet.travelDirNormalized.x < -0.300000011920929) && hitPos.y > (double)this.top && hitPos.y < (double)this.bottom || (hitPos.y - 8.0 < (double)this.top && this.Down() != null && bullet.travelDirNormalized.y > 0.300000011920929 || hitPos.y + 8.0 > (double)this.bottom && this.Up() != null && bullet.travelDirNormalized.y < -0.300000011920929) && hitPos.x > (double)this.left && hitPos.x < (double)this.right) && this.oppositeEnd != null)
            {
                float rng = bullet._totalLength - (bullet._actualStart - hitPos).length;
                float num = 0.0f;
                if ((double)rng > 0.0)
                {
                    bool flag1 = false;
                    bool flag2 = bullet.ammo is ATMissile;
                    if (bullet.ammo != null && !bullet.ammo.flawlessPipeTravel && !flag2 && this.hasKinks)
                    {
                        flag1 = true;
                        ++this._failBullets;
                        if (this._failBullets > 5)
                            flag1 = false;
                    }
                    if (flag2)
                        rng = bullet.ammo.range;
                    if ((bool)this.oppositeEnd.trapdoor && bullet.ammo.penetration < (double)this.oppositeEnd.thickness)
                        flag1 = true;
                    if (!flag1)
                    {
                        if (this.oppositeEnd.Left() != null)
                            bullet.DoRebound(this.oppositeEnd.endOffset, 0.0f + num, rng);
                        if (this.oppositeEnd.Right() != null)
                            bullet.DoRebound(this.oppositeEnd.endOffset, 180f + num, rng);
                        if (this.oppositeEnd.Up() != null)
                            bullet.DoRebound(this.oppositeEnd.endOffset, 270f + num, rng);
                        if (this.oppositeEnd.Down() != null)
                            bullet.DoRebound(this.oppositeEnd.endOffset, 90f + num, rng);
                    }
                    this.entered = true;
                    return true;
                }
            }
            return base.Hit(bullet, hitPos);
        }

        private void ReConnect()
        {
            if (!this.searchUp && !this.searchDown && !this.searchLeft && !this.searchRight)
                return;
            this.connections.Clear();
            this._pipeLeft = null;
            this._pipeRight = null;
            this._pipeUp = null;
            this._pipeDown = null;
            Dictionary<PipeTileset.Direction, PipeTileset> neighbors = this.GetNeighbors();
            if (this.searchUp && this.Up(neighbors) != null)
                this.MakeConnection(this.Up(neighbors));
            if (this.searchDown && this.Down(neighbors) != null)
                this.MakeConnection(this.Down(neighbors));
            if ((this.searchLeft || this.flipHorizontal && this.searchRight) && this.Left(neighbors) != null)
                this.MakeConnection(this.Left(neighbors));
            if (!this.searchRight && (!this.flipHorizontal || !this.searchLeft) || this.Right(neighbors) == null)
                return;
            this.MakeConnection(this.Right(neighbors));
        }

        public PipeTileset Up(
          Dictionary<PipeTileset.Direction, PipeTileset> pNeighbors = null)
        {
            if (pNeighbors == null)
                return this._pipeUp;
            PipeTileset pipeTileset;
            pNeighbors.TryGetValue(PipeTileset.Direction.Up, out pipeTileset);
            return pipeTileset;
        }

        public PipeTileset Down(
          Dictionary<PipeTileset.Direction, PipeTileset> pNeighbors = null)
        {
            if (pNeighbors == null)
                return this._pipeDown;
            PipeTileset pipeTileset;
            pNeighbors.TryGetValue(PipeTileset.Direction.Down, out pipeTileset);
            return pipeTileset;
        }

        public PipeTileset Left(
          Dictionary<PipeTileset.Direction, PipeTileset> pNeighbors = null)
        {
            if (pNeighbors == null)
                return this._pipeLeft;
            PipeTileset pipeTileset;
            pNeighbors.TryGetValue(PipeTileset.Direction.Left, out pipeTileset);
            return pipeTileset;
        }

        public PipeTileset Right(
          Dictionary<PipeTileset.Direction, PipeTileset> pNeighbors = null)
        {
            if (pNeighbors == null)
                return this._pipeRight;
            PipeTileset pipeTileset;
            pNeighbors.TryGetValue(PipeTileset.Direction.Right, out pipeTileset);
            return pipeTileset;
        }

        protected virtual Dictionary<PipeTileset.Direction, PipeTileset> GetNeighbors()
        {
            Dictionary<PipeTileset.Direction, PipeTileset> neighbors = new Dictionary<PipeTileset.Direction, PipeTileset>();
            PipeTileset pipeTileset1 = Level.CheckPointAll<PipeTileset>(this.x, this.y - 16f).Where<PipeTileset>(x => x.group == this.group).FirstOrDefault<PipeTileset>();
            if (pipeTileset1 != null)
                neighbors[PipeTileset.Direction.Up] = pipeTileset1;
            PipeTileset pipeTileset2 = Level.CheckPointAll<PipeTileset>(this.x, this.y + 16f).Where<PipeTileset>(x => x.group == this.group).FirstOrDefault<PipeTileset>();
            if (pipeTileset2 != null)
                neighbors[PipeTileset.Direction.Down] = pipeTileset2;
            PipeTileset pipeTileset3 = Level.CheckPointAll<PipeTileset>(this.x - 16f, this.y).Where<PipeTileset>(x => x.group == this.group).FirstOrDefault<PipeTileset>();
            if (pipeTileset3 != null)
                neighbors[PipeTileset.Direction.Left] = pipeTileset3;
            PipeTileset pipeTileset4 = Level.CheckPointAll<PipeTileset>(this.x + 16f, this.y).Where<PipeTileset>(x => x.group == this.group).FirstOrDefault<PipeTileset>();
            if (pipeTileset4 != null)
                neighbors[PipeTileset.Direction.Right] = pipeTileset4;
            return neighbors;
        }

        public bool ReadyForConnection() => this.connections.Count <= 1;

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
                        this.hasKinks = true;
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
            if ((double)pWith.y == (double)this.y)
            {
                if ((double)pWith.x > (double)this.x)
                {
                    pWith.connections[PipeTileset.Direction.Left] = this;
                    pWith._pipeLeft = this;
                    this.connections[PipeTileset.Direction.Right] = pWith;
                    this._pipeRight = pWith;
                }
                if ((double)pWith.x < (double)this.x)
                {
                    pWith.connections[PipeTileset.Direction.Right] = this;
                    pWith._pipeRight = this;
                    this.connections[PipeTileset.Direction.Left] = pWith;
                    this._pipeLeft = pWith;
                }
            }
            else if ((double)pWith.x == (double)this.x)
            {
                if ((double)pWith.y > (double)this.y)
                {
                    pWith.connections[PipeTileset.Direction.Up] = this;
                    pWith._pipeUp = this;
                    this.connections[PipeTileset.Direction.Down] = pWith;
                    this._pipeDown = pWith;
                }
                if ((double)pWith.y < (double)this.y)
                {
                    pWith.connections[PipeTileset.Direction.Down] = this;
                    pWith._pipeDown = this;
                    this.connections[PipeTileset.Direction.Up] = pWith;
                    this._pipeUp = pWith;
                }
            }
            this.UpdateConnectionFrame();
            pWith.UpdateConnectionFrame();
        }

        private void BreakConnection(PipeTileset pWith)
        {
            if ((double)pWith.y == (double)this.y)
            {
                if ((double)pWith.x > (double)this.x)
                {
                    pWith.connections.Remove(PipeTileset.Direction.Left);
                    pWith.searchLeft = false;
                    this.connections.Remove(PipeTileset.Direction.Right);
                    this._pipeRight = null;
                }
                if ((double)pWith.x < (double)this.x)
                {
                    pWith.connections.Remove(PipeTileset.Direction.Right);
                    pWith._pipeRight = null;
                    pWith.searchRight = false;
                    this.connections.Remove(PipeTileset.Direction.Left);
                    this._pipeLeft = null;
                }
            }
            else if ((double)pWith.x == (double)this.x)
            {
                if ((double)pWith.y > (double)this.y)
                {
                    pWith.connections.Remove(PipeTileset.Direction.Up);
                    pWith._pipeUp = null;
                    pWith.searchUp = false;
                    this.connections.Remove(PipeTileset.Direction.Down);
                    this._pipeDown = null;
                }
                if ((double)pWith.y < (double)this.y)
                {
                    pWith.connections.Remove(PipeTileset.Direction.Down);
                    pWith._pipeDown = null;
                    pWith.searchDown = false;
                    this.connections.Remove(PipeTileset.Direction.Up);
                    this._pipeUp = null;
                }
            }
            this.UpdateConnectionFrame();
            pWith.UpdateConnectionFrame();
        }

        private void UpdateConnectionFrame()
        {
            this.OnUpdateConnectionFrame();
            if (this.connections.Count == 1)
            {
                this._drawBlockOverlay = Level.CheckPoint<Block>(this.position, this) != null;
                this._foregroundDraw = true;
            }
            else
                this._foregroundDraw = false;
            if (this.frame == 0)
            {
                this.collisionSize = new Vec2(14f, 14f);
                this.collisionOffset = new Vec2(-7f, -7f);
            }
            else if (this.frame == 1)
            {
                this.collisionSize = new Vec2(15f, 15f);
                this.collisionOffset = new Vec2(-7f, -7f);
            }
            else if (this.frame == 2)
            {
                this.collisionSize = new Vec2(15f, 15f);
                this.collisionOffset = new Vec2(-8f, -7f);
            }
            else if (this.frame == 3)
            {
                this.collisionSize = new Vec2(16f, 14f);
                this.collisionOffset = new Vec2(-8f, -7f);
            }
            else if (this.frame == 5)
            {
                this.collisionSize = new Vec2(15f, 15f);
                this.collisionOffset = new Vec2(-7f, -8f);
            }
            else if (this.frame == 6)
            {
                this.collisionSize = new Vec2(15f, 15f);
                this.collisionOffset = new Vec2(-8f, -8f);
            }
            else if (this.frame == 7)
            {
                this.collisionSize = new Vec2(14f, 16f);
                this.collisionOffset = new Vec2(-7f, -8f);
            }
            else if (this.frame == 8)
            {
                this.collisionSize = new Vec2(14f, 15f);
                this.collisionOffset = new Vec2(-7f, -7f);
            }
            else if (this.frame == 9)
            {
                this.collisionSize = new Vec2(14f, 15f);
                this.collisionOffset = new Vec2(-7f, -8f);
            }
            else if (this.frame == 10)
            {
                this.collisionSize = new Vec2(15f, 14f);
                this.collisionOffset = new Vec2(-7f, -7f);
            }
            else if (this.frame == 11)
            {
                this.collisionSize = new Vec2(15f, 14f);
                this.collisionOffset = new Vec2(-8f, -7f);
            }
            if (this.IsBackground())
            {
                this.solid = false;
                this.thickness = 0.0f;
                this.physicsMaterial = PhysicsMaterial.Default;
            }
            else
            {
                this.solid = true;
                this.thickness = 3f;
                this.physicsMaterial = PhysicsMaterial.Metal;
            }
        }

        private void OnUpdateConnectionFrame()
        {
            if (this.connections.Count == 0)
                this._sprite.frame = 0;
            else if (this.connections.Count == 1)
            {
                if (this.Up() != null)
                    this._sprite.frame = 9;
                else if (this.Down() != null)
                    this._sprite.frame = 8;
                else if (this.Left() != null)
                {
                    this._sprite.frame = 11;
                }
                else
                {
                    if (this.Right() == null)
                        return;
                    this._sprite.frame = 10;
                }
            }
            else if (this.Up() != null && this.Right() != null)
                this._sprite.frame = 5;
            else if (this.Up() != null && this.Left() != null)
                this._sprite.frame = 6;
            else if (this.Up() != null && this.Down() != null)
                this._sprite.frame = 7;
            else if (this.Down() != null && this.Right() != null)
                this._sprite.frame = 1;
            else if (this.Down() != null && this.Left() != null)
            {
                this._sprite.frame = 2;
            }
            else
            {
                if (this.Left() == null || this.Right() == null)
                    return;
                this._sprite.frame = 3;
            }
        }

        private bool AttemptObviousConnection(
          Dictionary<PipeTileset.Direction, PipeTileset> pNeighbors)
        {
            bool flag = false;
            if (this.Up(pNeighbors) == null && this.Down(pNeighbors) == null)
            {
                if (this.Left(pNeighbors) != null && this.Left(pNeighbors).ReadyForConnection())
                {
                    this.MakeConnection(this.Left(pNeighbors));
                    flag = true;
                }
                if (this.Right(pNeighbors) != null && this.Right(pNeighbors).ReadyForConnection())
                {
                    this.MakeConnection(this.Right(pNeighbors));
                    flag = true;
                }
            }
            if (this.Left(pNeighbors) == null && this.Right(pNeighbors) == null)
            {
                if (this.Up(pNeighbors) != null && this.Up(pNeighbors).ReadyForConnection())
                {
                    this.MakeConnection(this.Up(pNeighbors));
                    flag = true;
                }
                if (this.Down(pNeighbors) != null && this.Down(pNeighbors).ReadyForConnection())
                {
                    this.MakeConnection(this.Down(pNeighbors));
                    flag = true;
                }
            }
            if (!flag)
            {
                List<PipeTileset> pipeTilesetList = new List<PipeTileset>();
                foreach (KeyValuePair<PipeTileset.Direction, PipeTileset> pNeighbor in pNeighbors)
                {
                    if (pNeighbor.Value.ReadyForConnection())
                        pipeTilesetList.Add(pNeighbor.Value);
                }
                if (pipeTilesetList.Count <= 0 || pipeTilesetList.Count > 2)
                    return false;
                flag = true;
                if (PipeTileset._lastAdd != null && (pipeTilesetList[0] == PipeTileset._lastAdd || pipeTilesetList.Count > 1 && pipeTilesetList[1] == PipeTileset._lastAdd))
                {
                    this.MakeConnection(PipeTileset._lastAdd);
                }
                else
                {
                    this.MakeConnection(pipeTilesetList[0]);
                    if (pipeTilesetList.Count == 2)
                        this.MakeConnection(pipeTilesetList[1]);
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
            if (this.connections.Count == 1)
            {
                if (this._pipeUp != null)
                {
                    ++num2;
                    pTowards1 = this._pipeUp;
                    pipeTileset2 = this;
                }
                else if (this._pipeDown != null)
                {
                    ++num1;
                    pTowards1 = this._pipeDown;
                    pipeTileset2 = this;
                }
                else if (this._pipeLeft != null)
                {
                    pTowards1 = this._pipeLeft;
                    pipeTileset2 = this;
                }
                else if (this._pipeRight != null)
                {
                    pTowards1 = this._pipeRight;
                    pipeTileset2 = this;
                }
            }
            else
            {
                if (this._pipeUp != null)
                {
                    if (pTowards1 == null)
                        pTowards1 = this._pipeUp;
                    else
                        pTowards2 = this._pipeUp;
                }
                if (this._pipeDown != null)
                {
                    if (pTowards1 == null)
                        pTowards1 = this._pipeDown;
                    else
                        pTowards2 = this._pipeDown;
                }
                if (this._pipeLeft != null)
                {
                    if (pTowards1 == null)
                        pTowards1 = this._pipeLeft;
                    else
                        pTowards2 = this._pipeLeft;
                }
                if (this._pipeRight != null)
                {
                    if (pTowards1 == null)
                        pTowards1 = this._pipeRight;
                    else
                        pTowards2 = this._pipeRight;
                }
            }
            if (pTowards1 != null)
            {
                pipeTileset1 = this.Travel(pTowards1);
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
                pipeTileset2 = this.Travel(pTowards2);
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
            this._validPipe = num3 != 0;
            this._validPipe = this.connections.Count > 0;
            if (pipeTileset1 != null)
                pipeTileset1._validPipe = this._validPipe;
            if (pipeTileset2 != null)
                pipeTileset2._validPipe = this._validPipe;
            if (this._validPipe)
            {
                pipeTileset1._oppositeEnd = pipeTileset2;
                pipeTileset2._oppositeEnd = pipeTileset1;
                PipeTileset pipeTileset3 = pipeTileset1;
                PipeTileset pipeTileset4 = pipeTileset2;
                Vec2 vec2 = pipeTileset1.position - pipeTileset2.position;
                double length;
                float num4 = (float)(length = (double)vec2.length);
                pipeTileset4.travelLength = (float)length;
                double num5 = (double)num4;
                pipeTileset3.travelLength = (float)num5;
            }
            this._testedValidity = true;
        }

        private void DrawParticles()
        {
            if (this.partWait == -100)
                this.partRot = Rando.Float(10f);
            Vec2 vec2_1 = this.endNormal;
            Vec2 vec2_2 = vec2_1.Rotate(Maths.DegToRad(this.Up() != null || this.Right() != null ? 90f : -90f), Vec2.Zero);
            --this.partWait;
            if (this.partWait <= 0)
            {
                PipeTileset.PipeParticle pipeParticle = null;
                foreach (PipeTileset.PipeParticle particle in this._particles)
                {
                    if (particle.alpha >= 1.0)
                    {
                        pipeParticle = particle;
                        break;
                    }
                }
                if (pipeParticle == null)
                {
                    pipeParticle = new PipeTileset.PipeParticle();
                    this._particles.Add(pipeParticle);
                }
                pipeParticle.position = this.position + this.endNormal * 20f + Maths.AngleToVec(this.partRot) * (10f + Rando.Float(24f)) * vec2_2;
                pipeParticle.alpha = 0.0f;
                pipeParticle.velocity = Vec2.Zero;
                this.partWait = 5;
                this.partRot += 1.72152f;
            }
            for (int index = 0; index < this._particles.Count; ++index)
            {
                if (_particles[index].alpha < 1.0)
                {
                    Vec2 vec2_3 = this.position - this._particles[index].position;
                    this._particles[index].velocity -= this.endNormal * 0.03f;
                    this._particles[index].position -= vec2_3 * vec2_2 * 0.07f;
                    Graphics.DrawLine(this._particles[index].position, this._particles[index].position + this._particles[index].velocity * 3f, Color.White * this._particles[index].alpha, 0.75f, this.depth - 10);
                    this._particles[index].position += this._particles[index].velocity;
                    this._particles[index].alpha += 0.016f;
                    vec2_1 = this._particles[index].position * this.endNormal - this.position * this.endNormal;
                    if ((double)vec2_1.length < 2.0)
                        this._particles[index].alpha = 1f;
                }
            }
        }

        public void OnDrawLayer(Layer pLayer)
        {
            if (!this._foregroundDraw || pLayer != Layer.Blocks)
                return;
            int frame = this._sprite.frame;
            this._flapLerp = Lerp.FloatSmooth(this._flapLerp, this._flap, 0.25f);
            this._flap = Lerp.Float(this._flap, 0.0f, 0.15f);
            if (this._drawBlockOverlay)
            {
                this._sprite.frame += 8;
                Graphics.Draw(_sprite, this.position.x, this.position.y, (Depth)0.5f);
            }
            if (this.trapdoor.value)
            {
                Vec2 center = this._sprite.center;
                this._sprite.frame = 20;
                float num = this._drawBlockOverlay ? 10f : 9f;
                if (this.Left() != null)
                {
                    this._sprite.center = new Vec2(2f, 0.0f);
                    this._sprite.angleDegrees = (float)(0.0 - _flapLerp * 90.0);
                    Graphics.Draw(_sprite, this.position.x + num, this.position.y - 9f, (Depth)0.5f);
                    this._sprite.center = new Vec2(9f, 9f);
                    this._sprite.angleDegrees = 0.0f;
                    this._sprite.frame = 21;
                    Graphics.Draw(_sprite, this.position.x + (num - 1f), this.position.y - 9f, (Depth)0.4f);
                }
                else if (this.Right() != null)
                {
                    this._sprite.center = new Vec2(2f, 18f);
                    this._sprite.angleDegrees = (float)(180.0 + _flapLerp * 90.0);
                    this._sprite.flipV = true;
                    Graphics.Draw(_sprite, this.position.x - num, this.position.y - 9f, (Depth)0.5f);
                    this._sprite.center = new Vec2(9f, 9f);
                    this._sprite.angleDegrees = 0.0f;
                    this._sprite.frame = 21;
                    this._sprite.flipH = true;
                    Graphics.Draw(_sprite, this.position.x - (num - 1f), this.position.y - 9f, (Depth)0.4f);
                }
                else if (this.Up() != null)
                {
                    this._sprite.center = new Vec2(2f, 0.0f);
                    this._sprite.angleDegrees = (float)(90.0 - _flapLerp * 90.0);
                    Graphics.Draw(_sprite, this.position.x + 9f, this.position.y + num, (Depth)0.5f);
                    this._sprite.center = new Vec2(9f, 9f);
                    this._sprite.angleDegrees = 90f;
                    this._sprite.frame = 21;
                    Graphics.Draw(_sprite, this.position.x + 9f, this.position.y + (num - 1f), (Depth)0.4f);
                }
                else if (this.Down() != null)
                {
                    this._sprite.center = new Vec2(2f, 18f);
                    this._sprite.angleDegrees = (float)(270.0 + _flapLerp * 90.0);
                    this._sprite.flipV = true;
                    Graphics.Draw(_sprite, this.position.x + 9f, this.position.y - num, (Depth)0.5f);
                    this._sprite.center = new Vec2(9f, 9f);
                    this._sprite.angleDegrees = 90f;
                    this._sprite.frame = 21;
                    this._sprite.flipH = true;
                    Graphics.Draw(_sprite, this.position.x + 9f, this.position.y - (num - 1f), (Depth)0.4f);
                }
                this._sprite.flipV = false;
                this._sprite.flipH = false;
                this._sprite.center = center;
            }
            this._sprite.frame = frame;
        }

        public void FlapPipe() => this._flap = 1.9f;

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
