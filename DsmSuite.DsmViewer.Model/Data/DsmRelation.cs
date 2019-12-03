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

        public DsmRelation(int consumerId, int providerId, string type, int weight)
        {
            ConsumerId = consumerId;
            ProviderId = providerId;
            _typeId = TypeRegistration.AddTypeName(type);
            Weight = weight;
        }

        /// <summary>
        /// Unique id of consumer element.
        /// </summary>
        public int ConsumerId { get; }

        /// <summary>
        /// Unique id of provider element.
        /// </summary>
        public int ProviderId { get; }

        /// <summary>
        /// Type of relation.
        /// </summary>
        public string Type => TypeRegistration.GetTypeName(_typeId);

        /// <summary>
        /// Strength or weight of the relation
        /// </summary>
        public int Weight { get; }
    }
}
