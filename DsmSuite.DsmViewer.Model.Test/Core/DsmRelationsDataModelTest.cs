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
    ///   | c1 |      |      |      |      |      | 1    |
    /// c +----+------+------+------+------+------+------+
    ///   | c2 |  5   |      |      |      | 1    |      |
    /// --+----+------+------+------+------+------+------+
    /// </summary>
    /// 
    [TestClass]
    public class DsmRelationssDataModelTest
    {
        DsmElementModel _elementsDataModel;
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
            _elementsDataModel = new DsmElementModel();

            CreateElementHierarchy();
        }

        [TestMethod]
        public void WhenModelIsConstructedThenItIsEmpty()
        {
            DsmRelationModel model = new DsmRelationModel(_elementsDataModel);
            Assert.AreEqual(0, model.GetExportedRelationCount());
        }
        
        [TestMethod]
        public void GivenModelIsNotEmptyWhenClearIsCalledThenItIsEmpty()
        {
            DsmRelationModel model = new DsmRelationModel(_elementsDataModel);
            Assert.AreEqual(0, model.GetExportedRelationCount());

            model.AddRelation(_a.Id, _b.Id, "type", 1);
            Assert.AreEqual(1, model.GetExportedRelationCount());

            model.Clear();

            Assert.AreEqual(0, model.GetExportedRelationCount());
        }

        [TestMethod]
        public void GivenModelIsEmptyWhenAddRelationIsCalledThenItsHasOneRelation()
        {
            DsmRelationModel model = new DsmRelationModel(_elementsDataModel);
            Assert.AreEqual(0, model.GetExportedRelationCount());

            IDsmRelation relation = model.AddRelation(_a.Id, _b.Id, "type", 1);
            Assert.IsNotNull(relation);

            Assert.AreEqual(1, model.GetExportedRelationCount());
        }

        [TestMethod]
        public void GivenModelIsEmptyWhenImportRelationIsCalledThenItsHasOneRelation()
        {
            DsmRelationModel model = new DsmRelationModel(_elementsDataModel);
            Assert.AreEqual(0, model.GetExportedRelationCount());

            IDsmRelation relation = model.ImportRelation(1, _a.Id, _b.Id, "type", 1, false);
            Assert.IsNotNull(relation);

            Assert.AreEqual(1, model.GetExportedRelationCount());
        }

        [TestMethod]
        public void GivenModelIsFilledWhenGetWeightWithSelfThenReturnsZero()
        {
            DsmRelationModel model = new DsmRelationModel(_elementsDataModel);
            CreateElementRelations(model);
            Assert.AreEqual(11, model.GetExportedRelationCount());

            foreach(IDsmElement element in _elementsDataModel.GetElements())
            {
                Assert.AreEqual(0, model.GetDependencyWeight(element.Id, element.Id));
            }
        }

        [TestMethod]
        public void GivenModelIsFilledWhenGetWeightWithOtherThenReturnsCalculatedDerivedWeights()
        {
            DsmRelationModel model = new DsmRelationModel(_elementsDataModel);
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
        public void GivenModelIsFilledWhenChangeRelationWeightThenUpdatesCalculatedDerivedWeights()
        {
            DsmRelationModel model = new DsmRelationModel(_elementsDataModel);
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

            model.ChangeRelationWeight(relation, 5);

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
            DsmRelationModel model = new DsmRelationModel(_elementsDataModel);
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
            DsmRelationModel model = new DsmRelationModel(_elementsDataModel);
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
            DsmRelationModel model = new DsmRelationModel(_elementsDataModel);
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
            DsmRelationModel model = new DsmRelationModel(_elementsDataModel);
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
        public void GivenNoCyclicRelationExistsBetweenElementsWhenIsCyclicDependencyThenReturnsCycleTypeNone()
        {
            DsmRelationModel model = new DsmRelationModel(_elementsDataModel);
            CreateElementRelations(model);

            Assert.AreEqual(0, model.GetDependencyWeight(_b1.Id, _b2.Id));
            Assert.AreEqual(0, model.GetDependencyWeight(_b2.Id, _b1.Id));
            Assert.AreEqual(CycleType.None, model.IsCyclicDependency(_b1.Id, _b2.Id));
        }

        [TestMethod]
        public void GivenDirectCycleExistsBetweenElementsWhenIsCyclicDependencyThenReturnsCycleTypeSystem()
        {
            DsmRelationModel model = new DsmRelationModel(_elementsDataModel);
            CreateElementRelations(model);

            Assert.AreEqual(1, model.GetDirectDependencyWeight(_c1.Id, _c2.Id));
            Assert.AreEqual(1, model.GetDirectDependencyWeight(_c2.Id, _c1.Id));
            Assert.AreEqual(CycleType.System, model.IsCyclicDependency(_c1.Id, _c2.Id));
        }

        [TestMethod]
        public void GivenIndirectCycleExistsBetweenElementsWhenIsCyclicDependencyThenReturnsCycleTypeHierarchical()
        {
            DsmRelationModel model = new DsmRelationModel(_elementsDataModel);
            CreateElementRelations(model);

            Assert.AreEqual(1234, model.GetDependencyWeight(_a.Id, _b.Id));
            Assert.AreEqual(5, model.GetDependencyWeight(_b.Id, _a.Id));
            Assert.AreEqual(CycleType.Hierarchical, model.IsCyclicDependency(_a.Id, _b.Id));
        }

        [TestMethod]
        public void GivenNoCycleExistsBetweenElementsWhenIsCyclicDependencyThenReturnsFalse()
        {
            DsmRelationModel model = new DsmRelationModel(_elementsDataModel);
            CreateElementRelations(model);

            Assert.AreEqual(5, model.GetDependencyWeight(_a1.Id, _c2.Id));
            Assert.AreEqual(0, model.GetDependencyWeight(_c2.Id, _a1.Id));
            Assert.AreEqual(CycleType.None, model.IsCyclicDependency(_a1.Id, _c2.Id));
        }



        [TestMethod]
        public void GivenRelationExistsBetweenElementsWhenFindRelationsThenReturnsRelationBwetweenTheElements()
        {
            DsmRelationModel model = new DsmRelationModel(_elementsDataModel);
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
            DsmRelationModel model = new DsmRelationModel(_elementsDataModel);
            CreateElementRelations(model);

            List<IDsmRelation> relations = model.FindIngoingRelations(_a).OrderBy(x => x.Id).ToList();
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
            DsmRelationModel model = new DsmRelationModel(_elementsDataModel);
            CreateElementRelations(model);

            List<IDsmRelation> relations = model.FindOutgoingRelations(_a).OrderBy(x => x.Id).ToList();
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

        [TestMethod]
        public void GivenMatrixIsFilledWhenChildElementParentIsChangedThenWeightsAreUpdated()
        {
            DsmRelationModel model = new DsmRelationModel(_elementsDataModel);
            CreateElementRelations(model);

            Assert.AreEqual(1030, model.GetDependencyWeight(_a1.Id, _b.Id));
            Assert.AreEqual(1234, model.GetDependencyWeight(_a.Id, _b.Id));
            Assert.AreEqual(5, model.GetDependencyWeight(_a.Id, _c.Id));

            _elementsDataModel.ChangeElementParent(_c2, _b);

            Assert.AreEqual(1035, model.GetDependencyWeight(_a1.Id, _b.Id));
            Assert.AreEqual(1239, model.GetDependencyWeight(_a.Id, _b.Id));
            Assert.AreEqual(0, model.GetDependencyWeight(_a.Id, _c.Id));
        }

        [TestMethod]
        public void GivenMatrixIsFilledWhenRootElementParentIsChangedThenWeightsAreUpdated()
        {
            DsmRelationModel model = new DsmRelationModel(_elementsDataModel);
            CreateElementRelations(model);

            Assert.AreEqual(1234, model.GetDependencyWeight(_a.Id, _b.Id));

            _elementsDataModel.ChangeElementParent(_c, _b);

            Assert.AreEqual(1239, model.GetDependencyWeight(_a.Id, _b.Id));
        }

        private void CreateElementHierarchy()
        {
            _a = _elementsDataModel.ImportElement(11, "a", "", 1, false, null, false);
            _a1 = _elementsDataModel.ImportElement(12, "a1", "eta", 2, false, _a.Id, false);
            _a2 = _elementsDataModel.ImportElement(13, "a2", "eta", 3, false, _a.Id, false);
            _b = _elementsDataModel.ImportElement(14, "b", "", 4, false, null, false);
            _b1 = _elementsDataModel.ImportElement(15, "b1", "etb", 5, false, _b.Id, false);
            _b2 = _elementsDataModel.ImportElement(16, "b2", "etb", 6, false, _b.Id, false);
            _c = _elementsDataModel.ImportElement(17, "c", "", 7, false, null, false);
            _c1 = _elementsDataModel.ImportElement(18, "c1", "etc", 8, false, _c.Id, false);
            _c2 = _elementsDataModel.ImportElement(19, "c2", "etc", 9, false, _c.Id, false);
        }

        private void CreateElementRelations(DsmRelationModel relationsDataModel)
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

            relationsDataModel.AddRelation(_c1.Id, _c2.Id, "", 1);
            relationsDataModel.AddRelation(_c2.Id, _c1.Id, "", 1);
        }
    }
}
