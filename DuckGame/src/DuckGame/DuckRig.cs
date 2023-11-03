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
                _hatPoints.Clear();
                _chestPoints.Clear();
                BinaryReader binaryReader = new BinaryReader(File.OpenRead(Content.path + "rig_duckRig.rig"));
                int num = binaryReader.ReadInt32();
                for (int index = 0; index < num; ++index)
                {
                    _hatPoints.Add(new Vec2()
                    {
                        x = binaryReader.ReadInt32(),
                        y = binaryReader.ReadInt32()
                    });
                    _chestPoints.Add(new Vec2()
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

        public static Vec2 GetHatPoint(int frame) => _hatPoints[frame];

        public static Vec2 GetChestPoint(int frame) => _chestPoints[frame];
    }
}
