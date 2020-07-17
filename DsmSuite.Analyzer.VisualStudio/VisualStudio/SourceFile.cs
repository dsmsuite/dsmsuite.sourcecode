using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.VisualStudio.VisualStudio
{
    public class SourceFile
    {
        private readonly FileInfo _sourceFileInfo;
        private readonly HashSet<string> _includes;
        private readonly IncludeResolveStrategy _includeResolveStrategy;

        public SourceFile(string filename)
        {
            ProjectFolder = "";
            _sourceFileInfo = new FileInfo(filename);
            _includes = new HashSet<string>();
        }

        public SourceFile(FileInfo sourceFileInfo, string projectProjectFolder, IncludeResolveStrategy includeResolveStrategy)
        {
            ProjectFolder = projectProjectFolder;
            _sourceFileInfo = sourceFileInfo;
            _includes = new HashSet<string>();
            _includeResolveStrategy = includeResolveStrategy;
        }

        public FileInfo SourceFileInfo => _sourceFileInfo;

        public string Name => _sourceFileInfo.FullName;

        public string FileType => (_sourceFileInfo.Extension.Length > 0) ? _sourceFileInfo.Extension.Substring(1) : "";

        public string ProjectFolder { get; }

        public ICollection<string> Includes => _includes;
      
        public void Analyze()
        {
            _includes.Clear();

            if (_sourceFileInfo.Exists && (_includeResolveStrategy != null))
            {
                using (FileStream stream = _sourceFileInfo.OpenRead())
                {
                    StreamReader sr = new StreamReader(stream);
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string relativeIncludeFilename = ExtractFileFromIncludeStatement(line);

                        if (relativeIncludeFilename != null)
                        {
                            string absoluteIncludeFilename = _includeResolveStrategy.Resolve(_sourceFileInfo.FullName, relativeIncludeFilename);
                            if (absoluteIncludeFilename != null)
                            {
                                _includes.Add(absoluteIncludeFilename);
                            }
                        }
                    }
                }
            }
        }

        private string ExtractFileFromIncludeStatement(string line)
        {
            string includedFilename = null;

            if (line.StartsWith("#include"))
            {
                char[] separators = { '\t', ' ', '\"', '>', '<' };
                string[] elements = line.Split(separators);

                if (elements.Length > 2 && elements[2].Length > 1)
                {
                    includedFilename = elements[2];
                }
            }

            return includedFilename;
        }
    }
}
