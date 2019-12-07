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
            int relationId = 1;
            int consumerId = 2;
            int providerId = 3;
            string relationType = "include";
            int weight = 4;
            DsmRelation relation = new DsmRelation(relationId, consumerId, providerId, relationType, weight);
            Assert.AreEqual(relationId, relation.Id);
            Assert.AreEqual(consumerId, relation.Consumer);
            Assert.AreEqual(providerId, relation.Provider);
            Assert.AreEqual(relationType, relation.Type);
            Assert.AreEqual(weight, relation.Weight);
        }
    }
}
