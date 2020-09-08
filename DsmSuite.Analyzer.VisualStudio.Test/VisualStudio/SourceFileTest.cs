using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Analyzer.VisualStudio.Test.Util;
using DsmSuite.Analyzer.VisualStudio.VisualStudio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.VisualStudio.Test.VisualStudio
{
    [TestClass]
    public class SourceFileTest
    {
        [TestMethod]
        public void TestFileInfo()
        {
            string testDataDirectory = TestData.TestDataDirectory;
            FileInfo fileInfo = new FileInfo(Path.Combine(testDataDirectory, @"DirA\ClassA1.cpp"));

            SourceFile sourceFile = new SourceFile(fileInfo, null, null);
            Assert.AreEqual(fileInfo, sourceFile.SourceFileInfo);
        }

        [TestMethod]
        public void TestNameIsFullPath()
        {
            string testDataDirectory = TestData.TestDataDirectory;
            FileInfo fileInfo = new FileInfo(Path.Combine(testDataDirectory, @"DirA\ClassA1.cpp"));

            SourceFile sourceFile = new SourceFile(fileInfo, null, null);
            Assert.AreEqual(fileInfo.FullName, sourceFile.Name);
        }

        [TestMethod]
        public void TestFileTypeIsExtension()
        {
            string testDataDirectory = TestData.TestDataDirectory;
            FileInfo fileInfo = new FileInfo(Path.Combine(testDataDirectory, @"DirA\ClassA1.cpp"));

            SourceFile sourceFile = new SourceFile(fileInfo, null, null);
            Assert.AreEqual("cpp", sourceFile.FileType);
        }

        [TestMethod]
        public void TestProjectFolder()
        {
            string testDataDirectory = TestData.TestDataDirectory;
            FileInfo fileInfo = new FileInfo(Path.Combine(testDataDirectory, @"DirA\ClassA1.cpp"));

            SourceFile sourceFile = new SourceFile(fileInfo, "ProjectFolderName", null);
            Assert.AreEqual("ProjectFolderName", sourceFile.ProjectFolder);
        }
       
        [TestMethod]
        public void TestAnalyzeFindsIncludes()
        {
            string testDataDirectory = TestData.TestDataDirectory;
            FileInfo fileInfo1 = new FileInfo(Path.Combine(testDataDirectory, @"DirA\ClassA2.cpp"));
            List<string> projectIncludes = new List<string>
            {
                Path.Combine(testDataDirectory, "DirA"),
                Path.Combine(testDataDirectory, "DirB"),
                Path.Combine(testDataDirectory, "DirC"),
                Path.Combine(testDataDirectory, "DirD")
            };
            List<string> externalIncludes = new List<string>();
            externalIncludes.Add(Path.Combine(testDataDirectory, "DirExternal"));
            List<string> systemIncludes = AnalyzerSettings.CreateDefault().SystemIncludeDirectories;

            IncludeResolveStrategy includeResolveStrategy = new IncludeResolveStrategy(projectIncludes, externalIncludes, systemIncludes);
            SourceFile sourceFile = new SourceFile(fileInfo1, null, includeResolveStrategy);
            sourceFile.Analyze();

            ImmutableHashSet<string> includes = sourceFile.Includes.ToImmutableHashSet();
            Assert.IsTrue(includes.Contains(Path.Combine(testDataDirectory, @"DirA\ClassA2.h")));
            Assert.IsTrue(includes.Contains(Path.Combine(testDataDirectory, @"DirA\ClassA1.h")));
            Assert.IsTrue(includes.Contains(Path.Combine(testDataDirectory, @"DirB\ClassB1.h")));
            Assert.IsTrue(includes.Contains(Path.Combine(testDataDirectory, @"DirC\ClassC1.h")));
            Assert.IsTrue(includes.Contains(Path.Combine(testDataDirectory, @"DirD\ClassD1.h")));
            Assert.IsTrue(includes.Contains(Path.Combine(testDataDirectory, @"DirExternal\External.h")));
            Assert.IsTrue(includes.Contains(@"C:\Program Files (x86)\Windows Kits\8.1\Include\um\windows.h"));
            Assert.AreEqual(7, sourceFile.Includes.Count);
        }
    }
}
