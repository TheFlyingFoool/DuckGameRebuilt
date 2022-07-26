// Decompiled with JetBrains decompiler
// Type: DuckGame.DCLine
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class DCLine
    {
        public string line;
        public Color color;
        public int threadIndex;
        public DateTime timestamp;
        public DCSection section;
        public Verbosity verbosity;
        public int frames;

        public override string ToString() => this.line + " | " + this.timestamp.ToLongTimeString();

        public string ToSendString() => this.timestamp.ToLongTimeString() + " " + this.SectionString() + " " + this.color.ToDGColorString() + this.line + "\n";

        public string ToShortString() => this.SectionString() + " " + this.line + "\n";

        public static Color ColorForSection(DCSection s)
        {
            switch (s)
            {
                case DCSection.NetCore:
                    return Colors.DGBlue;
                case DCSection.DuckNet:
                    return Colors.DGPink;
                case DCSection.GhostMan:
                    return Colors.DGPurple;
                case DCSection.Steam:
                    return Colors.DGOrange;
                case DCSection.Mod:
                    return Colors.DGGreen;
                case DCSection.Connection:
                    return Colors.DGYellow;
                case DCSection.Ack:
                    return Colors.DGGreen;
                default:
                    return Color.White;
            }
        }

        public static string StringForSection(DCSection s, bool colored, bool small, bool formatting = true)
        {
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

        public string SectionString(bool colored = true, bool small = false) => DCLine.StringForSection(this.section, colored, small);
    }
}
