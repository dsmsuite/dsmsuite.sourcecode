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

        private const int ElementId = 1;
        private const int OldParentId = 2;
        private const int NewParentId= 3;

        private const int OldIndex = 4;
        private const int NewIndex = 5;

        [TestInitialize()]
        public void Setup()
        {
            _model = new Mock<IDsmModel>();
            _element = new Mock<IDsmElement>();
            _oldParent = new Mock<IDsmElement>();
            _newParent = new Mock<IDsmElement>();

            _element.Setup(x => x.Id).Returns(ElementId);
            _element.Setup(x => x.Parent).Returns(_oldParent.Object);
            _oldParent.Setup(x => x.IndexOfChild(_element.Object)).Returns(OldIndex);
            _oldParent.Setup(x => x.Id).Returns(OldParentId);
            _newParent.Setup(x => x.Id).Returns(NewParentId);

            _model.Setup(x => x.GetElementById(ElementId)).Returns(_element.Object);
            _model.Setup(x => x.GetElementById(OldParentId)).Returns(_oldParent.Object);
            _model.Setup(x => x.GetElementById(NewParentId)).Returns(_newParent.Object);

            _data = new Dictionary<string, string>
            {
                ["element"] = ElementId.ToString(),
                ["old"] = OldParentId.ToString(),
                ["oldIndex"] = OldIndex.ToString(),
                ["new"] = NewParentId.ToString(),
                ["newIndex"] = NewIndex.ToString()
            };
        }

        [TestMethod]
        public void WhenDoActionThenElementParentIsChangedDataModel()
        {
            ElementChangeParentAction action = new ElementChangeParentAction(_model.Object, _element.Object, _newParent.Object, NewIndex);
            action.Do();

            _model.Verify(x => x.ChangeElementParent(_element.Object, _newParent.Object, NewIndex), Times.Once());
        }

        [TestMethod]
        public void WhenUndoActionThenElementParentIsRevertedDataModel()
        {
            ElementChangeParentAction action = new ElementChangeParentAction(_model.Object, _element.Object, _newParent.Object, NewIndex);
            action.Undo();

            _model.Verify(x => x.ChangeElementParent(_element.Object, _oldParent.Object, OldIndex), Times.Once());
        }

        [TestMethod]
        public void GivenLoadedActionWhenGettingDataThenActionAttributesMatch()
        {
            object[] args = { _model.Object, _data };
            ElementChangeParentAction action = new ElementChangeParentAction(args);

            Assert.AreEqual(5, action.Data.Count);
            Assert.AreEqual(ElementId.ToString(), _data["element"]);
            Assert.AreEqual(OldParentId.ToString(), _data["old"]);
            Assert.AreEqual(OldIndex.ToString(), _data["oldIndex"]);
            Assert.AreEqual(NewParentId.ToString(), _data["new"]);
            Assert.AreEqual(NewIndex.ToString(), _data["newIndex"]);
        }
    }
}
