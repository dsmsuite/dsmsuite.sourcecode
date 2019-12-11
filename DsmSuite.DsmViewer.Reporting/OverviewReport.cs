using System.IO;
using System.Xml;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.Common.Model.Interface;

namespace DsmSuite.DsmViewer.Reporting
{
    public class OverviewReport : BaseReport
    {
        public OverviewReport(IDsmModel model) : base(model)
        {
        }

        protected override void WriteReportData()
        {
            AddHeader(1, "Overview");
            AddHeader(2, "Meta data");

            foreach (string group in Model.GetMetaDataGroups())
            {
                AddHeader(3, group);

                XmlNode tbody = CreateTable();
                foreach (IMetaDataItem item in Model.GetMetaDataGroupItems(group))
                {
                    string[] cells = { item.Name, ": " , item.Value };
                    AddTableRow(tbody, cells);
                }
            }
        }

        protected override void WriteHtmlStyles(StringWriter sw)
        {
            base.WriteHtmlStyles(sw);
            sw.WriteLine("td      { width: 600px; }");
        }
    }
}
