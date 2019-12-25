using System;
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

namespace DsmSuite.DsmViewer.Application.Test.Import
{
    [TestClass]
    public class UpdateExistingModelPolicyTest
    {
        Mock<IDsmModel> _dsmModel;

        private const string _dsmFilename = "file.dsm";
            
        Mock<IActionManager> _actionManager;

        Mock<IMetaDataItem> _createMetaDataItem;
        private const string _metaDataGroup = "group";
        private const string _metaDataItemName = "itemname";
        private const string _metaDataItemValue = "itemvalue";

        Mock<IDsmElement> _existingElement;
        Mock<IDsmElement> _createdElement;
        Mock<IDsmElement> _elementParent;
        Mock<IDsmElement> _elementChild;
        private const int _elementId = 1;
        private const string _elementFullName = "a.b.c";
        private const string _elementName = "c";
        private const string _elementType = "etype";
        private const int _elementParentId = 2;

        Mock<IDsmRelation> _existingRelation;
        Mock<IDsmRelation> _createdRelation;
        Mock<IDsmElement> _consumer;
        Mock<IDsmElement> _provider;
        private const int _consumerId = 3;
        private const int _providerId = 4;
        private const string _relationType = "rtype";
        private const int _relationWeight = 4;
        private const int _newRelationWeight = 5;

        [TestInitialize]
        public void TestInitialize()
        {
            _dsmModel = new Mock<IDsmModel>();

            _actionManager = new Mock<IActionManager>();

            _createMetaDataItem = new Mock<IMetaDataItem>();

            _existingElement = new Mock<IDsmElement>();
            _existingElement.Setup(x => x.Id).Returns(_elementId);
            _createdElement = new Mock<IDsmElement>();
            _elementParent = new Mock<IDsmElement>();
            _elementParent.Setup(x => x.Id).Returns(_elementParentId);
            _elementChild = new Mock<IDsmElement>();
            _consumer = new Mock<IDsmElement>();
            _consumer.Setup(x => x.Id).Returns(_consumerId);
            _provider = new Mock<IDsmElement>();
            _provider.Setup(x => x.Id).Returns(_providerId);

            _dsmModel.Setup(x => x.GetElementById(_consumerId)).Returns(_consumer.Object);
            _dsmModel.Setup(x => x.GetElementById(_providerId)).Returns(_provider.Object);

            _existingRelation = new Mock<IDsmRelation>();
            _existingRelation.Setup(x => x.Id).Returns(_providerId);
            _existingRelation.Setup(x => x.ConsumerId).Returns(_consumerId);
            _existingRelation.Setup(x => x.ProviderId).Returns(_providerId);
            _existingRelation.Setup(x => x.Type).Returns(_relationType);
            _existingRelation.Setup(x => x.Weight).Returns(_relationWeight);

            _createdRelation = new Mock<IDsmRelation>();
        }

        [TestMethod]
        public void WhenPolicyIsConstructedThenModelIsLoaded()
        {
            UpdateExistingModelPolicy policy = new UpdateExistingModelPolicy(_dsmModel.Object, _dsmFilename, _actionManager.Object);

            _dsmModel.Verify(x => x.LoadModel(_dsmFilename, null), Times.Once());
        }

        [TestMethod]
        public void WhenMetaDataItemIsImportedThenMetaDataItemIsAddedToModel()
        {
            UpdateExistingModelPolicy policy = new UpdateExistingModelPolicy(_dsmModel.Object, _dsmFilename, _actionManager.Object);

            _dsmModel.Setup(x => x.AddMetaData(_metaDataGroup, _metaDataItemName, _metaDataItemValue)).Returns(_createMetaDataItem.Object);

            IMetaDataItem metaDataItem = policy.ImportMetaDataItem(_metaDataGroup, _metaDataItemName, _metaDataItemValue);
            Assert.AreEqual(_createMetaDataItem.Object, metaDataItem);

            _dsmModel.Verify(x => x.AddMetaData(_metaDataGroup, _metaDataItemName, _metaDataItemValue), Times.Once());
        }

        [TestMethod]
        public void GivenElementIsNotInModelWhenElementIsImportedThenElementCreateActionIsExecuted()
        {
            IDsmElement foundElement = null;
            _dsmModel.Setup(x => x.GetElementByFullname(_elementFullName)).Returns(foundElement);
            _actionManager.Setup(x => x.Execute(It.IsAny<ElementCreateAction>())).Returns(_createdElement.Object);

            UpdateExistingModelPolicy policy = new UpdateExistingModelPolicy(_dsmModel.Object, _dsmFilename, _actionManager.Object);
            IDsmElement element = policy.ImportElement(_elementFullName, _elementName, _elementType, _elementParent.Object);
            Assert.AreEqual(_createdElement.Object, element);

            _actionManager.Verify(x => x.Execute(It.IsAny<ElementCreateAction>()), Times.Once);
        }

