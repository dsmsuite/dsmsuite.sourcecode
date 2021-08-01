using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Transformations.Settings;
using DsmSuite.Analyzer.Transformations.Transformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.Transformations.Test.Transformation
{
    [TestClass]
    public class MergeHeaderAndSourceFileDirectoriesTransformationActionTest
    {
        [TestMethod]
        public void RenameOnlyMatchingElements()
        {
            DsiModel dataModel = new DsiModel("Test", new List<string>(), System.Reflection.Assembly.GetExecutingAssembly());

            dataModel.AddElement("element1Name", "class", null);
            dataModel.AddElement("element2Name", "class", null);

            IDsiElement element1Before = dataModel.FindElementByName("element1Name");
            Assert.IsNotNull(element1Before);
            IDsiElement element2Before = dataModel.FindElementByName("element2Name");
            Assert.IsNotNull(element2Before);
            IDsiElement element3Before = dataModel.FindElementByName("element3Name");
            Assert.IsNull(element3Before);
            IDsiElement element4Before = dataModel.FindElementByName("element4Name");
            Assert.IsNull(element4Before);

            List<TransformationModuleMergeRule> rules = new List<TransformationModuleMergeRule>
            {
                new TransformationModuleMergeRule() {From = "element1Name", To = "element3Name"},
                new TransformationModuleMergeRule() {From = "ELEMENT2NAME", To = "element4Name"}
            };

            MergeHeaderAndSourceFileDirectoriesTransformationAction transformation = new MergeHeaderAndSourceFileDirectoriesTransformationAction(dataModel, rules, null);
            transformation.Execute();

            IDsiElement element1After = dataModel.FindElementByName("element1Name");
            Assert.IsNull(element1After);
            IDsiElement element2After = dataModel.FindElementByName("element2Name");
            Assert.IsNotNull(element2After);
            IDsiElement element3After = dataModel.FindElementByName("element3Name");
            Assert.IsNotNull(element3After);
            IDsiElement element4After = dataModel.FindElementByName("element4Name");
            Assert.IsNull(element4After);

            // Element1 renamed
            Assert.AreEqual(element1Before.Id, element3After.Id);
            Assert.AreEqual("element3Name", element3After.Name);
            Assert.AreEqual(element1Before.Type, element3After.Type);
            Assert.AreEqual(element1Before.Annotation, element3After.Annotation);

            // Element2 not renamed because replace is case sensitive
            Assert.AreEqual(element2Before.Id, element2After.Id);
            Assert.AreEqual(element2Before.Name, element2After.Name);
            Assert.AreEqual(element2Before.Type, element2After.Type);
            Assert.AreEqual(element2Before.Annotation, element2After.Annotation);
        }
    }
}
