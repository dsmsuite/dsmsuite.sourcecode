using System;
using System.Collections.Generic;
using System.IO;
using DsmSuite.Analyzer.Util;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.VisualStudio.VisualStudio
{
    public class IncludeResolveStrategy
    {
        private readonly IReadOnlyCollection<string> _projectIncludeDirectories;
        private readonly IReadOnlyCollection<string> _systemIncludeDirectories;

        public IncludeResolveStrategy(IReadOnlyCollection<string> projectIncludeDirectories, IReadOnlyCollection<string> systemIncludeDirectories)
        {
            _projectIncludeDirectories = projectIncludeDirectories;
            _systemIncludeDirectories = systemIncludeDirectories;
        }

        public IReadOnlyCollection<string> ProjectIncludeDirectories => _projectIncludeDirectories;

        public IReadOnlyCollection<string> SystemIncludeDirectories => _systemIncludeDirectories;

        public string Resolve(string sourceFile, string relativeIncludeFilename)
        {
            string resolvedIncludeFilename = null;

            try
            {
                if (File.Exists(relativeIncludeFilename))
                {
                    // The include path is an absolute path, so no resolving is required
                    resolvedIncludeFilename = relativeIncludeFilename;
                }
                else
                {
                    //Like Visual Studio look for headers in this order:
                    // -In the current source directory.
                    // -In the Additional Include Directories in the project properties(under C++ | General).
                    // -In the Visual Studio C++ Include directories under Tools → Options → Projects and Solutions → VC++ Directories.
                    string sourceDirectory = Path.GetDirectoryName(sourceFile);
                    if (sourceDirectory != null)
                    {
                        ResolveUsingIncludeDirectory(relativeIncludeFilename, sourceDirectory, ref resolvedIncludeFilename);
                        ResolveUsingIncludeDirectories(relativeIncludeFilename, _projectIncludeDirectories, false, ref resolvedIncludeFilename);
                        ResolveUsingIncludeDirectories(relativeIncludeFilename, _systemIncludeDirectories, true, ref resolvedIncludeFilename);
                    }
                }

            }
            catch (Exception e)
            {
                Logger.LogException($"Resolve failed include={relativeIncludeFilename}", e);
            }

            if (resolvedIncludeFilename == null)
            {
                AnalyzerLogger.LogErrorIncludeFileNotFound(relativeIncludeFilename, sourceFile);
            }

            return resolvedIncludeFilename;
        }

        private void ResolveUsingIncludeDirectories(string relativeIncludeFilename, IEnumerable<string> includeDirectories, bool nested, ref string resolvedIncludeFilename)
        {
            foreach (string includeDirectory in includeDirectories)
            {
                if (Directory.Exists(includeDirectory))
                {
                    ResolveUsingIncludeDirectory(relativeIncludeFilename, includeDirectory, ref resolvedIncludeFilename);

                    if (nested)
                    {
                        List<string> includeSubDirectories = new List<string>();
                        DirectoryInfo includeDirectoryInfo = new DirectoryInfo(includeDirectory);
                        foreach (DirectoryInfo includeSubDirectoryInfo in includeDirectoryInfo.EnumerateDirectories())
                        {
                            includeSubDirectories.Add(includeSubDirectoryInfo.FullName);
                        }

                        ResolveUsingIncludeDirectories(relativeIncludeFilename, includeSubDirectories, true,
                            ref resolvedIncludeFilename);
                    }
                }
            }
        }

        private void ResolveUsingIncludeDirectory(string relativeIncludeFilename, string includeDirectory, ref string resolvedIncludeFilename)
        {
            if (resolvedIncludeFilename == null)
            {
                if (Directory.Exists(includeDirectory))
                {
                    string absoluteIncludeFilename = GetAbsolutePath(includeDirectory, relativeIncludeFilename);
                    if (File.Exists(absoluteIncludeFilename))
                    {
                        resolvedIncludeFilename = absoluteIncludeFilename;
                    }
                }
            }
        }

        private string GetAbsolutePath(string dir, string file)
        {
            return Path.GetFullPath(Path.Combine(dir, file));
        }
    }
}