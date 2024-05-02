//using NAudio.Midi;
//using System;

//namespace DuckGame
//{
//    internal static class Midi
//    {
//        private static MidiIn _midi;

//        public static void Initialize()
//        {
//            string[] strArray = new string[MidiIn.NumberOfDevices];
//            for (int midiInDeviceNumber = 0; midiInDeviceNumber < MidiIn.NumberOfDevices; ++midiInDeviceNumber)
//                strArray[midiInDeviceNumber] = MidiIn.DeviceInfo(midiInDeviceNumber).ProductName;
//            DuckGame.Midi._midi = new MidiIn(0);
//            DuckGame.Midi._midi.MessageReceived += new EventHandler<MidiInMessageEventArgs>(DuckGame.Midi.MidiMessage);
//        }

//        private static void MidiMessage(object sender, MidiInMessageEventArgs args)
//        {
//            int commandCode = (int)args.MidiEvent.CommandCode;
//        }
//    }
//}
