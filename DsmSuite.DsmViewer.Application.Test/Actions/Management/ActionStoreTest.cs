using DsmSuite.DsmViewer.Application.Actions.Element;
using DsmSuite.DsmViewer.Application.Actions.Management;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using Moq;

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

            List<IAction> managerActions = new List<IAction>
            {
                new ElementMoveUpAction(model.Object, element1.Object),
                new ElementMoveDownAction(model.Object, element2.Object)
            };

            model.Setup(x => x.GetElementById(1)).Returns(element1.Object);
            model.Setup(x => x.GetElementById(2)).Returns(element2.Object);
            manager.Setup(x => x.GetActionsInChronologicalOrder()).Returns(managerActions);
            manager.Setup(x => x.Validate()).Returns(true);

            store.SaveToModel();

            model.Verify(x => x.AddAction(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, string>>()), Times.Exactly(2));
            model.Verify(x => x.AddAction(ElementMoveUpAction.RegisteredType.ToString(), It.IsAny<IReadOnlyDictionary<string, string>>()), Times.Once());
            model.Verify(x => x.AddAction(ElementMoveDownAction.RegisteredType.ToString(), It.IsAny<IReadOnlyDictionary<string, string>>()), Times.Once());
        }
    }
}
