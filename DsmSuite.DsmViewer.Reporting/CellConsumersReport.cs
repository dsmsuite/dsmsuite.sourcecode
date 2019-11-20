using System.Xml;
using DsmSuite.DsmViewer.Model;
using System.Linq;

namespace DsmSuite.DsmViewer.Reporting
{
    public class CellConsumersReport : BaseReport
    {
        private IElement _provider;
        private IElement _consumer;

        public CellConsumersReport(IDsmModel model, IElement provider, IElement consumer) : 
            base(model)
        {
            _provider = provider;
            _consumer = consumer;
        }

        protected override void WriteReportData()
        {
            var relations = _model.FindRelations(_consumer, _provider);

            AddHeader(1, $"Consumers in relations between consumer {_consumer.Fullname} and provider {_provider.Fullname}");
            string[] headers = { "Nr", "Consumer name", "Consumer type" };
            XmlNode tbody = CreateTable(headers);

            int index = 1;
            foreach (IRelation relation in relations.OrderBy(x => x.Consumer.Fullname).GroupBy(x => x.Consumer.Fullname).Select(x => x.FirstOrDefault()).ToList())
            {
                string[] cells = { index.ToString(), relation.Consumer.Fullname, relation.Consumer.Type };
                AddTableRow(tbody, cells);
                index++;
            }
        }
    }
}
