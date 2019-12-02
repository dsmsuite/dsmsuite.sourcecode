using DsmSuite.DsmViewer.Model.Dependencies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.DsmViewer.Model.Test.Data
{
    /// <summary>
    /// Summary description for MetaDataTest
    /// </summary>
    [TestClass]
    public class MetaDataTest
    {
        [TestMethod]
        public void TestAddMetaData()
        {
            string group1 = "group1";
            string group2 = "group3";

            string name1 = "name1";
            string name2 = "name2";
            string name3 = "name3";

            string value1 = "value1";
            string value2 = "value2";
            string value3 = "value3";
            MetaData metaData = new MetaData();
            Assert.AreEqual(0, metaData.GetGroups().Count);

            metaData.AddMetaData(group1, name1, value1);
            Assert.AreEqual(1, metaData.GetGroups().Count);
            Assert.AreEqual(group1, metaData.GetGroups()[0]);

            Assert.AreEqual(1, metaData.GetNames(group1).Count);
            Assert.AreEqual(name1, metaData.GetNames(group1)[0]);
            Assert.AreEqual(value1, metaData.GetValue(group1, name1));

            metaData.AddMetaData(group1, name2, value2);
            Assert.AreEqual(1, metaData.GetGroups().Count);
            Assert.AreEqual(group1, metaData.GetGroups()[0]);

            Assert.AreEqual(2, metaData.GetNames(group1).Count);
            Assert.AreEqual(name1, metaData.GetNames(group1)[0]);
            Assert.AreEqual(value1, metaData.GetValue(group1, name1));
            Assert.AreEqual(name2, metaData.GetNames(group1)[1]);
            Assert.AreEqual(value2, metaData.GetValue(group1, name2));

            metaData.AddMetaData(group2, name3, value3);
            Assert.AreEqual(2, metaData.GetGroups().Count);
            Assert.AreEqual(group1, metaData.GetGroups()[0]);
            Assert.AreEqual(group2, metaData.GetGroups()[1]);

            Assert.AreEqual(1, metaData.GetNames(group2).Count);
            Assert.AreEqual(value3, metaData.GetValue(group2, name3));
        }

        [TestMethod]
        public void TestUpdateMetaData()
        {
            string group1 = "group1";
            string name1 = "name1";

            string value1 = "value1";
            string value2 = "value2";
            MetaData metaData = new MetaData();
            Assert.AreEqual(0, metaData.GetGroups().Count);

            metaData.AddMetaData(group1, name1, value1);
            Assert.AreEqual(1, metaData.GetGroups().Count);
            Assert.AreEqual(group1, metaData.GetGroups()[0]);

            Assert.AreEqual(1, metaData.GetNames(group1).Count);
            Assert.AreEqual(name1, metaData.GetNames(group1)[0]);
            Assert.AreEqual(value1, metaData.GetValue(group1, name1));

            metaData.AddMetaData(group1, name1, value2);
            Assert.AreEqual(1, metaData.GetGroups().Count);
            Assert.AreEqual(group1, metaData.GetGroups()[0]);

            Assert.AreEqual(1, metaData.GetNames(group1).Count);
            Assert.AreEqual(name1, metaData.GetNames(group1)[0]);
            Assert.AreEqual(value2, metaData.GetValue(group1, name1));
        }
    }
}
