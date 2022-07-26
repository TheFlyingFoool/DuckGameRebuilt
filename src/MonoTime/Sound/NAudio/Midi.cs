// Decompiled with JetBrains decompiler
// Type: DuckGame.Midi
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using NAudio.Midi;
using System;

namespace DuckGame
{
    internal static class Midi
    {
        private static MidiIn _midi;

        public static void Initialize()
        {
            string[] strArray = new string[MidiIn.NumberOfDevices];
            for (int midiInDeviceNumber = 0; midiInDeviceNumber < MidiIn.NumberOfDevices; ++midiInDeviceNumber)
                strArray[midiInDeviceNumber] = MidiIn.DeviceInfo(midiInDeviceNumber).ProductName;
            DuckGame.Midi._midi = new MidiIn(0);
            DuckGame.Midi._midi.MessageReceived += new EventHandler<MidiInMessageEventArgs>(DuckGame.Midi.MidiMessage);
        }

        private static void MidiMessage(object sender, MidiInMessageEventArgs args)
        {
            int commandCode = (int)args.MidiEvent.CommandCode;
        }
    }
}
