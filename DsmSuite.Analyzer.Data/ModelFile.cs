using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace DsmSuite.Analyzer.Data
{
    class ModelFile
    {
        private readonly FileInfo _fileInfo;
        private const int ZipLeadBytes = 0x04034b50;

        public delegate void ReadContent(Stream stream);
        public delegate void WriteContent(Stream stream);

        public ModelFile(string filename)
        {
            _fileInfo = new FileInfo(filename);
        }

        public void ReadFile(ReadContent readContent)
        {
            if (IsCompressedFile())
            {
                using (ZipArchive archive = ZipFile.OpenRead(_fileInfo.FullName))
                {
                    if (archive.Entries.Count == 1)
                    {
                        using (Stream entryStream = archive.Entries[0].Open())
                        {
                            readContent(entryStream);
                        }
                    }
                }
            }
            else
            {
                using (FileStream stream = new FileStream(_fileInfo.FullName, FileMode.Open, FileAccess.Read))
                {
                    readContent(stream);
                }
            }
        }

        public void WriteFile(WriteContent writeContent, bool compressed)
        {
            if (compressed)
            {
                using (FileStream fileStream = new FileStream(_fileInfo.FullName, FileMode.Create))
                {
                    using (ZipArchive archive = new ZipArchive(fileStream, ZipArchiveMode.Create, false))
                    {
                        ZipArchiveEntry entry = archive.CreateEntry(GetZipfileEntryName());
                        using (Stream entryStream = entry.Open())
                        {
                            writeContent(entryStream);
                        }
                    }
                }
            }
            else
            {
                using (FileStream fileStream = new FileStream(_fileInfo.FullName, FileMode.Create))
                {
                    writeContent(fileStream);
                }
            }
        }

        private bool IsCompressedFile()
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

        private string GetZipfileEntryName()
        {
            return _fileInfo.Name;
        }
    }
}
