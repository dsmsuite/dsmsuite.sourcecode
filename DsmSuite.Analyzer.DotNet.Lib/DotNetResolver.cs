using Mono.Cecil;
using System.Collections.Generic;

namespace DsmSuite.Analyzer.DotNet.Lib
{
    public class DotNetResolver
    {
        HashSet<string> _paths = new HashSet<string>();

        public DotNetResolver()
        {

        }

        public void AddSearchPath(BinaryFile assemblyFile)
        {
            string path = assemblyFile.FileInfo.DirectoryName;
            if (path != null && !_paths.Contains(path))
            {
                _paths.Add(path);
            }
        }

        public ReaderParameters ReaderParameters
        {
            get
            {
                DefaultAssemblyResolver resolver = new DefaultAssemblyResolver();

                IDictionary<string, bool> paths = new Dictionary<string, bool>();

                foreach (string path in _paths)
                {
                    resolver.AddSearchDirectory(path);
                }

                return new ReaderParameters() { AssemblyResolver = resolver };
            }
        }
    }
}
