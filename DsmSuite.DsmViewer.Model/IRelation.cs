namespace DsmSuite.DsmViewer.Model
{
    public interface IRelation
    {
        /// <summary>
        /// The consumer element.
        /// </summary>
        IElement Consumer { get; }

        /// <summary>
        /// The provider element.
        /// </summary>
        IElement Provider { get; }

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
