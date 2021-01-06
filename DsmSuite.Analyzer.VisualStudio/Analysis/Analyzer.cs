using System;
using System.IO;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Analyzer.VisualStudio.VisualStudio;
using DsmSuite.Common.Util;
using DsmSuite.Analyzer.DotNet.Lib;
using DsmSuite.Analyzer.VisualStudio.Utils;

namespace DsmSuite.Analyzer.VisualStudio.Analysis
{
    public class Analyzer
    {
        private readonly IDsiModel _model;
        private readonly AnalyzerSettings _analyzerSettings;
        private readonly SolutionFile _solutionFile;
        private readonly IProgress<ProgressInfo> _progress;
        private readonly ViewBase _view;

        public Analyzer(IDsiModel model, AnalyzerSettings analyzerSettings, IProgress<ProgressInfo> progress)
        {
            _model = model;
            _analyzerSettings = analyzerSettings;
            _progress = progress;
            _solutionFile = new SolutionFile(analyzerSettings.Input.Filename, _analyzerSettings, progress);

            if (_analyzerSettings.Analysis.ViewMode == ViewMode.SolutionView)
            {
                _view = new SolutionView(_solutionFile, _analyzerSettings);
            }
            else
            {
                _view = new DirectoryView(_solutionFile, _analyzerSettings);
            }
        }

        public void Analyze()
        {
            RegisterInterfaceFiles();
            AnalyzeSolution();
            RegisterDotNetTypes();
            RegisterDotNetRelations();
            RegisterSourceFiles();
            RegisterDirectIncludeRelations();
            RegisterGeneratedFileRelations();
            WriteFoundProjects();
            AnalyzerLogger.Flush();
        }

        private void RegisterInterfaceFiles()
        {
            foreach (string interfaceDirectory in _analyzerSettings.Input.InterfaceIncludeDirectories)
            {
                DirectoryInfo interfaceDirectoryInfo = new DirectoryInfo(interfaceDirectory);
                RegisterInterfaceFiles(interfaceDirectoryInfo);
            }
        }

        private void RegisterInterfaceFiles(DirectoryInfo interfaceDirectoryInfo)
        {
            if (interfaceDirectoryInfo.Exists)
            {
                foreach (FileInfo interfaceFile in interfaceDirectoryInfo.EnumerateFiles())
                {
                    if (interfaceFile.Exists)
                    {
                        _view.RegisterInterfaceFile(interfaceFile.FullName);
                    }
                    else
                    {
                        Logger.LogError($"Interface file {interfaceFile.FullName} does not exist");
                    }
                }

                foreach (DirectoryInfo subDirectoryInfo in interfaceDirectoryInfo.EnumerateDirectories())
                {
                    RegisterInterfaceFiles(subDirectoryInfo);
                }
            }
            else
            {
                Logger.LogError($"Interface directory {interfaceDirectoryInfo.FullName} does not exist");
            }
        }

        private void AnalyzeSolution()
        {
            _solutionFile.Analyze();
        }

        private void RegisterDotNetTypes()
        {
            foreach (ProjectFileBase visualStudioProject in _solutionFile.Projects)
            {
                foreach (DotNetType type in visualStudioProject.DotNetTypes)
                {
                    string name = _view.GetDotNetTypeName(visualStudioProject, type.Name);
                    _model.AddElement(name, type.Type, "");
                }
            }
        }

        private void RegisterDotNetRelations()
        {
            foreach (ProjectFileBase visualStudioProject in _solutionFile.Projects)
            {
                foreach (DotNetRelation relation in visualStudioProject.DotNetRelations)
                {
                    string consumerName = _view.GetDotNetTypeName(visualStudioProject, relation.ConsumerName);
                    string providerName = _view.GetDotNetTypeName(visualStudioProject, relation.ProviderName);
                    _model.AddRelation(consumerName, providerName, relation.Type, 1, null);
                }
            }
        }

