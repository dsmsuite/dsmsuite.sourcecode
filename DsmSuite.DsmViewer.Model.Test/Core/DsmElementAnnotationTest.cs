using DsmSuite.DsmViewer.Model.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.DsmViewer.Model.Test.Core
{
    [TestClass]
    public class DsmElementAnnotationTest
    {
        [TestMethod]
        public void WhenElementIsConstructedThenPropertiesAreSetAccordingArguments()
        {
            int elementId = 1;
            string text = "someText";
            DsmElementAnnotation annotation = new DsmElementAnnotation(elementId, text);
            Assert.AreEqual(elementId, annotation.ElementId);
            Assert.AreEqual(text, annotation.Text);
        }
    }
}
