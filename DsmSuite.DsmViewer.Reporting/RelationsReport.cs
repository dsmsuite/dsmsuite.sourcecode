using System.Linq;
using System.Xml;
using DsmSuite.DsmViewer.Model;

namespace DsmSuite.DsmViewer.Reporting
{
    public class RelationsReport : BaseReport
    {
        private IElement _provider;
        private IElement _consumer;

        public RelationsReport(IDsmModel model, IElement provider, IElement consumer) : 
            base(model)
        {
            _provider = provider;
            _consumer = consumer;
        }

        protected override void WriteReportData()
        {
            var relations = _model.FindRelations(_consumer, _provider);

            AddHeader(1, $"Relations between consumer {_consumer.Fullname} and provider {_provider.Fullname}");
            string[] headers = { "Nr", "Consumer name", "Provider name", "Relation type", "Weight", "Cyclic" };
            XmlNode tbody = CreateTable(headers);

            int index = 1;
            foreach (IRelation relation in relations.OrderBy(x => x.Provider.Fullname).ThenBy(x => x.Consumer.Fullname))
            {
                string cyclic = _model.IsCyclicDependency(relation.Consumer, relation.Provider) ? "yes" : "-";
                string[] cells = { index.ToString(), relation.Consumer.Fullname, relation.Provider.Fullname, relation.Type, relation.Weight.ToString(), cyclic };
                AddTableRow(tbody, cells);
                index++;
            }
        }
    }
}
