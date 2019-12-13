//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using DsmSuite.DsmViewer.Model.Core;
//using DsmSuite.DsmViewer.Model.Interfaces;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace DsmSuite.DsmViewer.Model.Test.Core
//{
//    /// <summary>
//    /// Dependency matrix used for tests:
//    /// -System cycle between a and b
//    /// -Hierarchical cycle between a and c
//    /// 
//    ///        | a           | b           | c           |
//    ///        +------+------+------+------+------+------+
//    ///        | a1   | a2   | b1   | b2   | c1   | c2   |
//    /// --+----+------+------+------+------+------+------+
//    ///   | a1 |      |      |      | 2    |      |      |
//    /// a +----+------+------+------+------+------+------+
//    ///   | a2 |      |      |      | 3    |  4   |      |
//    /// -------+------+------+------+------+------+------+
//    ///   | b1 | 1000 | 200  |      |      |      |      |
//    /// b +----+------+------+------+------+------+------+
//    ///   | b2 |  30  | 4    |      |      |      |      |
//    /// --+----+------+------+------+------+------+------+
//    ///   | c1 |      |      |      |      |      |      |
//    /// c +----+------+------+------+------+------+------+
//    ///   | c2 |  5   |      |      |      |      |      |
//    /// --+----+------+------+------+------+------+------+
//    /// </summary>
//    [TestClass]
//    public class DsmModelTest
//    {
//        private DsmModel _model;
//        private IDsmElement _a;
//        private IDsmElement _a1;
//        private IDsmElement _a2;
//        private IDsmElement _b;
//        private IDsmElement _b1;
//        private IDsmElement _b2;
//        private IDsmElement _c;
//        private IDsmElement _c1;
//        private IDsmElement _c2;

//        [TestInitialize()]
//        public void MyTestInitialize()
//        {
//            _model = new DsmModel("test", Assembly.GetExecutingAssembly());
//        }

//        [TestMethod]
//        public void CreateElementTest()
//        {
//            Assert.AreEqual(0, _model.ElementCount);

//            _a = _model.AddElement("a", "", null);
//            Assert.AreEqual(1, _a.Id);
//            Assert.AreEqual(0, _a.Order);
//            Assert.AreEqual("", _a.Type);
//            Assert.AreEqual("a", _a.Name);
//            Assert.AreEqual("a", _a.Fullname);
//            Assert.AreEqual(1, _model.ElementCount);
//            Assert.AreEqual(_a, _model.GetElementByFullname("a"));

//            _a1 = _model.AddElement("a1", "eta", _a.Id);
//            Assert.AreEqual(2, _a1.Id);
//            Assert.AreEqual(0, _a1.Order);
//            Assert.AreEqual("eta", _a1.Type);
//            Assert.AreEqual("a1", _a1.Name);
//            Assert.AreEqual("a.a1", _a1.Fullname);
//            Assert.AreEqual(2, _model.ElementCount);
//            Assert.AreEqual(_a1, _model.GetElementByFullname("a.a1"));

//            _a2 = _model.AddElement("a2", "eta", _a.Id);
//            Assert.AreEqual(3, _a2.Id);
//            Assert.AreEqual(0, _a2.Order);
//            Assert.AreEqual("eta", _a2.Type);
//            Assert.AreEqual("a2", _a2.Name);
//            Assert.AreEqual("a.a2", _a2.Fullname);
//            Assert.AreEqual(3, _model.ElementCount);
//            Assert.AreEqual(_a2, _model.GetElementByFullname("a.a2"));

//            _b = _model.AddElement("b", "", null);
//            Assert.AreEqual(4, _b.Id);
//            Assert.AreEqual(0, _b.Order);
//            Assert.AreEqual("", _b.Type);
//            Assert.AreEqual("b", _b.Name);
//            Assert.AreEqual("b", _b.Fullname);
//            Assert.AreEqual(4, _model.ElementCount);
//            Assert.AreEqual(_b, _model.GetElementByFullname("b"));

