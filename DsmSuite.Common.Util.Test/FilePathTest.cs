using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Common.Util.Test
{
    [TestClass]
    public class FilePathTest
    {
        [TestMethod]
        public void TestFilePathIsAbsolute()
        {
            string input = @"C:\temp\TestFileCopy.txt";
            string output = FilePath.ResolveFile(null, input);
            Assert.AreEqual(@"C:\temp\TestFileCopy.txt", output);
        }

        [TestMethod]
        public void TestFilePathIsRelative1()
        {
            string input = "TestFileCopy.txt";
            string path = @"C:\temp";
            string output = FilePath.ResolveFile(path, input);
            Assert.AreEqual(@"C:\temp\TestFileCopy.txt", output);
        }

        [TestMethod]
        public void TestFilePathIsRelative2()
        {
            string input = @"..\TestFileCopy.txt";
            string path = @"C:\temp\TestDir";
            string output = FilePath.ResolveFile(path, input);
            Assert.AreEqual(@"C:\temp\TestFileCopy.txt", output);
        }
    }
}
