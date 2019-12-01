using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Analyzer.Model.Data
{
    /// <summary>
    /// Represents a relation of a specific type between two elements
    /// </summary>
    public class Relation : IRelation
    {
        public Relation(int providerId, int consumerId, string type, int weight)
        {
            ConsumerId = consumerId;
            ProviderId = providerId;
            Type = type;
            Weight = weight;
        }

        public int ConsumerId { get; }
        public int ProviderId { get; }
        public string Type { get; }
        public int Weight { get; }

        public override int GetHashCode()
        {
            unchecked{
                var hashCode = ConsumerId.GetHashCode();
                hashCode = (hashCode * 397) ^ ProviderId.GetHashCode();
                hashCode = (hashCode * 397) ^ (Type?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Relation)obj);
        }

        private bool Equals(Relation other)
        {
            return Equals(ConsumerId, other.ConsumerId) && Equals(ProviderId, other.ProviderId) && string.Equals(Type, other.Type);
        }
    }
}
