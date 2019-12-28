using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Model.Interfaces;
using Moq;
using System.Collections.Generic;
using DsmSuite.DsmViewer.Application.Actions.Element;

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

            _model.Setup(x => x.AddElement(Name, Type, ParentId)).Returns(_element.Object);

            _data = new Dictionary<string, string>
            {
                ["element"] = ElementId.ToString(),
                ["name"] = Name,
                ["type"] = Type,
                ["parent"] = ParentId.ToString()
            };
        }

        [TestMethod]
        public void WhenDoActionThenElementIsAddedToDataModel()
        {
            ElementCreateAction action = new ElementCreateAction(_model.Object, Name, Type, _parent.Object);
            action.Do();

            _model.Verify(x => x.AddElement(Name, Type, ParentId), Times.Once());
        }

        [TestMethod]
        public void WhenUndoActionThenElementIsRemovedFromDataModel()
        {
            object[] args = { _model.Object, _data };
            ElementCreateAction action = new ElementCreateAction(args);
            action.Undo();

            _model.Verify(x => x.RemoveElement(ElementId), Times.Once());
        }

        [TestMethod]
        public void GivenLoadedActionWhenGettingDataThenActionAttributesMatch()
        {
            object[] args = { _model.Object, _data };
            ElementCreateAction action = new ElementCreateAction(args);

            Assert.AreEqual(4, action.Data.Count);
            Assert.AreEqual(ElementId.ToString(), _data["element"]);
            Assert.AreEqual(Name, _data["name"]);
            Assert.AreEqual(Type, _data["type"]);
            Assert.AreEqual(ParentId.ToString(), _data["parent"]);
        }
    }
}