//            _b1 = _model.AddElement("b1", "etb", _b.Id);
//            Assert.AreEqual(5, _b1.Id);
//            Assert.AreEqual(0, _b1.Order);
//            Assert.AreEqual("etb", _b1.Type);
//            Assert.AreEqual("b1", _b1.Name);
//            Assert.AreEqual("b.b1", _b1.Fullname);
//            Assert.AreEqual(5, _model.ElementCount);
//            Assert.AreEqual(_b1, _model.GetElementByFullname("b.b1"));

//            _b2 = _model.AddElement("b2", "etb", _b.Id);
//            Assert.AreEqual(6, _b2.Id);
//            Assert.AreEqual(0, _b2.Order);
//            Assert.AreEqual("etb", _b2.Type);
//            Assert.AreEqual("b2", _b2.Name);
//            Assert.AreEqual("b.b2", _b2.Fullname);
//            Assert.AreEqual(6, _model.ElementCount);
//            Assert.AreEqual(_b2, _model.GetElementByFullname("b.b2"));

//            _c = _model.AddElement("c", "", null);
//            Assert.AreEqual(7, _c.Id);
//            Assert.AreEqual(0, _c.Order);
//            Assert.AreEqual("", _c.Type);
//            Assert.AreEqual("c", _c.Name);
//            Assert.AreEqual("c", _c.Fullname);
//            Assert.AreEqual(7, _model.ElementCount);
//            Assert.AreEqual(_c, _model.GetElementByFullname("c"));

//            _c1 = _model.AddElement("c1", "etc", _c.Id);
//            Assert.AreEqual(8, _c1.Id);
//            Assert.AreEqual(0, _c1.Order);
//            Assert.AreEqual("etc", _c1.Type);
//            Assert.AreEqual("c1", _c1.Name);
//            Assert.AreEqual("c.c1", _c1.Fullname);
//            Assert.AreEqual(8, _model.ElementCount);
//            Assert.AreEqual(_c1, _model.GetElementByFullname("c.c1"));

//            _c2 = _model.AddElement("c2", "etc", _c.Id);
//            Assert.AreEqual(9, _c2.Id);
//            Assert.AreEqual(0, _c2.Order);
//            Assert.AreEqual("etc", _c2.Type);
//            Assert.AreEqual("c2", _c2.Name);
//            Assert.AreEqual("c.c2", _c2.Fullname);
//            Assert.AreEqual(9, _model.ElementCount);
//            Assert.AreEqual(_c2, _model.GetElementByFullname("c.c2"));
//        }

//        [TestMethod]
//        public void CreateElementWhenAlreadyExistsTest()
//        {
//            Assert.AreEqual(0, _model.ElementCount);

//            _a = _model.AddElement("a", "", null);
//            Assert.AreNotEqual(0, _a.Id);
//            Assert.AreEqual(0, _a.Order);
//            Assert.AreEqual("", _a.Type);
//            Assert.AreEqual("a", _a.Name);
//            Assert.AreEqual("a", _a.Fullname);
//            Assert.AreEqual(1, _model.ElementCount);
//            Assert.AreEqual(_a, _model.GetElementByFullname("a"));

//            _a1 = _model.AddElement("a1", "eta", _a.Id);
//            Assert.AreNotEqual(0, _a1.Id);
//            Assert.AreEqual(0, _a1.Order);
//            Assert.AreEqual("eta", _a1.Type);
//            Assert.AreEqual("a1", _a1.Name);
//            Assert.AreEqual("a.a1", _a1.Fullname);
//            Assert.AreEqual(2, _model.ElementCount);
//            Assert.AreEqual(_a1, _model.GetElementByFullname("a.a1"));

//            Assert.AreEqual(_a, _model.AddElement("a", "", null));
//            Assert.AreEqual(_a1, _model.AddElement("a1", "eta", _a.Id));
//        }

//        [TestMethod]
//        public void ImportElementTest()
//        {
//            Assert.AreEqual(0, _model.ElementCount);

