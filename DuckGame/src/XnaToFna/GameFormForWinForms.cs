using System;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace XnaToFna.ProxyForms
{
    /// <summary>
    /// This is an FNA compatibility shim for XNA mods that 
    /// were using references to XNA's "Game.Window.Handle" casted as a WinForm's Form.
    /// It is a non intrusive alternative to "ProxyForms.GameForm" 
    /// that doesn't require relinking multiple WinForms types.
    /// 
    /// What this shim does:
    /// - Keeps transparent compatibility with XNA mods on FNA (on Windows).
    /// - Supports non-Windows OS by using a fallback to return null/Zero.
    /// - Creates a valid Form handle using reflection to prevent hard references to WinForms.
    /// - Only requires re-linking "Game.Window.Handle" function, not the whole "Form" class and methods.
    /// 
    /// Why some mods need this:
    /// In XNA the "Game.Window.Handle" property was a pointer to the main "System.Windows.Forms.Form".
    /// In FNA "Game.Window.Handle" still exists, but it is an SDL handle, no longer a Form.
    /// The SDL handle can be of different types: HWND (on Windows) / X11 (on Linux) / Cocoa (on MacOS).
    /// The SDL handle cannot be converted into a WinForm's Form anymore, not even on Windows.
    /// 
    /// How to solve it:
    /// Re-linking the mod's DLL references to "Game.Window.Handle" 
    /// and replacing it with "GameFormForWinForms.GetHandle()".
    /// This will return a valid WinForm's Form handle on Windows, and null/Zero in Linux.
    /// 
    /// Example of mod's code having this problem (deprecated pattern):
    /// var form = ((System.Windows.Forms.Form) System.Windows.Forms.Control.FromHandle(DuckGame.MonoMain.instance.Window.Handle));
    /// form.FormClosing += SomeModFunction;
    /// 
    /// For new mods please use a Harmony prefix patch in 
    /// "MonoMain.OnExiting" for a cross-platform solution instead.
    /// </summary>
    internal static class GameFormForWinForms
    {
        /// <summary>
        /// This object type is "System.Windows.Forms.Form".
        /// It is created using reflection to avoid direct references to WinForms.
        /// </summary>
        public static object FormInstance;
        private static IntPtr _handle = IntPtr.Zero;
        private static bool _initialized;

        public const string WinFormsAssemblyName = "System.Windows.Forms";
        public const string WinFormsAssemblyStrongName = WinFormsAssemblyName + ", Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        public const string WinFormsNamespace = "System.Windows.Forms";

        public static class FormClass
        {
            public const string Name = "Form";
            public const string NameWithNamespace = WinFormsNamespace + "." + Name;

            public static class Methods
            {
                public const string Close = "Close";
                public const string Show = "Show";
            }
            public static class Properties
            {
                public const string Handle = "Handle";
                public const string ShowInTaskbar = "ShowInTaskbar";
                public const string FormBorderStyle = "FormBorderStyle";
                public const string Opacity = "Opacity";
            }
        }

        public static object GetInstance(Game game)
        {
            if (!_initialized) {
                CreateFormAndHandle(game);
            }
            return FormInstance;
        }

        public static IntPtr GetHandle(Game game)
        {
            if (!_initialized) {
                CreateFormAndHandle(game);
            }
            return _handle;
        }

        private static void CreateFormAndHandle(Game game)
        {
            try
            {
                XnaToFnaHelper.Log("[ProxyForms] GameFormForWinForms: Creating game's Windows Form");
                var formType = GetFormType();
                FormInstance = Activator.CreateInstance(formType);
                // InitializeForm(formType, _formInstance); // No known mod needs this yet.
                var handleProp = formType.GetProperty(FormClass.Properties.Handle);
                _handle = (IntPtr)handleProp.GetValue(FormInstance);
                HookFormToGameExit(game);
            }
            catch (Exception ex)
            {
                // Fallback for Linux/macOS
                XnaToFnaHelper.Log("[ProxyForms] GameFormForWinForms: Error while creating game's Windows Form:");
                XnaToFnaHelper.Log(ex.ToString());
                _handle = IntPtr.Zero;
            }
            _initialized = true;
        }

        private static Type GetFormType()
        {
            var formsAssembly = GetWinFormsAssembly();
            var formType = formsAssembly.GetType(FormClass.NameWithNamespace);
            return formType;
        }

        private static Assembly GetWinFormsAssembly()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var winFormsAssembly = assemblies.FirstOrDefault(a => a.GetName().Name == WinFormsAssemblyName);

            if (winFormsAssembly != null) {
                return winFormsAssembly;
            }
            // Won't work without the FULL strong name:
            winFormsAssembly = Assembly.Load(WinFormsAssemblyStrongName);
            return winFormsAssembly;
        }

        /// <summary>
        /// Sets the form to be hidden and calls "Form.Show()" to initialize it.
        /// In some rare cases, a mod could depend on form events 
        /// that only trigger if the form has been shown.
        /// </summary>
        private static void InitializeForm(Type formType, object windowsFormObj)
        {
            formType.GetProperty(FormClass.Properties.ShowInTaskbar)?.SetValue(windowsFormObj, false);
            formType.GetProperty(FormClass.Properties.FormBorderStyle)?.SetValue(windowsFormObj, 0); // None
            formType.GetProperty(FormClass.Properties.Opacity)?.SetValue(windowsFormObj, 0.0);
            formType.GetMethod(FormClass.Methods.Show, Type.EmptyTypes)?.Invoke(windowsFormObj, null);
        }

        public static void HookFormToGameExit(Game game)
        {
            HookFormToGameExit(FormInstance, game);
        }

        private static void HookFormToGameExit(object form, Game game)
        {
            if (form == null || game == null) {
                return;
            }
            game.Exiting += (s, e) => {
                try {
                    var close = form.GetType().GetMethod(FormClass.Methods.Close);
                    close?.Invoke(form, null);
                    XnaToFnaHelper.Log("[ProxyForms] GameFormForWinForms: Form closed");
                }
                catch(Exception ex) {
                    XnaToFnaHelper.Log("[ProxyForms] GameFormForWinForms: Error trying to run Form's 'Close()' method:");
                    XnaToFnaHelper.Log(ex.ToString());
                }
            };
        }
    }
}
