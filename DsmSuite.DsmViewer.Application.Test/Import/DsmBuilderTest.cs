using System.Reflection;
using DsmSuite.DsmViewer.Application.Import;
using DsmSuite.DsmViewer.Model.Core;
using DsmSuite.DsmViewer.Model.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.Analyzer.Model.Interface;
using Moq;
using System.Collections.Generic;
using DsmSuite.Common.Model.Interface;

namespace DsmSuite.DsmViewer.Application.Test.Import
{
    [TestClass]
    public class DsmBuilderTest
    {
        private const string _metaDataGroup = "group";
        private const string _metaDataItemName1 = "itemname1";
        private const string _metaDataItemValue1 = "itemvalue1";
        private const string _metaDataItemName2 = "itemname2";
        private const string _metaDataItemValue2 = "itemvalue2";

        private const int _dsiElementId1= 1;
        private const string _dsiElementName1 = "a.b.c";
        private const string _dsiElementType1 = "elementtype1";
        private const int _dsiElementId2 = 2;
        private const string _dsiElementName2 = "a.b.d";
        private const string _dsiElementType2 = "elementtype2";
        private const int _dsiElementId3 = 3;
        private const string _dsiElementName3 = "e";
        private const string _dsiElementType3 = "elementtype3";

        private const string _dsiRelationType1 = "relationtype1";
        private const int _dsiRelationWeight1 = 4;
        private const string _dsiRelationType2 = "relationtype2";
        private const int _dsiRelationWeight2 = 5;
        private const string _dsiRelationType3 = "relationtype3";
        private const int _dsiRelationWeight3 = 6;

        private const int _dsmElementIdA = 7;
        private const string _dsmElementNameA = "a";
        private const int _dsmElementIdB = 8;
        private const string _dsmElementNameB = "b";
        private const int _dsmElementIdC = 9;
        private const string _dsmElementNameC = "c";
        private const int _dsmElementIdD = 10;
        private const string _dsmElementNameD = "d";
        private const int _dsmElementIdE = 11;
        private const string _dsmElementNameE = "e";

        Mock<IDsiDataModel> dsiModel;
        Mock<IDsmModel> dsmModel;

        Mock<IMetaDataItem> metaDataItem1;
        Mock<IMetaDataItem> metaDataItem2;

        Mock<IDsiElement> dsiElement1;
        Mock<IDsiElement> dsiElement2;
        Mock<IDsiElement> dsiElement3;

        Mock<IDsiRelation> dsiRelation1;
        Mock<IDsiRelation> dsiRelation2;
        Mock<IDsiRelation> dsiRelation3;

        Mock<IDsmElement> dsmElementA;
        Mock<IDsmElement> dsmElementB;
        Mock<IDsmElement> dsmElementC;
        Mock<IDsmElement> dsmElementD;
        Mock<IDsmElement> dsmElementE;

