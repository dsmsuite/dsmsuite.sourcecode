using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.Model.Test.Core
{
    [TestClass]
    public class DsiElementTest
    {
        [TestMethod]
        public void WhenElementIsConstructedThenPropertiesAreSetAccordingArguments()
        {
            IDsiElement element = new DsiElement(1, "name", "type", null);
            Assert.AreEqual(1, element.Id);
            Assert.AreEqual("name", element.Name);
            Assert.AreEqual("type", element.Type);
        }
    }
}
