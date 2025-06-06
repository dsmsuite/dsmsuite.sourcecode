﻿using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Model.Interface;
using DsmSuite.DsmViewer.Application.Import.Common;
using DsmSuite.DsmViewer.Application.Import.Dsi;
using DsmSuite.DsmViewer.Model.Interfaces;
using Moq;

namespace DsmSuite.DsmViewer.Application.Test.Import.Dsi
{
    [TestClass]
    public class DsiImporterTest
    {
        private const string MetaDataGroup = "group";
        private const string MetaDataItemName1 = "itemname1";
        private const string MetaDataItemValue1 = "itemvalue1";
        private const string MetaDataItemName2 = "itemname2";
        private const string MetaDataItemValue2 = "itemvalue2";

        private const int DsiElementIdAbc = 1;
        private const string DsiElementNameA = "a";
        private const string DsiElementNameAb = "a.b";
        private const string DsiElementNameAbc = "a.b.c";
        private const string DsiElementTypeAbc = "elementtype1";
        private const int DsiElementIdAbd = 2;
        private const string DsiElementNameAbd = "a.b.d";
        private const string DsiElementTypeAbd = "elementtype2";
        private const int DsiElementIdE = 3;
        private const string DsiElementNameE = "e";
        private const string DsiElementTypeE = "elementtype3";

        private const string DsiRelationTypeAbCtoAbd = "relationtype1";
        private const int DsiRelationWeightAbCtoAbd = 4;
        private const string DsiRelationTypeAbDtoE = "relationtype2";
        private const int DsiRelationWeightAbDtoE = 5;
        private const string DsiRelationTypeAbCtoE = "relationtype3";
        private const int DsiRelationWeightAbCtoE = 6;

        private const int DsmElementIdA = 7;
        private const string DsmElementNameA = "a";
        private const int DsmElementIdB = 8;
        private const string DsmElementNameB = "b";
        private const int DsmElementIdC = 9;
        private const string DsmElementNameC = "c";
        private const int DsmElementIdD = 10;
        private const string DsmElementNameD = "d";
        private const int DsmElementIdE = 11;
        private const string DsmElementNameE = "e";

        Mock<IDsiModel> _dsiModel;
        Mock<IDsmModel> _dsmModel;

        Mock<IMetaDataItem> _metaDataItem1;
        Mock<IMetaDataItem> _metaDataItem2;

        Mock<IDsiElement> _dsiElementAbc;
        Mock<IDsiElement> _dsiElementAbd;
        Mock<IDsiElement> _dsiElementE;

        Mock<IDsiRelation> _dsiRelationAbCtoAbd;
        Mock<IDsiRelation> _dsiRelationAbDtoE;
        Mock<IDsiRelation> _dsiRelationAbCtoE;

        Mock<IDsmElement> _dsmElementA;
        Mock<IDsmElement> _dsmElementB;
        Mock<IDsmElement> _dsmElementC;
        Mock<IDsmElement> _dsmElementD;
        Mock<IDsmElement> _dsmElementE;

