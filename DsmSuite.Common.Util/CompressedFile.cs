using System;
using System.IO;
using System.IO.Compression;

namespace DsmSuite.Common.Util
{
    /// <typeparam name="TProggressType"></typeparam>
    public class CompressedFile<TProgressType>
    {
        private readonly string _filename;
        private const int ZipLeadBytes = 0x04034b50;

        public delegate void ReadContent(Stream stream, IProgress<TProgressType> progress);
        public delegate void WriteContent(Stream stream, IProgress<TProgressType> progress);

        public CompressedFile(string filename)
        {
            _filename = filename;
        }

        public void ReadFile(ReadContent readContent, IProgress<TProgressType> progress)
        {
            FileInfo fileInfo = new FileInfo(_filename);
            if (fileInfo.Exists)
            {
                if (IsCompressed)
                {
                    using (ZipArchive archive = ZipFile.OpenRead(fileInfo.FullName))
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
                    using (FileStream stream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
                    {
                        readContent(stream, progress);
                    }
                }
            }
        }

        public void WriteFile(WriteContent writeContent, IProgress<TProgressType> progress, bool compressed)
        {
            FileInfo fileInfo = new FileInfo(_filename);
            if (compressed)
            {
                using (FileStream fileStream = new FileStream(fileInfo.FullName, FileMode.Create))
                {
                    using (ZipArchive archive = new ZipArchive(fileStream, ZipArchiveMode.Create, false))
                    {
                        ZipArchiveEntry entry = archive.CreateEntry(fileInfo.Name);
                        using (Stream entryStream = entry.Open())
                        {
                            writeContent(entryStream, progress);
                        }
                    }
                }
            }
            else
            {
                using (FileStream fileStream = new FileStream(fileInfo.FullName, FileMode.Create))
                {
                    writeContent(fileStream, progress);
                }
            }
        }

        public bool FileExists
        {
            get
            {
                FileInfo fileInfo = new FileInfo(_filename);
                return fileInfo.Exists;
            }
        }

        public bool IsCompressed
        {
            get
            {
                bool isCompressedFile = false;

                FileInfo fileInfo = new FileInfo(_filename);

                if (fileInfo.Exists)
                {
                    using (FileStream stream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
                    {
                        byte[] bytes = new byte[4];
                        stream.Read(bytes, 0, 4);
                        if (BitConverter.ToInt32(bytes, 0) == ZipLeadBytes)
                        {
                            isCompressedFile = true;
                        }
                    }
                }

                return isCompressedFile;
            }
        }
    }
}
