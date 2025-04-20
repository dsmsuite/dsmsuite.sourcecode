namespace DsmSuite.Common.Util
{
    public static class FilePath
    {
        public static string ResolveFile(string path, string filename)
        {
            return Resolve(path, filename);
        }

        public static List<string> ResolveFiles(string path, IEnumerable<string> filenames)
        {
            List<string> resolvedFiles = new List<string>();
            foreach (string filename in filenames)
            {
                resolvedFiles.Add(Resolve(path, filename));
            }

            return resolvedFiles;
        }

        private static string Resolve_old(string path, string filename)
        {
            string absoluteFilename = "";
            string result = "";
            string[] assemblyFolders = filename.Split(';');
            int size = assemblyFolders.Length;
            foreach (string folder in assemblyFolders)
            {
                absoluteFilename = Path.GetFullPath(folder);
                if (!File.Exists(absoluteFilename))
                {
                    absoluteFilename = Path.GetFullPath(Path.Combine(path, folder));
                }
                result += absoluteFilename + ";";
                Logger.LogInfo("Resolve file: path=" + path + " file=" + filename + " as file=" + absoluteFilename);
            }
            if (result.Length > 0)
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result;
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