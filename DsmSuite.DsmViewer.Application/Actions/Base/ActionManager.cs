using System;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Model.Actions.Base
{
    public class ActionManager
    {
        private readonly List<IAction> _actions;
        private int? _currentActionIndex;

        public ActionManager()
        {
            _actions = new List<IAction>();
            _currentActionIndex = null;
        }
        public void Execute(IAction action)
        {
            ClearAllAfterCurrent();

            _actions.Add(action);
            _currentActionIndex = _actions.Count - 1;
            action.Do();
        }

        public void Undo()
        {
            if (_currentActionIndex.HasValue)
            {
                _actions[_currentActionIndex.Value].Undo();
            }
        }

        public void Redo()
        {
            if (_currentActionIndex.HasValue)
            {
                _actions[_currentActionIndex.Value].Do();
            }
        }

        public void ClearAll()
        {
            _actions.Clear();
        }

        public void ClearAllAfterCurrent()
        {
            if (_currentActionIndex.HasValue)
            {
                int next = _currentActionIndex.Value + 1;
                int count = _actions.Count - next;
                _actions.RemoveRange(next, count);
            }
        }

        public IEnumerable<string> GetActionDescriptions()
        {
            List<string> actionDescriptions = new List<string>();
            foreach (IAction action in _actions)
            {
                actionDescriptions.Add(action.Description);
            }
            return actionDescriptions;
        }
    }
}
