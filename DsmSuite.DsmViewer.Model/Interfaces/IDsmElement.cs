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
        string Name { get; set; }

        /// <summary>
        /// Full name of the element based on its position in the element hierarchy
        /// </summary>
        string Fullname { get; }

        /// <summary>
        /// Has the element any children.
        /// </summary>
        bool HasChildren { get; }

        /// <summary>
        /// Children of the element.
        /// </summary>
        IList<IDsmElement> Children { get; }

        /// <summary>
        /// Parent of the element.
        /// </summary>
        IDsmElement Parent { get; }

        /// <summary>
        /// Get the previous element with the same parent.
        /// </summary>
        IDsmElement PreviousSibling { get; }

        /// <summary>
        /// Get the next element with the same parent.
        /// </summary>
        IDsmElement NextSibling { get; }

        /// <summary>
        /// Get the first child.
        /// </summary>
        IDsmElement FirstChild { get; }

        /// <summary>
        /// Get the last child.
        /// </summary>
        IDsmElement LastChild { get; }

        int Depth { get; }

        /// <summary>
        /// Is the element expanded in the viewer.
        /// </summary>
        bool IsExpanded { get; set; }
    }
}
