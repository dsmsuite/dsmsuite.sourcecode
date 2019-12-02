using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Common.Util.Test
{
    [TestClass]
    public class CompressedFileTest : IProgress<int>
    {
        private readonly List<string> _lines = new List<string>();
        private int _progress;

        [TestInitialize]
        public void TestInitialize()
        {
            _lines.Clear();
            _progress = 0;
        }
        
        [TestMethod]
        public void TestFileDoesNotExist()
        {
            CompressedFile<int> file = new CompressedFile<int>(NotExistingFilePath);
            Assert.IsFalse(file.FileExists);
        }

        [TestMethod]
        public void TestIsUncompressedFile()
        {
            CompressedFile<int> file = new CompressedFile<int>(UncompressedFilePath);
            Assert.IsTrue(file.FileExists);
            Assert.IsFalse(file.IsCompressed);
        }

        [TestMethod]
        public void TestIsCompressedFile()
        {
            CompressedFile<int> file = new CompressedFile<int>(CompressedFilePath);
            Assert.IsTrue(file.FileExists);
            Assert.IsTrue(file.IsCompressed);
        }

        [TestMethod]
        public void TestReadUncompressedFile()
        {
            CompressedFile<int> file = new CompressedFile<int>(UncompressedFilePath);
            Assert.IsTrue(file.FileExists);
            file.ReadFile(ReadContent, this);
            Assert.AreEqual(4, _progress);
            Assert.AreEqual(4, _lines.Count);
            Assert.AreEqual("line0", _lines[0]);
            Assert.AreEqual("line1", _lines[1]);
            Assert.AreEqual("line2", _lines[2]);
            Assert.AreEqual("line3", _lines[3]);
        }

        [TestMethod]
        public void TestReadCompressedFile()
        {
            CompressedFile<int> file = new CompressedFile<int>(CompressedFilePath);
            Assert.IsTrue(file.FileExists);
            file.ReadFile(ReadContent, this);
            Assert.AreEqual(4, _progress);
            Assert.AreEqual(4, _lines.Count);
            Assert.AreEqual("line0", _lines[0]);
            Assert.AreEqual("line1", _lines[1]);
            Assert.AreEqual("line2", _lines[2]);
            Assert.AreEqual("line3", _lines[3]);
        }

        [TestMethod]
        public void TestWriteAndReadbackUncompressedFile()
        {
            string newPath = NewFilePath;
            CompressedFile<int> writtenFile = new CompressedFile<int>(newPath);
            Assert.IsFalse(writtenFile.FileExists);
            writtenFile.WriteFile(WriteContent, this, false);
            Assert.IsTrue(writtenFile.FileExists);
            Assert.AreEqual(4, _progress);

            _progress = 0;

            CompressedFile<int> readFile = new CompressedFile<int>(newPath);
            Assert.IsTrue(readFile.FileExists);
            readFile.ReadFile(ReadContent, this);
            Assert.AreEqual(4, _progress);
            Assert.AreEqual(4, _lines.Count);
            Assert.AreEqual("line0", _lines[0]);
            Assert.AreEqual("line1", _lines[1]);
            Assert.AreEqual("line2", _lines[2]);
            Assert.AreEqual("line3", _lines[3]);
        }

        [TestMethod]
        public void TestWriteAndReadbackCompressedFile()
        {
            string newPath = NewFilePath;
            CompressedFile<int> writtenFile = new CompressedFile<int>(newPath);
            Assert.IsFalse(writtenFile.FileExists);
            writtenFile.WriteFile(WriteContent, this, true);
            Assert.IsTrue(writtenFile.FileExists);
            Assert.AreEqual(4, _progress);

            _progress = 0;

            CompressedFile<int> readFile = new CompressedFile<int>(newPath);
            Assert.IsTrue(readFile.FileExists);
            readFile.ReadFile(ReadContent, this);
            Assert.AreEqual(4, _progress);
            Assert.AreEqual(4, _lines.Count);
            Assert.AreEqual("line0", _lines[0]);
            Assert.AreEqual("line1", _lines[1]);
            Assert.AreEqual("line2", _lines[2]);
            Assert.AreEqual("line3", _lines[3]);
        }

        private void ReadContent(Stream stream, IProgress<int> progress)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                string line;
                int lineCount = 0;
                do
                {
                    line = reader.ReadLine();
                    if (line != null)
                    {
                        _lines.Add(line);
                        lineCount++;
                        progress.Report(lineCount);
                    }
                } while (line != null);
            }
        }

        private void WriteContent(Stream stream, IProgress<int> progress)
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                for (int lineCount = 0; lineCount < 4; lineCount++)
                {
                    writer.WriteLine($"line{lineCount}");
                    progress.Report(lineCount+1);
                }
            }
        }

        private static string NotExistingFilePath => @"C:\Temp\TestFile.txt";

        private static string UncompressedFilePath => @"C:\Temp\TestFileCopy.txt";

        private static string CompressedFilePath => @"C:\Temp\TestFileCopy.zip";

        private static string NewFilePath => $@"C:\Temp\File{Guid.NewGuid().ToString()}.txt";

        public void Report(int progress)
        {
            _progress = progress;
        }
    }
}
