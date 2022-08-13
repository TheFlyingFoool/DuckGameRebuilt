// Decompiled with JetBrains decompiler
// Type: DuckGame.InputProfileCore
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            if (_profiles.TryGetValue(name, out inputProfile2))
                return inputProfile2;
            _profiles[name] = inputProfile1;
            return inputProfile1;
        }

        public InputProfile DefaultPlayer1 => Get(InputProfile.MPPlayer1);

        public InputProfile DefaultPlayer2 => Get(InputProfile.MPPlayer2);

        public InputProfile DefaultPlayer3 => Get(InputProfile.MPPlayer3);

        public InputProfile DefaultPlayer4 => Get(InputProfile.MPPlayer4);

        public InputProfile DefaultPlayer5 => Get(InputProfile.MPPlayer5);

        public InputProfile DefaultPlayer6 => Get(InputProfile.MPPlayer6);

        public InputProfile DefaultPlayer7 => Get(InputProfile.MPPlayer7);

        public InputProfile DefaultPlayer8 => Get(InputProfile.MPPlayer8);

        public List<InputProfile> defaultProfiles => new List<InputProfile>()
    {
      DefaultPlayer1,
      DefaultPlayer2,
      DefaultPlayer3,
      DefaultPlayer4,
      DefaultPlayer5,
      DefaultPlayer6,
      DefaultPlayer7,
      DefaultPlayer8
    };

        public InputProfile Get(string name)
        {
            InputProfile inputProfile;
            return _profiles.TryGetValue(name, out inputProfile) ? inputProfile : null;
        }

        public void Update()
        {
            foreach (KeyValuePair<string, InputProfile> profile in _profiles)
                profile.Value.UpdateTriggerStates();
        }

        public InputProfile GetVirtualInput(int index)
        {
            InputProfile virtualInput1;
            if (_virtualProfiles.TryGetValue(index, out virtualInput1))
                return virtualInput1;
            InputProfile virtualInput2 = Add("virtual" + index.ToString());
            virtualInput2.dindex = NetworkDebugger.currentIndex;
            VirtualInput device = new VirtualInput(index)
            {
                pdraw = NetworkDebugger.currentIndex
            };
            for (int index1 = 0; index1 < Network.synchronizedTriggers.Count; ++index1)
                virtualInput2.Map(device, Network.synchronizedTriggers[index1], index1);
            device.availableTriggers = Network.synchronizedTriggers;
            _virtualProfiles[index] = virtualInput2;
            return virtualInput2;
        }
    }
}
