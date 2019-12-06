using System;
using System.Collections.Generic;
using DsmSuite.Common.Util;

namespace DsmSuite.DsmViewer.Application.Actions.Base
{
    public class ActionManager
    {
        private readonly Stack<IAction> _undoActionStack;
        private readonly Stack<IAction> _redoActionStack;

        public event EventHandler ActionPerformned;

        public ActionManager()
        {
            _undoActionStack = new Stack<IAction>();
            _redoActionStack = new Stack<IAction>();
        }

        public void Execute(IAction action)
        {
            _undoActionStack.Push(action);
            _redoActionStack.Clear();
            action.Do();
            ActionPerformned?.Invoke(this, EventArgs.Empty);
            Logger.LogInfo("Do :{action.Description}");
        }

        public bool CanUndo()
        {
            return _undoActionStack.Count > 0;
        }

        public string GetUndoActionDescription()
        {
            string description = ""; ;
            if (_undoActionStack.Count > 0)
            {
                description = _undoActionStack.Peek().Description;
            };
            return description;
        }

        public void Undo()
        {
            if (_undoActionStack.Count > 0)
            {
                IAction action = _undoActionStack.Pop();
                if (action != null)
                {
                    _redoActionStack.Push(action);
                    action.Undo();
                    ActionPerformned?.Invoke(this, EventArgs.Empty);
                    Logger.LogInfo("Undo :{action.Description}");
                }
            }
        }

        public bool CanRedo()
        {
            return _redoActionStack.Count > 0;
        }

        public string GetRedoActionDescription()
        {
            string description = ""; ;
            if (_redoActionStack.Count > 0)
            {
                description = _redoActionStack.Peek().Description;
            };
            return description;
        }

        public void Redo()
        {
            if (_redoActionStack.Count > 0)
            {
                IAction action = _redoActionStack.Pop();
                if (action != null)
                {
                    _undoActionStack.Push(action);
                    action.Do();
                    ActionPerformned?.Invoke(this, EventArgs.Empty);
                    Logger.LogInfo("Redo :{action.Description}");
                }
            }
        }

        public void ClearAll()
        {
            _undoActionStack.Clear();
            _redoActionStack.Clear();
        }

        public IEnumerable<string> GetActionDescriptions()
        {
            List<string> actionDescriptions = new List<string>();
            foreach (IAction action in _undoActionStack)
            {
                actionDescriptions.Add(action.Description);
            }
            return actionDescriptions;
        }


    }
}
