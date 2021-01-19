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
        string Filename { get; }

        // Model persistency
        void Load(string dsiFilename,IProgress<ProgressInfo> progress);
        void Save(string dsiFilename, bool compressFile, IProgress<ProgressInfo> progress);

        // Meta data
        void AddMetaData(string name, string value);
        IEnumerable<string> GetMetaDataGroups();
        IEnumerable<IMetaDataItem> GetMetaDataGroupItems(string groupName);

        // Element editing
        IDsiElement AddElement(string name, string type, string annotation);
        void RemoveElement(IDsiElement element);
        void RenameElement(IDsiElement element, string newName);

        // Element queries
        IDsiElement FindElementById(int id);
        IDsiElement FindElementByName(string name);
        IEnumerable<IDsiElement> GetElements();
        int CurrentElementCount { get; }
        double ResolvedRelationPercentage { get; }
        ICollection<string> GetElementTypes();
        int GetElementTypeCount(string type);

        // Relation editing
        IDsiRelation AddRelation(string consumerName, string providerName, string type, int weight, string annotation);
        void SkipRelation(string consumerName, string providerName, string type);
        void AmbiguousRelation(string consumerName, string providerName, string type);
        // Relation queries
        ICollection<IDsiRelation> GetRelationsOfConsumer(int consumerId);
        IEnumerable<IDsiRelation> GetRelations();
        int CurrentRelationCount { get; }
        bool DoesRelationExist(int consumerId, int providerId);
        int ImportedRelationCount { get; }
        int ResolvedRelationCount { get; }
        ICollection<string> GetRelationTypes();
        int GetRelationTypeCount(string type);
    }
}
