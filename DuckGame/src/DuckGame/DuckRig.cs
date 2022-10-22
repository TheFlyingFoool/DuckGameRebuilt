// Decompiled with JetBrains decompiler
// Type: DuckGame.DuckRig
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.IO;

namespace DuckGame
{
    public class DuckRig
    {
        private static List<Vec2> _hatPoints = new List<Vec2>();
        private static List<Vec2> _chestPoints = new List<Vec2>();

        public static void Initialize()
        {
            try
            {
                DuckRig._hatPoints.Clear();
                DuckRig._chestPoints.Clear();
                BinaryReader binaryReader = new BinaryReader(System.IO.File.OpenRead(Content.path + "rig_duckRig.rig"));
                int num = binaryReader.ReadInt32();
                for (int index = 0; index < num; ++index)
                {
                    DuckRig._hatPoints.Add(new Vec2()
                    {
                        x = binaryReader.ReadInt32(),
                        y = binaryReader.ReadInt32()
                    });
                    DuckRig._chestPoints.Add(new Vec2()
                    {
                        x = binaryReader.ReadInt32(),
                        y = binaryReader.ReadInt32()
                    });
                }
                binaryReader.Close();
                binaryReader.Dispose();
            }
            catch (Exception ex)
            {
                Program.LogLine(MonoMain.GetExceptionString(ex));
            }
        }

        public static Vec2 GetHatPoint(int frame) => DuckRig._hatPoints[frame];

        public static Vec2 GetChestPoint(int frame) => DuckRig._chestPoints[frame];
    }
}
