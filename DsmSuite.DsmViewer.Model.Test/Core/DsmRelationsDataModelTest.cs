﻿using System.Linq;
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
    ///   | a2 |      |      |      | 3    |  4   |      |
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
        public void When_ModelIsConstructed_Then_ItIsEmpty()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.GetRelationCount());
        }
        
        [TestMethod]
        public void Given_ModelIsNotEmpty_When_ClearIsCalled_Then_ItIsEmpty()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.GetRelationCount());

            model.AddRelation(_a.Id, _b.Id, "type", 1);
            Assert.AreEqual(1, model.GetRelationCount());

            model.Clear();

            Assert.AreEqual(0, model.GetRelationCount());
        }

        [TestMethod]
        public void Given_ModelIsEmpty_When_AddRelationIsCalled_Then_ItsHasOneRelation()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.GetRelationCount());

            IDsmRelation relation = model.AddRelation(_a.Id, _b.Id, "type", 1);
            Assert.IsNotNull(relation);

            Assert.AreEqual(1, model.GetRelationCount());
        }

        [TestMethod]
        public void Given_ModelIsEmpty_When_ImportRelationIsCalled_Then_ItsHasOneRelation()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            Assert.AreEqual(0, model.GetRelationCount());

            IDsmRelation relation = model.ImportRelation(1, _a.Id, _b.Id, "type", 1);
            Assert.IsNotNull(relation);

            Assert.AreEqual(1, model.GetRelationCount());
        }

        [TestMethod]
        public void Given_ModelIsFilled_When_GetWeightWithSelf_Then_ReturnsZero()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            CreateElementRelations(model);
            Assert.AreEqual(8, model.GetRelationCount());

            foreach(DsmElement element in _elementsDataModel.GetElements())
            {
                Assert.AreEqual(0, model.GetDependencyWeight(element.Id, element.Id));
            }
        }

        [TestMethod]
        public void Given_ModelIsFilled_When_GetWeightWithOther_Then_ReturnsCalculatedDerivedWeights()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            CreateElementRelations(model);
            Assert.AreEqual(8, model.GetRelationCount());

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
        public void Given_ModelIsFilled_When_EditingRelationWeight_Then_UpdatesCalculatedDerivedWeights()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            CreateElementRelations(model);
            Assert.AreEqual(8, model.GetRelationCount());

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
        public void Given_ModelIsFilled_When_RemovingRelation_Then_ReducesRelationCount()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            CreateElementRelations(model);
            Assert.AreEqual(8, model.GetRelationCount());

            IDsmRelation relation = model.FindRelations(_a2, _b2).FirstOrDefault();
            model.RemoveRelation(relation.Id);
            Assert.AreEqual(7, model.GetRelationCount());
        }

        [TestMethod]
        public void Given_ModelIsFilled_When_UnremovingRelation_Then_RestoresRelationCount()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            CreateElementRelations(model);
            Assert.AreEqual(8, model.GetRelationCount());

            IDsmRelation relation = model.FindRelations(_a2, _b2).FirstOrDefault();
            model.RemoveRelation(relation.Id);
            Assert.AreEqual(7, model.GetRelationCount());

            model.UnremoveRelation(relation.Id);
            Assert.AreEqual(8, model.GetRelationCount());
        }

        [TestMethod]
        public void Given_ModelIsFilled_When_RemovingRelation_Then_ReducesCalculatedDerivedWeights()
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
        public void Given_ModelIsFilled_When_UnremovingRelation_Then_RestoresCalculatedDerivedWeights()
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
        public void Given_CycleExistsBetweenElements_When_IsCyclicDependency_Then_ReturnsTrue()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            CreateElementRelations(model);

            Assert.AreEqual(1234, model.GetDependencyWeight(_a.Id, _b.Id));
            Assert.AreEqual(5, model.GetDependencyWeight(_b.Id, _a.Id));
            Assert.IsTrue(model.IsCyclicDependency(_a.Id, _b.Id));
        }

        [TestMethod]
        public void Given_NoCycleExistsBetweenElements_When_IsCyclicDependency_Then_ReturnsFalse()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            CreateElementRelations(model);

            Assert.AreEqual(5, model.GetDependencyWeight(_a1.Id, _c2.Id));
            Assert.AreEqual(0, model.GetDependencyWeight(_c2.Id, _a1.Id));
            Assert.IsFalse(model.IsCyclicDependency(_a1.Id, _c2.Id));
        }

        [TestMethod]
        public void Given_NoRelationExistsBetweenElements_When_IsCyclicDependency_Then_ReturnsFalse()
        {
            DsmRelationsDataModel model = new DsmRelationsDataModel(_elementsDataModel);
            CreateElementRelations(model);

            Assert.AreEqual(0, model.GetDependencyWeight(_c1.Id, _c2.Id));
            Assert.AreEqual(0, model.GetDependencyWeight(_c2.Id, _c1.Id));
            Assert.IsFalse(model.IsCyclicDependency(_c1.Id, _c2.Id));
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
