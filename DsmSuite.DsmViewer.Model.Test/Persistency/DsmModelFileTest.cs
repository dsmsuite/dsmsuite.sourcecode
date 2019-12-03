using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Castle.Components.DictionaryAdapter;
using DsmSuite.DsmViewer.Model.Data;
using DsmSuite.DsmViewer.Model.Dependencies;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Model.Persistency;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.DsmViewer.Model.Test.Files
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
    public class DsmModelFileTest : IDsmModelFileCallback
    {
        private List<DsmElement> _rootElements = new List<DsmElement>();
        private List<DsmRelation> _relations = new List<DsmRelation>();

        [TestInitialize()]
        public void MyTestInitialize()
        {
        }

        [TestMethod]
        public void TestLoadModelUncompressed()
        {
            string filename = "Persistency/Uncompressed.dsm";

            DsmModelFile readModelFile = new DsmModelFile(filename, this);
            readModelFile.Load(null);
            Assert.IsFalse(readModelFile.IsCompressedFile());
            CheckModel();
        }

        [TestMethod]
        public void TestSaveModelUncompressed()
        {
            string actual = "Output.dsm";
            string expected = "Persistency/Uncompressed.dsm";

            CreateMatrix();

            DsmModelFile writtenModelFile = new DsmModelFile(actual, this);
            writtenModelFile.Save(false, null);
            Assert.IsFalse(writtenModelFile.IsCompressedFile());

            Assert.IsTrue(File.ReadAllBytes(actual).SequenceEqual(File.ReadAllBytes(expected)));
        }

        [TestMethod]
        public void TestSaveAndLoadBackModelCompressed()
        {
            string filename = "Persistency/Compressed.dsm";

            CreateMatrix();

            DsmModelFile writtenModelFile = new DsmModelFile(filename, this);
            writtenModelFile.Save(true, null);
            Assert.IsTrue(writtenModelFile.IsCompressedFile());

            ClearMatrix();

            DsmModelFile readModelFile = new DsmModelFile(filename, this);
            readModelFile.Load(null);
            Assert.IsTrue(readModelFile.IsCompressedFile());

            CheckModel();
        }

        private void CreateMatrix()
        {
            DsmElement a = new DsmElement(11, "a", "", 1, true);
            DsmElement a1 = new DsmElement(12, "a1", "eta", 2);
            DsmElement a2 = new DsmElement(13, "a2", "eta", 3);
            DsmElement b = new DsmElement(14, "b", "", 4);
            DsmElement b1 = new DsmElement(15, "b1", "etb", 5);
            DsmElement b2 = new DsmElement(16, "b2", "etb", 6);
            DsmElement c = new DsmElement(17, "c", "", 7);
            DsmElement c1 = new DsmElement(18, "c1", "etc", 8);
            DsmElement c2 = new DsmElement(19, "c2", "etc", 9);

            _rootElements.Add(a);
            a.AddChild(a1);
            a.AddChild(a2);
            _rootElements.Add(b);
            b.AddChild(b1);
            b.AddChild(b2);
            _rootElements.Add(c);
            c.AddChild(c1);
            c.AddChild(c2);

            _relations.Add(new DsmRelation(a1.Id, b1.Id, "ra", 1000));
            _relations.Add(new DsmRelation(a2.Id, b1.Id, "ra", 200));
            _relations.Add(new DsmRelation(a1.Id, b2.Id, "ra", 30));
            _relations.Add(new DsmRelation(a2.Id, b2.Id, "ra", 4));
            _relations.Add(new DsmRelation(a1.Id, c2.Id, "ra", 5));
            _relations.Add(new DsmRelation(b2.Id, a1.Id, "rb", 1));
            _relations.Add(new DsmRelation(b2.Id, a2.Id, "rb", 2));
            _relations.Add(new DsmRelation(c1.Id, a2.Id, "rc", 4));
        }

        private void ClearMatrix()
        {
            _rootElements.Clear();
            _relations.Clear();
        }

        private void CheckModel()
        {
            Assert.AreEqual(3, _rootElements.Count);

            IDsmElement a = _rootElements[0];
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

            IDsmElement b = _rootElements[1];
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

            IDsmElement c = _rootElements[2];
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

            Assert.AreEqual(a1.Id, _relations[0].ConsumerId);
            Assert.AreEqual(b1.Id, _relations[0].ProviderId);
            Assert.AreEqual("ra", _relations[0].Type);
            Assert.AreEqual(1000, _relations[0].Weight);

            Assert.AreEqual(a2.Id, _relations[1].ConsumerId);
            Assert.AreEqual(b1.Id, _relations[1].ProviderId);
            Assert.AreEqual("ra", _relations[1].Type);
            Assert.AreEqual(200, _relations[1].Weight);

            Assert.AreEqual(a1.Id, _relations[2].ConsumerId);
            Assert.AreEqual(b2.Id, _relations[2].ProviderId);
            Assert.AreEqual("ra", _relations[2].Type);
            Assert.AreEqual(30, _relations[2].Weight);

            Assert.AreEqual(a2.Id, _relations[3].ConsumerId);
            Assert.AreEqual(b2.Id, _relations[3].ProviderId);
            Assert.AreEqual("ra", _relations[3].Type);
            Assert.AreEqual(4, _relations[3].Weight);

            Assert.AreEqual(a1.Id, _relations[4].ConsumerId);
            Assert.AreEqual(c2.Id, _relations[4].ProviderId);
            Assert.AreEqual("ra", _relations[4].Type);
            Assert.AreEqual(5, _relations[4].Weight);

            Assert.AreEqual(b2.Id, _relations[5].ConsumerId);
            Assert.AreEqual(a1.Id, _relations[5].ProviderId);
            Assert.AreEqual("rb", _relations[5].Type);
            Assert.AreEqual(1, _relations[5].Weight);

            Assert.AreEqual(b2.Id, _relations[6].ConsumerId);
            Assert.AreEqual(a2.Id, _relations[6].ProviderId);
            Assert.AreEqual("rb", _relations[6].Type);
            Assert.AreEqual(2, _relations[6].Weight);

            Assert.AreEqual(c1.Id, _relations[7].ConsumerId);
            Assert.AreEqual(a2.Id, _relations[7].ProviderId);
            Assert.AreEqual("rc", _relations[7].Type);
            Assert.AreEqual(4, _relations[7].Weight);
        }

        public IDsmMetaDataItem ImportMetaDataItem(string groupName, string name, string value)
        {
            return null;
        }

        public IDsmElement ImportElement(int id, string name, string type, int order, bool expanded, int? parentId)
        {
            DsmElement element = new DsmElement(id, name, type, order, expanded);
            if (!parentId.HasValue)
            {
                _rootElements.Add(element);
            }
            else
            {
                foreach (DsmElement rootElement in _rootElements)
                {
                    if (rootElement.Id == parentId.Value)
                    {
                        rootElement.AddChild(element);
                    }
                }
            }
            return element;
        }

        public IDsmRelation ImportRelation(int consumerId, int providerId, string type, int weight)
        {
            DsmRelation relation = new DsmRelation(consumerId, providerId, type, weight);
            _relations.Add(relation);
            return relation;
        }

        public IList<string> GetMetaDataGroups()
        {
            return new List<string>();
        }

        public IList<IDsmMetaDataItem> GetMetaDataGroupItems(string groupName)
        {
            return new List<IDsmMetaDataItem>();
        }

        public IList<IDsmElement> GetRootElements()
        {
            return _rootElements.ToList<IDsmElement>();
        }

        public int GetElementCount()
        {
            int elementCount = _rootElements.Count;
            foreach (DsmElement rootElement in _rootElements)
            {
                elementCount += rootElement.Children.Count;
            }
            return elementCount;
        }

        public IList<IDsmRelation> GetRelations()
        {
            return _relations.ToList<IDsmRelation>();
        }

        public int GetRelationCount()
        {
            return _relations.Count;
        }
    }
}
