using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Model.Interfaces;
using Moq;
using DsmSuite.DsmViewer.Application.Actions.Relation;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Relation
{
    [TestClass]
    public class RelationEditTypeActionTest
    {
        private Mock<IDsmModel> _model;
        private Mock<IDsmRelation> _relation;
        private Mock<IDsmElement> _consumer;
        private Mock<IDsmElement> _provider;

        private const int _relationId = 1;
        private const int _consumerId = 2;
        private const int _providerId = 3;
        private const string _oldType = "oldtype";
        private const string _newType = "newtype";

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
            _relation.Setup(x => x.Type).Returns(_oldType);
            _model.Setup(x => x.GetElementById(_consumerId)).Returns(_consumer.Object);
            _model.Setup(x => x.GetElementById(_providerId)).Returns(_provider.Object);
        }

        [TestMethod]
        public void WhenDoActionThenRelationTypeIsChangedDataModel()
        {
            RelationEditTypeAction action = new RelationEditTypeAction(_model.Object, _relation.Object, _newType);
            action.Do();

            _model.Verify(x => x.EditRelationType(_relation.Object, _newType), Times.Once());
        }

        [TestMethod]
        public void WhenUndoActionThenRelationTypeIsRevertedDataModel()
        {
            RelationEditTypeAction action = new RelationEditTypeAction(_model.Object, _relation.Object, _newType);
            action.Undo();

            _model.Verify(x => x.EditRelationType(_relation.Object, _oldType), Times.Once());
        }
    }
}
