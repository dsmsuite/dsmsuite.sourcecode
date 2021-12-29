using System.Collections.Generic;
using System.IO;
using System.Linq;
using DsmSuite.DsmViewer.Model.Core;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Model.Persistency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.Common.Model.Core;
using DsmSuite.Common.Model.Interface;
using DsmSuite.Common.Model.Persistency;

namespace DsmSuite.DsmViewer.Model.Test.Persistency
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
    public class DsmModelFileTest : IMetaDataModelFileCallback, IDsmElementModelFileCallback, IDsmRelationModelFileCallback, IDsmActionModelFileCallback
    {
        private readonly DsmElement _rootElement = new DsmElement(0, "", "", null);
        private readonly List<DsmRelation> _relations = new List<DsmRelation>();
        private readonly Dictionary<string, List<IMetaDataItem>> _metaData = new Dictionary<string, List<IMetaDataItem>>();
        private readonly List<DsmAction> _actions = new List<DsmAction>();

        [TestInitialize()]
        public void MyTestInitialize()
        {
        }

        [TestMethod]
        public void TestLoadModel()
        {
            string inputFile = "DsmSuite.DsmViewer.Model.Test.Input.dsm";
            DsmModelFile readModelFile = new DsmModelFile(inputFile, this, this, this, this);
            readModelFile.Load(null);
            Assert.IsFalse(readModelFile.IsCompressedFile());

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

            Assert.AreEqual(3, _rootElement.Children.Count);

            IDsmElement a = _rootElement.Children[0];
            Assert.AreEqual(11, a.Id);
            Assert.AreEqual(1, a.Order);
            Assert.AreEqual("", a.Type);
            Assert.AreEqual("a", a.Name);
            Assert.AreEqual("a", a.Fullname);
            Assert.IsTrue(a.IsExpanded);

            Assert.AreEqual(2, a.Children.Count);
            IDsmElement a1 = a.Children[0];
            Assert.AreEqual(12, a1.Id);
            Assert.AreEqual(2, a1.Order);
            Assert.AreEqual("eta", a1.Type);
            Assert.AreEqual("a1", a1.Name);
            Assert.AreEqual("a.a1", a1.Fullname);
            Assert.IsFalse(a1.IsExpanded);

            IDsmElement a2 = a.Children[1];
            Assert.AreEqual(13, a2.Id);
            Assert.AreEqual(3, a2.Order);
            Assert.AreEqual("eta", a2.Type);
            Assert.AreEqual("a2", a2.Name);
            Assert.AreEqual("a.a2", a2.Fullname);
            Assert.IsFalse(a2.IsExpanded);

            IDsmElement b = _rootElement.Children[1];
            Assert.AreEqual(14, b.Id);
            Assert.AreEqual(4, b.Order);
            Assert.AreEqual("", b.Type);
            Assert.AreEqual("b", b.Name);
            Assert.AreEqual("b", b.Fullname);
            Assert.IsFalse(b.IsExpanded);

            Assert.AreEqual(2, b.Children.Count);
            IDsmElement b1 = b.Children[0];
            Assert.AreEqual(15, b1.Id);
            Assert.AreEqual(5, b1.Order);
            Assert.AreEqual("etb", b1.Type);
            Assert.AreEqual("b1", b1.Name);
            Assert.AreEqual("b.b1", b1.Fullname);
            Assert.IsFalse(b1.IsExpanded);

            IDsmElement b2 = b.Children[1];
            Assert.AreEqual(16, b2.Id);
            Assert.AreEqual(6, b2.Order);
            Assert.AreEqual("etb", b2.Type);
            Assert.AreEqual("b2", b2.Name);
            Assert.AreEqual("b.b2", b2.Fullname);
            Assert.IsFalse(b2.IsExpanded);

            IDsmElement c = _rootElement.Children[2];
            Assert.AreEqual(17, c.Id);
            Assert.AreEqual(7, c.Order);
            Assert.AreEqual("", c.Type);
            Assert.AreEqual("c", c.Name);
            Assert.AreEqual("c", c.Fullname);
            Assert.IsFalse(c.IsExpanded);

            Assert.AreEqual(2, b.Children.Count);
            IDsmElement c1 = c.Children[0];
            Assert.AreEqual(18, c1.Id);
            Assert.AreEqual(8, c1.Order);
            Assert.AreEqual("etc", c1.Type);
            Assert.AreEqual("c1", c1.Name);
            Assert.AreEqual("c.c1", c1.Fullname);
            Assert.IsFalse(c1.IsExpanded);

            IDsmElement c2 = c.Children[1];
            Assert.AreEqual(19, c2.Id);
            Assert.AreEqual(9, c2.Order);
            Assert.AreEqual("etc", c2.Type);
            Assert.AreEqual("c2", c2.Name);
            Assert.AreEqual("c.c2", c2.Fullname);
            Assert.IsFalse(c2.IsExpanded);

            Assert.AreEqual(91, _relations[0].Id);
            Assert.AreEqual(a1.Id, _relations[0].Consumer.Id);
            Assert.AreEqual(b1.Id, _relations[0].Provider.Id);
            Assert.AreEqual("ra", _relations[0].Type);
            Assert.AreEqual(1000, _relations[0].Weight);

            Assert.AreEqual(92, _relations[1].Id);
            Assert.AreEqual(a2.Id, _relations[1].Consumer.Id);
            Assert.AreEqual(b1.Id, _relations[1].Provider.Id);
            Assert.AreEqual("ra", _relations[1].Type);
            Assert.AreEqual(200, _relations[1].Weight);

            Assert.AreEqual(93, _relations[2].Id);
            Assert.AreEqual(a1.Id, _relations[2].Consumer.Id);
            Assert.AreEqual(b2.Id, _relations[2].Provider.Id);
            Assert.AreEqual("ra", _relations[2].Type);
            Assert.AreEqual(30, _relations[2].Weight);

            Assert.AreEqual(94, _relations[3].Id);
            Assert.AreEqual(a2.Id, _relations[3].Consumer.Id);
            Assert.AreEqual(b2.Id, _relations[3].Provider.Id);
            Assert.AreEqual("ra", _relations[3].Type);
            Assert.AreEqual(4, _relations[3].Weight);

            Assert.AreEqual(95, _relations[4].Id);
            Assert.AreEqual(a1.Id, _relations[4].Consumer.Id);
            Assert.AreEqual(c2.Id, _relations[4].Provider.Id);
            Assert.AreEqual("ra", _relations[4].Type);
            Assert.AreEqual(5, _relations[4].Weight);

            Assert.AreEqual(96, _relations[5].Id);
            Assert.AreEqual(b2.Id, _relations[5].Consumer.Id);
            Assert.AreEqual(a1.Id, _relations[5].Provider.Id);
            Assert.AreEqual("rb", _relations[5].Type);
            Assert.AreEqual(1, _relations[5].Weight);

            Assert.AreEqual(97, _relations[6].Id);
            Assert.AreEqual(b2.Id, _relations[6].Consumer.Id);
            Assert.AreEqual(a2.Id, _relations[6].Provider.Id);
            Assert.AreEqual("rb", _relations[6].Type);
            Assert.AreEqual(2, _relations[6].Weight);

            Assert.AreEqual(98, _relations[7].Id);
            Assert.AreEqual(c1.Id, _relations[7].Consumer.Id);
            Assert.AreEqual(a2.Id, _relations[7].Provider.Id);
            Assert.AreEqual("rc", _relations[7].Type);
            Assert.AreEqual(4, _relations[7].Weight);

            Assert.AreEqual(2, _actions.Count);

            Assert.AreEqual(1, _actions[0].Id);
            Assert.AreEqual("Action1", _actions[0].Type);
            Assert.AreEqual("value1a", _actions[0].Data["key1a"]);
            // Assert.AreEqual("value1b", _actions[0].Data["key1b"]);

            Assert.AreEqual(2, _actions[1].Id);
            Assert.AreEqual("Action2", _actions[1].Type);
            Assert.AreEqual("value2a", _actions[1].Data["key2a"]);
            Assert.AreEqual("value2b", _actions[1].Data["key2b"]);
            Assert.AreEqual("value2c", _actions[1].Data["key2c"]);
        }

        [TestMethod]
        public void TestSaveModel()
        {
            string inputFile = "DsmSuite.DsmViewer.Model.Test.Input.dsm";
            string outputFile = "DsmSuite.DsmViewer.Model.Test.Output.dsm";

            FillModelData();

            DsmModelFile writtenModelFile = new DsmModelFile(outputFile, this, this, this, this);
            writtenModelFile.Save(false, null);
            Assert.IsFalse(writtenModelFile.IsCompressedFile());

            Assert.IsTrue(File.ReadAllBytes(outputFile).SequenceEqual(File.ReadAllBytes(inputFile)));
        }

        private void FillModelData()
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

            DsmElement a = new DsmElement(11, "a", "", null, 1, true);
            DsmElement a1 = new DsmElement(12, "a1", "eta", null, 2);
            DsmElement a2 = new DsmElement(13, "a2", "eta", null, 3);
            DsmElement b = new DsmElement(14, "b", "", null, 4);
            DsmElement b1 = new DsmElement(15, "b1", "etb", null, 5);
            DsmElement b2 = new DsmElement(16, "b2", "etb", null, 6);
            DsmElement c = new DsmElement(17, "c", "", null, 7);
            DsmElement c1 = new DsmElement(18, "c1", "etc", null, 8);
            DsmElement c2 = new DsmElement(19, "c2", "etc", null, 9);

            _rootElement.AddChild(a);
            a.AddChild(a1);
            a.AddChild(a2);
            _rootElement.AddChild(b);
            b.AddChild(b1);
            b.AddChild(b2);
            _rootElement.AddChild(c);
            c.AddChild(c1);
            c.AddChild(c2);

            _relations.Add(new DsmRelation(91, a1, b1, "ra", 1000, null));
            _relations.Add(new DsmRelation(92, a2, b1, "ra", 200, null));
            _relations.Add(new DsmRelation(93, a1, b2, "ra", 30, null));
            _relations.Add(new DsmRelation(94, a2, b2, "ra", 4, null));
            _relations.Add(new DsmRelation(95, a1, c2, "ra", 5, null));
            _relations.Add(new DsmRelation(96, b2, a1, "rb", 1, null));
            _relations.Add(new DsmRelation(97, b2, a2, "rb", 2, null));
            _relations.Add(new DsmRelation(98, c1, a2, "rc", 4, null));

            Dictionary<string, string> data1 = new Dictionary<string, string>
            {
                ["key1a"] = "value1a",
                ["key1b"] = "value1b"
            };
            _actions.Add(new DsmAction(1, "Action1", data1));

            Dictionary<string, string> data2 = new Dictionary<string, string>
            {
                ["key2a"] = "value2a",
                ["key2b"] = "value2b",
                ["key2c"] = "value2c"
            };
            _actions.Add(new DsmAction(2, "Action2", data2));
        }

        public IMetaDataItem ImportMetaDataItem(string groupName, string name, string value)
        {
            if (!_metaData.ContainsKey(groupName))
            {
                _metaData[groupName] = new List<IMetaDataItem>();
            }

            MetaDataItem metaDataItem = new MetaDataItem(name, value);
            _metaData[groupName].Add(metaDataItem);
            return metaDataItem;
        }

        public IDsmAction ImportAction(int id, string type, IReadOnlyDictionary<string, string> data)
        {
            DsmAction action = new DsmAction(id, type, data);
            _actions.Add(action);
            return action;
        }

        public IDsmElement ImportElement(int id, string name, string type, IDictionary<string, string> properties, int order, bool expanded, int? parentId, bool deleted)
        {
            DsmElement element = new DsmElement(id, name, type, properties, order, expanded);
            if (!parentId.HasValue)
            {
                _rootElement.AddChild(element);
            }
            else
            {
                foreach (DsmElement rootElement in _rootElement.Children)
                {
                    if (rootElement.Id == parentId.Value)
                    {
                        rootElement.AddChild(element);
                    }
                }
            }
            return element;
        }

        public IDsmRelation ImportRelation(int relationId, IDsmElement consumer, IDsmElement provider, string type, int weight, IDictionary<string, string> properties, bool deleted)
        {
            DsmRelation relation = new DsmRelation(relationId, consumer, provider, type, weight, properties);
            _relations.Add(relation);
            return relation;
        }

        public IDsmElement FindElementById(int elementId)
        {
            IDsmElement found = null;
            FindElementById(_rootElement, elementId, ref found);
            return found;
        }

        private void FindElementById(IDsmElement element, int elementId, ref IDsmElement found)
        {
           if (element.Id == elementId)
           {
               found = element;
           }
           else
           {
               foreach(IDsmElement child in element.Children)
               {
                   FindElementById(child, elementId, ref found);
               }
           }
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

        public IEnumerable<IDsmAction> GetExportedActions()
        {
            return _actions;
        }

        public IDsmElement GetRootElement()
        {
            return _rootElement;
        }

        public int GetExportedElementCount()
        {
            int elementCount = _rootElement.Children.Count;
            foreach (DsmElement rootElement in _rootElement.Children)
            {
                elementCount += rootElement.Children.Count;
            }
            return elementCount;
        }

        public IEnumerable<IDsmRelation> GetExportedRelations()
        {
            return _relations;
        }

        public int GetExportedRelationCount()
        {
            return _relations.Count;
        }

        public int GetExportedActionCount()
        {
            return _actions.Count; 
        }
    }
}
