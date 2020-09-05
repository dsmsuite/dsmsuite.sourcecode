using DsmSuite.DsmViewer.Model.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.DsmViewer.Model.Test.Core
{
    [TestClass]
    public class ElementTest
    {
        [TestMethod]
        public void WhenElementIsConstructedThenPropertiesAreSetAccordingArguments()
        {
            int elementId = 1;
            string elementName = "name1";
            string elementType = "file";
            DsmElement element = new DsmElement(elementId, elementName, elementType);
            Assert.AreEqual(elementId, element.Id);
            Assert.AreEqual(elementName, element.Name);
            Assert.AreEqual(elementType, element.Type);
            Assert.AreEqual(0, element.Order);
            Assert.AreEqual(elementName, element.Fullname);
        }

        [TestMethod]
        public void TestElementHierarchy()
        {
            int parentId = 1;
            string parentName = "parent";
            DsmElement parent = new DsmElement(parentId, parentName, "");
            Assert.AreEqual(null, parent.Parent);
            Assert.AreEqual(0, parent.Children.Count);

            int child1Id = 10;
            string child1Name = "child1";
            DsmElement child1 = new DsmElement(child1Id, child1Name, "");
            Assert.AreEqual(null, child1.Parent);

            parent.AddChild(child1);
            Assert.AreEqual("parent.child1", child1.Fullname);

            Assert.AreEqual(1, parent.Children.Count);
            Assert.AreEqual(child1, parent.Children[0]);
            Assert.AreEqual(parent, child1.Parent);
            Assert.IsTrue(child1.IsRecursiveChildOf(parent));

            int child2Id = 100;
            string child2Name = "child2";
            DsmElement child2 = new DsmElement(child2Id, child2Name, "");
            Assert.AreEqual(null, child2.Parent);

            parent.AddChild(child2);
            Assert.AreEqual("parent.child2", child2.Fullname);

            Assert.AreEqual(2, parent.Children.Count);
            Assert.AreEqual(child1, parent.Children[0]);
            Assert.AreEqual(child2, parent.Children[1]);
            Assert.AreEqual(parent, child2.Parent);
            Assert.IsTrue(child2.IsRecursiveChildOf(parent));

            int child3Id = 1000;
            string child3Name = "child3";
            DsmElement child3 = new DsmElement(child3Id, child3Name, "");
            Assert.AreEqual(null, child3.Parent);
            Assert.AreEqual(null, child3.Parent);

            parent.AddChild(child3);
            Assert.AreEqual("parent.child3", child3.Fullname);

            Assert.AreEqual(3, parent.Children.Count);
            Assert.AreEqual(child1, parent.Children[0]);
            Assert.AreEqual(child2, parent.Children[1]);
            Assert.AreEqual(child3, parent.Children[2]);
            Assert.AreEqual(parent, child3.Parent);
            Assert.IsTrue(child3.IsRecursiveChildOf(parent));

            parent.RemoveChild(child1);
            Assert.AreEqual(null, child1.Parent);
            Assert.AreEqual(2, parent.Children.Count);
            Assert.AreEqual(child2, parent.Children[0]);
            Assert.AreEqual(child3, parent.Children[1]);
            Assert.IsFalse(child1.IsRecursiveChildOf(parent));

            parent.RemoveChild(child2);
            Assert.AreEqual(null, child2.Parent);
            Assert.AreEqual(1, parent.Children.Count);
            Assert.AreEqual(child3, parent.Children[0]);
            Assert.IsFalse(child2.IsRecursiveChildOf(parent));

            parent.RemoveChild(child3);
            Assert.AreEqual(null, child3.Parent);
            Assert.AreEqual(0, parent.Children.Count);
            Assert.IsFalse(child3.IsRecursiveChildOf(parent));
        }
    }
}
