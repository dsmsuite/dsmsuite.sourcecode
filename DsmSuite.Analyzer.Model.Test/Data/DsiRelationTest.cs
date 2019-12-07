using DsmSuite.Analyzer.Model.Data;
using DsmSuite.Analyzer.Model.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.Model.Test.Data
{
    [TestClass]
    public class DsiRelationTest
    {
        [TestMethod]
        public void TestRelationConstructor()
        {
            IDsiRelation relation = new DsiRelation(1, 2, "type", 3);
            Assert.AreEqual(1, relation.ConsumerId);
            Assert.AreEqual(2, relation.ProviderId);
            Assert.AreEqual("type", relation.Type);
            Assert.AreEqual(3, relation.Weight);
        }
    }
}
