using System.Linq;
using System.Xml;
using DsmSuite.DsmViewer.Model;

namespace DsmSuite.DsmViewer.Reporting
{
    public class ElementRequiredInterfacesReport : BaseReport
    {
        private IElement _element;

        public ElementRequiredInterfacesReport(IDsmModel model, IElement element) : 
            base(model)
        {
            _element = element;
        }

        protected override void WriteReportData()
        {
            var relations = _model.FindConsumerRelations(_element);

            AddHeader(1, $"Required interface of {_element.Fullname}");
            string[] headers = { "Nr", "Required name", "Required type" };
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