        [TestInitialize]
        public void TestInitialize()
        {
            dsiModel = new Mock<IDsiDataModel>();
            dsmModel = new Mock<IDsmModel>();

            List<string> metaDataGroups = new List<string>() { _metaDataGroup };
            dsiModel.Setup(x => x.GetMetaDataGroups()).Returns(metaDataGroups);

            metaDataItem1 = new Mock<IMetaDataItem>();
            metaDataItem1.Setup(x => x.Name).Returns(_metaDataItemName1);
            metaDataItem1.Setup(x => x.Value).Returns(_metaDataItemValue1);
            Mock<IMetaDataItem> metaDataItem2 = new Mock<IMetaDataItem>();
            metaDataItem2.Setup(x => x.Name).Returns(_metaDataItemName2);
            metaDataItem2.Setup(x => x.Value).Returns(_metaDataItemValue2);
            List<IMetaDataItem> metaDataItems = new List<IMetaDataItem>() { metaDataItem1.Object, metaDataItem2.Object };
            dsiModel.Setup(x => x.GetMetaDataGroupItems(_metaDataGroup)).Returns(metaDataItems);

            dsiElement1 = new Mock<IDsiElement>();
            dsiElement1.Setup(x => x.Id).Returns(_dsiElementId1);
            dsiElement1.Setup(x => x.Name).Returns(_dsiElementName1);
            dsiElement1.Setup(x => x.Type).Returns(_dsiElementType1);
            dsiElement2 = new Mock<IDsiElement>();
            dsiElement2.Setup(x => x.Id).Returns(_dsiElementId2);
            dsiElement2.Setup(x => x.Name).Returns(_dsiElementName2);
            dsiElement2.Setup(x => x.Type).Returns(_dsiElementType2);
            dsiElement3 = new Mock<IDsiElement>();
            dsiElement3.Setup(x => x.Id).Returns(_dsiElementId3);
            dsiElement3.Setup(x => x.Name).Returns(_dsiElementName3);
            dsiElement3.Setup(x => x.Type).Returns(_dsiElementType3);
            List<IDsiElement> dsiElements = new List<IDsiElement>() { dsiElement1.Object, dsiElement2.Object, dsiElement3.Object };
            dsiModel.Setup(x => x.GetElements()).Returns(dsiElements);

            dsiRelation1 = new Mock<IDsiRelation>();
            dsiRelation1.Setup(x => x.ConsumerId).Returns(_dsiElementId1);
            dsiRelation1.Setup(x => x.ProviderId).Returns(_dsiElementId2);
            dsiRelation1.Setup(x => x.Type).Returns(_dsiRelationType1);
            dsiRelation1.Setup(x => x.Weight).Returns(_dsiRelationWeight1);
            dsiRelation2 = new Mock<IDsiRelation>();
            dsiRelation2.Setup(x => x.ConsumerId).Returns(_dsiElementId2);
            dsiRelation2.Setup(x => x.ProviderId).Returns(_dsiElementId3);
            dsiRelation2.Setup(x => x.Type).Returns(_dsiRelationType2);
            dsiRelation2.Setup(x => x.Weight).Returns(_dsiRelationWeight2);
            dsiRelation3 = new Mock<IDsiRelation>();
            dsiRelation3.Setup(x => x.ConsumerId).Returns(_dsiElementId1);
            dsiRelation3.Setup(x => x.ProviderId).Returns(_dsiElementId3);
            dsiRelation3.Setup(x => x.Type).Returns(_dsiRelationType3);
            dsiRelation3.Setup(x => x.Weight).Returns(_dsiRelationWeight3);
            List<IDsiRelation> dsiRelations = new List<IDsiRelation>() { dsiRelation1.Object, dsiRelation2.Object, dsiRelation3.Object };
            dsiModel.Setup(x => x.GetRelations()).Returns(dsiRelations);

            dsmElementA = new Mock<IDsmElement>();
            dsmElementA.Setup(x => x.Id).Returns(_dsmElementIdA);
            dsmElementA.Setup(x => x.Name).Returns(_dsmElementNameA);
            dsmElementA.Setup(x => x.Type).Returns(string.Empty);
            dsmElementB = new Mock<IDsmElement>();
            dsmElementB.Setup(x => x.Id).Returns(_dsmElementIdB);
            dsmElementB.Setup(x => x.Name).Returns(_dsmElementNameB);
            dsmElementB.Setup(x => x.Type).Returns(string.Empty);
            dsmElementC = new Mock<IDsmElement>();
            dsmElementC.Setup(x => x.Id).Returns(_dsmElementIdC);
            dsmElementC.Setup(x => x.Name).Returns(_dsmElementNameC);
            dsmElementC.Setup(x => x.Type).Returns(_dsiElementType1);
            dsmElementD = new Mock<IDsmElement>();
            dsmElementD.Setup(x => x.Id).Returns(_dsmElementIdD);
            dsmElementD.Setup(x => x.Name).Returns(_dsmElementNameD);
            dsmElementD.Setup(x => x.Type).Returns(_dsiElementType2);
            dsmElementE = new Mock<IDsmElement>();
            dsmElementE.Setup(x => x.Id).Returns(_dsmElementIdE);
            dsmElementE.Setup(x => x.Name).Returns(_dsmElementNameE);
            dsmElementE.Setup(x => x.Type).Returns(_dsiElementType3);

            dsmModel.Setup(x => x.AddElement(_dsmElementNameA, string.Empty, null)).Returns(dsmElementA.Object);
            dsmModel.Setup(x => x.AddElement(_dsmElementNameB, string.Empty, _dsmElementIdA)).Returns(dsmElementB.Object);
            dsmModel.Setup(x => x.AddElement(_dsmElementNameC, _dsiElementType1, _dsmElementIdB)).Returns(dsmElementC.Object);
            dsmModel.Setup(x => x.AddElement(_dsmElementNameD, _dsiElementType2, _dsmElementIdB)).Returns(dsmElementD.Object);
            dsmModel.Setup(x => x.AddElement(_dsmElementNameE, _dsiElementType3, null)).Returns(dsmElementE.Object);
        }

