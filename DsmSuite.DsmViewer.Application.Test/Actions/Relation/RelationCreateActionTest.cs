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

        private const int RelationId = 1;
        private const int ConsumerId = 11;
        private const int ProviderId = 12;
        private const string Type = "newtype";
        private const int Weight = 456;

        [TestInitialize()]
        public void Setup()
        {
            _model = new Mock<IDsmModel>();
            _relation = new Mock<IDsmRelation>();
            _consumer = new Mock<IDsmElement>();
            _provider = new Mock<IDsmElement>();

            _model.Setup(x => x.GetElementById(ConsumerId)).Returns(_consumer.Object);
            _model.Setup(x => x.GetElementById(ProviderId)).Returns(_provider.Object);
            _model.Setup(x => x.AddRelation(ConsumerId, ProviderId, Type, Weight)).Returns(_relation.Object);
            _relation.Setup(x => x.Id).Returns(RelationId);
            _relation.Setup(x => x.ConsumerId).Returns(ConsumerId);
            _relation.Setup(x => x.ProviderId).Returns(ProviderId);
            _consumer.Setup(x => x.Id).Returns(ConsumerId);
            _provider.Setup(x => x.Id).Returns(ProviderId);

            _data = new Dictionary<string, string>
            {
                ["relation"] = RelationId.ToString(),
                ["consumer"] = ConsumerId.ToString(),
                ["provider"] = ProviderId.ToString(),
                ["type"] = Type,
                ["weight"] = Weight.ToString()
            };
        }

        [TestMethod]
        public void WhenDoActionThenRelationIsAddedToDataModel()
        {
            RelationCreateAction action = new RelationCreateAction(_model.Object, ConsumerId, ProviderId, Type, Weight);
            action.Do();

            _model.Verify(x => x.AddRelation(ConsumerId, ProviderId, Type, Weight), Times.Once());
        }

        [TestMethod]
        public void WhenUndoActionThenRelationIsRemovedFromDataModel()
        {
            _model.Setup(x => x.GetRelationById(RelationId)).Returns(_relation.Object);

            object[] args = { _model.Object, _data };
            RelationCreateAction action = new RelationCreateAction(args);
            action.Undo();

            _model.Verify(x => x.RemoveRelation(RelationId), Times.Once());
        }

        [TestMethod]
        public void GivenLoadedActionWhenGettingDataThenActionAttributesMatch()
        {
            _model.Setup(x => x.GetRelationById(RelationId)).Returns(_relation.Object);

            object[] args = { _model.Object, _data };
            RelationCreateAction action = new RelationCreateAction(args);

            Assert.AreEqual(5, action.Data.Count);
            Assert.AreEqual(RelationId.ToString(), _data["relation"]);
            Assert.AreEqual(ConsumerId.ToString(), _data["consumer"]);
            Assert.AreEqual(ProviderId.ToString(), _data["provider"]);
            Assert.AreEqual(Type, _data["type"]);
            Assert.AreEqual(Weight.ToString(), _data["weight"]); 
        }
    }
}