//            _a = _model.ImportElement(11, "a", "", 1, false, null);
//            Assert.AreEqual(11, _a.Id);
//            Assert.AreEqual(1, _a.Order);
//            Assert.AreEqual("", _a.Type);
//            Assert.AreEqual("a", _a.Name);
//            Assert.AreEqual("a", _a.Fullname);
//            Assert.AreEqual(1, _model.ElementCount);
//            Assert.AreEqual(_a, _model.GetElementByFullname("a"));
//            Assert.AreEqual(_a, _model.GetElementById(11));

//            _a1 = _model.ImportElement(12, "a1", "eta", 2, false, _a.Id);
//            Assert.AreEqual(12, _a1.Id);
//            Assert.AreEqual(2, _a1.Order);
//            Assert.AreEqual("eta", _a1.Type);
//            Assert.AreEqual("a1", _a1.Name);
//            Assert.AreEqual("a.a1", _a1.Fullname);
//            Assert.AreEqual(2, _model.ElementCount);
//            Assert.AreEqual(_a1, _model.GetElementByFullname("a.a1"));
//            Assert.AreEqual(_a1, _model.GetElementById(12));

//            _a2 = _model.ImportElement(13, "a2", "eta", 3, false, _a.Id);
//            Assert.AreEqual(13, _a2.Id);
//            Assert.AreEqual(3, _a2.Order);
//            Assert.AreEqual("eta", _a2.Type);
//            Assert.AreEqual("a2", _a2.Name);
//            Assert.AreEqual("a.a2", _a2.Fullname);
//            Assert.AreEqual(3, _model.ElementCount);
//            Assert.AreEqual(_a2, _model.GetElementByFullname("a.a2"));
//            Assert.AreEqual(_a2, _model.GetElementById(13));

//            _b = _model.ImportElement(14, "b", "", 4, false, null);
//            Assert.AreEqual(14, _b.Id);
//            Assert.AreEqual(4, _b.Order);
//            Assert.AreEqual("", _b.Type);
//            Assert.AreEqual("b", _b.Name);
//            Assert.AreEqual("b", _b.Fullname);
//            Assert.AreEqual(4, _model.ElementCount);
//            Assert.AreEqual(_b, _model.GetElementByFullname("b"));
//            Assert.AreEqual(_b, _model.GetElementById(14));

//            _b1 = _model.ImportElement(15, "b1", "etb", 5, false, _b.Id);
//            Assert.AreEqual(15, _b1.Id);
//            Assert.AreEqual(5, _b1.Order);
//            Assert.AreEqual("etb", _b1.Type);
//            Assert.AreEqual("b1", _b1.Name);
//            Assert.AreEqual("b.b1", _b1.Fullname);
//            Assert.AreEqual(5, _model.ElementCount);
//            Assert.AreEqual(_b1, _model.GetElementByFullname("b.b1"));
//            Assert.AreEqual(_b1, _model.GetElementById(15));

//            _b2 = _model.ImportElement(16, "b2", "etb", 6, false, _b.Id);
//            Assert.AreEqual(16, _b2.Id);
//            Assert.AreEqual(6, _b2.Order);
//            Assert.AreEqual("etb", _b2.Type);
//            Assert.AreEqual("b2", _b2.Name);
//            Assert.AreEqual("b.b2", _b2.Fullname);
//            Assert.AreEqual(6, _model.ElementCount);
//            Assert.AreEqual(_b2, _model.GetElementByFullname("b.b2"));
//            Assert.AreEqual(_b2, _model.GetElementById(16));

//            _c = _model.ImportElement(17, "c", "", 7, false, null);
//            Assert.AreEqual(17, _c.Id);
//            Assert.AreEqual(7, _c.Order);
//            Assert.AreEqual("", _c.Type);
//            Assert.AreEqual("c", _c.Name);
//            Assert.AreEqual("c", _c.Fullname);
//            Assert.AreEqual(7, _model.ElementCount);
//            Assert.AreEqual(_c, _model.GetElementByFullname("c"));
//            Assert.AreEqual(_c, _model.GetElementById(17));

