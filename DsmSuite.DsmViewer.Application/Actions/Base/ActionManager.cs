﻿using System;
using System.Collections.Generic;
using DsmSuite.Common.Util;

namespace DsmSuite.DsmViewer.Application.Actions.Base
{
    public class ActionManager
    {
        private readonly Stack<ActionBase> _undoActionStack;
        private readonly Stack<ActionBase> _redoActionStack;

        public event EventHandler ActionPerformed;

        public ActionManager()
        {
            _undoActionStack = new Stack<ActionBase>();
            _redoActionStack = new Stack<ActionBase>();
        }

        public void Execute(ActionBase action)
        {
            _undoActionStack.Push(action);
            _redoActionStack.Clear();
            action.Do();
            ActionPerformed?.Invoke(this, EventArgs.Empty);
            Logger.LogInfo("Do :{action.Description}");
        }

        public bool CanUndo()
        {
            return _undoActionStack.Count > 0;
        }

        public IAction GetCurrentUndoAction()
        {
            IAction action = null;
            if (_undoActionStack.Count > 0)
            {
                action = _undoActionStack.Peek();
            }
            return action;
        }

        public void Undo()
        {
            if (_undoActionStack.Count > 0)
            {
                ActionBase action = _undoActionStack.Pop();
                if (action != null)
                {
                    _redoActionStack.Push(action);
                    action.Undo();
                    ActionPerformed?.Invoke(this, EventArgs.Empty);
                    Logger.LogInfo("Undo :{action.Description}");
                }
            }
        }

        public bool CanRedo()
        {
            return _redoActionStack.Count > 0;
        }

        public IAction GetCurrentRedoAction()
        {
            IAction action = null;
            if (_redoActionStack.Count > 0)
            {
                action = _redoActionStack.Peek();
            }
            return action;
        }

        public void Redo()
        {
            if (_redoActionStack.Count > 0)
            {
                ActionBase action = _redoActionStack.Pop();
                if (action != null)
                {
                    _undoActionStack.Push(action);
                    action.Do();
                    ActionPerformed?.Invoke(this, EventArgs.Empty);
                    Logger.LogInfo("Redo :{action.Description}");
                }
            }
        }

        public void ClearAll()
        {
            _undoActionStack.Clear();
            _redoActionStack.Clear();
        }

        public IEnumerable<IAction> GetUndoActions()
        {
            return _undoActionStack;
        }
    }
}
