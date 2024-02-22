using DsmSuite.DsmViewer.Model.Core;
using DsmSuite.DsmViewer.Model.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Model.Test.Core
{
    [TestClass]
    public class RelationTest
    {
        [TestMethod]
        public void WhenRelationIsConstructedWithPropertiesThenRelationAccordingInputArguments()
        {
            int relationId = 1;
            int consumerId = 2;
            int providerId = 3;
            IDsmElement consumer = new DsmElement(consumerId, "element1", "type1", null);
            IDsmElement provider = new DsmElement(providerId, "element2", "type2", null);
            string relationType = "include";
            int weight = 4;
            DsmRelation relation = new DsmRelation(relationId, consumer, provider, relationType, weight, null);
            Assert.AreEqual(relationId, relation.Id);
            Assert.AreEqual(consumerId, relation.Consumer.Id);
            Assert.AreEqual(providerId, relation.Provider.Id);
            Assert.AreEqual(relationType, relation.Type);
            Assert.AreEqual(weight, relation.Weight);
            Assert.IsNotNull(relation.Properties);
            Assert.AreEqual(0, relation.Properties.Count);
        }

        [TestMethod]
        public void WhenRelationIsConstructedWithoutPropertiesThenRelationAccordingInputArguments()
        {
            Dictionary<string, string> relationProperties = new Dictionary<string, string>();
            relationProperties["annotation"] = "some text";
            relationProperties["version"] = "1.0";
            int relationId = 1;
            int consumerId = 2;
            int providerId = 3;
            IDsmElement consumer = new DsmElement(consumerId, "element1", "type1", null);
            IDsmElement provider = new DsmElement(providerId, "element2", "type2", null);
            string relationType = "include";
            int weight = 4;
            DsmRelation relation = new DsmRelation(relationId, consumer, provider, relationType, weight, relationProperties);
            Assert.AreEqual(relationId, relation.Id);
            Assert.AreEqual(consumerId, relation.Consumer.Id);
            Assert.AreEqual(providerId, relation.Provider.Id);
            Assert.AreEqual(relationType, relation.Type);
            Assert.AreEqual(weight, relation.Weight);
            Assert.IsNotNull(relation.Properties);
            Assert.AreEqual("some text", relation.Properties["annotation"]);
            Assert.AreEqual("1.0", relation.Properties["version"]);
        }
    }
}
