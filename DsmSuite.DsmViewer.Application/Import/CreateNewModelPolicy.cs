using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.Common.Model.Interface;
using DsmSuite.DsmViewer.Application.Algorithm;

namespace DsmSuite.DsmViewer.Application.Import
{
    public class CreateNewModelPolicy : IImportPolicy
    {
        private readonly IDsmModel _dsmModel;
        private readonly bool _autoPartition;

        public CreateNewModelPolicy(IDsmModel dsmmodel, bool autoPartition)
        {
            _dsmModel = dsmmodel;
            _autoPartition = autoPartition;
            _dsmModel.Clear();
        }

        public IMetaDataItem ImportMetaDataItem(string group, string name, string value)
        {
            return _dsmModel.AddMetaData(group, name, value);
        }

        public IDsmElement ImportElement(string fullname, string name, string type, IDsmElement parent)
        {
            IDsmElement element = _dsmModel.GetElementByFullname(fullname);
            if (element == null)
            {
                int? parentId = parent?.Id;
                element = _dsmModel.AddElement(name, type, parentId);
            }
            return element;
        }

        public IDsmRelation ImportRelation(int consumerId, int providerId, string type, int weight)
        {
            return _dsmModel.AddRelation(consumerId, providerId, type, weight);
        }

        public void FinalizeImport()
        {
            if (_autoPartition)
            {
                Partition();
            }

            _dsmModel.AssignElementOrder();
        }

        private void Partition()
        {
            foreach (IDsmElement element in _dsmModel.GetRootElements())
            {
                Partition(element);
            }
        }

        private void Partition(IDsmElement element)
        {
            ISortAlgorithm algorithm = SortAlgorithmFactory.CreateAlgorithm(_dsmModel, element, PartitionSortAlgorithm.AlgorithmName);
            _dsmModel.ReorderChildren(element, algorithm.Sort());

            foreach (IDsmElement child in element.Children)
            {
                Partition(child);
            }
        }
    }
}
