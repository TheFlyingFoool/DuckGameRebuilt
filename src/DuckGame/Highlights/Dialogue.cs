// Decompiled with JetBrains decompiler
// Type: DuckGame.Dialogue
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class Dialogue
    {
        private static MultiMap<string, string> _speechLines = new MultiMap<string, string>();

        public static void Initialize()
        {
            IEnumerable<DXMLNode> source = DuckXML.Load(DuckFile.OpenStream("Content/dialogue/sportscaster.tlk")).Elements(nameof(Dialogue));
            if (source == null)
                return;
            foreach (DXMLNode element1 in source.Elements<DXMLNode>())
            {
                foreach (DXMLNode element2 in element1.Elements("Line"))
                    Dialogue._speechLines.Add(element1.Name, element2.Value);
            }
        }

        public static string GetLine(string type) => !Dialogue._speechLines.ContainsKey(type) ? null : Dialogue._speechLines[type][Rando.Int(Dialogue._speechLines[type].Count - 1)];

        public static string GetRemark(
          string type,
          string name = null,
          string name2 = null,
          string extra01 = null,
          string extra02 = null)
        {
            string remark = Dialogue.GetLine(type);
            if (remark != null)
            {
                if (name != null)
                    remark = remark.Replace("%NAME%", name);
                if (name2 != null)
                    remark = remark.Replace("%NAME2%", name2);
                if (extra01 != null)
                    remark = remark.Replace("%DATA%", extra01);
                if (extra02 != null)
                    remark = remark.Replace("%DATA2%", extra02);
            }
            return remark;
        }

        public static string GetRemark(string type, ResultData data) => data.multi ? Dialogue.GetTeamRemark(type, data.name) : Dialogue.GetIndividualRemark(type, data.name);

        public static string GetWinnerRemark(ResultData data) => data.multi ? Dialogue.GetLine("WinnerTeamRemark").Replace("%NAME%", data.name) : Dialogue.GetLine("WinnerIndividualRemark").Replace("%NAME%", data.name);

        public static string GetRunnerUpRemark(string type, ResultData data)
        {
            if (data.multi)
            {
                if (type.Contains("Positive"))
                    return Dialogue.GetLine("PositiveRunnerUpTeamRemark").Replace("%NAME%", data.name);
                if (type.Contains("Neutral"))
                    return Dialogue.GetLine("NeutralRunnerUpTeamRemark").Replace("%NAME%", data.name);
                return type.Contains("Negative") ? Dialogue.GetLine("NegativeRunnerUpTeamRemark").Replace("%NAME%", data.name) : "I don't know what to say!";
            }
            if (type.Contains("Positive"))
                return Dialogue.GetLine("PositiveRunnerUpIndividualRemark").Replace("%NAME%", data.name);
            if (type.Contains("Neutral"))
                return Dialogue.GetLine("NeutralRunnerUpIndividualRemark").Replace("%NAME%", data.name);
            return type.Contains("Negative") ? Dialogue.GetLine("NegativeRunnerUpIndividualRemark").Replace("%NAME%", data.name) : "I don't know what to say!";
        }

        public static string GetTeamRemark(string type, string name)
        {
            if (type.Contains("Positive"))
                return Dialogue.GetLine("PositiveTeamRemark").Replace("%NAME%", name);
            if (type.Contains("Neutral"))
                return Dialogue.GetLine("NeutralTeamRemark").Replace("%NAME%", name);
            return type.Contains("Negative") ? Dialogue.GetLine("NegativeTeamRemark").Replace("%NAME%", name) : "I don't know what to say!";
        }

        public static string GetIndividualRemark(string type, string name)
        {
            if (type.Contains("Positive"))
                return Dialogue.GetLine("PositiveIndividualRemark").Replace("%NAME%", name);
            if (type.Contains("Neutral"))
                return Dialogue.GetLine("NeutralIndividualRemark").Replace("%NAME%", name);
            return type.Contains("Negative") ? Dialogue.GetLine("NegativeIndividualRemark").Replace("%NAME%", name) : "I don't know what to say!";
        }
    }
}
