// Decompiled with JetBrains decompiler
// Type: DuckGame.DCLine
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Linq;

namespace DuckGame
{
    public class DCLine
    {
        public string line;
        public Color color;
        public int threadIndex;
        public DateTime timestamp = DateTime.Now;
        public DCSection section;
        public Verbosity verbosity;
        public int frames;

        public override string ToString() => line + " | " + timestamp.ToLongTimeString();

        public string ToSendString() => timestamp.ToLongTimeString() + " " + SectionString() + " " + color.ToDGColorString() + line + "\n";

        public string ToShortString() => SectionString() + " " + line + "\n";

        public static Color ColorForSection(DCSection s) =>
            s switch
            {
                DCSection.NetCore => Colors.DGBlue,
                DCSection.DuckNet => Colors.DGPink,
                DCSection.GhostMan => Colors.DGPurple,
                DCSection.Steam => Colors.DGOrange,
                DCSection.Mod => Colors.DGGreen,
                DCSection.Connection => Colors.DGYellow,
                DCSection.Ack => Colors.DGGreen,
                _ => Color.White
            };

        private static readonly string[] s_colorTable = {
            Color.White.ToDGColorString(),
            Colors.DGBlue.ToDGColorString(),
            Colors.DGPink2.ToDGColorString(), 
            Colors.DGPurple.ToDGColorString(), 
            Colors.DGOrange.ToDGColorString(),
            Colors.DGGreen.ToDGColorString(), 
            Colors.DGYellow.ToDGColorString(), 
            Colors.DGGreen.ToDGColorString(),
        };

        private static readonly string[] s_nameTable = {
            string.Empty,
            DCSection.NetCore.ToString().ToUpper().SetLengthLogically(2),
            DCSection.DuckNet.ToString().ToUpper().SetLengthLogically(2),
            DCSection.GhostMan.ToString().ToUpper().SetLengthLogically(2),
            DCSection.Steam.ToString().ToUpper().SetLengthLogically(2),
            DCSection.Mod.ToString().ToUpper().SetLengthLogically(2),
            DCSection.Connection.ToString().ToUpper().SetLengthLogically(2),
            DCSection.Ack.ToString().ToUpper().SetLengthLogically(2),
            string.Empty,
            DCSection.NetCore.ToString().ToUpper().SetLengthLogically(4),
            DCSection.DuckNet.ToString().ToUpper().SetLengthLogically(4),
            DCSection.GhostMan.ToString().ToUpper().SetLengthLogically(4),
            DCSection.Steam.ToString().ToUpper().SetLengthLogically(4),
            DCSection.Mod.ToString().ToUpper().SetLengthLogically(4),
            DCSection.Connection.ToString().ToUpper().SetLengthLogically(4),
            DCSection.Ack.ToString().ToUpper().SetLengthLogically(4),
        };
        
