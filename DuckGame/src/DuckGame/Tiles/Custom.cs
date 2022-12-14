// Decompiled with JetBrains decompiler
// Type: DuckGame.Custom
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace DuckGame
{
    public class Custom
    {
        private static Dictionary<CustomType, Dictionary<string, CustomTileData>> _customTilesetDataInternal = new Dictionary<CustomType, Dictionary<string, CustomTileData>>()
    {
      {
        CustomType.Block,
        new Dictionary<string, CustomTileData>()
      },
      {
        CustomType.Platform,
        new Dictionary<string, CustomTileData>()
      },
      {
        CustomType.Background,
        new Dictionary<string, CustomTileData>()
      },
      {
        CustomType.Parallax,
        new Dictionary<string, CustomTileData>()
      }
    };
        private static Dictionary<CustomType, Dictionary<string, CustomTileData>> _customTilesetDataInternalPreview = new Dictionary<CustomType, Dictionary<string, CustomTileData>>()
    {
      {
        CustomType.Block,
        new Dictionary<string, CustomTileData>()
      },
      {
        CustomType.Platform,
        new Dictionary<string, CustomTileData>()
      },
      {
        CustomType.Background,
        new Dictionary<string, CustomTileData>()
      },
      {
        CustomType.Parallax,
        new Dictionary<string, CustomTileData>()
      }
    };
        private static Dictionary<CustomType, string[]> _dataInternal = new Dictionary<CustomType, string[]>()
    {
      {
        CustomType.Block,
        new string[3]{ "", "", "" }
      },
      {
        CustomType.Platform,
        new string[3]{ "", "", "" }
      },
      {
        CustomType.Background,
        new string[3]{ "", "", "" }
      },
      {
        CustomType.Parallax,
        new string[1]{ "" }
      }
    };
        private static Dictionary<CustomType, string[]> _dataInternalPreview = new Dictionary<CustomType, string[]>()
    {
      {
        CustomType.Block,
        new string[3]{ "", "", "" }
      },
      {
        CustomType.Platform,
        new string[3]{ "", "", "" }
      },
      {
        CustomType.Background,
        new string[3]{ "", "", "" }
      },
      {
        CustomType.Parallax,
        new string[1]{ "" }
      }
    };
        private static Dictionary<CustomType, CustomTileDataChunk[]> _previewDataInternal = new Dictionary<CustomType, CustomTileDataChunk[]>()
    {
      {
        CustomType.Block,
        new CustomTileDataChunk[3]
      },
      {
        CustomType.Platform,
        new CustomTileDataChunk[3]
      },
      {
        CustomType.Background,
        new CustomTileDataChunk[3]
      },
      {
        CustomType.Parallax,
        new CustomTileDataChunk[1]
      }
    };
        private static Dictionary<CustomType, CustomTileDataChunk[]> _previewDataInternalPreview = new Dictionary<CustomType, CustomTileDataChunk[]>()
    {
      {
        CustomType.Block,
        new CustomTileDataChunk[3]
      },
      {
        CustomType.Platform,
        new CustomTileDataChunk[3]
      },
      {
        CustomType.Background,
        new CustomTileDataChunk[3]
      },
      {
        CustomType.Parallax,
        new CustomTileDataChunk[1]
      }
    };

        public static void Clear(CustomType pType, string pPath)
        {
        }

        private static Dictionary<CustomType, Dictionary<string, CustomTileData>> _customTilesetData => !Content.renderingPreview ? _customTilesetDataInternal : _customTilesetDataInternalPreview;

        public static Dictionary<CustomType, string[]> data => !Content.renderingPreview ? _dataInternal : _dataInternalPreview;

        public static Dictionary<CustomType, CustomTileDataChunk[]> previewData => !Content.renderingPreview ? _previewDataInternal : _previewDataInternalPreview;

        public static void ClearCustomData()
        {
            foreach (KeyValuePair<CustomType, string[]> keyValuePair in data)
            {
                for (int index = 0; index < keyValuePair.Value.Length; ++index)
                {
                    keyValuePair.Value[index] = "";
                    previewData[keyValuePair.Key][index] = null;
                }
            }
        }

        public static string ApplyCustomData(CustomTileData tData, int index, CustomType type)
        {
            string key = tData.path + "@" + tData.checksum.ToString() + ".png";
            if (tData.texture != null)
            {
                string str = DuckFile.GetCustomDownloadDirectory(type) + key;
                try
                {
                    if (!DuckFile.FileExists(str))
                    {
                        DuckFile.CreatePath(str);
                        FileStream fileStream = File.Create(str);
                        tData.texture.SaveAsPng(fileStream, tData.texture.Width, tData.texture.Height);
                        fileStream.Close();
                    }
                }
                catch (Exception ex)
                {
                    DevConsole.Log(DCSection.General, "Access error saving " + str + ":");
                    DevConsole.Log(DCSection.General, ex.Message);
                }
            }
            else if (tData.path == null)
                return "";
            _customTilesetData[type][key] = tData;
            data[type][index] = key;
            return key;
        }

        public static CustomTileData LoadTileData(string path, CustomType type)
        {
            CustomTileData customTileData = new CustomTileData();
            if (path == "" || path == null)
                return customTileData;
            Texture2D texture2D = ContentPack.LoadTexture2D(DuckFile.GetCustomDirectory(type) + path + ".png");
            if (texture2D != null)
            {
                try
                {
                    Color[] data = new Color[texture2D.Width * texture2D.Height];
                    texture2D.GetData(data);
                    for (int index1 = 0; index1 < 5; ++index1)
                    {
                        int num1 = 112;
                        int num2 = 64 + index1 * 16;
                        if (index1 == 1)
                        {
                            int num3 = num2;
                            num2 = num1;
                            num1 = num3;
                        }
                        if (index1 == 3)
                        {
                            num1 = 96;
                            num2 = 112;
                        }
                        else if (index1 == 4)
                        {
                            num1 = 112;
                            num2 = 112;
                        }
                        int num4 = -1;
                        int num5 = 0;
                        for (int index2 = 0; index2 < 16; ++index2)
                        {
                            bool flag = false;
                            for (int index3 = 0; index3 < 16; ++index3)
                            {
                                int index4 = index1 != 1 ? num1 + index3 + (num2 + index2) * texture2D.Width : num2 + index2 + (num1 + index3) * texture2D.Width;
                                if (index1 == 3 || index1 == 4 ? data[index4].a > 0 : data[index4].r == 0 && data[index4].g == byte.MaxValue && data[index4].b == 0 && data[index4].a == byte.MaxValue)
                                {
                                    if (num4 == -1)
                                        num4 = index3;
                                }
                                else if (num4 != -1)
                                {
                                    num5 = index3 - num4;
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                                break;
                        }
                        switch (index1)
                        {
                            case 0:
                                customTileData.verticalWidth = num5;
                                break;
                            case 1:
                                customTileData.horizontalHeight = num5;
                                break;
                            case 2:
                                customTileData.verticalWidthThick = num5;
                                break;
                            case 3:
                                customTileData.leftNubber = num5 != 0;
                                break;
                            case 4:
                                customTileData.rightNubber = num5 != 0;
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    texture2D = null;
                }
                customTileData.texture = texture2D;
                customTileData.path = path;
                _customTilesetData[type][path] = customTileData;
            }
            return customTileData;
        }

        public static CustomTileData GetData(int index, CustomType type)
        {
            CustomTileData data;
            if (!_customTilesetData[type].TryGetValue(Custom.data[type][index], out data))
                data = LoadTileData(Custom.data[type][index], type);
            return data;
        }
    }
}
