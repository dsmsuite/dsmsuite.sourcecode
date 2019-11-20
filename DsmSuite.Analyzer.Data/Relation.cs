
namespace DsmSuite.Analyzer.Data
{
    /// <summary>
    /// Represents a relation of a specific type between two elements
    /// </summary>
    public class Relation : IRelation
    {
        public Relation(IElement provider, IElement consumer, string type, int weight)
        {
            Consumer = consumer;
            Provider = provider;
            Type = type;
            Weight = weight;
        }

        public IElement Consumer { get; }
        public IElement Provider { get; }
        public string Type { get; }
        public int Weight { get; }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Consumer?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (Provider?.GetHashCode() ?? 0);
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
            return Equals(Consumer, other.Consumer) && Equals(Provider, other.Provider) && string.Equals(Type, other.Type);
        }
    }
}
