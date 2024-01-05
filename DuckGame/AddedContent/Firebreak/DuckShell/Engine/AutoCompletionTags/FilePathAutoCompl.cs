using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DuckGame.ConsoleEngine
{
    public class FilePathAutoComplAttribute : AutoCompl
    {
        private string _parentDirectory;
        private readonly SystemEntryType _type;
        private readonly SearchOption _searchOption;
        private readonly Return _returnValue;

        public FilePathAutoComplAttribute(string parentDirectory, SystemEntryType type = default, SearchOption searchOption = default, Return returnValue = default)
        {
            _parentDirectory = parentDirectory;
            _type = type;
            _searchOption = searchOption;
            _returnValue = returnValue;
        }

        public override IList<string> Get(string word)
        {
            // convert |XYZ| to DuckFile.XYZ
            if (_parentDirectory.Contains('|'))
            {
                try
                {
                    _parentDirectory = Regex.Replace(_parentDirectory, @"\|([^\|]+?)\|",
                        m => (string)typeof(DuckFile).GetProperties(BindingFlags.Public | BindingFlags.Static)
                            .First(x => x.PropertyType == typeof(string) && x.Name == m.Groups[1].Value).GetValue(null));
                }
                catch (Exception e)
                {
                    throw new Exception("hi dev you fucked up your path formatting somewhere", e);
                }
            }
            
            string path = $"{_parentDirectory}/{word}";
            
            string[] entries = _type switch
            {
                SystemEntryType.Both => Directory.GetFileSystemEntries(path, "*", _searchOption),
                SystemEntryType.File => Directory.GetFiles(path, "*", _searchOption),
                SystemEntryType.Directory => Directory.GetDirectories(path, "*", _searchOption),
                _ => throw new InvalidOperationException()
            };

            return (_returnValue switch
            {
                Return.EntryName => entries.Select(FixPath_EntryName),
                Return.EntryNameNoExtension => entries.Select(FixPath_EntryNameNoExtension),
                Return.FullPath => entries.Select(FixPath_FullPath),
                Return.PathRelativeToParent => entries.Select(FixPath_PathRelativeToParent),
                Return.PathRelativeToParentNoExtension => entries.Select(FixPath_PathRelativeToParentNoExtension),
                _ => throw new InvalidOperationException()
            }).ToArray();
        }

        private string FixPath_EntryName(string fullPath)
        {
            return Path.GetFileName(fullPath);
        }

        private string FixPath_EntryNameNoExtension(string fullPath)
        {
            return Path.GetFileNameWithoutExtension(fullPath);
        }

        private string FixPath_FullPath(string fullPath)
        {
            return fullPath;
        }

        private string FixPath_PathRelativeToParent(string fullPath)
        {
            throw new NotImplementedException();
        }

        private string FixPath_PathRelativeToParentNoExtension(string fullPath)
        {
            throw new NotImplementedException();
        }

        public enum Return
        {
            EntryName,
            EntryNameNoExtension,
            FullPath,
            PathRelativeToParent,
            PathRelativeToParentNoExtension,
        }
    }

    public enum SystemEntryType
    {
        Both,
        File,
        Directory
    }
}