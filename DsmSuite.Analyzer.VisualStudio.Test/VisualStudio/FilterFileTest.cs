using System.IO;
using DsmSuite.Analyzer.VisualStudio.VisualStudio;
using DsmSuite.Analyzer.VisualStudio.Test.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.VisualStudio.Test.VisualStudio
{
    [TestClass]
    public class FilterFileTest
    {
        [TestMethod]
        public void TestRetrieveProjectFolderForExistingSourceFile()
        {
            string filterFilename = GetFilterFilename();
            Assert.IsTrue(File.Exists(filterFilename));

            FilterFile filterFile = new FilterFile(filterFilename);
            string actualProjectFolder = filterFile.GetSourceFileProjectFolder("DirA/ClassA1.h"); // Filter paths use forward slashes
            string expectedProjectFolder = "FolderA";
            Assert.AreEqual(expectedProjectFolder, actualProjectFolder);
        }

        [TestMethod]
        public void TestRetrieveProjectFolderForNotExistingSourceFile()
        {
            string filterFilename = GetFilterFilename();
            Assert.IsTrue(File.Exists(filterFilename));

            FilterFile filterFile = new FilterFile(filterFilename);
            string actualProjectFolder = filterFile.GetSourceFileProjectFolder(@"DirA\ClassA99.h");
            string expectedProjectFolder = "";
            Assert.AreEqual(expectedProjectFolder, actualProjectFolder);
        }

        private static string GetFilterFilename()
        {
            string testDataDirectory = TestData.TestDataDirectory;
            Assert.IsNotNull(testDataDirectory);
            string relativeFilterFilename = "DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj.filters";
            string absoluteFilterFilename = Path.Combine(testDataDirectory, relativeFilterFilename);
            return absoluteFilterFilename;
        }
    }
}
