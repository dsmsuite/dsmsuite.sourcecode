using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Model.Interfaces;
using Moq;
using DsmSuite.DsmViewer.Application.Import;
using DsmSuite.DsmViewer.Application.Actions.Management;
using DsmSuite.Common.Model.Interface;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Application.Actions.Element;
using System.Collections.Generic;
using DsmSuite.DsmViewer.Application.Actions.Relation;
using DsmSuite.DsmViewer.Application.Actions.Snapshot;

namespace DsmSuite.DsmViewer.Application.Test.Import
{
    [TestClass]
    public class UpdateExistingModelPolicyTest
    {
        Mock<IDsmModel> _dsmModel;

        private const string DsmFilename = "file.dsm";
            
        Mock<IActionManager> _actionManager;

        Mock<IMetaDataItem> _createMetaDataItem;
        private const string MetaDataGroup = "group";
        private const string MetaDataItemName = "itemname";
        private const string MetaDataItemValue = "itemvalue";

        Mock<IDsmElement> _existingElement;
        Mock<IDsmElement> _createdElement;
        Mock<IDsmElement> _elementParent;
        private const int ElementId = 1;
        private const string ElementFullName = "a.b.c";
        private const string ElementName = "c";
        private const string ElementType = "etype";
        private const int ElementParentId = 2;

        Mock<IDsmRelation> _existingRelation;
        Mock<IDsmRelation> _createdRelation;
        Mock<IDsmElement> _consumer;
        Mock<IDsmElement> _provider;
        private const int ConsumerId = 3;
        private const int ProviderId = 4;
        private const string RelationType = "rtype";
        private const int RelationWeight = 4;
        private const int NewRelationWeight = 5;

        [TestInitialize]
        public void TestInitialize()
        {
            _dsmModel = new Mock<IDsmModel>();

            _actionManager = new Mock<IActionManager>();

            _createMetaDataItem = new Mock<IMetaDataItem>();

            _existingElement = new Mock<IDsmElement>();
            _existingElement.Setup(x => x.Id).Returns(ElementId);
            _createdElement = new Mock<IDsmElement>();
            _elementParent = new Mock<IDsmElement>();
            _elementParent.Setup(x => x.Id).Returns(ElementParentId);
            _consumer = new Mock<IDsmElement>();
            _consumer.Setup(x => x.Id).Returns(ConsumerId);
            _provider = new Mock<IDsmElement>();
            _provider.Setup(x => x.Id).Returns(ProviderId);

            _dsmModel.Setup(x => x.GetElementById(ConsumerId)).Returns(_consumer.Object);
            _dsmModel.Setup(x => x.GetElementById(ProviderId)).Returns(_provider.Object);

            _existingRelation = new Mock<IDsmRelation>();
            _existingRelation.Setup(x => x.Id).Returns(ProviderId);
            _existingRelation.Setup(x => x.Consumer.Id).Returns(ConsumerId);
            _existingRelation.Setup(x => x.Provider.Id).Returns(ProviderId);
            _existingRelation.Setup(x => x.Type).Returns(RelationType);
            _existingRelation.Setup(x => x.Weight).Returns(RelationWeight);

            _createdRelation = new Mock<IDsmRelation>();
        }

        [TestMethod]
        public void WhenPolicyIsConstructedThenModelIsLoaded()
        {
            UpdateExistingModelPolicy policy = new UpdateExistingModelPolicy(_dsmModel.Object, DsmFilename, _actionManager.Object, null);

            _dsmModel.Verify(x => x.LoadModel(DsmFilename, null), Times.Once());
        }

        [TestMethod]
        public void WhenMetaDataItemIsImportedThenMetaDataItemIsAddedToModel()
        {
            UpdateExistingModelPolicy policy = new UpdateExistingModelPolicy(_dsmModel.Object, DsmFilename, _actionManager.Object, null);

            _dsmModel.Setup(x => x.AddMetaData(MetaDataGroup, MetaDataItemName, MetaDataItemValue)).Returns(_createMetaDataItem.Object);

            IMetaDataItem metaDataItem = policy.ImportMetaDataItem(MetaDataGroup, MetaDataItemName, MetaDataItemValue);
            Assert.AreEqual(_createMetaDataItem.Object, metaDataItem);

            _dsmModel.Verify(x => x.AddMetaData(MetaDataGroup, MetaDataItemName, MetaDataItemValue), Times.Once());
        }

        [TestMethod]
        public void GivenElementIsNotInModelWhenElementIsImportedThenElementCreateActionIsExecuted()
        {
            IDsmElement foundElement = null;
            _dsmModel.Setup(x => x.GetElementByFullname(ElementFullName)).Returns(foundElement);
            _actionManager.Setup(x => x.Execute(It.IsAny<ElementCreateAction>())).Returns(_createdElement.Object);

            UpdateExistingModelPolicy policy = new UpdateExistingModelPolicy(_dsmModel.Object, DsmFilename, _actionManager.Object, null);
            IDsmElement element = policy.ImportElement(ElementFullName, ElementName, ElementType, _elementParent.Object);
            Assert.AreEqual(_createdElement.Object, element);

            _actionManager.Verify(x => x.Execute(It.IsAny<ElementCreateAction>()), Times.Once);
        }

