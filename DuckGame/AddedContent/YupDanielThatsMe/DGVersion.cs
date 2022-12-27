namespace DuckGame
{
    public class DGVersion
    {
        public int[] VersionSegments;
        public string VersionString;
        public DGVersion(string version)
        {
            VersionString = version.Replace("v", "").Replace("-", "").Replace("beta", "");
            string[] VersionStringSegments = VersionString.Split('.');
            VersionSegments = new int[VersionStringSegments.Length];
            for (int i = 0; i < VersionStringSegments.Length; i++)
            {
                string VersionStringSegement = VersionStringSegments[i];
                if (!int.TryParse(VersionStringSegement, out int result))
                {
                    result = 0;
                }
                VersionSegments[i] = result;
            }
        }
        public static bool operator <(DGVersion version, DGVersion otherversion)
        {
            for (int i = 0; i < version.VersionSegments.Length; i++)
            {
                if (i >= otherversion.VersionSegments.Length)
                {
                    return false;
                }
                if (version.VersionSegments[i] < otherversion.VersionSegments[i])
                {
                    return true;
                }
                else if (version.VersionSegments[i] > otherversion.VersionSegments[i])
                {
                    return false;
                }
            }
            if (version.VersionSegments.Length == otherversion.VersionSegments.Length)
            {
                return false;
            }
            return true;
        }
        public static bool operator >(DGVersion version, DGVersion otherversion)
        {
            for (int i =0; i < version.VersionSegments.Length; i++)
            {
                if (i >= otherversion.VersionSegments.Length)
                {
                    return true;
                }
                if (version.VersionSegments[i] > otherversion.VersionSegments[i])
                {
                    return true;
                }
                else if (version.VersionSegments[i] < otherversion.VersionSegments[i])
                {
                    return false;
                }
            }
            return false;
        }

        public static bool operator <=(DGVersion version, DGVersion otherversion)
        {
            for (int i = 0; i < version.VersionSegments.Length; i++)
            {
                if (i >= otherversion.VersionSegments.Length)
                {
                    return false;
                }
                if (version.VersionSegments[i] < otherversion.VersionSegments[i])
                {
                    return true;
                }
                else if (version.VersionSegments[i] > otherversion.VersionSegments[i])
                {
                    return false;
                }
            }
            return true;
        }
        public static bool operator >=(DGVersion version, DGVersion otherversion)
        {
            for (int i = 0; i < version.VersionSegments.Length; i++)
            {
                if (i >= otherversion.VersionSegments.Length)
                {
                    return true;
                }
                if (version.VersionSegments[i] > otherversion.VersionSegments[i])
                {
                    return true;
                }
                else if (version.VersionSegments[i] < otherversion.VersionSegments[i])
                {
                    return false;
                }
            }
            if (version.VersionSegments.Length == otherversion.VersionSegments.Length)
            {
                return true;
            }
            return false;
        }
        public static bool operator ==(DGVersion version, DGVersion otherversion)
        {
            if (version.VersionSegments.Length != otherversion.VersionSegments.Length)
            {
                return false;
            }
            for (int i =0; i < version.VersionSegments.Length; i++)
            {
                if (version.VersionSegments[i] != otherversion.VersionSegments[i])
                {
                    return false;
                }
            }
            return true;
        }
        public static bool operator !=(DGVersion version, DGVersion otherversion)
        {
            return !(version == otherversion);
        }
    }
}