        public static string StringForSection(DCSection s, bool colored, bool small, bool formatting = true)
        {
            if (DGRSettings.UseDuckShell)
            {
                int sectionIndex = (int)s;
                string colorTag = colored && s != DCSection.General ? s_colorTable[sectionIndex] : string.Empty;
                string sectionCode = s_nameTable[sectionIndex + (small ? 0 : 8)];
                string spacer = s == DCSection.General ? string.Empty : " ";
                
                // TODO: replace concatenation with precalculated values
                return colorTag + sectionCode + spacer;
            }
            
            if (formatting)
            {
                if (small)
                {
                    if (colored)
                    {
                        switch (s)
                        {
                            case DCSection.NetCore:
                                return "|DGBLUE|NC  ";
                            case DCSection.DuckNet:
                                return "|PINK|DN  ";
                            case DCSection.GhostMan:
                                return "|DGPURPLE|GM  ";
                            case DCSection.Steam:
                                return "|DGORANGE|ST  ";
                            case DCSection.Mod:
                                return "|DGGREEN|MD  ";
                            case DCSection.Connection:
                                return "|DGYELLOW|CN  ";
                            case DCSection.Ack:
                                return "|DGGREEN|AK  ";
                        }
                    }
                    else
                    {
                        switch (s)
                        {
                            case DCSection.NetCore:
                                return "NC  ";
                            case DCSection.DuckNet:
                                return "DN  ";
                            case DCSection.GhostMan:
                                return "GM  ";
                            case DCSection.Steam:
                                return "ST  ";
                            case DCSection.Mod:
                                return "MD  ";
                            case DCSection.Connection:
                                return "CN  ";
                            case DCSection.Ack:
                                return "AK  ";
                        }
                    }
                }
                if (colored)
                {
                    switch (s)
                    {
                        case DCSection.NetCore:
                            return "|DGBLUE|NETCORE  ";
                        case DCSection.DuckNet:
                            return "|PINK|DUCKNET  ";
                        case DCSection.GhostMan:
                            return "|DGPURPLE|GHOSTMAN ";
                        case DCSection.Steam:
                            return "|DGORANGE|STEAM    ";
                        case DCSection.Mod:
                            return "|DGGREEN|MOD      ";
                        case DCSection.Connection:
                            return "|DGYELLOW|CONNECT  ";
                        case DCSection.Ack:
                            return "|DGGREEN|ACK      ";
                    }
                }
                else
                {
                    switch (s)
                    {
                        case DCSection.NetCore:
                            return "NETCORE  ";
                        case DCSection.DuckNet:
                            return "DUCKNET  ";
                        case DCSection.GhostMan:
                            return "GHOSTMAN ";
                        case DCSection.Steam:
                            return "STEAM    ";
                        case DCSection.Mod:
                            return "MOD      ";
                        case DCSection.Connection:
                            return "CONNECT  ";
                        case DCSection.Ack:
                            return "ACK      ";
                    }
                }
            }
            else
            {
                if (small)
                {
                    if (colored)
                    {
                        switch (s)
                        {
                            case DCSection.NetCore:
                                return "|DGBLUE|NC";
                            case DCSection.DuckNet:
                                return "|PINK|DN";
                            case DCSection.GhostMan:
                                return "|DGPURPLE|GM";
                            case DCSection.Steam:
                                return "|DGORANGE|ST";
                            case DCSection.Mod:
                                return "|DGGREEN|MD";
                            case DCSection.Connection:
                                return "|DGYELLOW|CN";
                            case DCSection.Ack:
                                return "|DGGREEN|AK";
                        }
                    }
                    else
                    {
                        switch (s)
                        {
                            case DCSection.NetCore:
                                return "NC";
                            case DCSection.DuckNet:
                                return "DN";
                            case DCSection.GhostMan:
                                return "GM";
                            case DCSection.Steam:
                                return "ST";
                            case DCSection.Mod:
                                return "MD";
                            case DCSection.Connection:
                                return "CN";
                            case DCSection.Ack:
                                return "AK";
                        }
                    }
                }
                if (colored)
                {
                    switch (s)
                    {
                        case DCSection.NetCore:
                            return "|DGBLUE|NETCORE";
                        case DCSection.DuckNet:
                            return "|PINK|DUCKNET";
                        case DCSection.GhostMan:
                            return "|DGPURPLE|GHOSTMAN";
                        case DCSection.Steam:
                            return "|DGORANGE|STEAM";
                        case DCSection.Mod:
                            return "|DGGREEN|MOD";
                        case DCSection.Connection:
                            return "|DGYELLOW|CONNECT";
                        case DCSection.Ack:
                            return "|DGGREEN|ACK";
                    }
                }
                else
                {
                    switch (s)
                    {
                        case DCSection.NetCore:
                            return "NETCORE";
                        case DCSection.DuckNet:
                            return "DUCKNET";
                        case DCSection.GhostMan:
                            return "GHOSTMAN";
                        case DCSection.Steam:
                            return "STEAM";
                        case DCSection.Mod:
                            return "MOD";
                        case DCSection.Connection:
                            return "CONNECT";
                        case DCSection.Ack:
                            return "ACK";
                    }
                }
            }
            return "";
        }

        public string SectionString(bool colored = true, bool small = false) => StringForSection(section, colored, small);
    }
}
