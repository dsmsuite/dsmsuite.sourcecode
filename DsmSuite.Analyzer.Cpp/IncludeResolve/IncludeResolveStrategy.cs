using System;
using System.Collections.Generic;
using System.IO;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Cpp.IncludeResolve
{
    public abstract class IncludeResolveStrategy : IIncludeResolveStrategy
    {
        private readonly IList<string> _includeDirectories;

        protected class Candidate
        {
            public Candidate(string filename)
            {
                Filename = filename;
            }
            public string Filename { get; }
            public bool Resolved { set; get; }
        };

        protected IncludeResolveStrategy(IList<string> includeDirectories)
        {
            _includeDirectories = includeDirectories;
        }

        public IList<string> Resolve(string sourceFilename, string relativeIncludeFilename)
        {
            List<string> includes = new List<string>();

            IList<Candidate> candidates = GetCandidates(relativeIncludeFilename);

            SelectCandidates(sourceFilename, candidates);

            foreach (Candidate candidate in candidates)
            {
                if (candidate.Resolved)
                {
                    includes.Add(candidate.Filename);
                }
            }

            LogCandidates(sourceFilename, relativeIncludeFilename, candidates);

            return includes;
        }

        private IList<Candidate> GetCandidates(string relativeIncludeFilename)
        {
            List<Candidate> candidates = new List<Candidate>();

            try
            {
                foreach (string includeDirectory in _includeDirectories)
                {
                    string absoluteIncludeFilename = Path.Combine(includeDirectory, relativeIncludeFilename);
                    absoluteIncludeFilename = Path.GetFullPath(absoluteIncludeFilename);
                    if (File.Exists(absoluteIncludeFilename))
                    {
                        candidates.Add(new Candidate(absoluteIncludeFilename));
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogException($"Get candidates failed include={relativeIncludeFilename}", e);
            }

            return candidates;
        }
        protected abstract void SelectCandidates(string sourceFilename, IList<Candidate> candidates);

        private static void LogCandidates(string sourceFilename, string relativeIncludeFilename, IList<Candidate> candidates)
        {
            List<Tuple<string, bool>> candidateInfo = new List<Tuple<string, bool>>();
            foreach (Candidate candidate in candidates)
            {
                candidateInfo.Add(new Tuple<string, bool>(candidate.Filename, candidate.Resolved));
            }

            Util.AnalyzerLogger.LogErrorIncludeFileAmbigious(sourceFilename, relativeIncludeFilename, candidateInfo);
        }
    }
}
