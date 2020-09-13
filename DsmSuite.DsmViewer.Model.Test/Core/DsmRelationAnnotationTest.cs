using System;
using DsmSuite.DsmViewer.Model.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.DsmViewer.Model.Test.Core
{
    [TestClass]
    public class DsmRelationAnnotationTest
    {
        [TestMethod]
        public void WhenElementIsConstructedThenPropertiesAreSetAccordingArguments()
        {
            int consumerId = 1;
            int providerId = 1;
            string text = "someText";
            DsmRelationAnnotation annotation = new DsmRelationAnnotation(consumerId, providerId, text);
            Assert.AreEqual(consumerId, annotation.ConsumerId);
            Assert.AreEqual(providerId, annotation.ProviderId);
            Assert.AreEqual(text, annotation.Text);
        }
    }
}
