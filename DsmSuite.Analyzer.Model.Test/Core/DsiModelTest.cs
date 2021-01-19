using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.Model.Test.Core
{
    [TestClass]
    public class DsiModelTest
    {
        [TestMethod]
        public void LoadingAndSavedModelRestoresThePreviousState()
        {
            string filename = "temp.dsi";

            DsiModel dataModel1 = new DsiModel("Test", new List<string>(), Assembly.GetExecutingAssembly());

            IDsiElement consumer = dataModel1.AddElement("consumerName", "class", "consumerSource");
            Assert.IsNotNull(consumer);
            IDsiElement provider1 = dataModel1.AddElement("provider1Name", "class", "provider1Source");
            Assert.IsNotNull(provider1);
            IDsiElement provider2 = dataModel1.AddElement("provider2Name", "class", "provider2Source");
            Assert.IsNotNull(provider2);

            dataModel1.AddRelation(consumer.Name, provider1.Name, "relationType2", 2, null);
            dataModel1.AddRelation(consumer.Name, provider2.Name, "relationType3", 3, null);

            dataModel1.Save(filename, false, null);

            DsiModel dataModel2 = new DsiModel("Test", new List<string>(), Assembly.GetExecutingAssembly());
            dataModel2.Load(filename, null);

            Assert.AreEqual(dataModel1.CurrentElementCount, dataModel2.CurrentElementCount);
            List<IDsiElement> dataModel1Elements = dataModel1.GetElements().ToList();
            List<IDsiElement> dataModel2Elements = dataModel2.GetElements().ToList();
            for (int elementIndex = 0; elementIndex < dataModel1.CurrentElementCount; elementIndex++)
            {
                Assert.AreEqual(dataModel1Elements[elementIndex].Id, dataModel2Elements[elementIndex].Id);
                Assert.AreEqual(dataModel1Elements[elementIndex].Name, dataModel2Elements[elementIndex].Name);
                Assert.AreEqual(dataModel1Elements[elementIndex].Type, dataModel2Elements[elementIndex].Type);
                Assert.AreEqual(dataModel1Elements[elementIndex].Annotation,
                    dataModel2Elements[elementIndex].Annotation);
                Assert.AreEqual(dataModel1.GetRelationsOfConsumer(dataModel1Elements[elementIndex].Id).Count,
                    dataModel2.GetRelationsOfConsumer(dataModel1Elements[elementIndex].Id).Count);

                List<IDsiRelation> dataModel1Relations =
                    dataModel1.GetRelationsOfConsumer(dataModel1Elements[elementIndex].Id).ToList();
                List<IDsiRelation> dataModel2Relations =
                    dataModel2.GetRelationsOfConsumer(dataModel2Elements[elementIndex].Id).ToList();

                for (int relationIndex = 0;
                    relationIndex < dataModel1.GetRelationsOfConsumer(dataModel1Elements[elementIndex].Id).Count;
                    relationIndex++)
                {
                    Assert.AreEqual(dataModel1Relations[relationIndex].ConsumerId,
                        dataModel2Relations[relationIndex].ConsumerId);
                    Assert.AreEqual(dataModel1Relations[relationIndex].ProviderId,
                        dataModel2Relations[relationIndex].ProviderId);
                    Assert.AreEqual(dataModel1Relations[relationIndex].Type, dataModel2Relations[relationIndex].Type);
                    Assert.AreEqual(dataModel1Relations[relationIndex].Weight,
                        dataModel2Relations[relationIndex].Weight);
                }
            }
        }

        [TestMethod]
        public void IgnoredElementIsNotAddedToModel()
        {
            List<string> ignoredNames = new List<string>();
            ignoredNames.Add("^doNotAdd");
            DsiModel dataModel = new DsiModel("Test", ignoredNames, Assembly.GetExecutingAssembly());

            IDsiElement consumer1 = dataModel.AddElement("addThis", "class", "consumerSource");
            Assert.IsNotNull(consumer1);
            Assert.AreEqual(1, dataModel.CurrentElementCount);

            IDsiElement consumer2 = dataModel.AddElement("doNotAddThis", "class", "consumerSource");
            Assert.IsNull(consumer2);
            Assert.AreEqual(1, dataModel.CurrentElementCount);
        }
    }
}