        private void RegisterSourceFiles()
        {
            int processedSourceFiles = 0;
            foreach (ProjectFileBase visualStudioProject in _solutionFile.Projects)
            {
                foreach (SourceFile sourceFile in visualStudioProject.SourceFiles)
                {
                    RegisterSourceFile(visualStudioProject, sourceFile);

                    processedSourceFiles++;
                    UpdateSourceFileProgress("Registering source files", processedSourceFiles, _solutionFile.TotalSourceFiles);
                }
            }
        }

        private void RegisterSourceFile(ProjectFileBase visualStudioProject, SourceFile sourceFile)
        {
            Logger.LogInfo("Source file registered: " + sourceFile.Name);

            if (sourceFile.SourceFileInfo.Exists)
            {
                AnalyzerLogger.LogFileFoundInVisualStudioProject(sourceFile.Name, visualStudioProject.ProjectName);

                _view.RegisterSourceFile(visualStudioProject, sourceFile);
                string name = _view.GetName(visualStudioProject, sourceFile);
                _model.AddElement(name, sourceFile.FileType, sourceFile.SourceFileInfo.FullName);
            }
            else
            {
                AnalyzerLogger.LogErrorFileNotFound(sourceFile.Name, visualStudioProject.ProjectName);
            }
        }

        private void RegisterDirectIncludeRelations()
        {
            int processedSourceFiles = 0;
            foreach (ProjectFileBase visualStudioProject in _solutionFile.Projects)
            {
                foreach (SourceFile sourceFile in visualStudioProject.SourceFiles)
                {
                    foreach (string includedFile in sourceFile.Includes)
                    {
                        RegisterDirectIncludeRelation(visualStudioProject, sourceFile, includedFile);
                    }

                    processedSourceFiles++;
                    UpdateSourceFileProgress("Registering source file includes", processedSourceFiles, _solutionFile.TotalSourceFiles);
                }
            }
        }

        private void RegisterDirectIncludeRelation(ProjectFileBase visualStudioProject, SourceFile sourceFile, string includedFile)
        {
            Logger.LogInfo("Include relation registered: " + sourceFile.Name + " -> " + includedFile);

            string consumerName = _view.GetName(visualStudioProject, sourceFile);
            string providerName = _view.ResolveProvider(visualStudioProject, includedFile);

            if (providerName != null)
            {
                if (providerName.Length > 0)
                {
                    FileInfo includedFileInfo = new FileInfo(includedFile);
                    string type = includedFileInfo.Extension.Substring(1);
                    _model.AddElement(providerName, type, includedFile);
                    _model.AddRelation(consumerName, providerName, "include", 1, null);
                }
                else
                {
                    _model.IgnoreRelation(consumerName, includedFile, "include");
                }
            }
            else
            {
                _model.SkipRelation(consumerName, includedFile, "include");
            }
        }

        private void RegisterGeneratedFileRelations()
        {
            foreach (ProjectFileBase visualStudioProject in _solutionFile.Projects)
            {
                foreach (GeneratedFileRelation relation in visualStudioProject.GeneratedFileRelations)
                {
                    Logger.LogInfo("Generated file relation registered: " + relation.Consumer.Name + " -> " + relation.Provider.Name);

                    string consumerName = _view.GetName(visualStudioProject, relation.Consumer);
                    string providerName = _view.GetName(visualStudioProject, relation.Provider);
                    _model.AddRelation(consumerName, providerName, "generated", 1, null);
                }
            }
        }
        
        private void WriteFoundProjects()
        {
            foreach (ProjectFileBase project in _solutionFile.Projects)
            {
                string projectName = project.ProjectFileInfo.FullName;
                string status = project.Success ? "ok" : "failed";
                AnalyzerLogger.LogProjectStatus(projectName, status);
            }
        }

        private void UpdateSourceFileProgress(string text, int currentItemCount, int totalItemCount)
        {
            ProgressInfo progressInfo = new ProgressInfo();
            progressInfo.ActionText = text;
            progressInfo.CurrentItemCount = currentItemCount;
            progressInfo.TotalItemCount = totalItemCount;
            progressInfo.ItemType = "files";
            progressInfo.Percentage = currentItemCount * 100 / totalItemCount;
            progressInfo.Done = currentItemCount == totalItemCount;
            _progress?.Report(progressInfo);
        }
    }
}
