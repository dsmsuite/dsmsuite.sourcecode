using System.Reflection;
using DsmSuite.DsmViewer.Builder;
using DsmSuite.DsmViewer.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.DsmBuilder.Test
{
    [TestClass]
    public class BuildIntegrationTest
    {
        [TestMethod]
        public void TestBuild()
        {
            BuilderSettings settings = new BuilderSettings
            {
                InputFilename = "Uncompressed.dsi",
                OutputFilename = "Uncompressed.dsm",
                CompressOutputFile = false
            };

            DsmModel model = new DsmModel("Test", Assembly.GetExecutingAssembly());
            Builder builder = new Builder(model, settings);
            builder.BuildModel();
            CheckModel(model, "On build model");
        }

        private void CheckModel(IDsmModel model, string message)
        {
            IElement a = model.GetElementByFullname("a");
            Assert.AreNotEqual(0, a.Id, message);
            Assert.AreNotEqual(0, a.Order, message);
            Assert.AreEqual("", a.Type, message);
            Assert.AreEqual("a", a.Name, message);
            Assert.AreEqual("a", a.Fullname, message);

            IElement a1 = model.GetElementByFullname("a.a1");
            Assert.AreNotEqual(0, a1.Id, message);
            Assert.AreNotEqual(0, a1.Order, message);
            Assert.AreEqual("eta", a1.Type, message);
            Assert.AreEqual("a1", a1.Name, message);
            Assert.AreEqual("a.a1", a1.Fullname, message);

            IElement a2 = model.GetElementByFullname("a.a2");
            Assert.AreNotEqual(0, a2.Id, message);
            Assert.AreNotEqual(0, a2.Order, message);
            Assert.AreEqual("eta", a2.Type, message);
            Assert.AreEqual("a2", a2.Name, message);
            Assert.AreEqual("a.a2", a2.Fullname, message);

            IElement b = model.GetElementByFullname("b");
            Assert.AreNotEqual(0, b.Id, message);
            Assert.AreNotEqual(0, b.Order, message);
            Assert.AreEqual("", b.Type, message);
            Assert.AreEqual("b", b.Name, message);
            Assert.AreEqual("b", b.Fullname, message);

            IElement b1 = model.GetElementByFullname("b.b1");
            Assert.AreNotEqual(0, b1.Id, message);
            Assert.AreNotEqual(0, b1.Order, message);
            Assert.AreEqual("etb", b1.Type, message);
            Assert.AreEqual("b1", b1.Name, message);
            Assert.AreEqual("b.b1", b1.Fullname, message);

            IElement b2 = model.GetElementByFullname("b.b2");
            Assert.AreNotEqual(0, b2.Id, message);
            Assert.AreNotEqual(0, b2.Order, message);
            Assert.AreEqual("etb", b2.Type, message);
            Assert.AreEqual("b2", b2.Name, message);
            Assert.AreEqual("b.b2", b2.Fullname, message);

            IElement c = model.GetElementByFullname("c");
            Assert.AreNotEqual(0, c.Id, message);
            Assert.AreNotEqual(0, c.Order, message);
            Assert.AreEqual("", c.Type, message);
            Assert.AreEqual("c", c.Name, message);
            Assert.AreEqual("c", c.Fullname, message);

            IElement c1 = model.GetElementByFullname("c.c1");
            Assert.AreNotEqual(0, c1.Id, message);
            Assert.AreNotEqual(0, c1.Order, message);
            Assert.AreEqual("etc", c1.Type, message);
            Assert.AreEqual("c1", c1.Name, message);
            Assert.AreEqual("c.c1", c1.Fullname, message);

            IElement c2 = model.GetElementByFullname("c.c2");
            Assert.AreNotEqual(0, c2.Id, message);
            Assert.AreNotEqual(0, c2.Order, message);
            Assert.AreEqual("etc", c2.Type, message);
            Assert.AreEqual("c2", c2.Name, message);
            Assert.AreEqual("c.c2", c2.Fullname, message);

            Assert.AreEqual(9, model.ElementCount);

            Assert.AreEqual(1000, model.GetDependencyWeight(a1, b1));
            Assert.AreEqual(200, model.GetDependencyWeight(a2, b1));
            Assert.AreEqual(30, model.GetDependencyWeight(a1, b2));
            Assert.AreEqual(4, model.GetDependencyWeight(a2, b2));
            Assert.AreEqual(5, model.GetDependencyWeight(a1, c2));
            Assert.AreEqual(1, model.GetDependencyWeight(b2, a1));
            Assert.AreEqual(2, model.GetDependencyWeight(b2, a2));
            Assert.AreEqual(4, model.GetDependencyWeight(c1, a2));
        }
    }
}
