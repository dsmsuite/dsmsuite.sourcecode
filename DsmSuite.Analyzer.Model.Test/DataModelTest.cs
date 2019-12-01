using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.Model.Test
{
    [TestClass]
    public class DataModelTest
    {
        [TestMethod]
        public void AddingNewElementIncreasesElementCount()
        {
            DataModel dataModel = new DataModel("Test", Assembly.GetExecutingAssembly());

            Assert.AreEqual(0, dataModel.Elements.Count);
            IElement element1 = dataModel.AddElement("element1Name", "class", "element1Source");
            Assert.IsNotNull(element1);
            Assert.AreEqual(0, element1.ElementId);
            Assert.AreEqual("element1Name", element1.Name);
            Assert.AreEqual("class", element1.Type);
            Assert.AreEqual("element1Source", element1.Source);
            Assert.AreEqual(1, dataModel.Elements.Count);

            IElement element2 = dataModel.AddElement("element2Name", "struct", "element2Source");
            Assert.IsNotNull(element2);
            Assert.AreEqual(1, element2.ElementId);
            Assert.AreEqual("element2Name", element2.Name);
            Assert.AreEqual("struct", element2.Type);
            Assert.AreEqual("element2Source", element2.Source);
            Assert.AreEqual(2, dataModel.Elements.Count);
        }

        [TestMethod]
        public void AddingExistingElementDoesNotIncreaseElementCount()
        {
            DataModel dataModel = new DataModel("Test", Assembly.GetExecutingAssembly());

            Assert.AreEqual(0, dataModel.Elements.Count);
            IElement element1 = dataModel.AddElement("elementName", "class", "elementSourceA");
            Assert.IsNotNull(element1);
            Assert.AreEqual(1, dataModel.Elements.Count);

            IElement element2 = dataModel.AddElement("elementName", "enum", "elementSourceB");
            Assert.IsNull(element2);
            Assert.AreEqual(1, dataModel.Elements.Count);
        }

        [TestMethod]
        public void ElementCanBeFoundByName()
        {
            DataModel dataModel = new DataModel("Test", Assembly.GetExecutingAssembly());

            dataModel.AddElement("element1Name", "class", "element1Source");
            dataModel.AddElement("element2Name", "class", "element2Source");
            dataModel.AddElement("element3Name", "struct", "element3Source");

            IElement element1 = dataModel.FindElement("element1Name");
            Assert.IsNotNull(element1);
            Assert.AreEqual(0, element1.ElementId);
            Assert.AreEqual("element1Name", element1.Name);
            Assert.AreEqual("class", element1.Type);
            Assert.AreEqual("element1Source", element1.Source);

            IElement element2 = dataModel.FindElement("element2Name");
            Assert.IsNotNull(element2);
            Assert.AreEqual(1, element2.ElementId);
            Assert.AreEqual("element2Name", element2.Name);
            Assert.AreEqual("class", element2.Type);
            Assert.AreEqual("element2Source", element2.Source);

            IElement element3 = dataModel.FindElement("element3Name");
            Assert.IsNotNull(element3);
            Assert.AreEqual(2, element3.ElementId);
            Assert.AreEqual("element3Name", element3.Name);
            Assert.AreEqual("struct", element3.Type);
            Assert.AreEqual("element3Source", element3.Source);

            IElement element4 = dataModel.FindElement("element4Name");
            Assert.IsNull(element4);
        }

        [TestMethod]
        public void ElementCanBeRemoved()
        {
            DataModel dataModel = new DataModel("Test", Assembly.GetExecutingAssembly());

            IElement consumer = dataModel.AddElement("consumerName", "class", "consumerSource");
            Assert.IsNotNull(consumer);
            IElement provider1 = dataModel.AddElement("provider1Name", "class", "provider1Source");
            Assert.IsNotNull(provider1);
            IElement provider2 = dataModel.AddElement("provider2Name", "class", "provider2Source");
            Assert.IsNotNull(provider2);

            string relation1Type = "relationType1";
            int relation1Weight = 3;
            IRelation relation1 = dataModel.AddRelation(consumer.Name, provider1.Name, relation1Type, relation1Weight, "context");
            Assert.IsNotNull(relation1);
            Assert.AreEqual(consumer.ElementId, relation1.ConsumerId);
            Assert.AreEqual(provider1.ElementId, relation1.ProviderId);
            Assert.AreEqual(relation1Type, relation1.Type);
            Assert.AreEqual(relation1Weight, relation1.Weight);

            string relation2Type = "relationType2";
            int relation2Weight = 4;
            IRelation relation2 = dataModel.AddRelation(consumer.Name, provider2.Name, relation2Type, relation2Weight, "context");
            Assert.IsNotNull(relation2);
            Assert.AreEqual(consumer.ElementId, relation2.ConsumerId);
            Assert.AreEqual(provider2.ElementId, relation2.ProviderId);
            Assert.AreEqual(relation2Type, relation2.Type);
            Assert.AreEqual(relation2Weight, relation2.Weight);

            Assert.AreEqual(2, dataModel.GetProviderRelations(consumer).Count);

            dataModel.RemoveElement(provider2);
            dataModel.Cleanup();

            Assert.AreEqual(1, dataModel.GetProviderRelations(consumer).Count);
            IRelation[] relationsRetrieved = dataModel.GetProviderRelations(consumer).ToArray();
            Assert.AreEqual(consumer.ElementId, relationsRetrieved[0].ConsumerId);
            Assert.AreEqual(provider1.ElementId, relationsRetrieved[0].ProviderId);
            Assert.AreEqual(relation1Type, relationsRetrieved[0].Type);
            Assert.AreEqual(relation1Weight, relationsRetrieved[0].Weight);
        }

        [TestMethod]
        public void RenamedElementCanBeFoundByName()
        {
            DataModel dataModel = new DataModel("Test", Assembly.GetExecutingAssembly());

            dataModel.AddElement("element1Name", "class", "element1Source");
            dataModel.AddElement("element2Name", "enum", "element2Source");
            dataModel.AddElement("element3Name", "struct", "element3Source");

            IElement element2 = dataModel.FindElement("element2Name");
            Assert.IsNotNull(element2);
            Assert.AreEqual(1, element2.ElementId);
            Assert.AreEqual("element2Name", element2.Name);
            Assert.AreEqual("enum", element2.Type);
            Assert.AreEqual("element2Source", element2.Source);

            dataModel.RenameElement(element2, "element2NewName");

            IElement renamedElement2 = dataModel.FindElement("element2NewName");
            Assert.IsNotNull(renamedElement2);
            Assert.AreEqual(1, renamedElement2.ElementId);
            Assert.AreEqual("element2NewName", renamedElement2.Name);
            Assert.AreEqual("enum", renamedElement2.Type);
            Assert.AreEqual("element2Source", renamedElement2.Source);

            IElement originalElement2 = dataModel.FindElement("elementName");
            Assert.IsNull(originalElement2);
        }

        [TestMethod]
        public void AddingAreRelationAddsItToTheConsumerElementAsProviderRelation()
        {
            DataModel dataModel = new DataModel("Test", Assembly.GetExecutingAssembly());

            IElement consumer = dataModel.AddElement("consumerName", "class", "consumerSource");
            Assert.IsNotNull(consumer);
            IElement provider1 = dataModel.AddElement("provider1Name", "class", "provider1Source");
            Assert.IsNotNull(provider1);
            IElement provider2 = dataModel.AddElement("provider2Name", "class", "provider2Source");
            Assert.IsNotNull(provider2);

            string relation1Type = "relationType1";
            int relation1Weight = 3;
            IRelation relation1 = dataModel.AddRelation(consumer.Name, provider1.Name, relation1Type, relation1Weight, "context");
            Assert.IsNotNull(relation1);
            Assert.AreEqual(consumer.ElementId, relation1.ConsumerId);
            Assert.AreEqual(provider1.ElementId, relation1.ProviderId);
            Assert.AreEqual(relation1Type, relation1.Type);
            Assert.AreEqual(relation1Weight, relation1.Weight);

            Assert.AreEqual(1, dataModel.GetProviderRelations(consumer).Count);

            string relation2Type = "relationType2";
            int relation2Weight = 4;
            IRelation relation2 = dataModel.AddRelation(consumer.Name, provider2.Name, relation2Type, relation2Weight, "context");
            Assert.IsNotNull(relation2);
            Assert.AreEqual(consumer.ElementId, relation2.ConsumerId);
            Assert.AreEqual(provider2.ElementId, relation2.ProviderId);
            Assert.AreEqual(relation2Type, relation2.Type);
            Assert.AreEqual(relation2Weight, relation2.Weight);

            Assert.AreEqual(2, dataModel.GetProviderRelations(consumer).Count);
        }

        [TestMethod]
        public void LoadingAndSavedModelRestoresThePreviousState()
        {
            string filename = "temp.dsi";

            DataModel dataModel1 = new DataModel("Test", Assembly.GetExecutingAssembly());

            IElement consumer = dataModel1.AddElement("consumerName", "class", "consumerSource");
            Assert.IsNotNull(consumer);
            IElement provider1 = dataModel1.AddElement("provider1Name", "class", "provider1Source");
            Assert.IsNotNull(provider1);
            IElement provider2 = dataModel1.AddElement("provider2Name", "class", "provider2Source");
            Assert.IsNotNull(provider2);

            dataModel1.AddRelation(consumer.Name, provider1.Name, "relationType2", 2, "context");
            dataModel1.AddRelation(consumer.Name, provider2.Name, "relationType3", 3, "context");

            dataModel1.Save(filename, false);

            DataModel dataModel2 = new DataModel("Test", Assembly.GetExecutingAssembly());
            dataModel2.Load(filename);

            Assert.AreEqual(dataModel1.Elements.Count, dataModel2.Elements.Count);
            List<IElement> dataModel1Elements = dataModel1.Elements.ToList();
            List<IElement> dataModel2Elements = dataModel2.Elements.ToList();
            for (int elementIndex = 0; elementIndex < dataModel1.Elements.Count; elementIndex++)
            {
                Assert.AreEqual(dataModel1Elements[elementIndex].ElementId, dataModel2Elements[elementIndex].ElementId);
                Assert.AreEqual(dataModel1Elements[elementIndex].Name, dataModel2Elements[elementIndex].Name);
                Assert.AreEqual(dataModel1Elements[elementIndex].Type, dataModel2Elements[elementIndex].Type);
                Assert.AreEqual(dataModel1Elements[elementIndex].Source, dataModel2Elements[elementIndex].Source);
                Assert.AreEqual(dataModel1.GetProviderRelations(dataModel1Elements[elementIndex]).Count, dataModel2.GetProviderRelations(dataModel1Elements[elementIndex]).Count);

                List<IRelation> dataModel1Relations = dataModel1.GetProviderRelations(dataModel1Elements[elementIndex]).ToList();
                List<IRelation> dataModel2Relations = dataModel2.GetProviderRelations(dataModel2Elements[elementIndex]).ToList();

                for (int relationIndex = 0; relationIndex < dataModel1.GetProviderRelations(dataModel1Elements[elementIndex]).Count;
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
