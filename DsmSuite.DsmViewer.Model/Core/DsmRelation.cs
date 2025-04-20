using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Core
{
    /// <summary>
    /// Relation between two elements.
    /// </summary>
    public class DsmRelation : IDsmRelation
    {
        private char _typeId;
        private static readonly NameRegistration RelationTypeNameRegistration = new NameRegistration();
        private static readonly NameRegistration RelationPropertyNameRegistration = new NameRegistration();

        public DsmRelation(int id, IDsmElement consumer, IDsmElement provider, string type, int weight, IDictionary<string, string> properties)
        {
            Id = id;
            Consumer = consumer;
            Provider = provider;
            _typeId = RelationTypeNameRegistration.RegisterName(type);
            Weight = weight;
            Properties = (properties != null) ? properties : new Dictionary<string, string>();

            foreach (string key in Properties.Keys)
            {
                RelationPropertyNameRegistration.RegisterName(key);
            }
        }

        public int Id { get; }

        public IDsmElement Consumer { get; }

        public IDsmElement Provider { get; }

        public string Type
        {
            get { return RelationTypeNameRegistration.GetRegisteredName(_typeId); }
            set { _typeId = RelationTypeNameRegistration.RegisterName(value); }
        }

        public int Weight { get; set; }

        public IDictionary<string, string> Properties { get; }

        public IEnumerable<string> DiscoveredRelationPropertyNames()
        {
            return RelationPropertyNameRegistration.GetRegisteredNames();
        }

        public bool IsDeleted { get; set; }

        public static IEnumerable<string> GetTypeNames()
        {
            return RelationTypeNameRegistration.GetRegisteredNames();
        }
    }
}
