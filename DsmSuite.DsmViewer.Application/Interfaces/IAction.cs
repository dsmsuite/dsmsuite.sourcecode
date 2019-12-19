using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Interfaces
{
    public interface IAction
    {
        string ActionName { get; }
        string Title { get; }
        string Description { get; }

        IReadOnlyDictionary<string, string> Pack();
    }
}
