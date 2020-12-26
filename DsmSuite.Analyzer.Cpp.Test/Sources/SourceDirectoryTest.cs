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
            AnalyzerSettings analyzerSettings = AnalyzerSettings.CreateDefault();
            analyzerSettings.Input.RootDirectory = TestData.RootDirectory;
            analyzerSettings.Input.SourceDirectories.Clear();
            analyzerSettings.Input.SourceDirectories.Add(TestData.RootDirectory);
            analyzerSettings.Input.ExternalIncludeDirectories.Clear();
            analyzerSettings.Input.IgnorePaths.Clear();
            analyzerSettings.Analysis.ResolveMethod = ResolveMethod.AddBestMatch;

        SourceDirectory sourceDirectory = new SourceDirectory(analyzerSettings);
            sourceDirectory.Analyze();
            const int numberOfHeaderFiles = 7;
            const int numberOfImplementationFiles = 5;
            Assert.AreEqual(numberOfHeaderFiles + numberOfImplementationFiles, sourceDirectory.SourceFiles.Count);
        }
    }
}
