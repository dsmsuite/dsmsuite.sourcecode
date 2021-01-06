using System;
using System.Collections.Generic;
using System.IO;
using DsmSuite.Analyzer.Cpp.Utils;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Cpp.IncludeResolve
{
    public abstract class IncludeResolveStrategy : IIncludeResolveStrategy
    {
        private readonly IList<string> _includeDirectories;

        protected IncludeResolveStrategy(IList<string> includeDirectories)
        {
            _includeDirectories = includeDirectories;
        }

        public IList<IncludeCandidate> GetCandidates(string relativeIncludeFilename)
        {
            List<IncludeCandidate> candidates = new List<IncludeCandidate>();

            try
            {
                foreach (string includeDirectory in _includeDirectories)
                {
                    string absoluteIncludeFilename = Path.Combine(includeDirectory, relativeIncludeFilename);
                    absoluteIncludeFilename = Path.GetFullPath(absoluteIncludeFilename);
                    if (File.Exists(absoluteIncludeFilename))
                    {
                        candidates.Add(new IncludeCandidate(absoluteIncludeFilename));
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogException($"Get candidates failed include={relativeIncludeFilename}", e);
            }

            return candidates;
        }

        public IList<string> Resolve(string sourceFilename, string relativeIncludeFilename, IList<IncludeCandidate> candidates)
        {
            List<string> includes = new List<string>();

            SelectCandidates(sourceFilename, candidates);

            foreach (IncludeCandidate candidate in candidates)
            {
                if (candidate.Resolved)
                {
                    includes.Add(candidate.Filename);
                }
            }

            LogCandidates(sourceFilename, relativeIncludeFilename, candidates);

            return includes;
        }


        protected abstract void SelectCandidates(string sourceFilename, IList<IncludeCandidate> candidates);

        private static void LogCandidates(string sourceFilename, string relativeIncludeFilename, IList<IncludeCandidate> candidates)
        {
            List<Tuple<string, bool>> candidateInfo = new List<Tuple<string, bool>>();
            foreach (IncludeCandidate candidate in candidates)
            {
                candidateInfo.Add(new Tuple<string, bool>(candidate.Filename, candidate.Resolved));
            }

            AnalyzerLogger.LogErrorIncludeFileAmbiguous(sourceFilename, relativeIncludeFilename, candidateInfo);
        }
    }
}
