using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Analyzer.Model.Data
{
    /// <summary>
    /// Represents a relation of a specific type between two elements
    /// </summary>
    public class DsiRelation : IDsiRelation
    {
        public DsiRelation(int consumer, int provider, string type, int weight)
        {
            Consumer = consumer;
            Provider = provider;
            Type = type;
            Weight = weight;
        }

        public int Consumer { get; }
        public int Provider { get; }
        public string Type { get; }
        public int Weight { get; }
    }
}
