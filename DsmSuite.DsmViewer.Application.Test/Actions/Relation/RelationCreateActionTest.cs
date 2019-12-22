using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Application.Actions.Relation;
using Moq;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Relation
{
    [TestClass]
    public class RelationCreateActionTest
    {
        private Mock<IDsmModel> _model;
        private Mock<IDsmRelation> _relation;
        private Mock<IDsmElement> _consumer;
        private Mock<IDsmElement> _provider;

        private Dictionary<string, string> _data;

        private const int _relationId = 1;
        private const int _consumerId = 11;
        private const int _providerId = 12;
        private const string _type = "newtype";
        private const int _weight = 456;

        [TestInitialize()]
        public void Setup()
        {
            _model = new Mock<IDsmModel>();
            _relation = new Mock<IDsmRelation>();
            _consumer = new Mock<IDsmElement>();
            _provider = new Mock<IDsmElement>();

            _model.Setup(x => x.GetElementById(_consumerId)).Returns(_consumer.Object);
            _model.Setup(x => x.GetElementById(_providerId)).Returns(_provider.Object);
            _model.Setup(x => x.AddRelation(_consumerId, _providerId, _type, _weight)).Returns(_relation.Object);
            _relation.Setup(x => x.Id).Returns(_relationId);
            _consumer.Setup(x => x.Id).Returns(_consumerId);
            _provider.Setup(x => x.Id).Returns(_providerId);

            _data = new Dictionary<string, string>();
            _data["relation"] = _relationId.ToString();
            _data["consumer"] = _consumerId.ToString();
            _data["provider"] = _providerId.ToString();
            _data["type"] = _type;
            _data["weight"] = _weight.ToString();
        }

        [TestMethod]
        public void WhenDoActionThenRelationIsAddedToDataModel()
        {
            RelationCreateAction action = new RelationCreateAction(_model.Object, _consumerId, _providerId, _type, _weight);
            action.Do();

            _model.Verify(x => x.AddRelation(_consumerId, _providerId, _type, _weight), Times.Once());
        }

        [TestMethod]
        public void WhenUndoActionThenRelationIsRemovedFromDataModel()
        {
            _model.Setup(x => x.GetRelationById(_relationId)).Returns(_relation.Object);

            object[] args = { _model.Object, _data };
            RelationCreateAction action = new RelationCreateAction(args);
            action.Undo();

            _model.Verify(x => x.RemoveRelation(_relationId), Times.Once());
        }

        [TestMethod]
        public void GivenLoadedActionWhenGettingDataThenActionAttributesMatch()
        {
            _model.Setup(x => x.GetRelationById(_relationId)).Returns(_relation.Object);

            object[] args = { _model.Object, _data };
            RelationCreateAction action = new RelationCreateAction(args);

            Assert.AreEqual(5, action.Data.Count);
            Assert.AreEqual(_relationId.ToString(), _data["relation"]);
            Assert.AreEqual(_consumerId.ToString(), _data["consumer"]);
            Assert.AreEqual(_providerId.ToString(), _data["provider"]);
            Assert.AreEqual(_type, _data["type"]);
            Assert.AreEqual(_weight.ToString(), _data["weight"]); 
        }
    }
}
