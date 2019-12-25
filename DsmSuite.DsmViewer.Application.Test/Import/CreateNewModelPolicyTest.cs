using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Application.Import;
using DsmSuite.DsmViewer.Application.Sorting;
using DsmSuite.Common.Model.Interface;
using System.Collections.Generic;
using DsmSuite.DsmViewer.Application.Test.Stubs;

namespace DsmSuite.DsmViewer.Application.Test.Import
{
    [TestClass]
    public class CreateNewModelPolicyTest
    {
        Mock<IDsmModel> _dsmModel;

        Mock<IMetaDataItem> _createMetaDataItem;
        private const string _metaDataGroup = "group";
        private const string _metaDataItemName = "itemname";
        private const string _metaDataItemValue = "itemvalue";

        Mock<IDsmElement> _existingElement;
        Mock<IDsmElement> _createdElement;
        Mock<IDsmElement> _elementParent;
        Mock<IDsmElement> _elementChild;
        private const string _elementFullName = "a.b.c";
        private const string _elementName = "c";
        private const string _elementType = "etype";
        private const int _elementParentId = 1;

        Mock<IDsmRelation> _createdRelation;
        private const int _consumerId = 2;
        private const int _providerId = 3;
        private const string _relationType = "rtype";
        private const int _relationWeight = 4;

        [TestInitialize]
        public void TestInitialize()
        {
            _dsmModel = new Mock<IDsmModel>();

            _createMetaDataItem = new Mock<IMetaDataItem>();
 
            _existingElement = new Mock<IDsmElement>();
            _createdElement = new Mock<IDsmElement>();
            _elementParent = new Mock<IDsmElement>();
            _elementChild = new Mock<IDsmElement>();

            _createdRelation = new Mock<IDsmRelation>();

            _elementParent.Setup(x => x.Id).Returns(_elementParentId);

            SortAlgorithmFactory.RegisterAlgorithm(PartitionSortAlgorithm.AlgorithmName, typeof(StubbedSortAlgorithm));
        }

        [TestMethod]
        public void WhenPolicyIsConstructedThenModelIsCleared()
        {
            CreateNewModelPolicy policy = new CreateNewModelPolicy(_dsmModel.Object, false);

            _dsmModel.Verify(x => x.Clear(), Times.Once());
        }

        [TestMethod]
        public void WhenMetaDataItemIsImportedThenMetaDataItemIsAddedToModel()
        {
            _dsmModel.Setup(x => x.AddMetaData(_metaDataGroup, _metaDataItemName, _metaDataItemValue)).Returns(_createMetaDataItem.Object);

            CreateNewModelPolicy policy = new CreateNewModelPolicy(_dsmModel.Object, false);
            IMetaDataItem metaDataItem = policy.ImportMetaDataItem(_metaDataGroup, _metaDataItemName, _metaDataItemValue);
            Assert.AreEqual(_createMetaDataItem.Object, metaDataItem);

            _dsmModel.Verify(x => x.AddMetaData(_metaDataGroup, _metaDataItemName, _metaDataItemValue), Times.Once());
        }

        [TestMethod]
        public void GivenElementIsNotInModelWhenElementIsImportedThenElementIsAddedToModel()
        {
            IDsmElement foundElement = null;
            _dsmModel.Setup(x => x.GetElementByFullname(_elementFullName)).Returns(foundElement);
            _dsmModel.Setup(x => x.AddElement(_elementName, _elementType, _elementParentId)).Returns(_createdElement.Object);

            CreateNewModelPolicy policy = new CreateNewModelPolicy(_dsmModel.Object, false);
            IDsmElement element = policy.ImportElement(_elementFullName, _elementName, _elementType, _elementParent.Object);
            Assert.AreEqual(_createdElement.Object, element);

            _dsmModel.Verify(x => x.AddElement(_elementName, _elementType, _elementParentId), Times.Once());
        }

        [TestMethod]
        public void GivenElementIsInModelWhenElementIsImportedThenElementIsNotAddedAgainToModel()
        {
            IDsmElement foundElement = _existingElement.Object;
            _dsmModel.Setup(x => x.GetElementByFullname(_elementFullName)).Returns(foundElement);

            CreateNewModelPolicy policy = new CreateNewModelPolicy(_dsmModel.Object, false);
            IDsmElement element = policy.ImportElement(_elementFullName, _elementName, _elementType, _elementParent.Object);
            Assert.AreEqual(_existingElement.Object, element);

            _dsmModel.Verify(x => x.AddElement(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>()), Times.Never());
        }

        [TestMethod]
        public void WhenRelationIsImportedThenRelationIsAddedToModel()
        {
            _dsmModel.Setup(x => x.AddRelation(_consumerId, _providerId, _relationType, _relationWeight)).Returns(_createdRelation.Object);

            CreateNewModelPolicy policy = new CreateNewModelPolicy(_dsmModel.Object, false);
            IDsmRelation relation = policy.ImportRelation(_consumerId, _providerId, _relationType, _relationWeight);
            Assert.AreEqual(_createdRelation.Object, relation);

            _dsmModel.Verify(x => x.AddRelation(_consumerId, _providerId, _relationType, _relationWeight), Times.Once());
        }

        [TestMethod]
        public void WhenImportIsFinalizedThenElementOrderIsAssigned()
        {
            CreateNewModelPolicy policy = new CreateNewModelPolicy(_dsmModel.Object, false);

            policy.FinalizeImport();

            _dsmModel.Verify(x => x.AssignElementOrder(), Times.Once());
        }

        [TestMethod]
        public void GiveAutoPartitionIsOffWhenImportIsFinalizedThenNoPartitioningIsDone()
        {
            List<IDsmElement> rootElements = new List<IDsmElement>() { _existingElement.Object };
            _dsmModel.Setup(x => x.GetRootElements()).Returns(rootElements);

            CreateNewModelPolicy policy = new CreateNewModelPolicy(_dsmModel.Object, false);

            policy.FinalizeImport();

            _dsmModel.Verify(x => x.ReorderChildren(It.IsAny<IDsmElement>(), It.IsAny<ISortResult>()), Times.Never());
        }

        [TestMethod]
        public void GiveAutoPartitionIsOnWhenImportIsFinalizedThenPartitioningIsDone()
        {
            List<IDsmElement> rootElements = new List<IDsmElement>() { _existingElement.Object };
            _dsmModel.Setup(x => x.GetRootElements()).Returns(rootElements);
            List<IDsmElement> children = new List<IDsmElement>() { _elementChild.Object };
            _existingElement.Setup(x => x.Children).Returns(children);
            List<IDsmElement> nochildren = new List<IDsmElement>();
            _elementChild.Setup(x => x.Children).Returns(nochildren);

            CreateNewModelPolicy policy = new CreateNewModelPolicy(_dsmModel.Object, true);

            policy.FinalizeImport();

            _dsmModel.Verify(x => x.ReorderChildren(_existingElement.Object, It.IsAny<ISortResult>()), Times.Exactly(1));
            _dsmModel.Verify(x => x.ReorderChildren(_elementChild.Object, It.IsAny<ISortResult>()), Times.Exactly(1));
            _dsmModel.Verify(x => x.ReorderChildren(It.IsAny<IDsmElement>(), It.IsAny<ISortResult>()), Times.Exactly(2));
        }
    }
}
