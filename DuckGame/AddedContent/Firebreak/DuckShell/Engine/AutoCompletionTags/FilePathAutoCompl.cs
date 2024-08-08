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
            
            string path = $"{_parentDirectory}";

            Func<string, string> pathFixerMethod = _returnValue switch
            {
                Return.EntryName                       => FixPath_EntryName,
                Return.EntryNameNoExtension            => FixPath_EntryNameNoExtension,
                Return.FullPath                        => FixPath_FullPath,
                Return.PathRelativeToParent            => FixPath_PathRelativeToParent,
                Return.PathRelativeToParentNoExtension => FixPath_PathRelativeToParentNoExtension,
                _                                      => throw new NotImplementedException()
            };
            
            IEnumerable<string> entries = (_type switch
            {
                SystemEntryType.Both => Directory.GetFileSystemEntries(path, "*", _searchOption),
                SystemEntryType.File => Directory.GetFiles(path, "*", _searchOption),
                SystemEntryType.Directory => Directory.GetDirectories(path, "*", _searchOption),
                _ => throw new InvalidOperationException()
            }).Where(x =>
            {
                string fixedPath = pathFixerMethod(x);
                string fileName = Path.GetFileName(x);
                
                return fixedPath.StartsWith(word)
                       || fileName.StartsWith(word)
                       || fixedPath.Contains(word);
            });

            return entries
                .Select(pathFixerMethod)
                .Select(x => x.Replace('\\', '/'))
                .ToArray();
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
            return fullPath.Substring(_parentDirectory.Length + 1);
        }

        private string FixPath_PathRelativeToParentNoExtension(string fullPath)
        {
            string relativePath = FixPath_PathRelativeToParent(fullPath);
            
            int length;
            return (length = relativePath.LastIndexOf('.')) == -1
                ? relativePath
                : relativePath.Substring(0, length);
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