using System.Linq;
using DsmSuite.Analyzer.Model.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.Analyzer.Model.Interface;
using System.Collections.Generic;

namespace DsmSuite.Analyzer.Model.Test.Core
{
    [TestClass]
    public class DsiElementsDataModelTest
    {
        [TestMethod]
        public void When_ModelIsConstructed_Then_ItIsEmpty()
        {
            DsiElementsDataModel model = new DsiElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);
        }

        [TestMethod]
        public void Given_ModelIsNotEmpty_When_ClearIsCalled_Then_ItIsEmpty()
        {
            DsiElementsDataModel model = new DsiElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name", "type", "source");
            Assert.AreEqual(1, model.TotalElementCount);

            model.Clear();

            Assert.AreEqual(0, model.TotalElementCount);
        }

        [TestMethod]
        public void Given_ModelIsEmpty_When_AddElementIsCalled_Then_ItsHasOneElement()
        {
            DsiElementsDataModel model = new DsiElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsiElement element = model.AddElement("name", "type", "source");
            Assert.IsNotNull(element);
            Assert.AreEqual(1, model.TotalElementCount);
        }

        [TestMethod]
        public void Given_AnElementIsInTheModel_When_AddElementIsCalledAgainForThatElement_Then_ItStillHasOneElement()
        {
            DsiElementsDataModel model = new DsiElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsiElement element1 = model.AddElement("name", "type", "source");
            Assert.IsNotNull(element1);
            Assert.AreEqual(1, model.TotalElementCount);

            IDsiElement element2 = model.AddElement("name", "type", "source");
            Assert.IsNull(element2);
            Assert.AreEqual(1, model.TotalElementCount);
        }

        [TestMethod]
        public void Given_AnElementIsInTheModel_When_AddElementIsCalledForAnotherElement_Then_ItHasTwoElement()
        {
            DsiElementsDataModel model = new DsiElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsiElement element1 = model.AddElement("name1", "type", "source");
            Assert.IsNotNull(element1);
            Assert.AreEqual(1, model.TotalElementCount);

            IDsiElement element2 = model.AddElement("name2", "type", "source");
            Assert.IsNotNull(element2);
            Assert.AreEqual(2, model.TotalElementCount);
        }

        [TestMethod]
        public void Given_ModelIsEmpty_When_ImportElementIsCalled_Then_ItsHasOneElement()
        {
            DsiElementsDataModel model = new DsiElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name", "type", "source");
            Assert.AreEqual(1, model.TotalElementCount);
        }

        [TestMethod]
        public void Given_AnElementIsInTheModel_When_FindByIdIsCalledItsId_Then_ElementIsFound()
        {
            DsiElementsDataModel model = new DsiElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name", "type", "source");

            IDsiElement foundElement = model.FindElementById(1);
            Assert.IsNotNull(foundElement);
            Assert.AreEqual(1, foundElement.Id);
            Assert.AreEqual("name", foundElement.Name);
            Assert.AreEqual("type", foundElement.Type);
            Assert.AreEqual("source", foundElement.Source);
        }

        [TestMethod]
        public void Given_AnElementIsInTheModel_When_FindByIdIsCalledWithAnotherId_Then_ElementIsNotFound()
        {
            DsiElementsDataModel model = new DsiElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name", "type", "source");

            IDsiElement foundElement = model.FindElementById(2);
            Assert.IsNull(foundElement);
        }

        [TestMethod]
        public void Given_AnElementIsInTheModel_When_RemoveElementIsCalled_Then_ElementIsNotFoundAnymoreByItsId()
        {
            DsiElementsDataModel model = new DsiElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name", "type", "source");
            IDsiElement foundElementBefore = model.FindElementById(1);
            Assert.IsNotNull(foundElementBefore);

            model.RemoveElement(foundElementBefore);

            IDsiElement foundElementAfter = model.FindElementById(1);
            Assert.IsNull(foundElementAfter);
        }

