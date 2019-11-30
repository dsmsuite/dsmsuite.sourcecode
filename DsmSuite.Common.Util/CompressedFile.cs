using System;
using System.IO;
using System.IO.Compression;

namespace DsmSuite.Common.Util
{
    public class CompressedFile
    {
        private readonly FileInfo _fileInfo;
        private readonly bool _isCompressed;
        private const int ZipLeadBytes = 0x04034b50;

        public delegate void ReadContent(Stream stream, IProgress<int> progress);
        public delegate void WriteContent(Stream stream, IProgress<int> progress);

        public CompressedFile(string filename)
        {
            _fileInfo = new FileInfo(filename);
            _isCompressed = false;
            if (_fileInfo.Exists)
            {
                _isCompressed = DetectZipLeadBytes();
            }
        }

        public void ReadFile(ReadContent readContent, IProgress<int> progress)
        {
            if (_isCompressed)
            {
                using (ZipArchive archive = ZipFile.OpenRead(_fileInfo.FullName))
                {
                    if (archive.Entries.Count == 1)
                    {
                        using (Stream entryStream = archive.Entries[0].Open())
                        {
                            readContent(entryStream, progress);
                        }
                    }
                }
            }
            else
            {
                using (FileStream stream = new FileStream(_fileInfo.FullName, FileMode.Open, FileAccess.Read))
                {
                    readContent(stream, progress);
                }
            }
        }

        public void WriteFile(WriteContent writeContent, IProgress<int> progress, bool compressed)
        {
            if (compressed)
            {
                using (FileStream fileStream = new FileStream(_fileInfo.FullName, FileMode.Create))
                {
                    using (ZipArchive archive = new ZipArchive(fileStream, ZipArchiveMode.Create, false))
                    {
                        ZipArchiveEntry entry = archive.CreateEntry(_fileInfo.Name);
                        using (Stream entryStream = entry.Open())
                        {
                            writeContent(entryStream, progress);
                        }
                    }
                }
            }
            else
            {
                using (FileStream fileStream = new FileStream(_fileInfo.FullName, FileMode.Create))
                {
                    writeContent(fileStream, progress);
                }
            }
        }

        public bool FileExists => _fileInfo.Exists;

        public bool IsCompressed => _isCompressed;

        private bool DetectZipLeadBytes()
        {
            bool isCompressedFile = false;

            using (FileStream stream = new FileStream(_fileInfo.FullName, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[4];
                stream.Read(bytes, 0, 4);
                if (BitConverter.ToInt32(bytes, 0) == ZipLeadBytes)
                {
                    isCompressedFile = true;
                }
            }

            return isCompressedFile;
        }
    }
}
