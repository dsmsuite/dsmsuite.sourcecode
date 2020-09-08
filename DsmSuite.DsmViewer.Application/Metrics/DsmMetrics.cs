using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Metrics
{
    public class DsmMetrics
    {
        public int GetElementSize(IDsmElement element)
        {
            int count = 0;
            CountChildern(element, ref count);
            return count;
        }

        private void CountChildern(IDsmElement element, ref int count)
        {
            count++;

            foreach (IDsmElement child in element.Children)
            {
                CountChildern(child, ref count);
            }
        }
    }
}
