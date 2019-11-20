using System.Linq;
using System.Xml;
using DsmSuite.DsmViewer.Model;

namespace DsmSuite.DsmViewer.Reporting
{
    public class ElementConsumersReport : BaseReport
    {
        private IElement _element;

        public ElementConsumersReport(IDsmModel model, IElement element) : 
            base(model)
        {
            _element = element;
        }

        protected override void WriteReportData()
        {
            var relations = _model.FindProviderRelations(_element);

            AddHeader(1, $"Consumers of {_element.Fullname}");
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
