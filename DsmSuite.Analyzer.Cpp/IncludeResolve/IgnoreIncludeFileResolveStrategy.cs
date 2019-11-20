using System.Collections.Generic;

namespace DsmSuite.Analyzer.Cpp.IncludeResolve
{
    public class IgnoreIncludeFileResolveStrategy : IncludeResolveStrategy
    {
        public IgnoreIncludeFileResolveStrategy(List<string> includeDirectories) : base(includeDirectories)
        {
        }

        protected override void SelectCandidates(string sourceFilename, IList<Candidate> candidates)
        {
        }
    }
}
