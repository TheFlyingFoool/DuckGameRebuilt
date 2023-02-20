using DuckGame;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace DGShared.AddedContent
{
    //Absolute Asset Loader, so fuck you relative path Exception
    public static class ALoader
    {
        static MethodInfo getContentReader = typeof(ContentManager).GetMethod("GetContentReaderFromXnb", BindingFlags.Instance | BindingFlags.NonPublic);
        //static MethodInfo readAsset = AccessTools.Method(typeof(ContentReader), "ReadAsset").MakeGenericMethod(typeof(Effect));
        static Dictionary<string, object> loadedAssets = (Dictionary<string, object>)typeof(ContentManager).GetField("loadedAssets", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(MonoMain.instance.Content);

        public static T GetAsset<T>(string path)
        {
            T asset;
            if (loadedAssets.TryGetValue(path, out var assetr))
            {
                return (T)assetr;
            }
            MethodInfo readAsset = typeof(ContentReader).GetMethod("ReadAsset", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], null).MakeGenericMethod(typeof(T));
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                using (Stream str = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using(BinaryReader stream = new BinaryReader(str))
                {
                    var cr = (ContentReader)getContentReader.Invoke(MonoMain.instance.Content, new object[] { path, str, stream, null });
                    asset = (T)readAsset.Invoke(cr, null);
                    loadedAssets.Add(path, asset);
                    cr.Dispose();
                    return asset;
                }
            }
            throw new System.Exception($"Asset doesnt exits on the path {path}");
        }
    }
}
