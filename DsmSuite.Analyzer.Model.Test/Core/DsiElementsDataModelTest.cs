using System.Linq;
using DsmSuite.Analyzer.Model.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.Analyzer.Model.Interface;

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
        public void Given_ModelIsFilled_When_FindByIdIsCalledWithKnownId_Then_ElementIsFound()
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
        public void Given_ModelIsFilled_When_FindByIdIsCalledWithUnknownId_Then_ElementIsNotFound()
        {
            DsiElementsDataModel model = new DsiElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name", "type", "source");

            IDsiElement foundElement = model.FindElementById(2);
            Assert.IsNull(foundElement);
        }

        [TestMethod]
        public void Given_ModelIsFilled_When_FindByIdIsCalledWithKnownName_Then_ElementIsFound()
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
        public void Given_ModelIsFilled_When_FindByIdIsCalledWithUnknownName_Then_ElementIsNotFound()
        {
            DsiElementsDataModel model = new DsiElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name", "type", "source");

            IDsiElement foundElement = model.FindElementByName("unknown");
            Assert.IsNull(foundElement);
        }

        [TestMethod]
        public void Given_AnElementIsInTheModel_When_RenameElementIsCalled_Then_ItCanBeFoundUnderThatName()
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
    }
}
