// Decompiled with JetBrains decompiler
// Type: DuckGame.DGPath
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DuckGame
{
    public struct DGPath
    {
        public const char Slash = '/';
        private object _specialData;
        private string _path;
        private Dictionary<string, DGPath[]> _filesAndDirectories;
        private DGPath[] _directories;
        private Dictionary<string, DGPath[]> _files;
        private bool _file;
        private bool _rooted;
        private static StringBuilder kBuilder = new StringBuilder();

        public object specialData => _specialData;

        public string path => _path;

        public bool exists
        {
            get
            {
                if (_specialData != null)
                    return false;
                return _file ? System.IO.File.Exists(_path) : Directory.Exists(_path);
            }
        }

        /// <summary>Gets the current directory represented by the path</summary>
        public DGPath directory
        {
            get
            {
                for (int index = _path.Length - 1; index >= 0; --index)
                {
                    if (_path[index] == '/')
                        return CopyNewPath(_path.Substring(0, index + 1));
                }
                return (DGPath)_path;
            }
        }

        /// <summary>
        /// Returns directoryName if this is a directory, otherwise returns fileName.
        /// </summary>
        public string name
        {
            get
            {
                if (_specialData != null)
                    return _path;
                return _file ? fileName : directoryName;
            }
        }

        public string directoryName
        {
            get
            {
                if (_specialData != null)
                    return _path;
                bool flag = false;
                int length = _path.Length - 1;
                for (int index = _path.Length - 1; index >= 0; --index)
                {
                    if (_path[index] == '/')
                    {
                        if (flag)
                            return _path.Substring(index + 1, length - index - 1);
                        flag = true;
                        length = index;
                    }
                }
                return !flag ? "" : _path.Substring(0, length);
            }
        }

        public string fileName
        {
            get
            {
                if (_specialData != null)
                    return _path;
                if (!_file)
                    return "";
                for (int index = _path.Length - 1; index >= 0; --index)
                {
                    if (_path[index] == '/')
                        return _path.Substring(index + 1, _path.Length - index - 1);
                }
                return _path;
            }
        }

        public DGPath[] GetFilesAndDirectories(params string[] pExtensions)
        {
            if (_filesAndDirectories == null)
                _filesAndDirectories = new Dictionary<string, DGPath[]>();
            string key = "";
            foreach (string pExtension in pExtensions)
                key += pExtension;
            DGPath[] filesAndDirectories;
            if (!_filesAndDirectories.TryGetValue(key, out filesAndDirectories))
            {
                List<DGPath> dgPathList = new List<DGPath>();
                dgPathList.AddRange(GetDirectories());
                dgPathList.AddRange(GetFiles(pExtensions));
                filesAndDirectories = dgPathList.ToArray();
            }
            if (filesAndDirectories == null)
                filesAndDirectories = new DGPath[0];
            _filesAndDirectories[key] = filesAndDirectories;
            return filesAndDirectories;
        }

        public DGPath[] GetDirectories()
        {
            if (_directories == null)
            {
                if (!isDirectory)
                    throw new DGPath.DGPathException("DGPath.GetDirectories() does not work on file paths, only on directory paths.");
                if (!exists)
                {
                    _directories = new DGPath[0];
                }
                else
                {
                    List<DGPath> dgPathList = new List<DGPath>();
                    foreach (string directory in Directory.GetDirectories(_path))
                        dgPathList.Add((DGPath)directory);
                    _directories = dgPathList.ToArray();
                }
            }
            return _directories;
        }

        public DGPath[] GetFiles(params string[] pExtensions)
        {
            if (_files == null)
                _files = new Dictionary<string, DGPath[]>();
            string key = "";
            foreach (string pExtension in pExtensions)
                key += pExtension;
            DGPath[] files;
            if (!_files.TryGetValue(key, out files))
            {
                if (!isDirectory)
                    throw new DGPath.DGPathException("DGPath.GetFiles() does not work on file paths, only on directory paths.");
                if (!exists)
                    files = new DGPath[0];
                List<DGPath> dgPathList = new List<DGPath>();
                foreach (string file in Directory.GetFiles(_path))
                {
                    if (pExtensions.Length != 0)
                    {
                        foreach (string pExtension in pExtensions)
                        {
                            if (file.EndsWith(pExtension, StringComparison.InvariantCultureIgnoreCase))
                            {
                                dgPathList.Add((DGPath)file);
                                break;
                            }
                        }
                    }
                    else
                        dgPathList.Add((DGPath)file);
                }
                files = dgPathList.ToArray();
                _files[key] = files;
            }
            return files;
        }

        public bool isDirectory => !_file;

        public bool isFullPath => !_file;

        internal void CheckFileValidity(bool pMustBeFile = true, bool pWriting = false)
        {
            if (pMustBeFile && !_file)
                throw new DGPath.DGPathException("DGPath.ReadText(" + _path + ") failed: path is a directory, not a file.");
            if (pWriting)
                Directory.CreateDirectory((string)directory);
            else if (!exists)
                throw new DGPath.DGPathException("DGPath.ReadText(" + _path + ") failed: file does not exist.");
        }

        public DGPath(string pPath)
        {
            bool flag = false;
            _file = false;
            _rooted = false;
            _directories = null;
            _files = null;
            _filesAndDirectories = null;
            _specialData = null;
            int index = 0;
            DGPath.kBuilder.Clear();
            if (pPath.Length > 1 && pPath[1] == ':')
            {
                _rooted = true;
                DGPath.kBuilder.Append(char.ToUpper(pPath[0]));
                ++index;
            }
            for (; index < pPath.Length; ++index)
            {
                char ch = pPath[index];
                switch (ch)
                {
                    case '.':
                        _file = true;
                        goto default;
                    case '/':
                    case '\\':
                        if (!flag)
                        {
                            DGPath.kBuilder.Append('/');
                            flag = true;
                        }
                        _file = false;
                        break;
                    default:
                        DGPath.kBuilder.Append(ch);
                        flag = false;
                        break;
                }
            }
            _path = DGPath.kBuilder.ToString();
            if (_file || _path[_path.Length - 1] == '/')
                return;
            _path += "/";
        }

        public static DGPath Special(string pName, object pSpecialData) => new DGPath()
        {
            _path = pName,
            _specialData = pSpecialData
        };

        public DGPath Up()
        {
            bool flag = false;
            for (int index = _path.Length - 1; index >= 0; --index)
            {
                if (_path[index] == '/')
                {
                    if (flag)
                        return CopyNewPath(_path.Substring(0, index + 1));
                    flag = true;
                }
            }
            return this;
        }

        public DGPath Down(string pFolder) => pFolder == null ? this : this + (DGPath)pFolder;

        public DGPath Move(string pFile) => pFile == null ? this : this + (DGPath)pFile;

        public static implicit operator string(DGPath pPath) => pPath._path;

        public static implicit operator DGPath(string pPath) => new DGPath(pPath);

        public static bool operator ==(DGPath value1, DGPath value2) => value1._path == value2._path;

        public static bool operator !=(DGPath value1, DGPath value2) => value1._path != value2._path;

        public static DGPath operator +(DGPath value1, DGPath value2)
        {
            if (value2._rooted)
                throw new DGPath.DGPathException("(DGPath1 + DGPath2) failed- DGPath2 must be a subfolder and not a fully rooted path (C:/ type paths are not allowed)");
            if (value1._file)
                throw new DGPath.DGPathException("(DGPath1 + DGPath2) failed- DGPath1 must NOT be a file!");
            return new DGPath()
            {
                _path = value1._path + value2._path,
                _rooted = value1._rooted,
                _file = value2._file
            };
        }

        public static bool operator <(DGPath value1, DGPath value2) => value2._path.Contains(value1._path);

        public static bool operator >(DGPath value1, DGPath value2) => value1._path.Contains(value2._path);

        /// <summary>Removes the root path from the current path</summary>
        /// <param name="pRoot">The current root to remove.</param>
        /// <returns>The unrooted path.</returns>
        public DGPath Unroot(DGPath pRoot) => _path.Contains(pRoot._path) ? CopyNewPath(_path.Replace(pRoot._path, "")) : this;

        public static DGPath operator -(DGPath value1, DGPath value2) => value1._path.Length < value2._path.Length ? value2.Unroot(value1) : value1.Unroot(value2);

        internal DGPath CopyNewPath(string pPath) => new DGPath()
        {
            _path = pPath,
            _rooted = _rooted
        };

        public override string ToString() => _path;

        public void Delete()
        {
            CheckFileValidity(false);
            if (_file)
                System.IO.File.Delete(_path);
            else
                Directory.Delete(_path);
        }

        public string ReadText()
        {
            CheckFileValidity();
            return System.IO.File.ReadAllText(_path);
        }

        public void WriteText(string pText)
        {
            CheckFileValidity(pWriting: true);
            System.IO.File.WriteAllText(_path, pText);
        }

        public string[] ReadLines()
        {
            CheckFileValidity();
            return System.IO.File.ReadAllLines(_path);
        }

        public void WriteLines(string[] pLines)
        {
            CheckFileValidity(pWriting: true);
            System.IO.File.WriteAllLines(_path, pLines);
        }

        public byte[] ReadBytes()
        {
            CheckFileValidity();
            return System.IO.File.ReadAllBytes(_path);
        }

        public void WriteBytes(byte[] pBytes)
        {
            CheckFileValidity(pWriting: true);
            System.IO.File.WriteAllBytes(_path, pBytes);
        }

        public void CreatePath() => CheckFileValidity(pWriting: true);

        public class DGPathException : Exception
        {
            public DGPathException(string pMessage)
              : base(pMessage)
            {
            }
        }
    }
}
