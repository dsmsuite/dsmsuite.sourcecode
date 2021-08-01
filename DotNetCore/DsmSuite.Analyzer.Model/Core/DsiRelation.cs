using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Analyzer.Model.Core
{
    /// <summary>
    /// Represents a relation of a specific type between two elements
    /// </summary>
    public class DsiRelation : IDsiRelation
    {
        public DsiRelation(int consumerId, int providerId, string type, int weight, string annotation)
        {
            ConsumerId = consumerId;
            ProviderId = providerId;
            Type = type;
            Weight = weight;
            Annotation = annotation;
        }

        public int ConsumerId { get; }
        public int ProviderId { get; }
        public string Type { get; }
        public int Weight { get; set; }
        public string Annotation { get; }
    }
}
