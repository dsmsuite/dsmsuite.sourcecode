using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.Model.Test.Core
{
    [TestClass]
    public class DsiDataModelTest
    {
        [TestMethod]
        public void AddingNewElementIncreasesElementCount()
        {
            DsiDataModel dataModel = new DsiDataModel("Test", Assembly.GetExecutingAssembly());

            Assert.AreEqual(0, dataModel.TotalElementCount);
            IDsiElement element1 = dataModel.AddElement("element1Name", "class", "element1Source");
            Assert.IsNotNull(element1);
            Assert.AreEqual(1, element1.Id);
            Assert.AreEqual("element1Name", element1.Name);
            Assert.AreEqual("class", element1.Type);
            Assert.AreEqual("element1Source", element1.Source);
            Assert.AreEqual(1, dataModel.TotalElementCount);

            IDsiElement element2 = dataModel.AddElement("element2Name", "struct", "element2Source");
            Assert.IsNotNull(element2);
            Assert.AreEqual(2, element2.Id);
            Assert.AreEqual("element2Name", element2.Name);
            Assert.AreEqual("struct", element2.Type);
            Assert.AreEqual("element2Source", element2.Source);
            Assert.AreEqual(2, dataModel.TotalElementCount);
        }

        [TestMethod]
        public void AddingExistingElementDoesNotIncreaseElementCount()
        {
            DsiDataModel dataModel = new DsiDataModel("Test", Assembly.GetExecutingAssembly());

            Assert.AreEqual(0, dataModel.TotalElementCount);
            IDsiElement element1 = dataModel.AddElement("elementName", "class", "elementSourceA");
            Assert.IsNotNull(element1);
            Assert.AreEqual(1, dataModel.TotalElementCount);

            IDsiElement element2 = dataModel.AddElement("elementName", "enum", "elementSourceB");
            Assert.IsNull(element2);
            Assert.AreEqual(1, dataModel.TotalElementCount);
        }

        [TestMethod]
        public void ElementCanBeFoundByName()
        {
            DsiDataModel dataModel = new DsiDataModel("Test", Assembly.GetExecutingAssembly());

            dataModel.AddElement("element1Name", "class", "element1Source");
            dataModel.AddElement("element2Name", "class", "element2Source");
            dataModel.AddElement("element3Name", "struct", "element3Source");

            IDsiElement element1 = dataModel.FindElementByName("element1Name");
            Assert.IsNotNull(element1);
            Assert.AreEqual(1, element1.Id);
            Assert.AreEqual("element1Name", element1.Name);
            Assert.AreEqual("class", element1.Type);
            Assert.AreEqual("element1Source", element1.Source);

            IDsiElement element2 = dataModel.FindElementByName("element2Name");
            Assert.IsNotNull(element2);
            Assert.AreEqual(2, element2.Id);
            Assert.AreEqual("element2Name", element2.Name);
            Assert.AreEqual("class", element2.Type);
            Assert.AreEqual("element2Source", element2.Source);

            IDsiElement element3 = dataModel.FindElementByName("element3Name");
            Assert.IsNotNull(element3);
            Assert.AreEqual(3, element3.Id);
            Assert.AreEqual("element3Name", element3.Name);
            Assert.AreEqual("struct", element3.Type);
            Assert.AreEqual("element3Source", element3.Source);

            IDsiElement element4 = dataModel.FindElementByName("element4Name");
            Assert.IsNull(element4);
        }

        [TestMethod]
        public void ElementCanBeRemoved()
        {
            DsiDataModel dataModel = new DsiDataModel("Test", Assembly.GetExecutingAssembly());

            IDsiElement consumer = dataModel.AddElement("consumerName", "class", "consumerSource");
            Assert.IsNotNull(consumer);
            IDsiElement provider1 = dataModel.AddElement("provider1Name", "class", "provider1Source");
            Assert.IsNotNull(provider1);
            IDsiElement provider2 = dataModel.AddElement("provider2Name", "class", "provider2Source");
            Assert.IsNotNull(provider2);

            string relation1Type = "relationType1";
            int relation1Weight = 3;
            IDsiRelation relation1 = dataModel.AddRelation(consumer.Name, provider1.Name, relation1Type, relation1Weight, "context");
            Assert.IsNotNull(relation1);
            Assert.AreEqual(consumer.Id, relation1.ConsumerId);
            Assert.AreEqual(provider1.Id, relation1.ProviderId);
            Assert.AreEqual(relation1Type, relation1.Type);
            Assert.AreEqual(relation1Weight, relation1.Weight);

            string relation2Type = "relationType2";
            int relation2Weight = 4;
            IDsiRelation relation2 = dataModel.AddRelation(consumer.Name, provider2.Name, relation2Type, relation2Weight, "context");
            Assert.IsNotNull(relation2);
            Assert.AreEqual(consumer.Id, relation2.ConsumerId);
            Assert.AreEqual(provider2.Id, relation2.ProviderId);
            Assert.AreEqual(relation2Type, relation2.Type);
            Assert.AreEqual(relation2Weight, relation2.Weight);

            Assert.AreEqual(2, dataModel.GetRelationsOfConsumer(consumer.Id).Count);

            dataModel.RemoveElement(provider2);
            dataModel.Cleanup();

            Assert.AreEqual(1, dataModel.GetRelationsOfConsumer(consumer.Id).Count);
            IDsiRelation[] relationsRetrieved = dataModel.GetRelationsOfConsumer(consumer.Id).ToArray();
            Assert.AreEqual(consumer.Id, relationsRetrieved[0].ConsumerId);
            Assert.AreEqual(provider1.Id, relationsRetrieved[0].ProviderId);
            Assert.AreEqual(relation1Type, relationsRetrieved[0].Type);
            Assert.AreEqual(relation1Weight, relationsRetrieved[0].Weight);
        }

        [TestMethod]
        public void RenamedElementCanBeFoundByName()
        {
            DsiDataModel dataModel = new DsiDataModel("Test", Assembly.GetExecutingAssembly());

            dataModel.AddElement("element1Name", "class", "element1Source");
            dataModel.AddElement("element2Name", "enum", "element2Source");
            dataModel.AddElement("element3Name", "struct", "element3Source");

            IDsiElement element2 = dataModel.FindElementByName("element2Name");
            Assert.IsNotNull(element2);
            Assert.AreEqual(2, element2.Id);
            Assert.AreEqual("element2Name", element2.Name);
            Assert.AreEqual("enum", element2.Type);
            Assert.AreEqual("element2Source", element2.Source);

            dataModel.RenameElement(element2, "element2NewName");

            IDsiElement renamedElement2 = dataModel.FindElementByName("element2NewName");
            Assert.IsNotNull(renamedElement2);
            Assert.AreEqual(2, renamedElement2.Id);
            Assert.AreEqual("element2NewName", renamedElement2.Name);
            Assert.AreEqual("enum", renamedElement2.Type);
            Assert.AreEqual("element2Source", renamedElement2.Source);

            IDsiElement originalElement2 = dataModel.FindElementByName("elementName");
            Assert.IsNull(originalElement2);
        }

        [TestMethod]
        public void AddingAreRelationAddsItToTheConsumerElementAsProviderRelation()
        {
            DsiDataModel dataModel = new DsiDataModel("Test", Assembly.GetExecutingAssembly());

            IDsiElement consumer = dataModel.AddElement("consumerName", "class", "consumerSource");
            Assert.IsNotNull(consumer);
            IDsiElement provider1 = dataModel.AddElement("provider1Name", "class", "provider1Source");
            Assert.IsNotNull(provider1);
            IDsiElement provider2 = dataModel.AddElement("provider2Name", "class", "provider2Source");
            Assert.IsNotNull(provider2);

            string relation1Type = "relationType1";
            int relation1Weight = 3;
            IDsiRelation relation1 = dataModel.AddRelation(consumer.Name, provider1.Name, relation1Type, relation1Weight, "context");
            Assert.IsNotNull(relation1);
            Assert.AreEqual(consumer.Id, relation1.ConsumerId);
            Assert.AreEqual(provider1.Id, relation1.ProviderId);
            Assert.AreEqual(relation1Type, relation1.Type);
            Assert.AreEqual(relation1Weight, relation1.Weight);

            Assert.AreEqual(1, dataModel.GetRelationsOfConsumer(consumer.Id).Count);

            string relation2Type = "relationType2";
            int relation2Weight = 4;
            IDsiRelation relation2 = dataModel.AddRelation(consumer.Name, provider2.Name, relation2Type, relation2Weight, "context");
            Assert.IsNotNull(relation2);
            Assert.AreEqual(consumer.Id, relation2.ConsumerId);
            Assert.AreEqual(provider2.Id, relation2.ProviderId);
            Assert.AreEqual(relation2Type, relation2.Type);
            Assert.AreEqual(relation2Weight, relation2.Weight);

            Assert.AreEqual(2, dataModel.GetRelationsOfConsumer(consumer.Id).Count);
        }

        [TestMethod]
        public void LoadingAndSavedModelRestoresThePreviousState()
        {
            string filename = "temp.dsi";

            DsiDataModel dataModel1 = new DsiDataModel("Test", Assembly.GetExecutingAssembly());

            IDsiElement consumer = dataModel1.AddElement("consumerName", "class", "consumerSource");
            Assert.IsNotNull(consumer);
            IDsiElement provider1 = dataModel1.AddElement("provider1Name", "class", "provider1Source");
            Assert.IsNotNull(provider1);
            IDsiElement provider2 = dataModel1.AddElement("provider2Name", "class", "provider2Source");
            Assert.IsNotNull(provider2);

            dataModel1.AddRelation(consumer.Name, provider1.Name, "relationType2", 2, "context");
            dataModel1.AddRelation(consumer.Name, provider2.Name, "relationType3", 3, "context");

            dataModel1.Save(filename, false);

            DsiDataModel dataModel2 = new DsiDataModel("Test", Assembly.GetExecutingAssembly());
            dataModel2.Load(filename);

            Assert.AreEqual(dataModel1.TotalElementCount, dataModel2.TotalElementCount);
            List<IDsiElement> dataModel1Elements = dataModel1.GetElements().ToList();
            List<IDsiElement> dataModel2Elements = dataModel2.GetElements().ToList();
            for (int elementIndex = 0; elementIndex < dataModel1.TotalElementCount; elementIndex++)
            {
                Assert.AreEqual(dataModel1Elements[elementIndex].Id, dataModel2Elements[elementIndex].Id);
                Assert.AreEqual(dataModel1Elements[elementIndex].Name, dataModel2Elements[elementIndex].Name);
                Assert.AreEqual(dataModel1Elements[elementIndex].Type, dataModel2Elements[elementIndex].Type);
                Assert.AreEqual(dataModel1Elements[elementIndex].Source, dataModel2Elements[elementIndex].Source);
                Assert.AreEqual(dataModel1.GetRelationsOfConsumer(dataModel1Elements[elementIndex].Id).Count, dataModel2.GetRelationsOfConsumer(dataModel1Elements[elementIndex].Id).Count);

                List<IDsiRelation> dataModel1Relations = dataModel1.GetRelationsOfConsumer(dataModel1Elements[elementIndex].Id).ToList();
                List<IDsiRelation> dataModel2Relations = dataModel2.GetRelationsOfConsumer(dataModel2Elements[elementIndex].Id).ToList();

                for (int relationIndex = 0; relationIndex < dataModel1.GetRelationsOfConsumer(dataModel1Elements[elementIndex].Id).Count;
                    relationIndex++)
                {
                    Assert.AreEqual(dataModel1Relations[relationIndex].ConsumerId, dataModel2Relations[relationIndex].ConsumerId);
                    Assert.AreEqual(dataModel1Relations[relationIndex].ProviderId, dataModel2Relations[relationIndex].ProviderId);
                    Assert.AreEqual(dataModel1Relations[relationIndex].Type, dataModel2Relations[relationIndex].Type);
                    Assert.AreEqual(dataModel1Relations[relationIndex].Weight, dataModel2Relations[relationIndex].Weight);
                }
            }
        }
    }
}
