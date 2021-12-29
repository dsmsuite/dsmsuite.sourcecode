using System;
using System.Collections.Generic;
using DsmSuite.Common.Model.Interface;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Import.Common
{
    public interface IDsmBuilder
    {
        IMetaDataItem ImportMetaDataItem(string group, string name, string value);
        IDsmElement ImportElement(string fullname, string name, string type, IDsmElement parent, IDictionary<string, string> properties);
        IDsmRelation ImportRelation(int consumerId, int providerId, string type, int weight, IDictionary<string, string> properties);
        void FinalizeImport(IProgress<ProgressInfo> progress);
    }
}
