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
            foreach (DXMLNode element1 in source.Elements())
            {
                foreach (DXMLNode element2 in element1.Elements("Line"))
                    _speechLines.Add(element1.Name, element2.Value);
            }
        }

        public static string GetLine(string type) => !_speechLines.ContainsKey(type) ? null : _speechLines[type][Rando.Int(_speechLines[type].Count - 1)];

        public static string GetRemark(
          string type,
          string name = null,
          string name2 = null,
          string extra01 = null,
          string extra02 = null)
        {
            string remark = GetLine(type);
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

        public static string GetRemark(string type, ResultData data) => data.multi ? GetTeamRemark(type, data.name) : GetIndividualRemark(type, data.name);

        public static string GetWinnerRemark(ResultData data) => data.multi ? GetLine("WinnerTeamRemark").Replace("%NAME%", data.name) : GetLine("WinnerIndividualRemark").Replace("%NAME%", data.name);

        public static string GetRunnerUpRemark(string type, ResultData data)
        {
            if (data.multi)
            {
                if (type.Contains("Positive"))
                    return GetLine("PositiveRunnerUpTeamRemark").Replace("%NAME%", data.name);
                if (type.Contains("Neutral"))
                    return GetLine("NeutralRunnerUpTeamRemark").Replace("%NAME%", data.name);
                return type.Contains("Negative") ? GetLine("NegativeRunnerUpTeamRemark").Replace("%NAME%", data.name) : "I don't know what to say!";
            }
            if (type.Contains("Positive"))
                return GetLine("PositiveRunnerUpIndividualRemark").Replace("%NAME%", data.name);
            if (type.Contains("Neutral"))
                return GetLine("NeutralRunnerUpIndividualRemark").Replace("%NAME%", data.name);
            return type.Contains("Negative") ? GetLine("NegativeRunnerUpIndividualRemark").Replace("%NAME%", data.name) : "I don't know what to say!";
        }

        public static string GetTeamRemark(string type, string name)
        {
            if (type.Contains("Positive"))
                return GetLine("PositiveTeamRemark").Replace("%NAME%", name);
            if (type.Contains("Neutral"))
                return GetLine("NeutralTeamRemark").Replace("%NAME%", name);
            return type.Contains("Negative") ? GetLine("NegativeTeamRemark").Replace("%NAME%", name) : "I don't know what to say!";
        }

        public static string GetIndividualRemark(string type, string name)
        {
            if (type.Contains("Positive"))
                return GetLine("PositiveIndividualRemark").Replace("%NAME%", name);
            if (type.Contains("Neutral"))
                return GetLine("NeutralIndividualRemark").Replace("%NAME%", name);
            return type.Contains("Negative") ? GetLine("NegativeIndividualRemark").Replace("%NAME%", name) : "I don't know what to say!";
        }
    }
}
