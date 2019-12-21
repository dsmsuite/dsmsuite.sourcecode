using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Application.Actions.Base;
using System.Collections.Generic;
using Moq;

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
        public void WhenActionIsExecutedThenActionIsExecuted()
        {
            ActionManager manager = new ActionManager();
            Mock<ActionBase> actionMock = new Mock<ActionBase>();
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

            Mock<ActionBase> actionMock = new Mock<ActionBase>();
            manager.Execute(actionMock.Object);

            Assert.IsTrue(eventFired);
        }

        [TestMethod]
        public void WhenActionIsExecutedThenActionCanBeUndone()
        {
            ActionManager manager = new ActionManager();
            Mock<ActionBase> actionMock = new Mock<ActionBase>();
            manager.Execute(actionMock.Object);
            Assert.IsTrue(manager.CanUndo());
            Assert.IsNotNull(manager.GetCurrentUndoAction());
        }

        [TestMethod]
        public void WhenActionIsAddedThenActionIsNotExecuted()
        {
            ActionManager manager = new ActionManager();
            Mock<ActionBase> actionMock = new Mock<ActionBase>();
            manager.Add(actionMock.Object);
            actionMock.Verify(x => x.Do(), Times.Never());
        }

        [TestMethod]
        public void WhenActionIsAddedThenActionCanBeUndone()
        {
            ActionManager manager = new ActionManager();
            Mock<ActionBase> actionMock = new Mock<ActionBase>();
            manager.Add(actionMock.Object);
            Assert.IsTrue(manager.CanUndo());
            Assert.IsNotNull(manager.GetCurrentUndoAction());
        }
    }
}