//            _c1 = _model.ImportElement(18, "c1", "etc", 8, false, _c.Id);
//            Assert.AreEqual(18, _c1.Id);
//            Assert.AreEqual(8, _c1.Order);
//            Assert.AreEqual("etc", _c1.Type);
//            Assert.AreEqual("c1", _c1.Name);
//            Assert.AreEqual("c.c1", _c1.Fullname);
//            Assert.AreEqual(8, _model.ElementCount);
//            Assert.AreEqual(_c1, _model.GetElementByFullname("c.c1"));
//            Assert.AreEqual(_c1, _model.GetElementById(18));

//            _c2 = _model.ImportElement(19, "c2", "etc", 9, false, _c.Id);
//            Assert.AreEqual(19, _c2.Id);
//            Assert.AreEqual(9, _c2.Order);
//            Assert.AreEqual("etc", _c2.Type);
//            Assert.AreEqual("c2", _c2.Name);
//            Assert.AreEqual("c.c2", _c2.Fullname);
//            Assert.AreEqual(9, _model.ElementCount);
//            Assert.AreEqual(_c2, _model.GetElementByFullname("c.c2"));
//            Assert.AreEqual(_c2, _model.GetElementById(19));
//        }


//        [TestMethod]
//        public void AddExistingElementWithNotExistingParentTest()
//        {
//            int notExistingParentId = 123;
//            Assert.IsNull(_model.ImportElement(11, "a", "", 1, false, notExistingParentId));
//        }

//        [TestMethod]
//        public void RemoveElementWithChildrenTest()
//        {
//            CreateHierarchy();

//            Assert.AreEqual(9, _model.ElementCount);
//            Assert.AreEqual(_b, _model.GetElementByFullname("b"));
//            Assert.AreEqual(_b, _model.GetElementById(14));
//            Assert.AreEqual(_b1, _model.GetElementByFullname("b.b1"));
//            Assert.AreEqual(_b1, _model.GetElementById(15));
//            Assert.AreEqual(_b2, _model.GetElementByFullname("b.b2"));
//            Assert.AreEqual(_b2, _model.GetElementById(16));
//            _model.RemoveElement(_b.Id);
//            Assert.AreEqual(6, _model.ElementCount);
//            Assert.IsNull(_model.GetElementByFullname("b"));
//            Assert.IsNull(_model.GetElementById(14));
//            Assert.IsNull(_model.GetElementByFullname("b.b1"));
//            Assert.IsNull(_model.GetElementById(15));
//            Assert.IsNull(_model.GetElementByFullname("b.b2"));
//            Assert.IsNull(_model.GetElementById(16));
//        }

//        [TestMethod]
//        public void RemoveElementWithoutChildrenTest()
//        {
//            CreateHierarchy();

//            Assert.AreEqual(9, _model.ElementCount);
//            Assert.AreEqual(_a2, _model.GetElementByFullname("a.a2"));
//            Assert.AreEqual(_a2, _model.GetElementById(13));
//            _model.RemoveElement(_a2.Id);
//            Assert.AreEqual(8, _model.ElementCount);
//            Assert.IsNull(_model.GetElementByFullname("a.a2"));
//            Assert.IsNull(_model.GetElementById(13));
//        }

//        [TestMethod]
//        public void AddingRelationsIncreasesWeight()
//        {
//            CreateHierarchy();

//            _model.AddRelation(_a1.Id, _b1.Id, "ra", 1000);
//            Assert.AreEqual(1000, _model.GetDependencyWeight(_a.Id, _b.Id));
//            _model.AddRelation(_a2.Id, _b1.Id, "ra", 200);
//            Assert.AreEqual(1200, _model.GetDependencyWeight(_a.Id, _b.Id));
//            _model.AddRelation(_a1.Id, _b2.Id, "ra", 30);
//            Assert.AreEqual(1230, _model.GetDependencyWeight(_a.Id, _b.Id));
//            _model.AddRelation(_a2.Id, _b2.Id, "ra", 4);
//            Assert.AreEqual(1234, _model.GetDependencyWeight(_a.Id, _b.Id));
//        }

