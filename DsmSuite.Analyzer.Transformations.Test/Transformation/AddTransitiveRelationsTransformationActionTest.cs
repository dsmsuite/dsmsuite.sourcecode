using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Transformations.Transformation;
using System.Reflection;

namespace DsmSuite.Analyzer.Transformations.Test.Transformation
{
    [TestClass]
    public class AddTransitiveRelationsTransformationActionTest
    {
        [TestMethod]
        public void AddTransitiveRelations()
        {
            DsiModel dataModel = new DsiModel("Test", new List<string>(), Assembly.GetExecutingAssembly());

            IDsiElement element1 = dataModel.AddElement("element1Name", "class", null);
            Assert.IsNotNull(element1);
            IDsiElement element2 = dataModel.AddElement("element2Name", "class", null);
            Assert.IsNotNull(element2);
            IDsiElement element3 = dataModel.AddElement("element3Name", "class", null);
            Assert.IsNotNull(element3);
            IDsiElement element4 = dataModel.AddElement("element4Name", "class", null);
            Assert.IsNotNull(element4);
            IDsiElement element5 = dataModel.AddElement("element5Name", "class", null);
            Assert.IsNotNull(element5);

            dataModel.AddRelation(element1.Name, element2.Name, "", 1, null);
            dataModel.AddRelation(element2.Name, element3.Name, "", 1, null);
            dataModel.AddRelation(element3.Name, element4.Name, "", 1, null);
            dataModel.AddRelation(element4.Name, element5.Name, "", 1, null);

            Assert.AreEqual(1, dataModel.GetRelationsOfConsumer(element1.Id).Count);
            Assert.IsTrue(dataModel.DoesRelationExist(element1.Id, element2.Id));
            Assert.AreEqual(1, dataModel.GetRelationsOfConsumer(element2.Id).Count);
            Assert.IsTrue(dataModel.DoesRelationExist(element2.Id, element3.Id));
            Assert.AreEqual(1, dataModel.GetRelationsOfConsumer(element3.Id).Count);
            Assert.IsTrue(dataModel.DoesRelationExist(element3.Id, element4.Id));
            Assert.AreEqual(1, dataModel.GetRelationsOfConsumer(element4.Id).Count);
            Assert.IsTrue(dataModel.DoesRelationExist(element4.Id, element5.Id));
            Assert.AreEqual(0, dataModel.GetRelationsOfConsumer(element5.Id).Count);

            AddTransitiveRelationsTransformationAction transformation = new AddTransitiveRelationsTransformationAction(dataModel, null);
            transformation.Execute();

            Assert.AreEqual(4, dataModel.GetRelationsOfConsumer(element1.Id).Count);
            Assert.IsTrue(dataModel.DoesRelationExist(element1.Id, element2.Id));
            Assert.IsTrue(dataModel.DoesRelationExist(element1.Id, element3.Id));
            Assert.IsTrue(dataModel.DoesRelationExist(element1.Id, element4.Id));
            Assert.IsTrue(dataModel.DoesRelationExist(element1.Id, element5.Id));
            Assert.AreEqual(3, dataModel.GetRelationsOfConsumer(element2.Id).Count);
            Assert.IsTrue(dataModel.DoesRelationExist(element2.Id, element3.Id));
            Assert.IsTrue(dataModel.DoesRelationExist(element2.Id, element4.Id));
            Assert.IsTrue(dataModel.DoesRelationExist(element2.Id, element5.Id));
            Assert.AreEqual(2, dataModel.GetRelationsOfConsumer(element3.Id).Count);
            Assert.IsTrue(dataModel.DoesRelationExist(element3.Id, element4.Id));
            Assert.IsTrue(dataModel.DoesRelationExist(element3.Id, element5.Id));
            Assert.AreEqual(1, dataModel.GetRelationsOfConsumer(element4.Id).Count);
            Assert.IsTrue(dataModel.DoesRelationExist(element4.Id, element5.Id));
            Assert.AreEqual(0, dataModel.GetRelationsOfConsumer(element5.Id).Count);
        }
    }
}
