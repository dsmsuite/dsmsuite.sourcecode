﻿using DsmSuite.DsmViewer.Application.Actions.Element;
using DsmSuite.DsmViewer.Model.Interfaces;
using Moq;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Element
{
    [TestClass]
    public class ElementChangeNameActionTest
    {
        private Mock<IDsmModel> _model;
        private Mock<IDsmElement> _element;

        private Dictionary<string, string> _data;

        private const int ElementId = 1;
        private const string OldName = "oldname";
        private const string NewName = "newname";

        [TestInitialize()]
        public void Setup()
        {
            _model = new Mock<IDsmModel>();
            _element = new Mock<IDsmElement>();

            _element.Setup(x => x.Id).Returns(ElementId);
            _element.Setup(x => x.Name).Returns(OldName);
            _model.Setup(x => x.GetElementById(ElementId)).Returns(_element.Object);

            _data = new Dictionary<string, string>
            {
                ["element"] = ElementId.ToString(),
                ["old"] = OldName,
                ["new"] = NewName
            };
        }

        [TestMethod]
        public void WhenDoActionThenElementNameIsChangedDataModel()
        {
            ElementChangeNameAction action = new ElementChangeNameAction(_model.Object, _element.Object, NewName);
            action.Do();
            Assert.IsTrue(action.IsValid());

            _model.Verify(x => x.ChangeElementName(_element.Object, NewName), Times.Once());
        }

        [TestMethod]
        public void WhenUndoActionThenElementNameIsRevertedDataModel()
        {
            ElementChangeNameAction action = new ElementChangeNameAction(_model.Object, _element.Object, NewName);
            action.Undo();
            Assert.IsTrue(action.IsValid());

            _model.Verify(x => x.ChangeElementName(_element.Object, OldName), Times.Once());
        }
    }
}
