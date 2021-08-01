using DsmSuite.Common.Model.Interface;

namespace DsmSuite.Common.Model.Core
{
    public class MetaDataItem : IMetaDataItem
    {
        public MetaDataItem(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; set; }
    }
}
