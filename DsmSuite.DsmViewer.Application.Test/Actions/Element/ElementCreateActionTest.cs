using DsmSuite.DsmViewer.Application.Actions.Element;
using DsmSuite.DsmViewer.Model.Interfaces;
using Moq;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Element
{
    [TestClass]
    public class ElementCreateActionTest
    {
        private Mock<IDsmModel> _model;
        private Mock<IDsmElement> _element;
        private Mock<IDsmElement> _parent;

        private Dictionary<string, string> _data;

        private const int ElementId = 1;
        private const string Name = "name";
        private const string Type = "type";
        private const int ParentId = 456;
        private const int Index = 3;

        [TestInitialize()]
        public void Setup()
        {
            _model = new Mock<IDsmModel>();
            _element = new Mock<IDsmElement>();
            _parent = new Mock<IDsmElement>();

            _model.Setup(x => x.GetElementById(ElementId)).Returns(_element.Object);
            _element.Setup(x => x.Id).Returns(ElementId);
            _model.Setup(x => x.GetElementById(ParentId)).Returns(_parent.Object);
            _parent.Setup(x => x.Id).Returns(ParentId);

            _model.Setup(x => x.AddElement(Name, Type, ParentId, Index, null)).Returns(_element.Object);

            _data = new Dictionary<string, string>
            {
                ["element"] = ElementId.ToString(),
                ["name"] = Name,
                ["type"] = Type,
                ["parent"] = ParentId.ToString(),
                ["index"] = Index.ToString()
            };
        }

        [TestMethod]
        public void WhenDoActionThenElementIsAddedToDataModel()
        {
            ElementCreateAction action = new ElementCreateAction(_model.Object, Name, Type, _parent.Object, Index);
            action.Do();
            Assert.IsTrue(action.IsValid());

            _model.Verify(x => x.AddElement(Name, Type, ParentId, Index, null), Times.Once());
        }
    }
}
