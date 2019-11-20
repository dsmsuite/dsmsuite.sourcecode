
using System.Collections.Generic;

namespace DsmSuite.Analyzer.Data
{
    /// <summary>
    /// Represents element of a component. Both the ElementId and Name uniquely identify an element.
    /// </summary>
    public class Element : IElement
    {
        private readonly List<IRelation> _providerRelations = new List<IRelation>();

        public Element(int elementId, string name, string type, string soure){
            ElementId = elementId;
            Name = name;
            Type = type;
            Source = soure;
        }

        public IRelation AddRelation(IElement provider, string type, int strength)
        {
            Relation relation = new Relation(provider, this, type, strength);
            _providerRelations.Add(relation);
            return relation;
        }

        public ICollection<IRelation> Providers => _providerRelations;

        public int ElementId { get; }

        public string Name {
            get;
            set;
        }
        public string Type { get; }
        public string Source { get; }

        public override int GetHashCode()
        {
            return ElementId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Element)obj);
        }

        private bool Equals(Element other)
        {
            return string.Equals(Name, other.Name);
        }
    }
}
