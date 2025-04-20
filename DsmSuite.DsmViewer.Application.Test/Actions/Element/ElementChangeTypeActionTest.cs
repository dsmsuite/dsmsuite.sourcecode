using DsmSuite.DsmViewer.Application.Actions.Element;
using DsmSuite.DsmViewer.Model.Interfaces;
using Moq;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Element
{
    [TestClass]
    public class ElementChangeTypeActionTest
    {
        private Mock<IDsmModel> _model;
        private Mock<IDsmElement> _element;

        private Dictionary<string, string> _data;

        private const int ElementId = 1;
        private const string OldType = "oldtype";
        private const string NewType = "newtype";

        [TestInitialize()]
        public void Setup()
        {
            _model = new Mock<IDsmModel>();
            _element = new Mock<IDsmElement>();

            _element.Setup(x => x.Id).Returns(ElementId);
            _element.Setup(x => x.Type).Returns(OldType);
            _model.Setup(x => x.GetElementById(ElementId)).Returns(_element.Object);

            _data = new Dictionary<string, string>
            {
                ["element"] = ElementId.ToString(),
                ["old"] = OldType,
                ["new"] = NewType
            };
        }

        [TestMethod]
        public void WhenDoActionThenElementTypeIsChangedDataModel()
        {
            ElementChangeTypeAction action = new ElementChangeTypeAction(_model.Object, _element.Object, NewType);
            action.Do();
            Assert.IsTrue(action.IsValid());

            _model.Verify(x => x.ChangeElementType(_element.Object, NewType), Times.Once());
        }

        [TestMethod]
        public void WhenUndoActionThenElementTypeIsRevertedDataModel()
        {
            ElementChangeTypeAction action = new ElementChangeTypeAction(_model.Object, _element.Object, NewType);
            action.Undo();
            Assert.IsTrue(action.IsValid());

            _model.Verify(x => x.ChangeElementType(_element.Object, OldType), Times.Once());
        }
    }
}
