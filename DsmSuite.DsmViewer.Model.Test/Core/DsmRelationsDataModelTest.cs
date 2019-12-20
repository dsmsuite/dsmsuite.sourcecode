using System.Linq;
using DsmSuite.DsmViewer.Model.Core;
using DsmSuite.DsmViewer.Model.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Model.Test.Core
{
    /// <summary>
    /// Dependency matrix used for tests:
    /// -System cycle between a and b
    /// -Hierarchical cycle between a and c
    /// 
    ///        | a           | b           | c           |
    ///        +------+------+------+------+------+------+
    ///        | a1   | a2   | b1   | b2   | c1   | c2   |
    /// --+----+------+------+------+------+------+------+
    ///   | a1 |      |      |      | 2    |      |      |
    /// a +----+------+------+------+------+------+------+
    ///   | a2 |  1   |      |      | 3    |  4   |      |
    /// -------+------+------+------+------+------+------+
    ///   | b1 | 1000 | 200  |      |      |      |      |
    /// b +----+------+------+------+------+------+------+
    ///   | b2 |  30  | 4    |      |      |      |      |
    /// --+----+------+------+------+------+------+------+
    ///   | c1 |      |      |      |      |      |      |
    /// c +----+------+------+------+------+------+------+
    ///   | c2 |  5   |      |      |      |      |      |
    /// --+----+------+------+------+------+------+------+
    /// </summary>
    /// 
    [TestClass]
    public class DsmRelationssDataModelTest
    {
        DsmElementsDataModel _elementsDataModel;
        private IDsmElement _a;
        private IDsmElement _a1;
        private IDsmElement _a2;
        private IDsmElement _b;
        private IDsmElement _b1;
        private IDsmElement _b2;
        private IDsmElement _c;
        private IDsmElement _c1;
        private IDsmElement _c2;

        [TestInitialize]
        public void TestInitialize()
        {
            _elementsDataModel = new DsmElementsDataModel();

            CreateElementHierarchy();
        }

        [TestMethod]
        public void WhenModelIsConstructedThenItIsEmpty()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.GetRelationCount());
        }
        
        [TestMethod]
        public void GivenModelIsNotEmptyWhenClearIsCalledThenItIsEmpty()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.GetRelationCount());

            model.AddRelation(_a.Id, _b.Id, "type", 1);
            Assert.AreEqual(1, model.GetRelationCount());

            model.Clear();

            Assert.AreEqual(0, model.GetRelationCount());
        }

        [TestMethod]
        public void GivenModelIsEmptyWhenAddRelationIsCalledThenItsHasOneRelation()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.GetRelationCount());

            IDsmRelation relation = model.AddRelation(_a.Id, _b.Id, "type", 1);
            Assert.IsNotNull(relation);

            Assert.AreEqual(1, model.GetRelationCount());
        }

        [TestMethod]
        public void GivenModelIsEmptyWhenImportRelationIsCalledThenItsHasOneRelation()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.GetRelationCount());

            IDsmRelation relation = model.ImportRelation(1, _a.Id, _b.Id, "type", 1);
            Assert.IsNotNull(relation);

            Assert.AreEqual(1, model.GetRelationCount());
        }

        [TestMethod]
        public void GivenModelIsFilledWhenGetWeightWithSelfThenReturnsZero()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            CreateElementRelations(model);
            Assert.AreEqual(9, model.GetRelationCount());

            foreach(IDsmElement element in _elementsDataModel.GetElements())
            {
                Assert.AreEqual(0, model.GetDependencyWeight(element.Id, element.Id));
            }
        }

        [TestMethod]
        public void GivenModelIsFilledWhenGetWeightWithOtherThenReturnsCalculatedDerivedWeights()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            CreateElementRelations(model);

            Assert.AreEqual(1000, model.GetDependencyWeight(_a1.Id, _b1.Id));
            Assert.AreEqual(200, model.GetDependencyWeight(_a2.Id, _b1.Id));
            Assert.AreEqual(30, model.GetDependencyWeight(_a1.Id, _b2.Id));
            Assert.AreEqual(4, model.GetDependencyWeight(_a2.Id, _b2.Id));

            Assert.AreEqual(1030, model.GetDependencyWeight(_a1.Id, _b.Id));
            Assert.AreEqual(204, model.GetDependencyWeight(_a2.Id, _b.Id));
            Assert.AreEqual(1200, model.GetDependencyWeight(_a.Id, _b1.Id));
            Assert.AreEqual(34, model.GetDependencyWeight(_a.Id, _b2.Id));

            Assert.AreEqual(1234, model.GetDependencyWeight(_a.Id, _b.Id));
        }

        [TestMethod]
        public void GivenModelIsFilledWhenEditingRelationWeightThenUpdatesCalculatedDerivedWeights()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            CreateElementRelations(model);
            
            Assert.AreEqual(1000, model.GetDependencyWeight(_a1.Id, _b1.Id));
            Assert.AreEqual(200, model.GetDependencyWeight(_a2.Id, _b1.Id));
            Assert.AreEqual(30, model.GetDependencyWeight(_a1.Id, _b2.Id));
            Assert.AreEqual(4, model.GetDependencyWeight(_a2.Id, _b2.Id));

            Assert.AreEqual(1030, model.GetDependencyWeight(_a1.Id, _b.Id));
            Assert.AreEqual(204, model.GetDependencyWeight(_a2.Id, _b.Id));
            Assert.AreEqual(1200, model.GetDependencyWeight(_a.Id, _b1.Id));
            Assert.AreEqual(34, model.GetDependencyWeight(_a.Id, _b2.Id));

            Assert.AreEqual(1234, model.GetDependencyWeight(_a.Id, _b.Id));

            IDsmRelation relation = model.FindRelations(_a2, _b2).FirstOrDefault();
            Assert.IsNotNull(relation);

            model.EditRelation(relation, relation.Type, 5);

            Assert.AreEqual(1000, model.GetDependencyWeight(_a1.Id, _b1.Id));
            Assert.AreEqual(200, model.GetDependencyWeight(_a2.Id, _b1.Id));
            Assert.AreEqual(30, model.GetDependencyWeight(_a1.Id, _b2.Id));
            Assert.AreEqual(5, model.GetDependencyWeight(_a2.Id, _b2.Id));

            Assert.AreEqual(1030, model.GetDependencyWeight(_a1.Id, _b.Id));
            Assert.AreEqual(205, model.GetDependencyWeight(_a2.Id, _b.Id));
            Assert.AreEqual(1200, model.GetDependencyWeight(_a.Id, _b1.Id));
            Assert.AreEqual(35, model.GetDependencyWeight(_a.Id, _b2.Id));

            Assert.AreEqual(1235, model.GetDependencyWeight(_a.Id, _b.Id));
        }

        [TestMethod]
        public void GivenModelIsFilledWhenRemovingRelationThenReducesRelationCount()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            CreateElementRelations(model);
            int relationCountBefore = model.GetRelationCount();

            IDsmRelation relation = model.FindRelations(_a2, _b2).FirstOrDefault();
            Assert.IsNotNull(relation);

            model.RemoveRelation(relation.Id);
            Assert.AreEqual(relationCountBefore - 1, model.GetRelationCount());
        }

        [TestMethod]
        public void GivenModelIsFilledWhenUnremovingRelationThenRestoresRelationCount()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            CreateElementRelations(model);

            IDsmRelation relation = model.FindRelations(_a2, _b2).FirstOrDefault();
            Assert.IsNotNull(relation);

            model.RemoveRelation(relation.Id);
            int relationCountBefore = model.GetRelationCount();

            model.UnremoveRelation(relation.Id);
            Assert.AreEqual(relationCountBefore + 1, model.GetRelationCount());
        }

        [TestMethod]
        public void GivenModelIsFilledWhenRemovingRelationThenReducesCalculatedDerivedWeights()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            CreateElementRelations(model);

            Assert.AreEqual(1000, model.GetDependencyWeight(_a1.Id, _b1.Id));
            Assert.AreEqual(200, model.GetDependencyWeight(_a2.Id, _b1.Id));
            Assert.AreEqual(30, model.GetDependencyWeight(_a1.Id, _b2.Id));
            Assert.AreEqual(4, model.GetDependencyWeight(_a2.Id, _b2.Id));

            Assert.AreEqual(1030, model.GetDependencyWeight(_a1.Id, _b.Id));
            Assert.AreEqual(204, model.GetDependencyWeight(_a2.Id, _b.Id));
            Assert.AreEqual(1200, model.GetDependencyWeight(_a.Id, _b1.Id));
            Assert.AreEqual(34, model.GetDependencyWeight(_a.Id, _b2.Id));

            Assert.AreEqual(1234, model.GetDependencyWeight(_a.Id, _b.Id));

            IDsmRelation relation = model.FindRelations(_a2, _b2).FirstOrDefault();
            Assert.IsNotNull(relation);

            model.RemoveRelation(relation.Id);

            Assert.AreEqual(1000, model.GetDependencyWeight(_a1.Id, _b1.Id));
            Assert.AreEqual(200, model.GetDependencyWeight(_a2.Id, _b1.Id));
            Assert.AreEqual(30, model.GetDependencyWeight(_a1.Id, _b2.Id));
            Assert.AreEqual(0, model.GetDependencyWeight(_a2.Id, _b2.Id));

            Assert.AreEqual(1030, model.GetDependencyWeight(_a1.Id, _b.Id));
            Assert.AreEqual(200, model.GetDependencyWeight(_a2.Id, _b.Id));
            Assert.AreEqual(1200, model.GetDependencyWeight(_a.Id, _b1.Id));
            Assert.AreEqual(30, model.GetDependencyWeight(_a.Id, _b2.Id));

            Assert.AreEqual(1230, model.GetDependencyWeight(_a.Id, _b.Id));
        }

        [TestMethod]
        public void GivenModelIsFilledWhenUnremovingRelationThenRestoresCalculatedDerivedWeights()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            CreateElementRelations(model);

            Assert.AreEqual(1000, model.GetDependencyWeight(_a1.Id, _b1.Id));
            Assert.AreEqual(200, model.GetDependencyWeight(_a2.Id, _b1.Id));
            Assert.AreEqual(30, model.GetDependencyWeight(_a1.Id, _b2.Id));
            Assert.AreEqual(4, model.GetDependencyWeight(_a2.Id, _b2.Id));

            Assert.AreEqual(1030, model.GetDependencyWeight(_a1.Id, _b.Id));
            Assert.AreEqual(204, model.GetDependencyWeight(_a2.Id, _b.Id));
            Assert.AreEqual(1200, model.GetDependencyWeight(_a.Id, _b1.Id));
            Assert.AreEqual(34, model.GetDependencyWeight(_a.Id, _b2.Id));

            Assert.AreEqual(1234, model.GetDependencyWeight(_a.Id, _b.Id));

            IDsmRelation relation = model.FindRelations(_a2, _b2).FirstOrDefault();
            Assert.IsNotNull(relation);

            model.RemoveRelation(relation.Id);

            Assert.AreEqual(1000, model.GetDependencyWeight(_a1.Id, _b1.Id));
            Assert.AreEqual(200, model.GetDependencyWeight(_a2.Id, _b1.Id));
            Assert.AreEqual(30, model.GetDependencyWeight(_a1.Id, _b2.Id));
            Assert.AreEqual(0, model.GetDependencyWeight(_a2.Id, _b2.Id));

            Assert.AreEqual(1030, model.GetDependencyWeight(_a1.Id, _b.Id));
            Assert.AreEqual(200, model.GetDependencyWeight(_a2.Id, _b.Id));
            Assert.AreEqual(1200, model.GetDependencyWeight(_a.Id, _b1.Id));
            Assert.AreEqual(30, model.GetDependencyWeight(_a.Id, _b2.Id));

            Assert.AreEqual(1230, model.GetDependencyWeight(_a.Id, _b.Id));

            model.UnremoveRelation(relation.Id);

            Assert.AreEqual(1000, model.GetDependencyWeight(_a1.Id, _b1.Id));
            Assert.AreEqual(200, model.GetDependencyWeight(_a2.Id, _b1.Id));
            Assert.AreEqual(30, model.GetDependencyWeight(_a1.Id, _b2.Id));
            Assert.AreEqual(4, model.GetDependencyWeight(_a2.Id, _b2.Id));

            Assert.AreEqual(1030, model.GetDependencyWeight(_a1.Id, _b.Id));
            Assert.AreEqual(204, model.GetDependencyWeight(_a2.Id, _b.Id));
            Assert.AreEqual(1200, model.GetDependencyWeight(_a.Id, _b1.Id));
            Assert.AreEqual(34, model.GetDependencyWeight(_a.Id, _b2.Id));

            Assert.AreEqual(1234, model.GetDependencyWeight(_a.Id, _b.Id));
        }

        [TestMethod]
        public void GivenCycleExistsBetweenElementsWhenIsCyclicDependencyThenReturnsTrue()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            CreateElementRelations(model);

            Assert.AreEqual(1234, model.GetDependencyWeight(_a.Id, _b.Id));
            Assert.AreEqual(5, model.GetDependencyWeight(_b.Id, _a.Id));
            Assert.IsTrue(model.IsCyclicDependency(_a.Id, _b.Id));
        }

        [TestMethod]
        public void GivenNoCycleExistsBetweenElementsWhenIsCyclicDependencyThenReturnsFalse()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            CreateElementRelations(model);

            Assert.AreEqual(5, model.GetDependencyWeight(_a1.Id, _c2.Id));
            Assert.AreEqual(0, model.GetDependencyWeight(_c2.Id, _a1.Id));
            Assert.IsFalse(model.IsCyclicDependency(_a1.Id, _c2.Id));
        }

        [TestMethod]
        public void GivenNoRelationExistsBetweenElementsWhenIsCyclicDependencyThenReturnsFalse()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            CreateElementRelations(model);

            Assert.AreEqual(0, model.GetDependencyWeight(_c1.Id, _c2.Id));
            Assert.AreEqual(0, model.GetDependencyWeight(_c2.Id, _c1.Id));
            Assert.IsFalse(model.IsCyclicDependency(_c1.Id, _c2.Id));
        }

        [TestMethod]
        public void GivenRelationExistsBetweenElementsWhenFindRelationsThenReturnsRelationBwetweenTheElements()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            CreateElementRelations(model);

            List<IDsmRelation> relations = model.FindRelations(_a, _b).OrderBy(x => x.Id).ToList();
            Assert.AreEqual(4, relations.Count);
            Assert.AreEqual(_a1.Id, relations[0].ConsumerId);
            Assert.AreEqual(_b1.Id, relations[0].ProviderId);
            Assert.AreEqual(1000, relations[0].Weight);

            Assert.AreEqual(_a2.Id, relations[1].ConsumerId);
            Assert.AreEqual(_b1.Id, relations[1].ProviderId);
            Assert.AreEqual(200, relations[1].Weight);

            Assert.AreEqual(_a1.Id, relations[2].ConsumerId);
            Assert.AreEqual(_b2.Id, relations[2].ProviderId);
            Assert.AreEqual(30, relations[2].Weight);

            Assert.AreEqual(_a2.Id, relations[3].ConsumerId);
            Assert.AreEqual(_b2.Id, relations[3].ProviderId);
            Assert.AreEqual(4, relations[3].Weight);
        }

        [TestMethod]
        public void GivenRelationExistsBetweenElementsWhenFindRelationsWhereElementHasProviderRoleThenReturnsRelationFromConsumers()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            CreateElementRelations(model);

            List<IDsmRelation> relations = model.FindRelationsWhereElementHasProviderRole(_a).OrderBy(x => x.Id).ToList();
            Assert.AreEqual(3, relations.Count);

            Assert.AreEqual(_b2.Id, relations[0].ConsumerId);
            Assert.AreEqual(_a1.Id, relations[0].ProviderId);
            Assert.AreEqual(2, relations[0].Weight);

            Assert.AreEqual(_b2.Id, relations[1].ConsumerId);
            Assert.AreEqual(_a2.Id, relations[1].ProviderId);
            Assert.AreEqual(3, relations[1].Weight);

            Assert.AreEqual(_c1.Id, relations[2].ConsumerId);
            Assert.AreEqual(_a2.Id, relations[2].ProviderId);
            Assert.AreEqual(4, relations[2].Weight);
        }

        [TestMethod]
        public void GivenRelationExistsBetweenElementsWhenFindRelationsWhereElementHasConsumerRoleThenReturnsRelationFromConsumers()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            CreateElementRelations(model);

            List<IDsmRelation> relations = model.FindRelationsWhereElementHasConsumerRole(_a).OrderBy(x => x.Id).ToList();
            Assert.AreEqual(5, relations.Count);

            Assert.AreEqual(_a1.Id, relations[0].ConsumerId);
            Assert.AreEqual(_b1.Id, relations[0].ProviderId);
            Assert.AreEqual(1000, relations[0].Weight);

            Assert.AreEqual(_a2.Id, relations[1].ConsumerId);
            Assert.AreEqual(_b1.Id, relations[1].ProviderId);
            Assert.AreEqual(200, relations[1].Weight);

            Assert.AreEqual(_a1.Id, relations[2].ConsumerId);
            Assert.AreEqual(_b2.Id, relations[2].ProviderId);
            Assert.AreEqual(30, relations[2].Weight);

            Assert.AreEqual(_a2.Id, relations[3].ConsumerId);
            Assert.AreEqual(_b2.Id, relations[3].ProviderId);
            Assert.AreEqual(4, relations[3].Weight);

            Assert.AreEqual(_a1.Id, relations[4].ConsumerId);
            Assert.AreEqual(_c2.Id, relations[4].ProviderId);
            Assert.AreEqual(5, relations[4].Weight);
        }

        private void CreateElementHierarchy()
        {
            _a = _elementsDataModel.ImportElement(11, "a", "", 1, false, null);
            _a1 = _elementsDataModel.ImportElement(12, "a1", "eta", 2, false, _a.Id);
            _a2 = _elementsDataModel.ImportElement(13, "a2", "eta", 3, false, _a.Id);
            _b = _elementsDataModel.ImportElement(14, "b", "", 4, false, null);
            _b1 = _elementsDataModel.ImportElement(15, "b1", "etb", 5, false, _b.Id);
            _b2 = _elementsDataModel.ImportElement(16, "b2", "etb", 6, false, _b.Id);
            _c = _elementsDataModel.ImportElement(17, "c", "", 7, false, null);
            _c1 = _elementsDataModel.ImportElement(18, "c1", "etc", 8, false, _c.Id);
            _c2 = _elementsDataModel.ImportElement(19, "c2", "etc", 9, false, _c.Id);
        }

        private void CreateElementRelations(DsmRelationsDataModel relationsDataModel)
        {
            relationsDataModel.AddRelation(_a1.Id, _a2.Id, "", 1);

            relationsDataModel.AddRelation(_a1.Id, _b1.Id, "", 1000);
            relationsDataModel.AddRelation(_a2.Id, _b1.Id, "", 200);
            relationsDataModel.AddRelation(_a1.Id, _b2.Id, "", 30);
            relationsDataModel.AddRelation(_a2.Id, _b2.Id, "", 4);

            relationsDataModel.AddRelation(_a1.Id, _c2.Id, "", 5);

            relationsDataModel.AddRelation(_b2.Id, _a1.Id, "", 2);
            relationsDataModel.AddRelation(_b2.Id, _a2.Id, "", 3);
            relationsDataModel.AddRelation(_c1.Id, _a2.Id, "", 4);
        }
    }
}
