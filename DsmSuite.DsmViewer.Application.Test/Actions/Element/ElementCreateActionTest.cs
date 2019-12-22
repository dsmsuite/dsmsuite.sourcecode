using System;
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

        private const int _elementId = 1;
        private const string _name = "name";
        private const string _type = "type";
        private const int _parentId = 456;

        [TestInitialize()]
        public void Setup()
        {
            _model = new Mock<IDsmModel>();
            _element = new Mock<IDsmElement>();
            _parent = new Mock<IDsmElement>();

            _model.Setup(x => x.GetElementById(_elementId)).Returns(_element.Object);
            _element.Setup(x => x.Id).Returns(_elementId);
            _model.Setup(x => x.GetElementById(_parentId)).Returns(_parent.Object);
            _parent.Setup(x => x.Id).Returns(_parentId);

            _model.Setup(x => x.AddElement(_name, _type, _parentId)).Returns(_element.Object);

            _data = new Dictionary<string, string>();
            _data["element"] = _elementId.ToString();
            _data["name"] = _name;
            _data["type"] = _type;
            _data["parent"] = _parentId.ToString();
        }

        [TestMethod]
        public void WhenDoActionThenElementIsAddedToDataModel()
        {
            ElementCreateAction action = new ElementCreateAction(_model.Object, _name, _type, _parent.Object);
            action.Do();

            _model.Verify(x => x.AddElement(_name, _type, _parentId), Times.Once());
        }

        [TestMethod]
        public void WhenUndoActionThenElementIsRemovedFromDataModel()
        {
            object[] args = { _model.Object, _data };
            ElementCreateAction action = new ElementCreateAction(args);
            action.Undo();

            _model.Verify(x => x.RemoveElement(_elementId), Times.Once());
        }

        [TestMethod]
        public void GivenLoadedActionWhenGettingDataThenActionAttributesMatch()
        {
            object[] args = { _model.Object, _data };
            ElementCreateAction action = new ElementCreateAction(args);

            Assert.AreEqual(4, action.Data.Count);
            Assert.AreEqual(_elementId.ToString(), _data["element"]);
            Assert.AreEqual(_name, _data["name"]);
            Assert.AreEqual(_type, _data["type"]);
            Assert.AreEqual(_parentId.ToString(), _data["parent"]);
        }
    }
}
