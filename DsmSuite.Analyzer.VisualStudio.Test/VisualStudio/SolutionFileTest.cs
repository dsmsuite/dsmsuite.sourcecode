using System.IO;
using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Analyzer.VisualStudio.Test.Util;
using DsmSuite.Analyzer.VisualStudio.VisualStudio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.VisualStudio.Test.VisualStudio
{
    [TestClass]
    public class SolutionFileTest
    {
        [TestMethod]
        public void TestSolutionNameWithoutGroup()
        {
            string testDataDirectory = TestData.TestDataDirectory;
            string solutionFilename = Path.GetFullPath(Path.Combine(testDataDirectory, @"..\DsmSuite.sln"));

            AnalyzerSettings analyzerSettings = AnalyzerSettings.CreateDefault();
            SolutionFile solutionFile = new SolutionFile(solutionFilename, analyzerSettings, null);
            Assert.AreEqual("DsmSuite.sln", solutionFile.Name);
        }

        [TestMethod]
        public void TestVcxProjectFoundInCorrectFolder()
        {
            string testDataDirectory = TestData.TestDataDirectory;
            string solutionFilename = Path.GetFullPath(Path.Combine(testDataDirectory, @"..\DsmSuite.sln"));

            AnalyzerSettings analyzerSettings = AnalyzerSettings.CreateDefault();
            SolutionFile solutionFile = new SolutionFile(solutionFilename, analyzerSettings, null);
            solutionFile.Analyze();

            Assert.IsTrue(solutionFile.Projects.Count > 0);
            foreach (VcxProjectFile projectFile in solutionFile.Projects)
            {
                if (projectFile.ProjectName == "DsmSuite.Analyzer.VisualStudio.Test.Data")
                {
                    Assert.AreEqual("Analyzers.VisualStudioAnalyzer", projectFile.SolutionFolder);
                }
            }
        }
    }
}
