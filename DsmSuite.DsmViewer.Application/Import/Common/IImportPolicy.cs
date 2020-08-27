using System;
using DsmSuite.Common.Model.Interface;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Import.Common
{
    public interface IImportPolicy
    {
        IMetaDataItem ImportMetaDataItem(string group, string name, string value);
        IDsmElement ImportElement(string fullname, string name, string type, IDsmElement parent);
        IDsmRelation ImportRelation(int consumerId, int providerId, string type, int weight);
        void FinalizeImport(IProgress<ProgressInfo> progress);
    }
}
