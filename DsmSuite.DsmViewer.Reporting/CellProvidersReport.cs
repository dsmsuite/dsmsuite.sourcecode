using System.Linq;
using System.Xml;
using DsmSuite.DsmViewer.Model;

namespace DsmSuite.DsmViewer.Reporting
{
    public class CellProvidersReport : BaseReport
    {
        private IElement _provider;
        private IElement _consumer;

        public CellProvidersReport(IDsmModel model, IElement provider, IElement consumer) : 
            base(model)
        {
            _provider = provider;
            _consumer = consumer;
        }

        protected override void WriteReportData()
        {
            var relations = _model.FindRelations(_consumer, _provider);

            AddHeader(1, $"Providers in relations between consumer {_consumer.Fullname} and provider {_provider.Fullname}");
            string[] headers = { "Nr", "Provider name", "Provider type" };
            XmlNode tbody = CreateTable(headers);

            int index = 1;
            foreach (IRelation relation in relations.OrderBy(x => x.Provider.Fullname).GroupBy(x => x.Provider.Fullname).Select(x => x.FirstOrDefault()).ToList())
            {
                string[] cells = { index.ToString(), relation.Provider.Fullname, relation.Provider.Type };
                AddTableRow(tbody, cells);
                index++;
            }
        }
    }
}
