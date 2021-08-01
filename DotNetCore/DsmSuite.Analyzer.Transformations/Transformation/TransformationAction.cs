using System;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Transformations.Transformation
{
    public abstract class TransformationAction
    {
        private readonly IProgress<ProgressInfo> _progress;
        protected TransformationAction(string name, IProgress<ProgressInfo> progress)
        {
            Name = name;
            _progress = progress;
        }

        public abstract void Execute();

        public string Name { get; }
        
        protected void UpdateTransformationProgress(string actionName, int currentItemCount, int totalItemCount)
        {
            ProgressInfo progressInfo = new ProgressInfo
            {
                ActionText = actionName,
                CurrentItemCount = currentItemCount,
                TotalItemCount = totalItemCount,
                ItemType = "elements",
                Percentage = currentItemCount * 100 / totalItemCount,
                Done = currentItemCount == totalItemCount
            };
            _progress?.Report(progressInfo);
        }
    }
}
