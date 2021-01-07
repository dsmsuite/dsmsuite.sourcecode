using DsmSuite.DsmViewer.Model.Core;
using DsmSuite.DsmViewer.Model.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.DsmViewer.Model.Test.Core
{
    [TestClass]
    public class RelationTest
    {
        [TestMethod]
        public void WhenRelationIsConstructedThenPropertiesAreSetAccordingArguments()
        {
            int relationId = 1;
            int consumerId = 2;
            int providerId = 3;
            IDsmElement consumer = new DsmElement(consumerId, "element1", "type1");
            IDsmElement provider = new DsmElement(providerId, "element2", "type2");
            string relationType = "include";
            int weight = 4;
            DsmRelation relation = new DsmRelation(relationId, consumer, provider, relationType, weight);
            Assert.AreEqual(relationId, relation.Id);
            Assert.AreEqual(consumerId, relation.Consumer.Id);
            Assert.AreEqual(providerId, relation.Provider.Id);
            Assert.AreEqual(relationType, relation.Type);
            Assert.AreEqual(weight, relation.Weight);
        }
    }
}