        [TestMethod]
        public void WhenBuildingDsmThenMetaDataIsCopie()
        {
            DsmBuilder builder = new DsmBuilder(dsiModel.Object, dsmModel.Object);
            builder.CreateDsm(false);

            dsmModel.Verify(x => x.AddMetaData(_metaDataGroup, _metaDataItemName1, _metaDataItemValue1), Times.Once());
            dsmModel.Verify(x => x.AddMetaData(_metaDataGroup, _metaDataItemName2, _metaDataItemValue2), Times.Once());
            dsmModel.Verify(x => x.AddMetaData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [TestMethod]
        public void WhenBuildingDsmThenElementHierarchyIsCreated()
        {
            DsmBuilder builder = new DsmBuilder(dsiModel.Object, dsmModel.Object);
            builder.CreateDsm(false);

            dsmModel.Verify(x => x.AddElement(_dsmElementNameA, string.Empty, null), Times.Exactly(2)); // For a.b.c and a.b.d
            dsmModel.Verify(x => x.AddElement(_dsmElementNameB, string.Empty, _dsmElementIdA), Times.Exactly(2));  // For a.b.c and a.b.d
            dsmModel.Verify(x => x.AddElement(_dsmElementNameC, _dsiElementType1, _dsmElementIdB), Times.Exactly(1)); // For a.b.c
            dsmModel.Verify(x => x.AddElement(_dsmElementNameD, _dsiElementType2, _dsmElementIdB), Times.Exactly(1)); // For a.b.d
            dsmModel.Verify(x => x.AddElement(_dsmElementNameE, _dsiElementType3, null), Times.Exactly(1)); // For e
            dsmModel.Verify(x => x.AddElement(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>()), Times.Exactly(7));
        }

        [TestMethod]
        public void WhenBuildingDsmThenRelationsAreResolvedAndAdded()
        {
            DsmBuilder builder = new DsmBuilder(dsiModel.Object, dsmModel.Object);
            builder.CreateDsm(false);

            dsmModel.Verify(x => x.AddRelation(_dsmElementIdC, _dsmElementIdD, _dsiRelationType1, _dsiRelationWeight1), Times.Exactly(1)); // Between a.b.c and a.b.d
            dsmModel.Verify(x => x.AddRelation(_dsmElementIdD, _dsmElementIdE, _dsiRelationType2, _dsiRelationWeight2), Times.Exactly(1)); // Between a.b.d and a
            dsmModel.Verify(x => x.AddRelation(_dsmElementIdC, _dsmElementIdE, _dsiRelationType3, _dsiRelationWeight3), Times.Exactly(1)); // Between a.b.d and a
            dsmModel.Verify(x => x.AddRelation(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(3));
        }
    }
}
