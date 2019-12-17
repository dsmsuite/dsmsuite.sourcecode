using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Model.Core
{
    public class DsmAction : IDsmAction
    {
        public DsmAction(int id, string type, string data)
        {
            Id = id;
            Type = type;
            Data = data;
        }

        public int Id { get; }

        public string Type { get; }

        public string Data { get; }
    }
}
