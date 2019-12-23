namespace DsmSuite.DsmViewer.Model.Interfaces
{
    public interface ISortResult
    {
        int GetIndex(int currentIndex);
        int GetNumberOfElements();
        bool IsValid { get; }
    }
}
