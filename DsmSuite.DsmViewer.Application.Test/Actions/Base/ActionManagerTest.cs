using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Application.Actions.Base;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Base
{
    [TestClass]
    public class ActionManagerTest
    {
        class ActionStub : ActionBase
        {
            public int _doCount;
            public int _undoCount;

            public ActionStub(string actionName) : base(null)
            {
                ActionName = actionName;
            }

            public override string ActionName { get; }
            public override string Description => "Description";
            public override string Title => "Title";
            public override void Do()
            {
                _doCount++;
            }

            public override void Undo()
            {
                _undoCount++;
            }

            public override IReadOnlyDictionary<string, string> Pack()
            {
                return new Dictionary<string, string>();
            }

            public int DoCount => _doCount;
            public int UndoCount => _undoCount;
        }

        [TestMethod]
        public void WhenConstructedThenNoActionsCanBeUndone()
        {
            ActionManager manager = new ActionManager();
            Assert.IsFalse(manager.CanUndo());
            Assert.IsNull(manager.GetCurrentUndoAction());
        }

        [TestMethod]
        public void WhenConstructedThenNoActionsCanBeRedone()
        {
            ActionManager manager = new ActionManager();
            Assert.IsFalse(manager.CanRedo());
            Assert.IsNull(manager.GetCurrentRedoAction());
        }

        [TestMethod]
        public void WhenActionIsExecutedThenActionIsExecuted()
        {
            ActionManager manager = new ActionManager();
            ActionStub action = new ActionStub("action1");
            manager.Execute(action);
            Assert.AreEqual(1, action.DoCount);
        }

        [TestMethod]
        public void WhenActionIsExecutedThenActionPerformedEventIsFired()
        {
            bool eventFired = false;

            ActionManager manager = new ActionManager();
            manager.ActionPerformed += (o, i) =>
            {
                eventFired = true;
            };

            ActionStub action = new ActionStub("action1");
            manager.Execute(action);

            Assert.IsTrue(eventFired);
        }

        [TestMethod]
        public void WhenActionIsExecutedThenActionCanBeUndone()
        {
            ActionManager manager = new ActionManager();
            ActionStub action = new ActionStub("action1");
            manager.Execute(action);
            Assert.IsTrue(manager.CanRedo());
            Assert.IsNotNull(manager.GetCurrentRedoAction());
        }

        [TestMethod]
        public void WhenActionIsAddedThenActionIsNotExecuted()
        {
            ActionManager manager = new ActionManager();
            ActionStub action = new ActionStub("action1");
            manager.Execute(action);
            Assert.AreEqual(0, action.DoCount);
        }

        [TestMethod]
        public void WhenActionIsAddThenActionCanBeUndone()
        {
            ActionManager manager = new ActionManager();
            ActionStub action = new ActionStub("action1");
            manager.Execute(action);
            Assert.IsTrue(manager.CanRedo());
            Assert.IsNotNull(manager.GetCurrentRedoAction());
        }
    }
}
