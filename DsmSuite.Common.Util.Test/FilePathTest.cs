using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void When_ResolveFileIsCalledWithAnAbsolutePathAndRelativeFilename_Then_TheResolvedAbsoluteFileNameIsReturned()
        {
            string path = @"C:\temp\TestDir";
            string input = @"..\TestFileCopy.txt";
            string output = FilePath.ResolveFile(path, input);
            Assert.AreEqual(@"C:\temp\TestFileCopy.txt", output);
        }
    }
}
