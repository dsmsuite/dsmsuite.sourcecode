using System;
using System.Diagnostics;
using DsmSuite.Analyzer.Data;
using DsmSuite.Analyzer.Util;
using EA;

namespace DsmSuite.Analyzer.Uml.Analysis
{
    public class Analyzer
    {
        private readonly IDataModel _model;
        private readonly AnalyzerSettings _analyzerSettings;
        private readonly EA.Repository _repository;

        public Analyzer(IDataModel model, AnalyzerSettings analyzerSettings)
        {
            _model = model;
            _analyzerSettings = analyzerSettings;
            _repository = new EA.Repository();
        }

        public void Analyze()
        {
            Logger.LogUserMessage("Analyzing file=" + _analyzerSettings.InputFilename);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                bool success = _repository.OpenFile(_analyzerSettings.InputFilename);
                if (success)
                {
                    for (short index = 0; index < _repository.Models.Count; index++)
                    {
                        EA.Package model = (EA.Package)_repository.Models.GetAt(index);
                        FindPackageElements(model);
                    }

                    for (short index = 0; index < _repository.Models.Count; index++)
                    {
                        EA.Package model = (EA.Package)_repository.Models.GetAt(index);
                        FindPackageRelations(model);
                    }

                    _repository.CloseFile();
                }

                _repository.Exit();
            }
            catch (Exception e)
            {
                Logger.LogException(e, "reading EA model failed");
            }

            Process currentProcess = Process.GetCurrentProcess();
            const long million = 1000000;
            long peakPagedMemMb = currentProcess.PeakPagedMemorySize64 / million;
            long peakVirtualMemMb = currentProcess.PeakVirtualMemorySize64 / million;
            long peakWorkingSetMb = currentProcess.PeakWorkingSet64 / million;
            Logger.LogUserMessage($" peak physical memory usage {peakWorkingSetMb:0.000}MB");
            Logger.LogUserMessage($" peak paged memory usage    {peakPagedMemMb:0.000}MB");
            Logger.LogUserMessage($" peak virtual memory usage  {peakVirtualMemMb:0.000}MB");

            stopWatch.Stop();
            Logger.LogUserMessage($" total elapsed time={stopWatch.Elapsed}");
        }

        private void FindPackageElements(EA.Package package)
        {
            for (short index = 0; index < package.Packages.Count; index++)
            {
                EA.Package subpackage = (EA.Package)package.Packages.GetAt(index);
                FindPackageElements(subpackage);
            }

            for (short index = 0; index < package.Elements.Count; index++)
            {
                EA.Element element = (EA.Element)package.Elements.GetAt(index);
                FindNestedElements(element);
            }
        }

        private void FindNestedElements(EA.Element element)
        {
            RegisterElement(element);

            for (short index = 0; index < element.Elements.Count; index++)
            {
                EA.Element nestedElement = (EA.Element)element.Elements.GetAt(index);
                FindNestedElements(nestedElement);
            }
        }

        private void FindPackageRelations(EA.Package package)
        {
            for (short index = 0; index < package.Packages.Count; index++)
            {
                EA.Package subpackage = (EA.Package)package.Packages.GetAt(index);
                FindPackageRelations(subpackage);
            }

            for (short index = 0; index < package.Elements.Count; index++)
            {
                EA.Element element = (EA.Element)package.Elements.GetAt(index);
                FindElementRelations(element);
            }
        }

        private void FindElementRelations(EA.Element element)
        {
            for (short index = 0; index < element.Connectors.Count; index++)
            {
                EA.Connector connector = (EA.Connector)element.Connectors.GetAt(index);

                RegisterRelation(connector);
            }

            for (short index = 0; index < element.Elements.Count; index++)
            {
                EA.Element nestedElement = (EA.Element)element.Elements.GetAt(index);
                FindElementRelations(nestedElement);
            }
        }

        private void RegisterElement(EA.Element element)
        {
            Logger.LogInfo("Register model element:" + ExtractUniqueName(element));
            _model.AddElement(ExtractUniqueName(element), element.Type, _analyzerSettings.InputFilename);
        }

        private void RegisterRelation(EA.Connector connector)
        {
            EA.Element client = _repository.GetElementByID(connector.ClientID);
            EA.Element supplier = _repository.GetElementByID(connector.SupplierID);

            if (client != null && supplier != null)
            {
                string consumerName = ExtractUniqueName(client);
                string providerName = ExtractUniqueName(supplier);

                RegisterRelation(connector, consumerName, providerName);
            }
        }

        private void RegisterRelation(Connector connector, string consumerName, string providerName)
        {
            _model.AddRelation(consumerName, providerName, connector.Type, 1, "model");
        }

        private string ExtractUniqueName(EA.Element element)
        {
            int packageId = element.PackageID;

            string name = element.Name;
            while (packageId > 0)
            {
                EA.Package parentPackage = _repository.GetPackageByID(packageId);
                name = parentPackage.Name + "." + name;

                packageId = parentPackage.ParentID;
            }

            return name;
        }
    }
}
