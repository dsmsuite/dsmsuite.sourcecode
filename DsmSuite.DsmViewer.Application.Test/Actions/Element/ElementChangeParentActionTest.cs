using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Model.Interfaces;
using Moq;
using System.Collections.Generic;
using DsmSuite.DsmViewer.Application.Actions.Element;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Element
{
    [TestClass]
    public class ElementChangeParentActionTest
    {
        private Mock<IDsmModel> _model;
        private Mock<IDsmElement> _element;
        private Mock<IDsmElement> _oldParent;
        private Mock<IDsmElement> _newParent;

        private Dictionary<string, string> _data;

        private const int _elementId = 1;
        private const int _oldParentId = 2;
        private const int _newParentId= 3;

        [TestInitialize()]
        public void Setup()
        {
            _model = new Mock<IDsmModel>();
            _element = new Mock<IDsmElement>();
            _oldParent = new Mock<IDsmElement>();
            _newParent = new Mock<IDsmElement>();

            _element.Setup(x => x.Id).Returns(_elementId);
            _element.Setup(x => x.Parent).Returns(_oldParent.Object);
            _oldParent.Setup(x => x.Id).Returns(_oldParentId);
            _newParent.Setup(x => x.Id).Returns(_newParentId);

            _model.Setup(x => x.GetElementById(_elementId)).Returns(_element.Object);
            _model.Setup(x => x.GetElementById(_oldParentId)).Returns(_oldParent.Object);
            _model.Setup(x => x.GetElementById(_newParentId)).Returns(_newParent.Object);

            _data = new Dictionary<string, string>();
            _data["element"] = _elementId.ToString();
            _data["old"] = _oldParentId.ToString();
            _data["new"] = _newParentId.ToString();
        }

        [TestMethod]
        public void WhenDoActionThenElementParentIsChangedDataModel()
        {
            ElementChangeParentAction action = new ElementChangeParentAction(_model.Object, _element.Object, _newParent.Object);
            action.Do();

            _model.Verify(x => x.ChangeElementParent(_element.Object, _newParent.Object), Times.Once());
        }

        [TestMethod]
        public void WhenUndoActionThenElementParentIsRevertedDataModel()
        {
            ElementChangeParentAction action = new ElementChangeParentAction(_model.Object, _element.Object, _newParent.Object);
            action.Undo();

            _model.Verify(x => x.ChangeElementParent(_element.Object, _oldParent.Object), Times.Once());
        }

        [TestMethod]
        public void GivenLoadedActionWhenGettingDataThenActionAttributesMatch()
        {
            object[] args = { _model.Object, _data };
            ElementChangeParentAction action = new ElementChangeParentAction(args);

            Assert.AreEqual(3, action.Data.Count);
            Assert.AreEqual(_elementId.ToString(), _data["element"]);
            Assert.AreEqual(_oldParentId.ToString(), _data["old"]);
            Assert.AreEqual(_newParentId.ToString(), _data["new"]);
        }
    }
}
