// Decompiled with JetBrains decompiler
// Type: DuckGame.WireTileset
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Stuff|Wires")]
    [BaggedProperty("isOnlineCapable", true)]
    public class WireTileset : AutoTile
    {
        private List<WireConnection> _connections = new List<WireConnection>();
        private List<WireTileset.WireSignal> _signals = new List<WireTileset.WireSignal>();
        private List<WireTileset.WireSignal> _addSignals = new List<WireTileset.WireSignal>();
        private List<WireTileset.WireSignal> _removeSignals = new List<WireTileset.WireSignal>();
        private WireConnection _centerWire;
        private Sprite _signalSprite;
        public bool dullSignalLeft;
        public bool dullSignalRight;
        public bool dullSignalUp;
        public bool dullSignalDown;

        public override int frame
        {
            get => base.frame;
            set
            {
                base.frame = value;
                if (base.frame == value)
                    return;
                UpdateConnections();
            }
        }

        public WireConnection centerWire => _centerWire;

        public WireTileset(float x, float y)
          : base(x, y, "wireTileset")
        {
            _editorName = "Wire";
            editorTooltip = "Connects a Button to other wire objects, allowing the Button to trigger the object's behavior.";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidth = 8f;
            verticalWidthThick = 8f;
            horizontalHeight = 8f;
            layer = Layer.Foreground;
            depth = -0.8f;
            weight = 1f;
            _signalSprite = new Sprite("wireBulge");
            _signalSprite.CenterOrigin();
        }

        public void Emit(WireTileset.WireSignal signal = null, float overshoot = 0f, int type = 0)
        {
            if (_centerWire == null)
                return;
            if (signal == null)
            {
                if (_centerWire.up != null)
                    _signals.Add(new WireTileset.WireSignal()
                    {
                        position = _centerWire.position,
                        prevPosition = _centerWire.position,
                        travel = _centerWire.up,
                        from = _centerWire,
                        currentWire = this,
                        signalType = type
                    });
                if (_centerWire.down != null)
                    _signals.Add(new WireTileset.WireSignal()
                    {
                        position = _centerWire.position,
                        prevPosition = _centerWire.position,
                        travel = _centerWire.down,
                        from = _centerWire,
                        currentWire = this,
                        signalType = type
                    });
                if (_centerWire.left != null)
                    _signals.Add(new WireTileset.WireSignal()
                    {
                        position = _centerWire.position,
                        prevPosition = _centerWire.position,
                        travel = _centerWire.left,
                        from = _centerWire,
                        currentWire = this,
                        signalType = type
                    });
                if (_centerWire.right == null)
                    return;
                _signals.Add(new WireTileset.WireSignal()
                {
                    position = _centerWire.position,
                    prevPosition = _centerWire.position,
                    travel = _centerWire.right,
                    from = _centerWire,
                    currentWire = this,
                    signalType = type
                });
            }
            else
            {
                WireConnection travel = signal.travel;
                _removeSignals.Add(signal);
                signal.finished = true;
                if (travel == _centerWire)
                    Level.CheckCircle<IWirePeripheral>(_centerWire.position, 3f)?.Pulse(signal.signalType, this);
                if (travel.up != null && travel.up != signal.from && !dullSignalUp)
                {
                    WireTileset.WireSignal signal1 = new WireTileset.WireSignal()
                    {
                        position = travel.position,
                        travel = travel.up,
                        from = travel,
                        currentWire = this,
                        life = signal.life,
                        prevPosition = signal.prevPosition,
                        signalType = signal.signalType
                    };
                    TravelSignal(signal1, overshoot, false);
                    if (!signal1.finished)
                        _addSignals.Add(signal1);
                }
                if (travel.down != null && travel.down != signal.from && !dullSignalDown)
                {
                    WireTileset.WireSignal signal2 = new WireTileset.WireSignal()
                    {
                        position = travel.position,
                        travel = travel.down,
                        from = travel,
                        currentWire = this,
                        life = signal.life,
                        prevPosition = signal.prevPosition,
                        signalType = signal.signalType
                    };
                    TravelSignal(signal2, overshoot, false);
                    if (!signal2.finished)
                        _addSignals.Add(signal2);
                }
                if (travel.left != null && travel.left != signal.from && !dullSignalLeft)
                {
                    WireTileset.WireSignal signal3 = new WireTileset.WireSignal()
                    {
                        position = travel.position,
                        travel = travel.left,
                        from = travel,
                        currentWire = this,
                        life = signal.life,
                        prevPosition = signal.prevPosition,
                        signalType = signal.signalType
                    };
                    TravelSignal(signal3, overshoot, false);
                    if (!signal3.finished)
                        _addSignals.Add(signal3);
                }
                if (travel.right != null && travel.right != signal.from && !dullSignalRight)
                {
                    WireTileset.WireSignal signal4 = new WireTileset.WireSignal()
                    {
                        position = travel.position,
                        travel = travel.right,
                        from = travel,
                        currentWire = this,
                        life = signal.life,
                        prevPosition = signal.prevPosition,
                        signalType = signal.signalType
                    };
                    TravelSignal(signal4, overshoot, false);
                    if (!signal4.finished)
                        _addSignals.Add(signal4);
                }
                Vec2 position = signal.travel.position;
                if (travel.wireLeft && !dullSignalLeft && this.leftTile is WireTileset leftTile && leftTile != signal.currentWire)
                {
                    signal.travel = leftTile.GetConnection(position);
                    leftTile.Emit(signal, overshoot, signal.signalType);
                }
                if (travel.wireRight && !dullSignalRight && this.rightTile is WireTileset rightTile && rightTile != signal.currentWire)
                {
                    signal.travel = rightTile.GetConnection(position);
                    rightTile.Emit(signal, overshoot, signal.signalType);
                }
                if (travel.wireUp && !dullSignalUp && this.upTile is WireTileset upTile && upTile != signal.currentWire)
                {
                    signal.travel = upTile.GetConnection(position);
                    upTile.Emit(signal, overshoot, signal.signalType);
                }
                if (travel.wireDown && !dullSignalDown && this.downTile is WireTileset downTile && downTile != signal.currentWire)
                {
                    signal.travel = downTile.GetConnection(position);
                    downTile.Emit(signal, overshoot, signal.signalType);
                }
                dullSignalDown = false;
                dullSignalUp = false;
                dullSignalLeft = false;
                dullSignalRight = false;
            }
        }

        public void TravelSignal(WireTileset.WireSignal signal, float travelSpeed, bool updatePrev = true)
        {
            if (updatePrev)
                signal.prevPosition = signal.position;
            float overshoot;
            if (signal.travel.position.x < signal.position.x)
            {
                signal.position.x -= travelSpeed;
                overshoot = signal.travel.position.x - signal.position.x;
            }
            else if (signal.travel.position.x > signal.position.x)
            {
                signal.position.x += travelSpeed;
                overshoot = signal.position.x - signal.travel.position.x;
            }
            else if (signal.travel.position.y > signal.position.y)
            {
                signal.position.y += travelSpeed;
                overshoot = signal.position.y - signal.travel.position.y;
            }
            else if (signal.travel.position.y < signal.position.y)
            {
                signal.position.y -= travelSpeed;
                overshoot = signal.travel.position.y - signal.position.y;
            }
            else
                overshoot = 0f;
            signal.life -= (float)(travelSpeed / 16.0 * 0.00999999977648258);
            if (overshoot >= 0.0 && signal.life > 0.0)
                Emit(signal, overshoot, signal.signalType);
            if (signal.life > 0.0)
                return;
            _removeSignals.Add(signal);
        }

        public WireConnection GetConnection(Vec2 pos)
        {
            float num = 9999f;
            WireConnection connection1 = _centerWire;
            foreach (WireConnection connection2 in _connections)
            {
                float lengthSq = (connection2.position - pos).lengthSq;
                if (lengthSq < num)
                {
                    num = lengthSq;
                    connection1 = connection2;
                }
            }
            return connection1;
        }

        public override void Update()
        {
            if (_centerWire == null)
                UpdateConnections();
            float travelSpeed = 16f;
            foreach (WireTileset.WireSignal signal in _signals)
                TravelSignal(signal, travelSpeed);
            foreach (WireTileset.WireSignal removeSignal in _removeSignals)
                _signals.Remove(removeSignal);
            foreach (WireTileset.WireSignal addSignal in _addSignals)
                _signals.Add(addSignal);
            _removeSignals.Clear();
            _addSignals.Clear();
            base.Update();
        }

        public override void Draw()
        {
            foreach (WireTileset.WireSignal signal in _signals)
            {
                _signalSprite.depth = -0.6f;
                _signalSprite.alpha = signal.life;
                _signalSprite.xscale = _signalSprite.yscale = 1f;
                Graphics.Draw(_signalSprite, signal.position.x, signal.position.y);
                Vec2 prevPosition = signal.prevPosition;
                Vec2 vec2 = signal.position - signal.prevPosition;
                float length = vec2.length;
                vec2.Normalize();
                float num = 0.3f;
                for (int index = 0; index < 3; ++index)
                {
                    Sprite signalSprite = _signalSprite;
                    signalSprite.depth -= 1;
                    prevPosition += vec2 * (length / 4f);
                    _signalSprite.alpha = num * signal.life;
                    num += 0.2f;
                    Graphics.Draw(_signalSprite, prevPosition.x, prevPosition.y);
                }
            }
            base.Draw();
        }

        private void UpdateConnections()
        {
            upTile = Level.CheckPoint<AutoTile>(x, y - 16f, this);
            downTile = Level.CheckPoint<AutoTile>(x, y + 16f, this);
            leftTile = Level.CheckPoint<AutoTile>(x - 16f, y, this);
            rightTile = Level.CheckPoint<AutoTile>(x + 16f, y, this);
            _connections.Clear();
            if (_sprite.frame == 32 || _sprite.frame == 41)
            {
                _centerWire = new WireConnection()
                {
                    position = position + new Vec2(0f, -4f)
                };
                WireConnection wireConnection = new WireConnection()
                {
                    position = position + new Vec2(8f, -4f),
                    left = _centerWire,
                    wireRight = true
                };
                _centerWire.right = wireConnection;
                _connections.Add(_centerWire);
                _connections.Add(wireConnection);
            }
            else if (_sprite.frame == 37 || _sprite.frame == 43)
            {
                _centerWire = new WireConnection()
                {
                    position = position + new Vec2(0f, -4f)
                };
                WireConnection wireConnection = new WireConnection()
                {
                    position = position + new Vec2(-8f, -4f),
                    right = _centerWire,
                    wireLeft = true
                };
                _centerWire.left = wireConnection;
                _connections.Add(_centerWire);
                _connections.Add(wireConnection);
            }
            else if (_sprite.frame == 33 || _sprite.frame == 35 || _sprite.frame == 36 || _sprite.frame == 59)
            {
                _centerWire = new WireConnection()
                {
                    position = position + new Vec2(0f, -4f)
                };
                WireConnection wireConnection1 = new WireConnection()
                {
                    position = position + new Vec2(8f, -4f),
                    left = _centerWire,
                    wireRight = true
                };
                WireConnection wireConnection2 = new WireConnection()
                {
                    position = position + new Vec2(-8f, -4f),
                    right = _centerWire,
                    wireLeft = true
                };
                _centerWire.right = wireConnection1;
                _centerWire.left = wireConnection2;
                _connections.Add(_centerWire);
                _connections.Add(wireConnection1);
                _connections.Add(wireConnection2);
            }
            else if (_sprite.frame == 34)
            {
                _centerWire = new WireConnection()
                {
                    position = position + new Vec2(0f, -4f)
                };
                WireConnection wireConnection3 = new WireConnection()
                {
                    position = position + new Vec2(8f, -4f),
                    left = _centerWire,
                    wireRight = true
                };
                WireConnection wireConnection4 = new WireConnection()
                {
                    position = position + new Vec2(-8f, -4f),
                    right = _centerWire,
                    wireLeft = true
                };
                WireConnection wireConnection5 = new WireConnection()
                {
                    position = position + new Vec2(0f, 8f),
                    up = _centerWire,
                    wireDown = true
                };
                _centerWire.right = wireConnection3;
                _centerWire.left = wireConnection4;
                _centerWire.down = wireConnection5;
                _connections.Add(_centerWire);
                _connections.Add(wireConnection3);
                _connections.Add(wireConnection4);
                _connections.Add(wireConnection5);
            }
            else if (_sprite.frame == 42)
            {
                _centerWire = new WireConnection()
                {
                    position = position + new Vec2(0f, -4f)
                };
                WireConnection wireConnection6 = new WireConnection()
                {
                    position = position + new Vec2(8f, -4f),
                    left = _centerWire,
                    wireRight = true
                };
                WireConnection wireConnection7 = new WireConnection()
                {
                    position = position + new Vec2(-8f, -4f),
                    right = _centerWire,
                    wireLeft = true
                };
                WireConnection wireConnection8 = new WireConnection()
                {
                    position = position + new Vec2(0f, 8f),
                    up = _centerWire,
                    wireDown = true
                };
                WireConnection wireConnection9 = new WireConnection()
                {
                    position = position + new Vec2(0f, -8f),
                    down = _centerWire,
                    wireUp = true
                };
                _centerWire.right = wireConnection6;
                _centerWire.left = wireConnection7;
                _centerWire.down = wireConnection8;
                _centerWire.up = wireConnection9;
                _connections.Add(_centerWire);
                _connections.Add(wireConnection6);
                _connections.Add(wireConnection7);
                _connections.Add(wireConnection8);
                _connections.Add(wireConnection9);
            }
            else if (_sprite.frame == 44)
            {
                _centerWire = new WireConnection()
                {
                    position = position + new Vec2(0f, 0f)
                };
                WireConnection wireConnection = new WireConnection()
                {
                    position = position + new Vec2(0f, -8f),
                    down = _centerWire,
                    wireUp = true
                };
                _centerWire.up = wireConnection;
                _connections.Add(_centerWire);
                _connections.Add(wireConnection);
            }
            else if (_sprite.frame == 45)
            {
                _centerWire = new WireConnection()
                {
                    position = position + new Vec2(0f, -4f)
                };
                WireConnection wireConnection10 = new WireConnection()
                {
                    position = position + new Vec2(-8f, -4f),
                    right = _centerWire,
                    wireLeft = true
                };
                WireConnection wireConnection11 = new WireConnection()
                {
                    position = position + new Vec2(0f, 8f),
                    up = _centerWire,
                    wireDown = true
                };
                WireConnection wireConnection12 = new WireConnection()
                {
                    position = position + new Vec2(0f, -8f),
                    down = _centerWire,
                    wireUp = true
                };
                _centerWire.left = wireConnection10;
                _centerWire.down = wireConnection11;
                _centerWire.up = wireConnection12;
                _connections.Add(_centerWire);
                _connections.Add(wireConnection10);
                _connections.Add(wireConnection11);
                _connections.Add(wireConnection12);
            }
            else if (_sprite.frame == 49)
            {
                _centerWire = new WireConnection()
                {
                    position = position + new Vec2(0f, 0f)
                };
                WireConnection wireConnection = new WireConnection()
                {
                    position = position + new Vec2(0f, 8f),
                    up = _centerWire,
                    wireDown = true
                };
                _centerWire.down = wireConnection;
                _connections.Add(_centerWire);
                _connections.Add(wireConnection);
            }
            else if (_sprite.frame == 50)
            {
                _centerWire = new WireConnection()
                {
                    position = position + new Vec2(0f, 0f)
                };
                WireConnection wireConnection13 = new WireConnection()
                {
                    position = position + new Vec2(0f, 8f),
                    up = _centerWire,
                    wireDown = true
                };
                WireConnection wireConnection14 = new WireConnection()
                {
                    position = position + new Vec2(0f, -8f),
                    down = _centerWire,
                    wireUp = true
                };
                _centerWire.down = wireConnection13;
                _centerWire.up = wireConnection14;
                _connections.Add(_centerWire);
                _connections.Add(wireConnection13);
                _connections.Add(wireConnection14);
            }
            else if (_sprite.frame == 51)
            {
                _centerWire = new WireConnection()
                {
                    position = position + new Vec2(0f, -4f)
                };
                WireConnection wireConnection15 = new WireConnection()
                {
                    position = position + new Vec2(8f, -4f),
                    left = _centerWire,
                    wireRight = true
                };
                WireConnection wireConnection16 = new WireConnection()
                {
                    position = position + new Vec2(0f, 8f),
                    up = _centerWire,
                    wireDown = true
                };
                _centerWire.right = wireConnection15;
                _centerWire.down = wireConnection16;
                _connections.Add(_centerWire);
                _connections.Add(wireConnection15);
                _connections.Add(wireConnection16);
            }
            else if (_sprite.frame == 52)
            {
                _centerWire = new WireConnection()
                {
                    position = position + new Vec2(0f, -4f)
                };
                WireConnection wireConnection17 = new WireConnection()
                {
                    position = position + new Vec2(-8f, -4f),
                    right = _centerWire,
                    wireLeft = true
                };
                WireConnection wireConnection18 = new WireConnection()
                {
                    position = position + new Vec2(0f, 8f),
                    up = _centerWire,
                    wireDown = true
                };
                _centerWire.left = wireConnection17;
                _centerWire.down = wireConnection18;
                _connections.Add(_centerWire);
                _connections.Add(wireConnection17);
                _connections.Add(wireConnection18);
            }
            else if (_sprite.frame == 53)
            {
                _centerWire = new WireConnection()
                {
                    position = position + new Vec2(0f, -4f)
                };
                WireConnection wireConnection19 = new WireConnection()
                {
                    position = position + new Vec2(8f, -4f),
                    left = _centerWire,
                    wireRight = true
                };
                WireConnection wireConnection20 = new WireConnection()
                {
                    position = position + new Vec2(0f, 8f),
                    up = _centerWire,
                    wireDown = true
                };
                WireConnection wireConnection21 = new WireConnection()
                {
                    position = position + new Vec2(0f, -8f),
                    down = _centerWire,
                    wireUp = true
                };
                _centerWire.right = wireConnection19;
                _centerWire.down = wireConnection20;
                _centerWire.up = wireConnection21;
                _connections.Add(_centerWire);
                _connections.Add(wireConnection19);
                _connections.Add(wireConnection20);
                _connections.Add(wireConnection21);
            }
            else if (_sprite.frame == 57)
            {
                _centerWire = new WireConnection()
                {
                    position = position + new Vec2(0f, -4f)
                };
                WireConnection wireConnection22 = new WireConnection()
                {
                    position = position + new Vec2(8f, -4f),
                    left = _centerWire,
                    wireRight = true
                };
                WireConnection wireConnection23 = new WireConnection()
                {
                    position = position + new Vec2(-8f, -4f),
                    right = _centerWire,
                    wireLeft = true
                };
                WireConnection wireConnection24 = new WireConnection()
                {
                    position = position + new Vec2(0f, -8f),
                    down = _centerWire,
                    wireUp = true
                };
                _centerWire.right = wireConnection22;
                _centerWire.left = wireConnection23;
                _centerWire.up = wireConnection24;
                _connections.Add(_centerWire);
                _connections.Add(wireConnection22);
                _connections.Add(wireConnection23);
                _connections.Add(wireConnection24);
            }
            else if (_sprite.frame == 58)
            {
                _centerWire = new WireConnection()
                {
                    position = position + new Vec2(0f, -4f)
                };
                WireConnection wireConnection25 = new WireConnection()
                {
                    position = position + new Vec2(8f, -4f),
                    left = _centerWire,
                    wireRight = true
                };
                WireConnection wireConnection26 = new WireConnection()
                {
                    position = position + new Vec2(0f, -8f),
                    down = _centerWire,
                    wireUp = true
                };
                _centerWire.right = wireConnection25;
                _centerWire.up = wireConnection26;
                _connections.Add(_centerWire);
                _connections.Add(wireConnection25);
                _connections.Add(wireConnection26);
            }
            else
            {
                if (_sprite.frame != 60)
                    return;
                _centerWire = new WireConnection()
                {
                    position = position + new Vec2(0f, -4f)
                };
                WireConnection wireConnection27 = new WireConnection()
                {
                    position = position + new Vec2(-8f, -4f),
                    right = _centerWire,
                    wireLeft = true
                };
                WireConnection wireConnection28 = new WireConnection()
                {
                    position = position + new Vec2(0f, -8f),
                    down = _centerWire,
                    wireUp = true
                };
                _centerWire.left = wireConnection27;
                _centerWire.up = wireConnection28;
                _connections.Add(_centerWire);
                _connections.Add(wireConnection27);
                _connections.Add(wireConnection28);
            }
        }

        public class WireSignal
        {
            public Vec2 position;
            public WireConnection travel;
            public WireConnection from;
            public WireTileset currentWire;
            public bool finished;
            public int signalType;
            public float life = 1f;
            public Vec2 prevPosition;
        }
    }
}
