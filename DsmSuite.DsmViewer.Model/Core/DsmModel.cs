using System;
using System.Collections.Generic;
using System.Reflection;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model.Algorithms;
using DsmSuite.DsmViewer.Model.Data;
using DsmSuite.DsmViewer.Model.Dependencies;
using DsmSuite.DsmViewer.Model.Files.Dsm;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Core
{
    public class DsmModel : IDsmModel
    {
        private readonly string _processStep;
        private bool _isModified;
        private readonly MetaData _metaData;
        private readonly DependencyModel _dependencyModel;

        public event EventHandler<bool> Modified;

        public class RelationInternal : IDsmRelation
        {
            public RelationInternal(DependencyModel dependencyModel, DsmRelation relation)
            {
                Consumer = dependencyModel.GetElementById(relation.ConsumerId);
                Provider = dependencyModel.GetElementById(relation.ProviderId);
                Type = relation.Type;
                Weight = relation.Weight;
            }

            public IDsmElement Consumer { get; }


            public IDsmElement Provider { get; }


            public string Type { get; }


            public int Weight { get; }
        }

        public DsmModel(string processStep, Assembly executingAssembly)
        {
            _processStep = processStep;
            _metaData = new MetaData();
            _dependencyModel = new DependencyModel();

            if (processStep != null)
            {
                _metaData.AddMetaData(processStep, "Executable", SystemInfo.GetExecutableInfo(executingAssembly));
            }
        }

        public string ModelFilename { get; private set; }

        public bool IsModified
        {
            get
            {
                return _isModified;
            }
            private set
            {
                _isModified = value;
                Modified?.Invoke(this, _isModified);
            }
        }

        public bool IsCompressed { get; private set; }

        public void Clear()
        {
            _dependencyModel.Clear();
            _metaData.Clear();
        }

        public void LoadModel(string dsmFilename, IProgress<ProgressInfo> progress)
        {
            Clear();
            DsmModelFileReader dsmModelFile = new DsmModelFileReader(dsmFilename, _dependencyModel, _metaData);
            dsmModelFile.ReadFile(progress);
            IsCompressed = dsmModelFile.IsCompressedFile();
            ModelFilename = dsmFilename;
        }

        public void SaveModel(string dsmFilename, bool compressFile, IProgress<ProgressInfo> progress)
        {
            if (_processStep != null)
            {
                _metaData.AddMetaData(_processStep, "Total elements found", $"{_dependencyModel.ElementCount}");
                _metaData.AddMetaData(_processStep, "Total relations found", $"{_dependencyModel.RelationCount} (density={_dependencyModel.RelationDensity:0.000} %)");
                _metaData.AddMetaData(_processStep, "System cycles found", $"{_dependencyModel.SystemCycalityCount} (cycality={_dependencyModel.SystemCycalityPercentage:0.000} %)");
                _metaData.AddMetaData(_processStep, "Hierarchical cycles found", $"{_dependencyModel.HierarchicalCycalityCount} (cycality={_dependencyModel.HierarchicalCycalityPercentage:0.000} %)");
            }

            DsmModelFileWriter dsmModelFile = new DsmModelFileWriter(dsmFilename, _dependencyModel, _metaData);
            dsmModelFile.WriteFile(compressFile, progress);
            IsModified = false;
            ModelFilename = dsmFilename;
        }

        public IList<IDsmElement> RootElements => _dependencyModel.RootElements;

        /// <summary>
        /// Create element in selected parent. The element id will be assigned based on the fullname
        /// of the element.
        /// </summary>
        /// <param name="name">The name of the element</param>
        /// <param name="type">The type of element</param>
        /// <param name="parentId">The element id of the parent</param>
        /// <returns></returns>
        public IDsmElement CreateElement(string name, string type, int? parentId)
        {
            return _dependencyModel.CreateElement(name, type, parentId);
        }

        public void RemoveElement(int id)
        {
            _dependencyModel.RemoveElement(id);
        }

        public void RestoreElement(int id)
        {
            _dependencyModel.RestoreElement(id);
        }

        /// <summary>
        /// Add a relation  between two elements.
        /// </summary>
        /// <param name="consumerId">The consumer</param>
        /// <param name="providerId">The provider</param>
        /// <param name="type">The type of relation</param>
        /// <param name="weight">The weight or strength of the relation</param>
        public void AddRelation(int consumerId, int providerId, string type, int weight)
        {
            _dependencyModel.AddRelation(consumerId, providerId, type, weight);
        }

        public void RemoveRelation(int consumerId, int providerId, string type, int weight)
        {
            _dependencyModel.RemoveRelation(consumerId, providerId, type, weight);
        }

        public void UnremoveRelation(int consumerId, int providerId, string type, int weight)
        {
            _dependencyModel.UnremoveRelation(consumerId, providerId, type, weight);
        }

        public void AssignElementOrder()
        {
            _dependencyModel.AssignElementOrder();
        }

        public int ElementCount => _dependencyModel.ElementCount;
        public void AddMetaData(string group, string name, string value)
        {
            _metaData.AddMetaData(group, name, value);
        }

        public IDsmElement GetElementById(int id)
        {
            return _dependencyModel.GetElementById(id);
        }

        public IDsmElement GetElementByFullname(string fullname)
        {
            return _dependencyModel.GetElementByFullname(fullname);
        }

        public IEnumerable<IDsmElement> GetElementsWithFullnameContainingText(string text)
        {
            return _dependencyModel.GetElementsWithFullnameContainingText(text);
        }

        public int GetDependencyWeight(IDsmElement consumer, IDsmElement provider)
        {
            return _dependencyModel.GetDependencyWeight(consumer.Id, provider.Id);
        }

        public bool IsCyclicDependency(IDsmElement consumer, IDsmElement provider)
        {
            return _dependencyModel.IsCyclicDependency(consumer.Id, provider.Id);
        }

        public IList<IDsmRelation> FindRelations(IDsmElement consumer, IDsmElement provider)
        {
            return Convert(_dependencyModel.FindRelations(consumer, provider));
        }

        public IList<IDsmRelation> FindProviderRelations(IDsmElement element)
        {
            return Convert(_dependencyModel.FindElementConsumerRelations(element));
        }

        public IList<IDsmRelation> FindConsumerRelations(IDsmElement element)
        {
            return Convert(_dependencyModel.FindElementProviderRelations(element));
        }

        public IList<IDsmElement> FindProviders(IDsmElement element)
        {
            return Convert(_dependencyModel.FindElementProviders(element));
        }

        public IList<IDsmElement> FindConsumers(IDsmElement element)
        {
            return Convert(_dependencyModel.FindElementConsumers(element));
        }

        private IList<IDsmRelation> Convert(ICollection<DsmRelation> relations)
        {
            List<IDsmRelation> result = new List<IDsmRelation>();
            foreach (DsmRelation relation in relations)
            {
                result.Add(Convert(relation));
            }
            return result;
        }

        private IDsmRelation Convert(DsmRelation relation)
        {
            return new RelationInternal(_dependencyModel, relation);
        }

        private IList<IDsmElement> Convert(ICollection<DsmElement> elements)
        {
            List<IDsmElement> result = new List<IDsmElement>();
            foreach (DsmElement element in elements)
            {
                result.Add(element);
            }
            return result;
        }

        public void Partition(IDsmElement element)
        {
            Partitioner partitioner = new Partitioner(element, this);
            partitioner.Partition();
            AssignElementOrder();
            IsModified = true;
        }

        public IDsmElement NextSibling(IDsmElement element)
        {
            IDsmElement next = null;
            if (element != null)
            {
                next = element.NextSibling;
            }
            return next;
        }

        public IDsmElement PreviousSibling(IDsmElement element)
        {
            IDsmElement previous = null;
            if (element != null)
            {
                previous = element.PreviousSibling;
            }
            return previous;
        }

        public bool Swap(IDsmElement first, IDsmElement second)
        {
            bool ok = false;
            if (_dependencyModel.Swap(first, second))
            {
                ok = true;
                IsModified = true;
            }

            return ok;
        }

        public IList<string> GetGroups()
        {
            return _metaData.GetGroups();
        }

        public IList<string> GetNames(string group)
        {
            return _metaData.GetNames(group);
        }

        public string GetValue(string group, string name)
        {
            return _metaData.GetValue(group, name);
        }
    }
}
