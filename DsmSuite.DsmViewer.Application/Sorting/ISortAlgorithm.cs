using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Algorithm
{
    public interface ISortAlgorithm
    {
        ISortResult Sort();
        string Name { get; }
    }
}
