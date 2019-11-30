using System.IO;
using System.Linq;
using DsmSuite.DsmViewer.Model.Dependencies;
using DsmSuite.DsmViewer.Model.Files.Dsm;
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
    public class DsmModelFileTest
    {

        [TestInitialize()]
        public void MyTestInitialize()
        {
        }

        [TestMethod]
        public void TestReadModelUncompressed()
        {
            string filename = "Files/Uncompressed.dsm";

            DependencyModel readModel = new DependencyModel();
            MetaData readMetaData = new MetaData();
            DsmModelFileReader readModelFile = new DsmModelFileReader(filename, readModel, readMetaData);
            readModelFile.ReadFile(null);
            Assert.IsFalse(readModelFile.IsCompressedFile());
            CheckModel(readModel, "On read model");
        }

        [TestMethod]
        public void TestWriteModelUncompressed()
        {
            string actual = "Output.dsm";
            string expected = "Files/Uncompressed.dsm";

            DependencyModel writtenModel = new DependencyModel();
            CreateMatrix(writtenModel);
            CheckModel(writtenModel, "On written model");

            MetaData writtenMetaData = new MetaData();

            DsmModelFileWriter writtenModelFile = new DsmModelFileWriter(actual, writtenModel, writtenMetaData);
            writtenModelFile.WriteFile(false, null);
            Assert.IsFalse(writtenModelFile.IsCompressedFile());
            Assert.AreEqual(9, writtenModel.ElementCount);

            Assert.IsTrue(File.ReadAllBytes(actual).SequenceEqual(File.ReadAllBytes(expected)));
        }

        [TestMethod]
        public void TestWriteAndReadBackModelCompressed()
        {
            string filename = "Compressed.dsm";

            DependencyModel writtenModel = new DependencyModel();
            CreateMatrix(writtenModel);
            CheckModel(writtenModel, "On written model");

            MetaData writtenMetaData = new MetaData();

            DsmModelFileWriter writtenModelFile = new DsmModelFileWriter(filename, writtenModel, writtenMetaData);
            writtenModelFile.WriteFile(true, null);
            Assert.IsTrue(writtenModelFile.IsCompressedFile());

            DependencyModel readModel = new DependencyModel();
            MetaData readMetaData = new MetaData();
            DsmModelFileReader readModelFile = new DsmModelFileReader(filename, readModel, readMetaData);
            readModelFile.ReadFile(null);
            Assert.IsTrue(readModelFile.IsCompressedFile());
            CheckModel(readModel, "On read model");
        }

        private void CreateMatrix(DependencyModel dependencyModel)
        {
            IElement a = dependencyModel.AddElement(11, "a", "", 1, true, null);
            IElement a1 = dependencyModel.AddElement(12, "a1", "eta", 2, false, a.Id);
            IElement a2 = dependencyModel.AddElement(13, "a2", "eta", 3, false, a.Id);
            IElement b = dependencyModel.AddElement(14, "b", "", 4, false, null);
            IElement b1 = dependencyModel.AddElement(15, "b1", "etb", 5, false, b.Id);
            IElement b2 = dependencyModel.AddElement(16, "b2", "etb", 6, false, b.Id);
            IElement c = dependencyModel.AddElement(17, "c", "", 7, false, null);
            IElement c1 = dependencyModel.AddElement(18, "c1", "etc", 8, false, c.Id);
            IElement c2 = dependencyModel.AddElement(19, "c2", "etc", 9, false, c.Id);

            dependencyModel.AssignElementOrder();

            dependencyModel.AddRelation(a1.Id, b1.Id, "ra", 1000);
            dependencyModel.AddRelation(a2.Id, b1.Id, "ra", 200);
            dependencyModel.AddRelation(a1.Id, b2.Id, "ra", 30);
            dependencyModel.AddRelation(a2.Id, b2.Id, "ra", 4);
            dependencyModel.AddRelation(a1.Id, c2.Id, "ra", 5);
            dependencyModel.AddRelation(b2.Id, a1.Id, "rb", 1);
            dependencyModel.AddRelation(b2.Id, a2.Id, "rb", 2);
            dependencyModel.AddRelation(c1.Id, a2.Id, "rc", 4);
        }

        private void CheckModel(DependencyModel dependencyModel, string message)
        {
            IElement a = dependencyModel.GetElementByFullname("a");
            Assert.AreEqual(11, a.Id, message);
            Assert.AreEqual(1, a.Order, message);
            Assert.AreEqual("", a.Type, message);
            Assert.AreEqual("a", a.Name, message);
            Assert.AreEqual("a", a.Fullname, message);

            IElement a1 = dependencyModel.GetElementByFullname("a.a1");
            Assert.AreEqual(12, a1.Id, message);
            Assert.AreEqual(2, a1.Order, message);
            Assert.AreEqual("eta", a1.Type, message);
            Assert.AreEqual("a1", a1.Name, message);
            Assert.AreEqual("a.a1", a1.Fullname, message);

            IElement a2 = dependencyModel.GetElementByFullname("a.a2");
            Assert.AreEqual(13, a2.Id, message);
            Assert.AreEqual(3, a2.Order, message);
            Assert.AreEqual("eta", a2.Type, message);
            Assert.AreEqual("a2", a2.Name, message);
            Assert.AreEqual("a.a2", a2.Fullname, message);

            IElement b = dependencyModel.GetElementByFullname("b");
            Assert.AreEqual(14, b.Id, message);
            Assert.AreEqual(4, b.Order, message);
            Assert.AreEqual("", b.Type, message);
            Assert.AreEqual("b", b.Name, message);
            Assert.AreEqual("b", b.Fullname, message);

            IElement b1 = dependencyModel.GetElementByFullname("b.b1");
            Assert.AreEqual(15, b1.Id, message);
            Assert.AreEqual(5, b1.Order, message);
            Assert.AreEqual("etb", b1.Type, message);
            Assert.AreEqual("b1", b1.Name, message);
            Assert.AreEqual("b.b1", b1.Fullname, message);

            IElement b2 = dependencyModel.GetElementByFullname("b.b2");
            Assert.AreEqual(16, b2.Id, message);
            Assert.AreEqual(6, b2.Order, message);
            Assert.AreEqual("etb", b2.Type, message);
            Assert.AreEqual("b2", b2.Name, message);
            Assert.AreEqual("b.b2", b2.Fullname, message);

            IElement c = dependencyModel.GetElementByFullname("c");
            Assert.AreEqual(17, c.Id, message);
            Assert.AreEqual(7, c.Order, message);
            Assert.AreEqual("", c.Type, message);
            Assert.AreEqual("c", c.Name, message);
            Assert.AreEqual("c", c.Fullname, message);

            IElement c1 = dependencyModel.GetElementByFullname("c.c1");
            Assert.AreEqual(18, c1.Id, message);
            Assert.AreEqual(8, c1.Order, message);
            Assert.AreEqual("etc", c1.Type, message);
            Assert.AreEqual("c1", c1.Name, message);
            Assert.AreEqual("c.c1", c1.Fullname, message);

            IElement c2 = dependencyModel.GetElementByFullname("c.c2");
            Assert.AreEqual(19, c2.Id, message);
            Assert.AreEqual(9, c2.Order, message);
            Assert.AreEqual("etc", c2.Type, message);
            Assert.AreEqual("c2", c2.Name, message);
            Assert.AreEqual("c.c2", c2.Fullname, message);

            Assert.AreEqual(9, dependencyModel.ElementCount);

            Assert.AreEqual(1000, dependencyModel.GetDependencyWeight(a1.Id, b1.Id));
            Assert.AreEqual(200, dependencyModel.GetDependencyWeight(a2.Id, b1.Id));
            Assert.AreEqual(30, dependencyModel.GetDependencyWeight(a1.Id, b2.Id));
            Assert.AreEqual(4, dependencyModel.GetDependencyWeight(a2.Id, b2.Id));
            Assert.AreEqual(5, dependencyModel.GetDependencyWeight(a1.Id, c2.Id));
            Assert.AreEqual(1, dependencyModel.GetDependencyWeight(b2.Id, a1.Id));
            Assert.AreEqual(2, dependencyModel.GetDependencyWeight(b2.Id, a2.Id));
            Assert.AreEqual(4, dependencyModel.GetDependencyWeight(c1.Id, a2.Id));
        }
    }
}
