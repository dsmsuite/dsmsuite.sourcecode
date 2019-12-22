using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Application.Actions.Base;
using System.Collections.Generic;
using Moq;
using DsmSuite.DsmViewer.Application.Interfaces;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Base
{
    [TestClass]
    public class ActionManagerTest
    {
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
        public void WhenActionIsExecutedThenDoActionIsExecuted()
        {
            ActionManager manager = new ActionManager();
            Mock<IAction> actionMock = new Mock<IAction>();
            manager.Execute(actionMock.Object);
            actionMock.Verify(x => x.Do(), Times.Once());
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

            Mock<IAction> actionMock = new Mock<IAction>();
            manager.Execute(actionMock.Object);

            Assert.IsTrue(eventFired);
        }

        [TestMethod]
        public void WhenActionIsExecutedThenActionCanBeUndone()
        {
            ActionManager manager = new ActionManager();
            Mock<IAction> actionMock = new Mock<IAction>();
            manager.Execute(actionMock.Object);
            Assert.IsTrue(manager.CanUndo());
            Assert.AreEqual(actionMock.Object, manager.GetCurrentUndoAction());
        }

        [TestMethod]
        public void GivenTwoActionsWhereExecutedWhenActionRetrievedThenTheyAreReturnedInReverseChronologicalOrder()
        {
            ActionManager manager = new ActionManager();
            Mock<IAction> actionMock1 = new Mock<IAction>();
            manager.Execute(actionMock1.Object);
            Mock<IAction> actionMock2 = new Mock<IAction>();
            manager.Execute(actionMock2.Object);

            List<IAction> actions = manager.GetUndoActions().ToList();
            Assert.AreEqual(2, actions.Count);
            Assert.AreEqual(actionMock2.Object, actions[0]);
            Assert.AreEqual(actionMock1.Object, actions[1]);
        }

        [TestMethod]
        public void GivenTwoActionsWhereExecutedAndOneUndoneWhenActionAreClearedThenNoActionsAreLeftToUndoOrRedo()
        {
            ActionManager manager = new ActionManager();
            Mock<IAction> actionMock1 = new Mock<IAction>();
            manager.Execute(actionMock1.Object);
            Mock<IAction> actionMock2 = new Mock<IAction>();
            manager.Execute(actionMock2.Object);
            manager.Undo();

            Assert.IsTrue(manager.CanUndo());
            Assert.AreEqual(actionMock1.Object, manager.GetCurrentUndoAction());
            Assert.IsTrue(manager.CanRedo());
            Assert.AreEqual(actionMock2.Object, manager.GetCurrentRedoAction());

            manager.Clear();

            Assert.IsFalse(manager.CanUndo());
            Assert.IsNull(manager.GetCurrentUndoAction());
            Assert.IsFalse(manager.CanRedo());
            Assert.IsNull(manager.GetCurrentRedoAction());
        }

        [TestMethod]
        public void WhenActionIsUndoneThenUndoActionIsExecuted()
        {
            ActionManager manager = new ActionManager();
            Mock<IAction> actionMock = new Mock<IAction>();
            manager.Execute(actionMock.Object);

            manager.Undo();
            actionMock.Verify(x => x.Undo(), Times.Once());
        }

        [TestMethod]
        public void WhenActionIsUndoneThenActionCanBeRedone()
        {
            ActionManager manager = new ActionManager();
            Mock<IAction> actionMock = new Mock<IAction>();
            manager.Execute(actionMock.Object);
            Assert.IsTrue(manager.CanUndo());
            Assert.IsNotNull(manager.GetCurrentUndoAction());
            Assert.IsFalse(manager.CanRedo());
            Assert.IsNull(manager.GetCurrentRedoAction());

            manager.Undo();

            Assert.IsFalse(manager.CanUndo());
            Assert.IsNull(manager.GetCurrentUndoAction());
            Assert.IsTrue(manager.CanRedo());
            Assert.AreEqual(actionMock.Object, manager.GetCurrentRedoAction());
        }

        [TestMethod]
        public void WhenActionIsRedoneThenDoActionIsExecuted()
        {
            ActionManager manager = new ActionManager();
            Mock<IAction> actionMock = new Mock<IAction>();
            manager.Execute(actionMock.Object);

            manager.Undo();
            actionMock.Verify(x => x.Undo(), Times.Once());

            manager.Redo();
            actionMock.Verify(x => x.Do(), Times.Exactly(2));
        }

        [TestMethod]
        public void WhenActionIsRedoneThenActionCanBeUndoneAgain()
        {
            ActionManager manager = new ActionManager();
            Mock<IAction> actionMock = new Mock<IAction>();
            manager.Execute(actionMock.Object);
            manager.Undo();

            Assert.IsFalse(manager.CanUndo());
            Assert.IsNull(manager.GetCurrentUndoAction());
            Assert.IsTrue(manager.CanRedo());
            Assert.AreEqual(actionMock.Object, manager.GetCurrentRedoAction());

            manager.Redo();

            Assert.IsTrue(manager.CanUndo());
            Assert.AreEqual(actionMock.Object, manager.GetCurrentUndoAction());
            Assert.IsFalse(manager.CanRedo());
            Assert.IsNull(manager.GetCurrentRedoAction());
        }

        [TestMethod]
        public void WhenActionIsAddedThenActionIsNotExecuted()
        {
            ActionManager manager = new ActionManager();
            Mock<IAction> actionMock = new Mock<IAction>();
            manager.Add(actionMock.Object);
            actionMock.Verify(x => x.Do(), Times.Never());
        }

        [TestMethod]
        public void WhenActionIsAddedThenActionCanBeUndone()
        {
            ActionManager manager = new ActionManager();
            Mock<IAction> actionMock = new Mock<IAction>();
            manager.Add(actionMock.Object);
            Assert.IsTrue(manager.CanUndo());
            Assert.IsNotNull(manager.GetCurrentUndoAction());
        }
    }
}
