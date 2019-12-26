using System.Linq;
using DsmSuite.Analyzer.Model.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.Analyzer.Model.Interface;
using System.Collections.Generic;

namespace DsmSuite.Analyzer.Model.Test.Core
{
    [TestClass]
    public class DsiElementModelTest
    {
        [TestMethod]
        public void WhenModelIsConstructedThenItIsEmpty()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.TotalElementCount);
        }

        [TestMethod]
        public void GivenModelIsNotEmptyWhenClearIsCalledThenItIsEmpty()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name", "type", "source");
            Assert.AreEqual(1, model.TotalElementCount);

            model.Clear();

            Assert.AreEqual(0, model.TotalElementCount);
        }

        [TestMethod]
        public void GivenModelIsEmptyWhenAddElementIsCalledThenItsHasOneElement()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsiElement element = model.AddElement("name", "type", "source");
            Assert.IsNotNull(element);
            Assert.AreEqual(1, model.TotalElementCount);
        }

        [TestMethod]
        public void GivenAnElementIsInTheModelWhenAddElementIsCalledAgainForThatElementThenItStillHasOneElement()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsiElement element1 = model.AddElement("name", "type", "source");
            Assert.IsNotNull(element1);
            Assert.AreEqual(1, model.TotalElementCount);

            IDsiElement element2 = model.AddElement("name", "type", "source");
            Assert.IsNull(element2);
            Assert.AreEqual(1, model.TotalElementCount);
        }

        [TestMethod]
        public void GivenAnElementIsInTheModelWhenAddElementIsCalledForAnotherElementThenItHasTwoElement()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsiElement element1 = model.AddElement("name1", "type", "source");
            Assert.IsNotNull(element1);
            Assert.AreEqual(1, model.TotalElementCount);

            IDsiElement element2 = model.AddElement("name2", "type", "source");
            Assert.IsNotNull(element2);
            Assert.AreEqual(2, model.TotalElementCount);
        }

        [TestMethod]
        public void GivenModelIsEmptyWhenImportElementIsCalledThenItsHasOneElement()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name", "type", "source");
            Assert.AreEqual(1, model.TotalElementCount);
        }

        [TestMethod]
        public void GivenAnElementIsInTheModelWhenFindByIdIsCalledItsIdThenElementIsFound()
        {
            DsiElementModel model = new DsiElementModel();
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
        public void GivenAnElementIsInTheModelWhenFindByIdIsCalledWithAnotherIdThenElementIsNotFound()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name", "type", "source");

            IDsiElement foundElement = model.FindElementById(2);
            Assert.IsNull(foundElement);
        }

        [TestMethod]
        public void GivenAnElementIsInTheModelWhenRemoveElementIsCalledThenElementIsNotFoundAnymoreByItsId()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name", "type", "source");
            IDsiElement foundElementBefore = model.FindElementById(1);
            Assert.IsNotNull(foundElementBefore);

            model.RemoveElement(foundElementBefore);

            IDsiElement foundElementAfter = model.FindElementById(1);
            Assert.IsNull(foundElementAfter);
        }

        [TestMethod]
        public void GivenAnElementIsInTheModelWhenFindByIdIsCalledWithItsNameThenElementIsFound()
        {
            DsiElementModel model = new DsiElementModel();
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
        public void GivenAnElementIsInTheModelWhenFindByIdIsCalledWithAnotherNameThenElementIsNotFound()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name", "type", "source");

            IDsiElement foundElement = model.FindElementByName("unknown");
            Assert.IsNull(foundElement);
        }

        [TestMethod]
        public void GivenAnElementIsInTheModelWhenRemoveElementIsCalledThenElementIsNotFoundAnymoreByItName()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name", "type", "source");
            IDsiElement foundElementBefore = model.FindElementByName("name");
            Assert.IsNotNull(foundElementBefore);

            model.RemoveElement(foundElementBefore);

            IDsiElement foundElementAfter = model.FindElementByName("name");
            Assert.IsNull(foundElementAfter);
        }

        [TestMethod]
        public void WhenRenameElementIsCalledThenItCanBeFoundUnderThatName()
        {
            DsiElementModel model = new DsiElementModel();
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
        public void WhenAddElementIsCalledUsingTwoDifferentTypesThenTwoElementTypesAreFound()
        {
            DsiElementModel model = new DsiElementModel();
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
        public void GivenMultipleElementAreInTheModelWhenGetElementsIsCalledTheyAreAllReturned()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name1", "type1", "source1");
            model.ImportElement(2, "name2", "type2", "source2");
            model.ImportElement(3, "name3", "type3", "source3");

            List<IDsiElement> elements = model.GetExportedElements().ToList();
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
