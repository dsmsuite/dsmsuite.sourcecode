
using System.Collections.Generic;

namespace DsmSuite.Analyzer.Cpp.IncludeResolve
{
    public interface IIncludeResolveStrategy
    {
        IList<string> Resolve(string sourceFilename, string relativeIncludeFilename);
    }
}
