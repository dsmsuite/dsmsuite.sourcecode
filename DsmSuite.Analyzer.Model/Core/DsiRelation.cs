using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Analyzer.Model.Core
{
    /// <summary>
    /// Represents a relation of a specific type between two elements
    /// </summary>
    public class DsiRelation : IDsiRelation
    {
        public DsiRelation(int consumerId, int providerId, string type, int weight, IDictionary<string, string> properties)
        {
            ConsumerId = consumerId;
            ProviderId = providerId;
            Type = type;
            Weight = weight;
            Properties = properties;
        }

        public int ConsumerId { get; }
        public int ProviderId { get; }
        public string Type { get; }
        public int Weight { get; set; }
        public IDictionary<string, string> Properties { get; }
    }
}
