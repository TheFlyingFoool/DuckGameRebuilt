using MonoMod;
using System;
using System.Reflection;

namespace XnaToFna.StubXDK.Net
{
    public static class __NetworkSession__
    {
        private static Type t_NetworkNotAvailableException;
        private static ConstructorInfo ctor_NetworkNotAvailableException;

        [MonoModHook("System.Int32 Microsoft.Xna.Framework.Net.NetworkSession::get_BytesPerSecondSent()")]
        public static int get_BytesPerSecondSent(object session) => 0;

        [MonoModHook("System.Int32 Microsoft.Xna.Framework.Net.NetworkSession::get_BytesPerSecondReceived()")]
        public static int get_BytesPerSecondReceived(object session) => 0;

        [MonoModHook("System.IAsyncResult Microsoft.Xna.Framework.Net.NetworkSession::BeginJoinInvited(System.Collections.Generic.IEnumerable`1<Microsoft.Xna.Framework.GamerServices.SignedInGamer>,System.AsyncCallback,System.Object)")]
        public static IAsyncResult BeginJoinInvited(object gamers, AsyncCallback cb, object obj)
        {
            if (t_NetworkNotAvailableException == null)
            {
                t_NetworkNotAvailableException = StubXDKHelper.GamerServicesAsm.GetType("Microsoft.Xna.Framework.Net.NetworkNotAvailableException");
                ctor_NetworkNotAvailableException = t_NetworkNotAvailableException.GetConstructor(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, new Type[0], null);
            }
            throw (Exception)ctor_NetworkNotAvailableException.Invoke(new object[0]);
        }

        [MonoModHook("System.IAsyncResult Microsoft.Xna.Framework.Net.NetworkSession::BeginJoinInvited(System.Int32,System.AsyncCallback,System.Object)")]
        public static IAsyncResult BeginJoinInvited(int a, AsyncCallback cb, object obj)
        {
            if (t_NetworkNotAvailableException == null)
            {
                t_NetworkNotAvailableException = StubXDKHelper.GamerServicesAsm.GetType("Microsoft.Xna.Framework.Net.NetworkNotAvailableException");
                ctor_NetworkNotAvailableException = t_NetworkNotAvailableException.GetConstructor(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, new Type[0], null);
            }
            throw (Exception)ctor_NetworkNotAvailableException.Invoke(new object[0]);
        }

        [MonoModHook("Microsoft.Xna.Framework.Net.NetworkSession Microsoft.Xna.Framework.Net.NetworkSession::EndJoinInvited(System.IAsyncResult)")]
        public static object EndJoinInvited(IAsyncResult result)
        {
            if (t_NetworkNotAvailableException == null)
            {
                t_NetworkNotAvailableException = StubXDKHelper.GamerServicesAsm.GetType("Microsoft.Xna.Framework.Net.NetworkNotAvailableException");
                ctor_NetworkNotAvailableException = t_NetworkNotAvailableException.GetConstructor(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, new Type[0], null);
            }
            throw (Exception)ctor_NetworkNotAvailableException.Invoke(new object[0]);
        }
    }
}
