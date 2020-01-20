using System;
using System.Linq;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.Common.Model.Interface;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Application.Sorting;

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

        public void FinalizeImport(IProgress<ProgressInfo> progress)
        {
            if (_autoPartition)
            {
                Partition();
            }

            _dsmModel.AssignElementOrder();
        }

        private void Partition()
        {
            int partitionedElements = 0;
            Console.WriteLine($"Partitioning {_dsmModel.GetElementCount()} elements");
            Partition(_dsmModel.GetRootElement(), ref partitionedElements);
            Console.Write("progress elements={0}", partitionedElements);
        }

        private void Partition(IDsmElement element, ref int partitionedElements)
        {
            ISortAlgorithm algorithm = SortAlgorithmFactory.CreateAlgorithm(_dsmModel, element, PartitionSortAlgorithm.AlgorithmName);
            _dsmModel.ReorderChildren(element, algorithm.Sort());
            partitionedElements++;
            Console.Write("\r progress elements={0}", partitionedElements);

            foreach (IDsmElement child in element.Children)
            {
                Partition(child, ref partitionedElements);

            }
        }
    }
}
