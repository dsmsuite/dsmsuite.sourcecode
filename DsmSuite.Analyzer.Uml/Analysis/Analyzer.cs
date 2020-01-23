using System;
using System.Diagnostics;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Uml.Settings;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Uml.Analysis
{
    public class Analyzer
    {
        private readonly IDsiModel _model;
        private readonly AnalyzerSettings _analyzerSettings;
        private readonly EA.Repository _repository;

        public Analyzer(IDsiModel model, AnalyzerSettings analyzerSettings)
        {
            _model = model;
            _analyzerSettings = analyzerSettings;
            _repository = new EA.Repository();
        }

        public void Analyze(IProgress<ProgressInfo> progress)
        {
            try
            {
                bool success = _repository.OpenFile(_analyzerSettings.InputFilename);
                if (success)
                {
                    int elementCount = 0;
                    for (short index = 0; index < _repository.Models.Count; index++)
                    {
                        EA.Package model = (EA.Package) _repository.Models.GetAt(index);
                        FindPackageElements(model, ref elementCount, progress);
                    }
                    UpdateProgress(progress, "Reading UML elements", elementCount, "elements", true);

                    int relationCount = 0;
                    for (short index = 0; index < _repository.Models.Count; index++)
                    {
                        EA.Package model = (EA.Package) _repository.Models.GetAt(index);
                        FindPackageRelations(model, ref relationCount, progress);
                    }
                    UpdateProgress(progress, "Reading UML relations", relationCount, "relations", true);

                    _repository.CloseFile();
                }

                _repository.Exit();
            }
            catch (Exception e)
            {
                Logger.LogException($"Reading EA model failed file={_analyzerSettings.InputFilename}", e);
            }
        }

        private void FindPackageElements(EA.Package package, ref int elementCount, IProgress<ProgressInfo> progress)
        {
            for (short index = 0; index < package.Packages.Count; index++)
            {
                EA.Package subpackage = (EA.Package) package.Packages.GetAt(index);
                FindPackageElements(subpackage, ref elementCount, progress);
            }

            for (short index = 0; index < package.Elements.Count; index++)
            {
                EA.Element element = (EA.Element) package.Elements.GetAt(index);
                FindNestedElements(element, ref elementCount, progress);
            }
        }

        private void FindNestedElements(EA.Element element, ref int elementCount, IProgress<ProgressInfo> progress)
        {
            RegisterElement(element);
            elementCount++;
            UpdateProgress(progress, "Reading UML elements", elementCount, "elements", false);

            for (short index = 0; index < element.Elements.Count; index++)
            {
                EA.Element nestedElement = (EA.Element) element.Elements.GetAt(index);
                FindNestedElements(nestedElement, ref elementCount, progress);
            }
        }

        private void FindPackageRelations(EA.Package package, ref int relationCount, IProgress<ProgressInfo> progress)
        {
            for (short index = 0; index < package.Packages.Count; index++)
            {
                EA.Package subpackage = (EA.Package) package.Packages.GetAt(index);
                FindPackageRelations(subpackage, ref relationCount, progress);
            }

            for (short index = 0; index < package.Elements.Count; index++)
            {
                EA.Element element = (EA.Element) package.Elements.GetAt(index);
                FindElementRelations(element, ref relationCount, progress);
            }
        }

        private void FindElementRelations(EA.Element element, ref int relationCount, IProgress<ProgressInfo> progress)
        {
            for (short index = 0; index < element.Connectors.Count; index++)
            {
                EA.Connector connector = (EA.Connector) element.Connectors.GetAt(index);

                RegisterRelation(connector);
                relationCount++;
                UpdateProgress(progress, "Reading UML relations", relationCount, "relations", false);
            }

            for (short index = 0; index < element.Elements.Count; index++)
            {
                EA.Element nestedElement = (EA.Element) element.Elements.GetAt(index);
                FindElementRelations(nestedElement, ref relationCount, progress);
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

        private void RegisterRelation(EA.Connector connector, string consumerName, string providerName)
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

        protected void UpdateProgress(IProgress<ProgressInfo> progress, string actionText, int itemCount, string itemType, bool done)
        {
            if (progress != null)
            {
                ProgressInfo progressInfoInfo = new ProgressInfo
                {
                    ActionText = actionText,
                    TotalItemCount = 0,
                    CurrentItemCount = itemCount,
                    ItemType = itemType,
                    Percentage = null,
                    Done = done
                };

                progress.Report(progressInfoInfo);
            }
        }
    }
}
