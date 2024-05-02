using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace XnaToFna
{
    public static class BinaryFormatterHelper
    {
        public static readonly Assembly FNA = typeof(Game).Assembly;

        public static BinaryFormatter Create() => new BinaryFormatter()
        {
            Binder = new XnaToFnaSerializationBinderWrapper(null)
        };

        public static BinaryFormatter Create(
          ISurrogateSelector selector,
          StreamingContext context)
        {
            return new BinaryFormatter(selector, context)
            {
                Binder = new XnaToFnaSerializationBinderWrapper(null)
            };
        }

        public static SerializationBinder get_Binder(this BinaryFormatter self) => (self.Binder is XnaToFnaSerializationBinderWrapper binder ? binder.Inner : null) ?? self.Binder;

        public static void set_Binder(this BinaryFormatter self, SerializationBinder binder) => self.Binder = new XnaToFnaSerializationBinderWrapper(binder);

        public class XnaToFnaSerializationBinderWrapper : SerializationBinder
        {
            public readonly SerializationBinder Inner;

            public XnaToFnaSerializationBinderWrapper(SerializationBinder inner) => Inner = inner;

            public override Type BindToType(string assemblyName, string typeName)
            {
                if (!(assemblyName != "Microsoft.Xna.Framework") || assemblyName.StartsWith("Microsoft.Xna.Framework,") || assemblyName.StartsWith("Microsoft.Xna.Framework."))
                    return FNA.GetType(typeName);
                return Inner?.BindToType(assemblyName, typeName);
            }

            public override void BindToName(
              Type serializedType,
              out string assemblyName,
              out string typeName)
            {
                if (Inner != null)
                    Inner.BindToName(serializedType, out assemblyName, out typeName);
                else
                    base.BindToName(serializedType, out assemblyName, out typeName);
            }
        }
    }
}