        [TestInitialize]
        public void TestInitialize()
        {
            _dsiModel = new Mock<IDsiModel>();
            _dsmModel = new Mock<IDsmModel>();

            List<string> metaDataGroups = new List<string>() { MetaDataGroup };
            _dsiModel.Setup(x => x.GetMetaDataGroups()).Returns(metaDataGroups);

            _metaDataItem1 = new Mock<IMetaDataItem>();
            _metaDataItem1.Setup(x => x.Name).Returns(MetaDataItemName1);
            _metaDataItem1.Setup(x => x.Value).Returns(MetaDataItemValue1);
            _metaDataItem2 = new Mock<IMetaDataItem>();
            _metaDataItem2.Setup(x => x.Name).Returns(MetaDataItemName2);
            _metaDataItem2.Setup(x => x.Value).Returns(MetaDataItemValue2);
            List<IMetaDataItem> metaDataItems = new List<IMetaDataItem>() { _metaDataItem1.Object, _metaDataItem2.Object };
            _dsiModel.Setup(x => x.GetMetaDataGroupItems(MetaDataGroup)).Returns(metaDataItems);

            _dsiElementAbc = new Mock<IDsiElement>();
            _dsiElementAbc.Setup(x => x.Id).Returns(DsiElementIdAbc);
            _dsiElementAbc.Setup(x => x.Name).Returns(DsiElementNameAbc);
            _dsiElementAbc.Setup(x => x.Type).Returns(DsiElementTypeAbc);
            _dsiElementAbd = new Mock<IDsiElement>();
            _dsiElementAbd.Setup(x => x.Id).Returns(DsiElementIdAbd);
            _dsiElementAbd.Setup(x => x.Name).Returns(DsiElementNameAbd);
            _dsiElementAbd.Setup(x => x.Type).Returns(DsiElementTypeAbd);
            _dsiElementE = new Mock<IDsiElement>();
            _dsiElementE.Setup(x => x.Id).Returns(DsiElementIdE);
            _dsiElementE.Setup(x => x.Name).Returns(DsiElementNameE);
            _dsiElementE.Setup(x => x.Type).Returns(DsiElementTypeE);
            List<IDsiElement> dsiElements = new List<IDsiElement>() { _dsiElementAbc.Object, _dsiElementAbd.Object, _dsiElementE.Object };
            _dsiModel.Setup(x => x.GetElements()).Returns(dsiElements);

            _dsiRelationAbCtoAbd = new Mock<IDsiRelation>();
            _dsiRelationAbCtoAbd.Setup(x => x.ConsumerId).Returns(DsiElementIdAbc);
            _dsiRelationAbCtoAbd.Setup(x => x.ProviderId).Returns(DsiElementIdAbd);
            _dsiRelationAbCtoAbd.Setup(x => x.Type).Returns(DsiRelationTypeAbCtoAbd);
            _dsiRelationAbCtoAbd.Setup(x => x.Weight).Returns(DsiRelationWeightAbCtoAbd);
            _dsiRelationAbDtoE = new Mock<IDsiRelation>();
            _dsiRelationAbDtoE.Setup(x => x.ConsumerId).Returns(DsiElementIdAbd);
            _dsiRelationAbDtoE.Setup(x => x.ProviderId).Returns(DsiElementIdE);
            _dsiRelationAbDtoE.Setup(x => x.Type).Returns(DsiRelationTypeAbDtoE);
            _dsiRelationAbDtoE.Setup(x => x.Weight).Returns(DsiRelationWeightAbDtoE);
            _dsiRelationAbCtoE = new Mock<IDsiRelation>();
            _dsiRelationAbCtoE.Setup(x => x.ConsumerId).Returns(DsiElementIdAbc);
            _dsiRelationAbCtoE.Setup(x => x.ProviderId).Returns(DsiElementIdE);
            _dsiRelationAbCtoE.Setup(x => x.Type).Returns(DsiRelationTypeAbCtoE);
            _dsiRelationAbCtoE.Setup(x => x.Weight).Returns(DsiRelationWeightAbCtoE);
            List<IDsiRelation> dsiRelations = new List<IDsiRelation>() { _dsiRelationAbCtoAbd.Object, _dsiRelationAbDtoE.Object, _dsiRelationAbCtoE.Object };
            _dsiModel.Setup(x => x.GetRelations()).Returns(dsiRelations);

            _dsmElementA = new Mock<IDsmElement>();
            _dsmElementA.Setup(x => x.Id).Returns(DsmElementIdA);
            _dsmElementA.Setup(x => x.Name).Returns(DsmElementNameA);
            _dsmElementA.Setup(x => x.Type).Returns(string.Empty);
            _dsmElementB = new Mock<IDsmElement>();
            _dsmElementB.Setup(x => x.Id).Returns(DsmElementIdB);
            _dsmElementB.Setup(x => x.Name).Returns(DsmElementNameB);
            _dsmElementB.Setup(x => x.Type).Returns(string.Empty);
            _dsmElementC = new Mock<IDsmElement>();
            _dsmElementC.Setup(x => x.Id).Returns(DsmElementIdC);
            _dsmElementC.Setup(x => x.Name).Returns(DsmElementNameC);
            _dsmElementC.Setup(x => x.Type).Returns(DsiElementTypeAbc);
            _dsmElementD = new Mock<IDsmElement>();
            _dsmElementD.Setup(x => x.Id).Returns(DsmElementIdD);
            _dsmElementD.Setup(x => x.Name).Returns(DsmElementNameD);
            _dsmElementD.Setup(x => x.Type).Returns(DsiElementTypeAbd);
            _dsmElementE = new Mock<IDsmElement>();
            _dsmElementE.Setup(x => x.Id).Returns(DsmElementIdE);
            _dsmElementE.Setup(x => x.Name).Returns(DsmElementNameE);
            _dsmElementE.Setup(x => x.Type).Returns(DsiElementTypeE);
        }
    }
}
