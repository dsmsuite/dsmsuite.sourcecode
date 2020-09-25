using System.Collections.Generic;
using System.Linq;
using DsmSuite.DsmViewer.Model.Core;
using DsmSuite.DsmViewer.Model.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        DsmRelationModel _relationsDataModel;
        DsmElementModel _elementsDataModel;

        private DsmElement _root;

        private IDsmElement _a;
        private IDsmElement _a1;
        private IDsmElement _a2;
        private IDsmElement _b;
        private IDsmElement _b1;
        private IDsmElement _b2;
        private IDsmElement _c;
        private IDsmElement _c1;
        private IDsmElement _c2;

        private Dictionary<IDsmElement, Dictionary<IDsmElement, int>> _expectedlWeights;
        private Dictionary<IDsmElement, Dictionary<IDsmElement, int>> _actualWeights;

                [TestInitialize]
        public void TestInitialize()
        {
            _relationsDataModel = new DsmRelationModel();
            _elementsDataModel = new DsmElementModel(_relationsDataModel);
            _root = _elementsDataModel.GetRootElement() as DsmElement;
            _expectedlWeights = new Dictionary<IDsmElement, Dictionary<IDsmElement, int>>();
            _actualWeights = new Dictionary<IDsmElement, Dictionary<IDsmElement, int>>();

            CreateElementHierarchy();
        }

        [TestMethod]
        public void WhenModelIsConstructedThenItIsEmpty()
        {
            Assert.AreEqual(0, _relationsDataModel.GetExportedRelationCount());
        }

        [TestMethod]
        public void WhenOutgoingRelatonsAreAddedThenGetOutgoingRelationsReturnsAll()
        {
            Assert.AreEqual(0, _relationsDataModel.GetExportedRelationCount());

            IDsmRelation relation1 = _relationsDataModel.AddRelation(_a, _b, "type", 1);
            Assert.AreEqual(1, _relationsDataModel.FindOutgoingRelations(_a).Count());

            IDsmRelation relation2 = _relationsDataModel.AddRelation(_a, _c, "type", 1);
            Assert.AreEqual(2, _relationsDataModel.FindOutgoingRelations(_a).Count());

            _relationsDataModel.RemoveRelation(relation1.Id);
            Assert.AreEqual(1, _relationsDataModel.FindOutgoingRelations(_a).Count());

            _relationsDataModel.RemoveRelation(relation2.Id);
            Assert.AreEqual(0, _relationsDataModel.FindOutgoingRelations(_a).Count());
        }

        [TestMethod]
        public void WhenIngoingRelatonsAreAddedThenGetIngoingRelationsReturnsAll()
        {
            Assert.AreEqual(0, _relationsDataModel.GetExportedRelationCount());

            IDsmRelation relation1 = _relationsDataModel.AddRelation(_a, _b, "type", 1);
            Assert.AreEqual(1, _relationsDataModel.FindIngoingRelations(_b).Count());

            IDsmRelation relation2 = _relationsDataModel.AddRelation(_c, _b, "type", 1);
            Assert.AreEqual(2, _relationsDataModel.FindIngoingRelations(_b).Count());

            _relationsDataModel.RemoveRelation(relation1.Id);
            Assert.AreEqual(1, _relationsDataModel.FindIngoingRelations(_b).Count());

            _relationsDataModel.RemoveRelation(relation2.Id);
            Assert.AreEqual(0, _relationsDataModel.FindIngoingRelations(_b).Count());
        }

        [TestMethod]
        public void WhenIngoingAndOutgoingRelatonsAreAddedThenGetExternalRelationsReturnsOnlyExternalOnes()
        {
            Assert.AreEqual(0, _relationsDataModel.GetExportedRelationCount());

            IDsmRelation relation1 = _relationsDataModel.AddRelation(_a1, _b1, "type", 1);
            Assert.AreEqual(1, _relationsDataModel.FindExternalRelations(_a).Count());
            Assert.AreEqual(1, _relationsDataModel.FindExternalRelations(_b).Count());

            IDsmRelation relation2 = _relationsDataModel.AddRelation(_b2, _a1, "type", 1);
            Assert.AreEqual(2, _relationsDataModel.FindExternalRelations(_a).Count());
            Assert.AreEqual(2, _relationsDataModel.FindExternalRelations(_b).Count());

            IDsmRelation relation3 = _relationsDataModel.AddRelation(_a1, _a2, "type", 1);
            Assert.AreEqual(2, _relationsDataModel.FindExternalRelations(_a).Count());
            Assert.AreEqual(2, _relationsDataModel.FindExternalRelations(_b).Count());

            _relationsDataModel.RemoveRelation(relation1.Id);
            Assert.AreEqual(1, _relationsDataModel.FindExternalRelations(_a).Count());
            Assert.AreEqual(1, _relationsDataModel.FindExternalRelations(_b).Count());

            _relationsDataModel.RemoveRelation(relation2.Id);
            Assert.AreEqual(0, _relationsDataModel.FindExternalRelations(_a).Count());
            Assert.AreEqual(0, _relationsDataModel.FindExternalRelations(_b).Count());

            _relationsDataModel.RemoveRelation(relation3.Id);
            Assert.AreEqual(0, _relationsDataModel.FindExternalRelations(_a).Count());
            Assert.AreEqual(0, _relationsDataModel.FindExternalRelations(_b).Count());
        }

        [TestMethod]
        public void WhenIngoingAndOutgoingRelatonsAreAddedThenGetInternalRelationsReturnsOnlyInternalOnes()
        {
            Assert.AreEqual(0, _relationsDataModel.GetExportedRelationCount());

            IDsmRelation relation1 = _relationsDataModel.AddRelation(_a1, _b1, "type", 1);
            Assert.AreEqual(0, _relationsDataModel.FindInternalRelations(_a).Count());
            Assert.AreEqual(0, _relationsDataModel.FindInternalRelations(_b).Count());

            IDsmRelation relation2 = _relationsDataModel.AddRelation(_b2, _a1, "type", 1);
            Assert.AreEqual(0, _relationsDataModel.FindInternalRelations(_a).Count());
            Assert.AreEqual(0, _relationsDataModel.FindInternalRelations(_b).Count());

            IDsmRelation relation3 = _relationsDataModel.AddRelation(_a1, _a2, "type", 1);
            Assert.AreEqual(1, _relationsDataModel.FindInternalRelations(_a).Count());
            Assert.AreEqual(1, _relationsDataModel.FindInternalRelations(_a).Count());

            _relationsDataModel.RemoveRelation(relation1.Id);
            Assert.AreEqual(1, _relationsDataModel.FindInternalRelations(_a).Count());
            Assert.AreEqual(0, _relationsDataModel.FindInternalRelations(_b).Count());

            _relationsDataModel.RemoveRelation(relation2.Id);
            Assert.AreEqual(1, _relationsDataModel.FindInternalRelations(_a).Count());
            Assert.AreEqual(0, _relationsDataModel.FindInternalRelations(_b).Count());

            _relationsDataModel.RemoveRelation(relation3.Id);
            Assert.AreEqual(0, _relationsDataModel.FindInternalRelations(_a).Count());
            Assert.AreEqual(0, _relationsDataModel.FindInternalRelations(_b).Count());
        }

        [TestMethod]
        public void GivenModelIsNotEmptyWhenClearIsCalledThenItIsEmpty()
        {
            Assert.AreEqual(0, _relationsDataModel.GetExportedRelationCount());

            _relationsDataModel.AddRelation(_a, _b, "type", 1);
            Assert.AreEqual(1, _relationsDataModel.GetExportedRelationCount());

            _relationsDataModel.Clear();

            Assert.AreEqual(0, _relationsDataModel.GetExportedRelationCount());
        }

        [TestMethod]
        public void GivenModelIsEmptyWhenAddRelationIsCalledThenItsHasOneRelation()
        {
            Assert.AreEqual(0, _relationsDataModel.GetExportedRelationCount());

            IDsmRelation relation = _relationsDataModel.AddRelation(_a, _b, "type", 1);
            Assert.IsNotNull(relation);

            Assert.AreEqual(1, _relationsDataModel.GetExportedRelationCount());
        }

        [TestMethod]
        public void GivenModelIsEmptyWhenImportRelationIsCalledThenItsHasOneRelation()
        {
            Assert.AreEqual(0, _relationsDataModel.GetExportedRelationCount());

            IDsmRelation relation = _relationsDataModel.ImportRelation(1, _a, _b, "type", 1, false);
            Assert.IsNotNull(relation);

            Assert.AreEqual(1, _relationsDataModel.GetExportedRelationCount());
        }

        [TestMethod]
        public void GivenModelIsFilledWhenGetWeightWithSelfThenReturnsZero()
        {
            CreateElementRelations(_relationsDataModel);
            Assert.AreEqual(11, _relationsDataModel.GetExportedRelationCount());

            foreach(IDsmElement element in _elementsDataModel.GetElements())
            {
                Assert.AreEqual(0, _relationsDataModel.GetDependencyWeight(element, element));
            }
        }

        [TestMethod]
        public void GivenModelIsFilledWhenGetWeightWithOtherThenReturnsCalculatedDerivedWeights()
        {
            CreateElementRelations(_relationsDataModel);

            CheckDependencyWeights();
        }

        [TestMethod]
        public void GivenModelIsFilledWhenChangeRelationWeightThenUpdatesCalculatedDerivedWeights()
        {
            CreateElementRelations(_relationsDataModel);

            IDsmRelation relation = _relationsDataModel.FindRelations(_a2, _b2).FirstOrDefault();
            Assert.IsNotNull(relation);
            _relationsDataModel.ChangeRelationWeight(relation, 5);

            _expectedlWeights[_a2][_b2] = 5;
            _expectedlWeights[_a2][_b] = 205;
            _expectedlWeights[_a][_b2] = 35;
            _expectedlWeights[_a][_b] = 1235;

            CheckDependencyWeights();
        }

        [TestMethod]
        public void GivenModelIsFilledWhenRemovingRelationThenReducesRelationCount()
        {
            CreateElementRelations(_relationsDataModel);
            int relationCountBefore = _relationsDataModel.GetRelationCount();

            IDsmRelation relation = _relationsDataModel.FindRelations(_a2, _b2).FirstOrDefault();
            Assert.IsNotNull(relation);

            _relationsDataModel.RemoveRelation(relation.Id);
            Assert.AreEqual(relationCountBefore - 1, _relationsDataModel.GetRelationCount());
        }

        [TestMethod]
        public void GivenModelIsFilledWhenUnremovingRelationThenRestoresRelationCount()
        {
            CreateElementRelations(_relationsDataModel);

            IDsmRelation relation = _relationsDataModel.FindRelations(_a2, _b2).FirstOrDefault();
            Assert.IsNotNull(relation);

            _relationsDataModel.RemoveRelation(relation.Id);
            int relationCountBefore = _relationsDataModel.GetRelationCount();

            _relationsDataModel.UnremoveRelation(relation.Id);
            Assert.AreEqual(relationCountBefore + 1, _relationsDataModel.GetRelationCount());
        }

        [TestMethod]
        public void GivenModelIsFilledWhenRemovingRelationThenReducesCalculatedDerivedWeights()
        {
            CreateElementRelations(_relationsDataModel);

            IDsmRelation relation = _relationsDataModel.FindRelations(_a2, _b2).FirstOrDefault();
            Assert.IsNotNull(relation);
            _relationsDataModel.RemoveRelation(relation.Id);

            _expectedlWeights[_a2][_b2] = 0;
            _expectedlWeights[_a2][_b] = 200;
            _expectedlWeights[_a][_b2] = 30;
            _expectedlWeights[_a][_b] = 1230;

            CheckDependencyWeights();
        }

        [TestMethod]
        public void GivenModelIsFilledWhenUnremovingRelationThenRestoresCalculatedDerivedWeights()
        {
            CreateElementRelations(_relationsDataModel);

            IDsmRelation relation = _relationsDataModel.FindRelations(_a2, _b2).FirstOrDefault();
            Assert.IsNotNull(relation);
            _relationsDataModel.RemoveRelation(relation.Id);

            _expectedlWeights[_a2][_b2] = 0;
            _expectedlWeights[_a2][_b] = 200;
            _expectedlWeights[_a][_b2] = 30;
            _expectedlWeights[_a][_b] = 1230;

            CheckDependencyWeights();

            _relationsDataModel.UnremoveRelation(relation.Id);

            _expectedlWeights[_a2][_b2] = 4;
            _expectedlWeights[_a2][_b] = 204;
            _expectedlWeights[_a][_b2] = 34;
            _expectedlWeights[_a][_b] = 1234;

            CheckDependencyWeights();
        }

        [TestMethod]
        public void GivenNoCyclicRelationExistsBetweenElementsWhenIsCyclicDependencyThenReturnsCycleTypeNone()
        {
            CreateElementRelations(_relationsDataModel);

            Assert.AreEqual(0, _relationsDataModel.GetDependencyWeight(_b1, _b2));
            Assert.AreEqual(0, _relationsDataModel.GetDependencyWeight(_b2, _b1));
            Assert.AreEqual(CycleType.None, _relationsDataModel.IsCyclicDependency(_b1, _b2));
        }

        [TestMethod]
        public void GivenDirectCycleExistsBetweenElementsWhenIsCyclicDependencyThenReturnsCycleTypeSystem()
        {
            CreateElementRelations(_relationsDataModel);

            Assert.AreEqual(1, _relationsDataModel.GetDirectDependencyWeight(_c1, _c2));
            Assert.AreEqual(1, _relationsDataModel.GetDirectDependencyWeight(_c2, _c1));
            Assert.AreEqual(CycleType.System, _relationsDataModel.IsCyclicDependency(_c1, _c2));
        }

        [TestMethod]
        public void GivenIndirectCycleExistsBetweenElementsWhenIsCyclicDependencyThenReturnsCycleTypeHierarchical()
        {
            CreateElementRelations(_relationsDataModel);

            Assert.AreEqual(1234, _relationsDataModel.GetDependencyWeight(_a, _b));
            Assert.AreEqual(5, _relationsDataModel.GetDependencyWeight(_b, _a));
            Assert.AreEqual(CycleType.Hierarchical, _relationsDataModel.IsCyclicDependency(_a, _b));
        }

        [TestMethod]
        public void GivenNoCycleExistsBetweenElementsWhenIsCyclicDependencyThenReturnsFalse()
        {
            CreateElementRelations(_relationsDataModel);

            Assert.AreEqual(5, _relationsDataModel.GetDependencyWeight(_a1, _c2));
            Assert.AreEqual(0, _relationsDataModel.GetDependencyWeight(_c2, _a1));
            Assert.AreEqual(CycleType.None, _relationsDataModel.IsCyclicDependency(_a1, _c2));
        }
        
        [TestMethod]
        public void GivenRelationExistsBetweenElementsWhenFindRelationsThenReturnsRelationBwetweenTheElements()
        {
            CreateElementRelations(_relationsDataModel);

            List<DsmRelation> relations = _relationsDataModel.FindRelations(_a, _b).OrderBy(x => x.Id).ToList();
            Assert.AreEqual(4, relations.Count);
            Assert.AreEqual(_a1.Id, relations[0].Consumer.Id);
            Assert.AreEqual(_b1.Id, relations[0].Provider.Id);
            Assert.AreEqual(1000, relations[0].Weight);

            Assert.AreEqual(_a2.Id, relations[1].Consumer.Id);
            Assert.AreEqual(_b1.Id, relations[1].Provider.Id);
            Assert.AreEqual(200, relations[1].Weight);

            Assert.AreEqual(_a1.Id, relations[2].Consumer.Id);
            Assert.AreEqual(_b2.Id, relations[2].Provider.Id);
            Assert.AreEqual(30, relations[2].Weight);

            Assert.AreEqual(_a2.Id, relations[3].Consumer.Id);
            Assert.AreEqual(_b2.Id, relations[3].Provider.Id);
            Assert.AreEqual(4, relations[3].Weight);
        }

        [TestMethod]
        public void GivenRelationExistsBetweenElementsWhenFindRelationsWhereElementHasProviderRoleThenReturnsRelationFromConsumers()
        {
            CreateElementRelations(_relationsDataModel);

            List<IDsmRelation> relations = _relationsDataModel.FindIngoingRelations(_a).OrderBy(x => x.Id).ToList();
            Assert.AreEqual(3, relations.Count);

            Assert.AreEqual(_b2.Id, relations[0].Consumer.Id);
            Assert.AreEqual(_a1.Id, relations[0].Provider.Id);
            Assert.AreEqual(2, relations[0].Weight);

            Assert.AreEqual(_b2.Id, relations[1].Consumer.Id);
            Assert.AreEqual(_a2.Id, relations[1].Provider.Id);
            Assert.AreEqual(3, relations[1].Weight);

            Assert.AreEqual(_c1.Id, relations[2].Consumer.Id);
            Assert.AreEqual(_a2.Id, relations[2].Provider.Id);
            Assert.AreEqual(4, relations[2].Weight);
        }

        [TestMethod]
        public void GivenRelationExistsBetweenElementsWhenFindRelationsWhereElementHasConsumerRoleThenReturnsRelationFromConsumers()
        {
            CreateElementRelations(_relationsDataModel);

            List<IDsmRelation> relations = _relationsDataModel.FindOutgoingRelations(_a).OrderBy(x => x.Id).ToList();
            Assert.AreEqual(5, relations.Count);

            Assert.AreEqual(_a1.Id, relations[0].Consumer.Id);
            Assert.AreEqual(_b1.Id, relations[0].Provider.Id);
            Assert.AreEqual(1000, relations[0].Weight);

            Assert.AreEqual(_a2.Id, relations[1].Consumer.Id);
            Assert.AreEqual(_b1.Id, relations[1].Provider.Id);
            Assert.AreEqual(200, relations[1].Weight);

            Assert.AreEqual(_a1.Id, relations[2].Consumer.Id);
            Assert.AreEqual(_b2.Id, relations[2].Provider.Id);
            Assert.AreEqual(30, relations[2].Weight);

            Assert.AreEqual(_a2.Id, relations[3].Consumer.Id);
            Assert.AreEqual(_b2.Id, relations[3].Provider.Id);
            Assert.AreEqual(4, relations[3].Weight);

            Assert.AreEqual(_a1.Id, relations[4].Consumer.Id);
            Assert.AreEqual(_c2.Id, relations[4].Provider.Id);
            Assert.AreEqual(5, relations[4].Weight);
        }

        [TestMethod]
        public void GivenMatrixIsFilledWhenChildElementParentIsChangedThenWeightsAreUpdated()
        {
            CreateElementRelations(_relationsDataModel);

            _elementsDataModel.ChangeElementParent(_c2, _b, 0);

            // Derived a 
            _expectedlWeights[_a1][_b] = 1035;
            _expectedlWeights[_a][_b] = 1239;

            _expectedlWeights[_a][_c] = 0;
            _expectedlWeights[_a1][_c] = 0;

            // Derived b
            _expectedlWeights[_b][_c] = 1;
            _expectedlWeights[_b][_c1] = 1;

            // Derived c
            _expectedlWeights[_c][_b] = 1;
            _expectedlWeights[_c1][_b] = 1;
            _expectedlWeights[_c][_c2] = 1;
            _expectedlWeights[_c2][_c] = 1;
            
            CheckDependencyWeights();
        }

        [TestMethod]
        public void GivenMatrixIsFilledWhenRootElementParentIsChangedThenWeightsAreUpdated()
        {
            CreateElementRelations(_relationsDataModel);

            Assert.AreEqual(1234, _relationsDataModel.GetDependencyWeight(_a, _b));

            _elementsDataModel.ChangeElementParent(_c, _b, 0);

            Assert.AreEqual(1239, _relationsDataModel.GetDependencyWeight(_a, _b));
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
            _relationsDataModel.AddRelation(_a1, _a2, "", 1);

            _relationsDataModel.AddRelation(_a1, _b1, "", 1000);
            _relationsDataModel.AddRelation(_a2, _b1, "", 200);
            _relationsDataModel.AddRelation(_a1, _b2, "", 30);
            _relationsDataModel.AddRelation(_a2, _b2, "", 4);

            _relationsDataModel.AddRelation(_a1, _c2, "", 5);

            _relationsDataModel.AddRelation(_b2, _a1, "", 2);
            _relationsDataModel.AddRelation(_b2, _a2, "", 3);
            _relationsDataModel.AddRelation(_c1, _a2, "", 4);

            _relationsDataModel.AddRelation(_c1, _c2, "", 1);
            _relationsDataModel.AddRelation(_c2, _c1, "", 1);

            SetExpectedDependencyWeights();
        }

        private void SetExpectedDependencyWeights()
        {
            IDictionary<int, DsmElement> children = _root.GetElementAndItsChildren();
            foreach (DsmElement consumer in children.Values)
            {
                foreach (DsmElement provider in children.Values)
                {
                    if (!_expectedlWeights.ContainsKey(consumer))
                    {
                        _expectedlWeights[consumer] = new Dictionary<IDsmElement, int>();
                    }
                    _expectedlWeights[consumer][provider] = 0;
                }
            }

            // Direct a
            _expectedlWeights[_a1][_a2] = 1;

            _expectedlWeights[_a1][_b1] = 1000;
            _expectedlWeights[_a2][_b1] = 200;
            _expectedlWeights[_a1][_b2] = 30;
            _expectedlWeights[_a2][_b2] = 4;

            _expectedlWeights[_a1][_c2] = 5;

            // Direct b
            _expectedlWeights[_b2][_a1] = 2;
            _expectedlWeights[_b2][_a2] = 3;

            // Direct c
            _expectedlWeights[_c1][_a2] = 4;

            _expectedlWeights[_c1][_c2] = 1;
            _expectedlWeights[_c2][_c1] = 1;

            // Derived a
            _expectedlWeights[_a1][_b] = 1030;
            _expectedlWeights[_a2][_b] = 204;
            _expectedlWeights[_a][_b1] = 1200;
            _expectedlWeights[_a][_b2] = 34;

            _expectedlWeights[_a][_b] = 1234;

            _expectedlWeights[_a][_c2] = 5;
            _expectedlWeights[_a1][_c] = 5;
            _expectedlWeights[_a][_c] = 5;

            // Derived b
            _expectedlWeights[_b][_a1] = 2;
            _expectedlWeights[_b][_a2] = 3;
            _expectedlWeights[_b2][_a] = 5;
            _expectedlWeights[_b][_a] = 5;

            // Derived c
            _expectedlWeights[_c1][_a] = 4;
            _expectedlWeights[_c][_a2] = 4;
            _expectedlWeights[_c][_a] = 4;
        }
        
        private void CheckDependencyWeights()
        {
            SetActualDependencyWeights();
            IDictionary<int, DsmElement> children = _root.GetElementAndItsChildren();
            foreach (DsmElement consumer in children.Values)
            {
                foreach (DsmElement provider in children.Values)
                {
                    int expected = _expectedlWeights[consumer][provider];
                    int actual = _actualWeights[consumer][provider];
                    Assert.AreEqual(expected, actual, $"Weight not equal consumer={consumer.Fullname} provider={provider.Fullname} expected={expected}");
                }
            }
        }

        private void SetActualDependencyWeights()
        {
            IDictionary<int, DsmElement> children = _root.GetElementAndItsChildren();
            foreach (DsmElement consumer in children.Values)
            {
                foreach (DsmElement provider in children.Values)
                {
                    if (!_actualWeights.ContainsKey(consumer))
                    {
                        _actualWeights[consumer] = new Dictionary<IDsmElement, int>();
                    }
                    _actualWeights[consumer][provider] = consumer.Dependencies.GetDerivedDependencyWeight(provider);
                }
            }
        }
    }
}
