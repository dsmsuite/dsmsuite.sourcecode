using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Application.Actions.Element;
using System.Collections.Generic;
using DsmSuite.DsmViewer.Application.Algorithm;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Element
{
    [TestClass]
    public class ElementPartitionActionTest
    {
        private Mock<IDsmModel> _model;
        private Mock<IDsmElement> _element;
        private Mock<ISortAlgorithm> _sortAlgorithm;
        private string _algorithm;
        private string _order;

        private Dictionary<string, string> _data;

        private const int _elementId = 1;
        private const string _usedAlgorithm = "MockedAlgorithm";
        private const string _sortResult = "2,0,1";
        private const string _inverseSortResult = "1,2,0";

        public class StubbedSortAlgorithm : ISortAlgorithm
        {
            public StubbedSortAlgorithm(object[] args) { }

            public string Name => "Stub";

            public SortResult Sort()
            {
                return new SortResult(_sortResult);
            }
        }

        [TestInitialize()]
        public void Setup()
        {
            _model = new Mock<IDsmModel>();
            _element = new Mock<IDsmElement>();
            _sortAlgorithm = new Mock<ISortAlgorithm>();

            SortAlgorithmFactory.RegisterAlgorithm(_usedAlgorithm, typeof(StubbedSortAlgorithm));

            _element.Setup(x => x.Id).Returns(_elementId);
            _model.Setup(x => x.GetElementById(_elementId)).Returns(_element.Object);

            _data = new Dictionary<string, string>();
            _data["element"] = _elementId.ToString();
            _data["algorithm"] = _algorithm;
            _data["order"] = _sortResult;
        }

        [TestMethod]
        public void WhenDoActionThenElementsChilderenAreSorted()
        {
            ElementPartitionAction action = new ElementPartitionAction(_model.Object, _element.Object, _usedAlgorithm);
            action.Do();

            _model.Verify(x => x.ReorderChildren(_element.Object, It.Is<SortResult>(i => i.Data == _sortResult)), Times.Once());
        }

        [TestMethod]
        public void WhenUndoActionThenElementsChilderenAreSortIsReverted()
        {
            object[] args = { _model.Object, _data };
            ElementPartitionAction action = new ElementPartitionAction(args);
            action.Undo();

            _model.Verify(x => x.ReorderChildren(_element.Object, It.Is<SortResult>(i => i.Data == _inverseSortResult)), Times.Once());
        }

        [TestMethod]
        public void GivenLoadedActionWhenGettingDataThenActionAttributesMatch()
        {
            object[] args = { _model.Object, _data };
            ElementPartitionAction action = new ElementPartitionAction(args);

            Assert.AreEqual(3, action.Data.Count);
            Assert.AreEqual(_elementId.ToString(), _data["element"]);
            Assert.AreEqual(_algorithm, _data["algorithm"]);
            Assert.AreEqual(_sortResult, _data["order"]);
        }
    }
}