        [TestMethod]
        public void GivenElementIsInModelWhenElementIsImportedThenNoActionIsTaken()
        {
            IDsmElement foundElement = _existingElement.Object;
            _dsmModel.Setup(x => x.GetElementByFullname(_elementFullName)).Returns(foundElement);

            UpdateExistingModelPolicy policy = new UpdateExistingModelPolicy(_dsmModel.Object, _dsmFilename, _actionManager.Object);
            IDsmElement element = policy.ImportElement(_elementFullName, _elementName, _elementType, _elementParent.Object);
            Assert.AreEqual(_existingElement.Object, element);

            _actionManager.Verify(x => x.Execute(It.IsAny<IAction>()), Times.Never);
        }

        [TestMethod]
        public void GivenElementIsInModelButNotImportedAgainWhenImportIsFinalizedThenElementDeleteActionIsExecuted()
        {
            IDsmElement foundElement = _existingElement.Object;
            _dsmModel.Setup(x => x.GetElementByFullname(_elementFullName)).Returns(foundElement);
            List<IDsmElement> elements = new List<IDsmElement>() { _existingElement.Object };
            _dsmModel.Setup(x => x.GetElements()).Returns(elements);

            UpdateExistingModelPolicy policy = new UpdateExistingModelPolicy(_dsmModel.Object, _dsmFilename, _actionManager.Object);
            policy.FinalizeImport();

            _actionManager.Verify(x => x.Execute(It.IsAny<ElementDeleteAction>()), Times.Once);
        }

        [TestMethod]
        public void GivenRelationIsNotInModelWhenRelationIsImportedThenRelationCreateActionIsExecuted()
        {
            IDsmRelation foundRelation = null;
            _dsmModel.Setup(x => x.FindRelation(_consumerId, _providerId, _relationType)).Returns(foundRelation);
            _actionManager.Setup(x => x.Execute(It.IsAny<RelationCreateAction>())).Returns(_createdRelation.Object);

            UpdateExistingModelPolicy policy = new UpdateExistingModelPolicy(_dsmModel.Object, _dsmFilename, _actionManager.Object);
            IDsmRelation relation = policy.ImportRelation(_consumerId, _providerId, _relationType, _relationWeight);
            Assert.AreEqual(_createdRelation.Object, relation);

            _actionManager.Verify(x => x.Execute(It.IsAny<RelationCreateAction>()), Times.Once);
        }

        [TestMethod]
        public void GivenRelationIsInModelWithTheSameWeightWhenRelationIsImportedThenNoActionIsTaken()
        {
            IDsmRelation foundRelation = _existingRelation.Object;
            _dsmModel.Setup(x => x.FindRelation(_consumerId, _providerId, _relationType)).Returns(foundRelation);

            UpdateExistingModelPolicy policy = new UpdateExistingModelPolicy(_dsmModel.Object, _dsmFilename, _actionManager.Object);
            IDsmRelation relation = policy.ImportRelation(_consumerId, _providerId, _relationType, _relationWeight);
            Assert.AreEqual(_existingRelation.Object, relation);

            _actionManager.Verify(x => x.Execute(It.IsAny<IAction>()), Times.Never);
        }

        [TestMethod]
        public void GivenRelationIsInModelWithTheOtherWeightWhenRelationIsImportedThenRelationUpdateWeigthActionIsExecuted()
        {
            IDsmRelation foundRelation = _existingRelation.Object;
            _dsmModel.Setup(x => x.FindRelation(_consumerId, _providerId, _relationType)).Returns(foundRelation);

            UpdateExistingModelPolicy policy = new UpdateExistingModelPolicy(_dsmModel.Object, _dsmFilename, _actionManager.Object);
            IDsmRelation relation = policy.ImportRelation(_consumerId, _providerId, _relationType, _newRelationWeight);
            Assert.AreEqual(_existingRelation.Object, relation);

            _actionManager.Verify(x => x.Execute(It.IsAny<RelationChangeWeightAction>()), Times.Once);
        }

        [TestMethod]
        public void GivenRelationIsInModelButNotImportedAgainWhenImportIsFinalizedThenRelationDeleteActionIsExecuted()
        {
            IDsmRelation foundRelation = _existingRelation.Object;
            _dsmModel.Setup(x => x.FindRelation(_consumerId, _providerId, _relationType)).Returns(foundRelation);
            List<IDsmRelation> relations = new List<IDsmRelation>() { _existingRelation.Object };
            _dsmModel.Setup(x => x.GetRelations()).Returns(relations);

            UpdateExistingModelPolicy policy = new UpdateExistingModelPolicy(_dsmModel.Object, _dsmFilename, _actionManager.Object);
            policy.FinalizeImport();

            _actionManager.Verify(x => x.Execute(It.IsAny<RelationDeleteAction>()), Times.Once);
        }

        [TestMethod]
        public void WhenImportIsFinalizedThenElementOrderIsAssigned()
        {
            UpdateExistingModelPolicy policy = new UpdateExistingModelPolicy(_dsmModel.Object, _dsmFilename, _actionManager.Object);

            policy.FinalizeImport();

            _dsmModel.Verify(x => x.AssignElementOrder(), Times.Once());
        }
    }
}
