using System.Linq;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DsmSuite.Analyzer.Model.Test.Core
{
    [TestClass]
    public class DsiRelationsDataModelTest
    {
        DsiElementsDataModel _relationsDataModel;
        IDsiElement _a;
        IDsiElement _b;
        IDsiElement _c;

        [TestInitialize]
        public void TestInitialize()
        {
            _relationsDataModel = new DsiElementsDataModel();
            _a = _relationsDataModel.AddElement("a", "", "");
            _b = _relationsDataModel.AddElement("b", "", "");
            _c = _relationsDataModel.AddElement("c", "", "");
        }

        [TestMethod]
        public void When_ModelIsConstructed_Then_ItIsEmpty()
        {
            DsiRelationsDataModel model = new DsiRelationsDataModel(_relationsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);
            Assert.AreEqual(0, model.ResolvedRelationCount);
        }

        [TestMethod]
        public void Given_ModelIsNotEmpty_When_ClearIsCalled_Then_ItIsEmpty()
        {
            DsiRelationsDataModel model = new DsiRelationsDataModel(_relationsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);
            Assert.AreEqual(0, model.ResolvedRelationCount);

            model.AddRelation(_a.Name, _b.Name, "type", 2, "context");
            Assert.AreEqual(1, model.TotalRelationCount);
            Assert.AreEqual(1, model.ResolvedRelationCount);

            model.Clear();

            Assert.AreEqual(0, model.TotalRelationCount);
            Assert.AreEqual(0, model.ResolvedRelationCount);
        }

        [TestMethod]
        public void Given_ModelIsEmpty_When_AddRelationIsCalled_Then_ItsHasOneRelation()
        {
            DsiRelationsDataModel model = new DsiRelationsDataModel(_relationsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);
            Assert.AreEqual(0, model.ResolvedRelationCount);

            model.AddRelation(_a.Name, _b.Name, "type", 2, "context");
            Assert.AreEqual(1, model.TotalRelationCount);
            Assert.AreEqual(1, model.ResolvedRelationCount);

            List<IDsiRelation> relations = model.GetRelationsOfConsumer(_a.Id).ToList();
            Assert.AreEqual(1, relations.Count);
            Assert.AreEqual(_a.Id, relations[0].ConsumerId);
            Assert.AreEqual(_b.Id, relations[0].ProviderId);
            Assert.AreEqual("type", relations[0].Type);
            Assert.AreEqual(2, relations[0].Weight);
        }

        [TestMethod]
        public void Given_AnRelationIsInTheModel_When_AddRelationIsCalledAgainForThatRelation_Then_ItsHasOneRelationButWeightHasIncreased()
        {
            DsiRelationsDataModel model = new DsiRelationsDataModel(_relationsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);
            Assert.AreEqual(0, model.ResolvedRelationCount);

            model.AddRelation(_a.Name, _b.Name, "type", 2, "context");
            Assert.AreEqual(1, model.TotalRelationCount);
            Assert.AreEqual(1, model.ResolvedRelationCount);

            List<IDsiRelation> relationsBefore = model.GetRelationsOfConsumer(_a.Id).ToList();
            Assert.AreEqual(1, relationsBefore.Count);
            Assert.AreEqual(_a.Id, relationsBefore[0].ConsumerId);
            Assert.AreEqual(_b.Id, relationsBefore[0].ProviderId);
            Assert.AreEqual("type", relationsBefore[0].Type);
            Assert.AreEqual(2, relationsBefore[0].Weight);

            model.AddRelation(_a.Name, _b.Name, "type", 3, "context");
            Assert.AreEqual(1, model.TotalRelationCount);
            Assert.AreEqual(1, model.ResolvedRelationCount);

            List<IDsiRelation> relationsAfter = model.GetRelationsOfConsumer(_a.Id).ToList();
            Assert.AreEqual(1, relationsAfter.Count);
            Assert.AreEqual(_a.Id, relationsAfter[0].ConsumerId);
            Assert.AreEqual(_b.Id, relationsAfter[0].ProviderId);
            Assert.AreEqual("type", relationsAfter[0].Type);
            Assert.AreEqual(5, relationsAfter[0].Weight);
        }

        [TestMethod]
        public void Given_AnRelationIsInTheModel_When_AddRelationtIsCalledWithAnotherProvider_Then_ItHasTwoRelations()
        {
            DsiRelationsDataModel model = new DsiRelationsDataModel(_relationsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);
            Assert.AreEqual(0, model.ResolvedRelationCount);

            model.AddRelation(_a.Name, _b.Name, "type", 2, "context");
            Assert.AreEqual(1, model.TotalRelationCount);
            Assert.AreEqual(1, model.ResolvedRelationCount);

            List<IDsiRelation> relationsBefore = model.GetRelationsOfConsumer(_a.Id).ToList();
            Assert.AreEqual(1, relationsBefore.Count);
            Assert.AreEqual(_a.Id, relationsBefore[0].ConsumerId);
            Assert.AreEqual(_b.Id, relationsBefore[0].ProviderId);
            Assert.AreEqual("type", relationsBefore[0].Type);
            Assert.AreEqual(2, relationsBefore[0].Weight);

            model.AddRelation(_a.Name, _c.Name, "type", 3, "context");
            Assert.AreEqual(2, model.TotalRelationCount);
            Assert.AreEqual(2, model.ResolvedRelationCount);

            List<IDsiRelation> relationsAfter = model.GetRelationsOfConsumer(_a.Id).ToList();
            Assert.AreEqual(2, relationsAfter.Count);

            Assert.AreEqual(_a.Id, relationsAfter[0].ConsumerId);
            Assert.AreEqual(_b.Id, relationsAfter[0].ProviderId);
            Assert.AreEqual("type", relationsAfter[0].Type);
            Assert.AreEqual(2, relationsAfter[0].Weight);

            Assert.AreEqual(_a.Id, relationsAfter[1].ConsumerId);
            Assert.AreEqual(_c.Id, relationsAfter[1].ProviderId);
            Assert.AreEqual("type", relationsAfter[1].Type);
            Assert.AreEqual(3, relationsAfter[1].Weight);
        }

        [TestMethod]
        public void Given_AnRelationIsInTheModel_When_AddRelationtIsCalledWithAnotherType_Then_ItHasTwoRelations()
        {
            DsiRelationsDataModel model = new DsiRelationsDataModel(_relationsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);
            Assert.AreEqual(0, model.ResolvedRelationCount);

            model.AddRelation(_a.Name, _b.Name, "type1", 2, "context");
            Assert.AreEqual(1, model.TotalRelationCount);
            Assert.AreEqual(1, model.ResolvedRelationCount);

            List<IDsiRelation> relationsBefore = model.GetRelationsOfConsumer(_a.Id).ToList();
            Assert.AreEqual(1, relationsBefore.Count);
            Assert.AreEqual(_a.Id, relationsBefore[0].ConsumerId);
            Assert.AreEqual(_b.Id, relationsBefore[0].ProviderId);
            Assert.AreEqual("type1", relationsBefore[0].Type);
            Assert.AreEqual(2, relationsBefore[0].Weight);

            model.AddRelation(_a.Name, _b.Name, "type2", 3, "context");
            Assert.AreEqual(2, model.TotalRelationCount);
            Assert.AreEqual(2, model.ResolvedRelationCount);

            List<IDsiRelation> relationsAfter = model.GetRelationsOfConsumer(_a.Id).ToList();
            Assert.AreEqual(2, relationsAfter.Count);

            Assert.AreEqual(_a.Id, relationsAfter[0].ConsumerId);
            Assert.AreEqual(_b.Id, relationsAfter[0].ProviderId);
            Assert.AreEqual("type1", relationsAfter[0].Type);
            Assert.AreEqual(2, relationsAfter[0].Weight);

            Assert.AreEqual(_a.Id, relationsAfter[1].ConsumerId);
            Assert.AreEqual(_b.Id, relationsAfter[1].ProviderId);
            Assert.AreEqual("type2", relationsAfter[1].Type);
            Assert.AreEqual(3, relationsAfter[1].Weight);
        }

        [TestMethod]
        public void Given_ModelIsEmpty_When_ImportRelationIsCalled_Then_ItsHasOneRelation()
        {
            DsiRelationsDataModel model = new DsiRelationsDataModel(_relationsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);
            Assert.AreEqual(0, model.ResolvedRelationCount);

            model.ImportRelation(_a.Id, _b.Id, "type", 2);
            Assert.AreEqual(1, model.TotalRelationCount);
            Assert.AreEqual(1, model.ResolvedRelationCount);

            List<IDsiRelation> relationsAfter = model.GetRelationsOfConsumer(_a.Id).ToList();
            Assert.AreEqual(1, relationsAfter.Count);
            Assert.AreEqual(_a.Id, relationsAfter[0].ConsumerId);
            Assert.AreEqual(_b.Id, relationsAfter[0].ProviderId);
            Assert.AreEqual("type", relationsAfter[0].Type);
            Assert.AreEqual(2, relationsAfter[0].Weight);
        }

        [TestMethod]
        public void Given_AnRelationIsInTheModel_When_DoesRelationExistIsCalled_Then_TrueIsReturned()
        {
            DsiRelationsDataModel model = new DsiRelationsDataModel(_relationsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);
            Assert.AreEqual(0, model.ResolvedRelationCount);

            model.ImportRelation(_a.Id, _b.Id, "type", 2);
            Assert.AreEqual(1, model.TotalRelationCount);
            Assert.AreEqual(1, model.ResolvedRelationCount);

            Assert.IsTrue(model.DoesRelationExist(_a.Id, _b.Id));
        }

        [TestMethod]
        public void Given_AnRelationIsNotInTheModel_When_DoesRelationExistIsCalled_Then_FalseIsReturned()
        {
            DsiRelationsDataModel model = new DsiRelationsDataModel(_relationsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);
            Assert.AreEqual(0, model.ResolvedRelationCount);

            model.ImportRelation(_a.Id, _b.Id, "type", 2);
            Assert.AreEqual(1, model.TotalRelationCount);
            Assert.AreEqual(1, model.ResolvedRelationCount);

            Assert.IsFalse(model.DoesRelationExist(_a.Id, _c.Id));
        }
    }
}