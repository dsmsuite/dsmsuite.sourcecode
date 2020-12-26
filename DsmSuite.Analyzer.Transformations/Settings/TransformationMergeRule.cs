using System;

namespace DsmSuite.Analyzer.Transformations.Settings
{
    [Serializable]
    public class TransformationMergeRule
    {
        public string From { get; set; }

        public string To { get; set; }
    }
}
