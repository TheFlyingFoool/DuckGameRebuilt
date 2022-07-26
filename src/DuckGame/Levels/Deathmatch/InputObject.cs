// Decompiled with JetBrains decompiler
// Type: DuckGame.InputObject
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            get => this._profileNumber;
            set
            {
                this._profileNumber = value;
                this.duckProfile = DuckNetwork.profiles[(int)this._profileNumber];
                if (this.duckProfile == null || this.duckProfile.connection != DuckNetwork.localConnection)
                    return;
                this.connection = DuckNetwork.localConnection;
            }
        }

        public Vec2 leftStick
        {
            get => this.isServerForObject && this.inputProfile != null ? this.inputProfile.leftStick : this._leftStick;
            set => this._leftStick = value;
        }

        public Vec2 rightStick
        {
            get => this.isServerForObject && this.inputProfile != null ? this.inputProfile.rightStick : this._rightStick;
            set => this._rightStick = value;
        }

        public float leftTrigger
        {
            get
            {
                if (!this.isServerForObject || this.inputProfile == null)
                    return this._leftTrigger;
                float leftTrigger = this.inputProfile.leftTrigger;
                if (this.inputProfile.hasMotionAxis)
                    leftTrigger += this.inputProfile.motionAxis;
                return leftTrigger;
            }
            set => this._leftTrigger = value;
        }

        public InputProfile inputProfile => this.duckProfile != null ? this.duckProfile.inputProfile : this._blankProfile;

        public InputObject()
          : base()
        {
        }

        public override void Update()
        {
            if (this.duckProfile != null && this.duckProfile.connection == DuckNetwork.localConnection)
                Thing.Fondle((Thing)this, DuckNetwork.localConnection);
            if (this.isServerForObject && this.inputProfile != null)
            {
                if (!Network.isServer)
                    this.inputProfile.UpdateTriggerStates();
                if ((int)this.prevState != (int)this.inputProfile.state)
                {
                    global::DuckGame.NetIndex8 authority = this.authority;
                    this.authority = ++authority;
                    this._inputChangeIndex += 1;
                }
                this.prevState = this.inputProfile.state;
            }
            RegisteredVote vote = Vote.GetVote(DuckNetwork.profiles[(int)this._profileNumber]);
            if (vote != null)
            {
                vote.leftStick = this.leftStick;
                vote.rightStick = this.rightStick;
            }
            if (Level.current is RockScoreboard)
            {
                foreach (Slot3D slot in (Level.current as RockScoreboard)._slots)
                {
                    if (slot.duck != null && slot.duck.profile == this.duckProfile)
                    {
                        if (this.inputProfile.virtualDevice != null)
                        {
                            this.inputProfile.virtualDevice.leftStick = this.leftStick;
                            this.inputProfile.virtualDevice.rightStick = this.rightStick;
                        }
                        slot.ai._manualQuack = this.inputProfile;
                        slot.duck.manualQuackPitch = true;
                        slot.duck.quackPitch = (byte)((double)this.leftTrigger * (double)byte.MaxValue);
                    }
                }
            }
            base.Update();
        }
    }
}
