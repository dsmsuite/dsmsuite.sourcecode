using System.Collections.Generic;
using System.IO;
using DsmSuite.Analyzer.Cpp.IncludeResolve;
using DsmSuite.Analyzer.Cpp.Test.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.Cpp.Test.IncludeResolve
{
    [TestClass]
    public class BestMatchIncludeFileResolveStrategyTest
    {
        [TestMethod]
        public void TestResolveIncludeBestStrategy()
        {
            string implementationFile = Path.Combine(TestData.RootDirectory, @"PackageA1\FileA.cpp");
            string headerFile = "FileA.h";

            List<string> includePaths = new List<string>
            {
                Path.Combine(TestData.RootDirectory, "PackageA1"),
                Path.Combine(TestData.RootDirectory, "PackageA2"),
                Path.Combine(TestData.RootDirectory, "PackageA3")
            };
            IIncludeResolveStrategy includeResolveStrategy = new BestMatchIncludeFileResolveStrategy(includePaths);
            IList<IncludeCandidate> candidates = includeResolveStrategy.GetCandidates(headerFile);
            IList<string> includes = includeResolveStrategy.Resolve(implementationFile, headerFile, candidates);
            Assert.AreEqual(1, includes.Count);
            Assert.AreEqual(Path.GetFullPath(Path.Combine(TestData.RootDirectory, @"PackageA1\FileA.h")), includes[0]);
        }
    }
}
