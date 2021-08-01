using DsmSuite.Common.Model.Core;
using DsmSuite.Common.Model.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Common.Model.Test.Core
{
    [TestClass]
    public class DsiMetaDataItemTest
    {
        [TestMethod]
        public void WhenItemIsConstructedThenPropertiesAreSetAccordingArguments()
        {
            IMetaDataItem item = new MetaDataItem("name", "value");
            Assert.AreEqual("name", item.Name);
            Assert.AreEqual("value", item.Value);
        }
    }
}
