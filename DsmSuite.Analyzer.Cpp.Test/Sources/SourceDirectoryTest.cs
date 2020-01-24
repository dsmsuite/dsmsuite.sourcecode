using System.Collections.Generic;
using DsmSuite.Analyzer.Cpp.Settings;
using DsmSuite.Analyzer.Cpp.Sources;
using DsmSuite.Analyzer.Cpp.Test.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.Cpp.Test.Sources
{
    [TestClass]
    public class SourceDirectoryTest
    {
        [TestMethod]
        public void TestSourceFilesFound()
        {
            AnalyzerSettings analyzerSettings = new AnalyzerSettings
            {
                ResolveMethod = ResolveMethod.AddBestMatch,
                SourceDirectories = new List<string>(),
                ExternalIncludeDirectories = new List<string>(),
                IgnorePaths = new List<string>()
            };
            analyzerSettings.SourceDirectories.Add(TestData.RootDirectory);
            analyzerSettings.RootDirectory = TestData.RootDirectory;

            SourceDirectory sourceDirectory = new SourceDirectory(analyzerSettings);
            sourceDirectory.Analyze();
            const int numberOfHeaderFiles = 7;
            const int numberOfImplementationFiles = 5;
            Assert.AreEqual(numberOfHeaderFiles + numberOfImplementationFiles, sourceDirectory.SourceFiles.Count);
        }
    }
}
