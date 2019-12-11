using DsmSuite.Common.Model.Core;
using DsmSuite.Common.Model.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.Model.Test.Data
{
    [TestClass]
    public class DsiMetaDataItemTest
    {
        [TestMethod]
        public void When_ItemIsConstructed_Then_PropertiesAreSetAccordingArguments()
        {
            IMetaDataItem item = new MetaDataItem("name", "value");
            Assert.AreEqual("name", item.Name);
            Assert.AreEqual("value", item.Value);
        }
    }
}
