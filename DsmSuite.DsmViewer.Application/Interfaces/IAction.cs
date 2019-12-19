using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Interfaces
{
    public interface IAction
    {
        string ClassName { get; }
        string Title { get; }
        string Details { get; }
        string Description { get; }

        IReadOnlyDictionary<string, string> Pack();
        IAction Unpack(IReadOnlyDictionary<string, string> data);
    }
}
