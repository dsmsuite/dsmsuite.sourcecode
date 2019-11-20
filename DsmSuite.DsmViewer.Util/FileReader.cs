using System;
using System.IO;
using System.IO.Compression;

namespace DsmSuite.DsmViewer.Model.Files
{
    public abstract class FileReader
    {
        private readonly FileInfo _fileInfo;
        private const int ZipLeadBytes = 0x04034b50;

        protected abstract void ReadContent(Stream stream);

        protected FileReader(string filename)
        {
            _fileInfo = new FileInfo(filename);
        }

        public void ReadFile()
        {
            if (IsCompressedFile())
            {
                using (ZipArchive archive = ZipFile.OpenRead(_fileInfo.FullName))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string expectedEntry = GetZipfileEntryName();
                        if (entry.FullName.EndsWith(expectedEntry))
                        {
                            using (Stream entryStream = entry.Open())
                            {
                                ReadContent(entryStream);
                            }
                        }
                    }
                }
            }
            else
            {
                using (FileStream stream = new FileStream(_fileInfo.FullName, FileMode.Open, FileAccess.Read))
                {
                    ReadContent(stream);
                }
            }
        }

        public bool IsCompressedFile()
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
