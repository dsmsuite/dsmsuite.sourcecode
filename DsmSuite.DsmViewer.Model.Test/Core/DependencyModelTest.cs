using System;
using System.Collections.Generic;
using System.Linq;
using DsmSuite.DsmViewer.Model.Data;
using DsmSuite.DsmViewer.Model.Dependencies;
using DsmSuite.DsmViewer.Model.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.DsmViewer.Model.Test.Dependencies
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
    [TestClass]
    public class DependencyModelTest
    {
        private DependencyModel _dependencyModel;
        private IDsmElement _a;
        private IDsmElement _a1;
        private IDsmElement _a2;
        private IDsmElement _b;
        private IDsmElement _b1;
        private IDsmElement _b2;
        private IDsmElement _c;
        private IDsmElement _c1;
        private IDsmElement _c2;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            _dependencyModel = new DependencyModel();
        }



        [TestMethod]
        public void CreateElementTest()
        {
            Assert.AreEqual(0, _dependencyModel.ElementCount);

            _a = _dependencyModel.CreateElement("a", "", null);
            Assert.AreNotEqual(0, _a.Id);
            Assert.AreEqual(0, _a.Order);
            Assert.AreEqual("", _a.Type);
            Assert.AreEqual("a", _a.Name);
            Assert.AreEqual("a", _a.Fullname);
            Assert.AreEqual(1, _dependencyModel.ElementCount);
            Assert.AreEqual(_a, _dependencyModel.GetElementByFullname("a"));

            _a1 = _dependencyModel.CreateElement("a1", "eta", _a.Id);
            Assert.AreNotEqual(0, _a1.Id);
            Assert.AreEqual(0, _a1.Order);
            Assert.AreEqual("eta", _a1.Type);
            Assert.AreEqual("a1", _a1.Name);
            Assert.AreEqual("a.a1", _a1.Fullname);
            Assert.AreEqual(2, _dependencyModel.ElementCount);
            Assert.AreEqual(_a1, _dependencyModel.GetElementByFullname("a.a1"));

            _a2 = _dependencyModel.CreateElement("a2", "eta", _a.Id);
            Assert.AreNotEqual(0, _a2.Id);
            Assert.AreEqual(0, _a2.Order);
            Assert.AreEqual("eta", _a2.Type);
            Assert.AreEqual("a2", _a2.Name);
            Assert.AreEqual("a.a2", _a2.Fullname);
            Assert.AreEqual(3, _dependencyModel.ElementCount);
            Assert.AreEqual(_a2, _dependencyModel.GetElementByFullname("a.a2"));

            _b = _dependencyModel.CreateElement("b", "", null);
            Assert.AreNotEqual(0, _b.Id);
            Assert.AreEqual(0, _b.Order);
            Assert.AreEqual("", _b.Type);
            Assert.AreEqual("b", _b.Name);
            Assert.AreEqual("b", _b.Fullname);
            Assert.AreEqual(4, _dependencyModel.ElementCount);
            Assert.AreEqual(_b, _dependencyModel.GetElementByFullname("b"));

            _b1 = _dependencyModel.CreateElement("b1", "etb", _b.Id);
            Assert.AreNotEqual(0, _b1.Id);
            Assert.AreEqual(0, _b1.Order);
            Assert.AreEqual("etb", _b1.Type);
            Assert.AreEqual("b1", _b1.Name);
            Assert.AreEqual("b.b1", _b1.Fullname);
            Assert.AreEqual(5, _dependencyModel.ElementCount);
            Assert.AreEqual(_b1, _dependencyModel.GetElementByFullname("b.b1"));

            _b2 = _dependencyModel.CreateElement("b2", "etb", _b.Id);
            Assert.AreNotEqual(0, _b2.Id);
            Assert.AreEqual(0, _b2.Order);
            Assert.AreEqual("etb", _b2.Type);
            Assert.AreEqual("b2", _b2.Name);
            Assert.AreEqual("b.b2", _b2.Fullname);
            Assert.AreEqual(6, _dependencyModel.ElementCount);
            Assert.AreEqual(_b2, _dependencyModel.GetElementByFullname("b.b2"));

            _c = _dependencyModel.CreateElement("c", "", null);
            Assert.AreNotEqual(0, _c.Id);
            Assert.AreEqual(0, _c.Order);
            Assert.AreEqual("", _c.Type);
            Assert.AreEqual("c", _c.Name);
            Assert.AreEqual("c", _c.Fullname);
            Assert.AreEqual(7, _dependencyModel.ElementCount);
            Assert.AreEqual(_c, _dependencyModel.GetElementByFullname("c"));

            _c1 = _dependencyModel.CreateElement("c1", "etc", _c.Id);
            Assert.AreNotEqual(0, _c1.Id);
            Assert.AreEqual(0, _c1.Order);
            Assert.AreEqual("etc", _c1.Type);
            Assert.AreEqual("c1", _c1.Name);
            Assert.AreEqual("c.c1", _c1.Fullname);
            Assert.AreEqual(8, _dependencyModel.ElementCount);
            Assert.AreEqual(_c1, _dependencyModel.GetElementByFullname("c.c1"));

            _c2 = _dependencyModel.CreateElement("c2", "etc", _c.Id);
            Assert.AreNotEqual(0, _c2.Id);
            Assert.AreEqual(0, _c2.Order);
            Assert.AreEqual("etc", _c2.Type);
            Assert.AreEqual("c2", _c2.Name);
            Assert.AreEqual("c.c2", _c2.Fullname);
            Assert.AreEqual(9, _dependencyModel.ElementCount);
            Assert.AreEqual(_c2, _dependencyModel.GetElementByFullname("c.c2"));
        }

        [TestMethod]
        public void CreateElementWhenAlreadyExistsTest()
        {
            Assert.AreEqual(0, _dependencyModel.ElementCount);

            _a = _dependencyModel.CreateElement("a", "", null);
            Assert.AreNotEqual(0, _a.Id);
            Assert.AreEqual(0, _a.Order);
            Assert.AreEqual("", _a.Type);
            Assert.AreEqual("a", _a.Name);
            Assert.AreEqual("a", _a.Fullname);
            Assert.AreEqual(1, _dependencyModel.ElementCount);
            Assert.AreEqual(_a, _dependencyModel.GetElementByFullname("a"));

            _a1 = _dependencyModel.CreateElement("a1", "eta", _a.Id);
            Assert.AreNotEqual(0, _a1.Id);
            Assert.AreEqual(0, _a1.Order);
            Assert.AreEqual("eta", _a1.Type);
            Assert.AreEqual("a1", _a1.Name);
            Assert.AreEqual("a.a1", _a1.Fullname);
            Assert.AreEqual(2, _dependencyModel.ElementCount);
            Assert.AreEqual(_a1, _dependencyModel.GetElementByFullname("a.a1"));

            Assert.AreEqual(_a, _dependencyModel.CreateElement("a", "", null));
            Assert.AreEqual(_a1, _dependencyModel.CreateElement("a1", "eta", _a.Id));
        }

        [TestMethod]
        public void AddElementTest()
        {
            Assert.AreEqual(0, _dependencyModel.ElementCount);

            _a = _dependencyModel.AddElement(11, "a", "", 1, false, null);
            Assert.AreEqual(11, _a.Id);
            Assert.AreEqual(1, _a.Order);
            Assert.AreEqual("", _a.Type);
            Assert.AreEqual("a", _a.Name);
            Assert.AreEqual("a", _a.Fullname);
            Assert.AreEqual(1, _dependencyModel.ElementCount);
            Assert.AreEqual(_a, _dependencyModel.GetElementByFullname("a"));
            Assert.AreEqual(_a, _dependencyModel.GetElementById(11));

            _a1 = _dependencyModel.AddElement(12, "a1", "eta", 2, false, _a.Id);
            Assert.AreEqual(12, _a1.Id);
            Assert.AreEqual(2, _a1.Order);
            Assert.AreEqual("eta", _a1.Type);
            Assert.AreEqual("a1", _a1.Name);
            Assert.AreEqual("a.a1", _a1.Fullname);
            Assert.AreEqual(2, _dependencyModel.ElementCount);
            Assert.AreEqual(_a1, _dependencyModel.GetElementByFullname("a.a1"));
            Assert.AreEqual(_a1, _dependencyModel.GetElementById(12));

            _a2 = _dependencyModel.AddElement(13, "a2", "eta", 3, false, _a.Id);
            Assert.AreEqual(13, _a2.Id);
            Assert.AreEqual(3, _a2.Order);
            Assert.AreEqual("eta", _a2.Type);
            Assert.AreEqual("a2", _a2.Name);
            Assert.AreEqual("a.a2", _a2.Fullname);
            Assert.AreEqual(3, _dependencyModel.ElementCount);
            Assert.AreEqual(_a2, _dependencyModel.GetElementByFullname("a.a2"));
            Assert.AreEqual(_a2, _dependencyModel.GetElementById(13));

            _b = _dependencyModel.AddElement(14, "b", "", 4, false, null);
            Assert.AreEqual(14, _b.Id);
            Assert.AreEqual(4, _b.Order);
            Assert.AreEqual("", _b.Type);
            Assert.AreEqual("b", _b.Name);
            Assert.AreEqual("b", _b.Fullname);
            Assert.AreEqual(4, _dependencyModel.ElementCount);
            Assert.AreEqual(_b, _dependencyModel.GetElementByFullname("b"));
            Assert.AreEqual(_b, _dependencyModel.GetElementById(14));

            _b1 = _dependencyModel.AddElement(15, "b1", "etb", 5, false, _b.Id);
            Assert.AreEqual(15, _b1.Id);
            Assert.AreEqual(5, _b1.Order);
            Assert.AreEqual("etb", _b1.Type);
            Assert.AreEqual("b1", _b1.Name);
            Assert.AreEqual("b.b1", _b1.Fullname);
            Assert.AreEqual(5, _dependencyModel.ElementCount);
            Assert.AreEqual(_b1, _dependencyModel.GetElementByFullname("b.b1"));
            Assert.AreEqual(_b1, _dependencyModel.GetElementById(15));

            _b2 = _dependencyModel.AddElement(16, "b2", "etb", 6, false, _b.Id);
            Assert.AreEqual(16, _b2.Id);
            Assert.AreEqual(6, _b2.Order);
            Assert.AreEqual("etb", _b2.Type);
            Assert.AreEqual("b2", _b2.Name);
            Assert.AreEqual("b.b2", _b2.Fullname);
            Assert.AreEqual(6, _dependencyModel.ElementCount);
            Assert.AreEqual(_b2, _dependencyModel.GetElementByFullname("b.b2"));
            Assert.AreEqual(_b2, _dependencyModel.GetElementById(16));

            _c = _dependencyModel.AddElement(17, "c", "", 7, false, null);
            Assert.AreEqual(17, _c.Id);
            Assert.AreEqual(7, _c.Order);
            Assert.AreEqual("", _c.Type);
            Assert.AreEqual("c", _c.Name);
            Assert.AreEqual("c", _c.Fullname);
            Assert.AreEqual(7, _dependencyModel.ElementCount);
            Assert.AreEqual(_c, _dependencyModel.GetElementByFullname("c"));
            Assert.AreEqual(_c, _dependencyModel.GetElementById(17));

            _c1 = _dependencyModel.AddElement(18, "c1", "etc", 8, false, _c.Id);

            Assert.AreEqual(18, _c1.Id);
            Assert.AreEqual(8, _c1.Order);
            Assert.AreEqual("etc", _c1.Type);
            Assert.AreEqual("c1", _c1.Name);
            Assert.AreEqual("c.c1", _c1.Fullname);
            Assert.AreEqual(8, _dependencyModel.ElementCount);
            Assert.AreEqual(_c1, _dependencyModel.GetElementByFullname("c.c1"));
            Assert.AreEqual(_c1, _dependencyModel.GetElementById(18));

            _c2 = _dependencyModel.AddElement(19, "c2", "etc", 9, false, _c.Id);
            Assert.AreEqual(19, _c2.Id);
            Assert.AreEqual(9, _c2.Order);
            Assert.AreEqual("etc", _c2.Type);
            Assert.AreEqual("c2", _c2.Name);
            Assert.AreEqual("c.c2", _c2.Fullname);
            Assert.AreEqual(9, _dependencyModel.ElementCount);
            Assert.AreEqual(_c2, _dependencyModel.GetElementByFullname("c.c2"));
            Assert.AreEqual(_c2, _dependencyModel.GetElementById(19));
        }


        [TestMethod]
        public void AddExistingElementWithNotExistingParentTest()
        {
            int notExistingParentId = 123;
            Assert.IsNull(_dependencyModel.AddElement(11, "a", "", 1, false, notExistingParentId));
        }

        [TestMethod]
        public void RemoveElementWithChildrenTest()
        {
            CreateHierarchy();

            Assert.AreEqual(9, _dependencyModel.ElementCount);
            Assert.AreEqual(_b, _dependencyModel.GetElementByFullname("b"));
            Assert.AreEqual(_b, _dependencyModel.GetElementById(14));
            Assert.AreEqual(_b1, _dependencyModel.GetElementByFullname("b.b1"));
            Assert.AreEqual(_b1, _dependencyModel.GetElementById(15));
            Assert.AreEqual(_b2, _dependencyModel.GetElementByFullname("b.b2"));
            Assert.AreEqual(_b2, _dependencyModel.GetElementById(16));
            _dependencyModel.RemoveElement(_b.Id);
            Assert.AreEqual(6, _dependencyModel.ElementCount);
            Assert.IsNull(_dependencyModel.GetElementByFullname("b"));
            Assert.IsNull(_dependencyModel.GetElementById(14));
            Assert.IsNull(_dependencyModel.GetElementByFullname("b.b1"));
            Assert.IsNull(_dependencyModel.GetElementById(15));
            Assert.IsNull(_dependencyModel.GetElementByFullname("b.b2"));
            Assert.IsNull(_dependencyModel.GetElementById(16));
        }

        [TestMethod]
        public void RemoveElementWithoutChildrenTest()
        {
            CreateHierarchy();

            Assert.AreEqual(9, _dependencyModel.ElementCount);
            Assert.AreEqual(_a2, _dependencyModel.GetElementByFullname("a.a2"));
            Assert.AreEqual(_a2, _dependencyModel.GetElementById(13));
            _dependencyModel.RemoveElement(_a2.Id);
            Assert.AreEqual(8, _dependencyModel.ElementCount);
            Assert.IsNull(_dependencyModel.GetElementByFullname("a.a2"));
            Assert.IsNull(_dependencyModel.GetElementById(13));
        }

        [TestMethod]
        public void AddingRelationsIncreasesWeight()
        {
            CreateHierarchy();

            _dependencyModel.AddRelation(_a1.Id, _b1.Id, "ra", 1000);
            Assert.AreEqual(1000, _dependencyModel.GetDependencyWeight(_a.Id, _b.Id));
            _dependencyModel.AddRelation(_a2.Id, _b1.Id, "ra", 200);
            Assert.AreEqual(1200, _dependencyModel.GetDependencyWeight(_a.Id, _b.Id));
            _dependencyModel.AddRelation(_a1.Id, _b2.Id, "ra", 30);
            Assert.AreEqual(1230, _dependencyModel.GetDependencyWeight(_a.Id, _b.Id));
            _dependencyModel.AddRelation(_a2.Id, _b2.Id, "ra", 4);
            Assert.AreEqual(1234, _dependencyModel.GetDependencyWeight(_a.Id, _b.Id));
        }

        [TestMethod]
        public void RemovingRelationsDecreasesWeight()
        {
            CreateMatrix();

            IList<IDsmRelation> relations = _dependencyModel.FindRelations(_a, _b).OrderBy(x => x.Weight).ToList();

            _dependencyModel.RemoveRelation(relations[0]);
            Assert.AreEqual(1230, _dependencyModel.GetDependencyWeight(_a.Id, _b.Id));
            _dependencyModel.RemoveRelation(relations[1]);
            Assert.AreEqual(1200, _dependencyModel.GetDependencyWeight(_a.Id, _b.Id));
            _dependencyModel.RemoveRelation(relations[2]);
            Assert.AreEqual(1000, _dependencyModel.GetDependencyWeight(_a.Id, _b.Id));
            _dependencyModel.RemoveRelation(relations[3]);
            Assert.AreEqual(0, _dependencyModel.GetDependencyWeight(_a.Id, _b.Id));
        }


        [TestMethod]
        public void IsCyclicDependencyTest()
        {
            CreateHierarchy();

            AddRelationsBetweenAandB();
            AddRelationsBetweenAandC();
            Assert.IsFalse(_dependencyModel.IsCyclicDependency(_a.Id, _b.Id));
            Assert.IsFalse(_dependencyModel.IsCyclicDependency(_a.Id, _c.Id));

            AddRelationsBetweenBandA();
            Assert.IsTrue(_dependencyModel.IsCyclicDependency(_a.Id, _b.Id));
            Assert.IsFalse(_dependencyModel.IsCyclicDependency(_a.Id, _c.Id));

            AddRelationsBetweenCandA();
            Assert.IsTrue(_dependencyModel.IsCyclicDependency(_a.Id, _b.Id));
            Assert.IsTrue(_dependencyModel.IsCyclicDependency(_a.Id, _c.Id));
        }

        [TestMethod]
        public void FindRelationsTest()
        {
            CreateMatrix();

            IList<IDsmRelation> relations = _dependencyModel.FindRelations(_a, _b).OrderBy(x => x.Weight).ToList();
            Assert.AreEqual(4, relations.Count);

            Assert.AreEqual(4, relations[0].Weight);
            Assert.AreEqual(_a2.Id, relations[0].ConsumerId);
            Assert.AreEqual(_b2.Id, relations[0].ProviderId);

            Assert.AreEqual(30, relations[1].Weight);
            Assert.AreEqual(_a1.Id, relations[1].ConsumerId);
            Assert.AreEqual(_b2.Id, relations[1].ProviderId);

            Assert.AreEqual(200, relations[2].Weight);
            Assert.AreEqual(_a2.Id, relations[2].ConsumerId);
            Assert.AreEqual(_b1.Id, relations[2].ProviderId);

            Assert.AreEqual(1000, relations[3].Weight);
            Assert.AreEqual(_a1.Id, relations[3].ConsumerId);
            Assert.AreEqual(_b1.Id, relations[3].ProviderId);
        }

        [TestMethod]
        public void FindNoRelationsTest()
        {
            CreateMatrix();

            IList<IDsmRelation> relations = _dependencyModel.FindRelations(_b, _b).OrderBy(x => x.Weight).ToList();
            Assert.AreEqual(0, relations.Count);
        }


        [TestMethod]
        public void FindElementConsumerRelationsTest()
        {
            CreateMatrix();

            IList<IDsmRelation> relations = _dependencyModel.FindElementConsumerRelations(_b).OrderBy(x => x.Weight).ToList();
            Assert.AreEqual(4, relations.Count);

            Assert.AreEqual(4, relations[0].Weight);
            Assert.AreEqual(_a2.Id, relations[0].ConsumerId);
            Assert.AreEqual(_b2.Id, relations[0].ProviderId);

            Assert.AreEqual(30, relations[1].Weight);
            Assert.AreEqual(_a1.Id, relations[1].ConsumerId);
            Assert.AreEqual(_b2.Id, relations[1].ProviderId);

            Assert.AreEqual(200, relations[2].Weight);
            Assert.AreEqual(_a2.Id, relations[2].ConsumerId);
            Assert.AreEqual(_b1.Id, relations[2].ProviderId);

            Assert.AreEqual(1000, relations[3].Weight);
            Assert.AreEqual(_a1.Id, relations[3].ConsumerId);
            Assert.AreEqual(_b1.Id, relations[3].ProviderId);
        }

        [TestMethod]
        public void FindElementProviderRelationsTest()
        {
            CreateMatrix();

            IList<IDsmRelation> relations = _dependencyModel.FindElementProviderRelations(_a).OrderBy(x => x.Weight).ToList();
            Assert.AreEqual(5, relations.Count);

            Assert.AreEqual(4, relations[0].Weight);
            Assert.AreEqual(_a2.Id, relations[0].ConsumerId);
            Assert.AreEqual(_b2.Id, relations[0].ProviderId);

            Assert.AreEqual(5, relations[1].Weight);
            Assert.AreEqual(_a1.Id, relations[1].ConsumerId);
            Assert.AreEqual(_c2.Id, relations[1].ProviderId);

            Assert.AreEqual(30, relations[2].Weight);
            Assert.AreEqual(_a1.Id, relations[2].ConsumerId);
            Assert.AreEqual(_b2.Id, relations[2].ProviderId);

            Assert.AreEqual(200, relations[3].Weight);
            Assert.AreEqual(_a2.Id, relations[3].ConsumerId);
            Assert.AreEqual(_b1.Id, relations[3].ProviderId);

            Assert.AreEqual(1000, relations[4].Weight);
            Assert.AreEqual(_a1.Id, relations[4].ConsumerId);
            Assert.AreEqual(_b1.Id, relations[4].ProviderId);
        }

        [TestMethod]
        public void FindElementConsumersTest()
        {
            CreateMatrix();

            IList<IDsmElement> elements = _dependencyModel.FindElementConsumers(_b).OrderBy(x => x.Fullname).ToList();

            Assert.AreEqual(2, elements.Count);
            Assert.AreEqual(_a1, elements[0]);
            Assert.AreEqual(_a2, elements[1]);
        }

        [TestMethod]
        public void FindElementProvidersTest()
        {
            CreateMatrix();

            IList<IDsmElement> elements = _dependencyModel.FindElementProviders(_a).OrderBy(x => x.Fullname).ToList();

            Assert.AreEqual(3, elements.Count);
            Assert.AreEqual(_b1, elements[0]);
            Assert.AreEqual(_b2, elements[1]);
            Assert.AreEqual(_c2, elements[2]);
        }

        [TestMethod]
        public void TestElementSwap()
        {
            CreateHierarchy();

            Assert.AreEqual(2, _a.Children.Count);
            Assert.AreEqual(_a1, _a.Children[0]);
            Assert.AreEqual(_a2, _a.Children[1]);
            Assert.AreEqual(2, _a1.Order);
            Assert.AreEqual(3, _a2.Order);

            Assert.IsTrue(_dependencyModel.Swap(_a1, _a2));

            Assert.AreEqual(2, _a.Children.Count);
            Assert.AreEqual(_a2, _a.Children[0]);
            Assert.AreEqual(_a1, _a.Children[1]);
            Assert.AreEqual(3, _a1.Order);
            Assert.AreEqual(2, _a2.Order);
        }

        [TestMethod]
        public void RemoveRelationTest()
        {
            Assert.Inconclusive("Not implemented yet");
        }

        [TestMethod]
        public void RelationDensityTest()
        {
            Assert.Inconclusive("Not implemented yet");
        }

        [TestMethod]
        public void RemoveElementWithRelationsTest()
        {
            Assert.Inconclusive("Not implemented yet");
        }


        [TestMethod]
        public void RenameElementTest()
        {
            Assert.Inconclusive("Not implemented yet");
        }

        [TestMethod]
        public void MoveElementTest()
        {
            Assert.Inconclusive("Not implemented yet");
        }


        [TestMethod]
        public void PartitionElementTest()
        {
            Assert.Inconclusive("Not implemented yet");
        }

        [TestMethod]
        public void AssignElementOrderTest()
        {
            _a = _dependencyModel.AddElement(11, "a", "", 0, false, null);
            _a1 = _dependencyModel.AddElement(12, "a1", "eta", 0, false, _a.Id);
            _a2 = _dependencyModel.AddElement(13, "a2", "eta", 0, false, _a.Id);
            _b = _dependencyModel.AddElement(14, "b", "", 0, false, null);
            _b1 = _dependencyModel.AddElement(15, "b1", "etb", 0, false, _b.Id);
            _b2 = _dependencyModel.AddElement(16, "b2", "etb", 0, false, _b.Id);
            _c = _dependencyModel.AddElement(17, "c", "", 0, false, null);
            _c1 = _dependencyModel.AddElement(18, "c1", "etc", 0,false, _c.Id);
            _c2 = _dependencyModel.AddElement(19, "c2", "etc", 0, false, _c.Id);

            Assert.AreEqual(0, _a.Order);
            Assert.AreEqual(0, _a1.Order);
            Assert.AreEqual(0, _a2.Order);
            Assert.AreEqual(0, _b.Order);
            Assert.AreEqual(0, _b1.Order);
            Assert.AreEqual(0, _b2.Order);
            Assert.AreEqual(0, _c.Order);
            Assert.AreEqual(0, _c1.Order);
            Assert.AreEqual(0, _c2.Order);

            _dependencyModel.AssignElementOrder();

            Assert.AreEqual(1, _a.Order);
            Assert.AreEqual(2, _a1.Order);
            Assert.AreEqual(3, _a2.Order);
            Assert.AreEqual(4, _b.Order);
            Assert.AreEqual(5, _b1.Order);
            Assert.AreEqual(6, _b2.Order);
            Assert.AreEqual(7, _c.Order);
            Assert.AreEqual(8, _c1.Order);
            Assert.AreEqual(9, _c2.Order);

        }

        [TestMethod]
        public void SystemCycalityTest()
        {
            Assert.Inconclusive("Not implemented yet");
        }


        [TestMethod]
        public void HierarchicalCycalityTest()
        {
            Assert.Inconclusive("Not implemented yet");
        }

        private void CreateMatrix()
        {
            CreateHierarchy();
            AddRelationsBetweenAandB();
            AddRelationsBetweenAandC();
            AddRelationsBetweenBandA();
            AddRelationsBetweenCandA();
        }

        private void CreateHierarchy()
        {
            _a = _dependencyModel.AddElement(11, "a", "", 1, false, null);
            _a1 = _dependencyModel.AddElement(12, "a1", "eta", 2, false, _a.Id);
            _a2 = _dependencyModel.AddElement(13, "a2", "eta", 3, false, _a.Id);
            _b = _dependencyModel.AddElement(14, "b", "", 4, false, null);
            _b1 = _dependencyModel.AddElement(15, "b1", "etb", 5, false, _b.Id);
            _b2 = _dependencyModel.AddElement(16, "b2", "etb", 6, false, _b.Id);
            _c = _dependencyModel.AddElement(17, "c", "", 7, false, null);
            _c1 = _dependencyModel.AddElement(18, "c1", "etc", 8, false, _c.Id);
            _c2 = _dependencyModel.AddElement(19, "c2", "etc", 9, false, _c.Id);
            Assert.AreEqual(9, _dependencyModel.ElementCount);
        }

        private void AddRelationsBetweenAandB()
        {
            _dependencyModel.AddRelation(_a1.Id, _b1.Id, "ra", 1000);
            Assert.AreEqual(1000, _dependencyModel.GetDependencyWeight(_a.Id, _b.Id));
            _dependencyModel.AddRelation(_a2.Id, _b1.Id, "ra", 200);
            Assert.AreEqual(1200, _dependencyModel.GetDependencyWeight(_a.Id, _b.Id));
            _dependencyModel.AddRelation(_a1.Id, _b2.Id, "ra", 30);
            Assert.AreEqual(1230, _dependencyModel.GetDependencyWeight(_a.Id, _b.Id));
            _dependencyModel.AddRelation(_a2.Id, _b2.Id, "ra", 4);
            Assert.AreEqual(1234, _dependencyModel.GetDependencyWeight(_a.Id, _b.Id));
        }

        private void AddRelationsBetweenAandC()
        {
            _dependencyModel.AddRelation(_a1.Id, _c2.Id, "ra", 5);
            Assert.AreEqual(5, _dependencyModel.GetDependencyWeight(_a.Id, _c.Id));
        }

        private void AddRelationsBetweenBandA()
        {
            _dependencyModel.AddRelation(_b2.Id, _a1.Id, "rb", 1);
            Assert.AreEqual(1, _dependencyModel.GetDependencyWeight(_b.Id, _a.Id));
            _dependencyModel.AddRelation(_b2.Id, _a2.Id, "rb", 2);
            Assert.AreEqual(3, _dependencyModel.GetDependencyWeight(_b.Id, _a.Id));
        }

        private void AddRelationsBetweenCandA()
        {
            _dependencyModel.AddRelation(_c1.Id, _a2.Id, "rc", 4);
            Assert.AreEqual(4, _dependencyModel.GetDependencyWeight(_c.Id, _a.Id));
        }
    }
}
