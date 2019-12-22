using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Model.Interfaces;
using Moq;
using System.Collections.Generic;
using DsmSuite.DsmViewer.Application.Actions.Element;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Element
{
    [TestClass]
    public class ElementEditTypeActionTest
    {
        private Mock<IDsmModel> _model;
        private Mock<IDsmElement> _element;

        private Dictionary<string, string> _data;

        private const int _elementId = 1;
        private const string _oldType = "oldtype";
        private const string _newType = "newtype";

        [TestInitialize()]
        public void Setup()
        {
            _model = new Mock<IDsmModel>();
            _element = new Mock<IDsmElement>();

            _element.Setup(x => x.Id).Returns(_elementId);
            _element.Setup(x => x.Type).Returns(_oldType);
            _model.Setup(x => x.GetElementById(_elementId)).Returns(_element.Object);

            _data = new Dictionary<string, string>();
            _data["element"] = _elementId.ToString();
            _data["old"] = _oldType;
            _data["new"] = _newType;
        }

        [TestMethod]
        public void WhenDoActionThenElementTypeIsChangedDataModel()
        {
            ElementEditTypeAction action = new ElementEditTypeAction(_model.Object, _element.Object, _newType);
            action.Do();

            _model.Verify(x => x.EditElementType(_element.Object, _newType), Times.Once());
        }

        [TestMethod]
        public void WhenUndoActionThenElementTypeIsRevertedDataModel()
        {
            ElementEditTypeAction action = new ElementEditTypeAction(_model.Object, _element.Object, _newType);
            action.Undo();

            _model.Verify(x => x.EditElementType(_element.Object, _oldType), Times.Once());
        }

        [TestMethod]
        public void GivenLoadedActionWhenGettingDataThenActionAttributesMatch()
        {
            object[] args = { _model.Object, _data };
            ElementEditTypeAction action = new ElementEditTypeAction(args);

            Assert.AreEqual(3, action.Data.Count);
            Assert.AreEqual(_elementId.ToString(), _data["element"]);
            Assert.AreEqual(_oldType, _data["old"]);
            Assert.AreEqual(_newType, _data["new"]);
        }
    }
}
