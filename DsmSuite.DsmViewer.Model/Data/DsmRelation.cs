using DsmSuite.DsmViewer.Model.Core;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Data
{
    /// <summary>
    /// Relation between two elements.
    /// </summary>
    public class DsmRelation : IDsmRelation
    {
        private readonly char _typeId;
        private static readonly TypeRegistration TypeRegistration = new TypeRegistration();

        public DsmRelation(int id, int consumer, int provider, string type, int weight)
        {
            Id = id;
            Consumer = consumer;
            Provider = provider;
            _typeId = TypeRegistration.AddTypeName(type);
            Weight = weight;
        }

        public int Id { get; }

        public int Consumer { get; }

        public int Provider { get; }

        public string Type => TypeRegistration.GetTypeName(_typeId);

        public int Weight { get; }
    }
}
