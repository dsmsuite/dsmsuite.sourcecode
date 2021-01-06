using System.Collections.Generic;
using System.Reflection;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Transformations.Transformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.Transformations.Test.Transformation
{
    [TestClass]
    public class RemoveIgnoreNamesTransformationActionTest
    {
        [TestMethod]
        public void RemoveIgnoredName()
        {
            DsiModel dataModel = new DsiModel("Test", Assembly.GetExecutingAssembly());

            IDsiElement element1 = dataModel.AddElement("element1Name", "class", "");
            Assert.IsNotNull(element1);
            IDsiElement element2 = dataModel.AddElement("element2Name", "class", "");
            Assert.IsNotNull(element2);
            IDsiElement element3 = dataModel.AddElement("element3Name", "class", "");
            Assert.IsNotNull(element3);
            IDsiElement element4 = dataModel.AddElement("element4Name", "class", "");
            Assert.IsNotNull(element4);
            IDsiElement element5 = dataModel.AddElement("element5Name", "class", "");
            Assert.IsNotNull(element5);

            dataModel.AddRelation(element1.Name, element2.Name, "", 1, "context");
            dataModel.AddRelation(element2.Name, element3.Name, "", 1, "context");
            dataModel.AddRelation(element3.Name, element4.Name, "", 1, "context");
            dataModel.AddRelation(element4.Name, element5.Name, "", 1, "context");

            Assert.IsNotNull(dataModel.FindElementByName("element1Name"));
            Assert.IsNotNull(dataModel.FindElementByName("element2Name"));
            Assert.IsNotNull(dataModel.FindElementByName("element3Name"));
            Assert.IsNotNull(dataModel.FindElementByName("element4Name"));
            Assert.IsNotNull(dataModel.FindElementByName("element5Name"));
            Assert.AreEqual(5, dataModel.CurrentElementCount);

            Assert.AreEqual(4, dataModel.CurrentRelationCount);

            List<string> ignoredNames = new List<string> {"element[34]"}; // Names can be regular expressions
            RemoveIgnoreNamesTransformationAction transformation = new RemoveIgnoreNamesTransformationAction(dataModel, ignoredNames, null);
            transformation.Execute();

            Assert.IsNotNull(dataModel.FindElementByName("element1Name"));
            Assert.IsNotNull(dataModel.FindElementByName("element2Name"));
            Assert.IsNotNull(dataModel.FindElementByName("element5Name"));
            Assert.AreEqual(3, dataModel.CurrentElementCount);

            Assert.AreEqual(1, dataModel.CurrentRelationCount);
        }
    }
}
