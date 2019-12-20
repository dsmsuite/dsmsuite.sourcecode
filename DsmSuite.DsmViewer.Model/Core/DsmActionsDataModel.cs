using System.Collections.Generic;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Core
{
    public class DsmActionsDataModel
    {
        private readonly List<IDsmAction> _actions;

        public DsmActionsDataModel()
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

        public void ClearActions()
        {
            _actions.Clear();
        }

        public IEnumerable<IDsmAction> GetActions()
        {
            return _actions;
        }

        public int GetActionCount()
        {
            return _actions.Count;
        }
    }
}
