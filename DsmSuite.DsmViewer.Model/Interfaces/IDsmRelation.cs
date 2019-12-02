namespace DsmSuite.DsmViewer.Model.Interfaces
{
    public interface IDsmRelation
    {
        /// <summary>
        /// The consumer element.
        /// </summary>
        int ConsumerId { get; }

        /// <summary>
        /// The provider element.
        /// </summary>
        int ProviderId { get; }

        /// <summary>
        /// Type of relation.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Strength or weight of the relation
        /// </summary>
        int Weight { get; }
    }
}
