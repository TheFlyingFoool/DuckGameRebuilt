using System;
using System.Reflection;

namespace XnaToFna.StubXDK
{
    public static class StubXDKHelper
    {
        private static Assembly _GamerServicesAsm;

        public static Assembly GamerServicesAsm
        {
            get
            {
                if (_GamerServicesAsm != null)
                    return _GamerServicesAsm;
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetType("Microsoft.Xna.Framework.GamerServices.GamerPresence") != null)
                        return _GamerServicesAsm = assembly;
                }
                return null;
            }
        }
    }
}
