using DsmSuite.Analyzer.Cpp.IncludeResolve;
using DsmSuite.Analyzer.Cpp.Test.Util;

namespace DsmSuite.Analyzer.Cpp.Test.IncludeResolve
{
    [TestClass]
    public class AllIncludeFileResolveStrategyTest
    {
        [TestMethod]
        public void TestResolveIncludeAllStrategy()
        {
            string implementationFile = Path.Combine(TestData.RootDirectory, @"PackageA1\FileA.cpp");
            string headerFile = "FileA.h";

            List<string> includePaths = new List<string>
            {
                Path.Combine(TestData.RootDirectory, "PackageA1"),
                Path.Combine(TestData.RootDirectory, "PackageA2"),
                Path.Combine(TestData.RootDirectory, "PackageA3")
            };
            IIncludeResolveStrategy includeResolveStrategy = new AllIncludeFileResolveStrategy(includePaths);
            IList<IncludeCandidate> candidates = includeResolveStrategy.GetCandidates(headerFile);
            IList<string> includes = includeResolveStrategy.Resolve(implementationFile, headerFile, candidates);
            Assert.AreEqual(3, includes.Count);
            Assert.AreEqual(Path.GetFullPath(Path.Combine(TestData.RootDirectory, @"PackageA1\FileA.h")), includes[0]);
            Assert.AreEqual(Path.GetFullPath(Path.Combine(TestData.RootDirectory, @"PackageA2\FileA.h")), includes[1]);
            Assert.AreEqual(Path.GetFullPath(Path.Combine(TestData.RootDirectory, @"PackageA3\FileA.h")), includes[2]);
        }
    }
}
