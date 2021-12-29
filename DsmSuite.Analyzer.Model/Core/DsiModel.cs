using System.Collections.Generic;
using System.Reflection;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Model.Persistency;
using DsmSuite.Common.Model.Core;
using DsmSuite.Common.Util;
using DsmSuite.Common.Model.Interface;
using System;
using System.Text.RegularExpressions;

namespace DsmSuite.Analyzer.Model.Core
{
    public class DsiModel : IDsiModel
    {
        private readonly MetaDataModel _metaDataModel;
        private readonly DsiElementModel _elementsDataModel;
        private readonly DsiRelationModel _relationsDataModel;
        private readonly List<string> _ignoredNames;

        public DsiModel(string processStep, List<string> ignoredNames, Assembly executingAssembly)
        {
            _metaDataModel = new MetaDataModel(processStep, executingAssembly);
            _elementsDataModel = new DsiElementModel();
            _relationsDataModel = new DsiRelationModel(_elementsDataModel);

            _ignoredNames = ignoredNames;
        }

        public string Filename { get; private set; }

        public void Load(string dsiFilename, IProgress<ProgressInfo> progress)
        {
            Logger.LogDataModelMessage($"Load data model file={dsiFilename}");

            Filename = dsiFilename;
            DsiModelFile modelFile =
                new DsiModelFile(dsiFilename, _metaDataModel, _elementsDataModel, _relationsDataModel);
            modelFile.Load(progress);
        }

        public void Save(string dsiFilename, bool compressFile, IProgress<ProgressInfo> progress)
        {
            Logger.LogDataModelMessage($"Save data model file={dsiFilename} compress={compressFile}");

            Filename = dsiFilename;

            foreach (string type in GetElementTypes())
            {
                _metaDataModel.AddMetaDataItemToDefaultGroup($"- '{type}' elements found",
                    $"{GetElementTypeCount(type)}");
            }

            _metaDataModel.AddMetaDataItemToDefaultGroup("Total elements found", $"{CurrentElementCount}");

            foreach (string type in GetRelationTypes())
            {
                _metaDataModel.AddMetaDataItemToDefaultGroup($"- '{type}' relations found",
                    $"{GetRelationTypeCount(type)}");
            }

            _metaDataModel.AddMetaDataItemToDefaultGroup("Total relations found", $"{ImportedRelationCount}");
            _metaDataModel.AddMetaDataItemToDefaultGroup("Total relations resolved",
                $"{ResolvedRelationCount} (confidence={ResolvedRelationPercentage:0.000} %)");

            DsiModelFile modelFile =
                new DsiModelFile(dsiFilename, _metaDataModel, _elementsDataModel, _relationsDataModel);
            modelFile.Save(compressFile, progress);
        }

        public void AddMetaData(string name, string value)
        {
            _metaDataModel.AddMetaDataItemToDefaultGroup(name, value);
        }

        public IEnumerable<string> GetMetaDataGroups()
        {
            return _metaDataModel.GetExportedMetaDataGroups();
        }

        public IEnumerable<IMetaDataItem> GetMetaDataGroupItems(string groupName)
        {
            return _metaDataModel.GetExportedMetaDataGroupItems(groupName);
        }

        public IDsiElement AddElement(string name, string type, IDictionary<string, string> properties)
        {
            IDsiElement element = null;

            if (!Ignore(name))
            {
                element = _elementsDataModel.AddElement(name, type, properties);
            }
            else
            {
                _elementsDataModel.IgnoreElement(name, type);
            }

            return element;
        }

        public void RemoveElement(IDsiElement element)
        {
            _elementsDataModel.RemoveElement(element);
        }

        public void RenameElement(IDsiElement element, string newName)
        {
            _elementsDataModel.RenameElement(element, newName);
        }

        public IDsiElement FindElementById(int id)
        {
            return _elementsDataModel.FindElementById(id);
        }

        public IDsiElement FindElementByName(string name)
        {
            return _elementsDataModel.FindElementByName(name);
        }

        public IEnumerable<IDsiElement> GetElements()
        {
            return _elementsDataModel.GetElements();
        }

        public ICollection<string> GetElementTypes()
        {
            return _elementsDataModel.GetElementTypes();
        }

        public int GetElementTypeCount(string type)
        {
            return _elementsDataModel.GetElementTypeCount(type);
        }

        public int CurrentElementCount => _elementsDataModel.CurrentElementCount;

        public IDsiRelation AddRelation(string consumerName, string providerName, string type, int weight, IDictionary<string, string> properties)
        {
            IDsiRelation relation = null;

            if (!Ignore(consumerName) && !Ignore(providerName))
            {
                relation = _relationsDataModel.AddRelation(consumerName, providerName, type, weight, properties);
            }
            else
            {
                _relationsDataModel.IgnoreRelation(consumerName, providerName, type, weight);
            }

            return relation;
        }

        public void SkipRelation(string consumerName, string providerName, string type)
        {
            _relationsDataModel.SkipRelation(consumerName, providerName, type);
        }

        public void AmbiguousRelation(string consumerName, string providerName, string type)
        {
            _relationsDataModel.AmbiguousRelation(consumerName, providerName, type);
        }

        public ICollection<string> GetRelationTypes()
        {
            return _relationsDataModel.GetRelationTypes();
        }

        public int GetRelationTypeCount(string type)
        {
            return _relationsDataModel.GetRelationTypeCount(type);
        }

        public ICollection<IDsiRelation> GetRelationsOfConsumer(int consumerId)
        {
            return _relationsDataModel.GetRelationsOfConsumer(consumerId);
        }

        public IEnumerable<IDsiRelation> GetRelations()
        {
            return _relationsDataModel.GetRelations();
        }

        public int CurrentRelationCount => _relationsDataModel.CurrentRelationCount;

        public bool DoesRelationExist(int consumerId, int providerId)
        {
            return _relationsDataModel.DoesRelationExist(consumerId, providerId);
        }

        public int ImportedRelationCount => _relationsDataModel.ImportedRelationCount;

        public int ResolvedRelationCount => _relationsDataModel.ResolvedRelationCount;

        public double ResolvedRelationPercentage => _relationsDataModel.ResolvedRelationPercentage;

        public double AmbiguousRelationPercentage => _relationsDataModel.AmbiguousRelationPercentage;

        private bool Ignore(string name)
        {
            bool ignore = false;

            foreach (string ignoredName in _ignoredNames)
            {
                Regex regex = new Regex(ignoredName);
                Match match = regex.Match(name);
                if (match.Success)
                {
                    ignore = true;
                }
            }
            return ignore;
        }
    }
}
