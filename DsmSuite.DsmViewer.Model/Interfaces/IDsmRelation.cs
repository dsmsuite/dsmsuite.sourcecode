namespace DsmSuite.DsmViewer.Model.Interfaces
{
    public interface IDsmRelation
    {
        /// <summary>
        /// The consumer element.
        /// </summary>
        IDsmElement Consumer { get; }

        /// <summary>
        /// The provider element.
        /// </summary>
        IDsmElement Provider { get; }

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
