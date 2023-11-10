namespace DuckGame
{
    public class NMVersionMismatch : NMDuckNetwork
    {
        public byte byteCode;
        public string serverVersion;

        public Type GetCode() => (Type)byteCode;

        public NMVersionMismatch()
        {
        }

        public NMVersionMismatch(Type code, string ver)
        {
            byteCode = (byte)code;
            serverVersion = ver;
        }

        public enum Type
        {
            Match = -1, // 0xFFFFFFFF
            Older = 0,
            Newer = 1,
            Error = 2,
        }
    }
}
