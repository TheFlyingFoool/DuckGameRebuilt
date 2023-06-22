using DuckGame.ConsoleInterface;
using DuckGame.MMConfig;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace DuckGame
{
    [AttributeUsage(AttributeTargets.Method)]
    public partial class OnKeybindAttribute : Attribute
    {
        public Type ConfigClass { get; }
        public string FieldName { get; }

        private string _keybindString;
        public static List<OnKeybindAttribute> All;
        #if DEBUG
        public MethodInfo Method;
        #else
        public Action Method;
        #endif

        public string KeybindString
        {
            get
            {
                if (_keybindString is null)
                {
                    FieldInfo wantedCategoryFieldInfo = typeof(MMKeymapConfig).GetFields()
                        .First(x => x.FieldType.FullName == ConfigClass.FullName);
                    _keybindString = wantedCategoryFieldInfo.FieldType.GetField(FieldName)
                        .GetValue(wantedCategoryFieldInfo.GetValue(MallardManager.Config.Keymap)).ToString();
                }

                return _keybindString;
            }
        }

        public OnKeybindAttribute(Type configClass, string fieldName)
        {
            ConfigClass = configClass;
            FieldName = fieldName;
        }
        
        public OnKeybindAttribute(string keybind)
        {
            _keybindString = keybind;
        }
        
        public OnKeybindAttribute(Keys key)
        {
            _keybindString = key.ToString().ToLower();
        }

        [DrawingContext(CustomID = "keybindUpdate")]
        public static void Update()
        {
            UpdateInputActivationReception(); // is pressed/released checks and additions to register
            UpdateKeybindActivationActions(); // check if keybinds are activated and run methods
            
            // testing

            // foreach (string keybind in new[]
            //          {
            //              "numpad1,numpad2,numpad3,numpad4,numpad5",
            //              "numpad7,numpad9",
            //              "_ctrl+z",
            //              "MENUUP+MENURIGHT"
            //          })
            // {
            //     AcrossTime.Do(keybind, 500.Milliseconds(), _ =>
            //     {
            //         if (!IsActive(keybind))
            //             return;
            //
            //         RebuiltMono.Draw(keybind, new Vec2(Layer.HUD.width / 2, Layer.HUD.height / 2),
            //             Color.Red,
            //             2f, 3, ContentAlignment.MiddleCenter);
            //     });
            // }
        }

        private static void UpdateInputActivationReception()
        {
            KeybindReceptionHandler.UpdateKeyboardInputs();
            KeybindReceptionHandler.UpdateDuckInputs();
        }

        private static void UpdateKeybindActivationActions()
        {
            foreach (OnKeybindAttribute keybindAttribute in All)
            {
                if (!KeybindReceptionHandler.HasListUpdated)
                    continue;

                KeybindReceptionHandler.HasListUpdated = false;
                
                if (!keybindAttribute.IsActive())
                    continue;

                #if DEBUG
                keybindAttribute.Method.Invoke(null, null);
                #else
                keybindAttribute.Method();
                #endif
            }
        }

        public static void OnResults(Dictionary<Type, List<(MemberInfo MemberInfo, Attribute Attribute)>> all)
        {
            All = new List<OnKeybindAttribute>();
            var allAutoConfig = all[typeof(OnKeybindAttribute)];
            foreach ((MemberInfo memberInfo, Attribute vAttribute) in allAutoConfig)
            {
                OnKeybindAttribute attribute = (OnKeybindAttribute)vAttribute;
                #if DEBUG
                attribute.Method = (MethodInfo)memberInfo;
                #else
                attribute.Method = (Action)Delegate.CreateDelegate(typeof(Action), (MethodInfo)memberInfo);
                #endif
                All.Add(attribute);
            }
        }
    }
}