//        [TestMethod]
//        public void RemovingRelationsDecreasesWeight()
//        {
//            CreateMatrix();

//            IList<IDsmRelation> relations = _model.FindRelations(_a, _b).OrderBy(x => x.Weight).ToList();

//            _model.RemoveRelation(relations[0].Id);
//            Assert.AreEqual(1230, _model.GetDependencyWeight(_a.Id, _b.Id));
//            _model.RemoveRelation(relations[1].Id);
//            Assert.AreEqual(1200, _model.GetDependencyWeight(_a.Id, _b.Id));
//            _model.RemoveRelation(relations[2].Id);
//            Assert.AreEqual(1000, _model.GetDependencyWeight(_a.Id, _b.Id));
//            _model.RemoveRelation(relations[3].Id);
//            Assert.AreEqual(0, _model.GetDependencyWeight(_a.Id, _b.Id));
//        }


//        [TestMethod]
//        public void IsCyclicDependencyTest()
//        {
//            CreateHierarchy();

//            AddRelationsBetweenAandB();
//            AddRelationsBetweenAandC();
//            Assert.IsFalse(_model.IsCyclicDependency(_a.Id, _b.Id));
//            Assert.IsFalse(_model.IsCyclicDependency(_a.Id, _c.Id));

//            AddRelationsBetweenBandA();
//            Assert.IsTrue(_model.IsCyclicDependency(_a.Id, _b.Id));
//            Assert.IsFalse(_model.IsCyclicDependency(_a.Id, _c.Id));

//            AddRelationsBetweenCandA();
//            Assert.IsTrue(_model.IsCyclicDependency(_a.Id, _b.Id));
//            Assert.IsTrue(_model.IsCyclicDependency(_a.Id, _c.Id));
//        }

//        [TestMethod]
//        public void FindRelationsTest()
//        {
//            CreateMatrix();

//            IList<IDsmRelation> relations = _model.FindRelations(_a, _b).OrderBy(x => x.Weight).ToList();
//            Assert.AreEqual(4, relations.Count);

//            Assert.AreEqual(4, relations[0].Weight);
//            Assert.AreEqual(_a2.Id, relations[0].ConsumerId);
//            Assert.AreEqual(_b2.Id, relations[0].ProviderId);

//            Assert.AreEqual(30, relations[1].Weight);
//            Assert.AreEqual(_a1.Id, relations[1].ConsumerId);
//            Assert.AreEqual(_b2.Id, relations[1].ProviderId);

//            Assert.AreEqual(200, relations[2].Weight);
//            Assert.AreEqual(_a2.Id, relations[2].ConsumerId);
//            Assert.AreEqual(_b1.Id, relations[2].ProviderId);

//            Assert.AreEqual(1000, relations[3].Weight);
//            Assert.AreEqual(_a1.Id, relations[3].ConsumerId);
//            Assert.AreEqual(_b1.Id, relations[3].ProviderId);
//        }

//        [TestMethod]
//        public void FindNoRelationsTest()
//        {
//            CreateMatrix();

//            IList<IDsmRelation> relations = _model.FindRelations(_b, _b).OrderBy(x => x.Weight).ToList();
//            Assert.AreEqual(0, relations.Count);
//        }


//        [TestMethod]
//        public void FindProviderRelationsTest()
//        {
//            CreateMatrix();

//            IList<IDsmRelation> relations = _model.FindProviderRelations(_b).OrderBy(x => x.Weight).ToList();
//            Assert.AreEqual(4, relations.Count);

//            Assert.AreEqual(4, relations[0].Weight);
//            Assert.AreEqual(_a2.Id, relations[0].ConsumerId);
//            Assert.AreEqual(_b2.Id, relations[0].ProviderId);

//            Assert.AreEqual(30, relations[1].Weight);
//            Assert.AreEqual(_a1.Id, relations[1].ConsumerId);
//            Assert.AreEqual(_b2.Id, relations[1].ProviderId);

