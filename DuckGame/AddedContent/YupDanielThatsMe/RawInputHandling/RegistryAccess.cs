using Microsoft.Win32;

namespace RawInput
{
    static internal class RegistryAccess
    {
        static internal RegistryKey GetDeviceKey(string device)
        {
            string[] split = device.Substring(4).Split('#');

            string classCode = split[0];       // ACPI (Class code)
            string subClassCode = split[1];    // PNP0303 (SubClass code)
            string protocolCode = split[2];    // 3&13c0b0c5&0 (Protocol code)

            return Registry.LocalMachine.OpenSubKey(string.Format(@"System\CurrentControlSet\Enum\{0}\{1}\{2}", classCode, subClassCode, protocolCode));
        }

        static internal string GetClassType(string classGuid)
        {
            RegistryKey classGuidKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\" + classGuid);

            return classGuidKey != null ? (string)classGuidKey.GetValue("Class") : string.Empty;
        }
    }
}
