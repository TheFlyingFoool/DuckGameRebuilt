// Decompiled with JetBrains decompiler
// Type: DuckGame.NMMatchSettings
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            this.roundsPerIntermission = varRoundsPerIntermission;
            this.winsPerSet = varWinsPerSet;
            this.randomPercent = varRandomPercent;
            this.workshopPercent = varWorkshopPercent;
            this.teams = varTeams;
            this._enabledModifiers = enabledModifiers;
            this.initialSettings = initial;
            this.customPercent = varCustomPercent;
            this.customLevels = varCustomLevels;
            this.normalPercent = varNormalPercent;
            this.wallmode = varWallmode;
            this.clientLevels = varClientLevels;
        }

        protected override void OnSerialize()
        {
            base.OnSerialize();
            this._serializedData.Write((byte)this._enabledModifiers.Count);
            foreach (byte enabledModifier in this._enabledModifiers)
                this._serializedData.Write(enabledModifier);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            base.OnDeserialize(d);
            byte num = d.ReadByte();
            for (int index = 0; index < (int)num; ++index)
                this._enabledModifiers.Add(d.ReadByte());
        }

        public override void Activate()
        {
            DuckNetwork.SetMatchSettings(this.initialSettings, (int)this.winsPerSet, (int)this.roundsPerIntermission, this.teams, this.wallmode, (int)this.normalPercent, (int)this.randomPercent, (int)this.workshopPercent, (int)this.customPercent, this.customLevels, this._enabledModifiers, this.clientLevels);
            base.Activate();
        }
    }
}
