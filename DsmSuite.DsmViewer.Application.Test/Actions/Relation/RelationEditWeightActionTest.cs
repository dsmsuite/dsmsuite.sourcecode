using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Model.Interfaces;
using Moq;
using DsmSuite.DsmViewer.Application.Actions.Relation;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Relation
{
    [TestClass]
    public class RelationEditWeightActionTest
    {
        private Mock<IDsmModel> _model;
        private Mock<IDsmRelation> _relation;
        private Mock<IDsmElement> _consumer;
        private Mock<IDsmElement> _provider;

        private Dictionary<string, string> _data;

        private const int _relationId = 1;
        private const int _consumerId = 2;
        private const int _providerId = 3;
        private const int _oldWeight = 123;
        private const int _newWeight = 456;

        [TestInitialize()]
        public void Setup()
        {
            _model = new Mock<IDsmModel>();
            _relation = new Mock<IDsmRelation>();
            _consumer = new Mock<IDsmElement>();
            _provider = new Mock<IDsmElement>();

            _relation.Setup(x => x.Id).Returns(_relationId);
            _relation.Setup(x => x.ConsumerId).Returns(_consumerId);
            _relation.Setup(x => x.ProviderId).Returns(_providerId);
            _relation.Setup(x => x.Weight).Returns(_oldWeight);
            _model.Setup(x => x.GetElementById(_consumerId)).Returns(_consumer.Object);
            _model.Setup(x => x.GetElementById(_providerId)).Returns(_provider.Object);

            _data = new Dictionary<string, string>();
            _data["relation"] = _relationId.ToString();
            _data["old"] = _oldWeight.ToString();
            _data["new"] = _newWeight.ToString();
        }

        [TestMethod]
        public void WhenDoActionThenRelationWeightIsChangedDataModel()
        {
            RelationEditWeightAction action = new RelationEditWeightAction(_model.Object, _relation.Object, _newWeight);
            action.Do();

            _model.Verify(x => x.EditRelationWeight(_relation.Object, _newWeight), Times.Once());
        }

        [TestMethod]
        public void WhenUndoActionThenRelationWeightIsRevertedDataModel()
        {
            RelationEditWeightAction action = new RelationEditWeightAction(_model.Object, _relation.Object, _newWeight);
            action.Undo();

            _model.Verify(x => x.EditRelationWeight(_relation.Object, _oldWeight), Times.Once());
        }

        [TestMethod]
        public void GivenLoadedActionWhenGettingDataThenActionAttributesMatch()
        {
            _model.Setup(x => x.GetRelationById(_relationId)).Returns(_relation.Object);

            object[] args = { _model.Object, _data };
            RelationEditWeightAction action = new RelationEditWeightAction(args);

            Assert.AreEqual(3, action.Data.Count);
            Assert.AreEqual(_relationId.ToString(), _data["relation"]);
            Assert.AreEqual(_oldWeight.ToString(), _data["old"]);
            Assert.AreEqual(_newWeight.ToString(), _data["new"]);
        }
    }
}
