using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Analyzer.Model.Core
{
    /// <summary>
    /// Represents element of a component. Both the ElementId and Name uniquely identify an element.
    /// </summary>
    public class DsiElement : IDsiElement
    {
        public DsiElement(int id, string name, string type, IDictionary<string, string> properties)
        {
            Id = id;
            Name = name;
            Type = type;
            Properties = properties;
        }

        public int Id { get; }
        public string Name { get; set; }
        public string Type { get; }
        public IDictionary<string, string> Properties { get; }
    }
}
