using System;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Model.Interfaces
{
    public interface IDsmElement : IComparable
    {
        /// <summary>
        /// Unique and non-modifiable Number identifying the element.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Number identifying sequential order of the element in element tree.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Type of element.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Name of the element.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Full name of the element based on its position in the element hierarchy
        /// </summary>
        string Fullname { get; }

        bool IsDeleted { get; }

        /// <summary>
        /// Has the element any children.
        /// </summary>
        bool HasChildren { get; }

        int DirectChildCount { get; }
        int TotalChildCount { get; }

        /// <summary>
        /// Children of the element.
        /// </summary>
        IList<IDsmElement> Children { get; }

        IList<IDsmElement> ExportedChildren { get; }

        /// <summary>
        /// Parent of the element.
        /// </summary>
        IDsmElement Parent { get; }

        /// <summary>
        /// Is the element expanded in the viewer.
        /// </summary>
        bool IsExpanded { get; set; }
    }
}
