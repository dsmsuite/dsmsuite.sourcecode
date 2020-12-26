using System;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Uml.Settings;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Uml.Analysis
{
    public class Analyzer
    {
        private readonly IDsiModel _model;
        private readonly AnalyzerSettings _analyzerSettings;
        private readonly IProgress<ProgressInfo> _progress;
        private readonly EA.Repository _repository;
        private int _elementCount;
        private int _relationCount;

        public Analyzer(IDsiModel model, AnalyzerSettings analyzerSettings, IProgress<ProgressInfo> progress)
        {
            _model = model;
            _analyzerSettings = analyzerSettings;
            _progress = progress;
            _repository = new EA.Repository();
        }

        public void Analyze()
        {
            try
            {
                bool success = _repository.OpenFile(_analyzerSettings.Input.Filename);
                if (success)
                {
                    _elementCount = 0;
                    for (short index = 0; index < _repository.Models.Count; index++)
                    {
                        EA.Package model = (EA.Package) _repository.Models.GetAt(index);
                        FindPackageElements(model);
                    }
                    UpdateElementProgress(true);

                    _relationCount = 0;
                    for (short index = 0; index < _repository.Models.Count; index++)
                    {
                        EA.Package model = (EA.Package) _repository.Models.GetAt(index);
                        FindPackageRelations(model);
                    }
                    UpdateRelationProgress(true);

                    _repository.CloseFile();
                }

                _repository.Exit();
            }
            catch (Exception e)
            {
                Logger.LogException($"Reading EA model failed file={_analyzerSettings.Input.Filename}", e);
            }
        }

        private void FindPackageElements(EA.Package package)
        {
            for (short index = 0; index < package.Packages.Count; index++)
            {
                EA.Package subpackage = (EA.Package) package.Packages.GetAt(index);
                FindPackageElements(subpackage);
            }

            for (short index = 0; index < package.Elements.Count; index++)
            {
                EA.Element element = (EA.Element) package.Elements.GetAt(index);
                FindNestedElements(element);
            }
        }

        private void FindNestedElements(EA.Element element)
        {
            RegisterElement(element);

            for (short index = 0; index < element.Elements.Count; index++)
            {
                EA.Element nestedElement = (EA.Element) element.Elements.GetAt(index);
                FindNestedElements(nestedElement);
            }
        }

        private void FindPackageRelations(EA.Package package)
        {
            for (short index = 0; index < package.Packages.Count; index++)
            {
                EA.Package subpackage = (EA.Package) package.Packages.GetAt(index);
                FindPackageRelations(subpackage);
            }

            for (short index = 0; index < package.Elements.Count; index++)
            {
                EA.Element element = (EA.Element) package.Elements.GetAt(index);
                FindElementRelations(element);
            }
        }

        private void FindElementRelations(EA.Element element)
        {
            for (short index = 0; index < element.Connectors.Count; index++)
            {
                EA.Connector connector = (EA.Connector) element.Connectors.GetAt(index);

                RegisterRelation(connector);
            }

            for (short index = 0; index < element.Elements.Count; index++)
            {
                EA.Element nestedElement = (EA.Element) element.Elements.GetAt(index);
                FindElementRelations(nestedElement);
            }
        }

        private void RegisterElement(EA.Element element)
        {
            Logger.LogInfo("Register model element:" + ExtractUniqueName(element));
            _model.AddElement(ExtractUniqueName(element), element.Type, _analyzerSettings.Input.Filename);
            _elementCount++;
            UpdateElementProgress(false);
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
            _model.AddRelation(consumerName, providerName, connector.Type, 1, null);
            _relationCount++;
            UpdateRelationProgress(false);
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

        private void UpdateElementProgress(bool done)
        {
            ProgressInfo progressInfo = new ProgressInfo();
            progressInfo.ActionText = "Reading UML elements";
            progressInfo.CurrentItemCount = _elementCount;
            progressInfo.TotalItemCount = 0;
            progressInfo.ItemType = "elements";
            progressInfo.Percentage = null;
            progressInfo.Done = done;
            _progress?.Report(progressInfo);
        }

        private void UpdateRelationProgress(bool done)
        {
            ProgressInfo progressInfo = new ProgressInfo();
            progressInfo.ActionText = "Reading UML relations";
            progressInfo.CurrentItemCount = _relationCount;
            progressInfo.TotalItemCount = 0;
            progressInfo.ItemType = "relations";
            progressInfo.Percentage = null;
            progressInfo.Done = done;
            _progress?.Report(progressInfo);
        }
    }
}
