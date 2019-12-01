using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Analyzer.Model.Persistency
{
    public interface IDsiModelFileReaderCallback
    {
        void ReadMetaDataGroup(string groupName);
        void ReadMetaDataItem(string groupName, IMetaDataItem metaDataItem);
        int ReadElement(IElement element);
        int ReadRelation(IRelation relation);
    }
}
