using System.Collections.Generic;

namespace DsmSuite.Analyzer.Cpp.IncludeResolve
{
    public class IgnoreIncludeFileResolveStrategy : IncludeResolveStrategy
    {
        public IgnoreIncludeFileResolveStrategy(IList<string> includeDirectories) : base(includeDirectories)
        {
        }

        protected override void SelectCandidates(string sourceFilename, IList<IncludeCandidate> candidates)
        {
        }
    }
}
