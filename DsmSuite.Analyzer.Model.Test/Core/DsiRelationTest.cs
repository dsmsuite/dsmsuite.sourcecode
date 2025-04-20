using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Analyzer.Model.Test.Core
{
    [TestClass]
    public class DsiRelationTest
    {
        [TestMethod]
        public void WhenRelationIsConstructedWithoutPropertiesThenRelationAccordingInputArguments()
        {
            IDsiRelation relation = new DsiRelation(1, 2, "type", 3, null);
            Assert.AreEqual(1, relation.ConsumerId);
            Assert.AreEqual(2, relation.ProviderId);
            Assert.AreEqual("type", relation.Type);
            Assert.AreEqual(3, relation.Weight);
            Assert.IsNull(relation.Properties);
        }

        [TestMethod]
        public void WhenRelationIsConstructedWithPropertieThenRelationtAccordingInputArguments()
        {
            Dictionary<string, string> relationProperties = new Dictionary<string, string>();
            relationProperties["annotation"] = "some text";
            relationProperties["version"] = "1.0";
            IDsiRelation relation = new DsiRelation(1, 2, "type", 3, relationProperties);
            Assert.AreEqual(1, relation.ConsumerId);
            Assert.AreEqual(2, relation.ProviderId);
            Assert.AreEqual("type", relation.Type);
            Assert.AreEqual(3, relation.Weight);
            Assert.IsNotNull(relation.Properties);
            Assert.AreEqual("some text", relation.Properties["annotation"]);
            Assert.AreEqual("1.0", relation.Properties["version"]);
        }
    }
}
