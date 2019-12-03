using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Data
{
    public class DsmMetaDataItem : IDsmMetaDataItem
    {
        public DsmMetaDataItem(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; }
    }
}
