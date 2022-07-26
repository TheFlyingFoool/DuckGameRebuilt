// Decompiled with JetBrains decompiler
// Type: DuckGame.InputProfileCore
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class InputProfileCore
    {
        public Dictionary<string, InputProfile> _profiles = new Dictionary<string, InputProfile>();
        public Dictionary<int, InputProfile> _virtualProfiles = new Dictionary<int, InputProfile>();

        public InputProfile Add(string name)
        {
            InputProfile inputProfile1 = new InputProfile(name);
            InputProfile inputProfile2;
            if (this._profiles.TryGetValue(name, out inputProfile2))
                return inputProfile2;
            this._profiles[name] = inputProfile1;
            return inputProfile1;
        }

        public InputProfile DefaultPlayer1 => this.Get(InputProfile.MPPlayer1);

        public InputProfile DefaultPlayer2 => this.Get(InputProfile.MPPlayer2);

        public InputProfile DefaultPlayer3 => this.Get(InputProfile.MPPlayer3);

        public InputProfile DefaultPlayer4 => this.Get(InputProfile.MPPlayer4);

        public InputProfile DefaultPlayer5 => this.Get(InputProfile.MPPlayer5);

        public InputProfile DefaultPlayer6 => this.Get(InputProfile.MPPlayer6);

        public InputProfile DefaultPlayer7 => this.Get(InputProfile.MPPlayer7);

        public InputProfile DefaultPlayer8 => this.Get(InputProfile.MPPlayer8);

        public List<InputProfile> defaultProfiles => new List<InputProfile>()
    {
      this.DefaultPlayer1,
      this.DefaultPlayer2,
      this.DefaultPlayer3,
      this.DefaultPlayer4,
      this.DefaultPlayer5,
      this.DefaultPlayer6,
      this.DefaultPlayer7,
      this.DefaultPlayer8
    };

        public InputProfile Get(string name)
        {
            InputProfile inputProfile;
            return this._profiles.TryGetValue(name, out inputProfile) ? inputProfile : (InputProfile)null;
        }

        public void Update()
        {
            foreach (KeyValuePair<string, InputProfile> profile in this._profiles)
                profile.Value.UpdateTriggerStates();
        }

        public InputProfile GetVirtualInput(int index)
        {
            InputProfile virtualInput1;
            if (this._virtualProfiles.TryGetValue(index, out virtualInput1))
                return virtualInput1;
            InputProfile virtualInput2 = this.Add("virtual" + index.ToString());
            virtualInput2.dindex = NetworkDebugger.currentIndex;
            VirtualInput device = new VirtualInput(index);
            device.pdraw = NetworkDebugger.currentIndex;
            for (int index1 = 0; index1 < Network.synchronizedTriggers.Count; ++index1)
                virtualInput2.Map((InputDevice)device, Network.synchronizedTriggers[index1], index1);
            device.availableTriggers = Network.synchronizedTriggers;
            this._virtualProfiles[index] = virtualInput2;
            return virtualInput2;
        }
    }
}
