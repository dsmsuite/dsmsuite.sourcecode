using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Metrics
{
    public class DsmMetrics
    {
        public int GetElementSize(IDsmElement element)
        {
            int count = 0;
            CountChildren(element, ref count);
            return count;
        }

        private void CountChildren(IDsmElement element, ref int count)
        {
            count++;

            foreach (IDsmElement child in element.Children)
            {
                CountChildren(child, ref count);
            }
        }
    }
}
