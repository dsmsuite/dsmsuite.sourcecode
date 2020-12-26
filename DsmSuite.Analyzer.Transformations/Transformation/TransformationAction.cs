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
            ProgressInfo progressInfo = new ProgressInfo();
            progressInfo.ActionText = actionName;
            progressInfo.CurrentItemCount = currentItemCount;
            progressInfo.TotalItemCount = totalItemCount;
            progressInfo.ItemType = "elements";
            progressInfo.Percentage = currentItemCount * 100 / totalItemCount;
            progressInfo.Done = currentItemCount == totalItemCount;
            _progress?.Report(progressInfo);
        }
    }
}
