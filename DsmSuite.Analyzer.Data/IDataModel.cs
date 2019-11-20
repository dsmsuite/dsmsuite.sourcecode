using System.Collections.Generic;

namespace DsmSuite.Analyzer.Data
{
    /// <summary>
    /// Interface to the data model. An interface has been introduced to improve testability.
    /// </summary>
    public interface IDataModel
    {
        ICollection<string> GetElementTypes();
        int GetElementTypeCount(string type);
        ICollection<string> GetRelationTypes();
        int GetRelationTypeCount(string type);

        void AddMetaData(string name, string value);

        void Load(string dsiFilename);
        void Save(string dsiFilename, bool compressFile);

        IElement AddElement(string name, string type, string source);
        void RemoveElement(string name);
        void RenameElement(IElement element, string newName);
        IElement FindElement(string name);
        ICollection<IElement> Elements { get; }
        int TotalElementCount { get; }
        double ResolvedRelationPercentage { get; }

        IRelation AddRelation(string consumerName, string providerName, string type, int weight, string context);
        void SkipRelation(string consumerName, string providerName, string type, string context);
        ICollection<IRelation> GetProviderRelations(IElement consumer);
        bool DoesRelationExist(IElement consumer, IElement provider);
        int TotalRelationCount { get; }
        int ResolvedRelationCount { get; }

        void Cleanup();
    }
}
