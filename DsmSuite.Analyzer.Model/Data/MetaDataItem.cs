using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Analyzer.Model.Data
{
    class MetaDataItem : IMetaDataItem
    {
        public MetaDataItem(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }

        public string Value { get; }
    }
}
