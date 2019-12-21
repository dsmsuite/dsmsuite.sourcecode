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

        public DsmRelation(int id, int consumerId, int providerId, string type, int weight)
        {
            Id = id;
            ConsumerId = consumerId;
            ProviderId = providerId;
            _typeId = TypeRegistration.AddTypeName(type);
            Weight = weight;
        }

        public int Id { get; }

        public int ConsumerId { get; }

        public int ProviderId { get; }

        public string Type
        {
            get { return TypeRegistration.GetTypeName(_typeId); }
            set { _typeId = TypeRegistration.AddTypeName(value); }
        }

        public int Weight { get; set; }

        public bool IsDeleted { get; set; }
    }
}
