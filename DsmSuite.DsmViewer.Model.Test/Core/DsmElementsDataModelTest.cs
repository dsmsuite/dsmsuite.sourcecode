using System.Linq;
using DsmSuite.DsmViewer.Model.Core;
using DsmSuite.DsmViewer.Model.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;

namespace DsmSuite.DsmViewer.Model.Test.Core
{
    [TestClass]
    public class DsmElementsDataModelTest
    {
        class Sequence : IElementSequence
        {
            private int _size;

            public Sequence(int size)
            {
                _size = size;
            }

            public int GetIndex(int currentIndex)
            {
                int newIndex = currentIndex + 1;
                if (newIndex >= _size)
                {
                    newIndex = 0;
                }
                return newIndex;
            }

            public int GetNumberOfElements()
            {
                return _size;
            }
        }

        [TestMethod]
        public void When_ModelIsConstructed_Then_ItIsEmpty()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);
        }

        [TestMethod]
        public void Given_ModelIsNotEmpty_When_ClearIsCalled_Then_ItIsEmpty()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name", "type", 0, false, null);
            Assert.AreEqual(1, model.TotalElementCount);

            model.Clear();

            Assert.AreEqual(0, model.TotalElementCount);
        }

        [TestMethod]
        public void Given_ModelIsEmpty_When_AddElementIsCalled_Then_ItsHasOneElement()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsmElement a = model.AddElement("a", "type", null);
            Assert.IsNotNull(a);

            Assert.AreEqual(1, model.TotalElementCount);
        }

        [TestMethod]
        public void Given_ModelIsEmpty_When_AddElementIsCalledTwice_Then_ItsHasTwoElements()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsmElement a = model.AddElement("a", "type", null);
            Assert.IsNotNull(a);
            Assert.AreEqual("a", a.Fullname);

            IDsmElement b = model.AddElement("b", "type", a.Id);
            Assert.IsNotNull(b);
            Assert.AreEqual("a.b", b.Fullname);

            Assert.AreEqual(2, model.TotalElementCount);
        }

        [TestMethod]
        public void Given_ModelIsEmpty_When_ImportElementIsCalled_Then_ItsHasOneElement()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name", "type", 0, false, null);
            Assert.AreEqual(1, model.TotalElementCount);
        }

        [TestMethod]
        public void Given_AnElementIsInTheModel_When_FindByIdIsCalledItsId_Then_ElementIsFound()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name", "type", 10, true, null);

            IDsmElement foundElement = model.FindElementById(1);
            Assert.IsNotNull(foundElement);
            Assert.AreEqual(1, foundElement.Id);
            Assert.AreEqual("name", foundElement.Name);
            Assert.AreEqual("type", foundElement.Type);
            Assert.AreEqual(10, foundElement.Order);
            Assert.AreEqual(true, foundElement.IsExpanded);
            Assert.IsNotNull(foundElement.Parent); // root element
            Assert.IsNull(foundElement.Parent.Parent);
        }

        [TestMethod]
        public void Given_AnElementIsInTheModel_When_FindByIdIsCalledWithAnotherId_Then_ElementIsNotFound()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            model.ImportElement(1, "name", "type", 10, true, null);

            IDsmElement foundElement = model.FindElementById(2);
            Assert.IsNull(foundElement);
        }

        [TestMethod]
        public void Given_AnElementIsInTheModel_When_FindByIdIsCalledWithItsName_Then_ElementIsFound()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsmElement a = model.ImportElement(1, "a", "type", 10, true, null);
            IDsmElement b = model.ImportElement(2, "b", "type", 11, true, a.Id);

            IDsmElement foundElement = model.FindElementByFullname("a.b");
            Assert.IsNotNull(foundElement);
            Assert.AreEqual(2, foundElement.Id);
            Assert.AreEqual("b", foundElement.Name);
            Assert.AreEqual("a.b", foundElement.Fullname);
            Assert.AreEqual("type", foundElement.Type);
            Assert.AreEqual(11, foundElement.Order);
            Assert.AreEqual(true, foundElement.IsExpanded);
            Assert.AreEqual(a, foundElement.Parent);
        }

        [TestMethod]
        public void Given_AnElementIsInTheModel_When_FindByIdIsCalledWithAnotherName_Then_ElementIsNotFound()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsmElement a = model.ImportElement(1, "a", "type", 10, true, null);
            IDsmElement b = model.ImportElement(2, "b", "type", 11, true, a.Id);

            IDsmElement foundElement = model.FindElementByFullname("a.c");
            Assert.IsNull(foundElement);
        }

        [TestMethod]
        public void When_EditElementIsCalledToChangeName_Then_ItCanBeFoundUnderThatName()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsmElement a = model.ImportElement(1, "a", "type", 10, true, null);
            IDsmElement b = model.ImportElement(2, "b", "type", 11, true, a.Id);

            IDsmElement foundElementBefore = model.FindElementByFullname("a.c");
            Assert.IsNull(foundElementBefore);

            model.EditElement(b, "c", b.Type);

            IDsmElement foundElementAfter = model.FindElementByFullname("a.c");
            Assert.IsNotNull(foundElementAfter);
        }

        [TestMethod]
        public void When_EditElementIsCalledToChangeType_Then_TypeIsChanged()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsmElement a = model.ImportElement(1, "a", "type", 10, true, null);
            IDsmElement b = model.ImportElement(2, "b", "type", 11, true, a.Id);

            IDsmElement foundElementBefore = model.FindElementByFullname("a.b");
            Assert.IsNotNull(foundElementBefore);
            Assert.AreEqual("type", foundElementBefore.Type);

            model.EditElement(b, b.Name, "type1");

            IDsmElement foundElementAfter = model.FindElementByFullname("a.b");
            Assert.IsNotNull(foundElementAfter);
            Assert.AreEqual("type1", foundElementAfter.Type);
        }

        [TestMethod]
        public void When_ChangeElementParentIsCalled_Then_ItCanBeFoundAtTheNewLocation()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsmElement a = model.ImportElement(1, "a", "type", 10, true, null);
            IDsmElement b = model.ImportElement(2, "b", "type", 11, true, a.Id);

            IDsmElement c = model.ImportElement(3, "c", "type", 12, true, null);

            IDsmElement foundElementBefore = model.FindElementByFullname("a.b");
            Assert.IsNotNull(foundElementBefore);
            Assert.AreEqual(a, foundElementBefore.Parent);

            model.ChangeElementParent(b, c);

            IDsmElement foundElementAfter = model.FindElementByFullname("c.b");
            Assert.IsNotNull(foundElementAfter);
            Assert.AreEqual(c, foundElementAfter.Parent);
        }

        [TestMethod]
        public void Given_AnElementIsInTheModel_When_SearchIsCalledWithTextPartOfItsName_Then_ElementIsFound()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsmElement a = model.ImportElement(1, "a", "type", 10, true, null);
            IDsmElement b = model.ImportElement(2, "b", "type", 11, true, a.Id);

            IDsmElement foundElement = model.SearchElements(".b").ToList().FirstOrDefault();
            Assert.IsNotNull(foundElement);
            Assert.AreEqual(2, foundElement.Id);
            Assert.AreEqual("b", foundElement.Name);
            Assert.AreEqual("a.b", foundElement.Fullname);
            Assert.AreEqual("type", foundElement.Type);
            Assert.AreEqual(11, foundElement.Order);
            Assert.AreEqual(true, foundElement.IsExpanded);
            Assert.AreEqual(a, foundElement.Parent);
        }

        [TestMethod]
        public void Given_AnElementIsInTheModel_When_SearchIsCalledWithTextNotPartOfItsName_Then_ElementIsFound()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsmElement a = model.ImportElement(1, "a", "type", 10, true, null);
            IDsmElement b = model.ImportElement(2, "b", "type", 11, true, a.Id);

            IDsmElement foundElement = model.SearchElements(".c").ToList().FirstOrDefault();
            Assert.IsNull(foundElement);
        }


 
        [TestMethod]
        public void Given_AnElementIsInTheModel_When_RemoveElementIsCalled_Then_ElementAndItsChildrenAreRemoved()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsmElement a = model.AddElement("a", "", null);
            Assert.AreEqual(1, a.Id);
            IDsmElement a1 = model.AddElement("a1", "eta", a.Id);
            Assert.AreEqual(2, a1.Id);
            IDsmElement a2 = model.AddElement("a2", "eta", a.Id);
            Assert.AreEqual(3, a2.Id);

            IDsmElement b = model.AddElement("b", "", null);
            Assert.AreEqual(4, b.Id);
            IDsmElement b1 = model.AddElement("b1", "etb", b.Id);
            Assert.AreEqual(5, b1.Id);

            Assert.AreEqual(5, model.TotalElementCount);

            List<IDsmElement> rootElementsBefore = model.GetRootElements().OrderBy(x => x.Id).ToList();
            Assert.AreEqual(2, rootElementsBefore.Count);

            Assert.AreEqual(a, rootElementsBefore[0]);
            Assert.AreEqual(2, rootElementsBefore[0].Children.Count);
            Assert.AreEqual(a1, rootElementsBefore[0].Children[0]);
            Assert.AreEqual(a2, rootElementsBefore[0].Children[1]);

            Assert.AreEqual(b, rootElementsBefore[1]);
            Assert.AreEqual(1, rootElementsBefore[1].Children.Count);
            Assert.AreEqual(b1, rootElementsBefore[1].Children[0]);

            model.RemoveElement(a);

            Assert.AreEqual(2, model.TotalElementCount);

            List<IDsmElement> rootElementsAfter = model.GetRootElements().OrderBy(x => x.Id).ToList();
            Assert.AreEqual(1, rootElementsAfter.Count);

            Assert.AreEqual(b, rootElementsAfter[0]);
            Assert.AreEqual(1, rootElementsAfter[0].Children.Count);
            Assert.AreEqual(b1, rootElementsAfter[0].Children[0]);
        }

        [TestMethod]
        public void Given_AnElementIsInTheModel_When_UnremoveElementIsCalled_Then_ElementAndItsChildrenAreRestored()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsmElement a = model.AddElement("a", "", null);
            Assert.AreEqual(1, a.Id);
            IDsmElement a1 = model.AddElement("a1", "eta", a.Id);
            Assert.AreEqual(2, a1.Id);
            IDsmElement a2 = model.AddElement("a2", "eta", a.Id);
            Assert.AreEqual(3, a2.Id);

            IDsmElement b = model.AddElement("b", "", null);
            Assert.AreEqual(4, b.Id);
            IDsmElement b1 = model.AddElement("b1", "etb", b.Id);
            Assert.AreEqual(5, b1.Id);

            Assert.AreEqual(5, model.TotalElementCount);

            model.RemoveElement(a);

            Assert.AreEqual(2, model.TotalElementCount);

            List<IDsmElement> rootElementsBefore = model.GetRootElements().OrderBy(x => x.Id).ToList();
            Assert.AreEqual(1, rootElementsBefore.Count);

            Assert.AreEqual(b, rootElementsBefore[0]);
            Assert.AreEqual(1, rootElementsBefore[0].Children.Count);
            Assert.AreEqual(b1, rootElementsBefore[0].Children[0]);

            model.UnremoveElement(a);

            Assert.AreEqual(5, model.TotalElementCount);

            List<IDsmElement> rootElementsAfter = model.GetRootElements().OrderBy(x => x.Id).ToList();
            Assert.AreEqual(2, rootElementsAfter.Count);

            Assert.AreEqual(a, rootElementsAfter[0]);
            Assert.AreEqual(2, rootElementsAfter[0].Children.Count);
            Assert.AreEqual(a1, rootElementsAfter[0].Children[0]);
            Assert.AreEqual(a2, rootElementsAfter[0].Children[1]);

            Assert.AreEqual(b, rootElementsAfter[1]);
            Assert.AreEqual(1, rootElementsAfter[1].Children.Count);
            Assert.AreEqual(b1, rootElementsAfter[1].Children[0]);
        }

        [TestMethod]
        public void Given_AnElementIsInTheModel_When_RemoveElementIsCalledOnLastChild_Then_ParentIsCollapsed()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsmElement a = model.AddElement("a", "", null);
            Assert.AreEqual(1, a.Id);
            IDsmElement a1 = model.AddElement("a1", "eta", a.Id);
            Assert.AreEqual(2, a1.Id);
            Assert.AreEqual(2, model.TotalElementCount);
            Assert.AreEqual(1, a.Children.Count);

            a.IsExpanded = true;

            model.RemoveElement(a1);
            Assert.AreEqual(1, model.TotalElementCount);
            Assert.AreEqual(0, a.Children.Count);

            Assert.IsFalse(a.IsExpanded);
        }

        [TestMethod]
        public void Given_MultipleElementAreInTheModel_When_GetElementsIsCalled_TheyAreAllReturned()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsmElement a = model.AddElement("a", "", null);
            Assert.AreEqual(1, a.Id);
            IDsmElement a1 = model.AddElement("a1", "eta", a.Id);
            Assert.AreEqual(2, a1.Id);

            IDsmElement b = model.AddElement("b", "", null);
            Assert.AreEqual(3, b.Id);
            IDsmElement b1 = model.AddElement("b1", "etb", b.Id);
            Assert.AreEqual(4, b1.Id);
            IDsmElement b2 = model.AddElement("b2", "etb", b.Id);
            Assert.AreEqual(5, b2.Id);

            IDsmElement c = model.AddElement("c", "", null);
            Assert.AreEqual(6, c.Id);
            IDsmElement c1 = model.AddElement("c1", "etc", c.Id);
            Assert.AreEqual(7, c1.Id);
            IDsmElement c2 = model.AddElement("c2", "etc", c.Id);
            Assert.AreEqual(8, c2.Id);
            IDsmElement c3 = model.AddElement("c3", "etc", c.Id);
            Assert.AreEqual(9, c3.Id);

            List<IDsmElement> rootElements = model.GetRootElements().OrderBy(x => x.Id).ToList();
            Assert.AreEqual(3, rootElements.Count);

            Assert.AreEqual(a, rootElements[0]);
            Assert.AreEqual(1, rootElements[0].Children.Count);
            Assert.AreEqual(a1, rootElements[0].Children[0]);

            Assert.AreEqual(b, rootElements[1]);
            Assert.AreEqual(2, rootElements[1].Children.Count);
            Assert.AreEqual(b1, rootElements[1].Children[0]);
            Assert.AreEqual(b2, rootElements[1].Children[1]);

            Assert.AreEqual(c, rootElements[2]);
            Assert.AreEqual(3, rootElements[2].Children.Count);
            Assert.AreEqual(c1, rootElements[2].Children[0]);
            Assert.AreEqual(c2, rootElements[2].Children[1]);
            Assert.AreEqual(c3, rootElements[2].Children[2]);
        }

        [TestMethod]
        public void Given_MultipleElementAreInTheModel_When_SwapElementIsCalled_OrderIsChanged()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsmElement a = model.AddElement("a", "", null);
            Assert.AreEqual(1, a.Id);
            IDsmElement a1 = model.AddElement("a1", "eta", a.Id);
            Assert.AreEqual(2, a1.Id);
            IDsmElement a2 = model.AddElement("a2", "eta", a.Id);
            Assert.AreEqual(3, a2.Id);
            IDsmElement a3 = model.AddElement("a3", "eta", a.Id);
            Assert.AreEqual(4, a3.Id);

            List<IDsmElement> rootElementsBefore = model.GetRootElements().ToList();
            Assert.AreEqual(1, rootElementsBefore.Count);

            Assert.AreEqual(a, rootElementsBefore[0]);
            Assert.AreEqual(3, rootElementsBefore[0].Children.Count);
            Assert.AreEqual(a1, rootElementsBefore[0].Children[0]);
            Assert.AreEqual(a2, rootElementsBefore[0].Children[1]);
            Assert.AreEqual(a3, rootElementsBefore[0].Children[2]);

            model.Swap(a1, a2);

            List<IDsmElement> rootElementsAfter = model.GetRootElements().ToList();
            Assert.AreEqual(1, rootElementsAfter.Count);

            Assert.AreEqual(a, rootElementsAfter[0]);
            Assert.AreEqual(3, rootElementsAfter[0].Children.Count);
            Assert.AreEqual(a2, rootElementsAfter[0].Children[0]);
            Assert.AreEqual(a1, rootElementsAfter[0].Children[1]);
            Assert.AreEqual(a3, rootElementsAfter[0].Children[2]);
        }

        [TestMethod]
        public void Given_MultipleElementAreInTheModel_When_ReorderElementIsCalled_OrderIsChanged()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsmElement a = model.AddElement("a", "", null);
            Assert.AreEqual(1, a.Id);
            IDsmElement a1 = model.AddElement("a1", "eta", a.Id);
            Assert.AreEqual(2, a1.Id);
            IDsmElement a2 = model.AddElement("a2", "eta", a.Id);
            Assert.AreEqual(3, a2.Id);
            IDsmElement a3 = model.AddElement("a3", "eta", a.Id);
            Assert.AreEqual(4, a3.Id);
            IDsmElement a4 = model.AddElement("a4", "eta", a.Id);
            Assert.AreEqual(5, a4.Id);

            List<IDsmElement> rootElementsBefore = model.GetRootElements().ToList();
            Assert.AreEqual(1, rootElementsBefore.Count);

            Assert.AreEqual(a, rootElementsBefore[0]);
            Assert.AreEqual(4, rootElementsBefore[0].Children.Count);
            Assert.AreEqual(a1, rootElementsBefore[0].Children[0]);
            Assert.AreEqual(a2, rootElementsBefore[0].Children[1]);
            Assert.AreEqual(a3, rootElementsBefore[0].Children[2]);
            Assert.AreEqual(a4, rootElementsBefore[0].Children[3]);

            Sequence sequence = new Sequence(rootElementsBefore[0].Children.Count);
            model.ReorderChildren(a, sequence);

            List<IDsmElement> rootElementsAfter = model.GetRootElements().ToList();
            Assert.AreEqual(1, rootElementsAfter.Count);

            Assert.AreEqual(a, rootElementsAfter[0]);
            Assert.AreEqual(4, rootElementsAfter[0].Children.Count);
            Assert.AreEqual(a2, rootElementsAfter[0].Children[0]);
            Assert.AreEqual(a3, rootElementsAfter[0].Children[1]);
            Assert.AreEqual(a4, rootElementsAfter[0].Children[2]);
            Assert.AreEqual(a1, rootElementsAfter[0].Children[3]);
        }

        [TestMethod]
        public void Given_MultipleElementAreInTheModel_When_AssignElementOrderIsCalled_Then_ElementsHaveOrderSet()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsmElement a = model.AddElement("a", "", null);
            Assert.AreEqual(1, a.Id);
            IDsmElement a1 = model.AddElement("a1", "eta", a.Id);
            Assert.AreEqual(2, a1.Id);

            IDsmElement b = model.AddElement("b", "", null);
            Assert.AreEqual(3, b.Id);
            IDsmElement b1 = model.AddElement("b1", "etb", b.Id);
            Assert.AreEqual(4, b1.Id);
            IDsmElement b2 = model.AddElement("b2", "etb", b.Id);
            Assert.AreEqual(5, b2.Id);

            IDsmElement c = model.AddElement("c", "", null);
            Assert.AreEqual(6, c.Id);
            IDsmElement c1 = model.AddElement("c1", "etc", c.Id);
            Assert.AreEqual(7, c1.Id);
            IDsmElement c2 = model.AddElement("c2", "etc", c.Id);
            Assert.AreEqual(8, c2.Id);
            IDsmElement c3 = model.AddElement("c3", "etc", c.Id);
            Assert.AreEqual(9, c3.Id);

            Assert.AreEqual(0, a.Order);
            Assert.AreEqual(0, a1.Order);
            Assert.AreEqual(0, b.Order);
            Assert.AreEqual(0, b1.Order);
            Assert.AreEqual(0, b2.Order);
            Assert.AreEqual(0, c.Order);
            Assert.AreEqual(0, c1.Order);
            Assert.AreEqual(0, c2.Order);
            Assert.AreEqual(0, c3.Order);

            model.AssignElementOrder();

            Assert.AreEqual(1, a.Order);
            Assert.AreEqual(2, a1.Order);
            Assert.AreEqual(3, b.Order);
            Assert.AreEqual(4, b1.Order);
            Assert.AreEqual(5, b2.Order);
            Assert.AreEqual(6, c.Order);
            Assert.AreEqual(7, c1.Order);
            Assert.AreEqual(8, c2.Order);
            Assert.AreEqual(9, c3.Order);
        }

        [TestMethod]
        public void Given_MultipleElementAreInTheModel_When_NextSiblingIsCalledOnOtherElementThanTheLast_Then_NextElementIsReturned()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsmElement a = model.AddElement("a", "", null);
            Assert.AreEqual(1, a.Id);
            IDsmElement a1 = model.AddElement("a1", "eta", a.Id);
            Assert.AreEqual(2, a1.Id);
            IDsmElement a2 = model.AddElement("a2", "eta", a.Id);
            Assert.AreEqual(3, a2.Id);
            IDsmElement a3 = model.AddElement("a3", "eta", a.Id);
            Assert.AreEqual(4, a3.Id);

            Assert.AreEqual(a2, model.NextSibling(a1));
            Assert.AreEqual(a3, model.NextSibling(a2));
            Assert.AreEqual(null, model.NextSibling(a3));
        }

        [TestMethod]
        public void Given_MultipleElementAreInTheModel_When_PreviousSiblingIsCalledOnOtherElementThanTheFirst_Then_PreviousElementIsReturned()
        {
            DsmElementsDataModel model = new DsmElementsDataModel();
            Assert.AreEqual(0, model.TotalElementCount);

            IDsmElement a = model.AddElement("a", "", null);
            Assert.AreEqual(1, a.Id);
            IDsmElement a1 = model.AddElement("a1", "eta", a.Id);
            Assert.AreEqual(2, a1.Id);
            IDsmElement a2 = model.AddElement("a2", "eta", a.Id);
            Assert.AreEqual(3, a2.Id);
            IDsmElement a3 = model.AddElement("a3", "eta", a.Id);
            Assert.AreEqual(4, a3.Id);

            Assert.AreEqual(null, model.PreviousSibling(a1));
            Assert.AreEqual(a1, model.PreviousSibling(a2));
            Assert.AreEqual(a2, model.PreviousSibling(a3));
        }
    }
}
