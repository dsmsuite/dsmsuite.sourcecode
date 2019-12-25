using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Sorting
{
    public interface ISortAlgorithm
    {
        SortResult Sort();
        string Name { get; }
    }
}
