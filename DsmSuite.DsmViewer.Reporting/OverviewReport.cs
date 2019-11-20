using System.IO;
using System.Xml;
using DsmSuite.DsmViewer.Model;

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

            foreach (string group in Model.GetGroups())
            {
                AddHeader(3, group);

                XmlNode tbody = CreateTable();
                foreach (string name in Model.GetNames(group))
                {
                    string[] cells = { name, ": " + Model.GetValue(group, name) };
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