//            Assert.AreEqual(200, relations[2].Weight);
//            Assert.AreEqual(_a2.Id, relations[2].ConsumerId);
//            Assert.AreEqual(_b1.Id, relations[2].ProviderId);

//            Assert.AreEqual(1000, relations[3].Weight);
//            Assert.AreEqual(_a1.Id, relations[3].ConsumerId);
//            Assert.AreEqual(_b1.Id, relations[3].ProviderId);
//        }

//        [TestMethod]
//        public void FindConsumerRelationsTest()
//        {
//            CreateMatrix();

//            IList<IDsmRelation> relations = _model.FindConsumerRelations(_a).OrderBy(x => x.Weight).ToList();
//            Assert.AreEqual(5, relations.Count);

//            Assert.AreEqual(4, relations[0].Weight);
//            Assert.AreEqual(_a2.Id, relations[0].ConsumerId);
//            Assert.AreEqual(_b2.Id, relations[0].ProviderId);

//            Assert.AreEqual(5, relations[1].Weight);
//            Assert.AreEqual(_a1.Id, relations[1].ConsumerId);
//            Assert.AreEqual(_c2.Id, relations[1].ProviderId);

//            Assert.AreEqual(30, relations[2].Weight);
//            Assert.AreEqual(_a1.Id, relations[2].ConsumerId);
//            Assert.AreEqual(_b2.Id, relations[2].ProviderId);

//            Assert.AreEqual(200, relations[3].Weight);
//            Assert.AreEqual(_a2.Id, relations[3].ConsumerId);
//            Assert.AreEqual(_b1.Id, relations[3].ProviderId);

//            Assert.AreEqual(1000, relations[4].Weight);
//            Assert.AreEqual(_a1.Id, relations[4].ConsumerId);
//            Assert.AreEqual(_b1.Id, relations[4].ProviderId);
//        }

//        [TestMethod]
//        public void TestElementSwap()
//        {
//            CreateHierarchy();

//            Assert.AreEqual(2, _a.Children.Count);
//            Assert.AreEqual(_a1, _a.Children[0]);
//            Assert.AreEqual(_a2, _a.Children[1]);
//            Assert.AreEqual(2, _a1.Order);
//            Assert.AreEqual(3, _a2.Order);

//            Assert.IsTrue(_model.Swap(_a1, _a2));

//            Assert.AreEqual(2, _a.Children.Count);
//            Assert.AreEqual(_a2, _a.Children[0]);
//            Assert.AreEqual(_a1, _a.Children[1]);
//            Assert.AreEqual(3, _a1.Order);
//            Assert.AreEqual(2, _a2.Order);
//        }

//        [TestMethod]
//        public void RemoveRelationTest()
//        {
//        }

//        [TestMethod]
//        public void RelationDensityTest()
//        {
//        }

//        [TestMethod]
//        public void RemoveElementWithRelationsTest()
//        {
//        }


//        [TestMethod]
//        public void EditElementTest()
//        {
//        }

//        [TestMethod]
//        public void MoveElementTest()
//        {
//        }


//        [TestMethod]
//        public void PartitionElementTest()
//        {
//        }

//        [TestMethod]
//        public void AssignElementOrderTest()
//        {
//            _a = _model.ImportElement(11, "a", "", 0, false, null);
//            _a1 = _model.ImportElement(12, "a1", "eta", 0, false, _a.Id);
//            _a2 = _model.ImportElement(13, "a2", "eta", 0, false, _a.Id);
//            _b = _model.ImportElement(14, "b", "", 0, false, null);
//            _b1 = _model.ImportElement(15, "b1", "etb", 0, false, _b.Id);
//            _b2 = _model.ImportElement(16, "b2", "etb", 0, false, _b.Id);
//            _c = _model.ImportElement(17, "c", "", 0, false, null);
//            _c1 = _model.ImportElement(18, "c1", "etc", 0,false, _c.Id);
//            _c2 = _model.ImportElement(19, "c2", "etc", 0, false, _c.Id);

