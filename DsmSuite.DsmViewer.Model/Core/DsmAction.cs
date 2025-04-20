using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Core
{
    public class DsmAction : IDsmAction
    {
        public DsmAction(int id, string type, IReadOnlyDictionary<string, string> data)
        {
            Id = id;
            Type = type;
            Data = data;
        }

        public int Id { get; }

        public string Type { get; }

        public IReadOnlyDictionary<string, string> Data { get; }
    }
}
