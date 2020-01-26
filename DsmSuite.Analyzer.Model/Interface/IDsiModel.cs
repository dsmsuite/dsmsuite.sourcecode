using DsmSuite.Common.Model.Interface;
using DsmSuite.Common.Util;
using System;
using System.Collections.Generic;

namespace DsmSuite.Analyzer.Model.Interface
{
    /// <summary>
    /// Interface to the data model. An interface has been introduced to improve testability.
    /// </summary>
    public interface IDsiModel
    {
        // Model persistency
        void Load(string dsiFilename,IProgress<ProgressInfo> progress);
        void Save(string dsiFilename, bool compressFile, IProgress<ProgressInfo> progress);

        // Meta data
        void AddMetaData(string name, string value);
        IEnumerable<string> GetMetaDataGroups();
        IEnumerable<IMetaDataItem> GetMetaDataGroupItems(string groupName);

        // Element editing
        IDsiElement AddElement(string name, string type, string source);
        void RemoveElement(IDsiElement element);
        void RenameElement(IDsiElement element, string newName);

        // Element queries
        IDsiElement FindElementById(int id);
        IDsiElement FindElementByName(string name);
        IEnumerable<IDsiElement> GetElements();
        int GetElementCount();
        int TotalElementCount { get; }
        double ResolvedRelationPercentage { get; }
        ICollection<string> GetElementTypes();
        int GetElementTypeCount(string type);

        // Relation editing
        IDsiRelation AddRelation(string consumerName, string providerName, string type, int weight, string context);
        void SkipRelation(string consumerName, string providerName, string type, string context);

        // Relation queries
        ICollection<IDsiRelation> GetRelationsOfConsumer(int consumerId);
        IEnumerable<IDsiRelation> GetRelations();
        int GetRelationCount();
        bool DoesRelationExist(int consumerId, int providerId);
        int TotalRelationCount { get; }
        int ResolvedRelationCount { get; }
        ICollection<string> GetRelationTypes();
        int GetRelationTypeCount(string type);
    }
}
