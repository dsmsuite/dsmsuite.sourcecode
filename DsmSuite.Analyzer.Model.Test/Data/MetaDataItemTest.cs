using DsmSuite.Analyzer.Model.Data;
using DsmSuite.Analyzer.Model.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.Model.Test.Data
{
    [TestClass]
    public class MetaDataItemTest
    {
        [TestMethod]
        public void TestMetaDataItemConstructor()
        {
            IMetaDataItem item = new MetaDataItem("name", "value");
            Assert.AreEqual("name", item.Name);
            Assert.AreEqual("value", item.Value);
        }
    }
}
