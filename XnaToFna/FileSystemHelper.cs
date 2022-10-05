// Decompiled with JetBrains decompiler
// Type: XnaToFna.FileSystemHelper
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XnaToFna
{
  public static class FileSystemHelper
  {
    public static readonly bool MONO_IOMAP_ALL = Environment.GetEnvironmentVariable("MONO_IOMAP") == "all";
    public static readonly char[] DirectorySeparatorChars = new char[2]
    {
      '/',
      '\\'
    };
    private static Dictionary<string, string[]> _CachedDirectories = new Dictionary<string, string[]>();
    private static Dictionary<string, string[]> _CachedTargets = new Dictionary<string, string[]>();
    private static Dictionary<char, Dictionary<string, string>> _CachedChanges = new Dictionary<char, Dictionary<string, string>>();

    public static string[] GetDirectories(string path)
    {
      string[] directories1;
      if (FileSystemHelper._CachedDirectories.TryGetValue(path, out directories1))
        return directories1;
      string[] directories2;
      try
      {
        directories2 = Directory.GetDirectories(path);
      }
      catch
      {
        directories2 = null;
      }
      FileSystemHelper._CachedDirectories[path] = directories2;
      return directories2;
    }

    public static string[] GetTargets(string path)
    {
      string[] targets1;
      if (FileSystemHelper._CachedTargets.TryGetValue(path, out targets1))
        return targets1;
      string[] targets2;
      try
      {
        targets2 = Directory.GetFileSystemEntries(path);
      }
      catch
      {
        targets2 = null;
      }
      FileSystemHelper._CachedTargets[path] = targets2;
      return targets2;
    }

    public static string GetDirectory(string path, string next) => FileSystemHelper.GetNext(FileSystemHelper.GetDirectories(path), next);

    public static string GetTarget(string path, string next) => FileSystemHelper.GetNext(FileSystemHelper.GetTargets(path), next);

    public static string GetNext(string[] possible, string next)
    {
      if (possible == null)
        return null;
      for (int index = 0; index < possible.Length; ++index)
      {
        string fileName = Path.GetFileName(possible[index]);
        if (string.Equals(next, fileName, StringComparison.InvariantCultureIgnoreCase))
          return fileName;
      }
      return null;
    }

    public static string FixPath(string path) => FileSystemHelper.ChangePath(path, Path.DirectorySeparatorChar);

    public static string BreakPath(string path) => FileSystemHelper.ChangePath(path, '\\');

    public static string ChangePath(string path, char separator)
    {
      if (!FileSystemHelper.MONO_IOMAP_ALL)
      {
        string str = path;
        if (Directory.Exists(path) || File.Exists(path))
          return str;
        string path1 = path.Replace('/', separator).Replace('\\', separator);
        if (Directory.Exists(path1) || File.Exists(path1))
          return path1;
      }
      Dictionary<string, string> dictionary;
      if (!FileSystemHelper._CachedChanges.TryGetValue(separator, out dictionary))
        FileSystemHelper._CachedChanges[separator] = dictionary = new Dictionary<string, string>();
      string str1;
      if (dictionary.TryGetValue(path, out str1))
        return str1;
      string[] strArray = path.Split(FileSystemHelper.DirectorySeparatorChars);
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = false;
      if (Path.IsPathRooted(path))
      {
        if (flag = stringBuilder.Length == 0)
          stringBuilder.Append(separator);
        else
          stringBuilder.Append(strArray[0]);
      }
      for (int index = 1; index < strArray.Length; ++index)
      {
        string str2 = (index >= strArray.Length - 1 ? FileSystemHelper.GetTarget(stringBuilder.ToString(), strArray[index]) : FileSystemHelper.GetDirectory(stringBuilder.ToString(), strArray[index])) ?? strArray[index];
        if (index != 1 || !flag)
          stringBuilder.Append(separator);
        stringBuilder.Append(str2);
      }
      return dictionary[path] = stringBuilder.ToString();
    }
  }
}
