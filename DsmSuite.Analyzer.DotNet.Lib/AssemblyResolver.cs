using Mono.Cecil;
using System.Collections.Generic;

namespace DsmSuite.Analyzer.DotNet.Lib
{
    public class AssemblyResolver
    {
        HashSet<string> _paths = new HashSet<string>();

        public AssemblyResolver()
        {

        }

        public void AddSearchPath(string path)
        {
            if (path != null && !_paths.Contains(path))
            {
                _paths.Add(path);
            }
        }

        public ReaderParameters GetAssemblyReaderParameters()
        {
            var resolver = new DefaultAssemblyResolver();

            IDictionary<string, bool> paths = new Dictionary<string, bool>();

            foreach (string  path in _paths)
            {
                   resolver.AddSearchDirectory(path);
            }

            return new ReaderParameters() { AssemblyResolver = resolver };
        }
    }
}
