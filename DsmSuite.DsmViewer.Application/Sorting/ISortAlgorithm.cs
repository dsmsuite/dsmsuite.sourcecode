using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Algorithm
{
    public interface ISortAlgorithm
    {
        SortResult Sort();
        string Name { get; }
    }
}
