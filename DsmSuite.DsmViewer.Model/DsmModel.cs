using System;
using System.Collections.Generic;
using System.Reflection;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model.Algorithms;
using DsmSuite.DsmViewer.Model.Dependencies;
using DsmSuite.DsmViewer.Model.Files;
using DsmSuite.DsmViewer.Model.Files.Dsm;

namespace DsmSuite.DsmViewer.Model
{
    public class DsmModel : IDsmModel
    {
        private readonly string _processStep;
        private bool _isModified;
        private readonly MetaData _metaData;
        private readonly DependencyModel _dependencyModel;

        public event EventHandler<bool> Modified;

        public class RelationInternal : IRelation
        {
            public RelationInternal(DependencyModel dependencyModel, Relation relation)
            {
                Consumer = dependencyModel.GetElementById(relation.ConsumerId);
                Provider = dependencyModel.GetElementById(relation.ProviderId);
                Type = relation.Type;
                Weight = relation.Weight;
            }

            public IElement Consumer { get; }


            public IElement Provider { get; }


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

        public IList<IElement> RootElements => _dependencyModel.RootElements;

        /// <summary>
        /// Create element in selected parent. The element id will be assigned based on the fullname
        /// of the element.
        /// </summary>
        /// <param name="name">The name of the element</param>
        /// <param name="type">The type of element</param>
        /// <param name="parentId">The element id of the parent</param>
        /// <returns></returns>
        public IElement CreateElement(string name, string type, int? parentId)
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

        public IElement GetElementById(int id)
        {
            return _dependencyModel.GetElementById(id);
        }

        public IElement GetElementByFullname(string fullname)
        {
            return _dependencyModel.GetElementByFullname(fullname);
        }

        public IEnumerable<IElement> GetElementsWithFullnameContainingText(string text)
        {
            return _dependencyModel.GetElementsWithFullnameContainingText(text);
        }

        public int GetDependencyWeight(IElement consumer, IElement provider)
        {
            return _dependencyModel.GetDependencyWeight(consumer.Id, provider.Id);
        }

        public bool IsCyclicDependency(IElement consumer, IElement provider)
        {
            return _dependencyModel.IsCyclicDependency(consumer.Id, provider.Id);
        }

        public IList<IRelation> FindRelations(IElement consumer, IElement provider)
        {
            return Convert(_dependencyModel.FindRelations(consumer, provider));
        }

        public IList<IRelation> FindProviderRelations(IElement element)
        {
            return Convert(_dependencyModel.FindElementConsumerRelations(element));
        }

        public IList<IRelation> FindConsumerRelations(IElement element)
        {
            return Convert(_dependencyModel.FindElementProviderRelations(element));
        }

        public IList<IElement> FindProviders(IElement element)
        {
            return Convert(_dependencyModel.FindElementProviders(element));
        }

        public IList<IElement> FindConsumers(IElement element)
        {
            return Convert(_dependencyModel.FindElementConsumers(element));
        }

        private IList<IRelation> Convert(ICollection<Relation> relations)
        {
            List<IRelation> result = new List<IRelation>();
            foreach (Relation relation in relations)
            {
                result.Add(Convert(relation));
            }
            return result;
        }

        private IRelation Convert(Relation relation)
        {
            return new RelationInternal(_dependencyModel, relation);
        }

        private IList<IElement> Convert(ICollection<Element> elements)
        {
            List<IElement> result = new List<IElement>();
            foreach (Element element in elements)
            {
                result.Add(element);
            }
            return result;
        }

        public void Partition(IElement element)
        {
            Partitioner partitioner = new Partitioner(element, this);
            partitioner.Partition();
            AssignElementOrder();
            IsModified = true;
        }

        public IElement NextSibling(IElement element)
        {
            IElement next = null;
            if (element != null)
            {
                next = element.NextSibling;
            }
            return next;
        }

        public IElement PreviousSibling(IElement element)
        {
            IElement previous = null;
            if (element != null)
            {
                previous = element.PreviousSibling;
            }
            return previous;
        }

        public bool Swap(IElement first, IElement second)
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
