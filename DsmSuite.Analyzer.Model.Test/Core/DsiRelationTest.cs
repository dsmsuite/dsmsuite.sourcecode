using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.Model.Test.Core
{
    [TestClass]
    public class DsiRelationTest
    {
        [TestMethod]
        public void WhenRelationIsConstructedThenPropertiesAreSetAccordingArguments()
        {
            IDsiRelation relation = new DsiRelation(1, 2, "type", 3, null);
            Assert.AreEqual(1, relation.ConsumerId);
            Assert.AreEqual(2, relation.ProviderId);
            Assert.AreEqual("type", relation.Type);
            Assert.AreEqual(3, relation.Weight);
        }
    }
}
