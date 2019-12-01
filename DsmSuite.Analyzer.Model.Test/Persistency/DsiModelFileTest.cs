using System;
using System.Collections.Generic;
using System.IO;
using DsmSuite.Analyzer.Model.Data;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Model.Persistency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DsmSuite.Analyzer.Model.Test.Persistency
{
    [TestClass]
    public class DsiModelFileTest : IDsiModelFileCallback
    {
        private readonly List<IElement> _elements = new List<IElement>();
        private readonly List<IRelation> _relations = new List<IRelation>();
        private readonly Dictionary<string, List<IMetaDataItem>> _metaData = new Dictionary<string, List<IMetaDataItem>>();

        [TestInitialize]
        public void TestInitialize()
        {
            _elements.Clear();
            _relations.Clear();
            _metaData.Clear();
        }

        [TestMethod]
        public void TestLoadDsiModeiFile()
        {
            DsiModelFile modelFile = new DsiModelFile(TestFile, this);
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
        public void TestSaveAndLoadDsiModeiFile()
        {
            _metaData["group1"] = new List<IMetaDataItem>
            {
                new MetaDataItem("item1", "value1"),
                new MetaDataItem("item2", "value2")
            };

            _metaData["group2"] = new List<IMetaDataItem>
            {
                new MetaDataItem("item3", "value3"),
                new MetaDataItem("item4", "value4")
            };

            _elements.Add(new Element(1, "a.a1", "elementtype1", "source1"));
            _elements.Add(new Element(2, "a.a2", "elementtype2", "source2"));
            _elements.Add(new Element(3, "b.b1", "elementtype3", "source3"));

            _relations.Add(new Relation(1, 2, "relationtype1", 100));
            _relations.Add(new Relation(2, 3, "relationtype2", 200));

            string outputFilename = "Output.dsi";
            DsiModelFile modelFile = new DsiModelFile(outputFilename, this);
            modelFile.Save(false, null);

            Assert.IsTrue(File.ReadAllBytes(outputFilename).SequenceEqual(File.ReadAllBytes(TestFile)));
        }

        public void ImportMetaDataItem(string groupName, IMetaDataItem metaDataItem)
        {
            if (!_metaData.ContainsKey(groupName))
            {
                _metaData[groupName] = new List<IMetaDataItem>();
            }
            _metaData[groupName].Add(metaDataItem);
        }


        public void ImportElement(IElement element)
        {
            _elements.Add(element);
        }

        public void ImportRelation(IRelation relation)
        {
            _relations.Add(relation);
        }

        public IEnumerable<string> GetMetaDataGroups()
        {
            return _metaData.Keys;
        }

        public IEnumerable<IMetaDataItem> GetMetaDataGroupItems(string groupName)
        {
            if (_metaData.ContainsKey(groupName))
            {
                return _metaData[groupName];
            }
            else
            {
                return new List<IMetaDataItem>();
            }
        }

        public IEnumerable<IElement> GetElements()
        {
            return _elements;
        }

        public IEnumerable<IRelation> GetRelations()
        {
            return _relations;
        }

        public static string TestFile
        {
            get
            {
                string testData = @"..\..\DsmSuite.Analyzer.Model.Test";
                string pathExecutingAssembly = AppDomain.CurrentDomain.BaseDirectory;
                string filename = "Test.dsi";
                return Path.GetFullPath(Path.Combine(pathExecutingAssembly, testData, filename));
            }
        }
    }
}
