using System.Collections.Generic;
using System.IO;
using System.Linq;
using DsmSuite.Analyzer.Cpp.IncludeResolve;
using DsmSuite.Analyzer.Cpp.Sources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DsmSuite.Analyzer.Cpp.Test.Sources
{
    [TestClass]
    public class SourceFileTest
    {
        [TestMethod]
        public void TestName()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\myfile.cpp"));
            Assert.IsTrue(sourceFile.IsSourceCodeFile);
            Assert.AreEqual(@"D:\Data\myfile.cpp", sourceFile.Name);
        }

        [TestMethod]
        public void TestCppFileIsSourceCodeFile()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\myfile.cpp"));
            Assert.IsTrue(sourceFile.IsSourceCodeFile);
            Assert.AreEqual("cpp", sourceFile.Extension);
        }

        [TestMethod]
        public void TestCFileIsSourceCodeFile()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\myfile.c"));
            Assert.IsTrue(sourceFile.IsSourceCodeFile);
            Assert.AreEqual("c", sourceFile.Extension);
        }

        [TestMethod]
        public void TestHppFileIsSourceCodeFile()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\myfile.hpp"));
            Assert.IsTrue(sourceFile.IsSourceCodeFile);
            Assert.AreEqual("hpp", sourceFile.Extension);
        }

        [TestMethod]
        public void TestHFileIsSourceCodeFile()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\myfile.h"));
            Assert.IsTrue(sourceFile.IsSourceCodeFile);
            Assert.AreEqual("h", sourceFile.Extension);
        }

        [TestMethod]
        public void TestFindSystemInclude()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\myfile.cpp"));
            Assert.AreEqual("stdio.h", sourceFile.ExtractFileFromIncludeStatement("#include <stdio.h>"));
        }

        [TestMethod]
        public void TestFindNormalInclude()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\myfile.cpp"));
            Assert.AreEqual("myfile.h", sourceFile.ExtractFileFromIncludeStatement("#include \"myfile.h\""));
        }

        [TestMethod]
        public void TestFindRelativeInclude()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\myfile.cpp"));
            Assert.AreEqual("myfile.h", sourceFile.ExtractFileFromIncludeStatement("#include \"..\\..\\myfile.h\""));
        }

        [TestMethod]
        public void TestResolveIncludeFile()
        {
            string implementationFile = @"D:\Data\myfile.cpp";
            string headerFile = "myfile.h";
            List<IncludeCandidate> candidates = new List<IncludeCandidate> {new IncludeCandidate(@"D:\Data\myfile.h")};
            List<string> resolvedIncludes = new List<string> {@"D:\Data\myfile.h"};

            SourceFile sourceFile = new SourceFile(new FileInfo(implementationFile));

            var mock = new Mock<IIncludeResolveStrategy>();
            mock.Setup(x => x.GetCandidates(headerFile)).Returns(candidates);
            mock.Setup(x => x.Resolve(implementationFile, headerFile, candidates)).Returns(resolvedIncludes);

            Assert.AreEqual(0, sourceFile.Includes.Count);
            sourceFile.ResolveIncludeFile(headerFile, mock.Object);
            Assert.AreEqual(1, sourceFile.Includes.Count);
            Assert.AreEqual(resolvedIncludes[0], sourceFile.Includes.ToList()[0]);
        }
    }
}
