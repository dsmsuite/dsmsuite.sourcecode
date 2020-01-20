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
        private const string MetaDataGroup = "group";
        private const string MetaDataItemName = "itemname";
        private const string MetaDataItemValue = "itemvalue";

        Mock<IDsmElement> _existingElement;
        Mock<IDsmElement> _createdElement;
        Mock<IDsmElement> _elementParent;
        Mock<IDsmElement> _elementChild;
        private const string ElementFullName = "a.b.c";
        private const string ElementName = "c";
        private const string ElementType = "etype";
        private const int ElementParentId = 1;

        Mock<IDsmRelation> _createdRelation;
        private const int ConsumerId = 2;
        private const int ProviderId = 3;
        private const string RelationType = "rtype";
        private const int RelationWeight = 4;

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

            _elementParent.Setup(x => x.Id).Returns(ElementParentId);

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
            _dsmModel.Setup(x => x.AddMetaData(MetaDataGroup, MetaDataItemName, MetaDataItemValue)).Returns(_createMetaDataItem.Object);

            CreateNewModelPolicy policy = new CreateNewModelPolicy(_dsmModel.Object, false);
            IMetaDataItem metaDataItem = policy.ImportMetaDataItem(MetaDataGroup, MetaDataItemName, MetaDataItemValue);
            Assert.AreEqual(_createMetaDataItem.Object, metaDataItem);

            _dsmModel.Verify(x => x.AddMetaData(MetaDataGroup, MetaDataItemName, MetaDataItemValue), Times.Once());
        }

        [TestMethod]
        public void GivenElementIsNotInModelWhenElementIsImportedThenElementIsAddedToModel()
        {
            IDsmElement foundElement = null;
            _dsmModel.Setup(x => x.GetElementByFullname(ElementFullName)).Returns(foundElement);
            _dsmModel.Setup(x => x.AddElement(ElementName, ElementType, ElementParentId)).Returns(_createdElement.Object);

            CreateNewModelPolicy policy = new CreateNewModelPolicy(_dsmModel.Object, false);
            IDsmElement element = policy.ImportElement(ElementFullName, ElementName, ElementType, _elementParent.Object);
            Assert.AreEqual(_createdElement.Object, element);

            _dsmModel.Verify(x => x.AddElement(ElementName, ElementType, ElementParentId), Times.Once());
        }

        [TestMethod]
        public void GivenElementIsInModelWhenElementIsImportedThenElementIsNotAddedAgainToModel()
        {
            IDsmElement foundElement = _existingElement.Object;
            _dsmModel.Setup(x => x.GetElementByFullname(ElementFullName)).Returns(foundElement);

            CreateNewModelPolicy policy = new CreateNewModelPolicy(_dsmModel.Object, false);
            IDsmElement element = policy.ImportElement(ElementFullName, ElementName, ElementType, _elementParent.Object);
            Assert.AreEqual(_existingElement.Object, element);

            _dsmModel.Verify(x => x.AddElement(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>()), Times.Never());
        }

        [TestMethod]
        public void WhenRelationIsImportedThenRelationIsAddedToModel()
        {
            _dsmModel.Setup(x => x.AddRelation(ConsumerId, ProviderId, RelationType, RelationWeight)).Returns(_createdRelation.Object);

            CreateNewModelPolicy policy = new CreateNewModelPolicy(_dsmModel.Object, false);
            IDsmRelation relation = policy.ImportRelation(ConsumerId, ProviderId, RelationType, RelationWeight);
            Assert.AreEqual(_createdRelation.Object, relation);

            _dsmModel.Verify(x => x.AddRelation(ConsumerId, ProviderId, RelationType, RelationWeight), Times.Once());
        }

        [TestMethod]
        public void WhenImportIsFinalizedThenElementOrderIsAssigned()
        {
            CreateNewModelPolicy policy = new CreateNewModelPolicy(_dsmModel.Object, false);

            policy.FinalizeImport(null);

            _dsmModel.Verify(x => x.AssignElementOrder(), Times.Once());
        }

        [TestMethod]
        public void GiveAutoPartitionIsOffWhenImportIsFinalizedThenNoPartitioningIsDone()
        {
            _dsmModel.Setup(x => x.GetRootElement()).Returns(_existingElement.Object);

            CreateNewModelPolicy policy = new CreateNewModelPolicy(_dsmModel.Object, false);

            policy.FinalizeImport(null);

            _dsmModel.Verify(x => x.ReorderChildren(It.IsAny<IDsmElement>(), It.IsAny<ISortResult>()), Times.Never());
        }

        [TestMethod]
        public void GiveAutoPartitionIsOnWhenImportIsFinalizedThenPartitioningIsDone()
        {
            _dsmModel.Setup(x => x.GetRootElement()).Returns(_existingElement.Object);
            List<IDsmElement> children = new List<IDsmElement>() { _elementChild.Object };
            _existingElement.Setup(x => x.Children).Returns(children);
            List<IDsmElement> nochildren = new List<IDsmElement>();
            _elementChild.Setup(x => x.Children).Returns(nochildren);

            CreateNewModelPolicy policy = new CreateNewModelPolicy(_dsmModel.Object, true);

            policy.FinalizeImport(null);

            _dsmModel.Verify(x => x.ReorderChildren(_existingElement.Object, It.IsAny<ISortResult>()), Times.Exactly(1));
            _dsmModel.Verify(x => x.ReorderChildren(_elementChild.Object, It.IsAny<ISortResult>()), Times.Exactly(1));
            _dsmModel.Verify(x => x.ReorderChildren(It.IsAny<IDsmElement>(), It.IsAny<ISortResult>()), Times.Exactly(2));
        }
    }
}
