using System.Collections.Generic;

namespace DsmSuite.Analyzer.Model.Interface
{
    /// <summary>
    /// Interface to the data model. An interface has been introduced to improve testability.
    /// </summary>
    public interface IDsiDataModel
    {
        void Load(string dsiFilename);
        void Save(string dsiFilename, bool compressFile);

        void AddMetaData(string itemName, string itemValue);
        IEnumerable<string> GetMetaDataGroups();
        IEnumerable<IDsiMetaDataItem> GetMetaDataGroupItems(string groupName);

        IDsiElement CreateElement(string name, string type, string source);
        void RemoveElement(IDsiElement element);
        void RenameElement(IDsiElement element, string newName);
        IDsiElement FindElement(int id);
        IDsiElement FindElement(string name);
        IEnumerable<IDsiElement> GetElements();
        int TotalElementCount { get; }
        double ResolvedRelationPercentage { get; }
        
        ICollection<string> GetElementTypes();
        int GetElementTypeCount(string type);

        IDsiRelation AddRelation(string consumerName, string providerName, string type, int weight, string context);
        void SkipRelation(string consumerName, string providerName, string type, string context);
        ICollection<IDsiRelation> GetProviderRelations(IDsiElement consumer);
        IEnumerable<IDsiRelation> GetRelations();
        bool DoesRelationExist(IDsiElement consumer, IDsiElement provider);
        int TotalRelationCount { get; }
        int ResolvedRelationCount { get; }

        ICollection<string> GetRelationTypes();
        int GetRelationTypeCount(string type);

        void Cleanup();
    }
}
