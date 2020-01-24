using System.Collections.Generic;

namespace DsmSuite.Analyzer.Cpp.IncludeResolve
{
    public class AllIncludeFileResolveStrategy : IncludeResolveStrategy
    {
        public AllIncludeFileResolveStrategy(IList<string> includeDirectories) : base(includeDirectories)
        {
        }

        protected override void SelectCandidates(string sourceFilename, IList<Candidate> candidates)
        {
            foreach (Candidate candidate in candidates)
            {
                candidate.Resolved = true;
            }
        }
    }
}
