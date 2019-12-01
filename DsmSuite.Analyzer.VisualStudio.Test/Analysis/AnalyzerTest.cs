using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Analyzer.VisualStudio.Test.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.VisualStudio.Test.Analysis
{
    [TestClass]
    public class AnalyzerTest
    {
        [TestMethod]
        public void TestAnalyzeLogicalView()
        {
            AnalyzerSettings analyzerSettings = AnalyzerSettings.CreateDefault();
            analyzerSettings.ViewMode = ViewMode.LogicalView;
            analyzerSettings.SolutionGroups[0].SolutionFilenames.Add(Path.Combine(TestData.SolutionDirectory, "DsmSuite.sln"));
            analyzerSettings.ExternalIncludeDirectories.Add(new ExternalIncludeDirectory { Path=Path.Combine(TestData.TestDataDirectory, "DirExternal"), ResolveAs= "External" });
            analyzerSettings.InterfaceIncludeDirectories.Add(Path.Combine(TestData.TestDataDirectory, "DirInterfaces"));
            DataModel dataModel = new DataModel("Test", Assembly.GetExecutingAssembly());

            Analyzer.VisualStudio.Analysis.Analyzer analyzer = new Analyzer.VisualStudio.Analysis.Analyzer(dataModel, analyzerSettings);
            analyzer.Analyze();

            Assert.IsTrue(dataModel.TotalElementCount > 0);
            HashSet<string> elementNames = new HashSet<string>();
            Dictionary<string, HashSet<string>> providerNames = new Dictionary<string, HashSet<string>>();
            foreach (IElement element in dataModel.Elements)
            {
                elementNames.Add(element.Name);

                foreach (IRelation relation in dataModel.GetProviderRelations(element))
                {
                    if (!providerNames.ContainsKey(element.Name))
                    {
                        providerNames[element.Name] = new HashSet<string>();
                    }

                    IElement provider = dataModel.FindElement(relation.ProviderId);
                    providerNames[element.Name].Add(provider.Name);
                }
            }

            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderA.ClassA1.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderA.ClassA1.cpp"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderA.ClassA2.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderA.ClassA2.cpp"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderA.ClassA3.h"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderB.ClassB1.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderB.ClassB1.cpp"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderC.ClassC1.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderC.ClassC1.cpp"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderD.ClassD1.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderD.ClassD1.cpp"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderClones.Identical.ClassA1.cpp"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderClones.Identical.CopyClassA1.cpp"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderClones.NotIdentical.ClassA1.cpp"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderIDL.IInterface1.idl"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderIDL.IInterface1.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderIDL.IInterface1_i.c"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderIDL.MyTypeLibrary.tlb"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderIDL.IInterface2.idl"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderIDL.IInterface2.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderIDL.IInterface2_i.c"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderIDL.DsmSuite.Analyzer.VisualStudio.Test.Data.tlb"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).targetver.h"));

            Assert.IsTrue(elementNames.Contains("External.External.h"));

            HashSet<string> classA2ProviderNames = providerNames["DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderA.ClassA2.cpp"];
            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderA.ClassA2.h"));
            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderA.ClassA1.h"));
            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderB.ClassB1.h"));
            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderC.ClassC1.h"));
            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderD.ClassD1.h"));

            Assert.IsTrue(classA2ProviderNames.Contains("External.External.h"));

            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.vcxproj (lib).FolderA.ClassA3.h"));
        }

        [TestMethod]
        public void TestAnalyzePhysicalView()
        {
            AnalyzerSettings analyzerSettings = AnalyzerSettings.CreateDefault();
            analyzerSettings.ViewMode = ViewMode.PhysicalView;
            analyzerSettings.SolutionGroups[0].SolutionFilenames.Add(Path.Combine(TestData.SolutionDirectory, "DsmSuite.sln"));
            analyzerSettings.ExternalIncludeDirectories.Add(new ExternalIncludeDirectory { Path = Path.Combine(TestData.TestDataDirectory, "DirExternal"), ResolveAs = "External" });
            analyzerSettings.InterfaceIncludeDirectories.Add(Path.Combine(TestData.TestDataDirectory, "DirInterfaces"));
            analyzerSettings.RootDirectory = TestData.SolutionDirectory;

            DataModel dataModel = new DataModel("Test", Assembly.GetExecutingAssembly());

            Analyzer.VisualStudio.Analysis.Analyzer analyzer = new Analyzer.VisualStudio.Analysis.Analyzer(dataModel, analyzerSettings);
            analyzer.Analyze();

            HashSet<string> elementNames = new HashSet<string>();
            Dictionary<string, HashSet<string>> providerNames = new Dictionary<string, HashSet<string>>();
            foreach (IElement element in dataModel.Elements)
            {
                elementNames.Add(element.Name);

                foreach (IRelation relation in dataModel.GetProviderRelations(element))
                {
                    if (!providerNames.ContainsKey(element.Name))
                    {
                        providerNames[element.Name] = new HashSet<string>();
                    }

                    IElement provider = dataModel.FindElement(relation.ProviderId);
                    providerNames[element.Name].Add(provider.Name);
                }
            }

            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirA.ClassA1.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirA.ClassA1.cpp"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirA.ClassA2.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirA.ClassA2.cpp"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirA.ClassA3.h"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirB.ClassB1.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirB.ClassB1.cpp"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirC.ClassC1.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirC.ClassC1.cpp"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirD.ClassD1.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirD.ClassD1.cpp"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirClones.Identical.ClassA1.cpp"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirClones.Identical.CopyClassA1.cpp"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirClones.NotIdentical.ClassA1.cpp"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirIDL.IInterface1.idl"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.IdlOutput.IInterface1.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.IdlOutput.IInterface1_i.c"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.IdlOutput.MyTypeLibrary.tlb"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirIDL.IInterface2.idl"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.IdlOutput.IInterface2.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.IdlOutput.IInterface2_i.c"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.IdlOutput.DsmSuite.Analyzer.VisualStudio.Test.Data.tlb"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.targetver.h"));

            Assert.IsTrue(elementNames.Contains("External.External.h"));

            HashSet<string> classA2ProviderNames = providerNames["DsmSuite.Analyzer.VisualStudio.Test.Data.DirA.ClassA2.cpp"];
            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirA.ClassA2.h"));
            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirA.ClassA1.h"));
            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirB.ClassB1.h"));
            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirC.ClassC1.h"));
            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirD.ClassD1.h"));

            Assert.IsTrue(classA2ProviderNames.Contains("External.External.h"));

            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.DirA.ClassA3.h"));
        }
    }
}
