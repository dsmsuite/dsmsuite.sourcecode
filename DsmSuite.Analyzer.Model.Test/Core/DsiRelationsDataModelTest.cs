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
        DsiElementDataModel _elementsDataModel;
        IDsiElement _a;
        IDsiElement _b;
        IDsiElement _c;

        [TestInitialize]
        public void TestInitialize()
        {
            _elementsDataModel = new DsiElementDataModel();
            _a = _elementsDataModel.AddElement("a", "", "");
            _b = _elementsDataModel.AddElement("b", "", "");
            _c = _elementsDataModel.AddElement("c", "", "");
        }

        [TestMethod]
        public void WhenModelIsConstructedThenItIsEmpty()
        {
            DsiRelationDataModel model = new DsiRelationDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);
        }

        [TestMethod]
        public void GivenModelIsNotEmptyWhenClearIsCalledThenItIsEmpty()
        {
            DsiRelationDataModel model = new DsiRelationDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);

            model.AddRelation(_a.Name, _b.Name, "type", 2, "context");
            Assert.AreEqual(1, model.TotalRelationCount);

            model.Clear();

            Assert.AreEqual(0, model.TotalRelationCount);
        }

        [TestMethod]
        public void GivenModelIsEmptyWhenAddRelationIsCalledThenItsHasOneRelation()
        {
            DsiRelationDataModel model = new DsiRelationDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);

            IDsiRelation relation = model.AddRelation(_a.Name, _b.Name, "type", 2, "context");
            Assert.IsNotNull(relation);
            Assert.AreEqual(1, model.TotalRelationCount);
        }

        [TestMethod]
        public void GivenModelIsEmptyWhenAddRelationIsCalledThenTheRelationExists()
        {
            DsiRelationDataModel model = new DsiRelationDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);

            IDsiRelation relation = model.AddRelation(_a.Name, _b.Name, "type", 2, "context");
            Assert.IsNotNull(relation);

            Assert.IsTrue(model.DoesRelationExist(relation.ConsumerId, relation.ProviderId));
        }

        [TestMethod]
        public void GivenAnRelationIsInTheModelWhenAddRelationIsCalledAgainForThatRelationThenItsHasOneRelationButWeightHasIncreased()
        {
            DsiRelationDataModel model = new DsiRelationDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);

            IDsiRelation relation1 = model.AddRelation(_a.Name, _b.Name, "type", 2, "context");
            Assert.IsNotNull(relation1);

            Assert.AreEqual(1, model.TotalRelationCount);

            List<IDsiRelation> relationsBefore = model.GetRelationsOfConsumer(_a.Id).ToList();
            Assert.AreEqual(1, relationsBefore.Count);
            Assert.AreEqual(_a.Id, relationsBefore[0].ConsumerId);
            Assert.AreEqual(_b.Id, relationsBefore[0].ProviderId);
            Assert.AreEqual("type", relationsBefore[0].Type);
            Assert.AreEqual(2, relationsBefore[0].Weight);

            IDsiRelation relation2 = model.AddRelation(_a.Name, _b.Name, "type", 3, "context");
            Assert.IsNotNull(relation2);

            Assert.AreEqual(1, model.TotalRelationCount);

            List<IDsiRelation> relationsAfter = model.GetRelationsOfConsumer(_a.Id).ToList();
            Assert.AreEqual(1, relationsAfter.Count);
            Assert.AreEqual(_a.Id, relationsAfter[0].ConsumerId);
            Assert.AreEqual(_b.Id, relationsAfter[0].ProviderId);
            Assert.AreEqual("type", relationsAfter[0].Type);
            Assert.AreEqual(5, relationsAfter[0].Weight);
        }

        [TestMethod]
        public void GivenAnRelationIsInTheModelWhenAddRelationtIsCalledWithAnotherProviderThenItHasTwoRelations()
        {
            DsiRelationDataModel model = new DsiRelationDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);

            IDsiRelation relation1 = model.AddRelation(_a.Name, _b.Name, "type", 2, "context");
            Assert.IsNotNull(relation1);

            Assert.AreEqual(1, model.TotalRelationCount);

            IDsiRelation relation2 = model.AddRelation(_a.Name, _c.Name, "type", 3, "context");
            Assert.IsNotNull(relation2);

            Assert.AreEqual(2, model.TotalRelationCount);
        }

        [TestMethod]
        public void GivenAnRelationIsInTheModelWhenAddRelationtIsCalledWithAnotherTypeThenItHasTwoRelations()
        {
            DsiRelationDataModel model = new DsiRelationDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);

            IDsiRelation relation1 = model.AddRelation(_a.Name, _b.Name, "type1", 2, "context");
            Assert.IsNotNull(relation1);

            Assert.AreEqual(1, model.TotalRelationCount);

            IDsiRelation relation2 = model.AddRelation(_a.Name, _b.Name, "type2", 3, "context");
            Assert.IsNotNull(relation2);

            Assert.AreEqual(2, model.TotalRelationCount);
        }

        [TestMethod]
        public void GivenModelIsEmptyWhenImportRelationIsCalledThenItsHasOneRelation()
        {
            DsiRelationDataModel model = new DsiRelationDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);

            IDsiRelation relation = model.ImportRelation(_a.Id, _b.Id, "type", 2);
            Assert.IsNotNull(relation);

            Assert.AreEqual(1, model.TotalRelationCount);
        }

        [TestMethod]
        public void GivenAnRelationIsInTheModelWhenDoesRelationExistIsCalledThenTrueIsReturned()
        {
            DsiRelationDataModel model = new DsiRelationDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);

            IDsiRelation relation = model.ImportRelation(_a.Id, _b.Id, "type", 2);
            Assert.IsNotNull(relation);

            Assert.AreEqual(1, model.TotalRelationCount);

            Assert.IsTrue(model.DoesRelationExist(_a.Id, _b.Id));
        }

        [TestMethod]
        public void GivenAnRelationIsNotInTheModelWhenDoesRelationExistIsCalledThenFalseIsReturned()
        {
            DsiRelationDataModel model = new DsiRelationDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);

            IDsiRelation relation = model.ImportRelation(_a.Id, _b.Id, "type", 2);
            Assert.IsNotNull(relation);
            Assert.AreEqual(1, model.TotalRelationCount);

            Assert.IsFalse(model.DoesRelationExist(_a.Id, _c.Id));
        }

        [TestMethod]
        public void WhenAddRelationIsCalledUsingTwoDifferentTypesThenTwoRelationTypesAreFound()
        {
            DsiRelationDataModel model = new DsiRelationDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);

            IDsiRelation relation1 = model.ImportRelation(_a.Id, _b.Id, "type1", 2);
            Assert.IsNotNull(relation1);

            Assert.AreEqual(1, model.TotalRelationCount);

            IDsiRelation relation2 = model.ImportRelation(_a.Id, _b.Id, "type2", 2);
            Assert.IsNotNull(relation2);

            Assert.AreEqual(2, model.TotalRelationCount);

            List<string> relationTypes = model.GetRelationTypes().ToList();
            Assert.AreEqual(2, relationTypes.Count);
            Assert.AreEqual("type1", relationTypes[0]);
            Assert.AreEqual("type2", relationTypes[1]);

            Assert.AreEqual(1, model.GetRelationTypeCount("type1"));
            Assert.AreEqual(1, model.GetRelationTypeCount("type2"));
        }

        [TestMethod]
        public void GivenMultipleElementAreInTheModelWhenGetElementsIsCalledTheyAreAllReturned()
        {
            DsiRelationDataModel model = new DsiRelationDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);

            IDsiRelation relation1 = model.ImportRelation(_a.Id, _b.Id, "type1", 1);
            Assert.IsNotNull(relation1);
            IDsiRelation relation2 = model.ImportRelation(_b.Id, _c.Id, "type2", 2);
            Assert.IsNotNull(relation2);
            IDsiRelation relation3 = model.ImportRelation(_a.Id, _c.Id, "type3", 3);
            Assert.IsNotNull(relation3);

            List<IDsiRelation> relations = model.GetRelations().OrderBy(x => x.Weight).ToList();
            Assert.AreEqual(3, relations.Count);

            Assert.AreEqual(_a.Id, relations[0].ConsumerId);
            Assert.AreEqual(_b.Id, relations[0].ProviderId);
            Assert.AreEqual("type1", relations[0].Type);
            Assert.AreEqual(1, relations[0].Weight);

            Assert.AreEqual(_b.Id, relations[1].ConsumerId);
            Assert.AreEqual(_c.Id, relations[1].ProviderId);
            Assert.AreEqual("type2", relations[1].Type);
            Assert.AreEqual(2, relations[1].Weight);

            Assert.AreEqual(_a.Id, relations[2].ConsumerId);
            Assert.AreEqual(_c.Id, relations[2].ProviderId);
            Assert.AreEqual("type3", relations[2].Type);
            Assert.AreEqual(3, relations[2].Weight);
        }

        [TestMethod]
        public void GivenModelIsEmptyWhenGetRelationsOfConsumerIsCalledThenItsHasReturnsOneRelation()
        {
            DsiRelationDataModel model = new DsiRelationDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);

            IDsiRelation relation = model.AddRelation(_a.Name, _b.Name, "type", 2, "context");
            Assert.IsNotNull(relation);

            Assert.AreEqual(1, model.TotalRelationCount);

            List<IDsiRelation> relations = model.GetRelationsOfConsumer(_a.Id).ToList();
            Assert.AreEqual(1, relations.Count);
            Assert.AreEqual(_a.Id, relations[0].ConsumerId);
            Assert.AreEqual(_b.Id, relations[0].ProviderId);
            Assert.AreEqual("type", relations[0].Type);
            Assert.AreEqual(2, relations[0].Weight);
        }

        [TestMethod]
        public void GivenMultipleElementAreInTheModelWhenAddRelationIsCalled4Times1TimeWithNotExistingConsumerThenResolvedPercentageis75Percent()
        {
            DsiRelationDataModel model = new DsiRelationDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);

            IDsiRelation relation1 = model.AddRelation("a", "b", "type1", 1, "");
            Assert.IsNotNull(relation1);
            IDsiRelation relation2 = model.AddRelation("b", "c", "type2", 2, "");
            Assert.IsNotNull(relation2);
            IDsiRelation relation3 = model.AddRelation("a", "c", "type3", 3, "");
            Assert.IsNotNull(relation3);
            IDsiRelation relation4 = model.AddRelation("d", "c", "type4", 4, "");
            Assert.IsNull(relation4);

            Assert.AreEqual(4, model.TotalRelationCount);
            Assert.AreEqual(3, model.ResolvedRelationCount);
            Assert.AreEqual(75.0, model.ResolvedRelationPercentage);
        }

        [TestMethod]
        public void GivenMultipleElementAreInTheModelWhenAddRelationIsCalled4Times1TimeWithNotExistingProviderThenResolvedPercentageis75Percent()
        {
            DsiRelationDataModel model = new DsiRelationDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);

            IDsiRelation relation1 = model.AddRelation("a", "b", "type1", 1, "");
            Assert.IsNotNull(relation1);
            IDsiRelation relation2 = model.AddRelation("b", "c", "type2", 2, "");
            Assert.IsNotNull(relation2);
            IDsiRelation relation3 = model.AddRelation("a", "c", "type3", 3, "");
            Assert.IsNotNull(relation3);
            IDsiRelation relation4 = model.AddRelation("c", "d", "type4", 4, "");
            Assert.IsNull(relation4);

            Assert.AreEqual(4, model.TotalRelationCount);
            Assert.AreEqual(3, model.ResolvedRelationCount);
            Assert.AreEqual(75.0, model.ResolvedRelationPercentage);
        }

        [TestMethod]
        public void GivenMultipleElementAreInTheModelWhenAddRelationIsCalled3TimesAndSkipRelation1TimeThenResolvedPercentageis75Percent()
        {
            DsiRelationDataModel model = new DsiRelationDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);

            IDsiRelation relation1 = model.AddRelation("a", "b", "type1", 1, "");
            Assert.IsNotNull(relation1);
            IDsiRelation relation2 = model.AddRelation("b", "c", "type2", 2, "");
            Assert.IsNotNull(relation2);
            IDsiRelation relation3 = model.AddRelation("a", "c", "type3", 3, "");
            Assert.IsNotNull(relation3);
            model.SkipRelation("d", "c", "type4", "");

            Assert.AreEqual(4, model.TotalRelationCount);
            Assert.AreEqual(3, model.ResolvedRelationCount);
            Assert.AreEqual(75.0, model.ResolvedRelationPercentage);
        }

        [TestMethod]
        public void GivenMultipleElementAreInTheModelWhenAnElementIsRemovedThenAllRelationUsingThisElementAreRemoved()
        {
            DsiRelationDataModel model = new DsiRelationDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.TotalRelationCount);

            IDsiRelation relation1 = model.AddRelation("a", "b", "type1", 1, "");
            Assert.IsNotNull(relation1);
            IDsiRelation relation2 = model.AddRelation("b", "c", "type2", 2, "");
            Assert.IsNotNull(relation2);
            IDsiRelation relation3 = model.AddRelation("a", "c", "type3", 3, "");
            Assert.IsNotNull(relation3);
            Assert.AreEqual(3, model.TotalRelationCount);

            _elementsDataModel.RemoveElement(_b);
            Assert.AreEqual(1, model.TotalRelationCount);

            List<IDsiRelation> relations = model.GetRelations().OrderBy(x => x.Weight).ToList();
            Assert.AreEqual(1, relations.Count);

            Assert.AreEqual(_a.Id, relations[0].ConsumerId);
            Assert.AreEqual(_c.Id, relations[0].ProviderId);
            Assert.AreEqual("type3", relations[0].Type);
            Assert.AreEqual(3, relations[0].Weight);
        }
    }
}