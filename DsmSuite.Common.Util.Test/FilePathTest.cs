using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DsmSuite.Common.Util.Test
{
    [TestClass]
    public class FilePathTest
    {
        [TestMethod]
        public void When_ResolveFileIsCalledWithAnAbsoluteFilename_Then_TheAbsoluteFileNameIsReturned()
        {
            string input = @"C:\temp\TestFileCopy.txt";
            string output = FilePath.ResolveFile(null, input);
            Assert.AreEqual(@"C:\temp\TestFileCopy.txt", output);
        }

        [TestMethod]
        public void When_ResolveFileIsCalledWithAnAbsolutePathAndFilename_Then_TheResolvedAbsoluteFileNameIsReturned()
        {
            string path = @"C:\temp";
            string input = "TestFileCopy.txt";
            string output = FilePath.ResolveFile(path, input);
            Assert.AreEqual(@"C:\temp\TestFileCopy.txt", output);
        }

        [TestMethod]
        public void When_ResolveFileIsCalledWithAnAbsolutePathAndMultipleFilenames_Then_TheResolvedAbsoluteFileNamesAreReturned()
        {
            string path = @"C:\temp";
            string[] input = { "TestFileCopy.txt", "TestFileCopyAgain.txt" };
            List<string> output = FilePath.ResolveFiles(path, input);
            Assert.AreEqual(@"C:\temp\TestFileCopy.txt", output[0]);
            Assert.AreEqual(@"C:\temp\TestFileCopyAgain.txt", output[1]);
        }

        [TestMethod]
        public void When_ResolveFileIsCalledWithAnAbsolutePathAndRelativeFilename_Then_TheResolvedAbsoluteFileNameIsReturned()
        {
            string path = @"C:\temp\TestDir";
            string input = @"..\TestFileCopy.txt";
            string output = FilePath.ResolveFile(path, input);
            Assert.AreEqual(@"C:\temp\TestFileCopy.txt", output);
        }
    }
}
