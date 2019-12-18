using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Model.Interfaces
{
    public interface IDsmAction
    {
        int Id { get; }

        string Type { get; }

        IDictionary<string,string> Data { get; }
    }
}