//            Assert.AreEqual(0, _a.Order);
//            Assert.AreEqual(0, _a1.Order);
//            Assert.AreEqual(0, _a2.Order);
//            Assert.AreEqual(0, _b.Order);
//            Assert.AreEqual(0, _b1.Order);
//            Assert.AreEqual(0, _b2.Order);
//            Assert.AreEqual(0, _c.Order);
//            Assert.AreEqual(0, _c1.Order);
//            Assert.AreEqual(0, _c2.Order);

//            _model.AssignElementOrder();

//            Assert.AreEqual(1, _a.Order);
//            Assert.AreEqual(2, _a1.Order);
//            Assert.AreEqual(3, _a2.Order);
//            Assert.AreEqual(4, _b.Order);
//            Assert.AreEqual(5, _b1.Order);
//            Assert.AreEqual(6, _b2.Order);
//            Assert.AreEqual(7, _c.Order);
//            Assert.AreEqual(8, _c1.Order);
//            Assert.AreEqual(9, _c2.Order);

//        }

//        [TestMethod]
//        public void SystemCycalityTest()
//        {
//        }


//        [TestMethod]
//        public void HierarchicalCycalityTest()
//        {
//        }

//        private void CreateMatrix()
//        {
//            CreateHierarchy();
//            AddRelationsBetweenAandB();
//            AddRelationsBetweenAandC();
//            AddRelationsBetweenBandA();
//            AddRelationsBetweenCandA();
//        }

//        private void CreateHierarchy()
//        {
//            _a = _model.ImportElement(11, "a", "", 1, false, null);
//            _a1 = _model.ImportElement(12, "a1", "eta", 2, false, _a.Id);
//            _a2 = _model.ImportElement(13, "a2", "eta", 3, false, _a.Id);
//            _b = _model.ImportElement(14, "b", "", 4, false, null);
//            _b1 = _model.ImportElement(15, "b1", "etb", 5, false, _b.Id);
//            _b2 = _model.ImportElement(16, "b2", "etb", 6, false, _b.Id);
//            _c = _model.ImportElement(17, "c", "", 7, false, null);
//            _c1 = _model.ImportElement(18, "c1", "etc", 8, false, _c.Id);
//            _c2 = _model.ImportElement(19, "c2", "etc", 9, false, _c.Id);
//            Assert.AreEqual(9, _model.ElementCount);
//        }

//        private void AddRelationsBetweenAandB()
//        {
//            _model.AddRelation(_a1.Id, _b1.Id, "ra", 1000);
//            Assert.AreEqual(1000, _model.GetDependencyWeight(_a.Id, _b.Id));
//            _model.AddRelation(_a2.Id, _b1.Id, "ra", 200);
//            Assert.AreEqual(1200, _model.GetDependencyWeight(_a.Id, _b.Id));
//            _model.AddRelation(_a1.Id, _b2.Id, "ra", 30);
//            Assert.AreEqual(1230, _model.GetDependencyWeight(_a.Id, _b.Id));
//            _model.AddRelation(_a2.Id, _b2.Id, "ra", 4);
//            Assert.AreEqual(1234, _model.GetDependencyWeight(_a.Id, _b.Id));
//        }

//        private void AddRelationsBetweenAandC()
//        {
//            _model.AddRelation(_a1.Id, _c2.Id, "ra", 5);
//            Assert.AreEqual(5, _model.GetDependencyWeight(_a.Id, _c.Id));
//        }

//        private void AddRelationsBetweenBandA()
//        {
//            _model.AddRelation(_b2.Id, _a1.Id, "rb", 1);
//            Assert.AreEqual(1, _model.GetDependencyWeight(_b.Id, _a.Id));
//            _model.AddRelation(_b2.Id, _a2.Id, "rb", 2);
//            Assert.AreEqual(3, _model.GetDependencyWeight(_b.Id, _a.Id));
//        }

//        private void AddRelationsBetweenCandA()
//        {
//            _model.AddRelation(_c1.Id, _a2.Id, "rc", 4);
//            Assert.AreEqual(4, _model.GetDependencyWeight(_c.Id, _a.Id));
//        }
//    }
//}
