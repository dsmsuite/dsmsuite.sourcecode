using System;
using System.Collections.Generic;
using System.IO;
using DsmSuite.Analyzer.DotNet.Lib;
using DsmSuite.Analyzer.DotNet.Settings;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.DotNet.Analysis
{
    public class Analyzer
    {
        private readonly IDsiModel _model;
        private readonly AnalyzerSettings _analyzerSettings;
        private readonly IProgress<ProgressInfo> _progress;
        private readonly List<BinaryFile> _assemblyFiles = new List<BinaryFile>();
        private readonly DotNetResolver _resolver = new DotNetResolver();

        public Analyzer(IDsiModel model, AnalyzerSettings analyzerSettings, IProgress<ProgressInfo> progress)
        {
            _model = model;
            _analyzerSettings = analyzerSettings;
            _progress = progress;
        }

        public void Analyze()
        {
            FindAssemblies();
            FindTypes();
            FindRelations();
        }

        private void FindAssemblies()
        {
            foreach (string assemblyFilename in Directory.EnumerateFiles(_analyzerSettings.Input.AssemblyDirectory))
            {
                BinaryFile assemblyFile = new BinaryFile(assemblyFilename, _progress);

                if (assemblyFile.Exists && assemblyFile.IsAssembly)
                {
                    _assemblyFiles.Add(assemblyFile);
                    _resolver.AddSearchPath(assemblyFile);
                    UpdateAssemblyProgress(false);
                }
            }
            UpdateAssemblyProgress(true);
        }

        private void FindTypes()
        {
            foreach (BinaryFile assemblyFile in _assemblyFiles)
            {
                assemblyFile.FindTypes(_resolver);
                foreach (DotNetType type in assemblyFile.Types)
                {
                    _model.AddElement(type.Name, type.Type, null);
                }
            }
        }

        private void FindRelations()
        {
            foreach (BinaryFile assemblyFile in _assemblyFiles)
            {
                assemblyFile.FindRelations();
                foreach (DotNetRelation relation in assemblyFile.Relations)
                {
                    _model.AddRelation(relation.ConsumerName, relation.ProviderName, relation.Type, 1, null);
                }
            }
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
