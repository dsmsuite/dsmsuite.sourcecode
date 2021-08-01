using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Core
{
    /// <summary>
    /// Relation between two elements.
    /// </summary>
    public class DsmRelation : IDsmRelation
    {
        private char _typeId;
        private static readonly TypeRegistration TypeRegistration = new TypeRegistration();

        public DsmRelation(int id, IDsmElement consumer, IDsmElement provider, string type, int weight)
        {
            Id = id;
            Consumer = consumer;
            Provider = provider;
            _typeId = TypeRegistration.AddTypeName(type);
            Weight = weight;
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

        public bool IsDeleted { get; set; }
    }
}