        [TestMethod]
        public void Given_AnElementIsInTheModel_When_FindByIdIsCalledWithItsName_Then_ElementIsFound()
        {
            DsiElementsDataModel model = new DsiElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name", "type", "source");

            IDsiElement foundElement = model.FindElementByName("name");
            Assert.IsNotNull(foundElement);
            Assert.AreEqual(1, foundElement.Id);
            Assert.AreEqual("name", foundElement.Name);
            Assert.AreEqual("type", foundElement.Type);
            Assert.AreEqual("source", foundElement.Source);
        }

        [TestMethod]
        public void Given_AnElementIsInTheModel_When_FindByIdIsCalledWithAnotherName_Then_ElementIsNotFound()
        {
            DsiElementsDataModel model = new DsiElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name", "type", "source");

            IDsiElement foundElement = model.FindElementByName("unknown");
            Assert.IsNull(foundElement);
        }

        [TestMethod]
        public void Given_AnElementIsInTheModel_When_RemoveElementIsCalled_Then_ElementIsNotFoundAnymoreByItName()
        {
            DsiElementsDataModel model = new DsiElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name", "type", "source");
            IDsiElement foundElementBefore = model.FindElementByName("name");
            Assert.IsNotNull(foundElementBefore);

            model.RemoveElement(foundElementBefore);

            IDsiElement foundElementAfter = model.FindElementByName("name");
            Assert.IsNull(foundElementAfter);
        }

        [TestMethod]
        public void When_RenameElementIsCalled_Then_ItCanBeFoundUnderThatName()
        {
            DsiElementsDataModel model = new DsiElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsiElement element = model.AddElement("name", "type", "source");
            Assert.IsNotNull(element);
            Assert.AreEqual(1, model.TotalElementCount);

            model.RenameElement(element, "newname");
            Assert.AreEqual(1, model.TotalElementCount);

            IDsiElement foundElement = model.FindElementByName("newname");
            Assert.IsNotNull(foundElement);
            Assert.AreEqual(1, foundElement.Id);
            Assert.AreEqual("newname", foundElement.Name);
            Assert.AreEqual("type", foundElement.Type);
            Assert.AreEqual("source", foundElement.Source);
        }

        [TestMethod]
        public void When_AddElementIsCalledUsingTwoDifferentTypes_Then_TwoElementTypesAreFound()
        {
            DsiElementsDataModel model = new DsiElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsiElement element1 = model.AddElement("name1", "type1", "source1");
            Assert.IsNotNull(element1);
            Assert.AreEqual(1, model.TotalElementCount);

            IDsiElement element2 = model.AddElement("name2", "type2", "source2");
            Assert.IsNotNull(element2);
            Assert.AreEqual(2, model.TotalElementCount);

            IDsiElement element3 = model.AddElement("name3", "type2", "source3");
            Assert.IsNotNull(element3);
            Assert.AreEqual(3, model.TotalElementCount);

            List<string> elementTypes = model.GetElementTypes().ToList();
            Assert.AreEqual(2, elementTypes.Count);
            Assert.AreEqual("type1", elementTypes[0]);
            Assert.AreEqual("type2", elementTypes[1]);

            Assert.AreEqual(1, model.GetElementTypeCount("type1"));
            Assert.AreEqual(2, model.GetElementTypeCount("type2"));
        }

        [TestMethod]
        public void Given_MultipleElementAreInTheModel_When_GetElementsIsCalled_TheyAreAllReturned()
        {
            DsiElementsDataModel model = new DsiElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name1", "type1", "source1");
            model.ImportElement(2, "name2", "type2", "source2");
            model.ImportElement(3, "name3", "type3", "source3");

            List<IDsiElement> elements = model.GetElements().ToList();
            Assert.AreEqual(3, elements.Count);

            Assert.AreEqual(1, elements[0].Id);
            Assert.AreEqual("name1", elements[0].Name);
            Assert.AreEqual("type1", elements[0].Type);
            Assert.AreEqual("source1", elements[0].Source);

            Assert.AreEqual(2, elements[1].Id);
            Assert.AreEqual("name2", elements[1].Name);
            Assert.AreEqual("type2", elements[1].Type);
            Assert.AreEqual("source2", elements[1].Source);

            Assert.AreEqual(3, elements[2].Id);
            Assert.AreEqual("name3", elements[2].Name);
            Assert.AreEqual("type3", elements[2].Type);
            Assert.AreEqual("source3", elements[2].Source);
        }
    }
}
