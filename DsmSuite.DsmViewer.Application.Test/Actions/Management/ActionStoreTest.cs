using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Application.Actions.Management;
using Moq;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Application.Actions.Snapshot;
using DsmSuite.DsmViewer.Application.Actions.Element;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Management
{
    [TestClass]
    public class ActionStoreTest
    {
        [TestMethod]
        public void GivenActionManagerContainsActionsWhenActionsAreSaveThenTheyAreAddedToTheDataModel()
        {
            Mock<IDsmModel> model = new Mock<IDsmModel>();
            Mock<IActionManager> manager = new Mock<IActionManager>();
            ActionStore store = new ActionStore(model.Object, manager.Object);

            Mock<IDsmElement> element1 = new Mock<IDsmElement>();
            Mock<IDsmElement> element2 = new Mock<IDsmElement>();

            List<IAction> managerActions = new List<IAction>();
            managerActions.Add(new ElementMoveUpAction(model.Object, element1.Object));
            managerActions.Add(new ElementMoveDownAction(model.Object, element2.Object));

            model.Setup(x => x.GetElementById(1)).Returns(element1.Object);
            model.Setup(x => x.GetElementById(2)).Returns(element2.Object);
            manager.Setup(x => x.GetActionsInChronologicalOrder()).Returns(managerActions);

            store.Save();

            model.Verify(x => x.AddAction(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, string>>()), Times.Exactly(2));
            model.Verify(x => x.AddAction(ElementMoveUpAction.TypeName, It.IsAny<IReadOnlyDictionary<string, string>>()), Times.Once());
            model.Verify(x => x.AddAction(ElementMoveDownAction.TypeName, It.IsAny<IReadOnlyDictionary<string, string>>()), Times.Once());
        }

        [TestMethod]
        public void GivenDataModelContainsActionsWhenActionsAreLoadedThenTheyAreAddedToTheActionManager()
        {
            Mock<IDsmModel> model = new Mock<IDsmModel>();
            Mock<IActionManager> manager = new Mock<IActionManager>();
            ActionStore store = new ActionStore(model.Object, manager.Object);

            Dictionary<string, string> data1 = new Dictionary<string, string>();
            data1["element"] = "1";
            Dictionary<string, string> data2 = new Dictionary<string, string>();
            data2["element"] = "2";

            Mock<IDsmAction> action1 = new Mock<IDsmAction>();
            action1.Setup(x => x.Type).Returns(ElementMoveUpAction.TypeName);
            action1.Setup(x => x.Data).Returns(data1);

            Mock<IDsmAction> action2 = new Mock<IDsmAction>();
            action2.Setup(x => x.Type).Returns(ElementMoveDownAction.TypeName);
            action2.Setup(x => x.Data).Returns(data2);

            List<IDsmAction> actions = new List<IDsmAction>();
            actions.Add(action1.Object);
            actions.Add(action2.Object);

            model.Setup(x => x.GetActions()).Returns(actions);

            Mock<IDsmElement> element1 = new Mock<IDsmElement>();
            Mock<IDsmElement> element2 = new Mock<IDsmElement>();

            model.Setup(x => x.GetElementById(1)).Returns(element1.Object);
            model.Setup(x => x.GetElementById(2)).Returns(element2.Object);

            store.Load();

            manager.Verify(x => x.Add(It.IsAny<ElementMoveDownAction>()), Times.Once());
            manager.Verify(x => x.Add(It.IsAny<ElementMoveUpAction>()), Times.Once());
        }
    }
}
