using System.Collections.Generic;
using System.IO;
using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Analyzer.VisualStudio.Test.Util;
using DsmSuite.Analyzer.VisualStudio.VisualStudio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.VisualStudio.Test.VisualStudio
{
    [TestClass]
    public class IncludeResolveStrategyTest
    {
        [TestMethod]
        public void TestResolveIncludeFileWithAbsolutePath()
        {
            IncludeResolveStrategy includeResolveStrategy = CreateIncludeResolveStrategy();
            string sourceFile = Path.Combine(TestData.TestDataDirectory, @"DirA\ClassA1.cpp");
            string includeFile = Path.Combine(TestData.TestDataDirectory, @"DirA\ClassA1.h");
            string expectedResolvedIncludeFile = includeFile;
            string actualResolvedIncludeFile = includeResolveStrategy.Resolve(sourceFile, includeFile);
            Assert.AreEqual(expectedResolvedIncludeFile, actualResolvedIncludeFile);
        }

        [TestMethod]
        public void TestResolveIncludeFileInSameDirectoryAsSourceFile()
        {
            IncludeResolveStrategy includeResolveStrategy = CreateIncludeResolveStrategy();
            string sourceFile = Path.Combine(TestData.TestDataDirectory, @"DirA\ClassA1.cpp");
            string includeFile = Path.Combine("ClassA1.h");
            string expectedResolvedIncludeFile = Path.Combine(TestData.TestDataDirectory, @"DirA\ClassA1.h");
            string actualResolvedIncludeFile = includeResolveStrategy.Resolve(sourceFile, includeFile);
            Assert.AreEqual(expectedResolvedIncludeFile, actualResolvedIncludeFile);
        }

        [TestMethod]
        public void TestResolveIncludeFileInSameDirectoryInProjectIncludeDirectories()
        {
            IncludeResolveStrategy includeResolveStrategy = CreateIncludeResolveStrategy();
            string sourceFile = Path.Combine(TestData.TestDataDirectory, @"DirA\ClassA1.cpp");
            string includeFile = Path.Combine("ClassB1.h");
            string expectedResolvedIncludeFile = Path.Combine(TestData.TestDataDirectory, @"DirB\ClassB1.h");
            string actualResolvedIncludeFile = includeResolveStrategy.Resolve(sourceFile, includeFile);
            Assert.AreEqual(expectedResolvedIncludeFile, actualResolvedIncludeFile);
        }

        [TestMethod]
        public void TestResolveIncludeFileInSameDirectoryInExternalIncludeDirectories()
        {
            IncludeResolveStrategy includeResolveStrategy = CreateIncludeResolveStrategy();
            string sourceFile = Path.Combine(TestData.TestDataDirectory, @"DirA\ClassA1.cpp");
            string includeFile = Path.Combine("External.h");
            string expectedResolvedIncludeFile = Path.Combine(TestData.TestDataDirectory, @"DirExternal\External.h");
            string actualResolvedIncludeFile = includeResolveStrategy.Resolve(sourceFile, includeFile);
            Assert.AreEqual(expectedResolvedIncludeFile, actualResolvedIncludeFile);
        }

        [TestMethod]
        public void TestResolveIncludeFileInSameDirectoryAsSystemIncludeDirectories()
        {
            IncludeResolveStrategy includeResolveStrategy = CreateIncludeResolveStrategy();
            string sourceFile = Path.Combine(TestData.TestDataDirectory, @"DirA\ClassA1.cpp");
            string includeFile = "windows.h";
            string expectedResolvedIncludeFile = @"C:\Program Files (x86)\Windows Kits\10\Include\10.0.18362.0\um\windows.h";
            string actualResolvedIncludeFile = includeResolveStrategy.Resolve(sourceFile, includeFile);
            Assert.AreEqual(expectedResolvedIncludeFile, actualResolvedIncludeFile);
        }

        private IncludeResolveStrategy CreateIncludeResolveStrategy()
        {
            string testDataDirectory = TestData.TestDataDirectory;
            Assert.IsNotNull(testDataDirectory);

            List<string> projectIncludeDirectories = new List<string>
            {
                Path.Combine(testDataDirectory, "DirA"),
                Path.Combine(testDataDirectory, "DirB"),
                Path.Combine(testDataDirectory, "DirExternal")
            };

            AnalyzerSettings analyzerSettings = AnalyzerSettings.CreateDefault();
            List<string> systemIncludeDirectories = analyzerSettings.Input.SystemIncludeDirectories;
            return new IncludeResolveStrategy(projectIncludeDirectories, systemIncludeDirectories);
        }
    }
}
