using System.Collections.Generic;
using System.IO;
using DsmSuite.Analyzer.Cpp.IncludeResolve;
using DsmSuite.Analyzer.Cpp.Test.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.Cpp.Test.IncludeResolve
{
    [TestClass]
    public class IgnoreIncludeFileResolveStrategyTest
    {
        [TestMethod]
        public void TestResolveIncludeIgnoreStrategy()
        {
            string implementationFile = Path.Combine(TestData.RootDirectory, @"PackageA1\FileA.cpp");
            string headerFile = "FileA.h";

            List<string> includePaths = new List<string>
            {
                Path.Combine(TestData.RootDirectory, "PackageA1"),
                Path.Combine(TestData.RootDirectory, "PackageA2"),
                Path.Combine(TestData.RootDirectory, "PackageA3")
            };
            IIncludeResolveStrategy includeResolveStrategy = new IgnoreIncludeFileResolveStrategy(includePaths);
            IList<string> includes = includeResolveStrategy.Resolve(implementationFile, headerFile);
            Assert.AreEqual(0, includes.Count);
        }
    }
}
