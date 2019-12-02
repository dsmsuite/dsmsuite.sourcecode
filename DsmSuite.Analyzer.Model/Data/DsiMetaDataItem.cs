using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Analyzer.Model.Data
{
    public class DsiMetaDataItem : IDsiMetaDataItem
    {
        public DsiMetaDataItem(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; }
    }
}
