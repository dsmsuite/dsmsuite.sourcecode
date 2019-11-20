using System.IO;
using System.IO.Compression;

namespace DsmSuite.DsmViewer.Model.Files
{
    public abstract class FileWriter
    {
        private readonly FileInfo _fileInfo;
        private bool _compressed;

        protected abstract void WriteContent(Stream stream);

        protected FileWriter(string filename)
        {
            _fileInfo = new FileInfo(filename);
        }

        public void WriteFile(bool compressed)
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
                            WriteContent(entryStream);
                        }
                    }
                }
            }
            else
            {
                using (FileStream fileStream = new FileStream(_fileInfo.FullName, FileMode.Create))
                {
                    WriteContent(fileStream);
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
