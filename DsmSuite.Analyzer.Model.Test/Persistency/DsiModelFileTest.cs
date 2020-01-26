using System.Collections.Generic;
using System.IO;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Model.Persistency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using DsmSuite.Common.Model.Core;
using DsmSuite.Common.Model.Interface;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Common.Model.Persistency;

namespace DsmSuite.Analyzer.Model.Test.Persistency
{
    [TestClass]
    public class DsiModelFileTest : IMetaDataModelFileCallback, IDsiElementModelFileCallback, IDsiRelationModelFileCallback
    {
        private readonly List<IDsiElement> _elements = new List<IDsiElement>();
        private readonly List<IDsiRelation> _relations = new List<IDsiRelation>();
        private readonly Dictionary<string, List<MetaDataItem>> _metaData = new Dictionary<string, List<MetaDataItem>>();

        [TestInitialize]
        public void TestInitialize()
        {
            _elements.Clear();
            _relations.Clear();
            _metaData.Clear();
        }

        [TestMethod]
        public void TestLoadModel()
        {
            string inputFilename = "DsmSuite.Analyzer.Model.Test.Input.dsi";

            DsiModelFile modelFile = new DsiModelFile(inputFilename, this, this, this);
            modelFile.Load(null);

            Assert.AreEqual(2, _metaData.Count);

            Assert.AreEqual(2, _metaData["group1"].Count);
            Assert.AreEqual("item1", _metaData["group1"][0].Name);
            Assert.AreEqual("value1", _metaData["group1"][0].Value);
            Assert.AreEqual("item2", _metaData["group1"][1].Name);
            Assert.AreEqual("value2", _metaData["group1"][1].Value);

            Assert.AreEqual(2, _metaData["group2"].Count);
            Assert.AreEqual("item3", _metaData["group2"][0].Name);
            Assert.AreEqual("value3", _metaData["group2"][0].Value);
            Assert.AreEqual("item4", _metaData["group2"][1].Name);
            Assert.AreEqual("value4", _metaData["group2"][1].Value);

            Assert.AreEqual(3, _elements.Count);

            Assert.AreEqual(1, _elements[0].Id);
            Assert.AreEqual("a.a1", _elements[0].Name);
            Assert.AreEqual("elementtype1", _elements[0].Type);
            Assert.AreEqual("source1", _elements[0].Source);

            Assert.AreEqual(2, _elements[1].Id);
            Assert.AreEqual("a.a2", _elements[1].Name);
            Assert.AreEqual("elementtype2", _elements[1].Type);
            Assert.AreEqual("source2", _elements[1].Source);

            Assert.AreEqual(3, _elements[2].Id);
            Assert.AreEqual("b.b1", _elements[2].Name);
            Assert.AreEqual("elementtype3", _elements[2].Type);
            Assert.AreEqual("source3", _elements[2].Source);

            Assert.AreEqual(2, _relations.Count);

            Assert.AreEqual(1, _relations[0].ConsumerId);
            Assert.AreEqual(2, _relations[0].ProviderId);
            Assert.AreEqual("relationtype1", _relations[0].Type);
            Assert.AreEqual(100, _relations[0].Weight);

            Assert.AreEqual(2, _relations[1].ConsumerId);
            Assert.AreEqual(3, _relations[1].ProviderId);
            Assert.AreEqual("relationtype2", _relations[1].Type);
            Assert.AreEqual(200, _relations[1].Weight);
        }

        [TestMethod]
        public void TestSaveModel()
        {
            string inputFilename = "DsmSuite.Analyzer.Model.Test.Input.dsi";
            string outputFilename = "DsmSuite.Analyzer.Model.Test.Output.dsi";

            FillModelData();

            DsiModelFile modelFile = new DsiModelFile(outputFilename, this, this, this);
            modelFile.Save(false, null);

            Assert.IsTrue(File.ReadAllBytes(outputFilename).SequenceEqual(File.ReadAllBytes(inputFilename)));
        }

        private void FillModelData()
        {
            _metaData["group1"] = new List<MetaDataItem>
            {
                new MetaDataItem("item1", "value1"),
                new MetaDataItem("item2", "value2")
            };

            _metaData["group2"] = new List<MetaDataItem>
            {
                new MetaDataItem("item3", "value3"),
                new MetaDataItem("item4", "value4")
            };

            _elements.Add(new DsiElement(1, "a.a1", "elementtype1", "source1"));
            _elements.Add(new DsiElement(2, "a.a2", "elementtype2", "source2"));
            _elements.Add(new DsiElement(3, "b.b1", "elementtype3", "source3"));

            _relations.Add(new DsiRelation(1, 2, "relationtype1", 100));
            _relations.Add(new DsiRelation(2, 3, "relationtype2", 200));
        }

        public IMetaDataItem ImportMetaDataItem(string groupName, string itemName, string itemValue)
        {
            if (!_metaData.ContainsKey(groupName))
            {
                _metaData[groupName] = new List<MetaDataItem>();
            }

            MetaDataItem item = new MetaDataItem(itemName, itemValue);
            _metaData[groupName].Add(item);
            return item;
        }

        public IDsiElement ImportElement(int elementId, string name, string type, string source)
        {
            DsiElement element = new DsiElement(elementId, name, type, source);
            _elements.Add(element);
            return element;
        }

        public IDsiRelation ImportRelation(int consumerId, int providerId, string type, int weight)
        {
            DsiRelation relation = new DsiRelation(consumerId, providerId, type, weight);
            _relations.Add(relation);
            return relation;
        }

        public IEnumerable<string> GetExportedMetaDataGroups()
        {
            return _metaData.Keys;
        }

        public IEnumerable<IMetaDataItem> GetExportedMetaDataGroupItems(string group)
        {
            if (_metaData.ContainsKey(group))
            {
                return _metaData[group];
            }
            else
            {
                return new List<IMetaDataItem>();
            }
        }

        public IEnumerable<IDsiElement> GetElements()
        {
            return _elements;
        }

        public int GetElementCount()
        {
            return _elements.Count;
        }

        public IEnumerable<IDsiRelation> GetRelations()
        {
            return _relations;
        }

        public int GetRelationCount()
        {
            return _relations.Count;
        }
    }
}
