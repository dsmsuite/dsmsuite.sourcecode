using DsmSuite.DsmViewer.Model.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.DsmViewer.Model.Test.Data
{
    [TestClass]
    public class RelationTest
    {
        [TestMethod]
        public void TestRelationConstructor()
        {
            int consumerId = 1;
            int providerId = 2;
            string relationType = "include";
            int weight = 3;
            DsmRelation relation = new DsmRelation(consumerId, providerId, relationType, weight);
            Assert.AreEqual(consumerId, relation.ConsumerId);
            Assert.AreEqual(providerId, relation.ProviderId);
            Assert.AreEqual(relationType, relation.Type);
            Assert.AreEqual(weight, relation.Weight);
        }
    }
}
