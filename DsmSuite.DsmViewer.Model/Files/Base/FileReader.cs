using System;
using System.IO;
using System.IO.Compression;
using DsmSuite.DsmViewer.Model.Persistency;

namespace DsmSuite.DsmViewer.Model.Files.Base
{
    public abstract class FileReader
    {
        private readonly FileInfo _fileInfo;
        private const int ZipLeadBytes = 0x04034b50;

        protected abstract void ReadContent(Stream stream, IProgress<DsmProgressInfo> progress);

        protected FileReader(string filename)
        {
            _fileInfo = new FileInfo(filename);
        }

        public void ReadFile(IProgress<DsmProgressInfo> progress)
        {
            if (IsCompressedFile())
            {
                using (ZipArchive archive = ZipFile.OpenRead(_fileInfo.FullName))
                {
                    if (archive.Entries.Count == 1)
                    {
                        using (Stream entryStream = archive.Entries[0].Open())
                        {
                            ReadContent(entryStream, progress);
                        }
                    }
                }
            }
            else
            {
                using (FileStream stream = new FileStream(_fileInfo.FullName, FileMode.Open, FileAccess.Read))
                {
                    ReadContent(stream, progress);
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
    }
}
