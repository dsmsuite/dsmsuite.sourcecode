using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Application.Sorting;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Import.Common
{
    public class ImporterBase
    {
        private readonly IDsmModel _dsmModel;
        private int _progressPercentage;

        public ImporterBase(IDsmModel dsmModel)
        {
            _dsmModel = dsmModel;
        }

        protected void Partition(IProgress<ProgressInfo> progress)
        {
            int totalElements = _dsmModel.GetElementCount();
            int progressedElements = 0;
            Partition(progress, _dsmModel.RootElement, totalElements, ref progressedElements);
        }

        protected void Partition(IProgress<ProgressInfo> progress, IDsmElement element, int totalElements, ref int progressedElements)
        {
            ISortAlgorithm algorithm = SortAlgorithmFactory.CreateAlgorithm(_dsmModel, element, PartitionSortAlgorithm.AlgorithmName);
            _dsmModel.ReorderChildren(element, algorithm.Sort());
            progressedElements++;
            UpdateProgress(progress, "Partition elements", totalElements, progressedElements);

            foreach (IDsmElement child in element.Children)
            {
                Partition(progress, child, totalElements, ref progressedElements);
            }
        }

        protected void UpdateProgress(IProgress<ProgressInfo> progress, string progressActionText, int totalItemCount, int progressedItemCount)
        {
            if (progress != null)
            {
                int currentProgressPercentage = 0;
                if (totalItemCount > 0)
                {
                    currentProgressPercentage = progressedItemCount * 100 / totalItemCount;
                }

                if (_progressPercentage != currentProgressPercentage)
                {
                    _progressPercentage = currentProgressPercentage;

                    ProgressInfo progressInfoInfo = new ProgressInfo
                    {
                        ActionText = progressActionText,
                        TotalItemCount = totalItemCount,
                        CurrentItemCount = progressedItemCount,
                        ItemType = "items",
                        Percentage = currentProgressPercentage,
                        Done = totalItemCount == progressedItemCount
                    };

                    progress.Report(progressInfoInfo);
                }
            }
        }
    }
}
