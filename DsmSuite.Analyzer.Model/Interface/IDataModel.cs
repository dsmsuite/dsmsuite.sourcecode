using System.Collections.Generic;

namespace DsmSuite.Analyzer.Model.Interface
{
    /// <summary>
    /// Interface to the data model. An interface has been introduced to improve testability.
    /// </summary>
    public interface IDataModel
    {
        void Load(string dsiFilename);
        void Save(string dsiFilename, bool compressFile);

        void ImportMetaDataItem(string groupName, IMetaDataItem metaDataItem);
        void AddMetaData(string itemName, string itemValue);
        IEnumerable<string> GetMetaDataGroups();
        IEnumerable<IMetaDataItem> GetMetaDataGroupItems(string groupName);

        void ImportElement(IElement element);
        IElement AddElement(string name, string type, string source);
        void RemoveElement(IElement element);
        void RenameElement(IElement element, string newName);
        IElement FindElement(int id);
        IElement FindElement(string name);
        IEnumerable<IElement> GetElements();
        int TotalElementCount { get; }
        double ResolvedRelationPercentage { get; }
        
        ICollection<string> GetElementTypes();
        int GetElementTypeCount(string type);

        void ImportRelation(IRelation relation);
        IRelation AddRelation(string consumerName, string providerName, string type, int weight, string context);
        void SkipRelation(string consumerName, string providerName, string type, string context);
        ICollection<IRelation> GetProviderRelations(IElement consumer);
        IEnumerable<IRelation> GetRelations();
        bool DoesRelationExist(IElement consumer, IElement provider);
        int TotalRelationCount { get; }
        int ResolvedRelationCount { get; }

        ICollection<string> GetRelationTypes();
        int GetRelationTypeCount(string type);

        void Cleanup();
    }
}
