using System;
using DsmSuite.Common.Util;

namespace DsmSuite.Transformer.Transformation
{
    public abstract class Action
    {
        protected Action(string name, bool enabled)
        {
            Name = name;
            IsEnabled = enabled;
        }

        public bool IsEnabled { get; }

        public void Execute(IProgress<ProgressInfo> progress)
        {
            if (IsEnabled)
            {
                ExecuteImpl(progress);
            }
        }

        protected abstract void ExecuteImpl(IProgress<ProgressInfo> progress);

        public string Name { get; }
    }
}
