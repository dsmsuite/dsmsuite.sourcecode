using DsmSuite.Analyzer.Cpp.IncludeResolve;
using System.Text.RegularExpressions;

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

        public void ResolveIncludeFile(string relativeIncludeFilename, IIncludeResolveStrategy includeResolveStrategy)
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

        public string ExtractFileFromIncludeStatement(string line)
        {
            // Preprocessing directives are lines in your program that start with #.
            // The # is followed by an identifier that is the directive name.
            // For example, #define is the directive that defines a macro.
            // Whitespace is also allowed before and after the `#'.

            string includedFilename = null;

            string lineWithoutComments = StripComments(line).Trim();

            Regex regex = new Regex("#[ ]{0,}include[ ]{1,}");
            Match match = regex.Match(lineWithoutComments);
            if (match.Success && match.Index == 0)
            {
                string normalizedFilename = lineWithoutComments.Substring(match.Value.Length + 1).Replace("\"", "").Replace(">", "").Replace("<", "").Trim();

                char[] separators = { '\\', '/' };
                string[] elements = normalizedFilename.Split(separators);

                includedFilename = elements.Length > 1 ? elements[elements.Length - 1] : normalizedFilename;
            }

            return includedFilename;
        }

        private static string StripComments(string line)
        {
            string result = line;

            int beginCommentStyle1 = line.IndexOf("//", StringComparison.Ordinal);
            if (beginCommentStyle1 != -1)
            {
                result = line.Substring(0, beginCommentStyle1).Trim();
            }

            int beginCommentStyle2 = line.IndexOf("/*", StringComparison.Ordinal);
            if (beginCommentStyle2 != -1)
            {
                result = line.Substring(0, beginCommentStyle2);
            }

            return result;
        }
    }
}
