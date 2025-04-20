using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Analyzer.Model.Test.Core
{
    [TestClass]
    public class DsiElementTest
    {
        [TestMethod]
        public void WhenElementIsConstructedWithoutPropertiesThenElementAccordingInputArguments()
        {
            IDsiElement element = new DsiElement(1, "name", "type", null);
            Assert.AreEqual(1, element.Id);
            Assert.AreEqual("name", element.Name);
            Assert.AreEqual("type", element.Type);
            Assert.IsNull(element.Properties);
        }

        [TestMethod]
        public void WhenElementIsConstructedWithPropertiesThenElementAccordingInputArguments()
        {
            Dictionary<string, string> elementProperties = new Dictionary<string, string>();
            elementProperties["annotation"] = "some text";
            elementProperties["version"] = "1.0";
            IDsiElement element = new DsiElement(1, "name", "type", elementProperties);
            Assert.AreEqual(1, element.Id);
            Assert.AreEqual("name", element.Name);
            Assert.AreEqual("type", element.Type);
            Assert.IsNotNull(element.Properties);
            Assert.AreEqual("some text", element.Properties["annotation"]);
            Assert.AreEqual("1.0", element.Properties["version"]);
        }
    }
}
