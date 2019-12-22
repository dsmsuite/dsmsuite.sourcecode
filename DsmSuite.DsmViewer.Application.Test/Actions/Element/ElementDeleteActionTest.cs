using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Application.Actions.Element;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Element
{
    [TestClass]
    public class ElementDeleteActionTest
    {
        private Mock<IDsmModel> _model;
        private Mock<IDsmElement> _element;

        [TestInitialize()]
        public void Setup()
        {
            _model = new Mock<IDsmModel>();
            _element = new Mock<IDsmElement>();
            _element.Setup(x => x.Id).Returns(1);
        }

        [TestMethod]
        public void WhenDoActionThenElementIsRemovedFromDataModel()
        {
            ElementDeleteAction action = new ElementDeleteAction(_model.Object, _element.Object);
            action.Do();

            _model.Verify(x => x.RemoveElement(1), Times.Once());
        }

        [TestMethod]
        public void WhenUndoActionThenElementIsRestoredInDataModel()
        {
            ElementDeleteAction action = new ElementDeleteAction(_model.Object, _element.Object);
            action.Undo();

            _model.Verify(x => x.UnremoveElement(1), Times.Once());
        }
    }
}
