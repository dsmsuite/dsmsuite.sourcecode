using DsmSuite.DsmViewer.Model.Dependencies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.DsmViewer.Model.Test.Dependencies
{
    [TestClass]
    public class HierarchicalNameTest
    {
        [TestMethod]
        public void HierarchicalNameDefaultConstructorTest()
        {
            HierarchicalName name = new HierarchicalName();
            Assert.AreEqual("", name.FullName);
            Assert.AreEqual("", name.ParentName);
            Assert.AreEqual("", name.Name);
            Assert.AreEqual(1, name.ElementCount);
        }

        [TestMethod]
        public void HierarchicalNameSingleArgumentConstructorTest()
        {
            HierarchicalName name = new HierarchicalName("a.b.c");
            Assert.AreEqual("a.b.c", name.FullName);
            Assert.AreEqual("a.b", name.ParentName);
            Assert.AreEqual("c", name.Name);
            Assert.AreEqual(3, name.ElementCount);
            Assert.AreEqual("a", name.Elements[0]);
            Assert.AreEqual("b", name.Elements[1]);
            Assert.AreEqual("c", name.Elements[2]);
        }

        [TestMethod]
        public void HierarchicalNameDualArgumentConstructorTest()
        {
            HierarchicalName name = new HierarchicalName("a.b", "c");
            Assert.AreEqual("a.b.c", name.FullName);
            Assert.AreEqual("a.b", name.ParentName);
            Assert.AreEqual("c", name.Name);
            Assert.AreEqual(3, name.ElementCount);
            Assert.AreEqual("a", name.Elements[0]);
            Assert.AreEqual("b", name.Elements[1]);
            Assert.AreEqual("c", name.Elements[2]);
        }

        [TestMethod]
        public void HierarchicalNameAddToEmptyNameTest()
        {
            HierarchicalName name = new HierarchicalName();
            name.Add("a");
            Assert.AreEqual("a", name.FullName);
            Assert.AreEqual("", name.ParentName);
            Assert.AreEqual("a", name.Name);
            Assert.AreEqual(1, name.ElementCount);
            Assert.AreEqual("a", name.Elements[0]);
        }

        [TestMethod]
        public void HierarchicalNameAddToNonEmptyNameTest()
        {
            HierarchicalName name = new HierarchicalName("a.b");
            name.Add("c");
            Assert.AreEqual("a.b.c", name.FullName);
            Assert.AreEqual("a.b", name.ParentName);
            Assert.AreEqual("c", name.Name);
            Assert.AreEqual(3, name.ElementCount);
            Assert.AreEqual("a", name.Elements[0]);
            Assert.AreEqual("b", name.Elements[1]);
            Assert.AreEqual("c", name.Elements[2]);
        }
    }
}
