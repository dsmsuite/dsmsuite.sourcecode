using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Analyzer.Model.Data
{
    /// <summary>
    /// Represents element of a component. Both the ElementId and Name uniquely identify an element.
    /// </summary>
    public class Element : IElement
    {
        public Element(int elementId, string name, string type, string soure){
            ElementId = elementId;
            Name = name;
            Type = type;
            Source = soure;
        }

        public int ElementId { get; }

        public string Name { get; set;}

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
