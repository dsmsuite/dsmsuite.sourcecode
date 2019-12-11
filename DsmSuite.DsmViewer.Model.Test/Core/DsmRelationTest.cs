using DsmSuite.DsmViewer.Model.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.DsmViewer.Model.Test.Core
{
    [TestClass]
    public class RelationTest
    {
        [TestMethod]
        public void When_RelationIsConstructed_Then_PropertiesAreSetAccordingArguments()
        {
            int relationId = 1;
            int consumerId = 2;
            int providerId = 3;
            string relationType = "include";
            int weight = 4;
            DsmRelation relation = new DsmRelation(relationId, consumerId, providerId, relationType, weight);
            Assert.AreEqual(relationId, relation.Id);
            Assert.AreEqual(consumerId, relation.ConsumerId);
            Assert.AreEqual(providerId, relation.ProviderId);
            Assert.AreEqual(relationType, relation.Type);
            Assert.AreEqual(weight, relation.Weight);
        }
    }
}
