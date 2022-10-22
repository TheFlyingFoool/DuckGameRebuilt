// Decompiled with JetBrains decompiler
// Type: DuckGame.InputObject
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class InputObject : Thing, ITakeInput
    {
        public StateBinding _profileNumberBinding = new StateBinding(nameof(profileNumber));
        public StateBinding _votedBinding = new StateBinding(nameof(voted));
        public StateBinding _inputChangeIndexBinding = new StateBinding(nameof(_inputChangeIndex));
        public StateBinding _leftStickBinding = new StateBinding(true, nameof(leftStick));
        public StateBinding _rightStickBinding = new StateBinding(true, nameof(rightStick));
        public StateBinding _leftTriggerBinding = new StateBinding(nameof(leftTrigger));
        public byte _inputChangeIndex;
        private sbyte _profileNumber;
        private Vec2 _leftStick;
        private Vec2 _rightStick;
        private float _leftTrigger;
        private InputProfile _blankProfile = new InputProfile();
        public Profile duckProfile;
        public bool voted;
        private ushort prevState;

        public sbyte profileNumber
        {
            get => _profileNumber;
            set
            {
                _profileNumber = value;
                duckProfile = DuckNetwork.profiles[_profileNumber];
                if (duckProfile == null || duckProfile.connection != DuckNetwork.localConnection)
                    return;
                connection = DuckNetwork.localConnection;
            }
        }

        public Vec2 leftStick
        {
            get => isServerForObject && inputProfile != null ? inputProfile.leftStick : _leftStick;
            set => _leftStick = value;
        }

        public Vec2 rightStick
        {
            get => isServerForObject && inputProfile != null ? inputProfile.rightStick : _rightStick;
            set => _rightStick = value;
        }

        public float leftTrigger
        {
            get
            {
                if (!isServerForObject || inputProfile == null)
                    return _leftTrigger;
                float leftTrigger = inputProfile.leftTrigger;
                if (inputProfile.hasMotionAxis)
                    leftTrigger += inputProfile.motionAxis;
                return leftTrigger;
            }
            set => _leftTrigger = value;
        }

        public InputProfile inputProfile => duckProfile != null ? duckProfile.inputProfile : _blankProfile;

        public InputObject()
          : base()
        {
        }

        public override void Update()
        {
            if (duckProfile != null && duckProfile.connection == DuckNetwork.localConnection)
                Thing.Fondle(this, DuckNetwork.localConnection);
            if (isServerForObject && inputProfile != null)
            {
                if (!Network.isServer)
                    inputProfile.UpdateTriggerStates();
                if (prevState != inputProfile.state)
                {
                    NetIndex8 authority = this.authority;
                    this.authority = ++authority;
                    _inputChangeIndex += 1;
                }
                prevState = inputProfile.state;
            }
            RegisteredVote vote = Vote.GetVote(DuckNetwork.profiles[_profileNumber]);
            if (vote != null)
            {
                vote.leftStick = leftStick;
                vote.rightStick = rightStick;
            }
            if (Level.current is RockScoreboard)
            {
                foreach (Slot3D slot in (Level.current as RockScoreboard)._slots)
                {
                    if (slot.duck != null && slot.duck.profile == duckProfile)
                    {
                        if (inputProfile.virtualDevice != null)
                        {
                            inputProfile.virtualDevice.leftStick = leftStick;
                            inputProfile.virtualDevice.rightStick = rightStick;
                        }
                        slot.ai._manualQuack = inputProfile;
                        slot.duck.manualQuackPitch = true;
                        slot.duck.quackPitch = (byte)(leftTrigger * byte.MaxValue);
                    }
                }
            }
            base.Update();
        }
    }
}
