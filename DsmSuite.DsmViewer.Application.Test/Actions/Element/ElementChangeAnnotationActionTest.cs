using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Model.Interfaces;
using Moq;
using System.Collections.Generic;
using DsmSuite.DsmViewer.Application.Actions.Element;
using DsmSuite.DsmViewer.Model.Core;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Element
{
    [TestClass]
    public class ElementChangeAnnotationActionTest
    {
        private Mock<IDsmModel> _model;
        private Mock<IDsmElement> _element;

        private Dictionary<string, string> _data;

        private const int ElementId = 1;
        private const string OldAnnotation = "oldannotation";
        private const string NewAnnotation = "newannotation";

        [TestInitialize()]
        public void Setup()
        {
            _model = new Mock<IDsmModel>();
            _element = new Mock<IDsmElement>();

            _element.Setup(x => x.Id).Returns(ElementId);
            _model.Setup(x => x.GetElementById(ElementId)).Returns(_element.Object);
            _model.Setup(x => x.FindElementAnnotation(_element.Object)).Returns(new DsmElementAnnotation(ElementId, OldAnnotation));

            _data = new Dictionary<string, string>
            {
                ["element"] = ElementId.ToString(),
                ["old"] = OldAnnotation,
                ["new"] = NewAnnotation
            };
        }

        [TestMethod]
        public void WhenDoActionThenElementNameIsChangedDataModel()
        {
            ElementChangeAnnotationAction action = new ElementChangeAnnotationAction(_model.Object, _element.Object, NewAnnotation);
            action.Do();

            _model.Verify(x => x.ChangeElementAnnotation(_element.Object, NewAnnotation), Times.Once());
        }

        [TestMethod]
        public void WhenUndoActionThenElementNameIsRevertedDataModel()
        {
            ElementChangeAnnotationAction action = new ElementChangeAnnotationAction(_model.Object, _element.Object, NewAnnotation);
            action.Undo();

            _model.Verify(x => x.ChangeElementAnnotation(_element.Object, OldAnnotation), Times.Once());
        }

        [TestMethod]
        public void GivenLoadedActionWhenGettingDataThenActionAttributesMatch()
        {
            object[] args = { _model.Object, _data };
            ElementChangeNameAction action = new ElementChangeNameAction(args);

            Assert.AreEqual(3, action.Data.Count);
            Assert.AreEqual(ElementId.ToString(), _data["element"]);
            Assert.AreEqual(OldAnnotation, _data["old"]);
            Assert.AreEqual(NewAnnotation, _data["new"]);
        }
    }
}
