using System.Collections.Generic;

namespace DuckGame
{
    public class NMMatchSettings : NMDuckNetworkEvent
    {
        public byte winsPerSet;
        public byte roundsPerIntermission;
        public byte randomPercent;
        public byte workshopPercent;
        public bool teams;
        public bool initialSettings;
        public byte customPercent;
        public byte normalPercent;
        public bool wallmode;
        public int customLevels;
        public bool clientLevels;
        private List<byte> _enabledModifiers = new List<byte>();

        public NMMatchSettings()
        {
        }

        public NMMatchSettings(
          bool initial,
          byte varWinsPerSet,
          byte varRoundsPerIntermission,
          byte varRandomPercent,
          byte varWorkshopPercent,
          byte varNormalPercent,
          bool varTeams,
          byte varCustomPercent,
          int varCustomLevels,
          bool varWallmode,
          List<byte> enabledModifiers,
          bool varClientLevels)
        {
            roundsPerIntermission = varRoundsPerIntermission;
            winsPerSet = varWinsPerSet;
            randomPercent = varRandomPercent;
            workshopPercent = varWorkshopPercent;
            teams = varTeams;
            _enabledModifiers = enabledModifiers;
            initialSettings = initial;
            customPercent = varCustomPercent;
            customLevels = varCustomLevels;
            normalPercent = varNormalPercent;
            wallmode = varWallmode;
            clientLevels = varClientLevels;
        }

        protected override void OnSerialize()
        {
            base.OnSerialize();
            _serializedData.Write((byte)_enabledModifiers.Count);
            foreach (byte enabledModifier in _enabledModifiers)
                _serializedData.Write(enabledModifier);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            base.OnDeserialize(d);
            byte num = d.ReadByte();
            for (int index = 0; index < num; ++index)
                _enabledModifiers.Add(d.ReadByte());
        }

        public override void Activate()
        {
            DuckNetwork.SetMatchSettings(initialSettings, winsPerSet, roundsPerIntermission, teams, wallmode, normalPercent, randomPercent, workshopPercent, customPercent, customLevels, _enabledModifiers, clientLevels);
            base.Activate();
        }
    }
}
