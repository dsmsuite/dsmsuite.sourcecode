using System.Collections.Generic;
using System.IO;
using DsmSuite.Analyzer.VisualStudio.Analysis;
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
            string expectedResolvedIncludeFile = @"C:\Program Files (x86)\Windows Kits\8.1\Include\um\windows.h";
            string actualResolvedIncludeFile = includeResolveStrategy.Resolve(sourceFile, includeFile);
            Assert.AreEqual(expectedResolvedIncludeFile, actualResolvedIncludeFile);
        }

        private IncludeResolveStrategy CreateIncludeResolveStrategy()
        {
            string testDataDirectory = TestData.TestDataDirectory;
            Assert.IsNotNull(testDataDirectory);

            List<string> projectIncludeDirectories = new List<string>();
            projectIncludeDirectories.Add(Path.Combine(testDataDirectory, "DirA"));
            projectIncludeDirectories.Add(Path.Combine(testDataDirectory, "DirB"));

            List<string> interfaceIncludeDirectories = new List<string>();
            interfaceIncludeDirectories.Add(Path.Combine(testDataDirectory, "DirInterfaces"));

            List<string> externalIncludeDirectories = new List<string>();
            externalIncludeDirectories.Add(Path.Combine(testDataDirectory, "DirExternal"));
            
            AnalyzerSettings analyzerSettings = AnalyzerSettings.CreateDefault();
            List<string> systemIncludeDirectories = analyzerSettings.SystemIncludeDirectories;
            return new IncludeResolveStrategy(projectIncludeDirectories, interfaceIncludeDirectories, externalIncludeDirectories, systemIncludeDirectories);
        }
    }
}
