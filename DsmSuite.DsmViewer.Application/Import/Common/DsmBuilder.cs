using DsmSuite.Common.Model.Interface;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Import.Common
{
    public class DsmBuilder : IDsmBuilder
    {
        private readonly IDsmModel _dsmModel;

        public DsmBuilder(IDsmModel dsmModel)
        {
            _dsmModel = dsmModel;
            _dsmModel.Clear();
        }

        public IMetaDataItem ImportMetaDataItem(string group, string name, string value)
        {
            return _dsmModel.AddMetaData(group, name, value);
        }

        public IDsmElement ImportElement(string fullname, string name, string type, IDsmElement parent, IDictionary<string, string> properties)
        {
            IDsmElement element = _dsmModel.GetElementByFullname(fullname);
            if (element == null)
            {
                int? parentId = parent?.Id;
                element = _dsmModel.AddElement(name, type, parentId, 0, properties);
            }
            return element;
        }

        public IDsmRelation ImportRelation(int consumerId, int providerId, string type, int weight, IDictionary<string, string> properties)
        {
            IDsmElement consumer = _dsmModel.GetElementById(consumerId);
            IDsmElement provider = _dsmModel.GetElementById(providerId);

            return _dsmModel.AddRelation(consumer, provider, type, weight, properties);
        }

        public void FinalizeImport(IProgress<ProgressInfo> progress)
        {
            _dsmModel.AssignElementOrder();
        }
    }
}
