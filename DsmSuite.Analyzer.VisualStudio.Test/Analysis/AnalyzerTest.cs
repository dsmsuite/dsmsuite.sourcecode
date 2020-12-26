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
            analyzerSettings.Input.Filename = Path.Combine(TestData.SolutionDirectory, "DsmSuite.sln");
            analyzerSettings.Input.ExternalIncludeDirectories.Clear();
            analyzerSettings.Input.ExternalIncludeDirectories.Add(new ExternalIncludeDirectory { Path = Path.Combine(TestData.TestDataDirectory, "DirExternal"), ResolveAs = "External" });
            analyzerSettings.Input.InterfaceIncludeDirectories.Add(Path.Combine(TestData.TestDataDirectory, "DirInterfaces"));
            analyzerSettings.Analysis.ViewMode = ViewMode.LogicalView;

            DsiModel dataModel = new DsiModel("Test", Assembly.GetExecutingAssembly());

            Analyzer.VisualStudio.Analysis.Analyzer analyzer = new Analyzer.VisualStudio.Analysis.Analyzer(dataModel, analyzerSettings, null);
            analyzer.Analyze();

            Assert.IsTrue(dataModel.TotalElementCount > 0);
            HashSet<string> elementNames = new HashSet<string>();
            Dictionary<string, HashSet<string>> providerNames = new Dictionary<string, HashSet<string>>();
            foreach (IDsiElement element in dataModel.GetElements())
            {
                elementNames.Add(element.Name);

                foreach (IDsiRelation relation in dataModel.GetRelationsOfConsumer(element.Id))
                {
                    if (!providerNames.ContainsKey(element.Name))
                    {
                        providerNames[element.Name] = new HashSet<string>();
                    }

                    IDsiElement provider = dataModel.FindElementById(relation.ProviderId);
                    providerNames[element.Name].Add(provider.Name);
                }
            }

            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderA.ClassA1.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderA.ClassA1.cpp"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderA.ClassA2.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderA.ClassA2.cpp"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderA.ClassA3.h"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderB.ClassB1.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderB.ClassB1.cpp"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderC.ClassC1.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderC.ClassC1.cpp"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderD.ClassD1.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderD.ClassD1.cpp"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderClones.Identical.ClassA1.cpp"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderClones.Identical.CopyClassA1.cpp"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderClones.NotIdentical.ClassA1.cpp"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderIDL.IInterface1.idl"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderIDL.IInterface1.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderIDL.IInterface1_i.c"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderIDL.MyTypeLibrary.tlb"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderIDL.IInterface2.idl"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderIDL.IInterface2.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderIDL.IInterface2_i.c"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderIDL.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.tlb"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).targetver.h"));

            Assert.IsTrue(elementNames.Contains("External.External.h"));

            HashSet<string> classA2ProviderNames = providerNames["DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderA.ClassA2.cpp"];
            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderA.ClassA2.h"));
            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderA.ClassA1.h"));
            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderB.ClassB1.h"));
            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderC.ClassC1.h"));
            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderD.ClassD1.h"));

            Assert.IsTrue(classA2ProviderNames.Contains("External.External.h"));
            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.sln.Analyzers.VisualStudioAnalyzer.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj (lib).FolderA.ClassA3.h"));
        }

        [TestMethod]
        public void TestAnalyzePhysicalView()
        {
            AnalyzerSettings analyzerSettings = AnalyzerSettings.CreateDefault();
            analyzerSettings.Input.Filename = Path.Combine(TestData.SolutionDirectory, "DsmSuite.sln");
            analyzerSettings.Input.RootDirectory = TestData.SolutionDirectory;
            analyzerSettings.Input.ExternalIncludeDirectories.Clear();
            analyzerSettings.Input.ExternalIncludeDirectories.Add(new ExternalIncludeDirectory { Path = Path.Combine(TestData.TestDataDirectory, "DirExternal"), ResolveAs = "External" });
            analyzerSettings.Input.InterfaceIncludeDirectories.Add(Path.Combine(TestData.TestDataDirectory, "DirInterfaces"));
            analyzerSettings.Analysis.ViewMode = ViewMode.PhysicalView;

            DsiModel dataModel = new DsiModel("Test", Assembly.GetExecutingAssembly());

            Analyzer.VisualStudio.Analysis.Analyzer analyzer = new Analyzer.VisualStudio.Analysis.Analyzer(dataModel, analyzerSettings, null);
            analyzer.Analyze();

            HashSet<string> elementNames = new HashSet<string>();
            Dictionary<string, HashSet<string>> providerNames = new Dictionary<string, HashSet<string>>();
            foreach (IDsiElement element in dataModel.GetElements())
            {
                elementNames.Add(element.Name);

                foreach (IDsiRelation relation in dataModel.GetRelationsOfConsumer(element.Id))
                {
                    if (!providerNames.ContainsKey(element.Name))
                    {
                        providerNames[element.Name] = new HashSet<string>();
                    }

                    IDsiElement provider = dataModel.FindElementById(relation.ProviderId);
                    providerNames[element.Name].Add(provider.Name);
                }
            }

            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirA.ClassA1.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirA.ClassA1.cpp"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirA.ClassA2.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirA.ClassA2.cpp"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirA.ClassA3.h"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirB.ClassB1.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirB.ClassB1.cpp"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirC.ClassC1.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirC.ClassC1.cpp"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirD.ClassD1.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirD.ClassD1.cpp"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirClones.Identical.ClassA1.cpp"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirClones.Identical.CopyClassA1.cpp"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirClones.NotIdentical.ClassA1.cpp"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirIDL.IInterface1.idl"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.IdlOutput.IInterface1.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.IdlOutput.IInterface1_i.c"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.IdlOutput.MyTypeLibrary.tlb"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirIDL.IInterface2.idl"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.IdlOutput.IInterface2.h"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.IdlOutput.IInterface2_i.c"));
            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.IdlOutput.DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.tlb"));

            Assert.IsTrue(elementNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.targetver.h"));

            Assert.IsTrue(elementNames.Contains("External.External.h"));

            HashSet<string> classA2ProviderNames = providerNames["DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirA.ClassA2.cpp"];
            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirA.ClassA2.h"));
            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirA.ClassA1.h"));
            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirB.ClassB1.h"));
            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirC.ClassC1.h"));
            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirD.ClassD1.h"));

            Assert.IsTrue(classA2ProviderNames.Contains("External.External.h"));

            Assert.IsTrue(classA2ProviderNames.Contains("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.DirA.ClassA3.h"));
        }
    }
}
