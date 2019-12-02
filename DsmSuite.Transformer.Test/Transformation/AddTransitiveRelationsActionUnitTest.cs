using System.Reflection;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Transformer.Transformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Transformer.Test.Transformation
{
    [TestClass]
    public class AddTransitiveRelationsActionUnitTest
    {
        [TestMethod]
        public void AddTransitiveRelations()
        {
            DsiDataModel dataModel = new DsiDataModel("Test", Assembly.GetExecutingAssembly());

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

            Assert.AreEqual(1, dataModel.GetProviderRelations(element1).Count);
            Assert.IsTrue(dataModel.DoesRelationExist(element1, element2));
            Assert.AreEqual(1, dataModel.GetProviderRelations(element2).Count);
            Assert.IsTrue(dataModel.DoesRelationExist(element2, element3));
            Assert.AreEqual(1, dataModel.GetProviderRelations(element3).Count);
            Assert.IsTrue(dataModel.DoesRelationExist(element3, element4));
            Assert.AreEqual(1, dataModel.GetProviderRelations(element4).Count);
            Assert.IsTrue(dataModel.DoesRelationExist(element4, element5));
            Assert.AreEqual(0, dataModel.GetProviderRelations(element5).Count);

            AddTransitiveRelationsAction transformation = new AddTransitiveRelationsAction(dataModel, true);
            transformation.Execute();

            Assert.AreEqual(4, dataModel.GetProviderRelations(element1).Count);
            Assert.IsTrue(dataModel.DoesRelationExist(element1, element2));
            Assert.IsTrue(dataModel.DoesRelationExist(element1, element3));
            Assert.IsTrue(dataModel.DoesRelationExist(element1, element4));
            Assert.IsTrue(dataModel.DoesRelationExist(element1, element5));
            Assert.AreEqual(3, dataModel.GetProviderRelations(element2).Count);
            Assert.IsTrue(dataModel.DoesRelationExist(element2, element3));
            Assert.IsTrue(dataModel.DoesRelationExist(element2, element4));
            Assert.IsTrue(dataModel.DoesRelationExist(element2, element5));
            Assert.AreEqual(2, dataModel.GetProviderRelations(element3).Count);
            Assert.IsTrue(dataModel.DoesRelationExist(element3, element4));
            Assert.IsTrue(dataModel.DoesRelationExist(element3, element5));
            Assert.AreEqual(1, dataModel.GetProviderRelations(element4).Count);
            Assert.IsTrue(dataModel.DoesRelationExist(element4, element5));
            Assert.AreEqual(0, dataModel.GetProviderRelations(element5).Count);
        }
    }
}
