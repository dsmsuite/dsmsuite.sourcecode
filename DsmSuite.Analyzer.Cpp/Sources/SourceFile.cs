using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DsmSuite.Analyzer.Cpp.IncludeResolve;

namespace DsmSuite.Analyzer.Cpp.Sources
{
    public class SourceFile
    {
        private readonly FileInfo _fileInfo;
        private readonly HashSet<string> _includes;
        private readonly HashSet<string> _unresolvedIncludes;
        private readonly HashSet<string> _ambiguousIncludes;

        public SourceFile(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
            _includes = new HashSet<string>();
            _unresolvedIncludes = new HashSet<string>();
            _ambiguousIncludes = new HashSet<string>();
        }

        public FileInfo FileInfo => _fileInfo;

        public string Name => _fileInfo.FullName;

        public string Extension => (_fileInfo.Extension.Length > 0) ? _fileInfo.Extension.Substring(1) : "";

        public bool IsSourceCodeFile => ((Extension == "c") || (Extension == "cpp") || (Extension == "h") || (Extension == "hpp"));

        public ICollection<string> Includes => _includes;

        public ICollection<string> UnresolvedIncludes => _unresolvedIncludes;
        public ICollection<string> AmbiguousIncludes => _ambiguousIncludes;

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
                IList<IncludeCandidate> candidates = includeResolveStrategy.GetCandidates(relativeIncludeFilename);
                if (candidates.Count > 1)
                {
                    AmbiguousIncludes.Add(relativeIncludeFilename);
                }
                IList<string> resolvedIncludes = includeResolveStrategy.Resolve(_fileInfo.FullName, relativeIncludeFilename, candidates);
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

            Regex regex = new Regex("#[ ]{0,}include");
            Match match = regex.Match(line);
            if (match.Success)
            {
                string normalizedFilename = line.Substring(match.Value.Length + 1).Replace("\"", "").Replace(">", "").Replace("<", "");

                char[] separators = { '\\', '/' };
                string[] elements = normalizedFilename.Split(separators);

                includedFilename = elements.Length > 1 ? elements[elements.Length - 1] : normalizedFilename;
            }

            return includedFilename;
        }
    }
}
