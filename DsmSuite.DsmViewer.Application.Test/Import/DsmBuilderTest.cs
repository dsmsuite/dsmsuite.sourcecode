using System.Reflection;
using DsmSuite.DsmViewer.Application.Import;
using DsmSuite.DsmViewer.Model.Core;
using DsmSuite.DsmViewer.Model.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.Analyzer.Model.Interface;
using Moq;
using System.Collections.Generic;
using DsmSuite.Common.Model.Interface;
using DsmSuite.DsmViewer.Application.Actions.Management;

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

        private const int _dsiElementIdABC = 1;
        private const string _dsiElementNameA = "a";
        private const string _dsiElementNameAB = "a.b";
        private const string _dsiElementNameABC = "a.b.c";
        private const string _dsiElementTypeABC = "elementtype1";
        private const int _dsiElementIdABD = 2;
        private const string _dsiElementNameABD = "a.b.d";
        private const string _dsiElementTypeABD = "elementtype2";
        private const int _dsiElementIdE = 3;
        private const string _dsiElementNameE = "e";
        private const string _dsiElementTypeE = "elementtype3";

        private const string _dsiRelationTypeABCtoABD = "relationtype1";
        private const int _dsiRelationWeightABCtoABD = 4;
        private const string _dsiRelationTypeABDtoE = "relationtype2";
        private const int _dsiRelationWeightABDtoE = 5;
        private const string _dsiRelationTypeABCtoE = "relationtype3";
        private const int _dsiRelationWeightABCtoE = 6;

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

        Mock<IDsiModel> _dsiModel;
        Mock<IImportPolicy> _importPolicy;

        Mock<IMetaDataItem> _metaDataItem1;
        Mock<IMetaDataItem> _metaDataItem2;

        Mock<IDsiElement> _dsiElementABC;
        Mock<IDsiElement> _dsiElementABD;
        Mock<IDsiElement> _dsiElementE;

        Mock<IDsiRelation> _dsiRelationABCtoABD;
        Mock<IDsiRelation> _dsiRelationABDtoE;
        Mock<IDsiRelation> _dsiRelationABCtoE;

        Mock<IDsmElement> _dsmElementA;
        Mock<IDsmElement> _dsmElementB;
        Mock<IDsmElement> _dsmElementC;
        Mock<IDsmElement> _dsmElementD;
        Mock<IDsmElement> _dsmElementE;

        [TestInitialize]
        public void TestInitialize()
        {
            _dsiModel = new Mock<IDsiModel>();
            _importPolicy = new Mock<IImportPolicy>();

            List<string> metaDataGroups = new List<string>() { _metaDataGroup };
            _dsiModel.Setup(x => x.GetMetaDataGroups()).Returns(metaDataGroups);

            _metaDataItem1 = new Mock<IMetaDataItem>();
            _metaDataItem1.Setup(x => x.Name).Returns(_metaDataItemName1);
            _metaDataItem1.Setup(x => x.Value).Returns(_metaDataItemValue1);
            _metaDataItem2 = new Mock<IMetaDataItem>();
            _metaDataItem2.Setup(x => x.Name).Returns(_metaDataItemName2);
            _metaDataItem2.Setup(x => x.Value).Returns(_metaDataItemValue2);
            List<IMetaDataItem> metaDataItems = new List<IMetaDataItem>() { _metaDataItem1.Object, _metaDataItem2.Object };
            _dsiModel.Setup(x => x.GetMetaDataGroupItems(_metaDataGroup)).Returns(metaDataItems);

            _dsiElementABC = new Mock<IDsiElement>();
            _dsiElementABC.Setup(x => x.Id).Returns(_dsiElementIdABC);
            _dsiElementABC.Setup(x => x.Name).Returns(_dsiElementNameABC);
            _dsiElementABC.Setup(x => x.Type).Returns(_dsiElementTypeABC);
            _dsiElementABD = new Mock<IDsiElement>();
            _dsiElementABD.Setup(x => x.Id).Returns(_dsiElementIdABD);
            _dsiElementABD.Setup(x => x.Name).Returns(_dsiElementNameABD);
            _dsiElementABD.Setup(x => x.Type).Returns(_dsiElementTypeABD);
            _dsiElementE = new Mock<IDsiElement>();
            _dsiElementE.Setup(x => x.Id).Returns(_dsiElementIdE);
            _dsiElementE.Setup(x => x.Name).Returns(_dsiElementNameE);
            _dsiElementE.Setup(x => x.Type).Returns(_dsiElementTypeE);
            List<IDsiElement> dsiElements = new List<IDsiElement>() { _dsiElementABC.Object, _dsiElementABD.Object, _dsiElementE.Object };
            _dsiModel.Setup(x => x.GetElements()).Returns(dsiElements);

            _dsiRelationABCtoABD = new Mock<IDsiRelation>();
            _dsiRelationABCtoABD.Setup(x => x.ConsumerId).Returns(_dsiElementIdABC);
            _dsiRelationABCtoABD.Setup(x => x.ProviderId).Returns(_dsiElementIdABD);
            _dsiRelationABCtoABD.Setup(x => x.Type).Returns(_dsiRelationTypeABCtoABD);
            _dsiRelationABCtoABD.Setup(x => x.Weight).Returns(_dsiRelationWeightABCtoABD);
            _dsiRelationABDtoE = new Mock<IDsiRelation>();
            _dsiRelationABDtoE.Setup(x => x.ConsumerId).Returns(_dsiElementIdABD);
            _dsiRelationABDtoE.Setup(x => x.ProviderId).Returns(_dsiElementIdE);
            _dsiRelationABDtoE.Setup(x => x.Type).Returns(_dsiRelationTypeABDtoE);
            _dsiRelationABDtoE.Setup(x => x.Weight).Returns(_dsiRelationWeightABDtoE);
            _dsiRelationABCtoE = new Mock<IDsiRelation>();
            _dsiRelationABCtoE.Setup(x => x.ConsumerId).Returns(_dsiElementIdABC);
            _dsiRelationABCtoE.Setup(x => x.ProviderId).Returns(_dsiElementIdE);
            _dsiRelationABCtoE.Setup(x => x.Type).Returns(_dsiRelationTypeABCtoE);
            _dsiRelationABCtoE.Setup(x => x.Weight).Returns(_dsiRelationWeightABCtoE);
            List<IDsiRelation> dsiRelations = new List<IDsiRelation>() { _dsiRelationABCtoABD.Object, _dsiRelationABDtoE.Object, _dsiRelationABCtoE.Object };
            _dsiModel.Setup(x => x.GetRelations()).Returns(dsiRelations);

            _dsmElementA = new Mock<IDsmElement>();
            _dsmElementA.Setup(x => x.Id).Returns(_dsmElementIdA);
            _dsmElementA.Setup(x => x.Name).Returns(_dsmElementNameA);
            _dsmElementA.Setup(x => x.Type).Returns(string.Empty);
            _dsmElementB = new Mock<IDsmElement>();
            _dsmElementB.Setup(x => x.Id).Returns(_dsmElementIdB);
            _dsmElementB.Setup(x => x.Name).Returns(_dsmElementNameB);
            _dsmElementB.Setup(x => x.Type).Returns(string.Empty);
            _dsmElementC = new Mock<IDsmElement>();
            _dsmElementC.Setup(x => x.Id).Returns(_dsmElementIdC);
            _dsmElementC.Setup(x => x.Name).Returns(_dsmElementNameC);
            _dsmElementC.Setup(x => x.Type).Returns(_dsiElementTypeABC);
            _dsmElementD = new Mock<IDsmElement>();
            _dsmElementD.Setup(x => x.Id).Returns(_dsmElementIdD);
            _dsmElementD.Setup(x => x.Name).Returns(_dsmElementNameD);
            _dsmElementD.Setup(x => x.Type).Returns(_dsiElementTypeABD);
            _dsmElementE = new Mock<IDsmElement>();
            _dsmElementE.Setup(x => x.Id).Returns(_dsmElementIdE);
            _dsmElementE.Setup(x => x.Name).Returns(_dsmElementNameE);
            _dsmElementE.Setup(x => x.Type).Returns(_dsiElementTypeE);

            _importPolicy.Setup(x => x.ImportElement(_dsiElementNameA, _dsmElementNameA, string.Empty, null)).Returns(_dsmElementA.Object);
            _importPolicy.Setup(x => x.ImportElement(_dsiElementNameAB, _dsmElementNameB, string.Empty, _dsmElementA.Object)).Returns(_dsmElementB.Object);
            _importPolicy.Setup(x => x.ImportElement(_dsiElementNameABC, _dsmElementNameC, _dsiElementTypeABC, _dsmElementB.Object)).Returns(_dsmElementC.Object);
            _importPolicy.Setup(x => x.ImportElement(_dsiElementNameABD, _dsmElementNameD, _dsiElementTypeABD, _dsmElementB.Object)).Returns(_dsmElementD.Object);
            _importPolicy.Setup(x => x.ImportElement(_dsmElementNameE, _dsmElementNameE, _dsiElementTypeE, null)).Returns(_dsmElementE.Object);
        }

        [TestMethod]
        public void WhenBuildingDsmThenMetaDataIsCopied()
        {
            DsmBuilder builder = new DsmBuilder(_dsiModel.Object, _importPolicy.Object);
            builder.Build();

            _importPolicy.Verify(x => x.ImportMetaDataItem(_metaDataGroup, _metaDataItemName1, _metaDataItemValue1), Times.Once());
            _importPolicy.Verify(x => x.ImportMetaDataItem(_metaDataGroup, _metaDataItemName2, _metaDataItemValue2), Times.Once());
            _importPolicy.Verify(x => x.ImportMetaDataItem(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [TestMethod]
        public void WhenBuildingDsmThenElementHierarchyIsCreated()
        {
            DsmBuilder builder = new DsmBuilder(_dsiModel.Object, _importPolicy.Object);
            builder.Build();

            _importPolicy.Verify(x => x.ImportElement(_dsiElementNameA, _dsmElementNameA, string.Empty, null), Times.Exactly(2)); // For a.b.c and a.b.d
            _importPolicy.Verify(x => x.ImportElement(_dsiElementNameAB, _dsmElementNameB, string.Empty, _dsmElementA.Object), Times.Exactly(2));  // For a.b.c and a.b.d
            _importPolicy.Verify(x => x.ImportElement(_dsiElementNameABC, _dsmElementNameC, _dsiElementTypeABC, _dsmElementB.Object), Times.Exactly(1)); // For a.b.c
            _importPolicy.Verify(x => x.ImportElement(_dsiElementNameABD, _dsmElementNameD, _dsiElementTypeABD, _dsmElementB.Object), Times.Exactly(1)); // For a.b.d
            _importPolicy.Verify(x => x.ImportElement(_dsiElementNameE, _dsmElementNameE, _dsiElementTypeE, null), Times.Exactly(1)); // For e
            _importPolicy.Verify(x => x.ImportElement(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDsmElement>()), Times.Exactly(7));
        }

        [TestMethod]
        public void WhenBuildingDsmThenRelationsAreResolvedAndAdded()
        {
            DsmBuilder builder = new DsmBuilder(_dsiModel.Object, _importPolicy.Object);
            builder.Build();

            _importPolicy.Verify(x => x.ImportRelation(_dsmElementIdC, _dsmElementIdD, _dsiRelationTypeABCtoABD, _dsiRelationWeightABCtoABD), Times.Exactly(1)); // Between a.b.c and a.b.d
            _importPolicy.Verify(x => x.ImportRelation(_dsmElementIdD, _dsmElementIdE, _dsiRelationTypeABDtoE, _dsiRelationWeightABDtoE), Times.Exactly(1)); // Between a.b.d and a
            _importPolicy.Verify(x => x.ImportRelation(_dsmElementIdC, _dsmElementIdE, _dsiRelationTypeABCtoE, _dsiRelationWeightABCtoE), Times.Exactly(1)); // Between a.b.d and a
            _importPolicy.Verify(x => x.ImportRelation(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(3));
        }
    }
}
