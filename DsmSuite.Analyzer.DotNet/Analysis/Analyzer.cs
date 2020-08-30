using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DsmSuite.Analyzer.DotNet.Settings;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Util;
using DsmSuite.Common.Util;
using Mono.Cecil;

namespace DsmSuite.Analyzer.DotNet.Analysis
{
    /// <summary>
    /// .Net code analyzer which uses Mono.Cecil to analyze dependencies between types in .Net binaries
    /// </summary>
    public class Analyzer
    {
        private readonly IDsiModel _model;
        private readonly AnalyzerSettings _analyzerSettings;
        private readonly IProgress<ProgressInfo> _progress;
        private readonly IList<TypeDefinition> _typeList = new List<TypeDefinition>();
        private readonly Dictionary<string, FileInfo> _typeAssemblyInfoList = new Dictionary<string, FileInfo>();
        private readonly List<AssemblyFile> _assemblyFiles = new List<AssemblyFile>();

        public Analyzer(IDsiModel model, AnalyzerSettings analyzerSettings, IProgress<ProgressInfo> progress)
        {
            _model = model;
            _analyzerSettings = analyzerSettings;
            _progress = progress;
        }

        public void Analyze()
        {
            FindAssemblies();
            ReaderParameters readerParameters = DetermineAssemblyReaderParameters();
            FindTypes(readerParameters);
            FindRelations();
            AnalyzerLogger.Flush();
        }

        private void FindAssemblies()
        {
            foreach (string assemblyFilename in Directory.EnumerateFiles(_analyzerSettings.AssemblyDirectory))
            {
                AssemblyFile assemblyFile = new AssemblyFile(assemblyFilename, _model, _analyzerSettings, _progress);

                if (assemblyFile.Exists && assemblyFile.IsAssembly)
                {
                    _assemblyFiles.Add(assemblyFile);
                    UpdateAssemblyProgress(false);
                }
            }
            UpdateAssemblyProgress(true);
        }

        private void FindTypes(ReaderParameters readerParameter)
        {
            foreach (AssemblyFile assemblyFile in _assemblyFiles)
            {
                assemblyFile.FindTypes(readerParameter);
            }
        }

        private void FindRelations()
        {
            foreach (AssemblyFile assemblyFile in _assemblyFiles)
            {
                assemblyFile.FindRelations();
            }
        }

        private ReaderParameters DetermineAssemblyReaderParameters()
        {
            var resolver = new DefaultAssemblyResolver();

            IDictionary<string, bool> paths = new Dictionary<string, bool>();

            foreach (AssemblyFile assemblyFile in _assemblyFiles)
            {
                if (assemblyFile.FileInfo.DirectoryName != null &&
                    paths.ContainsKey(assemblyFile.FileInfo.DirectoryName) == false)
                {
                    paths.Add(assemblyFile.FileInfo.DirectoryName, true);
                    resolver.AddSearchDirectory(assemblyFile.FileInfo.DirectoryName);
                }
            }

            ReaderParameters readerParameters = new ReaderParameters() { AssemblyResolver = resolver };
            return readerParameters;
        }

        private void UpdateAssemblyProgress(bool done)
        {
            ProgressInfo progressInfo = new ProgressInfo();
            progressInfo.ActionText = "Finding assemblies";
            progressInfo.CurrentItemCount = _assemblyFiles.Count;
            progressInfo.TotalItemCount = 0;
            progressInfo.ItemType = "assemblies";
            progressInfo.Percentage = null;
            progressInfo.Done = done;
            _progress?.Report(progressInfo);
        }
    }
}
