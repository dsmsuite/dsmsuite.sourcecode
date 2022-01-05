using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Model.Core
{
    /// <summary>
    /// Relation between two elements.
    /// </summary>
    public class DsmRelation : IDsmRelation
    {
        private char _typeId;
        private static readonly TypeRegistration TypeRegistration = new TypeRegistration();

        public DsmRelation(int id, IDsmElement consumer, IDsmElement provider, string type, int weight, IDictionary<string, string> properties)
        {
            Id = id;
            Consumer = consumer;
            Provider = provider;
            _typeId = TypeRegistration.AddTypeName(type);
            Weight = weight;
            Properties = properties;
        }

        public int Id { get; }

        public IDsmElement Consumer { get; }

        public IDsmElement Provider { get; }

        public string Type
        {
            get { return TypeRegistration.GetTypeName(_typeId); }
            set { _typeId = TypeRegistration.AddTypeName(value); }
        }

        public int Weight { get; set; }

        public IDictionary<string, string> Properties { get; }

        public bool IsDeleted { get; set; }

        public static IEnumerable<string> GetTypeNames()
        {
            return TypeRegistration.GetTypeNames();
        }
    }
}
