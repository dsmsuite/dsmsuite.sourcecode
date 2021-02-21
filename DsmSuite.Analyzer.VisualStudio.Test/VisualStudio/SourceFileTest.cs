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

            SourceFile sourceFile = new SourceFile(fileInfo, null, null, null);
            Assert.AreEqual(fileInfo, sourceFile.SourceFileInfo);
        }

        [TestMethod]
        public void TestNameIsFullPath()
        {
            string testDataDirectory = TestData.TestDataDirectory;
            FileInfo fileInfo = new FileInfo(Path.Combine(testDataDirectory, @"DirA\ClassA1.cpp"));

            SourceFile sourceFile = new SourceFile(fileInfo, null, null, null);
            Assert.AreEqual(fileInfo.FullName, sourceFile.Name);
        }

        [TestMethod]
        public void TestFileTypeIsExtension()
        {
            string testDataDirectory = TestData.TestDataDirectory;
            FileInfo fileInfo = new FileInfo(Path.Combine(testDataDirectory, @"DirA\ClassA1.cpp"));

            SourceFile sourceFile = new SourceFile(fileInfo, null, null, null);
            Assert.AreEqual("cpp", sourceFile.FileType);
        }

        [TestMethod]
        public void TestProjectFolder()
        {
            string testDataDirectory = TestData.TestDataDirectory;
            FileInfo fileInfo = new FileInfo(Path.Combine(testDataDirectory, @"DirA\ClassA1.cpp"));

            SourceFile sourceFile = new SourceFile(fileInfo, "ProjectFolderName", null, null);
            Assert.AreEqual("ProjectFolderName", sourceFile.ProjectFolder);
        }

        [TestMethod]
        public void TestClassWithSameNameAndSameContentHaveSameChecksum()
        {
            string testDataDirectory = TestData.TestDataDirectory;
            FileInfo fileInfo1 = new FileInfo(Path.Combine(testDataDirectory, @"DirA\ClassA1.cpp"));
            FileInfo fileInfo2 = new FileInfo(Path.Combine(testDataDirectory, "DirClones/Identical/ClassA1.cpp"));
            SourceFile sourceFile1 = new SourceFile(fileInfo1, null, null, null);
            SourceFile sourceFile2 = new SourceFile(fileInfo2, null, null, null);
            Assert.AreEqual(sourceFile1.Checksum, sourceFile2.Checksum);
        }

        [TestMethod]
        public void TestClassWithSameNameAndOtherContentHaveOtherChecksum()
        {
            string testDataDirectory = TestData.TestDataDirectory;
            FileInfo fileInfo1 = new FileInfo(Path.Combine(testDataDirectory, @"DirA\ClassA1.cpp"));
            FileInfo fileInfo2 = new FileInfo(Path.Combine(testDataDirectory, "DirClones/NotIdentical/ClassA1.cpp"));
            SourceFile sourceFile1 = new SourceFile(fileInfo1, null, null, null);
            SourceFile sourceFile2 = new SourceFile(fileInfo2, null, null, null);
            Assert.AreNotEqual(sourceFile1.Checksum, sourceFile2.Checksum);
        }

        [TestMethod]
        public void TestClassWithOtherNameAndSameContentHaveOtherIChecksum()
        {
            string testDataDirectory = TestData.TestDataDirectory;
            FileInfo fileInfo1 = new FileInfo(Path.Combine(testDataDirectory, @"DirA\ClassA1.cpp"));
            FileInfo fileInfo2 = new FileInfo(Path.Combine(testDataDirectory, "DirClones/Identical/CopyClassA1.cpp"));
            SourceFile sourceFile1 = new SourceFile(fileInfo1, null, null, null);
            SourceFile sourceFile2 = new SourceFile(fileInfo2, null, null, null);
            Assert.AreNotEqual(sourceFile1.Checksum, sourceFile2.Checksum);
        }
       
        [TestMethod]
        public void TestAnalyzeIncludesFoundInSourceFileUsingPredefinedIncludePaths()
        {
            string testDataDirectory = TestData.TestDataDirectory;
            FileInfo fileInfo1 = new FileInfo(Path.Combine(testDataDirectory, @"DirA\ClassA2.cpp"));
            List<string> projectIncludes = new List<string>
            {
                Path.Combine(testDataDirectory, "DirA"),
                Path.Combine(testDataDirectory, "DirB"),
                Path.Combine(testDataDirectory, "DirC"),
                Path.Combine(testDataDirectory, "DirD"),
                Path.Combine(testDataDirectory, "DirE"),
                Path.Combine(testDataDirectory, "DirF"),
                Path.Combine(testDataDirectory, "DirG"),
                Path.Combine(testDataDirectory, "DirExternal")
            };
            List<string> systemIncludes = AnalyzerSettings.CreateDefault().Input.SystemIncludeDirectories;

            IncludeResolveStrategy includeResolveStrategy = new IncludeResolveStrategy(projectIncludes, systemIncludes);
            SourceFile sourceFile = new SourceFile(fileInfo1, null, null, includeResolveStrategy);
            sourceFile.Analyze();

            ImmutableHashSet<string> includes = sourceFile.Includes.ToImmutableHashSet();
            Assert.IsTrue(includes.Contains(Path.Combine(testDataDirectory, @"C:\Program Files (x86)\Windows Kits\10\Include\10.0.18362.0\um\windows.h")));
            Assert.IsTrue(includes.Contains(Path.Combine(testDataDirectory, @"DirA\ClassA2.h")));
            Assert.IsTrue(includes.Contains(Path.Combine(testDataDirectory, @"DirA\ClassA1.h")));
            Assert.IsTrue(includes.Contains(Path.Combine(testDataDirectory, @"DirB\ClassB1.h")));
            Assert.IsTrue(includes.Contains(Path.Combine(testDataDirectory, @"DirC\ClassC1.h")));
            Assert.IsTrue(includes.Contains(Path.Combine(testDataDirectory, @"DirD\ClassD1.h")));
            Assert.IsTrue(includes.Contains(Path.Combine(testDataDirectory, @"DirE\ClassE1.h")));
            Assert.IsTrue(includes.Contains(Path.Combine(testDataDirectory, @"DirF\ClassF1.h")));
            Assert.IsTrue(includes.Contains(Path.Combine(testDataDirectory, @"DirG\ClassG1.h")));
            Assert.IsTrue(includes.Contains(Path.Combine(testDataDirectory, @"DirExternal\External.h")));
            Assert.IsTrue(includes.Contains(Path.Combine(testDataDirectory, @"DirInterfaces\ClassA3.h")));
            Assert.AreEqual(11, sourceFile.Includes.Count);
        }
    }
}
