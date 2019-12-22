using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Application.Actions.Element;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Element
{
    [TestClass]
    public class ElementDeleteActionTest
    {
        private Mock<IDsmModel> _model;
        private Mock<IDsmElement> _element;

        private Dictionary<string, string> _data;

        private const int _elementId = 1;

        [TestInitialize()]
        public void Setup()
        {
            _model = new Mock<IDsmModel>();
            _element = new Mock<IDsmElement>();
            _element.Setup(x => x.Id).Returns(_elementId);
            _model.Setup(x => x.GetDeletedElementById(_elementId)).Returns(_element.Object);

            _data = new Dictionary<string, string>();
            _data["element"] = _elementId.ToString();
        }

        [TestMethod]
        public void WhenDoActionThenElementIsRemovedFromDataModel()
        {
            ElementDeleteAction action = new ElementDeleteAction(_model.Object, _element.Object);
            action.Do();

            _model.Verify(x => x.RemoveElement(_elementId), Times.Once());
        }

        [TestMethod]
        public void WhenUndoActionThenElementIsRestoredInDataModel()
        {
            ElementDeleteAction action = new ElementDeleteAction(_model.Object, _element.Object);
            action.Undo();

            _model.Verify(x => x.UnremoveElement(_elementId), Times.Once());
        }

        [TestMethod]
        public void GivenLoadedActionWhenGettingDataThenActionAttributesMatch()
        {
            object[] args = { _model.Object, _data };
            ElementDeleteAction action = new ElementDeleteAction(args);

            Assert.AreEqual(1, action.Data.Count);
            Assert.AreEqual(_elementId.ToString(), _data["element"]);
        }
    }
}
