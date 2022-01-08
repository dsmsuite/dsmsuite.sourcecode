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

        IDictionary<string, string> Properties { get; }

        /// <summary>
        /// Full name of the element based on its position in the element hierarchy
        /// </summary>
        string Fullname { get; }

        string GetRelativeName(IDsmElement element);

        bool IsDeleted { get; }
        bool IsBookmarked { get; set; }
        bool IsRoot { get; }

        /// <summary>
        /// Has the element any children.
        /// </summary>
        bool HasChildren { get; }

        /// <summary>
        /// Children of the element.
        /// </summary>
        IList<IDsmElement> Children { get; }

        IList<IDsmElement> AllChildren { get; }

        int IndexOfChild(IDsmElement child);

        bool ContainsChildWithName(string name);

        /// <summary>
        /// Parent of the element.
        /// </summary>
        IDsmElement Parent { get; }

        /// <summary>
        /// Is the selected element a recursive child of this element.
        /// </summary>
        bool IsRecursiveChildOf(IDsmElement element);

        /// <summary>
        /// Is the element expanded in the viewer.
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Is the element match in search.
        /// </summary>
        bool IsMatch{ get; set; }

        /// <summary>
        /// Is the element included in the tree
        /// </summary>
        bool IsIncludedInTree { get; set; }
    }
}
