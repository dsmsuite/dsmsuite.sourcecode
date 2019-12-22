using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Model.Interfaces;
using Moq;
using System.Collections.Generic;
using DsmSuite.DsmViewer.Application.Actions.Element;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Element
{
    [TestClass]
    public class ElementEditNameActionTest
    {
        private Mock<IDsmModel> _model;
        private Mock<IDsmElement> _element;

        private Dictionary<string, string> _data;

        private const int _elementId = 1;
        private const string _oldName = "oldname";
        private const string _newName = "newname";

        [TestInitialize()]
        public void Setup()
        {
            _model = new Mock<IDsmModel>();
            _element = new Mock<IDsmElement>();

            _element.Setup(x => x.Id).Returns(_elementId);
            _element.Setup(x => x.Name).Returns(_oldName);
            _model.Setup(x => x.GetElementById(_elementId)).Returns(_element.Object);

            _data = new Dictionary<string, string>();
            _data["element"] = _elementId.ToString();
            _data["old"] = _oldName;
            _data["new"] = _newName;
        }

        [TestMethod]
        public void WhenDoActionThenElementNameIsChangedDataModel()
        {
            ElementEditNameAction action = new ElementEditNameAction(_model.Object, _element.Object, _newName);
            action.Do();

            _model.Verify(x => x.EditElementName(_element.Object, _newName), Times.Once());
        }

        [TestMethod]
        public void WhenUndoActionThenElementNameIsRevertedDataModel()
        {
            ElementEditNameAction action = new ElementEditNameAction(_model.Object, _element.Object, _newName);
            action.Undo();

            _model.Verify(x => x.EditElementName(_element.Object, _oldName), Times.Once());
        }

        [TestMethod]
        public void GivenLoadedActionWhenGettingDataThenActionAttributesMatch()
        {
            object[] args = { _model.Object, _data };
            ElementEditNameAction action = new ElementEditNameAction(args);

            Assert.AreEqual(3, action.Data.Count);
            Assert.AreEqual(_elementId.ToString(), _data["element"]);
            Assert.AreEqual(_oldName, _data["old"]);
            Assert.AreEqual(_newName, _data["new"]);
        }
    }
}
