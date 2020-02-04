using System;
using DsmSuite.Common.Util;

namespace DsmSuite.Transformer.Transformation
{
    public abstract class Action
    {
        private readonly IProgress<ProgressInfo> _progress;
        protected Action(string name, bool enabled, IProgress<ProgressInfo> progress)
        {
            Name = name;
            IsEnabled = enabled;
            _progress = progress;
        }

        public bool IsEnabled { get; }

        public void Execute()
        {
            if (IsEnabled)
            {
                ExecuteImpl();
            }
        }

        public string Name { get; }

        protected abstract void ExecuteImpl();

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
