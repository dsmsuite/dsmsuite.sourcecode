using System.Reflection;
using DsmSuite.DsmViewer.Builder.Settings;
using DsmSuite.DsmViewer.Model.Core;
using DsmSuite.DsmViewer.Model.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.DsmViewer.Builder.Test.Application
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
            Builder.Application.Builder builder = new Builder.Application.Builder(model, settings);
            builder.BuildModel();

            IDsmElement a = model.GetElementByFullname("a");
            Assert.AreNotEqual(0, a.Id);
            Assert.AreNotEqual(0, a.Order);
            Assert.AreEqual("", a.Type);
            Assert.AreEqual("a", a.Name);
            Assert.AreEqual("a", a.Fullname);

            IDsmElement a1 = model.GetElementByFullname("a.a1");
            Assert.AreNotEqual(0, a1.Id);
            Assert.AreNotEqual(0, a1.Order);
            Assert.AreEqual("eta", a1.Type);
            Assert.AreEqual("a1", a1.Name);
            Assert.AreEqual("a.a1", a1.Fullname);

            IDsmElement a2 = model.GetElementByFullname("a.a2");
            Assert.AreNotEqual(0, a2.Id);
            Assert.AreNotEqual(0, a2.Order);
            Assert.AreEqual("eta", a2.Type);
            Assert.AreEqual("a2", a2.Name);
            Assert.AreEqual("a.a2", a2.Fullname);

            IDsmElement b = model.GetElementByFullname("b");
            Assert.AreNotEqual(0, b.Id);
            Assert.AreNotEqual(0, b.Order);
            Assert.AreEqual("", b.Type);
            Assert.AreEqual("b", b.Name);
            Assert.AreEqual("b", b.Fullname);

            IDsmElement b1 = model.GetElementByFullname("b.b1");
            Assert.AreNotEqual(0, b1.Id);
            Assert.AreNotEqual(0, b1.Order);
            Assert.AreEqual("etb", b1.Type);
            Assert.AreEqual("b1", b1.Name);
            Assert.AreEqual("b.b1", b1.Fullname);

            IDsmElement b2 = model.GetElementByFullname("b.b2");
            Assert.AreNotEqual(0, b2.Id);
            Assert.AreNotEqual(0, b2.Order);
            Assert.AreEqual("etb", b2.Type);
            Assert.AreEqual("b2", b2.Name);
            Assert.AreEqual("b.b2", b2.Fullname);

            IDsmElement c = model.GetElementByFullname("c");
            Assert.AreNotEqual(0, c.Id);
            Assert.AreNotEqual(0, c.Order);
            Assert.AreEqual("", c.Type);
            Assert.AreEqual("c", c.Name);
            Assert.AreEqual("c", c.Fullname);

            IDsmElement c1 = model.GetElementByFullname("c.c1");
            Assert.AreNotEqual(0, c1.Id);
            Assert.AreNotEqual(0, c1.Order);
            Assert.AreEqual("etc", c1.Type);
            Assert.AreEqual("c1", c1.Name);
            Assert.AreEqual("c.c1", c1.Fullname);

            IDsmElement c2 = model.GetElementByFullname("c.c2");
            Assert.AreNotEqual(0, c2.Id);
            Assert.AreNotEqual(0, c2.Order);
            Assert.AreEqual("etc", c2.Type);
            Assert.AreEqual("c2", c2.Name);
            Assert.AreEqual("c.c2", c2.Fullname);

            Assert.AreEqual(9, model.ElementCount);

            Assert.AreEqual(1000, model.GetDependencyWeight(a1.Id, b1.Id));
            Assert.AreEqual(200, model.GetDependencyWeight(a2.Id, b1.Id));
            Assert.AreEqual(30, model.GetDependencyWeight(a1.Id, b2.Id));
            Assert.AreEqual(4, model.GetDependencyWeight(a2.Id, b2.Id));
            Assert.AreEqual(5, model.GetDependencyWeight(a1.Id, c2.Id));
            Assert.AreEqual(1, model.GetDependencyWeight(b2.Id, a1.Id));
            Assert.AreEqual(2, model.GetDependencyWeight(b2.Id, a2.Id));
            Assert.AreEqual(4, model.GetDependencyWeight(c1.Id, a2.Id));
        }
    }
}
