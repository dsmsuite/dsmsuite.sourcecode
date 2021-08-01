
using System.Collections.Generic;

namespace DsmSuite.Analyzer.Cpp.IncludeResolve
{
    public interface IIncludeResolveStrategy
    {
        IList<IncludeCandidate> GetCandidates(string relativeIncludeFilename);
        IList<string> Resolve(string sourceFilename, string relativeIncludeFilename, IList<IncludeCandidate> candidates);
    }
}
