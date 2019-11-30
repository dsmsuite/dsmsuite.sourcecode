using System;
using System.IO;
using System.IO.Compression;

namespace DsmSuite.DsmViewer.Model.Files.Base
{
    public abstract class FileWriter
    {
        private readonly FileInfo _fileInfo;
        private bool _compressed;

        protected abstract void WriteContent(Stream stream, IProgress<ProgressInfo> progress);

        protected FileWriter(string filename)
        {
            _fileInfo = new FileInfo(filename);
        }

        public void WriteFile(bool compressed, IProgress<ProgressInfo> progress)
        {
            _compressed = compressed;
            if (_compressed)
            {
                using (FileStream fileStream = new FileStream(_fileInfo.FullName, FileMode.Create))
                {
                    using (ZipArchive archive = new ZipArchive(fileStream, ZipArchiveMode.Create, false))
                    {
                        ZipArchiveEntry entry = archive.CreateEntry(GetZipfileEntryName());
                        using (Stream entryStream = entry.Open())
                        {
                            WriteContent(entryStream, progress);
                        }
                    }
                }
            }
            else
            {
                using (FileStream fileStream = new FileStream(_fileInfo.FullName, FileMode.Create))
                {
                    WriteContent(fileStream, progress);
                }
            }
        }

        public bool IsCompressedFile()
        {
            return _compressed;
        }


        private string GetZipfileEntryName()
        {
            return _fileInfo.Name;
        }
    }
}
