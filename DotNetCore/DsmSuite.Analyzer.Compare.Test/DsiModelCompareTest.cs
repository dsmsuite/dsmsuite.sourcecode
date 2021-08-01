using System.Collections.Generic;
using System.Reflection;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.Analyzer.CompareLib;

namespace DsmSuite.Analyzer.Compare.Test
{
    [TestClass]
    public class DsiModelCompareTest
    {
        private IDsiModel _oldModel;
        private IDsiModel _newModel;
        private DsiModelCompare _dsiModelCompare;
        private IEnumerable<IDsiModel> _allModels;
        private IEnumerable<IDsiModel> _onlyOldModel;
        private IEnumerable<IDsiModel> _onlyNewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _oldModel = new DsiModel("Test1", new List<string>(), Assembly.GetExecutingAssembly());
            _newModel = new DsiModel("Test2", new List<string>(), Assembly.GetExecutingAssembly());
            _dsiModelCompare = new DsiModelCompare(_oldModel, _newModel, null);
            _allModels = new [] {_oldModel, _newModel};
            _onlyOldModel = new [] { _oldModel };
            _onlyNewModel = new [] { _newModel };
        }
        
        [TestMethod]
        public void TestEmptyModelTheSame()
        {
            _dsiModelCompare.Compare();
            Assert.IsTrue(_dsiModelCompare.AreIdentical);
            Assert.AreEqual(0,_dsiModelCompare.AddedElementCount);
            Assert.AreEqual(0,_dsiModelCompare.RemovedElementCount);
            Assert.AreEqual(0, _dsiModelCompare.AddedRelationCount);
            Assert.AreEqual(0, _dsiModelCompare.RemovedRelationCount);
        }

        [TestMethod]
        public void TestNonEmptyModelTheSame()
        {
            AddElement(_allModels, "a.a1");
            AddElement(_allModels, "a.a2");
            AddElement(_allModels, "a.a3");

            AddRelation(_allModels, "a.a1", "a.a2");
            AddRelation(_allModels, "a.a2", "a.a3");

            _dsiModelCompare.Compare();
            Assert.IsTrue(_dsiModelCompare.AreIdentical);
            Assert.AreEqual(0, _dsiModelCompare.AddedElementCount);
            Assert.AreEqual(0, _dsiModelCompare.RemovedElementCount);
            Assert.AreEqual(0, _dsiModelCompare.AddedRelationCount);
            Assert.AreEqual(0, _dsiModelCompare.RemovedRelationCount);
        }

        [TestMethod]
        public void TestElementAdded()
        {
            AddElement(_allModels, "a.a1");
            AddElement(_allModels, "a.a2");
            AddElement(_onlyNewModel, "a.a3");

            _dsiModelCompare.Compare();
            Assert.IsFalse(_dsiModelCompare.AreIdentical);
            Assert.AreEqual(1, _dsiModelCompare.AddedElementCount);
            Assert.AreEqual(0, _dsiModelCompare.RemovedElementCount);
            Assert.AreEqual(0, _dsiModelCompare.AddedRelationCount);
            Assert.AreEqual(0, _dsiModelCompare.RemovedRelationCount);

            Assert.IsTrue(_dsiModelCompare.AddedElements.Contains("a.a3"));
        }

        [TestMethod]
        public void TestElementRemoved()
        {
            AddElement(_allModels, "a.a1");
            AddElement(_allModels, "a.a2");
            AddElement(_onlyOldModel, "a.a3");

            _dsiModelCompare.Compare();
            Assert.IsFalse(_dsiModelCompare.AreIdentical);
            Assert.AreEqual(0, _dsiModelCompare.AddedElementCount);
            Assert.AreEqual(1, _dsiModelCompare.RemovedElementCount);
            Assert.AreEqual(0, _dsiModelCompare.AddedRelationCount);
            Assert.AreEqual(0, _dsiModelCompare.RemovedRelationCount);

            Assert.IsTrue(_dsiModelCompare.RemovedElements.Contains("a.a3"));
        }

        [TestMethod]
        public void TestRelationAdded()
        {
            AddElement(_allModels, "a.a1");
            AddElement(_allModels, "a.a2");
            AddElement(_allModels, "a.a3");

            AddRelation(_allModels, "a.a1", "a.a2");
            AddRelation(_onlyNewModel, "a.a2", "a.a3");

            _dsiModelCompare.Compare();
            Assert.IsFalse(_dsiModelCompare.AreIdentical);
            Assert.AreEqual(0, _dsiModelCompare.AddedElementCount);
            Assert.AreEqual(0, _dsiModelCompare.RemovedElementCount);
            Assert.AreEqual(1, _dsiModelCompare.AddedRelationCount);
            Assert.AreEqual(0, _dsiModelCompare.RemovedRelationCount);

            Assert.IsTrue(_dsiModelCompare.AddedRelations.ContainsKey("a.a2"));
            Assert.AreEqual(1, _dsiModelCompare.AddedRelations["a.a2"].Count);
            Assert.IsTrue(_dsiModelCompare.AddedRelations["a.a2"].Contains("a.a3"));
        }

        [TestMethod]
        public void TestRelationRemoved()
        {
            AddElement(_allModels, "a.a1");
            AddElement(_allModels, "a.a2");
            AddElement(_allModels, "a.a3");

            AddRelation(_allModels, "a.a1", "a.a2");
            AddRelation(_onlyOldModel, "a.a2", "a.a3");

            _dsiModelCompare.Compare();
            Assert.IsFalse(_dsiModelCompare.AreIdentical);
            Assert.AreEqual(0, _dsiModelCompare.AddedElementCount);
            Assert.AreEqual(0, _dsiModelCompare.RemovedElementCount);
            Assert.AreEqual(0, _dsiModelCompare.AddedRelationCount);
            Assert.AreEqual(1, _dsiModelCompare.RemovedRelationCount);

            Assert.IsTrue(_dsiModelCompare.RemovedRelations.ContainsKey("a.a2"));
            Assert.AreEqual(1, _dsiModelCompare.RemovedRelations["a.a2"].Count);
            Assert.IsTrue(_dsiModelCompare.RemovedRelations["a.a2"].Contains("a.a3"));
        }

        private void AddElement(IEnumerable<IDsiModel> models, string name)
        {
            foreach(var model in models)
            {
                model.AddElement(name, "type", "");
            }
        }

        private void AddRelation(IEnumerable<IDsiModel> models, string consumer, string provider)
        {
            foreach (var model in models)
            {
                model.AddRelation(consumer, provider, "type", 1, "");
            }
        }
    }
}
