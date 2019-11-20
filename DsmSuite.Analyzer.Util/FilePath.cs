using System.Collections.Generic;
using System.IO;

namespace DsmSuite.Analyzer.Util
{
    public static class FilePath
    {
        public static string ResolveFile(string path, string filename)
        {
            return Resolve(path, filename);
        }

        public static List<string> ResolveFiles(string path, List<string> filenames)
        {
            List < string > resolvedFiles = new List<string>();
            foreach(string filename in filenames)
            {
                resolvedFiles.Add(Resolve(path, filename));
            }

            return resolvedFiles;
        }

        private static string Resolve(string path, string filename)
        {
            string absoluteFilename = Path.GetFullPath(filename);
            if (!File.Exists(absoluteFilename))
            {
                absoluteFilename = Path.GetFullPath(Path.Combine(path, filename));
            }

            Logger.LogInfo("Resolve file: path=" + path + " file=" + filename + " as file=" + absoluteFilename);
            return absoluteFilename;
        }
    }
}
