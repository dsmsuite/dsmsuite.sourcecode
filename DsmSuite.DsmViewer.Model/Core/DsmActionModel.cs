using System.Collections.Generic;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Model.Persistency;

namespace DsmSuite.DsmViewer.Model.Core
{
    public class DsmActionModel : IDsmActionModelFileCallback
    {
        private readonly List<IDsmAction> _actions;

        public DsmActionModel()
        {
            _actions = new List<IDsmAction>();
        }

        public void Clear()
        {
            _actions.Clear();
        }

        public IDsmAction ImportAction(int id, string type, IReadOnlyDictionary<string, string> data)
        {
            IDsmAction action = new DsmAction(id, type, data);
            _actions.Add(action);
            return action;
        }

        public IDsmAction AddAction(int id, string type, IReadOnlyDictionary<string, string> data)
        {
            IDsmAction action = new DsmAction(id, type, data);
            _actions.Add(action);
            return action;
        }

        public IEnumerable<IDsmAction> GetExportedActions()
        {
            return _actions;
        }

        public int GetExportedActionCount()
        {
            return _actions.Count;
        }
    }
}
