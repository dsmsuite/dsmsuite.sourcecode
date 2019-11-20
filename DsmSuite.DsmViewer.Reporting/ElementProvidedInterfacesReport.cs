using System.Linq;
using System.Xml;
using DsmSuite.DsmViewer.Model;

namespace DsmSuite.DsmViewer.Reporting
{
    public class ElementProvidedInterfacesReport : BaseReport
    {
        private IElement _element;

        public ElementProvidedInterfacesReport(IDsmModel model, IElement element) : 
            base(model)
        {
            _element = element;
        }

        protected override void WriteReportData()
        {
            var relations = _model.FindProviderRelations(_element);

            AddHeader(1, $"Provided interface of {_element.Fullname}");
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
