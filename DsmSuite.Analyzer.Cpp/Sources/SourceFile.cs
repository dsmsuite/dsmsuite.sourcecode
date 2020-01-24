using System.Collections.Generic;
using System.IO;
using DsmSuite.Analyzer.Cpp.IncludeResolve;

namespace DsmSuite.Analyzer.Cpp.Sources
{
    public class SourceFile
    {
        private readonly FileInfo _fileInfo;
        private readonly HashSet<string> _includes;
        private readonly HashSet<string> _unresolvedIncludes;

        public SourceFile(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
            _includes = new HashSet<string>();
            _unresolvedIncludes = new HashSet<string>();
        }

        public FileInfo FileInfo => _fileInfo;

        public string Name => _fileInfo.FullName;

        public string Extension => (_fileInfo.Extension.Length > 0) ? _fileInfo.Extension.Substring(1) : "";

        public bool IsSourceCodeFile => ((Extension == "c") || (Extension == "cpp") || (Extension == "h") || (Extension == "hpp"));

        public ICollection<string> Includes => _includes;

        public ICollection<string> UnresolvedIncludes => _unresolvedIncludes;

        public void Analyze(IIncludeResolveStrategy includeResolveStrategy)
        {
            _includes.Clear();

            if (_fileInfo.Exists)
            {
                using (FileStream stream = _fileInfo.OpenRead())
                {
                    StreamReader sr = new StreamReader(stream);
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string relativeIncludeFilename = ExtractFileFromIncludeStatement(line);
                        ResolveIncludeFile(relativeIncludeFilename, includeResolveStrategy);
                    }
                }
            }
        }

        internal void ResolveIncludeFile(string relativeIncludeFilename, IIncludeResolveStrategy includeResolveStrategy)
        {
            if (relativeIncludeFilename != null)
            {
                if (relativeIncludeFilename == "map")
                {

                }
                IList<string> resolvedIncludes = includeResolveStrategy.Resolve(_fileInfo.FullName, relativeIncludeFilename);
                foreach (string resolvedInclude in resolvedIncludes)
                {
                    _includes.Add(resolvedInclude);
                }

                if (resolvedIncludes.Count == 0)
                {
                    _unresolvedIncludes.Add(relativeIncludeFilename);
                }
            }
        }
        
        internal string ExtractFileFromIncludeStatement(string line)
        {
            string includedFilename = null;

            if (line.StartsWith("#include"))
            {
                char[] separators = { '\t', ' ', '\"', '>', '<' };
                string[] elements = line.Split(separators);

                if (elements.Length > 1 && elements[2].Length > 1)
                {
                    string normalizedFilename = elements[2];

                    char[] separators2 = {'\\', '/'};
                    string[] elements2 = normalizedFilename.Split(separators2);

                    includedFilename = elements2.Length > 1 ? elements2[elements2.Length - 1] : normalizedFilename;
                }
            }

            return includedFilename;
        }
    }
}
