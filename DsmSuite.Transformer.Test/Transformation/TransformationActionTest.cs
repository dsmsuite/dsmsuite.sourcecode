using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Transformer.Settings;
using DsmSuite.Transformer.Transformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Transformer.Test.Transformation
{
    [TestClass]
    public class TransformationActionTest
    {
        [TestMethod]
        public void RenameOnlyMatchingElements()
        {
            DsiModel dataModel = new DsiModel("Test", System.Reflection.Assembly.GetExecutingAssembly());

            dataModel.AddElement("element1Name", "class", "element1Source");
            dataModel.AddElement("element2Name", "class", "element2Source");

            IDsiElement element1Before = dataModel.FindElementByName("element1Name");
            Assert.IsNotNull(element1Before);
            IDsiElement element2Before = dataModel.FindElementByName("element2Name");
            Assert.IsNotNull(element2Before);
            IDsiElement element3Before = dataModel.FindElementByName("element3Name");
            Assert.IsNull(element3Before);
            IDsiElement element4Before = dataModel.FindElementByName("element4Name");
            Assert.IsNull(element4Before);

            MoveElementsSettings moveElementsSettings = new MoveElementsSettings
            {
                Enabled = true,
                Rules = new List<MoveElementRule>
                {
                    new MoveElementRule() {From = "element1Name", To = "element3Name"},
                    new MoveElementRule() {From = "ELEMENT2NAME", To = "element4Name"}
                }
            };

            MoveElementsAction transformation = new MoveElementsAction(dataModel, moveElementsSettings, null);
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
            Assert.AreEqual(element1Before.Source, element3After.Source);

            // Element2 not renamed because replace is case sensitive
            Assert.AreEqual(element2Before.Id, element2After.Id);
            Assert.AreEqual(element2Before.Name, element2After.Name);
            Assert.AreEqual(element2Before.Type, element2After.Type);
            Assert.AreEqual(element2Before.Source, element2After.Source);
        }
    }
}