        [TestMethod]
        public void GivenElementIsInModelWhenElementIsImportedThenNoActionIsTaken()
        {
            IDsmElement foundElement = _existingElement.Object;
            _dsmModel.Setup(x => x.GetElementByFullname(ElementFullName)).Returns(foundElement);

            UpdateExistingModelPolicy policy = new UpdateExistingModelPolicy(_dsmModel.Object, DsmFilename, _actionManager.Object, null);
            IDsmElement element = policy.ImportElement(ElementFullName, ElementName, ElementType, _elementParent.Object);
            Assert.AreEqual(_existingElement.Object, element);

            _actionManager.Verify(x => x.Execute(It.IsAny<IAction>()), Times.Never);
        }

        [TestMethod]
        public void GivenElementIsInModelButNotImportedAgainWhenImportIsFinalizedThenElementDeleteActionIsExecuted()
        {
            IDsmElement foundElement = _existingElement.Object;
            _dsmModel.Setup(x => x.GetElementByFullname(ElementFullName)).Returns(foundElement);
            List<IDsmElement> elements = new List<IDsmElement>() { _existingElement.Object };
            _dsmModel.Setup(x => x.GetElements()).Returns(elements);

            UpdateExistingModelPolicy policy = new UpdateExistingModelPolicy(_dsmModel.Object, DsmFilename, _actionManager.Object, null);
            policy.FinalizeImport(null);

            _actionManager.Verify(x => x.Execute(It.IsAny<ElementDeleteAction>()), Times.Once);
        }

        [TestMethod]
        public void GivenRelationIsNotInModelWhenRelationIsImportedThenRelationCreateActionIsExecuted()
        {
            IDsmRelation foundRelation = null;
            _dsmModel.Setup(x => x.FindRelation(ConsumerId, ProviderId, RelationType)).Returns(foundRelation);
            _actionManager.Setup(x => x.Execute(It.IsAny<RelationCreateAction>())).Returns(_createdRelation.Object);

            UpdateExistingModelPolicy policy = new UpdateExistingModelPolicy(_dsmModel.Object, DsmFilename, _actionManager.Object, null);
            IDsmRelation relation = policy.ImportRelation(ConsumerId, ProviderId, RelationType, RelationWeight);
            Assert.AreEqual(_createdRelation.Object, relation);

            _actionManager.Verify(x => x.Execute(It.IsAny<RelationCreateAction>()), Times.Once);
        }

        [TestMethod]
        public void GivenRelationIsInModelWithTheSameWeightWhenRelationIsImportedThenNoActionIsTaken()
        {
            IDsmRelation foundRelation = _existingRelation.Object;
            _dsmModel.Setup(x => x.FindRelation(ConsumerId, ProviderId, RelationType)).Returns(foundRelation);

            UpdateExistingModelPolicy policy = new UpdateExistingModelPolicy(_dsmModel.Object, DsmFilename, _actionManager.Object, null);
            IDsmRelation relation = policy.ImportRelation(ConsumerId, ProviderId, RelationType, RelationWeight);
            Assert.AreEqual(_existingRelation.Object, relation);

            _actionManager.Verify(x => x.Execute(It.IsAny<IAction>()), Times.Never);
        }

        [TestMethod]
        public void GivenRelationIsInModelWithTheOtherWeightWhenRelationIsImportedThenRelationUpdateWeigthActionIsExecuted()
        {
            IDsmRelation foundRelation = _existingRelation.Object;
            _dsmModel.Setup(x => x.FindRelation(ConsumerId, ProviderId, RelationType)).Returns(foundRelation);

            UpdateExistingModelPolicy policy = new UpdateExistingModelPolicy(_dsmModel.Object, DsmFilename, _actionManager.Object, null);
            IDsmRelation relation = policy.ImportRelation(ConsumerId, ProviderId, RelationType, NewRelationWeight);
            Assert.AreEqual(_existingRelation.Object, relation);

            _actionManager.Verify(x => x.Execute(It.IsAny<RelationChangeWeightAction>()), Times.Once);
        }

        [TestMethod]
        public void GivenRelationIsInModelButNotImportedAgainWhenImportIsFinalizedThenRelationDeleteActionIsExecuted()
        {
            IDsmRelation foundRelation = _existingRelation.Object;
            _dsmModel.Setup(x => x.FindRelation(ConsumerId, ProviderId, RelationType)).Returns(foundRelation);
            List<IDsmRelation> relations = new List<IDsmRelation>() { _existingRelation.Object };
            _dsmModel.Setup(x => x.GetRelations()).Returns(relations);

            UpdateExistingModelPolicy policy = new UpdateExistingModelPolicy(_dsmModel.Object, DsmFilename, _actionManager.Object, null);
            policy.FinalizeImport(null);

            _actionManager.Verify(x => x.Execute(It.IsAny<RelationDeleteAction>()), Times.Once);
        }

        [TestMethod]
        public void WhenImportIsFinalizedThenElementOrderIsAssigned()
        {
            UpdateExistingModelPolicy policy = new UpdateExistingModelPolicy(_dsmModel.Object, DsmFilename, _actionManager.Object, null);

            policy.FinalizeImport(null);

            _dsmModel.Verify(x => x.AssignElementOrder(), Times.Once());
        }

        [TestMethod]
        public void WhenImportIsFinalizedThenMakeTimeStampActionIsExecuted()
        {
            UpdateExistingModelPolicy policy = new UpdateExistingModelPolicy(_dsmModel.Object, DsmFilename, _actionManager.Object, null);

            policy.FinalizeImport(null);

            _actionManager.Verify(x => x.Execute(It.IsAny<MakeSnapshotAction>()), Times.Once);
        }
    }
}
