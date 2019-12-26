using DsmSuite.Common.Model.Interface;
using System.Collections.Generic;

namespace DsmSuite.Analyzer.Model.Interface
{
    /// <summary>
    /// Interface to the data model. An interface has been introduced to improve testability.
    /// </summary>
    public interface IDsiModel
    {
        void Load(string dsiFilename);
        void Save(string dsiFilename, bool compressFile);

        void AddMetaData(string name, string value);
        IEnumerable<string> GetMetaDataGroups();
        IEnumerable<IMetaDataItem> GetMetaDataGroupItems(string groupName);

        IDsiElement AddElement(string name, string type, string source);
        void RemoveElement(IDsiElement element);
        void RenameElement(IDsiElement element, string newName);
        IDsiElement FindElementById(int id);
        IDsiElement FindElementByName(string name);
        IEnumerable<IDsiElement> GetElements();
        int TotalElementCount { get; }
        double ResolvedRelationPercentage { get; }
        
        ICollection<string> GetElementTypes();
        int GetElementTypeCount(string type);

        IDsiRelation AddRelation(string consumerName, string providerName, string type, int weight, string context);
        void SkipRelation(string consumerName, string providerName, string type, string context);
        ICollection<IDsiRelation> GetRelationsOfConsumer(int consumerId);
        IEnumerable<IDsiRelation> GetRelations();
        bool DoesRelationExist(int consumerId, int providerId);
        int TotalRelationCount { get; }
        int ResolvedRelationCount { get; }

        ICollection<string> GetRelationTypes();
        int GetRelationTypeCount(string type);
    }
}
