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
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\my-file.cpp"));
            Assert.IsTrue(sourceFile.IsSourceCodeFile);
            Assert.AreEqual(@"D:\Data\my-file.cpp", sourceFile.Name);
        }

        [TestMethod]
        public void TestCppFileIsSourceCodeFile()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\my-file.cpp"));
            Assert.IsTrue(sourceFile.IsSourceCodeFile);
            Assert.AreEqual("cpp", sourceFile.Extension);
        }

        [TestMethod]
        public void TestCFileIsSourceCodeFile()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\my-file.c"));
            Assert.IsTrue(sourceFile.IsSourceCodeFile);
            Assert.AreEqual("c", sourceFile.Extension);
        }

        [TestMethod]
        public void TestHppFileIsSourceCodeFile()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\my-file.hpp"));
            Assert.IsTrue(sourceFile.IsSourceCodeFile);
            Assert.AreEqual("hpp", sourceFile.Extension);
        }

        [TestMethod]
        public void TestHFileIsSourceCodeFile()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\my-file.h"));
            Assert.IsTrue(sourceFile.IsSourceCodeFile);
            Assert.AreEqual("h", sourceFile.Extension);
        }

        [TestMethod]
        public void TestFindSystemInclude()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\my-file.cpp"));
            Assert.AreEqual("stdio.h", sourceFile.ExtractFileFromIncludeStatement("#include <stdio.h>"));
        }

        [TestMethod]
        public void TestFindNormalInclude()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\my-file.cpp"));
            Assert.AreEqual("my-file.h", sourceFile.ExtractFileFromIncludeStatement("#include \"my-file.h\""));
        }

        [TestMethod]
        public void TestFindNormalIncludeWithAdditionalSpaces()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\my-file.cpp"));
            Assert.AreEqual("my-file.h", sourceFile.ExtractFileFromIncludeStatement("#include  \"my-file.h\""));
        }

        [TestMethod]
        public void TestFindNormalIncludeWithCommentStyle1()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\my-file.cpp"));
            Assert.AreEqual("my-file.h", sourceFile.ExtractFileFromIncludeStatement("#include \"my-file.h\" // Some comment"));
        }

        [TestMethod]
        public void TestFindNormalIncludeWithCommentStyle2()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\my-file.cpp"));
            Assert.AreEqual("my-file.h", sourceFile.ExtractFileFromIncludeStatement("#include \"my-file.h\" /* Some comment */"));
        }

        [TestMethod]
        public void TestFindNormalIncludeWithCommentMultilineStyle2()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\my-file.cpp"));
            Assert.AreEqual("my-file.h", sourceFile.ExtractFileFromIncludeStatement("#include \"my-file.h\" /* Some comment "));
        }

        [TestMethod]
        public void TestFindNormalIncludeWithSpaceInBeforeHash()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\my-file.cpp"));
            Assert.AreEqual("my-file.h", sourceFile.ExtractFileFromIncludeStatement(" #include \"my-file.h\""));
        }

        [TestMethod]
        public void TestFindNormalIncludeWithSpaceBeforeInclude()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\my-file.cpp"));
            Assert.AreEqual("my-file.h", sourceFile.ExtractFileFromIncludeStatement("# include \"my-file.h\""));
        }

        [TestMethod]
        public void TestDoNotFindNormalIncludeWithNonSpaceBeforeHash()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\my-file.cpp"));
            Assert.IsNull(sourceFile.ExtractFileFromIncludeStatement("a #include \"my-file.h\""));
        }

        [TestMethod]
        public void TestFindRelativeInclude()
        {
            SourceFile sourceFile = new SourceFile(new FileInfo(@"D:\Data\my-file.cpp"));
            Assert.AreEqual("my-file.h", sourceFile.ExtractFileFromIncludeStatement("#include \"..\\..\\my-file.h\""));
        }

        [TestMethod]
        public void TestResolveIncludeFile()
        {
            string implementationFile = @"D:\Data\my-file.cpp";
            string headerFile = "my-file.h";
            List<IncludeCandidate> candidates = new List<IncludeCandidate> {new IncludeCandidate(@"D:\Data\my-file.h")};
            List<string> resolvedIncludes = new List<string> {@"D:\Data\my-file.h"};

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
