using System.Collections.Generic;
using DsmSuite.Analyzer.Data;
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
            DataModel dataModel = new DataModel("Test", System.Reflection.Assembly.GetExecutingAssembly());

            dataModel.AddElement("element1Name", "class", "element1Source");
            dataModel.AddElement("element2Name", "class", "element2Source");

            IElement element1Before = dataModel.FindElement("element1Name");
            Assert.IsNotNull(element1Before);
            IElement element2Before = dataModel.FindElement("element2Name");
            Assert.IsNotNull(element2Before);
            IElement element3Before = dataModel.FindElement("element3Name");
            Assert.IsNull(element3Before);
            IElement element4Before = dataModel.FindElement("element4Name");
            Assert.IsNull(element4Before);

            MoveElementsSettings moveElementsSettings = new MoveElementsSettings();
            moveElementsSettings.Enabled = true;
            moveElementsSettings.Rules = new List<MoveElementRule>
            {
                new MoveElementRule() {From = "element1Name", To = "element3Name"},
                new MoveElementRule() {From = "ELEMENT2NAME", To = "element4Name"}
            };
            MoveElementsAction transformation = new MoveElementsAction(dataModel, moveElementsSettings);
            transformation.Execute();

            IElement element1After = dataModel.FindElement("element1Name");
            Assert.IsNull(element1After);
            IElement element2After = dataModel.FindElement("element2Name");
            Assert.IsNotNull(element2After);
            IElement element3After = dataModel.FindElement("element3Name");
            Assert.IsNotNull(element3After);
            IElement element4After = dataModel.FindElement("element4Name");
            Assert.IsNull(element4After);

            // Element1 renamed
            Assert.AreEqual(element1Before.ElementId, element3After.ElementId);
            Assert.AreEqual("element3Name", element3After.Name);
            Assert.AreEqual(element1Before.Type, element3After.Type);
            Assert.AreEqual(element1Before.Source, element3After.Source);

            // Element2 not renamed because replace is case sensitive
            Assert.AreEqual(element2Before.ElementId, element2After.ElementId);
            Assert.AreEqual(element2Before.Name, element2After.Name);
            Assert.AreEqual(element2Before.Type, element2After.Type);
            Assert.AreEqual(element2Before.Source, element2After.Source);
        }
    }
}
